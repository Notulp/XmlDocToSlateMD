using System;
using System.Reflection;

namespace XmlToSlateMD
{
    public static class TypeEx
    {
        public static string Format(this Type self)
        {
            return self.FullName + " (" + Assembly.GetAssembly(self).GetName().Name + ".dll)";
        }
    }
}

