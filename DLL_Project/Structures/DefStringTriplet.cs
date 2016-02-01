﻿using System;
using System.Text;
using Verse;

namespace CommunityCoreLibrary
{

    public struct DefStringTriplet
    {
        public Def Def;
        public string Prefix;
        public string Suffix;

        public DefStringTriplet(Def def, string prefix = null, string suffix = null )
        {
            Def = def;
            Prefix = prefix;
            Suffix = suffix;
        }

        public override string ToString()
        {
            StringBuilder s = new StringBuilder();
            if (Prefix != "")
            {
                s.Append(Prefix + " ");
            }
            s.Append(Def.LabelCap);
            if (Suffix != "")
            {
                s.Append(" " + Suffix);
            }
            return s.ToString();
        }
    }
}
