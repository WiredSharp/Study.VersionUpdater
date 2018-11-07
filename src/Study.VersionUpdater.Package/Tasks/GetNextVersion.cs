using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Build.Framework;
using Microsoft.Build.Utilities;
using NuGet.Configuration;
using NuGet.Protocol;
using NuGet.Protocol.Core.Types;
using MSBuildTask = Microsoft.Build.Utilities.Task;

namespace VersionUpdater.Tasks
{
    public class GetNextVersion : MSBuildTask
    {
        public int TimeOut { get; set; }
        public string PackageId { get; set; }

        public override bool Execute()
        {
            MSBuildLogger logger = new MSBuildLogger(Log);
            List<Lazy<INuGetResourceProvider>> providers = new List<Lazy<INuGetResourceProvider>>();
            providers.AddRange(Repository.Provider.GetCoreV3());  // Add v3 API support
            PackageSource packageSource = new PackageSource("https://api.nuget.org/v3/index.json");
            SourceRepository sourceRepository = new SourceRepository(packageSource, providers);
            PackageMetadataResource packageMetadataResource = GetTaskResult(() => sourceRepository.GetResourceAsync<PackageMetadataResource>(), "repository does not respond on time");
            IEnumerable<IPackageSearchMetadata> searchMetadata = GetTaskResult(() => packageMetadataResource.GetMetadataAsync(PackageId, true, true, logger, CancellationToken.None), "cannot retrieve packages on time");
            return true;
        }

        private TResult GetTaskResult<TResult>(Func<Task<TResult>> asyncCall, string timeoutMessage)
        {
            Task<TResult> task = asyncCall();
            if (!task.Wait(TimeOut))
            {
                Log.LogError(timeoutMessage);
                throw new TimeoutException(timeoutMessage);
            }
            return task.Result;
        }
    }

    internal class MSBuildLogger : NuGet.Common.ILogger
    {
        private readonly TaskLoggingHelper _taskLogger;

        public MSBuildLogger(TaskLoggingHelper taskLogger)
        {
            _taskLogger = taskLogger;
        }

        public void LogDebug(string data)
        {
            _taskLogger.LogMessage(MessageImportance.Low, data);
        }

        public void LogError(string data)
        {
            _taskLogger.LogError(data);
        }

        public void LogErrorSummary(string data)
        {
            _taskLogger.LogError(data);
        }

        public void LogInformation(string data)
        {
            _taskLogger.LogMessage(MessageImportance.Normal, data);
        }

        public void LogInformationSummary(string data)
        {
            _taskLogger.LogMessage(MessageImportance.Normal, data);
        }

        public void LogMinimal(string data)
        {
            _taskLogger.LogMessage(MessageImportance.Low, data);
        }

        public void LogVerbose(string data)
        {
            _taskLogger.LogMessage(MessageImportance.Low, data);
        }

        public void LogWarning(string data)
        {
            _taskLogger.LogWarning(data);
        }
    }
}
