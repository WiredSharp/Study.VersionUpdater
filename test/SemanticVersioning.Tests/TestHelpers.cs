// //////////////////////////////////////////
// 
//  Created by BRUNEL Eric on 19.11.2018
// 
// //////////////////////////////////////////

using System.IO;
using NUnit.Framework;

namespace SemanticVersioning.Tests
{
    internal static class TestHelpers
    {
        public static DirectoryInfo GetSourceFolder()
        {
            return new DirectoryInfo(Path.Combine(TestContext.CurrentContext.TestDirectory, "playground", "sources"));
        }

        public static FileInfo GetSourceFile(string filename)
        {
            return new FileInfo(Path.Combine(TestContext.CurrentContext.TestDirectory, "playground", "sources", filename));
        }

        public static FileInfo GetTargetSourceFile(FileInfo fileInfo)
        {
            return new FileInfo(Path.Combine(TestContext.CurrentContext.TestDirectory,"playground","target", fileInfo.Name));
        }
    }
}