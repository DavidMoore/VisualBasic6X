using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Foundation.Build.VisualBasic6.VisualStudioIntegration.ProjectBase
{
    /// <summary>
    /// Defines the project load options from the ProjectSecurity dialog box
    /// </summary>
    public enum ProjectLoadOption
    {
        /// <summary>
        /// Load the project normally
        /// </summary>
        LoadNormally = 0,

        /// <summary>
        /// Load the project only for browsing
        /// </summary>
        LoadOnlyForBrowsing = 1,

        /// <summary>
        /// Do not load the project.
        /// </summary>
        DonNotLoad = 2
    };
}
