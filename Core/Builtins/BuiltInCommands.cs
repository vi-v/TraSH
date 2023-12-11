// MIT License - Copyright (c) Vishnu Vijayan
// This file is subject to the terms and conditions defined in
// LICENSE, which is part of this source code package

namespace TraSH.Builtins
{
    using System.Collections.Generic;

    public static class BuiltInCommands
    {
        public static readonly IBuiltInCommand Cd = new ChangeDirectory();

        public static readonly IBuiltInCommand Clear = new Clear();

        public static readonly IBuiltInCommand Exit = new Exit();

        public static readonly IDictionary<string, IBuiltInCommand> Map = new Dictionary<string, IBuiltInCommand>
        {
            { "cls", Clear},
            { "clear", Clear },
            { "cd", Cd },
            { "exit", Exit }
        };
    }
}
