namespace TraSH.builtins
{
    using System;
    using System.Collections.Generic;

    public class Clear : BuiltInCommand
    {
        public string Execute(IEnumerable<string> args)
        {
            Console.Clear();
            return string.Empty;
        }
    }
}
