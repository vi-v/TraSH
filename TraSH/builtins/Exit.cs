// MIT License - Copyright (c) Vishnu Vijayan
// This file is subject to the terms and conditions defined in
// LICENSE, which is part of this source code package

namespace TraSH.Builtins
{
    using System;
    using System.Collections.Generic;
    using System.Text;

    public class Exit : IBuiltInCommand
    {
        public string Execute(IEnumerable<string> args)
        {
            Console.WriteLine("Goodbye");
            Environment.Exit(0);
            return string.Empty;
        }
    }
}
