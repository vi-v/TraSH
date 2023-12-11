// MIT License - Copyright (c) Vishnu Vijayan
// This file is subject to the terms and conditions defined in
// LICENSE, which is part of this source code package

namespace TraSH
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

        public static bool IsControlActive(this ConsoleKeyInfo c)
        {
            return (c.Modifiers & ConsoleModifiers.Control) != 0;
        }
    }
}
