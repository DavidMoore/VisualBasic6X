namespace VisualBasic6X.Tests
{
    using System;
    using System.IO;
    using Converter.Console;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using VisualBasic6X.Tests.Helpers;

    [TestClass]
    public class When_calling_Convert_method_on_ProjectConverter
    {
        private ProjectConverter converter;
        private DirectoryInfo directory;
        private FileInfo tempFile;

        [TestInitialize]
        public void Initialize()
        {
            converter = new ProjectConverter();

            // Create a temporary directory
            directory = new DirectoryInfo(Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString()));
            directory.Create();

            // Create the VB6 project in the temp directory
            tempFile = new FileInfo(Path.Combine(directory.FullName, "Project.vbp"));
            tempFile.WriteEmbeddedResourceToFile("Project.vbp");
        }

        [TestCleanup]
        public void Cleanup()
        {
            if (directory != null)
            {
                directory.Refresh();
                if (directory.Exists) directory.Delete(true);
            }
        }
        
        [TestMethod, ExpectedException(typeof(ArgumentNullException))]
        public void Filename_must_not_be_null()
        {
            converter.Convert(null);
        }
    }
}