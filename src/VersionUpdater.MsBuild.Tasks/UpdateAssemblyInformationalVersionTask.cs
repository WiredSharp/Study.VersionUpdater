using System.IO;
using Microsoft.Build.Framework;
using SemanticVersioning;

namespace VersionUpdater.MsBuild.Tasks
{
    public class UpdateAssemblyInformationalVersionTask: MsBuildTask
    {
        [Required]
        public string VersionInfoFile { get; set; }

        [Required]
        public int VersionPatch { get; set; }

        public override bool Execute()
        {
            new AssemblyInfoUpdater().Update(SemanticVersionKind.AssemblyInformationalVersion, VersionPatch, new FileInfo(VersionInfoFile));
            return true;
        }
    }
}
