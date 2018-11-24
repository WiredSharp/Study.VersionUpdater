// //////////////////////////////////////////
// 
//  Created by BRUNEL Eric on 06.11.2018
// 
// //////////////////////////////////////////

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using NuGet.Configuration;
using NuGet.Protocol;
using NuGet.Protocol.Core.Types;
using NuGet.Versioning;
using SemanticVersioning.Serilog;
using Serilog;

namespace SemanticVersioning
{
    /// <summary>
    /// Generate next version according to last nuget package pushed in repository
    /// </summary>
    public class VersionGenerator
    {
        private readonly ILogger _logger;
        private PackageMetadataResource _packages;
        public string Repository { get; set; } = "https://api.nuget.org/v3/index.json";

        public VersionGenerator(string repositoryAddress)
        :this(repositoryAddress, Log.Logger)
        {
        }

        public VersionGenerator(string repositoryAddress, ILogger logger)
        {
            if (repositoryAddress == null) throw new ArgumentNullException(nameof(repositoryAddress));
            if (logger == null) throw new ArgumentNullException(nameof(logger));
            _logger = logger;
            Repository = repositoryAddress;
        }

        /// <summary>
        /// returns the next patch for package <paramref name="packageId"/> with major <paramref name="major"/> and minor <paramref name="minor"/>
        /// </summary>
        /// <param name="packageId"></param>
        /// <param name="major"></param>
        /// <param name="minor"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public async Task<int> GetNextPatchAsync(string packageId, int major, int minor, CancellationToken cancellationToken)
        {
            NuGetVersion lastVersion = await GetLastVersion(packageId, p => p.Identity.Version.Major == major && p.Identity.Version.Minor == minor, cancellationToken);
            if (lastVersion == null)
            {
                return 0;
            }
            else
            {
                return lastVersion.Patch + 1;
            }
        }

        public async Task<SemanticVersion> GetNextVersionAsync(string packageId, CancellationToken cancellationToken)
        {
            NuGetVersion lastVersion = await GetLastVersion(packageId, metadata => true, cancellationToken);
            if (lastVersion == null)
            {
                return new SemanticVersion(1,0,0);
            }
            else
            {
                return new SemanticVersion(lastVersion.Major, lastVersion.Minor, lastVersion.Patch + 1, lastVersion.Release);
            }
        }

        private async Task<NuGetVersion> GetLastVersion(string packageId, Func<IPackageSearchMetadata, bool> filter, CancellationToken cancellationToken)
        {
            _logger.Information($"retrieving nuget package metadata from {Repository}");
            NuGetLoggerAdapter logger = new NuGetLoggerAdapter(_logger);
            PackageMetadataResource packageMetadataResource = await GetPackageMetadataResourceAsync().ConfigureAwait(false);
            IEnumerable<IPackageSearchMetadata> matchingPackages = await packageMetadataResource
                .GetMetadataAsync(packageId, true, false, logger, cancellationToken).ConfigureAwait(false);
            return matchingPackages.Where(p => p.Identity.HasVersion && filter(p))
                .OrderByDescending(p => p.Identity.Version).FirstOrDefault()?.Identity.Version;
        }

        private async Task<PackageMetadataResource> GetPackageMetadataResourceAsync()
        {
            if (_packages != null)
            {
                return _packages;
            }
            List<Lazy<INuGetResourceProvider>> providers = new List<Lazy<INuGetResourceProvider>>();
            //providers.AddRange(NuGet.Protocol.Core.Types.Repository.Provider.GetCoreV2()); // Add v2 API support
            providers.AddRange(NuGet.Protocol.Core.Types.Repository.Provider.GetCoreV3()); // Add v3 API support
            PackageSource packageSource = new PackageSource(Repository);
            SourceRepository sourceRepository = new SourceRepository(packageSource, providers);
            _packages = await sourceRepository.GetResourceAsync<PackageMetadataResource>().ConfigureAwait(false);
            return _packages;
        }
    }
}