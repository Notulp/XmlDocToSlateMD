using System;

namespace XmlToSlateMD.Documentation
{
    public class FieldDoc : BaseDoc
    {
        public string defaultValue = "<null>";

        public string Summary = "<missing>";

        public string Type = "<missing>";

        public FieldDoc(TypeDoc parent)
            : base(parent)
        {
        }

        public override string ToString()
        {
            return $"{Name} | {Type} | {Summary}";
        }
    }
}