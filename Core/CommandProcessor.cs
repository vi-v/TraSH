// MIT License - Copyright (c) Vishnu Vijayan
// This file is subject to the terms and conditions defined in
// LICENSE, which is part of this source code package

namespace TraSH
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.IO;
    using System.Linq;
    using System.Threading;
    using TraSH.Builtins;
    using TraSH.Model;

    public class CommandProcessor
    {
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

            if (this.shellCommand.CommandList.Any(c => BuiltInCommands.Map.ContainsKey(c.Command)) && this.shellCommand.CommandList.Count > 1)
            {
                this.errWriter.WriteLine($"Cannot run commands together: {string.Join(',', this.shellCommand.CommandList.Select(c => c.Command))}");
                return;
            }

            if (BuiltInCommands.Map.ContainsKey(this.shellCommand.CommandList[0].Command))
            {
                this.ExecuteBuiltinCommand(this.shellCommand.CommandList[0]);
                return;
            }

            List<Process> procList = new List<Process>();
            List<Pipe> pipeList = new List<Pipe>();

            Process prevProc = null;
            foreach (SimpleCommand command in this.shellCommand.CommandList)
            {
                if (!CommandValidator.Validate(command.Command))
                {
                    this.errWriter.WriteLine($"Command not found: {command.Command}");
                    return;
                }

                Process proc = command.AsProcess();

                if (prevProc == null)
                {
                    proc.StartInfo.RedirectStandardInput = false;
                }
                else
                {
                    Pipe pipe = new Pipe(prevProc, proc);
                    pipeList.Add(pipe);
                    prevProc.Exited += (_, e) => { pipe.Close(); };
                }

                procList.Add(proc);
                prevProc = proc;
            }

            Pipe endPipe = new Pipe(prevProc, this.outWriter);
            pipeList.Add(endPipe);

            try
            {
                procList.ForEach(p => p.Start());
                pipeList.ForEach(p => p.Start());

                procList.ForEach(p => p.WaitForExit());
                endPipe.Wait();
            }
            catch (System.ComponentModel.Win32Exception)
            {
                procList.ForEach(p => p.Kill());
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
            var builtInCommand = BuiltInCommands.Map[command.Command];

            string output = builtInCommand.Execute(command.Arguments);

            this.outWriter.Write(output);
        }

        private void ReadStream(StreamReader reader, TextWriter outWriter)
        {
            new Thread(() =>
            {
                Debug.WriteLine($"{DateTime.Now.ToString("hh:mm:ss fffffff")} Started readstream");
                bool shouldBreak = false;
                while (!shouldBreak)
                {
                    int current;
                    while ((current = reader.Read()) >= 0)
                    {
                        //Debug.WriteLine($"{DateTime.Now.ToString("hh:mm:ss fffffff")} {(char)current}");
                        Console.Out.Write((char)current);
                    }
                    shouldBreak = true;
                }
            }).Start();
        }
    }
}
