using System;
using System.Collections.Generic;
using System.Linq;

namespace XmlToSlateMD.Documentation
{
    public class MethodDoc : BaseDoc
    {
        public string Summary = "<missing>";

        public List<ParameterDoc> Params = new List<ParameterDoc>();

        public Type ReturnType = typeof(void);

        public MethodDoc(TypeDoc parent)
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
            return $"## {Name}{Environment.NewLine}{Environment.NewLine}{Summary}{Environment.NewLine}{Environment.NewLine}### RETURN TYPE: {ReturnType.Name}{Environment.NewLine}{Environment.NewLine}{prms}";
        }
    }
}

