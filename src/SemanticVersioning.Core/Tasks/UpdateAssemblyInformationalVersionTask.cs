using System;
using System.IO;
using Microsoft.Build.Framework;
using Microsoft.Build.Utilities;
using SemanticVersioning.Serilog;
using Serilog;
using ILogger = Serilog.ILogger;

namespace SemanticVersioning.Tasks
{
    public class UpdateAssemblyInformationalVersionTask: Task
    {
        [Required]
        public string VersionInfoFile { get; set; }

        [Required]
        public int VersionPatch { get; set; }

        public override bool Execute()
        {
            ILogger logger = new LoggerConfiguration()
                .WriteTo.Sink(new MsbuildLogSink(Log))
                .CreateLogger();
            new AssemblyInfoUpdater(logger).Update(SemanticVersionKind.AssemblyInformationalVersion, VersionPatch, new FileInfo(VersionInfoFile));
            return true;
        }
    }
}
