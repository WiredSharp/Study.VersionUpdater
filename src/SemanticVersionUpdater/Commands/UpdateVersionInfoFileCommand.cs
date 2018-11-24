// //////////////////////////////////////////
// 
//  Created by BRUNEL Eric on 20.11.2018
// 
// //////////////////////////////////////////

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using CommandLine;

namespace SemanticVersioning.Updater.Commands
{
    [Verb("versioninfo",HelpText = "update assemblyinformationalversion attribute in cs file")]
    internal class UpdateVersionInfoFileCommand : Command
    {
        [Value(0, MetaName = "versioninfofiles", MetaValue = "versioninfo files pattern")]
        public IList<string> VersionInfoFiles { get; set; }

        [Option('p', "package", HelpText = "nuget package to lookup in repository for version", Required = true)]
        public string NugetPackage { get; set; }

        private string _nugetRepository;
        [Option('s', "source", HelpText = "nuget repository")]
        public string NugetRepository
        {
            get
            {
                if (_nugetRepository == null)
                {
                    _nugetRepository = Settings.Current.NugetRepository;
                }
                return _nugetRepository;
            }
            set { _nugetRepository = value; }
        }

        protected override void Execute()
        {
            SemanticVersion? nextVersion = null;
            var updater = new AssemblyInfoUpdater(Logger);
            FileInfo[] versionInfoFiles = ResolveVersionInfoFiles();
            if (!versionInfoFiles.Any())
            {
                Logger.Warning("no versioninfo files found.");
            }
            foreach (FileInfo versionInfoFile in versionInfoFiles)
            {
                if (!versionInfoFile.Exists)
                {
                    Logger.Warning($"{versionInfoFile} does not exist");
                    continue;
                }
                if (!nextVersion.HasValue)
                {
                    IReadOnlyCollection<SemanticVersion> versions = SemanticVersionReader.Read(SemanticVersionKind.AssemblyInformationalVersion, versionInfoFile);
                    if (versions.Any())
                    {
                        SemanticVersion currentVersion = versions.First();
                        Logger.Debug($"current version for {NugetPackage}: {currentVersion}");
                        nextVersion = UpdateVersionAsync(currentVersion).GetAwaiter().GetResult();
                        Logger.Debug($"next version for {NugetPackage}: {nextVersion}");
                    }
                }
                if (nextVersion.HasValue)
                {
                    updater.Update(SemanticVersionKind.AssemblyInformationalVersion, nextVersion.Value.Patch, versionInfoFile);
                }
            }
        }

        /// <summary>
        /// query nuget repository and update <paramref name="currentVersion"/> patch field to match the next patch value to come
        /// </summary>
        /// <param name="currentVersion"></param>
        /// <returns></returns>
        private async Task<SemanticVersion> UpdateVersionAsync(SemanticVersion currentVersion)
        {
            var generator = new VersionGenerator(NugetRepository);
            int nextPatch = await generator.GetNextPatchAsync(NugetPackage, currentVersion.Major, currentVersion.Minor, CancellationToken.None).ConfigureAwait(false);
            return new SemanticVersion(currentVersion.Major, currentVersion.Minor, nextPatch, currentVersion.Release);
        }

        protected FileInfo[] ResolveVersionInfoFiles()
        {
            if (VersionInfoFiles == null || VersionInfoFiles.Count == 0)
            {
                VersionInfoFiles = Directory.GetFiles(Environment.CurrentDirectory, "*Info.cs");
            }
            else
            {
                VersionInfoFiles = VersionInfoFiles.SelectMany(ExpandFilePattern).ToArray();
            }
            return VersionInfoFiles.Select(s => new FileInfo(s)).ToArray();
        }
    }
}