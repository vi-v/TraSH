namespace TraSH.Builtins
{
    using System;
    using System.Collections.Generic;
    using System.Text;

    public class Exit : BuiltInCommand
    {
        public string Execute(IEnumerable<string> args)
        {
            Console.WriteLine("Goodbye");
            Environment.Exit(0);
            return string.Empty;
        }
    }
}
