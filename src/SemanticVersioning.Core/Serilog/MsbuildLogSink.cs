// //////////////////////////////////////////
// 
//  Created by BRUNEL Eric on 19.11.2018
// 
// //////////////////////////////////////////

using System;
using Microsoft.Build.Framework;
using Microsoft.Build.Utilities;
using Serilog.Core;
using Serilog.Events;

namespace SemanticVersioning.Serilog
{
    public class MsbuildLogSink: ILogEventSink
    {
        private TaskLoggingHelper log;

        public MsbuildLogSink(TaskLoggingHelper log)
        {
            this.log = log;
        }

        public void Emit(LogEvent logEvent)
        {
            switch (logEvent.Level)
            {
                case LogEventLevel.Verbose:
                case LogEventLevel.Debug:
                    log.LogMessage(MessageImportance.Low, logEvent.RenderMessage());
                    break;
                case LogEventLevel.Information:
                    log.LogMessage(logEvent.RenderMessage());
                    break;
                case LogEventLevel.Warning:
                    log.LogWarning(logEvent.RenderMessage());
                    break;
                case LogEventLevel.Error:
                    log.LogError(logEvent.RenderMessage());
                    break;
                case LogEventLevel.Fatal:
                    log.LogCriticalMessage(null, null, null, null, 0,0,0,0, logEvent.RenderMessage());
                    break;
                default:
                    log.LogMessage(logEvent.RenderMessage());
                    break;
            }
        }
    }
}