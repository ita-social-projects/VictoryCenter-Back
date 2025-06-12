using Microsoft.Extensions.Logging;

namespace VictoryCenter.IntegrationTests.Utils;

public class InMemoryLoggerProvider : ILoggerProvider
{
    public List<LogEntry> Entries = [];

    public ILogger CreateLogger(string categoryName)
    {
        return new InMemoryLogger(Entries, categoryName);
    }

    public void Dispose()
    { }

    private class InMemoryLogger : ILogger
    {
        private readonly List<LogEntry> _entries;
        private readonly string _category;

        public InMemoryLogger(List<LogEntry> entries, string category)
        {
            _entries = entries;
            _category = category;
        }

        public IDisposable? BeginScope<TState>(TState state) where TState : notnull
        {
            return NullScope.Instance;
        }

        public bool IsEnabled(LogLevel logLevel) => true;

        public void Log<TState>(
            LogLevel logLevel,
            EventId eventId,
            TState state,
            Exception? exception,
            Func<TState, Exception?, string> formatter)
        {
            if (formatter == null) return;

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
        public void Dispose() { }
    }
}

public record LogEntry
{
    public string Category { get; init; } = string.Empty;
    public LogLevel LogLevel { get; init; }
    public string Message { get; init; } = string.Empty;
    public Exception? Exception { get; init; }
}
