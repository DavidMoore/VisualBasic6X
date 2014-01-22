using System;
using Microsoft.VisualStudio.Project;
using Microsoft.VisualStudio.Project.Automation;

namespace VisualBasic6
{
	/// <summary>
	/// This class extends the FileNode in order to represent a file 
	/// within the hierarchy.
	/// </summary>
	public class VisualBasic6FileNode : FileNode
	{
	    private OAVisualBasic6FileItem automationObject;

	    /// <summary>
		/// Initializes a new instance of the <see cref="VisualBasic6FileNode"/> class.
		/// </summary>
		/// <param name="root">The project node.</param>
		/// <param name="e">The project element node.</param>
		internal VisualBasic6FileNode(ProjectNode root, ProjectElement e) : base(root, e) {}

	    /// <summary>
		/// Gets the automation object for the file node.
		/// </summary>
		/// <returns></returns>
		public override object GetAutomationObject()
		{
			if(automationObject == null)
			{
				automationObject = new OAVisualBasic6FileItem(ProjectMgr.GetAutomationObject() as OAProject, this);
			}

			return automationObject;
		}

	    internal OleServiceProvider.ServiceCreatorCallback ServiceCreator
		{
			get { return CreateServices; }
		}

		private object CreateServices(Type serviceType)
		{
			object service = null;
			if(typeof(EnvDTE.ProjectItem) == serviceType)
			{
				service = GetAutomationObject();
			}
			return service;
		}
	}
}