namespace VisualBasic6X.Converter.Console
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Globalization;
    using System.IO;
    using System.Linq;
    using Microsoft.Build.Framework;
    using Microsoft.Build.Utilities;

    public class Vb6ProjectItems : Task
    {
        private IList<ITaskItem> classes;
        private IList<ITaskItem> components;
        private IList<ITaskItem> forms;
        private IList<ITaskItem> modules;
        private IList<ITaskItem> references;

        /// <summary>
        /// Gets or sets the Visual Basic 6 project file.
        /// </summary>
        /// <value>
        /// The Visual Basic 6 project file.
        /// </value>
        [Required]
        public ITaskItem Project { get; set; }

        /// <summary>
        /// Gets or sets the modules.
        /// </summary>
        /// <value>
        /// The modules.
        /// </value>
        [Output]
        public ITaskItem[] Modules
        {
            get { return modules.ToArray(); }
            set { modules = value; }
        }

        [Output]
        public ITaskItem[] Classes
        {
            get { return classes.ToArray(); }
            set { classes = value; }
        }

        [Output]
        public ITaskItem[] Forms
        {
            get { return forms.ToArray(); }
            set { forms = value; }
        }

        [Output]
        public ITaskItem ResourceFile { get; set; }

        [Output]
        public string OutputType { get; set; }

        [Output]
        public ITaskItem[] References
        {
            get { return references.ToArray(); }
            set { references = value; }
        }

        [Output]
        public ITaskItem[] Components
        {
            get { return components.ToArray(); }
            set { components = value; }
        }

        public ITaskItem Compatibility { get; set; }

        public override bool Execute()
        {
            var projectFile = new FileInfo(Project.GetMetadata("FullPath"));

            if (!projectFile.Exists)
            {
                throw new FileNotFoundException("Couldn't find project file", projectFile.FullName);
            }

            modules = new List<ITaskItem>();
            classes = new List<ITaskItem>();
            forms = new List<ITaskItem>();
            references = new List<ITaskItem>();
            components = new List<ITaskItem>();

            string startup = null;
            string iconForm = null;
            string compatibilityMode = null;

            // Get all the assignment lines in the project file (ignoring blank lines and section headers)
            var projectLines = File.ReadAllLines(projectFile.FullName)
                .Where(line => line.Contains("="));

            foreach (var line in projectLines)
            {
                // Split the key and value
                string[] keyAndValue = line.Split(new[] {"="}, StringSplitOptions.RemoveEmptyEntries);

                Debug.Assert(keyAndValue.Length == 2, "Key and value not split correctly", "Project line: {0}", line);

                string key = keyAndValue[0].ToLower(CultureInfo.InvariantCulture);
                string value = keyAndValue[1].Replace("\"", "").Trim();

                switch (key)
                {
                    case "Type":
                        OutputType = value;
                        break;
                    case "Reference":
                        references.Add(ParseReference(value));
                        break;
                    case "Object":
                        components.Add(ParseReference(value));
                        break;
                    case "Class":
                        classes.Add(ParseSourceFile(value));
                        break;
                    case "Module":
                        modules.Add(ParseSourceFile(value));
                        break;
                    case "Form":
                        modules.Add(ParseSourceFile(value));
                        break;
                    case "ResFile32":
                        ResourceFile = new TaskItem(value);
                        break;
                    case "CompatibleMode":
                        compatibilityMode = value;
                        break;
                    case "CompatibleEXE32":
                        Compatibility = new TaskItem(value);
                        break;
                    case "Startup":
                        startup = value;
                        break;
                    case "IconForm":
                        iconForm = value;
                        break;
                }
            }

            // Was there a startup form set?
            if (!string.IsNullOrWhiteSpace(startup))
            {
                ITaskItem form = FindForm(startup);
                if (form != null) form.SetMetadata("Startup", "true");
            }

            // Was there an icon form set?
            if (!string.IsNullOrWhiteSpace(iconForm))
            {
                ITaskItem form = FindForm(startup);
                if (form != null) form.SetMetadata("Icon", "true");
            }

            // Set the compatibility mode if specified
            if (Compatibility != null && !string.IsNullOrWhiteSpace(compatibilityMode))
            {
                Compatibility.SetMetadata("Mode", compatibilityMode);
            }

            return true;
        }

        private ITaskItem FindForm(string name)
        {
            return forms.SingleOrDefault(
                taskItem => taskItem.GetMetadata("Identity")
                                .Equals(name, StringComparison.OrdinalIgnoreCase));
        }

        private static ITaskItem ParseSourceFile(string value)
        {
            string[] parts = GetReferenceParts(value).ToArray();

            // If there's more than one part, we have an
            // alias to go with the filename.
            string alias = parts.Length > 1 ? parts[0] : null;
            string filename = alias == null ? parts[0] : parts[1];

            var taskItem = new TaskItem(filename);

            if (!string.IsNullOrWhiteSpace(alias)) taskItem.SetMetadata("Alias", alias);

            return taskItem;
        }

        private static ITaskItem ParseReference(string value)
        {
            VisualBasicReference reference = GetReference(value);

            var taskItem = new TaskItem(reference.Filename);
            taskItem.SetMetadata("Guid", reference.Guid.ToString());
            taskItem.SetMetadata("Version", reference.Version);

            if (!string.IsNullOrWhiteSpace(reference.Description))
            {
                taskItem.SetMetadata("Description", reference.Description);
            }

            return taskItem;
        }

        private static VisualBasicReference GetReference(string reference)
        {
            // Split the reference into its parts, delimited by #
            // 0 = GUID
            // 1 = Version
            // 2 = Not sure. Usually 0 and doesn't seem to mean anything.
            // 3 = Path and filename
            // 4 = Name (optional)
            string[] parts = GetReferenceParts(reference).ToArray();

            var guid = new Guid(parts[0]);
            string version = parts[1];
            string filename = parts[2];
            string description = parts.Length > 4 ? parts[4] : null;

            return new VisualBasicReference(guid, version, filename, description);
        }

        private static IEnumerable<string> GetReferenceParts(string reference)
        {
            return reference.Trim()
                .Split(new[] {'#', ';'}, StringSplitOptions.None)
                .Select(s => s.Trim())
                .ToArray();
        }
    }
}