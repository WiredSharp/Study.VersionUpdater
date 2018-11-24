// //////////////////////////////////////////
// 
//  Created by BRUNEL Eric on 19.11.2018
// 
// //////////////////////////////////////////

using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;

namespace SemanticVersioning
{
    /// <summary>
    /// reads assembly information from assembly attributes in C# source file
    /// </summary>
    public class SemanticVersionReader
    {
        public static IReadOnlyCollection<SemanticVersion> Read(SemanticVersionKind semanticVersionKind, FileInfo file)
        {
            string content = File.ReadAllText(file.FullName);
            return Read(semanticVersionKind, content);
        }

        public static IReadOnlyCollection<SemanticVersion> Read(SemanticVersionKind semanticVersionKind, string content)
        {
            Regex regex = semanticVersionKind.GetRegex();
            List<SemanticVersion> versions = new List<SemanticVersion>();
            foreach (Match match in regex.Matches(content))
            {
                if (match.Success)
                {
                    versions.Add(SemanticVersion.Parse(match.Groups["major"].Value, match.Groups["minor"].Value, match.Groups["patch"].Value, match.Groups["release"]?.Value));
                }
            }
            return versions;
        }
    }
}