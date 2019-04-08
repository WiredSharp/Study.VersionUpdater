// //////////////////////////////////////////
// 
//  Created by BRUNEL Eric on 20.11.2018
// 
// //////////////////////////////////////////

using System.Collections.Generic;
using System.IO;
using System.Linq;
using Microsoft.Build.Framework;
using SemanticVersioning;

namespace VersionUpdater.MsBuild.Tasks
{
    public class ReadAssemblyInformationalVersionTask: MsBuildTask
    {
        [Required]
        public string VersionInfoFile { get; set; }

        [Output]
        public int? Major => SemanticVersion?.Major;

        [Output]
        public int? Minor => SemanticVersion?.Minor;

        [Output]
        public int? Patch => SemanticVersion?.Patch;

        [Output] public string Release => SemanticVersion?.Release;

        [Output] public string Version => SemanticVersion?.ToString();

        public override bool Execute()
        {
            IReadOnlyCollection<SemanticVersion> versions = SemanticVersionReader.Read(SemanticVersionKind.AssemblyInformationalVersion, new FileInfo(VersionInfoFile));
            if (versions.Count == 0)
            {
                Log.LogWarning($"no assemblyInformationVersion found in {VersionInfoFile}");
                return false;
            }
            if (versions.Count > 1)
            {
                Log.LogWarning($"multiple assemblyInformationVersion found in {VersionInfoFile}");
            }
            SemanticVersion = versions.First();
            return true;
        }

        private SemanticVersion? SemanticVersion { get; set; }
    }
}