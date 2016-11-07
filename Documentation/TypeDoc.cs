using System;
using System.Collections.Generic;
using System.Linq;

namespace XmlDocToSlateMD.Documentation
{
    public class TypeDoc : BaseDoc
    {
        public string Summary = "&lt;missing&gt;";

        public List<CodeExample> CodeExamples = new List<CodeExample>();

        public List<ConstructorDoc> Constructors = new List<ConstructorDoc>();

        public List<MethodDoc> Methods = new List<MethodDoc>();

        public List<FieldDoc> Fields = new List<FieldDoc>();

        public List<PropertyDoc> Properties = new List<PropertyDoc>();

        public TypeDoc(AssemblyDoc parent)
            : base(parent)
        {
        }

        public override void RegisterChild(BaseDoc child)
        {
            if (child is ConstructorDoc) {
                Constructors.Add(child as ConstructorDoc);
            } else if (child is MethodDoc) {
                Methods.Add(child as MethodDoc);
            } else if (child is FieldDoc) {
                Fields.Add(child as FieldDoc);
            } else if (child is PropertyDoc) {
                Properties.Add(child as PropertyDoc);
            }
        }

        public override string ToString()
        {
            string result = $"# {Name}{Environment.NewLine}";
            result += String.Join(Environment.NewLine, (from example in CodeExamples select example.ToString()).ToArray()) + Environment.NewLine;
            result += String.Join(Environment.NewLine, (from constructor in Constructors select constructor.ToString()).ToArray()) + Environment.NewLine;
            result += String.Join(Environment.NewLine, (from method in Methods select method.ToString()).ToArray()) + Environment.NewLine;
            if (Fields.Count > 0) {
                result += Environment.NewLine + "### Field" + (Fields.Count == 1 ? "" : "s") + ":" + Environment.NewLine + Environment.NewLine;
                result += FieldDoc.Header;
            }
            result += String.Join(Environment.NewLine, (from field in Fields select field.ToString()).ToArray()) + Environment.NewLine + Environment.NewLine;
            if (Properties.Count > 0) {
                result += Environment.NewLine + "### Porpert" + (Properties.Count == 1 ? "y" : "ies") + ":" + Environment.NewLine + Environment.NewLine;
                result += ParameterDoc.Header;
            }
            result += String.Join(Environment.NewLine, (from property in Properties select property.ToString()).ToArray()) + Environment.NewLine;
            return result;
        }
    }
}

