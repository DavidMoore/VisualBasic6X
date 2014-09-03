namespace VisualBasic6X.Tests
{
    using System.IO;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using VisualBasic6X.Converter.Console.VisualBasic6;
    using VisualBasic6X.Tests.Helpers;

    [TestClass]
    public class Parse_method_on_VB6ProjectReader
    {
        private FileInfo tempFile;
        private VB6ProjectReader reader;

        [TestInitialize]
        public void Initialize()
        {
            tempFile = new FileInfo(Path.GetTempFileName());
            tempFile.WriteEmbeddedResourceToFile("Project.vbp");
            reader = new VB6ProjectReader();
        }

        [TestCleanup]
        public void Cleanup()
        {
            tempFile.Refresh();
            if (tempFile.Exists) tempFile.Delete();
        }

        [TestMethod]
        public void Returns_VB6Project_object()
        {
            VB6Project project = reader.Parse(tempFile.FullName);
            Assert.IsNotNull(project);
        }
    }
}
