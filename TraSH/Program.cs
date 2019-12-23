using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using TraSH.Builtins;

namespace TraSH
{
    class Program
    {
        private static readonly Dictionary<string, BuiltInCommand> builtInsMap = new Dictionary<string, BuiltInCommand>()
        {
            { "cls", new Clear() },
            { "clear", new Clear() },
            { "cd", new ChangeDirectory() },
            { "exit", new Exit() }
        };

        static void Main(string[] args)
        {
            Console.CancelKeyPress += delegate (object sender, ConsoleCancelEventArgs e)
            {
                e.Cancel = true;
                //Exit(Enumerable.Empty<string>());
            };

            LineEditor lineEditor = new LineEditor();

            lineEditor.LineReceived += delegate (object _, string e)
            {
                if (string.IsNullOrEmpty(e))
                {
                    Console.WriteLine();
                    lineEditor.PrintPrompt();
                    return;
                }

                IEnumerable<string> cmdArgs = e.Split(' ');
                if (builtInsMap.ContainsKey(cmdArgs.First()))
                {
                    ExecuteBuiltInCommand(cmdArgs);
                }
                else
                {
                    ExecuteExternalCommand(cmdArgs);
                }
                lineEditor.PrintPrompt();
            };

            lineEditor.PrintPrompt();
            lineEditor.Start();
        }

        private static void ExecuteBuiltInCommand(IEnumerable<string> args)
        {
            BuiltInCommand command = builtInsMap[args.First()];
            string output = command.Execute(args.Skip(1));

            if (!string.IsNullOrEmpty(output))
            {
                Console.WriteLine(output);
            }
        }

        private static void ExecuteExternalCommand(IEnumerable<string> args)
        {
            RunCmd(args, Console.Out);
        }

        private static void RunCmd(IEnumerable<string> args, TextWriter outputWriter)
        {
            string commandName = args.First();
            List<string> arguments = args.Skip(1).ToList();

            Process proc = new Process();
            ProcessStartInfo psi = new ProcessStartInfo();
            psi.WorkingDirectory = Environment.CurrentDirectory;
            psi.FileName = commandName;
            arguments.ForEach(a => psi.ArgumentList.Add(a));
            //psi.FileName = "cmd.exe";
            //psi.Arguments = "/c " + cmd;
            //psi.WindowStyle = ProcessWindowStyle.Hidden;
            psi.RedirectStandardOutput = true;

            psi.RedirectStandardError = true;

            psi.UseShellExecute = false;

            proc.StartInfo = psi;
            proc.OutputDataReceived += new DataReceivedEventHandler((_, e) =>
            {
                if (!string.IsNullOrEmpty(e.Data))
                {
                    outputWriter.WriteLine(e.Data);
                }
            });
            proc.ErrorDataReceived += new DataReceivedEventHandler((_, e) =>
            {
                if (!string.IsNullOrEmpty(e.Data))
                {
                    outputWriter.WriteLine(e.Data);
                }
            });

            try
            {
                proc.Start();
                proc.BeginOutputReadLine();
                proc.BeginErrorReadLine();
                proc.WaitForExit();
            }
            catch (System.ComponentModel.Win32Exception e)
            {
                Console.WriteLine($"{commandName}: Command not found");
            }
        }
    }
}
