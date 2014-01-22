using System.Runtime.InteropServices;
using Microsoft.VisualStudio.Project;
using Microsoft.VisualStudio.Project.Automation;

namespace VisualBasic6
{
    [ComVisible(true)]
    [Guid(GuidStrings.AutomationFileItem)]
    public class OAVisualBasic6FileItem : OAFileItem
    {
        /// <summary>
        /// Public constructor.
        /// </summary>
        /// <param name="project">Automation project.</param>
        /// <param name="node">Custom file node.</param>
        public OAVisualBasic6FileItem(OAProject project, FileNode node) : base(project, node) {}
    }
}