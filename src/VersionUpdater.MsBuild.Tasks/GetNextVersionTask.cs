using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Build.Framework;
using SemanticVersioning;

namespace VersionUpdater.MsBuild.Tasks
{
    public class GetNextVersionTask : MsBuildTask
    {
        public int TimeOutInMs { get; set; } = 5000;

        [Required]
        public string RepositoryAddress { get; set; }

        [Required]
        public int Minor { get; set; }

        [Required]
        public int Major { get; set; }

        [Required]
        public string PackageId { get; set; }

        [Output]
        public int NextPatch { get; set; }

        [Output] 
        public string Version => $"{Major}.{Minor}.{NextPatch}";

        public override bool Execute()
        {
            try
            {
                var updater = new VersionGenerator(RepositoryAddress);
                Task<int> task = updater.GetNextPatchAsync(PackageId, Major, Minor, CancellationToken.None);
                if (!task.Wait(TimeOutInMs))
                {
                    Log.LogWarning("unable to retrieve nuget data in time");
                    return false;
                }

                NextPatch = task.Result;
                return true;
            }
            catch (AggregateException ex)
            {
                Log.LogError($"cannot retrieve next version: {ex.InnerExceptions.First().Message}");
                return false;
            }
            catch (Exception ex)
            {
                Log.LogError($"cannot retrieve next version: {ex.Message}");
                return false;
            }
        }
    }
}
