using Microsoft.Extensions.Logging;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;

namespace UseTheOps.PolyglotInitiative.Tests
{
    public class InMemoryLoggerProvider : ILoggerProvider
    {
        private readonly InMemoryLogger _logger = new InMemoryLogger();
        public ILogger CreateLogger(string categoryName) => _logger;
        public void Dispose() { }
        public IList<LogEntry> GetLogs() => _logger.Logs;
    }

    public class InMemoryLogger : ILogger
    {
        public readonly List<LogEntry> Logs = new List<LogEntry>();
        public IDisposable BeginScope<TState>(TState state) => null;
        public bool IsEnabled(LogLevel logLevel) => true;
        public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception exception, Func<TState, Exception, string> formatter)
        {
            Logs.Add(new LogEntry
            {
                LogLevel = logLevel,
                Message = formatter(state, exception),
                Exception = exception,
                Timestamp = DateTime.UtcNow
            });
        }
    }

    public class LogEntry
    {
        public LogLevel LogLevel { get; set; }
        public string Message { get; set; }
        public Exception Exception { get; set; }
        public DateTime Timestamp { get; set; }
    }
}
