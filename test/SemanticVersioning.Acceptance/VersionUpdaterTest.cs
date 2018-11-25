// //////////////////////////////////////////
// 
//  Created by BRUNEL Eric on 21.11.2018
// 
// //////////////////////////////////////////

using System;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using NuGet.Versioning;
using NUnit.Framework;

namespace SemanticVersioning.Acceptance
{
    [TestFixture]
    public class VersionUpdaterTest : TestBase
    {
        private const string packageId = "Newtonsoft.Json";

        [Test]
        public async Task i_can_update_versionInfo_file()
        {
            SemanticVersion lastReleaseVersion = await GenerateVersionInfoFilesAsync(packageId);
            RunVersionUpdater(packageId, ".\\playground\\sources\\");
            foreach (FileInfo fileInfo in TestHelpers.GetSourceFolder().GetFiles("*info.cs"))
            {
                string fileContent = File.ReadAllText(fileInfo.FullName);
                Assert.That(fileContent, Does.Contain($"[assembly: AssemblyInformationalVersion(\"{lastReleaseVersion.Major}.{lastReleaseVersion.Minor}.{lastReleaseVersion.Patch+1}-alpha\")]"));
                Assert.That(fileContent, Does.Contain($"[assembly: AssemblyInformationalVersion(\"{lastReleaseVersion.Major}.{lastReleaseVersion.Minor}.{lastReleaseVersion.Patch+1}\")]"));
            }
        }

        private void RunVersionUpdater(string packageId, string sourceFolder)
        {
            var process = new Process
            {
                StartInfo = new ProcessStartInfo(Path.Combine(TestContext.CurrentContext.TestDirectory, "versionupdater.exe"))
                {
                    UseShellExecute = false,
                    RedirectStandardError = true,
                    RedirectStandardOutput = true,
                    CreateNoWindow = true,
                    WorkingDirectory = TestContext.CurrentContext.TestDirectory,
                    Arguments = $"versioninfo -v -p {packageId} -s {TestHelpers.RepositoryUrl} {sourceFolder}"
                }
            };
            try
            {
                process.OutputDataReceived += OnOutputDataReceived;
                process.ErrorDataReceived += OnErrorDataReceived;
                if (!process.Start())
                    Assert.Fail("cannot start the process");
                process.BeginOutputReadLine();
                process.BeginErrorReadLine();
                if (!process.WaitForExit((int) TimeSpan.FromSeconds(5).TotalMilliseconds))
                    Assert.Fail("execution timeout");
                Assert.AreEqual(0, process.ExitCode, "unexpected exit code");
                process.Close();
            }
            finally
            {
                process?.Dispose();
                process.OutputDataReceived -= OnOutputDataReceived;
                process.ErrorDataReceived -= OnErrorDataReceived;
            }
        }

        private static async Task<SemanticVersion> GenerateVersionInfoFilesAsync(string packageId)
        {
            NuGetVersion lastVersion = await TestHelpers.GetLastVersionAsync(packageId);
            TestContext.WriteLine($"last version found for {packageId}: {lastVersion}");
            if (!TestHelpers.GetSourceFolder().Exists)
            {
                TestHelpers.GetSourceFolder().Create();
            }
            foreach (FileInfo fileInfo in TestHelpers.GetTemplateFolder().GetFiles("*info.cs"))
            {
                File.WriteAllText(TestHelpers.GetSourceFile(fileInfo.Name).FullName,
                    File.ReadAllText(fileInfo.FullName).Replace("$version$", $"{lastVersion.Version.Major}.{lastVersion.Version.Minor}.{lastVersion.Version.Revision}"), Encoding.UTF8);
            }
            return new SemanticVersion(lastVersion.Major, lastVersion.Minor, lastVersion.Patch, lastVersion.Release);
        }

        private void OnErrorDataReceived(object sender, DataReceivedEventArgs e)
        {
            TestContext.WriteLine($"[ERROR] {e.Data}");
        }

        private void OnOutputDataReceived(object sender, DataReceivedEventArgs e)
        {
            TestContext.WriteLine(e.Data);
        }
    }
}