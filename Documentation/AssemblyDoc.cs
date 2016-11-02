using System;
using System.IO;
using System.Collections.Generic;

namespace XmlToSlateMD.Documentation
{
    public class AssemblyDoc : BaseDoc
    {
        public List<TypeDoc> Types = new List<TypeDoc>();

        public AssemblyDoc(BaseDoc parent = null)
            : base(parent)
        {
        }

        public override void RegisterChild(BaseDoc child)
        {
            Types.Add(child as TypeDoc);
        }

        public void ToFile()
        {
            File.WriteAllText(Path.Combine(Environment.CurrentDirectory, "_" + Name.Replace('.', '_').ToLower() + ".md"), ToString());
        }

        public override string ToString()
        {
            string result = "";
            foreach (var type in Types) {
                result += type + Environment.NewLine;
            }
            return result;
        }
    }
}

