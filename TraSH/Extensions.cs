﻿namespace TraSH
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Text;

    public static class Extensions
    {
        public static string RemainingSubstring(this StringBuilder sb, int index)
        {
            return sb.ToString(index, sb.Length - index);
        }
    }
}
