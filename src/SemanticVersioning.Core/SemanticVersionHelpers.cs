// //////////////////////////////////////////
// 
//  Created by BRUNEL Eric on 19.11.2018
// 
// //////////////////////////////////////////

using System;
using System.Text.RegularExpressions;

namespace SemanticVersioning
{
    internal static class SemanticVersionHelpers
    {
        private static readonly string _semverRegex = @"(?<major>\d+)\.(?<minor>\d+)\.(?<patch>\d+)(?<release>(-|#)\w*|)";
        private static readonly Regex _assemblyInformationalVersion = new Regex($@"(?<prefix>AssemblyInformationalVersion\(\s*"")(?<version>{_semverRegex})(?<suffix>"")", RegexOptions.IgnoreCase);

        public static Regex GetRegex(this SemanticVersionKind semanticVersionKind)
        {
            switch (semanticVersionKind)
            {
                case SemanticVersionKind.AssemblyInformationalVersion:
                    return _assemblyInformationalVersion;
                default:
                    throw new ArgumentOutOfRangeException(nameof(semanticVersionKind), semanticVersionKind, "unhandled assembly data");
            }
        }
    }
}