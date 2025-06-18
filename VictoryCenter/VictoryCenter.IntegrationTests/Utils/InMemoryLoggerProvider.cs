using System.Collections.Concurrent;
using Microsoft.Extensions.Logging;

namespace VictoryCenter.IntegrationTests.Utils;

public class InMemoryLoggerProvider : ILoggerProvider
{
    private readonly ConcurrentBag<LogEntry> _entries = [];
    public IReadOnlyCollection<LogEntry> Entries => [.. _entries];
    public ILogger CreateLogger(string categoryName)
        => new InMemoryLogger(_entries, categoryName);

    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    protected virtual void Dispose(bool disposing)
    {
    }

    private class InMemoryLogger : ILogger
    {
        private readonly ConcurrentBag<LogEntry> _entries;
        private readonly string _category;

        public InMemoryLogger(ConcurrentBag<LogEntry> entries, string category)
        {
            _entries = entries;
            _category = category;
        }

        public IDisposable BeginScope<TState>(TState state)
            where TState : notnull
            => NullScope.Instance;

        public bool IsEnabled(LogLevel logLevel) => true;

        public void Log<TState>(
            LogLevel logLevel,
            EventId eventId,
            TState state,
            Exception? exception,
            Func<TState, Exception?, string> formatter)
        {
            if (formatter == null)
            {
                return;
            }

            var message = formatter(state, exception);
            _entries.Add(new LogEntry
            {
                Category = _category,
                LogLevel = logLevel,
                Message = message,
                Exception = exception
            });
        }
    }

    private class NullScope : IDisposable
    {
        public static NullScope Instance { get; } = new();

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
        }
    }
}

public record LogEntry
{
    public string Category { get; init; } = string.Empty;
    public LogLevel LogLevel { get; init; }
    public string Message { get; init; } = string.Empty;
    public Exception? Exception { get; init; }
}
