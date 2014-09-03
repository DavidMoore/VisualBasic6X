namespace VisualBasic6X.Tests
{
    using System;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using VisualBasic6X.Converter.Console.VisualBasic6;

    [TestClass]
    public class GetReference_method_on_VB6ProjectReader
    {
        [TestMethod]
        public void Returns_component_reference_with_description_set_to_filename_without_extension()
        {
            var reference = VB6ProjectReader.ParseReference("{F9043C88-F6F2-101A-A3C9-08002B2F49FB}#1.2#0; ComDlg32.ocx");

            Assert.IsNotNull(reference);
            Assert.AreEqual(new Guid("{F9043C88-F6F2-101A-A3C9-08002B2F49FB}"), reference.Guid);
            Assert.AreEqual(1, reference.VersionMajor);
            Assert.AreEqual(2, reference.VersionMinor);
            Assert.AreEqual("ComDlg32.ocx", reference.FileName);
            Assert.AreEqual("ComDlg32", reference.Description);
        }

        [TestMethod]
        public void Returns_dll_reference()
        {
            var reference = VB6ProjectReader.ParseReference(@"*\G{00020430-0000-0000-C000-000000000046}#2.0#0#C:\Windows\SysWOW64\stdole2.tlb#OLE Automation");

            Assert.IsNotNull(reference);
            Assert.AreEqual(new Guid("{00020430-0000-0000-C000-000000000046}"), reference.Guid);
            Assert.AreEqual(2, reference.VersionMajor);
            Assert.AreEqual(0, reference.VersionMinor);
            Assert.AreEqual(@"C:\Windows\SysWOW64\stdole2.tlb", reference.FileName);
            Assert.AreEqual("OLE Automation", reference.Description);
        }
    }
}
