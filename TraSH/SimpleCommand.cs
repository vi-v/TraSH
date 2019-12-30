namespace TraSH
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;

    public class SimpleCommand
    {
        public SimpleCommand()
            : this(string.Empty, new List<string> ())
        {
        }

        public SimpleCommand(string command, IEnumerable<string> arguments)
        {
            this.Command = command;
            this.Arguments = arguments.ToList();
        }

        public string Command { get; set; }

        public List<string> Arguments { get; set; }

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            sb.Append(this.Command);

            if (this.Arguments.Count > 0)
            {
                sb.Append(" ");
                sb.Append(string.Join(" ", this.Arguments));
            }

            return sb.ToString();
        }
    }
}
