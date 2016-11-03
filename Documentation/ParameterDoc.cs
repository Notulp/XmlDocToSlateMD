using System;

namespace XmlToSlateMD.Documentation
{
    public class ParameterDoc : BaseDoc
    {
        public string defaultValue = "&lt;null&gt;";

        public string Summary = "&lt;missing&gt;";

        public Type Type = typeof(object);

        public static string Header = $"Name | Type | Default value | Summary{Environment.NewLine}" +
                                      $"--- | --- | --- | ---{Environment.NewLine}";

        public ParameterDoc(BaseDoc parent)
            : base(parent)
        {
        }

        public override string ToString()
        {
            return $"{Name} | {Type.Format()} | {defaultValue} | {Summary}";
        }
    }
}

