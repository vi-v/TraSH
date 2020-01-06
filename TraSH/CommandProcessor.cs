// MIT License - Copyright (c) Vishnu Vijayan
// This file is subject to the terms and conditions defined in
// LICENSE, which is part of this source code package

namespace TraSH
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Text;
    using TraSH.Model;

    public class CommandProcessor
    {
        private readonly ShellCommand shellCommand;
        private readonly TextWriter outWriter;
        private readonly TextWriter errWriter;

        public CommandProcessor(ShellCommand shellCommand)
            : this(shellCommand, Console.Out, Console.Error)
        {
        }

        public CommandProcessor(ShellCommand shellCommand, TextWriter outWriter, TextWriter errWriter)
        {
            this.shellCommand = shellCommand;
            this.outWriter = outWriter;
            this.errWriter = errWriter;
        }

        public void Run()
        {

        }
    }
}
