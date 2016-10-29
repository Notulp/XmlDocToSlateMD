using System;
using System.IO;
using System.Text;

namespace XmlToSlateMD
{
    public static class StringEx
    {
        public static Stream ToStream(this string self, Encoding encoding = null)
        {
            return new MemoryStream((encoding ?? Encoding.UTF8).GetBytes(self ?? ""));
        }
    }
}

