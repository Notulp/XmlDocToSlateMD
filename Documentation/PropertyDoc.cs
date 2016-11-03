using System;

namespace XmlToSlateMD.Documentation
{
    public class PropertyDoc : BaseDoc
    {
        public string defaultValue = "&lt;null&gt;";

        public static string Header = $"Name | Type | Summary | Getter/setter{Environment.NewLine}" +
                                      $"--- | --- | --- | ---{Environment.NewLine}";

        public string Summary = "&lt;missing&gt;";

        public Type Type = typeof(object);

        public bool GetterOnly = false;

        public PropertyDoc(TypeDoc parent)
            : base(parent)
        {
        }

        public override string ToString()
        {
            string getter = (GetterOnly ? "getter" : "getter/setter");
            return $"{Name} | {Type.Format()} | {Summary} | {getter}";
        }
    }
}

