# Temporary integration-timeout investigation

This diagnostic branch is based on ceb0deffdbdde202795f743a25af0a35919820b7,
the .NET 9 source commit used by run 44348792931. An earlier attempt exceeded the
60-minute build-job limit; a rerun of the same source completed successfully.
The original runner image cannot be assumed identical on a later ubuntu-latest run.

Do not merge this diagnostic workflow into release as a timeout fix.
Application code, fixtures, test cases, dependency lock files and runner label are
unchanged. Coverage and Sonar analysis remain enabled. A push to the feature branch
starts the existing build workflow; no pull request or deployment is needed.

## Instrumentation

- The original integration command runs under the same coverage collector.
- TRX, normal-verbosity console progress and VSTest blame sequencing are enabled.
- Five minutes without completion of a test triggers VSTest hang detection; this
  can also reflect an unusually slow test, not necessarily a deadlock.
- Every 15 seconds: OS/process memory, thread counts, CPU, cgroup statistics and
  testhost file-descriptor/inotify counts, without command lines or environments.
- System.Runtime counters per testhost provide managed-runtime measurements.
- Managed thread stacks are sampled every minute for comparison over time.
- A 120-second console-progress gap permits one bounded local heap dump collection.
  Console silence is a heuristic, not proof that a test is stuck.
- The integration wrapper stops at a 45-minute diagnostic budget (exit 124).
  That is an explicit diagnostic stop, not an assertion that the test failed.
- The integration step has a 55-minute limit, and this branch's job has a 75-minute
  limit solely to leave time for analysis and upload. Increasing that limit is not
  the proposed solution to the original timeout.
- Normal test failures retain their original nonzero exit code.

## Artifacts and safety

The always-run upload step retains text diagnostics and test results for three days.
Artifact name: integration-diagnostics-RUN_ID-ATTEMPT.

Raw dumps may contain process credentials, tokens or personal data. They are created
only in the runner's private-raw-dumps directory, never in the artifact paths.
After the test process exits, bounded analysis writes managed stacks and type-count
statistics (no locals, string values or environment) to the public directory, then
removes the dump. An interrupted runner may lose this analysis; unprocessed memory
still must not be published. Do not manually upload raw dumps to this public repo.

The upload includes only public/ and results/, with an additional .dmp exclusion.
An overall runner cancellation or infrastructure termination can still prevent upload;
always() is not a guarantee that a killed runner can execute another step.

## Reading a run

1. environment.txt: actual SHA, attempt, runner image version, SDK/runtime and RAM.
2. integration.log and results/: last visible test progress, TRX and any blame sequence.
3. resources.txt and runtime-PID.csv: whether memory, GC, CPU or threads change before a stall.
4. Consecutive stacks-TIME-PID.txt: compare repeated waits or active work.
5. stall.txt/deadline.txt and result.txt: distinguish a heuristic snapshot, diagnostic
   deadline, actual test failure and successful completion.
6. testhost-PID-analysis.txt: available only if heap collection/analysis succeeded.

One passing run does not establish that the historical intermittent timeout is fixed.
No fixture-lifetime or rate-limiter fixes are included in this branch.
