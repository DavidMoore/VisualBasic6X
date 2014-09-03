namespace VisualBasic6X.Tests.Helpers
{
    using System.IO;
    using System.Reflection;

    internal static class ResourceHelper
    {
        internal static void WriteEmbeddedResourceToFile(this FileInfo file, string resourceName)
        {
            using (var stream = file.OpenWrite())
            using (var resourceStream = Assembly.GetExecutingAssembly().GetManifestResourceStream("VisualBasic6X.Tests.Resources." + resourceName))
            {
                resourceStream.CopyTo(stream);
            }
        }
    }
}