// //////////////////////////////////////////
// 
//  Created by BRUNEL Eric on 07.11.2018
// 
// //////////////////////////////////////////

using System.Threading.Tasks;
using NUnit.Framework;
using SemanticVersioning.Tasks;

namespace SemanticVersioning.Acceptance
{
    [TestFixture]
    [Category("Acceptance")]
    public class GetNextVersionTaskTest : TestBase
    {
        private const string packageId = "Newtonsoft.Json";

        [Test]
        public async Task i_can_retrieve_next_patch()
        {
            const int major = 2;
            const int minor = 3;
            await AssertLastPatch(packageId, major, minor);
        }

        [Test]
        public async Task i_can_retrieve_next_patch_for_new_version()
        {
            const int major = 0;
            const int minor = 3;
            await AssertLastPatch(packageId, major, minor);
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
            var updateTask = new GetNextVersionTask()
            {
                RepositoryAddress = TestHelpers.RepositoryUrl,
                PackageId = packageId,
                Major = major,
                Minor = minor
            };
            bool success = updateTask.Execute();
            Assert.IsTrue(success, "task execution should be successful");
            int lastPatch = await TestHelpers.GetLastPatchAsync(packageId, major, minor).ConfigureAwait(false);
            if (lastPatch == 0)
            {
                TestContext.WriteLine($"no patch found for {packageId} v{major}.{minor}");
                Assert.AreEqual(0, updateTask.NextPatch, $"unexpected patch value returned for {packageId}");
            }
            else
            {
                TestContext.WriteLine($"last patch found for {packageId} v{major}.{minor}: {lastPatch}");
                Assert.AreEqual(lastPatch + 1, updateTask.NextPatch, $"unexpected patch value returned for {packageId}");
            }
        }
    }
}