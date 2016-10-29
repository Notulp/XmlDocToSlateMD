using System;

namespace XmlToSlateMD
{
    public class PropertyDoc : BaseDoc
    {
        public string Summary = "<missing>";

		public string Type = "<missing>";

        public bool GetterOnly = false;

		public PropertyDoc(TypeDoc parent) : base(parent)
		{
		}

        public override string ToString()
        {
			string getter = (GetterOnly ? "getter" : "getter/setter");
			return $"{Name} | {Type} | {Summary} | {getter}";
        }
	}
}

