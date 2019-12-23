using System.Collections.Generic;

namespace TraSH.Builtins
{
    public interface BuiltInCommand
    {
        string Execute(IEnumerable<string> args);
    }
}
