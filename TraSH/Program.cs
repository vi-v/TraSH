using Antlr4.Runtime;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using TraSH.Builtins;
using TraSH.Gen;
using TraSH.Model;

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
            };

            LineEditor lineEditor = new LineEditor();

            lineEditor.LineReceived += delegate (object _, string e)
            {
                if (string.IsNullOrEmpty(e))
                {
                    Console.WriteLine();
                    return;
                }

                AntlrInputStream inputStream = new AntlrInputStream(e);
                ShellLexer shellLexer = new ShellLexer(inputStream);
                CommonTokenStream commonTokenStream = new CommonTokenStream(shellLexer);
                ShellParser shellParser = new ShellParser(commonTokenStream);
                ShellParser.ShellCommandContext context = shellParser.shellCommand();
                ShellVisitor visitor = new ShellVisitor();
                ParserResult result = visitor.Visit(context);

                ShellCommand shellCommand = result.ShellCommandValue;
                SimpleCommand firstCommand = shellCommand.CommandList[0];

                IEnumerable<string> cmdArgs = e.Split(' ');
                if (builtInsMap.ContainsKey(firstCommand.Command))
                {
                    ExecuteBuiltInCommand(firstCommand);
                }
                else
                {
                    ExecuteExternalCommand(firstCommand);
                }
            };

            lineEditor.Start();
        }

        private static void ExecuteBuiltInCommand(SimpleCommand simpleCommand)
        {
            BuiltInCommand command = builtInsMap[simpleCommand.Command];
            string output = command.Execute(simpleCommand.Arguments);

            if (!string.IsNullOrEmpty(output))
            {
                Console.WriteLine(output);
            }
        }

        private static void ExecuteExternalCommand(SimpleCommand simpleCommand)
        {
            RunCmd(simpleCommand, Console.Out);
        }

        private static void RunCmd(SimpleCommand simpleCommand, TextWriter outputWriter)
        {
            Process proc = simpleCommand.AsProcess();
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
            catch (System.ComponentModel.Win32Exception)
            {
                Console.WriteLine($"{simpleCommand.Command}: Command not found");
            }
        }
    }
}
