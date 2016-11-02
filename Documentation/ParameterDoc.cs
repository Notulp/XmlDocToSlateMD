﻿using System;

namespace XmlToSlateMD.Documentation
{
    public class ParameterDoc : BaseDoc
    {
        public string defaultValue = "<null>";

        public string Summary = "<missing>";

        public string Type = "<missing>";

        public ParameterDoc(BaseDoc parent)
            : base(parent)
        {
        }

        public override string ToString()
        {
            return $"{Name} | {Type} | {defaultValue} | {Summary}";
        }
    }
}

