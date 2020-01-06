// MIT License - Copyright (c) Vishnu Vijayan
// This file is subject to the terms and conditions defined in
// LICENSE, which is part of this source code package

namespace TraSH
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.IO;
    using System.Text;
    using System.Threading;
    using TraSH.Builtins;
    using TraSH.Model;

    public class CommandProcessor
    {
        private readonly Dictionary<string, BuiltInCommand> builtInsMap = new Dictionary<string, BuiltInCommand>()
        {
            { "cls", new Clear() },
            { "clear", new Clear() },
            { "cd", new ChangeDirectory() },
            { "exit", new Exit() }
        };

        private readonly ShellCommand shellCommand;
        private readonly TextWriter outWriter;
        private readonly TextWriter errWriter;

        public CommandProcessor(ShellCommand shellCommand)
            : this(shellCommand, Console.Out, Console.Error)
        {
        }

        public CommandProcessor(ShellCommand shellCommand, TextWriter outWriter, TextWriter errWriter)
        {
            this.shellCommand = shellCommand;
            this.outWriter = outWriter;
            this.errWriter = errWriter;
        }

        public void Run()
        {
            if (this.shellCommand.CommandList.Count == 0)
            {
                return;
            }

            SimpleCommand firstCommand = shellCommand.CommandList[0];
            if (this.builtInsMap.ContainsKey(firstCommand.Command))
            {
                this.ExecuteBuiltinCommand(firstCommand);
            }
            else
            {
                try
                {
                    ExecuteExternalCommand(firstCommand);
                }
                catch (System.ComponentModel.Win32Exception)
                {
                    this.errWriter.WriteLine($"{firstCommand.Command}: Command not found");
                }
            }
        }

        private void ExecuteExternalCommand(SimpleCommand command)
        {
            Process proc = command.AsProcess();
            proc.StartInfo.RedirectStandardInput = false;

            proc.Start();

            this.ReadStream(proc.StandardOutput, this.outWriter);
            this.ReadStream(proc.StandardError, this.errWriter);

            proc.WaitForExit();
        }

        private void ExecuteBuiltinCommand(SimpleCommand command)
        {
            BuiltInCommand builtInCommand = this.builtInsMap[command.Command];
            
            string output = builtInCommand.Execute(command.Arguments);

            this.outWriter.Write(output);
        }

        private void ReadStream(StreamReader reader, TextWriter outWriter)
        {
            new Thread(() =>
            {
                bool shouldBreak = false;
                while (!shouldBreak)
                {
                    int current;
                    while ((current = reader.Read()) >= 0)
                    {
                        outWriter.Write((char)current);
                    }
                    shouldBreak = true;
                }
            }).Start();
        }
    }
}
