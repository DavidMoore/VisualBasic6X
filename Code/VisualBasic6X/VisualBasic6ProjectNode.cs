﻿using System;
using System.Drawing;
using System.IO;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using EnvDTE;
using Microsoft.VisualStudio.Project;
using VSLangProj;
using Microsoft.VisualStudio.Project.Automation;

namespace VisualBasic6
{
    /// <summary>
    /// This class extends the ProjectNode in order to represent our project 
    /// within the hierarchy.
    /// </summary>
    [Guid(GuidStrings.ProjectNode)]
    public class VisualBasic6ProjectNode : ProjectNode
    {
        internal enum VisualBasic6ProjectImageName
        {
            Project = 0,
        }

        internal const string ProjectTypeName = "Visual Basic 6 Project";

        private VisualBasic6ProjectPackage package;
        internal static int imageOffset;
        private static ImageList imageList;
        private VSLangProj.VSProject vsProject;

        /// <summary>
        /// Initializes the <see cref="VisualBasic6ProjectNode"/> class.
        /// </summary>
        static VisualBasic6ProjectNode()
        {
            var assembly = typeof (VisualBasic6ProjectNode).Assembly;
            imageList = Utilities.GetImageList(assembly.GetManifestResourceStream(assembly.GetName().Name + ".Resources.Project16x16.bmp"));
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="VisualBasic6ProjectNode"/> class.
        /// </summary>
        /// <param name="package">Value of the project package for initialize internal package field.</param>
        public VisualBasic6ProjectNode(VisualBasic6ProjectPackage package)
        {
            this.package = package;

            InitializeImageList();

            CanProjectDeleteItems = true;
            CanFileNodesHaveChilds = true;
        }

        /// <summary>
        /// Gets or sets the image list.
        /// </summary>
        /// <value>The image list.</value>
        public static ImageList ImageList
        {
            get
            {
                return imageList;
            }
            set
            {
                imageList = value;
            }
        }

        protected internal VSLangProj.VSProject VSProject
        {
            get
            {
                if(vsProject == null)
                {
                    vsProject = new OAVSProject(this);
                }

                return vsProject;
            }
        }

        /// <summary>
        /// Gets the project GUID.
        /// </summary>
        /// <value>The project GUID.</value>
        public override Guid ProjectGuid
        {
            get { return typeof(VisualBasic6ProjectFactory).GUID; }
        }

        /// <summary>
        /// Gets the type of the project.
        /// </summary>
        /// <value>The type of the project.</value>
        public override string ProjectType
        {
            get { return ProjectTypeName; }
        }

        /// <summary>
        /// Return an imageindex
        /// </summary>
        /// <value></value>
        /// <returns></returns>
        public override int ImageIndex
        {
            get
            {
                return imageOffset + (int)VisualBasic6ProjectImageName.Project;
            }
        }

        /// <summary>
        /// Returns an automation object representing this node
        /// </summary>
        /// <returns>The automation object</returns>
        public override object GetAutomationObject()
        {
            return new OAVisualBasic6Project(this);
        }

        /// <summary>
        /// Creates the file node.
        /// </summary>
        /// <param name="item">The project element item.</param>
        /// <returns></returns>
        public override FileNode CreateFileNode(ProjectElement item)
        {
            VisualBasic6FileNode node = new VisualBasic6FileNode(this, item);

            node.OleServiceProvider.AddService(typeof(EnvDTE.Project), new OleServiceProvider.ServiceCreatorCallback(this.CreateServices), false);
            node.OleServiceProvider.AddService(typeof(ProjectItem), node.ServiceCreator, false);
            node.OleServiceProvider.AddService(typeof(VSProject), new OleServiceProvider.ServiceCreatorCallback(this.CreateServices), false);

            return node;
        }

        /// <summary>
        /// Generate new Guid value and update it with GeneralPropertyPage GUID.
        /// </summary>
        /// <returns>Returns the property pages that are independent of configuration.</returns>
        protected override Guid[] GetConfigurationIndependentPropertyPages()
        {
            Guid[] result = new Guid[1];
            result[0] = typeof(GeneralPropertyPage).GUID;
            return result;
        }

        /// <summary>
        /// Overriding to provide project general property page.
        /// </summary>
        /// <returns>Returns the GeneralPropertyPage GUID value.</returns>
        protected override Guid[] GetPriorityProjectDesignerPages()
        {
            Guid[] result = new Guid[1];
            result[0] = typeof(GeneralPropertyPage).GUID;
            return result;
        }

        /// <summary>
        /// Adds the file from template.
        /// </summary>
        /// <param name="source">The source template.</param>
        /// <param name="target">The target file.</param>
        public override void AddFileFromTemplate(string source, string target)
        {
            if(!File.Exists(source))
            {
                throw new FileNotFoundException(string.Format("Template file not found: {0}", source));
            }

            // The class name is based on the new file name
            string className = Path.GetFileNameWithoutExtension(target);
            string namespce = this.FileTemplateProcessor.GetFileNamespace(target, this);

            this.FileTemplateProcessor.AddReplace("%className%", className);
            this.FileTemplateProcessor.AddReplace("%namespace%", namespce);
            try
            {
                this.FileTemplateProcessor.UntokenFile(source, target);

                this.FileTemplateProcessor.Reset();
            }
            catch(Exception e)
            {
                throw new FileLoadException("Failed to add template file to project", target, e);
            }
        }

        private void InitializeImageList()
        {
            imageOffset = this.ImageHandler.ImageList.Images.Count;

            foreach(Image img in ImageList.Images)
            {
                this.ImageHandler.AddImage(img);
            }
        }

        private object CreateServices(Type serviceType)
        {
            object service = null;
            if(typeof(VSLangProj.VSProject) == serviceType)
            {
                service = this.VSProject;
            }
            else if(typeof(EnvDTE.Project) == serviceType)
            {
                service = this.GetAutomationObject();
            }
            return service;
        }
    }
}