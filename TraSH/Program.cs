using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using Medallion.Shell;
using static Medallion.Shell.Shell;

namespace TraSH
{
    class Program
    {
        private static readonly Dictionary<string, Func<IEnumerable<string>, string>> builtInsMap = new Dictionary<string, Func<IEnumerable<string>, string>>()
        {
            { "cls", Clear },
            { "clear", Clear },
            { "exit", Exit }
        };

        static void Main(string[] args)
        {
            Console.CancelKeyPress += delegate (object sender, ConsoleCancelEventArgs e)
            {
                e.Cancel = true;
                //Exit(Enumerable.Empty<string>());
            };

            while (true)
            {
                Console.Write("TraSH> ");
                string rawCmd = Console.ReadLine();

                if (string.IsNullOrEmpty(rawCmd))
                {
                    continue;
                }

                IEnumerable<string> cmdArgs = rawCmd.Split(' ');
                if (builtInsMap.ContainsKey(cmdArgs.First()))
                {
                    ExecuteBuiltInCommand(cmdArgs);
                }
                else
                {
                    ExecuteExternalCommand(cmdArgs);
                }
            }
        }

        private static void ExecuteBuiltInCommand(IEnumerable<string> args)
        {
            string output = builtInsMap[args.First()](args.Skip(1));
            Console.WriteLine(output);
        }

        private static void ExecuteExternalCommand(IEnumerable<string> args)
        {
            RunCmd(string.Join(' ', args), Environment.CurrentDirectory, Console.Out);
        }

        private static void RunCmd(string cmd, string working_dir, TextWriter outputWriter)
        {
            Process proc = new Process();
            ProcessStartInfo psi = new ProcessStartInfo();
            psi.WorkingDirectory = working_dir;
            psi.FileName = "cmd.exe";
            psi.Arguments = "/c " + cmd;
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
            proc.Start();
            proc.BeginOutputReadLine();
            proc.BeginErrorReadLine();
            proc.WaitForExit();
        }

        private static string Clear(IEnumerable<string> args)
        {
            Console.Clear();
            return string.Empty;
        }

        private static string Exit(IEnumerable<string> args)
        {
            Console.WriteLine("Goodbye");
            Environment.Exit(0);
            return string.Empty;
        }
    }
}
