﻿// MIT License - Copyright (c) Vishnu Vijayan
// This file is subject to the terms and conditions defined in
// LICENSE, which is part of this source code package

namespace TraSH.Builtins
{
    using System.Collections.Generic;

    public interface BuiltInCommand
    {
        string Execute(IEnumerable<string> args);
    }
}
