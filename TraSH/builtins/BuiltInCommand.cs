using System.Collections.Generic;

namespace TraSH.builtins
{
    public interface BuiltInCommand
    {
        string Execute(IEnumerable<string> args);
    }
}
