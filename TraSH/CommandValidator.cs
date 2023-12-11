// MIT License - Copyright (c) Vishnu Vijayan
// This file is subject to the terms and conditions defined in
// LICENSE, which is part of this source code package

namespace TraSH
{
    using System;
    using System.IO;
    using TraSH.Builtins;

    public static class CommandValidator
    {
        public static bool Validate(string command)
        {
            if (BuiltInCommands.Map.ContainsKey(command))
            {
                return true;
            }

            if (File.Exists(command))
            {
                return true;
            }

            var pathValues = Environment.GetEnvironmentVariable("PATH");
            foreach (var path in pathValues.Split(Path.PathSeparator))
            {
                var fullPath = Path.Combine(path, command);
                if (File.Exists(fullPath) || File.Exists(fullPath + ".exe"))
                {
                    return true;
                }
            }

            return false;
        }
    }
}
