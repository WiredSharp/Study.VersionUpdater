// //////////////////////////////////////////
// 
//  Created by BRUNEL Eric on 20.11.2018
// 
// //////////////////////////////////////////

using Microsoft.Build.Utilities;
using SemanticVersioning.Serilog;
using Serilog;

namespace SemanticVersioning.Tasks
{
    public abstract class MsBuildTask:Task
    {
        protected ILogger Logger { get; }

        protected MsBuildTask()
        {
            Logger = new LoggerConfiguration()
                .WriteTo.Sink(new MsbuildLogSink(Log))
                .CreateLogger();            
        }
    }
}