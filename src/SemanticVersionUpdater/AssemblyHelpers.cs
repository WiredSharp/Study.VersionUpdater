// //////////////////////////////////////////
// 
//  Created by BRUNEL Eric on 13.07.2018
// 
// //////////////////////////////////////////

using System;
using System.IO;
using System.Reflection;
using System.Text;

namespace SemanticVersioning.Updater
{
    internal static class AssemblyHelpers
    {
        public static string ReadEmbeddedResource(string resourcePath)
        {
            Stream stream = Assembly.GetExecutingAssembly().GetManifestResourceStream($"{typeof(Program).Namespace}.{resourcePath}");
            if (stream == null)
            {
                throw new ArgumentException($"{resourcePath}: unknown resource", nameof(resourcePath));
            }
            using (stream)
            {
                using (var reader = new StreamReader(stream, Encoding.UTF8))
                {
                    return reader.ReadToEnd();
                }
            }
        }
        public static string GetAssemblyVersion()
        {
            return GetAssemblyVersion(Assembly.GetExecutingAssembly());
        }

        public static string GetAssemblyVersion(Assembly assembly)
        {
            return ((AssemblyInformationalVersionAttribute)Attribute.GetCustomAttribute(assembly, typeof(AssemblyInformationalVersionAttribute))).InformationalVersion;
        }
    }
}