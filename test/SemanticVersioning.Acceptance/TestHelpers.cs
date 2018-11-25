// //////////////////////////////////////////
// 
//  Created by BRUNEL Eric on 07.11.2018
// 
// //////////////////////////////////////////

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using NuGet.Configuration;
using NuGet.Protocol;
using NuGet.Protocol.Core.Types;
using NuGet.Versioning;
using NUnit.Framework;
using SemanticVersioning.Serilog;
using Serilog;

namespace SemanticVersioning.Acceptance
{
    public class TestHelpers
    {
        public const string RepositoryUrl = "https://api.nuget.org/v3/index.json";

        public static async Task<int> GetLastPatchAsync(string packageId, int major, int minor, bool includePreRelease = true)
        {
            return (await SearchNugetPackagesAsync(packageId).ConfigureAwait(false)).Where(p => p.Identity.HasVersion && p.Identity.Version.Minor == minor  && p.Identity.Version.Major == major && (includePreRelease || !p.Identity.Version.IsPrerelease)).DefaultIfEmpty().Max(p => p?.Identity.Version.Patch ?? 0);
        }

        public static async Task<NuGetVersion> GetLastVersionAsync(string packageId, bool includePreRelease = true)
        {
            return (await SearchNugetPackagesAsync(packageId).ConfigureAwait(false)).Where(p => p.Identity.HasVersion && (includePreRelease || !p.Identity.Version.IsPrerelease)).OrderByDescending(p => p.Identity.Version).FirstOrDefault()?.Identity.Version;
        }

        public static async Task<IEnumerable<IPackageSearchMetadata>> SearchNugetPackagesAsync(string packageId)
        {
            List<Lazy<INuGetResourceProvider>> providers = new List<Lazy<INuGetResourceProvider>>();
            //providers.AddRange(Repository.Provider.GetCoreV2()); // Add v2 API support, type not found in NuGet.Protocol.Core.Types
            providers.AddRange(Repository.Provider.GetCoreV3()); // Add v3 API support
            PackageSource packageSource = new PackageSource(RepositoryUrl);
            SourceRepository sourceRepository = new SourceRepository(packageSource, providers);
            var packages = await sourceRepository.GetResourceAsync<PackageMetadataResource>().ConfigureAwait(false);
            return await packages.GetMetadataAsync(packageId, true, false,
                new NuGetLoggerAdapter(Log.Logger),
                CancellationToken.None).ConfigureAwait(false);
        }

        public static DirectoryInfo GetSourceFolder()
        {
            return new DirectoryInfo(Path.Combine(TestContext.CurrentContext.TestDirectory, "playground", "sources"));
        }

        public static DirectoryInfo GetTemplateFolder()
        {
            return new DirectoryInfo(Path.Combine(TestContext.CurrentContext.TestDirectory, "playground", "templates"));
        }

        public static FileInfo GetFile(string filename)
        {
            return new FileInfo(Path.Combine(TestContext.CurrentContext.TestDirectory, "playground", filename));
        }

        public static FileInfo GetSourceFile(string filename)
        {
            return new FileInfo(Path.Combine(TestContext.CurrentContext.TestDirectory, "playground", "sources", filename));
        }

        public static FileInfo GetTargetSourceFile(FileInfo fileInfo)
        {
            return new FileInfo(Path.Combine(TestContext.CurrentContext.TestDirectory,"playground","target", fileInfo.Name));
        }    }
}