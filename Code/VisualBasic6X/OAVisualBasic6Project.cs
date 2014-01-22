using System.Runtime.InteropServices;
using Microsoft.VisualStudio.Project.Automation;

namespace VisualBasic6
{
	[ComVisible(true)]
	public class OAVisualBasic6Project : OAProject
	{
	    /// <summary>
		/// Public constructor.
		/// </summary>
		/// <param name="project">Custom project.</param>
		public OAVisualBasic6Project(VisualBasic6ProjectNode project) : base(project) {}
	}
}