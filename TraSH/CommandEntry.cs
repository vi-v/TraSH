namespace TraSH
{
    using System;
    using System.Text.RegularExpressions;

    public class CommandEntry : IComparable
    {
        private readonly string command;

        public CommandEntry(string command)
        {
            this.command = command;
        }

        public int CompareTo(object obj)
        {
            return string.Compare(this.command, obj.ToString(), StringComparison.Ordinal);
        }

        public override string ToString()
        {
            return command;
        }

        public string ToEscapedString()
        {
            return Regex.Escape(this.command);
        }
    }
}
