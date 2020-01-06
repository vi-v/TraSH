// MIT License - Copyright (c) Vishnu Vijayan
// This file is subject to the terms and conditions defined in
// LICENSE, which is part of this source code package

namespace TraSH
{
    using Antlr4.Runtime;
    using System;
    using TraSH.Gen;
    using TraSH.Model;

    public class Program
    {
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

                ShellCommand shellCommand = ParseCommand(e);
                CommandProcessor commandProcessor = new CommandProcessor(shellCommand);

                commandProcessor.Run();
            };

            lineEditor.Start();
        }

        private static ShellCommand ParseCommand(string command)
        {
            AntlrInputStream inputStream = new AntlrInputStream(command);
            ShellLexer shellLexer = new ShellLexer(inputStream);
            CommonTokenStream commonTokenStream = new CommonTokenStream(shellLexer);
            ShellParser shellParser = new ShellParser(commonTokenStream);
            ShellParser.ShellCommandContext context = shellParser.shellCommand();
            ShellVisitor visitor = new ShellVisitor();
            ParserResult result = visitor.Visit(context);

            return result.ShellCommandValue;
        }
    }
}
