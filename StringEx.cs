using System;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using System.Collections;
using System.Collections.Generic;

namespace XmlToSlateMD
{
    public static class StringEx
    {
        static Dictionary<string, string> AngleBracketsToHTMLSafe = new Dictionary<string, string>
        {
            {"<", "&lt;"},
            {">", "&gt;"}
        };

        static Dictionary<string, string> BracketsToHTMLSafe = new Dictionary<string, string>
        {
            {"(", "&#40;"},
            {")", "&#41;"}
        };

        static Dictionary<string, string> SquareBracketsToHTMLSafe = new Dictionary<string, string>
        {
            {"[", "&#91;"},
            {"]", "&#92;"}
        };

        public static string GetMemberName(this string methodOrPropOrFieldName)
        {
            if (!methodOrPropOrFieldName.Contains("("))
                return methodOrPropOrFieldName.Substring(methodOrPropOrFieldName.LastIndexOf('.') + 1);
            return Regex.Match(methodOrPropOrFieldName, "\\.([#A-z]*)\\(").Groups[1].Captures.Join();
        }

        public static string GetTypeName(this string methodOrPropOrFieldName)
        {
            if (!methodOrPropOrFieldName.Contains("("))
                return methodOrPropOrFieldName.Substring(2, methodOrPropOrFieldName.LastIndexOf(".") - 2);
            string typename = methodOrPropOrFieldName.Substring(0, methodOrPropOrFieldName.IndexOf("("));
            return typename.Substring(2, typename.LastIndexOf('.') - 2);
        }

        public static string HtmlSafe(this string self)
        {
            return self.HtmlSafeBrakets().HtmlSafeAngleBrakets().HtmlSafeSquareBrakets();
        }

        public static string HtmlSafeAngleBrakets(this string self)
        {
            foreach (KeyValuePair<string, string> kvp in AngleBracketsToHTMLSafe)
                self = self.Replace(kvp.Key, kvp.Value);
            return self;
        }

        public static string HtmlSafeBrakets(this string self)
        {
            foreach (KeyValuePair<string, string> kvp in BracketsToHTMLSafe)
                self = self.Replace(kvp.Key, kvp.Value);
            return self;
        }

        public static string HtmlSafeSquareBrakets(this string self)
        {
            foreach (KeyValuePair<string, string> kvp in SquareBracketsToHTMLSafe)
                self = self.Replace(kvp.Key, kvp.Value);
            return self;
        }

        public static Stream ToStream(this string self, Encoding encoding = null)
        {
            return new MemoryStream((encoding ?? Encoding.UTF8).GetBytes(self ?? ""));
        }

        public static string WrapInHref(this string self)
        {
            var linkedname = self.Replace('.', '-').ToLowerInvariant();
            return $"<a href=\"#{linkedname}\">{self.GetMemberName()}</a>";
        }
    }
}

