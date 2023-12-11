// MIT License - Copyright (c) Vishnu Vijayan
// This file is subject to the terms and conditions defined in
// LICENSE, which is part of this source code package

namespace TraSH.Model
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Linq;
    using System.Text;

    public class SimpleCommand
    {
        public SimpleCommand()
            : this(string.Empty, new List<string> ())
        {
        }

        public SimpleCommand(string command, IEnumerable<string> arguments)
        {
            this.Command = command;
            this.Arguments = arguments.ToList();
        }

        public string Command { get; set; }

        public List<string> Arguments { get; set; }

        public Process AsProcess()
        {
            Process proc = new Process();
            ProcessStartInfo psi = new ProcessStartInfo();
            psi.WorkingDirectory = Environment.CurrentDirectory;
            psi.FileName = this.Command;
            this.Arguments.ForEach(psi.ArgumentList.Add);
            psi.RedirectStandardInput = true;
            psi.RedirectStandardOutput = true;
            psi.RedirectStandardError = true;
            psi.UseShellExecute = false;

            proc.StartInfo = psi;

            return proc;
        }

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            sb.Append(this.Command);

            if (this.Arguments.Count > 0)
            {
                sb.Append(" ");
                sb.Append(string.Join(" ", this.Arguments));
            }

            return sb.ToString();
        }
    }
}
