namespace VisualBasic6X.Tests
{
    using System;
    using Converter.Console;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    [TestClass]
    public class VB6ProjectReader_ParseProjectLine
    {
        [TestMethod]
        public void Includes_multiple_equals_signs_in_value()
        {
            var result = VB6ProjectReader.ParseProjectLine("test=value=includes_this");
            Assert.AreEqual("test", result.Key);
            Assert.AreEqual("value=includes_this", result.Value);
        }
        
        [TestMethod]
        public void Converts_key_to_lowercase()
        {
            var result = VB6ProjectReader.ParseProjectLine("TeSt=value=includes_this");
            Assert.AreEqual("test", result.Key);
        }
    }
}