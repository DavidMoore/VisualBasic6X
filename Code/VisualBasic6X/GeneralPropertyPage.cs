using System;
using System.IO;
using System.Runtime.InteropServices;
using Microsoft.VisualStudio;
using Microsoft.VisualStudio.Project;
using Microsoft.VisualStudio.Shell.Interop;
using VisualBasic6.Attributes;

namespace VisualBasic6
{
    /// <summary>
    /// This class implements general property page for the project type.
    /// </summary>
    [ComVisible(true)]
    [Guid(GuidStrings.GeneralPropertyPage)]
    public class GeneralPropertyPage : SettingsPage
    {
        string applicationIcon;
        string assemblyName;
        string defaultNamespace;
        OutputType outputType;
        string startupObject;
        
        /// <summary>
        /// Explicitly defined default constructor.
        /// </summary>
        public GeneralPropertyPage()
        {
            Name = Resources.GetString(Resources.GeneralCaption);
        }


        /// <summary>
        /// Gets or sets Assembly Name.
        /// </summary>
        /// <remarks>IsDirty flag was switched to true.</remarks>
        [ResourcesCategory(Resources.AssemblyName)]
        [Attributes.LocDisplayName(Resources.AssemblyName)]
        [ResourcesDescriptionAttribute(Resources.AssemblyNameDescription)]
        public string AssemblyName
        {
            get { return assemblyName; }
            set
            {
                assemblyName = value;
                IsDirty = true;
            }
        }


        /// <summary>
        /// Gets or sets OutputType.
        /// </summary>
        /// <remarks>IsDirty flag was switched to true.</remarks>
        [ResourcesCategoryAttribute(Resources.Application)]
        [Attributes.LocDisplayName(Resources.OutputType)]
        [ResourcesDescriptionAttribute(Resources.OutputTypeDescription)]
        public OutputType OutputType
        {
            get { return outputType; }
            set
            {
                outputType = value;
                IsDirty = true;
            }
        }


        /// <summary>
        /// Gets or sets Default Namespace.
        /// </summary>
        /// <remarks>IsDirty flag was switched to true.</remarks>
        [ResourcesCategoryAttribute(Resources.Application)]
        [Attributes.LocDisplayName(Resources.DefaultNamespace)]
        [ResourcesDescriptionAttribute(Resources.DefaultNamespaceDescription)]
        public string DefaultNamespace
        {
            get { return defaultNamespace; }
            set
            {
                defaultNamespace = value;
                IsDirty = true;
            }
        }


        /// <summary>
        /// Gets or sets Startup Object.
        /// </summary>
        /// <remarks>IsDirty flag was switched to true.</remarks>
        [ResourcesCategoryAttribute(Resources.Application)]
        [Attributes.LocDisplayName(Resources.StartupObject)]
        [ResourcesDescriptionAttribute(Resources.StartupObjectDescription)]
        public string StartupObject
        {
            get { return startupObject; }
            set
            {
                startupObject = value;
                IsDirty = true;
            }
        }


        /// <summary>
        /// Gets or sets Application Icon.
        /// </summary>
        /// <remarks>IsDirty flag was switched to true.</remarks>
        [ResourcesCategoryAttribute(Resources.Application)]
        [Attributes.LocDisplayName(Resources.ApplicationIcon)]
        [ResourcesDescriptionAttribute(Resources.ApplicationIconDescription)]
        public string ApplicationIcon
        {
            get { return applicationIcon; }
            set
            {
                applicationIcon = value;
                IsDirty = true;
            }
        }


        /// <summary>
        /// Gets the path to the project file.
        /// </summary>
        /// <remarks>IsDirty flag was switched to true.</remarks>
        [ResourcesCategoryAttribute(Resources.Project)]
        [Attributes.LocDisplayName(Resources.ProjectFile)]
        [ResourcesDescriptionAttribute(Resources.ProjectFileDescription)]
        public string ProjectFile
        {
            get { return Path.GetFileName(ProjectMgr.ProjectFile); }
        }


        /// <summary>
        /// Gets the path to the project folder.
        /// </summary>
        /// <remarks>IsDirty flag was switched to true.</remarks>
        [ResourcesCategoryAttribute(Resources.Project)]
        [Attributes.LocDisplayName(Resources.ProjectFolder)]
        [ResourcesDescriptionAttribute(Resources.ProjectFolderDescription)]
        public string ProjectFolder
        {
            get { return Path.GetDirectoryName(ProjectMgr.ProjectFolder); }
        }


        /// <summary>
        /// Gets the output file name depending on current OutputType.
        /// </summary>
        /// <remarks>IsDirty flag was switched to true.</remarks>
        [ResourcesCategoryAttribute(Resources.Project)]
        [Attributes.LocDisplayName(Resources.OutputFile)]
        [ResourcesDescriptionAttribute(Resources.OutputFileDescription)]
        public string OutputFile
        {
            get
            {
                switch (outputType)
                {
                    case OutputType.Exe:
                    case OutputType.WinExe:
                        {
                            return assemblyName + ".exe";
                        }

                    default:
                        {
                            return assemblyName + ".dll";
                        }
                }
            }
        }
        
        /// <summary>
        /// Returns class FullName property value.
        /// </summary>
        public override string GetClassName()
        {
            return GetType().FullName;
        }

        /// <summary>
        /// Bind properties.
        /// </summary>
        protected override void BindProperties()
        {
            if (ProjectMgr == null) return;

            assemblyName = ProjectMgr.GetProjectProperty("AssemblyName", true);

            string outputType = ProjectMgr.GetProjectProperty("OutputType", false);

            if (outputType != null && outputType.Length > 0)
            {
                try
                {
                    this.outputType = (OutputType) Enum.Parse(typeof (OutputType), outputType);
                }
                catch (ArgumentException) {}
            }

            defaultNamespace = ProjectMgr.GetProjectProperty("RootNamespace", false);
            startupObject = ProjectMgr.GetProjectProperty("StartupObject", false);
            applicationIcon = ProjectMgr.GetProjectProperty("ApplicationIcon", false);
        }

        /// <summary>
        /// Apply Changes on project node.
        /// </summary>
        /// <returns>E_INVALIDARG if internal ProjectMgr is null, otherwise applies changes and return S_OK.</returns>
        protected override int ApplyChanges()
        {
            if (ProjectMgr == null) return VSConstants.E_INVALIDARG;

            var propertyPageFrame = (IVsPropertyPageFrame) ProjectMgr.Site.GetService((typeof (SVsPropertyPageFrame)));
            
            bool reloadRequired = false;

            ProjectMgr.SetProjectProperty("AssemblyName", assemblyName);
            ProjectMgr.SetProjectProperty("OutputType", outputType.ToString());
            ProjectMgr.SetProjectProperty("RootNamespace", defaultNamespace);
            ProjectMgr.SetProjectProperty("StartupObject", startupObject);
            ProjectMgr.SetProjectProperty("ApplicationIcon", applicationIcon);

            // if (reloadRequired) if (MessageBox.Show(SR.GetString(SR.ReloadPromptOnTargetFxChanged), SR.GetString(SR.ReloadPromptOnTargetFxChangedCaption), MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes) ProjectMgr.TargetFrameworkMoniker = targetFrameworkMoniker;

            IsDirty = false;

            if (reloadRequired)
            {
                // This prevents the property page from displaying bad data from the zombied (unloaded) project
                propertyPageFrame.HideFrame();
                propertyPageFrame.ShowFrame(GetType().GUID);
            }

            return VSConstants.S_OK;
        }
    }
}