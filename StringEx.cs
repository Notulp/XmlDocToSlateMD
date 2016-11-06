using System;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;

namespace XmlToSlateMD
{
    public static class StringEx
    {
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

