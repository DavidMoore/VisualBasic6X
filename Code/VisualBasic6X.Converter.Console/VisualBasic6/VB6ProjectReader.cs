using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;

namespace Foundation.Build.VisualBasic6
{
    public class VB6ProjectReader
    {
        /// <summary>
        /// Reads all the project values from the specified Visual Basic 6 project file.
        /// </summary>
        /// <param name="fileName">The VB6 project fileName.</param>
        /// <returns></returns>
        public VB6ProjectProperties Read(string fileName)
        {
            if (fileName == null) throw new ArgumentNullException("fileName");
            if (string.IsNullOrWhiteSpace(fileName)) throw new ArgumentException("The project fileName must not be empty.", "fileName");

            var projectFile = new FileInfo(fileName);

            if (!projectFile.Exists) throw new FileNotFoundException("Couldn't find project file.", projectFile.FullName);

            // Read in all the lines
            var lines = File.ReadAllLines(projectFile.FullName)
                .Where(s => !string.IsNullOrWhiteSpace(s) && s.Contains("="));

            var results = new Dictionary<string, IList<string>>();

            foreach (var line in lines)
            {
                // Split the key and value
                string[] keyAndValue = line.Split(new[] { "=" }, StringSplitOptions.RemoveEmptyEntries);
                Debug.Assert(keyAndValue.Length == 2, "Key and value not split correctly", "Project line: {0}", line);
                string key = keyAndValue[0].ToLower(CultureInfo.InvariantCulture);
                string value = keyAndValue[1].Replace("\"", "").Trim();

                // Are there any existing entries for this key?
                IList<string> valueCollection;

                if (!results.TryGetValue(key, out valueCollection))
                {
                    // There are no entries for this key so we'll have to initialize it first
                    valueCollection = new List<string>();
                }

                valueCollection.Add(value);

                results[key] = valueCollection;
            }

            return new VB6ProjectProperties(results);
        }

        public VB6Project Parse(string fileName)
        {
            var values = Read(fileName);
            var project = new VB6Project
                              {
                                  FileName = fileName,
                                  Name = values.GetSingleValue("Name"),
                                  ProjectType =
                                      (VB6ProjectType)
                                      Enum.Parse(typeof (VB6ProjectType), values.GetSingleValue("Type")),
                                  Startup = values.GetSingleValue("Startup"),
                                  IconForm = values.GetSingleValue("IconForm"),
                                  CompatibilityFile = values.GetSingleValue("CompatibleEXE32"),
                                  CompatibilityMode = values.GetSingleValue("CompatibleMode"),
                                  ResourceFile = values.GetSingleValue("ResFile32")
                              };


            // Parse in the source files
            var sourceFiles = ParseSourceItems(values.GetValues("Form"), VB6SourceFileType.Form)
                .Concat(ParseSourceItems(values.GetValues("Module"), VB6SourceFileType.Module))
                .Concat(ParseSourceItems(values.GetValues("Class"), VB6SourceFileType.Class));
            foreach (var sourceFile in sourceFiles) project.SourceFiles.Add(sourceFile);

            // References & Components
            var references = values.GetValues("Reference").Select(ParseReference);
            var components = values.GetValues("Object").Select(ParseReference);

            foreach (var component in components) project.Components.Add(component);
            foreach (var reference in references) project.References.Add(reference);
            
            return project;
        }

        private IEnumerable<VB6SourceFile> ParseSourceItems(IEnumerable<string> names, VB6SourceFileType sourceFileType)
        {
            if (names == null) return Enumerable.Empty<VB6SourceFile>();
            return names.Select(s => ParseSourceItem(s, sourceFileType));
        }

        private VB6SourceFile ParseSourceItem(string name, VB6SourceFileType sourceFileType)
        {
            var result = new VB6SourceFile();

            // If the name is in two parts split by a semi-colon or comma, that is the name and fileName
            var parts = name.Split(new[] {";", ","}, StringSplitOptions.RemoveEmptyEntries);

            result.Type = sourceFileType;

            switch (parts.Length)
            {
                case 1:
                    result.Name = Path.GetFileNameWithoutExtension(parts[0]);
                    result.FileName = parts[0];
                    break;
                case 2:
                    result.Name = parts[0];
                    result.FileName = parts[1];
                    break;
                default:
                    throw new ArgumentException("The source item name has too many parts (expecting 1 or 2 parts). Name: " + name, "name");
            }

            return result;
        }

        public static VB6Reference ParseReference(string reference)
        {
            // Split the reference into its parts, delimited by #
            // 0 = GUID
            // 1 = Version
            // 2 = Not sure. Usually 0 and doesn't seem to mean anything.
            // 3 = Path and fileName
            // 4 = Name (optional)
            string[] parts = GetReferenceParts(reference).ToArray();

            var guidString = parts[0];

            if (guidString.StartsWith("*")) guidString = guidString.Substring(3);
            var guid = new Guid(guidString);
            string version = parts[1];
            string filename = parts[3];

            string description = parts.Length > 4 ? parts[4] : Path.GetFileNameWithoutExtension(filename);

            return new VB6Reference(guid, version, filename, description);
        }

        private static IEnumerable<string> GetReferenceParts(string reference)
        {
            return reference.Trim()
                .Split(new[] { '#', ';' }, StringSplitOptions.None)
                .Select(s => s.Trim())
                .ToArray();
        }
    }
}