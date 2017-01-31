namespace VisualBasic6X.Converter.Console
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using Microsoft.Build.Evaluation;

    /// <summary>
    /// Converts a Visual Basic 6 project to the new .vbpx MSBuild project format.
    /// </summary>
    public class ProjectConverter
    {
        private static readonly IEnumerable<Guid> primaryReferences = new[]
        {
            new Guid("{00020430-0000-0000-C000-000000000046}"), // OLE Automation
            new Guid("{EF53050B-882E-4776-B643-EDA472E8E3F2}"), // Microsoft ActiveX Data Objects 
        };

        public Project Convert(VB6Project vb6Project)
        {
            if (vb6Project == null) throw new ArgumentNullException("vb6Project");

            var project = new Project();

            // Default target is Build
            project.Xml.DefaultTargets = "Build";

            // Default configuration is Debug
            var configurationProperty = project.Xml.AddProperty("Configuration", "Debug");
            configurationProperty.Condition = "'$(Configuration)' == ''";

            // Standard properties
            project.SetProperty("SchemaVersion", "2.0");
            project.SetProperty("ProjectGuid", Guid.NewGuid().ToString("B"));
            project.SetProperty("AssemblyName", vb6Project.Name);
            //project.SetProperty("RootNamespace", projectName);
            project.SetProperty("EnableUnmanagedDebugging", true.ToString());
            
            // Determine the project output type
            string projectType;
            switch (vb6Project.ProjectType)
            {
                case VB6ProjectType.OleExe:
                    // For an ActiveX exe, there's no equivalent standard property we can use,
                    // so we have a special property we can use to override.
                    project.SetProperty("VB6OutputType", "OleExe");
                    projectType = "WinExe";
                    break;
                case VB6ProjectType.Exe:
                    projectType = "WinExe";
                    break;
                case VB6ProjectType.OleDll:
                    projectType = "Library";
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
            project.SetProperty("ProjectType", projectType);

            // Debug configuration
            var debugConfig = project.Xml.AddPropertyGroup();
            debugConfig.Condition = "'$(Configuration)' == 'Debug'";
            debugConfig.AddProperty("OutputPath", @"bin\Debug\");

            // Release configuration
            var releaseConfig = project.Xml.AddPropertyGroup();
            releaseConfig.Condition = "'$(Configuration)' == 'Release'";
            releaseConfig.AddProperty("OutputPath", @"bin\Release\");

            // Will contain the items for compilation
            var compileGroup = project.Xml.AddItemGroup();

            // Will contain non-compilation items
            var noneGroup = project.Xml.AddItemGroup();

            // Add our source files
            foreach (var source in vb6Project.SourceFiles)
            {
                var item = compileGroup.AddItem("Compile", source.FileName);

                // Is this the startup item?
                if (source.Name.Equals(vb6Project.Startup, StringComparison.OrdinalIgnoreCase)) item.AddMetadata("Startup", "true");

                // If this is a form, we may need additional meta data
                if (source.Type != VB6SourceFileType.Form) continue;
                
                // Icon form?
                if (source.Name.Equals(vb6Project.IconForm, StringComparison.OrdinalIgnoreCase)) item.AddMetadata("Icon", "true");

                // Is there a .frx to go with this form? We will want to include it
                // in the project as a dependency.
                var formFrxPath = Path.Combine( Path.GetDirectoryName(vb6Project.FileName), source.Name + ".frx");
                var formFrxFile = new FileInfo(formFrxPath);
                if (!formFrxFile.Exists) continue;

                // We add the .frx as another item, but we make it dependent on the
                // parent form, so it shows up as nested in Visual Studio (like code-behind files).
                var frxItem = noneGroup.AddItem("Compile", formFrxFile.Name);
                frxItem.AddMetadata("DependentUpon", source.FileName);
            }

            // COM References
            var referenceGroup = project.Xml.AddItemGroup();
            foreach (var reference in vb6Project.References)
            {
                var item = referenceGroup.AddItem("COMReference", reference.Description);

                // Add the COM Reference meta data
                item.AddMetadata("Guid", reference.Guid.ToString("B"));
                item.AddMetadata("VersionMajor", reference.VersionMajor.ToString());
                item.AddMetadata("VersionMinor", reference.VersionMinor.ToString());
                item.AddMetadata("Lcid", "0");
                item.AddMetadata("Isolated", "False");
                item.AddMetadata("WrapperTool", IsPrimaryReference(reference.Guid) ? "primary" : "tlbimp");
                item.AddMetadata("EmbedInteropTypes", "True");
                item.AddMetadata("Private", "True");
            }
            foreach (var reference in vb6Project.Components)
            {
                var item = referenceGroup.AddItem("COMReference", reference.FileName);

                // Add the COM Reference meta data
                item.AddMetadata("Guid", reference.Guid.ToString("B"));
                item.AddMetadata("VersionMajor", reference.VersionMajor.ToString());
                item.AddMetadata("VersionMinor", reference.VersionMinor.ToString());
                item.AddMetadata("Lcid", "0");
                item.AddMetadata("Isolated", "False");
                item.AddMetadata("WrapperTool", IsPrimaryReference(reference.Guid) ? "primary" : "aximp");
                item.AddMetadata("EmbedInteropTypes", "True");
                item.AddMetadata("Private", "True");
            }

            // We need to import the Visual Basic 6 build targets for this to compile properly.
            project.Xml.AddImport(@"$(LocalAppData)\SadRobot\VisualBasic6X\VisualBasic6.targets");

            // The converted project will be in the same location as the project, except with the .vbpx extension.
            var convertedProject = new FileInfo(Path.Combine(Path.GetDirectoryName(vb6Project.FileName), Path.GetFileNameWithoutExtension(vb6Project.Name) + ".vbpx"));
            project.Save(convertedProject.FullName);

            return project;
        }

        private bool IsPrimaryReference(Guid guid)
        {
            return primaryReferences.Any(g => g.Equals(guid));
        }
    }
}