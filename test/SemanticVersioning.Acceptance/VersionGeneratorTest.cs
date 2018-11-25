// //////////////////////////////////////////
// 
//  Created by BRUNEL Eric on 20.11.2018
// 
// //////////////////////////////////////////

using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using NuGet.Versioning;
using NUnit.Framework;

namespace SemanticVersioning.Acceptance
{
    [TestFixture]
    public class VersionGeneratorTest : TestBase
    {
        private const string PackageId = "Newtonsoft.Json";

        [Test]
        [TestCaseSource(nameof(ExpectedPatches))]
        public async Task i_should_return_expected_patch(string packageId, int major, int minor, int patch)
        {
            var versioner = NewPackageVersioner();
            int nextPatch = await versioner.GetNextPatchAsync(packageId, major, minor, CancellationToken.None).ConfigureAwait(false);
            Assert.AreEqual(patch+1, nextPatch, "unexpected value for next patch");
        }

        [Test]
        public async Task i_get_zero_value_when_package_major_does_not_exist()
        {
            var versioner = NewPackageVersioner();
            var highestMajor = (await TestHelpers.SearchNugetPackagesAsync(PackageId).ConfigureAwait(false)).Max(p => p.Identity.Version.Major);
            int nextPatch = await versioner.GetNextPatchAsync(PackageId, highestMajor+10, 0, CancellationToken.None).ConfigureAwait(false);
            Assert.AreEqual(0, nextPatch, "unexpected value for next patch");
        }

        [Test]
        public async Task i_get_zero_value_when_package_minor_does_not_exist()
        {
            var versioner = NewPackageVersioner();
            var highestMinor = (await TestHelpers.SearchNugetPackagesAsync(PackageId).ConfigureAwait(false)).Max(p => p.Identity.Version.Minor);
            int nextPatch = await versioner.GetNextPatchAsync(PackageId, 1, highestMinor+10, CancellationToken.None).ConfigureAwait(false);
            Assert.AreEqual(0, nextPatch, "unexpected value for next patch");
        }

        [Test]
        public async Task i_can_retrieve_next_version()
        {
            await AssertLastVersion(PackageId);
        }

        [Test]
        public async Task i_can_retrieve_next_patch()
        {
            const int major = 2;
            const int minor = 3;
            await AssertLastPatch(PackageId, major, minor);
        }

        [Test]
        public async Task i_can_retrieve_next_patch_for_new_version()
        {
            const int major = 0;
            const int minor = 3;
            await AssertLastPatch(PackageId, major, minor);
        }

        [Test]
        public async Task i_can_retrieve_next_patch_for_new_package()
        {
            const string packageId = "B2EE949D-E576-4CB7-B6C0-27E9BBD27A04";
            const int major = 1;
            const int minor = 0;
            await AssertLastPatch(packageId, major, minor);
        }

        private static async Task AssertLastPatch(string packageId, int major, int minor)
        {
            var generator = new VersionGenerator(TestHelpers.RepositoryUrl);
            int nextPatch = await generator.GetNextPatchAsync(packageId, major, minor, CancellationToken.None).ConfigureAwait(false);
            int lastPatch = await TestHelpers.GetLastPatchAsync(packageId, major, minor).ConfigureAwait(false);
            if (lastPatch == 0)
            {
                TestContext.WriteLine($"no patch found for {packageId} v{major}.{minor}");
                Assert.AreEqual(0, nextPatch, $"unexpected patch value returned for {packageId}");
            }
            else
            {
                TestContext.WriteLine($"last patch found for {packageId} v{major}.{minor}: {lastPatch}");
                Assert.AreEqual(lastPatch + 1, nextPatch, $"unexpected patch value returned for {packageId}");
            }
        }

        private static async Task AssertLastVersion(string packageId)
        {
            var generator = new VersionGenerator(TestHelpers.RepositoryUrl);
            SemanticVersion nextVersion = await generator.GetNextVersionAsync(packageId, CancellationToken.None)
                .ConfigureAwait(false);
            NuGetVersion lastVersion = await TestHelpers.GetLastVersionAsync(packageId).ConfigureAwait(false);

            if (lastVersion != null)
            {
                TestContext.WriteLine($"last version found for {packageId}: {lastVersion}");
                Assert.AreEqual(lastVersion.Major, nextVersion.Major, $"unexpected major value returned for {packageId}");
                Assert.AreEqual(lastVersion.Minor, nextVersion.Minor, $"unexpected minor value returned for {packageId}");
                Assert.AreEqual(lastVersion.Patch+1, nextVersion.Patch, $"unexpected patch value returned for {packageId}");
                Assert.AreEqual(lastVersion.Release, nextVersion.Release, $"unexpected release value returned for {packageId}");
            }
            else
            {
                TestContext.WriteLine($"no version found for {packageId}");
                Assert.AreEqual(new SemanticVersion(1,0,0), nextVersion, $"unexpected version returned for {packageId}");                
            }
        }

        private VersionGenerator NewPackageVersioner()
        {
            return new VersionGenerator(TestHelpers.RepositoryUrl);
        }

        private static IEnumerable<TestCaseData> ExpectedPatches
        {
            get
            {
                List<TestCaseData> testcases = new List<TestCaseData>();
                foreach (var package in TestHelpers.SearchNugetPackagesAsync(PackageId).Result.Where(p => p.Identity.HasVersion).GroupBy(v => v.Identity.Version.Major*10000+v.Identity.Version.Minor))
                {
                    testcases.Add(new TestCaseData(PackageId, package.Key / 10000,
                        package.Key % 10000, package.Max(v => v.Identity.Version.Patch)));
                }
                return testcases;
            }
        }
    }
}