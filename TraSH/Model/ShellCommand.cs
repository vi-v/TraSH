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
