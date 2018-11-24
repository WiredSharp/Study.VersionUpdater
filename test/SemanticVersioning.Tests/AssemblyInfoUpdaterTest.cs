// //////////////////////////////////////////
// 
//  Created by BRUNEL Eric on 19.11.2018
// 
// //////////////////////////////////////////

using System;
using System.IO;
using NUnit.Framework;

namespace SemanticVersioning.Tests
{
    [TestFixture]
    public class AssemblyInfoUpdaterTest
    {
        [Test]
        public void i_can_update_assembly_informational_version_from_folder()
        {
            DirectoryInfo sourceFolder = TestHelpers.GetSourceFolder();
            new AssemblyInfoUpdater().Update(SemanticVersionKind.AssemblyInformationalVersion, 1, sourceFolder);
            foreach (FileInfo fileInfo in sourceFolder.GetFiles())
            {
                AssertFileEqual(TestHelpers.GetTargetSourceFile(fileInfo), fileInfo);
            }
        }

        [Test]
        public void i_can_update_assembly_informational_version_from_file_list()
        {
            DirectoryInfo sourceFolder = TestHelpers.GetSourceFolder();
            new AssemblyInfoUpdater().Update(SemanticVersionKind.AssemblyInformationalVersion, 1, sourceFolder.GetFiles());
            foreach (FileInfo fileInfo in sourceFolder.GetFiles())
            {
                AssertFileEqual(TestHelpers.GetTargetSourceFile(fileInfo), fileInfo);
            }
        }

        private static void AssertFileEqual(FileInfo expected, FileInfo actual)
        {
            Assert.AreEqual(File.Exists(expected.FullName), File.Exists(actual.FullName), $"{actual.Name}: both file does not have the same level of existence");
            string[] expectedLines = File.ReadAllLines(expected.FullName);
            string[] actualLines = File.ReadAllLines(actual.FullName);
            for (int lineIndex = 0; lineIndex < Math.Min(expectedLines.Length, actualLines.Length); lineIndex++)
            {
                Assert.AreEqual(expectedLines[lineIndex], actualLines[lineIndex], $"{actual.Name}#{lineIndex} does not match");
            }
            Assert.AreEqual(expected.Length, actual.Length, $"{actual.Name}: line count does not match");
            Assert.AreEqual(expected.Length, actual.Length, $"{actual.Name}: file length does not match");
        }
    }
}