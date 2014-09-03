namespace VisualBasic6X.Converter.Console.VisualBasic6
{
    using System.Collections.Generic;

    public class VB6Project
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="T:System.Object"/> class.
        /// </summary>
        public VB6Project()
        {
            SourceFiles = new List<VB6SourceFile>();
            References = new List<VB6Reference>();
            Components = new List<VB6Reference>();
        }

        /// <summary>
        /// Gets or sets the name of the project.
        /// </summary>
        /// <value>
        /// The project name.
        /// </value>
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the type of the project.
        /// </summary>
        /// <value>
        /// The type of the project.
        /// </value>
        public VB6ProjectType ProjectType { get; set; }

        /// <summary>
        /// Gets the collection of COM references.
        /// </summary>
        public IList<VB6Reference> References { get; private set; }

        /// <summary>
        /// Gets the collection of components (ActiveX Controls / OCX).
        /// </summary>
        public IList<VB6Reference> Components { get; private set; }

        /// <summary>
        /// Gets the collection of source files.
        /// </summary>
        public IList<VB6SourceFile> SourceFiles { get; private set; }

        /// <summary>
        /// Gets or sets the name of the startup item.
        /// </summary>
        /// <value>
        /// The name of the startup item.
        /// </value>
        public string Startup { get; set; }

        /// <summary>
        /// Gets or sets the name of the form providing the program icon (if applicable).
        /// </summary>
        /// <value>
        /// The name of the form providing the program icon.
        /// </value>
        public string IconForm { get; set; }

        public string CompatibilityFile { get; set; }

        public string CompatibilityMode { get; set; }

        /// <summary>
        /// Gets or sets the path to the resource file (if any).
        /// </summary>
        /// <value>
        /// The optional resource filename.
        /// </value>
        public string ResourceFile { get; set; }

        /// <summary>
        /// Gets or sets the filename.
        /// </summary>
        /// <value>
        /// The filename.
        /// </value>
        public string FileName { get; set; }
    }
}