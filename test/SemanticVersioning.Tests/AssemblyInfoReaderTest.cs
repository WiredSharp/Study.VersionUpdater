// //////////////////////////////////////////
// 
//  Created by BRUNEL Eric on 19.11.2018
// 
// //////////////////////////////////////////

using System;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;

namespace SemanticVersioning.Tests
{
    [TestFixture]
    public class AssemblyInfoReaderTest
    {
        [Test]
        public void i_can_read_assemblyInformationalVersion()
        {
            var assemblyInformationalVersions = SemanticVersionReader.Read(SemanticVersionKind.AssemblyInformationalVersion,
                TestHelpers.GetSourceFile("AssemblyInfo.cs"));
            CollectionAssert.AreEquivalent(new[] {new SemanticVersion(6,1,3,"-alpha"), new SemanticVersion(6,1,3)}, assemblyInformationalVersions);
        }
        
        [Test]
        [TestCaseSource(nameof(AssemblyInformationalVersions))]
        public SemanticVersion i_can_extract_assembly_informational_version(string content)
        {
            return SemanticVersionReader.Read(SemanticVersionKind.AssemblyInformationalVersion, content).First();
        }
        
        [Test]
        [TestCaseSource(nameof(NoMatchAssemblyInformationalVersions))]
        public void incomplete_assembly_informational_version_are_not_retrieved(string content)
        {
            Assert.IsEmpty(SemanticVersionReader.Read(SemanticVersionKind.AssemblyInformationalVersion, content), $"no version should be retrieved from {content}");
        }

        private static IEnumerable<TestCaseData> AssemblyInformationalVersions
        {
            get
            {
                yield return new TestCaseData("[assembly: AssemblyInformationalVersion(\"6.1.3\")]") { TestName = "regular version", ExpectedResult = new SemanticVersion(6,1,3)};
                yield return new TestCaseData("[assembly: AssemblyInformationalVersion(\"5.4.3-alpha\")]") { TestName = "pre-release version", ExpectedResult = new SemanticVersion(5,4,3,"-alpha")};
                yield return new TestCaseData("[assembly: AssemblyInformationalVersion(\"5.4.3#45356\")]") { TestName = "version with build number", ExpectedResult = new SemanticVersion(5,4,3,"#45356")};
            }
        }

        private static IEnumerable<TestCaseData> NoMatchAssemblyInformationalVersions
        {
            get
            {
                yield return new TestCaseData("[assembly: AssemblyInformationalVersion(\"\")]") { TestName = "empty version"};
                yield return new TestCaseData("[assembly: AssemblyInformationalVersion(\"5.4-alpha\")]") { TestName = "pre-release no patch"};
                yield return new TestCaseData("[assembly: AssemblyInformationalVersion(\"beta-5.4.3\")]") { TestName = "invalid pre-release format"};
                yield return new TestCaseData("[assembly: AssemblyInformationalVersion(\"15.46.23.3\")]") { TestName = "four fields version"};
                yield return new TestCaseData("[assembly: AssemblyInformationalVersion(\"a6.1.2\")]") { TestName = "non numeric major field"};
                yield return new TestCaseData("[assembly: AssemblyInformationalVersion(\"6.r1.5\")]") { TestName = "non numeric minor field"};
                yield return new TestCaseData("[assembly: AssemblyInformationalVersion(\"6.1.r5\")]") { TestName = "non numeric patch field"};
                yield return new TestCaseData("[assembly: AssemblyInformationalVersion(\"6.1\")]") { TestName = "no patch"};
            }
        }
    }
}