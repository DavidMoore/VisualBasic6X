/* ****************************************************************************
 *
 * Copyright (c) Microsoft Corporation. 
 *
 * This source code is subject to terms and conditions of the Apache License, Version 2.0. A 
 * copy of the license can be found in the License.html file at the root of this distribution. If 
 * you cannot locate the Apache License, Version 2.0, please send an email to 
 * ironpy@microsoft.com. By using this source code in any fashion, you are agreeing to be bound 
 * by the terms of the Apache License, Version 2.0.
 *
 * You must not remove this notice, or any other, from this software.
 *
 * ***************************************************************************/

using System;
using System.Diagnostics;
using System.Globalization;
using System.Runtime.InteropServices;
using Microsoft.VisualStudio;
using Microsoft.VisualStudio.Shell.Interop;

namespace Microsoft.VisualStudio.Project.Automation
{
	[ComVisible(true), CLSCompliant(false)]
	public class OASolutionFolder<T> : EnvDTE80.SolutionFolder
		where T : HierarchyNode
	{
		bool hidden = false;
		T node;

		public OASolutionFolder(T associatedNode)
		{
			if(associatedNode == null)
			{
				throw new ArgumentNullException("associatedNode");
			}

			Debug.Assert(associatedNode.ProjectMgr is ProjectContainerNode, "Expecting obejct of type" + typeof(ProjectContainerNode).Name);

			if(!(associatedNode.ProjectMgr is ProjectContainerNode))
				throw new ArgumentException(SR.GetString(SR.InvalidParameter, CultureInfo.CurrentUICulture), "associatedNode");

			this.node = associatedNode;
		}


		#region SolutionFolder Members

		public virtual EnvDTE.Project AddFromFile(string fileName)
		{
			ProjectContainerNode projectContainer = (ProjectContainerNode)this.node.ProjectMgr;
			ProjectElement newElement = new ProjectElement(projectContainer, fileName, ProjectFileConstants.SubProject);
			NestedProjectNode newNode = projectContainer.AddExistingNestedProject(newElement, __VSCREATEPROJFLAGS.CPF_NOTINSLNEXPLR | __VSCREATEPROJFLAGS.CPF_SILENT | __VSCREATEPROJFLAGS.CPF_OPENFILE);
			if(newNode == null)
				return null;
			// Now that the sub project was created, get its extensibility object so we can return it
			object newProject = null;
			if(ErrorHandler.Succeeded(newNode.NestedHierarchy.GetProperty(VSConstants.VSITEMID_ROOT, (int)__VSHPROPID.VSHPROPID_ExtObject, out newProject)))
				return newProject as EnvDTE.Project;
			else
				return null;
		}

		public virtual EnvDTE.Project AddFromTemplate(string fileName, string destination, string projectName)
		{
			bool isVSTemplate = Utilities.IsTemplateFile(fileName);

			NestedProjectNode newNode = null;
			if(isVSTemplate)
			{
				// Get the wizard to run, we will get called again and use the alternate code path
				ProjectElement newElement = new ProjectElement(this.node.ProjectMgr, System.IO.Path.Combine(destination, projectName), ProjectFileConstants.SubProject);
				newElement.SetMetadata(ProjectFileConstants.Template, fileName);
				((ProjectContainerNode)this.node.ProjectMgr).RunVsTemplateWizard(newElement, false);
			}
			else
			{
				if((String.IsNullOrEmpty(System.IO.Path.GetExtension(projectName))))
				{
					string targetExtension = System.IO.Path.GetExtension(fileName);
					projectName = System.IO.Path.ChangeExtension(projectName, targetExtension);
				}

				ProjectContainerNode projectContainer = (ProjectContainerNode)this.node.ProjectMgr;
				newNode = projectContainer.AddNestedProjectFromTemplate(fileName, destination, projectName, null, __VSCREATEPROJFLAGS.CPF_NOTINSLNEXPLR | __VSCREATEPROJFLAGS.CPF_SILENT | __VSCREATEPROJFLAGS.CPF_CLONEFILE);
			}
			if(newNode == null)
				return null;

			// Now that the sub project was created, get its extensibility object so we can return it
			object newProject = null;
			if(ErrorHandler.Succeeded(newNode.NestedHierarchy.GetProperty(VSConstants.VSITEMID_ROOT, (int)__VSHPROPID.VSHPROPID_ExtObject, out newProject)))
				return newProject as EnvDTE.Project;
			else
				return null;
		}

		public virtual EnvDTE.Project AddSolutionFolder(string Name)
		{
			throw new NotImplementedException();
		}

		public virtual EnvDTE.Project Parent
		{
			get
			{
				throw new NotImplementedException();
			}
		}

		public virtual bool Hidden
		{
			get
			{
				return hidden;
			}
			set
			{
				hidden = value;
			}
		}

		public virtual EnvDTE.DTE DTE
		{
			get
			{
				return (EnvDTE.DTE)this.node.ProjectMgr.Site.GetService(typeof(EnvDTE.DTE));
			}
		}

		#endregion
	}

}
