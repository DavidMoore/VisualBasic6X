namespace VisualBasic6X.Converter.Console
{
    using System;

    internal class VisualBasicReference
    {
        public VisualBasicReference(Guid guid, string version, string filename, string description)
        {
            Guid = guid;
            Version = version;
            Filename = filename;
            Description = description;
        }

        public Guid Guid { get; private set; }
        public string Version { get; private set; }
        public string Filename { get; private set; }
        public string Description { get; private set; }

        public string ToString(ReferenceType referenceType)
        {
            switch (referenceType)
            {
                case ReferenceType.Reference:
                    return string.Format("Reference=*\\G{0}#{1}#0#{2}#{3}", Guid, Version, Filename, Description);
                case ReferenceType.Component:
                    return string.Format("Object={0}#{1}#0#;{2}", Guid, Version, Filename);
                default:
                    throw new ArgumentOutOfRangeException("referenceType");
            }
        }
    }
}