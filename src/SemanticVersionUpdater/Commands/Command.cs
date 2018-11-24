using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using CommandLine;
using Serilog;
using Serilog.Events;

namespace SemanticVersioning.Updater.Commands
{

    internal abstract class Command
    {
        protected ILogger Logger { get; private set; }

        [Option('v',"verbose", HelpText = "Verbose")]
        public bool Verbose { get; set; }

        public ExitCode Run()
        {
            Initialize();
            string name = GetCommandName();
            Logger.Verbose("executing command {Name}...", name);
            try
            {
                Execute();
                return ExitCode.Success;
            }
            catch (Exception ex)
            {
                Logger.Error(ex, "... command {Name} execution failed", name);
                return ExitCode.ExecutionFailure;
            }
        }

        private string GetCommandName()
        {
            return GetAttribute<VerbAttribute>(GetType())?.Name ?? "N/A";
        }

        public static Type[] GetCommands()
        {
            return Assembly.GetExecutingAssembly().GetTypes().Where(t => GetAttribute<VerbAttribute>(t) != null).ToArray();
        }

        private static TAttribute GetAttribute<TAttribute>(MemberInfo t) where TAttribute : Attribute
        {
            return (TAttribute) Attribute.GetCustomAttribute(t, typeof(TAttribute));
        }

        protected abstract void Execute();

        protected void Initialize()
        {
            if (Verbose)
            {
                Program.LoggingLevel.MinimumLevel = LogEventLevel.Verbose;
            }
            Logger = Log.ForContext(GetType());
        }

        protected static IEnumerable<string> ExpandFilePattern(string filePattern)
        {
            string directoryName = Path.GetDirectoryName(filePattern);
            if (String.IsNullOrEmpty(directoryName))
            {
                directoryName = Environment.CurrentDirectory;
            }
            string searchPattern = Path.GetFileName(filePattern);
            if (String.IsNullOrEmpty(searchPattern))
            {
                searchPattern = "*Info.cs";
            }
            try
            {
                return Directory.GetFiles(directoryName, searchPattern);

            }
            catch (DirectoryNotFoundException)
            {
                return new string[0];
            }
        }
    }

    public enum ExitCode
    {
        Success = 0,
        CommandLine = 1,
        ExecutionFailure = 2,
        UnexpectedError = 100
    }
}