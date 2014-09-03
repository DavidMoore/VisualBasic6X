namespace VisualBasic6X.Converter.Console.VisualBasic6
{
    using System;
    using System.Globalization;

    public class VB6Reference
    {
        public VB6Reference(Guid guid, string version, string fileName, string description)
        {
            Guid = guid;
            Version = version;
            FileName = fileName;
            Description = description;

            // Split the version into parts
            var versionParts = version.Split(new[]{"."},StringSplitOptions.RemoveEmptyEntries);

            // The version numbers are hex in the VB6 project file
            VersionMajor = int.Parse(versionParts[0], NumberStyles.HexNumber);
            VersionMinor = int.Parse(versionParts[1], NumberStyles.HexNumber);
        }

        public Guid Guid { get; private set; }
        public string Version { get; private set; }
        public int VersionMajor { get; private set; }
        public int VersionMinor { get; private set; }
        public string FileName { get; private set; }
        public string Description { get; private set; }

        public string ToString(VB6ReferenceType referenceType)
        {
            switch (referenceType)
            {
                case VB6ReferenceType.Reference:
                    return string.Format("Reference=*\\G{0}#{1}#0#{2}#{3}", Guid, Version, FileName, Description);
                case VB6ReferenceType.Component:
                    return string.Format("Object={0}#{1}#0#;{2}", Guid, Version, FileName);
                default:
                    throw new ArgumentOutOfRangeException("referenceType");
            }
        }
    }
}