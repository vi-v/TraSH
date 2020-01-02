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
    }
}
