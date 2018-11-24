using System;
using System.Threading.Tasks;
using NuGet.Common;
using ILogger = Serilog.ILogger;

namespace SemanticVersioning.Serilog
{
    internal class NuGetLoggerAdapter: NuGet.Common.ILogger
    {
        private readonly ILogger _logger;

        public NuGetLoggerAdapter(ILogger logger)
        {
            if (logger == null) throw new ArgumentNullException(nameof(logger));
            _logger = logger;
        }

        public void LogErrorSummary(string data)
        {
            LogError(data);
        }

        public void LogDebug(string data)
        {
            _logger.Debug(data);
        }

        public void LogVerbose(string data)
        {
            _logger.Verbose(data);
        }

        public void LogInformation(string data)
        {
            _logger.Information(data);
        }

        public void LogMinimal(string data)
        {
            _logger.Debug(data);
        }

        public void LogWarning(string data)
        {
            _logger.Warning(data);
        }

        public void LogError(string data)
        {
            _logger.Error(data);
        }

        public void LogInformationSummary(string data)
        {
            _logger.Information(data);
        }

        public void Log(LogLevel level, string data)
        {
            switch (level)
            {
                case LogLevel.Debug:
                    LogDebug(data);
                    break;
                case LogLevel.Verbose:
                    LogVerbose(data);
                    break;
                case LogLevel.Information:
                    LogInformation(data);
                    break;
                case LogLevel.Minimal:
                    LogMinimal(data);
                    break;
                case LogLevel.Warning:
                    LogWarning(data);
                    break;
                case LogLevel.Error:
                    LogError(data);
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(level), level, null);
            }
        }

        public Task LogAsync(LogLevel level, string data)
        {
            Log(level, data);
            return Task.CompletedTask;
        }

        public void Log(ILogMessage message)
        {
            if (message == null) return;
            Log(message.Level, message.FormatWithCode());
        }

        public Task LogAsync(ILogMessage message)
        {
            Log(message);
            return Task.CompletedTask;
        }
    }
}