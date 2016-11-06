using System;
using System.Reflection;

namespace XmlToSlateMD
{
    public static class TypeEx
    {
        public static string Format(this Type self)
        {
            if (self.FullName.StartsWith("Pluton."))
                return (self.FullName.WrapInHref() + " (" + Assembly.GetAssembly(self).GetName().Name + ".dll)").HtmlSafeSquareBrakets();
            return (self.FullName + " (" + Assembly.GetAssembly(self).GetName().Name + ".dll)").HtmlSafeSquareBrakets();
        }
    }
}

