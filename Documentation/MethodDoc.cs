using System;
using System.Collections.Generic;
using System.Linq;

namespace XmlToSlateMD.Documentation
{
    public class MethodDoc : BaseDoc
    {
        public string Summary = "&lt;missing&gt;";

        public List<CodeExample> CodeExamples = new List<CodeExample>();

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
            var codes = String.Join(Environment.NewLine, (from example in CodeExamples select example.ToString()).ToArray()) + Environment.NewLine;
            return $"## {Name}{Environment.NewLine}{Environment.NewLine}" +
                $"{Summary}{Environment.NewLine}{Environment.NewLine}" +
                $"{codes}{Environment.NewLine}{Environment.NewLine}" +
                $"### Return type: {ReturnType.Format()}{Environment.NewLine}{Environment.NewLine}" +
                $"### Parameters:{Environment.NewLine}" +
                ParameterDoc.Header +
                $"{prms}";
        }
    }
}

