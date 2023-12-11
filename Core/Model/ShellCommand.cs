// MIT License - Copyright (c) Vishnu Vijayan
// This file is subject to the terms and conditions defined in
// LICENSE, which is part of this source code package

namespace TraSH.Model
{
    using System;
    using System.Collections.Generic;
    using System.Text;

    public class ShellCommand
    {
        public ShellCommand()
        {
            this.CommandList = new List<SimpleCommand>();
            this.IsBackground = false;
        }

        public List<SimpleCommand> CommandList { get; set; }

        public bool IsBackground { get; set; }

        public override string ToString()
        {
            return string.Join(" | ", this.CommandList);
        }
    }
}
