using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Serilog;

namespace SemanticVersioning
{
    public class SemanticVersionUpdater
    {
        protected ILogger Logger { get; }

        public SemanticVersionUpdater()
        {
            Logger = Log.Logger;
        }

        public SemanticVersionUpdater(ILogger logger)
        {
            Logger = logger;
        }

        public void Update(SemanticVersionKind semanticVersionKind, int newPatch, params FileInfo[] files)
        {
            Update(semanticVersionKind, newPatch, (IEnumerable<FileInfo>)files);
        }

        public void Update(SemanticVersionKind semanticVersionKind, int newPatch, IEnumerable<FileInfo> files)
        {
            if (files == null) throw new ArgumentNullException(nameof(files));
            foreach (FileInfo file in files)
            {
                Logger.Information($"updating {semanticVersionKind} patch to {newPatch} in {file.FullName}");
                string content = File.ReadAllText(file.FullName);
                string updatedContent = Update(semanticVersionKind, newPatch, content);
                File.WriteAllText(file.FullName, updatedContent, Encoding.UTF8);
            }
        }

        public static string Update(SemanticVersionKind semanticVersionKind, int newPatch, string content)
        {
            return semanticVersionKind.GetRegex().Replace(content, $"${{prefix}}${{major}}.${{minor}}.{newPatch}${{release}}${{suffix}}");
        }
    }
}