﻿using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.IO;
using System.Linq;
using System.Xml;
using System.Reflection;

using XmlToSlateMD.Documentation;

namespace XmlToSlateMD
{
    class MainClass
    {
        static AssemblyDoc CurrentAssembly;

        static Assembly reflectedAssembly;

        static List<Type> reflectedMethodParams;

        static Type reflectedType;

        static MethodBase reflectedMethod;

        static BaseDoc CurrentType;

        static BaseDoc CurrentMethod;

        static BaseDoc PreviousDoc;

        static int currentParam = 0;

        public static void Main(string[] args)
        {
            foreach (var file in Directory.GetFiles(Environment.CurrentDirectory)) {
                if (file.ToLowerInvariant().EndsWith(".dll")) {
                    Assembly.LoadFile(file);
                }
            }
            foreach (var file in Directory.GetFiles(Environment.CurrentDirectory)) {
                if (file.ToLowerInvariant().EndsWith(".xml")) {
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

                                    switch (xml.Name) {
                                        case "assembly":
                                            CurrentAssembly = new AssemblyDoc();
                                            CurrentDoc = CurrentAssembly;
                                            break;
                                        
                                        case "name":
                                            xml.Read();
                                            CurrentAssembly.Name = xml.Value;
                                            reflectedAssembly = AppDomain.CurrentDomain.GetAssemblies().ToList().First(a => a.GetName().Name == xml.Value);
                                            break;
                                        
                                        case "member":
                                            var memberName = xml["name"];
                                            char type = memberName[0];
                                            switch (type) {
                                                case 'T':
                                                    CurrentDoc = new TypeDoc(CurrentAssembly);
                                                    CurrentType = CurrentDoc;
                                                    reflectedType = reflectedAssembly.GetType(memberName.Substring(2));
                                                    break;
                                                case 'P':
                                                    CurrentDoc = new PropertyDoc(CurrentType as TypeDoc) {
                                                        Name = memberName.Substring(2)
                                                    };
                                                    break;
                                                case 'F':
                                                    CurrentDoc = new FieldDoc(CurrentType as TypeDoc) {
                                                        Name = memberName.Substring(2)
                                                    };
                                                    break;
                                                case 'M':
                                                    // check if the method is a method of the _current_ type, if it's not then it means the type of this method is not _documented_
                                                    // so we add an empty doc for it here
                                                    // (probably should do so for properties/fields as well)
                                                    if (CurrentType == null || !memberName.Contains(CurrentType.Name.Substring(2))) {
                                                        string typename = memberName.Substring(0, memberName.IndexOf("("));
                                                        typename = typename.Substring(2, typename.LastIndexOf('.') - 2);
                                                        CurrentType = new TypeDoc(CurrentAssembly) { Name = typename };
                                                        reflectedType = reflectedAssembly.GetType(typename);
                                                    }

                                                    reflectedMethodParams = new List<Type>();

                                                    string methodname = Regex.Match(memberName, "\\.([#A-z]*)\\(").Groups[1].Captures.Join();

                                                    string paramtypes = Regex.Match(memberName, "\\(([\\.,A-z0-9])*\\)").Groups[1].Captures.Join();

                                                    foreach (var paramtype in paramtypes.Split(',')) {
                                                        Type typ = null;
                                                        if (!TryFindType(paramtype, out typ)) {
                                                            var paramtype2 = Regex.Replace(paramtype, "(.*)\\.([A-z]*)$", "$1+$2");
                                                            TryFindType(paramtype2, out typ);
                                                        }
                                                        if (typ != null)
                                                            reflectedMethodParams.Add(typ);
                                                    }

                                                    if (!methodname.Contains('#'))
                                                        reflectedMethod = reflectedType.GetMethod(methodname, reflectedMethodParams.ToArray());
                                                    else
                                                        reflectedMethod = reflectedType.GetConstructor(reflectedMethodParams.ToArray());

                                                    // the method's name for constructors is #ctor, lets change that to the name of the Type
                                                    if (memberName.Contains("#ctor")) {
                                                        CurrentDoc = new ConstructorDoc(CurrentType as TypeDoc);
                                                        CurrentDoc.Name = memberName.Replace("#ctor", Regex.Match(memberName, ".([A-z]+).#").Groups[1].Value).Substring(2);
                                                        
                                                    } else {
                                                        CurrentDoc = new MethodDoc(CurrentType as TypeDoc);
                                                    }

                                                    CurrentMethod = CurrentDoc;
                                                    currentParam = 0;

                                                    if (reflectedMethod is MethodInfo)
                                                        CurrentMethod["ReturnType"] = (reflectedMethod as MethodInfo).ReturnType;
                                                    else
                                                        CurrentMethod["ReturnType"] = reflectedType;
                                                    
                                                    break;
                                            }
                                            if (CurrentDoc.Name == null)
                                                CurrentDoc.Name = memberName.Substring(2);
                                            break;

                                        case "summary":
                                            xml.Read();
                                            PreviousDoc["Summary"] = xml.Value;

                                            break;
                                        
                                        case "param":
                                            string name = xml["name"];
                                            xml.Read();
                                            CurrentDoc = new ParameterDoc(CurrentMethod) {
                                                Name = name,
                                                Summary = xml.Value,
                                                Type = reflectedMethodParams[currentParam].FullName + " (" + Assembly.GetAssembly(reflectedMethodParams[currentParam]).GetName().Name + ".dll)"
                                            };
                                            currentParam++;
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
                                            xml.Read();
                                            PreviousDoc["defaultValue"] = xml.Value;
                                            break;
                                    }

                                    if (CurrentDoc != null)
                                        PreviousDoc = CurrentDoc;
                                }
                            }
                        }
                        Console.WriteLine("CurrentAssembly is: " + Environment.NewLine + CurrentAssembly);
                        CurrentAssembly.ToFile();
                    }
                }
            }
        }

        public static readonly Dictionary<string, Type> typeCache = new Dictionary<string, Type>();

        public static bool TryFindType(string typeName, out Type t)
        {
            lock (typeCache) {
                if (!typeCache.TryGetValue(typeName, out t)) {
                    foreach (Assembly assembly in AppDomain.CurrentDomain.GetAssemblies()) {
                        t = assembly.GetType(typeName);
                        if (t != null) {
                            break;
                        }
                    }
                    typeCache[typeName] = t;
                }
            }
            return (t != null);
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
