#!/usr/bin/env bash
set -euo pipefail
umask 077

root="${M1_DIAGNOSTICS_DIR:-${RUNNER_TEMP:?}/m1-diagnostics}"
public="$root/public"
results="$root/results"
private="$root/private-raw-dumps"
mkdir -p "$public" "$results" "$private"

# Environment overrides are for short local harness checks, not application settings.
deadline="${M1_DEADLINE_SECONDS:-2700}"
interval="${M1_RESOURCE_INTERVAL_SECONDS:-15}"
stack_interval="${M1_STACK_INTERVAL_SECONDS:-60}"
stall_threshold="${M1_STALL_SECONDS:-120}"
for value in "$deadline" "$interval" "$stack_interval" "$stall_threshold"; do
  [[ "$value" =~ ^[1-9][0-9]*$ ]] || { echo 'Diagnostic intervals must be positive integers.' >&2; exit 2; }
done

test_pid=''
counter_pids=()
declare -A observed_hosts=()
dump_taken=false
started=$SECONDS
last_stack=-99999
last_dump=-99999

stop_tests() {
  if [[ -n "$test_pid" ]] && kill -0 -- "-$test_pid" 2>/dev/null; then
    kill -TERM -- "-$test_pid" 2>/dev/null || true
    sleep 2
    kill -KILL -- "-$test_pid" 2>/dev/null || true
  fi
}

stop_counters() {
  local pid attempt alive
  for pid in "${counter_pids[@]}"; do kill -INT "$pid" 2>/dev/null || true; done
  for ((attempt=0; attempt<5; attempt++)); do
    alive=false
    for pid in "${counter_pids[@]}"; do
      if kill -0 "$pid" 2>/dev/null; then alive=true; fi
    done
    [[ "$alive" == true ]] || break
    sleep 1
  done
  for pid in "${counter_pids[@]}"; do
    kill -TERM "$pid" 2>/dev/null || true
  done
  if ((${#counter_pids[@]})); then sleep 1; fi
  for pid in "${counter_pids[@]}"; do
    kill -KILL "$pid" 2>/dev/null || true
    wait "$pid" 2>/dev/null || true
  done
  counter_pids=()
}

cleanup() {
  stop_tests
  stop_counters
}
trap cleanup EXIT
trap 'exit 130' INT
trap 'exit 143' TERM

testhosts() {
  local pid parent depth
  for pid in $(pgrep -f 'testhost\.dll' || true); do
    parent="$pid"
    for ((depth=0; depth<32; depth++)); do
      parent=$(ps -o ppid= -p "$parent" 2>/dev/null | tr -d ' ' || true)
      [[ "$parent" =~ ^[0-9]+$ ]] || break
      if [[ "$parent" == "$test_pid" ]]; then
        echo "$pid"
        break
      fi
      (( parent > 1 )) || break
    done
  done
}

resources() {
  local pid
  {
    printf '\nUTC=%s elapsed_seconds=%s\n' "$(date -u +%FT%TZ)" "$((SECONDS-started))"
    grep -E 'MemAvailable|SwapFree' /proc/meminfo || true
    for file in memory.current memory.max memory.events cpu.stat; do
      if [[ -r "/sys/fs/cgroup/$file" ]]; then
        printf 'cgroup_%s=' "$file"
        tr '\n' ' ' < "/sys/fs/cgroup/$file"
        echo
      fi
    done
    # No process arguments, environment variables, token values or object contents.
    ps -eo pid,ppid,pcpu,pmem,rss,nlwp,etimes,comm
    for pid in "$@"; do
      printf 'testhost_pid=%s\n' "$pid"
      grep -E 'VmRSS|VmHWM|VmSize|Threads' "/proc/$pid/status" 2>/dev/null || true
      printf 'fds='
      find "/proc/$pid/fd" -maxdepth 1 -type l 2>/dev/null | wc -l || true
      printf 'inotify_instances='
      find "/proc/$pid/fd" -maxdepth 1 -lname 'anon_inode:inotify' 2>/dev/null | wc -l || true
    done
  } >> "$public/resources.txt" 2>&1
}

stacks() {
  local pid
  for pid in "$@"; do
    timeout 15s dotnet-stack report --process-id "$pid" \
      > "$public/stacks-$((SECONDS-started))-$pid.txt" 2>&1 || true
  done
}

heap() {
  local pid
  # One bounded collection per run. Raw memory never enters uploaded artifact paths.
  for pid in "$@"; do
    timeout 45s dotnet-dump collect --process-id "$pid" --type Heap \
      --output "$private/testhost-$pid.dmp" > "$public/dump-collection-$pid.txt" 2>&1 || true
    dump_taken=true
    break
  done
}

test_command="dotnet test VictoryCenter/VictoryCenter.IntegrationTests/VictoryCenter.IntegrationTests.csproj --configuration Release --no-restore --logger \"trx;LogFileName=integration.trx\" --logger \"console;verbosity=normal\" --results-directory \"$results\" --blame-hang-timeout 5m --blame-hang-dump-type none"
setsid dotnet-coverage collect "$test_command" -f xml -o coverage2.xml \
  > >(tee "$public/integration.log") 2>&1 &
test_pid=$!
result=0

while kill -0 "$test_pid" 2>/dev/null; do
  mapfile -t hosts < <(testhosts)
  resources "${hosts[@]}"
  for pid in "${hosts[@]}"; do
    if [[ -z "${observed_hosts[$pid]:-}" ]]; then
      observed_hosts[$pid]=true
      printf '%s testhost_pid=%s\n' "$(date -u +%FT%TZ)" "$pid" >> "$public/testhosts.txt"
      dotnet-counters collect --process-id "$pid" --counters System.Runtime \
        --refresh-interval 15 --format csv --output "$public/runtime-$pid.csv" \
        > "$public/counters-$pid.txt" 2>&1 &
      counter_pids+=("$!")
    fi
  done

  elapsed=$((SECONDS-started))
  if (( elapsed-last_stack >= stack_interval )); then
    stacks "${hosts[@]}"
    last_stack=$elapsed
  fi

  progress=$(stat -c %Y "$public/integration.log" 2>/dev/null || date +%s)
  idle=$(( $(date +%s)-progress ))
  if (( elapsed >= deadline )); then
    echo 'Diagnostic suite budget reached; this alone is not proof of a deadlock.' | tee "$public/deadline.txt"
    stacks "${hosts[@]}"
    if [[ "$dump_taken" == false ]]; then heap "${hosts[@]}"; fi
    stop_tests
    result=124
    break
  elif (( idle >= stall_threshold && elapsed-last_dump >= stall_threshold )) && [[ "$dump_taken" == false ]]; then
    printf 'No console progress for %s seconds; taking a diagnostic snapshot, not declaring a deadlock.\n' "$idle" | tee -a "$public/stall.txt"
    heap "${hosts[@]}"
    last_dump=$elapsed
  fi
  sleep "$interval"
done

if [[ "$result" == 0 ]]; then wait "$test_pid" || result=$?; else wait "$test_pid" || true; fi
stop_counters

shopt -s nullglob
for dump in "$private"/*.dmp; do
  name=$(basename "$dump" .dmp)
  timeout 60s dotnet-dump analyze "$dump" --command 'clrthreads' --command 'clrstack -all' \
    --command 'dumpheap -stat' --command exit > "$public/$name-analysis.txt" 2>&1 || true
  rm -f -- "$dump"
done
printf 'exit_code=%s\nelapsed_seconds=%s\n' "$result" "$((SECONDS-started))" > "$public/result.txt"
exit "$result"
