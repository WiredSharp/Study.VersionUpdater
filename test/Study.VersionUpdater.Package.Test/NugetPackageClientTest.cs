using NuGet.Configuration;
using NuGet.Protocol;
using NuGet.Protocol.Core.Types;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Study.VersionUpdater.Package.Test
{
    [TestFixture]
    public class NugetPackageClientTest
    {
        [Test]
        public void foo()
        {
            const string packageId = "NUnit";
            TestLogger logger = new TestLogger(s => TestContext.WriteLine(s));
            List<Lazy<INuGetResourceProvider>> providers = new List<Lazy<INuGetResourceProvider>>();
            providers.AddRange(Repository.Provider.GetCoreV3());  // Add v3 API support
            PackageSource packageSource = new PackageSource("https://api.nuget.org/v3/index.json");
            SourceRepository sourceRepository = new SourceRepository(packageSource, providers);
            PackageMetadataResource packageMetadataResource = GetTaskResult(() => sourceRepository.GetResourceAsync<PackageMetadataResource>(), "repository does not respond on time");
            IEnumerable<IPackageSearchMetadata> searchMetadata = GetTaskResult(() => packageMetadataResource.GetMetadataAsync(packageId, true, true, logger, CancellationToken.None), "cannot retrieve packages on time");
        }

        private TResult GetTaskResult<TResult>(Func<Task<TResult>> asyncCall, string timeoutMessage)
        {
            Task<TResult> task = asyncCall();
            Assert.IsTrue(task.Wait(3000), "request timeout");
            return task.Result;
        }
    }
}
