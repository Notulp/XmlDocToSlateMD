using System;
using System.Text.RegularExpressions;
using System.Collections.Generic;

namespace XmlDocToSlateMD
{
    public static class RegexEx
    {
        public static string Join(this CaptureCollection self)
        {
            var result = "";
            foreach (Capture capture in self)
                result += capture.Value;

            return result;
        }
    }
}

