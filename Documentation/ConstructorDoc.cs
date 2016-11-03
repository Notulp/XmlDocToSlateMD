using System;
using System.Collections.Generic;
using System.Linq;

namespace XmlToSlateMD.Documentation
{
    public class ConstructorDoc : BaseDoc
    {
        public string Summary = "&lt;missing&gt;";

        public List<ParameterDoc> Params = new List<ParameterDoc>();

        public Type ReturnType = typeof(void);

        public ConstructorDoc(TypeDoc parent)
            : base(parent)
        {
        }

        public override void RegisterChild(BaseDoc child)
        {
            Params.Add(child as ParameterDoc);
        }

        public override string ToString()
        {
            var prms = String.Join(Environment.NewLine, (from param in Params select param.ToString()).ToArray());
            return $"## {Name}{Environment.NewLine}{Environment.NewLine}" +
                $"{Summary}{Environment.NewLine}{Environment.NewLine}" +
                $"### Parameters:{Environment.NewLine}" +
                ParameterDoc.Header+
                $"{prms}";
        }
    }
}

