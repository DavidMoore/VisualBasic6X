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
using System.Diagnostics.CodeAnalysis;
using System.Runtime.InteropServices;
using EnvDTE;
using VSLangProj;

namespace Microsoft.VisualStudio.Project.Automation
{
	/// <summary>
	/// Represents a language-specific project item
	/// </summary>
	[SuppressMessage("Microsoft.Naming", "CA1709:IdentifiersShouldBeCasedCorrectly", MessageId = "OAVS")]
	[ComVisible(true), CLSCompliant(false)]
	public class OAVSProjectItem : VSProjectItem
	{
		#region fields
		private FileNode fileNode;
		#endregion

		#region ctors
		public OAVSProjectItem(FileNode fileNode)
		{
			this.FileNode = fileNode;
		}
		#endregion

		#region VSProjectItem Members

		public virtual EnvDTE.Project ContainingProject
		{
			get { return fileNode.ProjectMgr.GetAutomationObject() as EnvDTE.Project; }
		}

		public virtual ProjectItem ProjectItem
		{
			get { return fileNode.GetAutomationObject() as ProjectItem; }
		}

		public virtual DTE DTE
		{
			get { return (DTE)this.fileNode.ProjectMgr.Site.GetService(typeof(DTE)); }
		}

		public virtual void RunCustomTool()
		{
			this.FileNode.RunGenerator();
		}

		#endregion

		#region public properties
		/// <summary>
		/// File Node property
		/// </summary>
		public FileNode FileNode
		{
			get
			{
				return fileNode;
			}
			set
			{
				fileNode = value;
			}
		}
		#endregion

	}
}
