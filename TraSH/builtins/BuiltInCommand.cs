// MIT License - Copyright (c) Vishnu Vijayan
// This file is subject to the terms and conditions defined in
// LICENSE, which is part of this source code package

using System.Collections.Generic;

namespace TraSH.Builtins
{
    public interface BuiltInCommand
    {
        string Execute(IEnumerable<string> args);
    }
}
