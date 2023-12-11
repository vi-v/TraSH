// MIT License - Copyright (c) Vishnu Vijayan
// This file is subject to the terms and conditions defined in
// LICENSE, which is part of this source code package

namespace TraSH.Builtins
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;

    public class ChangeDirectory : IBuiltInCommand
    {
        public string Execute(IEnumerable<string> args)
        {
            List<string> arguments = args.ToList();

            string newDirectory = null;
            if (arguments.Count > 0)
            {
                newDirectory = arguments[0];
            }

            if (string.IsNullOrEmpty(newDirectory))
            {
                Directory.SetCurrentDirectory(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile));
            }
            else
            {
                if (!Directory.Exists(newDirectory) && !File.Exists(newDirectory))
                {
                    return $"{newDirectory}: No such file or directory";
                }
                else
                {
                    FileAttributes attr = File.GetAttributes(newDirectory);
                    if (attr.HasFlag(FileAttributes.Directory))
                    {
                        Directory.SetCurrentDirectory(newDirectory);
                        return string.Empty;
                    }
                    else
                    {
                        return $"{newDirectory}: Not a directory";
                    }
                }
            }

            return string.Empty;
        }
    }
}
