using System;
using System.Configuration;
using CommandLine;
using SemanticVersioning.Updater.Commands;
using Serilog;
using Serilog.Core;

namespace SemanticVersioning.Updater
{
    internal class Program
    {
        private static ILogger Logger;

        public static LoggingLevelSwitch LoggingLevel { get; private set; }

        private static void Main(string[] args)
        {
            int exitCode = 0;
            LoggingLevel = new LoggingLevelSwitch();
            Log.Logger = new LoggerConfiguration()
                .MinimumLevel.ControlledBy(LoggingLevel)
                .WriteTo.ColoredConsole()
                .WriteTo.RollingFile("logs\\versionupdater.log")
                .CreateLogger();
            Logger = Log.Logger.ForContext(typeof(Program));
            try
            {
                Settings.Current.NugetRepository = ConfigurationManager.AppSettings["nugetrepository"];
                Logger.Information($"starting version updater v{AssemblyHelpers.GetAssemblyVersion()}...");
                GetParser()
                    .ParseArguments(args, Command.GetCommands())
                    .WithParsed<Command>(cmd => exitCode = HandleCommand(cmd))
                    .WithNotParsed(errs => exitCode = 1);
            }
            catch (Exception e)
            {
                Logger.Fatal(e, "something goes wrong, execution aborted");
                exitCode = (int)ExitCode.UnexpectedError;
            }
            Logger.Information("...version updater execution completed");
            Environment.Exit(exitCode);
        }

        private static Parser GetParser()
        {
            return Parser.Default;
        }

        private static int HandleCommand(Command command)
        {
            int exitCode;
            try
            {
                exitCode = (int)command.Run();
            }
            catch (Exception e)
            {
                exitCode = (int)ExitCode.UnexpectedError;
                Logger.Fatal(e, "something goes wrong, execution aborted");
            }
            return exitCode;
        }
    }
}
