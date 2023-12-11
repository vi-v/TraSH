// MIT License - Copyright (c) Vishnu Vijayan
// This file is subject to the terms and conditions defined in
// LICENSE, which is part of this source code package

namespace TraSH.Model
{
    using System;
    using System.Collections.Generic;
    using System.Text;

    public class ParserResult
    {
        private readonly object rawValue;

        public ParserResult(object rawValue)
        {
            this.rawValue = rawValue;
        }

        public bool IsArg { get; set; }

        public string ArgValue { get => (string)this.rawValue; }

        public bool IsArgList { get; set; }

        public List<string> ArgListValue { get => (List<string>)this.rawValue; }

        public bool IsCmd { get; set; }

        public string CmdValue { get => (string)this.rawValue; }

        public bool IsSimpleCommand { get; set; }

        public SimpleCommand SimpleCommandValue { get => (SimpleCommand)this.rawValue; }

        public bool IsPipeList { get; set; }

        public List<SimpleCommand> PipeListValue { get => (List<SimpleCommand>)this.rawValue; }

        public bool IsShellCommand { get; set; }

        public ShellCommand ShellCommandValue { get => (ShellCommand)this.rawValue; }
    }
}
