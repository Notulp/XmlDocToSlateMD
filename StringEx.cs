using System;
using System.IO;
using System.Text;

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

