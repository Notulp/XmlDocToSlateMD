using System;

namespace XmlToSlateMD.Documentation
{
    public class FieldDoc : BaseDoc
    {
        public string defaultValue = "&lt;null&gt;";

        public static string Header = $"Name | Type | Summary{Environment.NewLine}" +
                                      $"--- | --- | ---{Environment.NewLine}";

        public string Summary = "&lt;missing&gt;";

        public Type Type = typeof(object);

        public FieldDoc(TypeDoc parent)
            : base(parent)
        {
        }

        public override string ToString()
        {
            return $"{Name} | {Type.Format()} | {Summary}";
        }
    }
}