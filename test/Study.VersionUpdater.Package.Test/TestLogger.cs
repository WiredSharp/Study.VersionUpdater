using System;
using System.Threading.Tasks;
using NuGet.Common;

namespace Study.VersionUpdater.Package.Test
{
    internal class TestLogger : ILogger
    {
        private readonly Action<string> _testLogger;

        public TestLogger(Action<string> testLogger)
        {
            _testLogger = testLogger;
        }

        public void Log(LogLevel level, string data)
        {
            _testLogger($"[{level}] data");
        }

        public void Log(ILogMessage message)
        {
            _testLogger(message.FormatWithCode());
        }

        public Task LogAsync(LogLevel level, string data)
        {
            Log(level, data);
            return Task.CompletedTask;
        }

        public Task LogAsync(ILogMessage message)
        {
            Log(message);
            return Task.CompletedTask;
        }

        public void LogDebug(string data)
        {
            Log(LogLevel.Debug, data);
        }

        public void LogError(string data)
        {
            Log(LogLevel.Error, data);
        }

        public void LogInformation(string data)
        {
            Log(LogLevel.Information, data);
        }

        public void LogInformationSummary(string data)
        {
            Log(LogLevel.Information, data);
        }

        public void LogMinimal(string data)
        {
            Log(LogLevel.Minimal, data);
        }

        public void LogVerbose(string data)
        {
            Log(LogLevel.Verbose, data);
        }

        public void LogWarning(string data)
        {
            Log(LogLevel.Warning, data);
        }
    }
}