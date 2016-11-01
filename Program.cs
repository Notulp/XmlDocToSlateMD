using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.IO;
using System.Linq;
using System.Xml;

using XmlToSlateMD.Documentation;

namespace XmlToSlateMD
{
    class MainClass
    {
        static AssemblyDoc CurrentAssembly;

        static BaseDoc CurrentType;

        static BaseDoc CurrentMethod;

        static BaseDoc PreviousDoc;

        public static void Main(string[] args)
        {
            foreach (var file in Directory.GetFiles(Environment.CurrentDirectory, "*.xml")) {
                string xmlStr = File.ReadAllText(file);

                xmlStr = Regex.Replace(xmlStr,
                                       "<see cref=\"[A-Z?]:(.*)\" \\/>",
                                       m => "&lt;a href=\"#" +
                                           m.Groups[1].Value.ToLower().Replace('.', '_') + "\"&gt;" +
                                           m.Groups[1].Value.Split('.').Last() +
                                           "&lt;/a&gt;"
                                      );

                xmlStr = Regex.Replace(xmlStr,
                                       @"<(c|code)>([^<])*<\/(c|code)>",
                                       m => "&lt;" +
                                           m.Groups[1].Value +
                                           "&gt;" +
                                           m.Groups[2].Captures.Join() +
                                           "&lt;/" +
                                           m.Groups[1].Value +
                                           "&gt;"
                                      );

                using (var stream = xmlStr.ToStream()) {
                    // TODO: Add reflection stuff, to:
                    // * get return types
                    // * types of parameters
                    // * types of fields/properties
                    // * determine if properties has setters or only getters

                    using (var xml = System.Xml.XmlReader.Create(stream)) {
                        while (xml.Read()) {
                            if (xml.IsStartElement()) {
                                BaseDoc CurrentDoc = null;

                                Console.WriteLine(xml.Name);

                                switch (xml.Name) {
                                    case "assembly":
                                        CurrentAssembly = new AssemblyDoc();
                                        CurrentDoc = CurrentAssembly;
                                        break;
                                        
                                    case "name":
                                        xml.Read();
                                        CurrentAssembly.Name = xml.Value;
                                        break;
                                        
                                    case "member":
                                        var memberName = xml["name"];
                                        char type = memberName[0];
                                        switch (type) {
                                            case 'T':
                                                CurrentDoc = new TypeDoc(CurrentAssembly);
                                                CurrentType = CurrentDoc;
                                                break;
                                            case 'P':
                                                CurrentDoc = new PropertyDoc(CurrentType as TypeDoc);
                                                break;
                                            case 'F':
                                                CurrentDoc = new FieldDoc(CurrentType as TypeDoc);
                                                break;
                                            case 'M':
                                                // the method's name for constructors is #ctor, lets change that to the name of the Type

                                                // check if the method is a method of the _current_ type, if it's not then it means the type of this method is not _documented_
                                                // so we add an empty doc for it here
                                                // (probably should do so for properties/fields as well)
                                                if (CurrentType == null || !memberName.Contains(CurrentType.Name.Substring(2))) {
                                                    string typename = memberName.Substring(0, memberName.IndexOf("("));
                                                    typename = typename.Substring(2, typename.LastIndexOf('.') - 2);
                                                    CurrentType = new TypeDoc(CurrentAssembly) { Name = typename };
                                                }

                                                if (memberName.Contains("#ctor")) {
                                                    CurrentDoc = new ConstructorDoc(CurrentType as TypeDoc);
                                                    CurrentDoc.Name = memberName.Replace("#ctor",
                                                                                         Regex.Match(memberName,
                                                                                                     ".([A-z]+).#").Groups[1].Value).Substring(2);
                                                } else {
                                                    CurrentDoc = new MethodDoc(CurrentType as TypeDoc);
                                                }
                                                CurrentMethod = CurrentDoc;
                                                break;
                                        }
                                        if (CurrentDoc.Name == null)
                                            CurrentDoc.Name = memberName.Substring(2);
                                        break;

                                    case "summary":
                                        xml.Read();
                                        PreviousDoc.SetFieldValue("Summary", xml.Value);

                                        break;
                                        
                                    case "param":
                                        string name = xml["name"];
                                        xml.Read();
                                        CurrentDoc = new ParameterDoc(CurrentMethod) {
                                            Name = name,
                                            Summary = xml.Value
                                        };
                                        break;
                                        
                                    case "csharp":
                                    case "javascript":
                                    case "python":
                                    case "lua":
                                        if (PreviousDoc is TypeDoc) {
                                            var cdoc = PreviousDoc as TypeDoc;
                                            var lang = xml.Name;
                                            xml.Read();

                                            cdoc.CodeExamples.Add(new CodeExample {
                                                Code = xml.Value,
                                                Language = (Language)Enum.Parse(typeof(Language), lang)
                                            });
                                        }
                                        break;
                                        
                                    case "v":
                                        break;
                                    case "value":
                                        break;
                                }

                                if (CurrentDoc != null)
                                    PreviousDoc = CurrentDoc;
                            }
                        }
                    }
                    Console.WriteLine("CurrentAssembly is: " + Environment.NewLine + CurrentAssembly);
                }
            }
        }
    }

    public enum Language
    {
        csharp,
        javascript,
        python,
        lua
    }

    public struct CodeExample
    {
        public Language Language;

        public string Code;

        public override string ToString()
        {
            return $"```{Language}\r\n{Code}\r\n```";
        }
    }
}
