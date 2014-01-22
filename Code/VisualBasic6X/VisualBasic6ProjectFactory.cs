using System.Runtime.InteropServices;
using Microsoft.VisualStudio.OLE.Interop;
using Microsoft.VisualStudio.Project;

namespace VisualBasic6
{
	/// <summary>
	/// Represent the methods for creating projects within the solution.
	/// </summary>
	[Guid(GuidStrings.ProjectFactory)]
	public class VisualBasic6ProjectFactory : ProjectFactory
	{
	    private readonly VisualBasic6ProjectPackage package;

	    /// <summary>
		/// Explicit default constructor.
		/// </summary>
		/// <param name="package">Value of the project package for initialize internal package field.</param>
		public VisualBasic6ProjectFactory(VisualBasic6ProjectPackage package) : base(package)
		{
			this.package = package;
		}

	    /// <summary>
		/// Creates a new project by cloning an existing template project.
		/// </summary>
		/// <returns></returns>
		protected override ProjectNode CreateProject()
		{
			var project = new VisualBasic6ProjectNode(package);
			project.SetSite((IServiceProvider)((System.IServiceProvider)package).GetService(typeof(IServiceProvider)));
			return project;
		}
	}
}