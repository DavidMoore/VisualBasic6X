namespace VisualBasic6X.Tests
{
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using Converter.Console;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using VisualBasic6X.Tests.Helpers;

    [TestClass]
    public class Read_method_on_VB6ProjectReader
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
        public void Returns_dictionary_mapping_key_to_multiple_values()
        {
            var results = reader.Read(tempFile.FullName);

            Assert.IsNotNull(results);
            IList<string> forms;
            Assert.IsTrue(results.TryGetValue("form", out forms));
            Assert.IsNotNull(forms);
            Assert.AreEqual(2, forms.Count());
        }
    }
}