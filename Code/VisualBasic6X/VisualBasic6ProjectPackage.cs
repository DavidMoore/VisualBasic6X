using System.Runtime.InteropServices;
using Microsoft.VisualStudio.Project;
using Microsoft.VisualStudio.Shell;

namespace VisualBasic6
{
    /// <summary>
    /// This class implements the package exposed by this assembly.
    /// </summary>
    /// <remarks>
    /// <para>A Visual Studio component can be registered under different registry roots; for instance
    /// when you debug your package you want to register it in the experimental hive. This
    /// attribute specifies the registry root to use if no one is provided to regpkg.exe with
    /// the /root switch.</para>
    /// <para>A description of the different attributes used here is given below:</para>
    /// <para>DefaultRegistryRoot: This defines the default registry root for registering the package. 
    /// We are currently using the experimental hive.</para>
    /// <para>ProvideObject: Declares that a package provides creatable objects of specified type.</para> 
    /// <para>ProvideProjectFactory: Declares that a package provides a project factory.</para>
    /// <para>ProvideProjectItem: Declares that a package provides a project item.</para> 
    /// </remarks>  
    [PackageRegistration(UseManagedResourcesOnly = true)]
    [DefaultRegistryRoot("Software\\Microsoft\\VisualStudio\\11.0")]
    [ProvideObject(typeof(GeneralPropertyPage))]
    [ProvideProjectFactory(typeof(VisualBasic6ProjectFactory), ProjectFactoryStrings.ProjectName, ProjectFactoryStrings.ProjectFileFilter,
        ProjectFactoryStrings.DefaultExtension, ProjectFactoryStrings.PossibleExtensions,ProjectFactoryStrings.TemplateDirectory, 
        LanguageVsTemplate = ProjectFactoryStrings.LanguageVsTemplate, NewProjectRequireNewFolderVsTemplate = false,
        DisplayProjectTypeVsTemplate = "Visual Basic 6 Project Type", ProjectSubTypeVsTemplate = "Visual Basic 6 Subtype",DisableOnlineTemplates = true)]
    [ProvideProjectItem(typeof(VisualBasic6ProjectFactory), "Visual Basic 6", @"Templates\ProjectItems", 500)]
    [Guid(GuidStrings.ProjectPackage)]
    [ProvideMSBuildTargets("VisualBasic6CompilerTasks", @"$PackageFolder$\VisualBasic6.targets")]
    public sealed class VisualBasic6ProjectPackage : ProjectPackage
    {
        /// <summary>
        /// Initialization of the package; this method is called right after the package is sited, so this is the place
        /// where you can put all the initilaization code that rely on services provided by VisualStudio.
        /// </summary>
        protected override void Initialize()
        {
            base.Initialize();
            this.RegisterProjectFactory(new VisualBasic6ProjectFactory(this));
        }
        
        public override string ProductUserContext
        {
            get { return "Visual Basic 6"; }
        }
    }
}