// //////////////////////////////////////////
// 
//  Created by BRUNEL Eric on 19.11.2018
// 
// //////////////////////////////////////////

using System;
using System.IO;
using Serilog;

namespace SemanticVersioning
{
    /// <summary>
    /// update C# source file for various assembly data
    /// </summary>
    public class AssemblyInfoUpdater : SemanticVersionUpdater
    {
        public AssemblyInfoUpdater() : base()
        {
        }

        public AssemblyInfoUpdater(ILogger logger) : base(logger)
        {
        }

        public void Update(SemanticVersionKind semanticVersionKind, int newPatch, DirectoryInfo folder)
        {
            if (folder == null) throw new ArgumentNullException(nameof(folder));
            Update(semanticVersionKind, newPatch, folder.GetFiles("*info.cs", SearchOption.AllDirectories));
        }
    }
}