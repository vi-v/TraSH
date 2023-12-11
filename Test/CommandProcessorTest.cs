// MIT License - Copyright (c) Vishnu Vijayan
// This file is subject to the terms and conditions defined in
// LICENSE, which is part of this source code package

namespace TraSH.Test
{
    using System;
    using System.Collections.Generic;
    using System.Text;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using FluentAssertions;
    using TraSH.Model;
    using System.IO;
    using TraSH;
    using System.Linq;

    [TestClass]
    public class CommandProcessorTest
    {
        private static readonly string catCommand = @"C:\Program Files (x86)\GnuWin32\bin\cat.exe";
        private static readonly string wcCommand = @"C:\Program Files (x86)\GnuWin32\bin\wc.exe";
        private static readonly string loremipsum = $"It is a long established fact that a reader will be distracted by the readable content of a page when looking at its layout. The point of using Lorem Ipsum is that it has a more-or-less normal distribution of letters, as opposed to using 'Content here, content here', making it look like readable English. Many desktop publishing packages and web page editors now use Lorem Ipsum as their default model text, and a search for 'lorem ipsum' will uncover many web sites still in their infancy. Various versions have evolved over the years, sometimes by accident, sometimes on purpose (injected humour and the like).";

        [TestMethod]
        public void RunBuiltinCommandTest()
        {
            string dirPath = Path.Join(Path.GetTempPath(), Guid.NewGuid().ToString());
            Directory.CreateDirectory(dirPath);
            ShellCommand shellCommand = new ShellCommand()
            {
                CommandList = new List<SimpleCommand>
                {
                    new SimpleCommand("cd", new List<string> { dirPath })
                }
            };
            CommandProcessor commandProcessor = new CommandProcessor(shellCommand);

            commandProcessor.Run();

            Environment.CurrentDirectory.Should().Be(dirPath);
        }

        [TestMethod]
        public void RunExternalCommandTest()
        {
            StringWriter stdOut = OutputStream(out StringBuilder outSb);
            StringWriter stdErr = OutputStream(out StringBuilder errSb);
            string file = TempFileWithContent(loremipsum);
            ShellCommand shellCommand = new ShellCommand()
            {
                CommandList = new List<SimpleCommand>
                {
                    new SimpleCommand(catCommand, new List<string> { file })
                }
            };
            CommandProcessor commandProcessor = new CommandProcessor(shellCommand, stdOut, stdErr);

            commandProcessor.Run();

            outSb.ToString().Should().Be(loremipsum);
            errSb.ToString().Should().BeNullOrEmpty();
        }

        [TestMethod]
        public void CommandDoesNotExistTest()
        {
            string invalidCommand = Guid.NewGuid().ToString();
            StringWriter stdOut = OutputStream(out StringBuilder outSb);
            StringWriter stdErr = OutputStream(out StringBuilder errSb);
            ShellCommand shellCommand = new ShellCommand()
            {
                CommandList = new List<SimpleCommand>
                {
                    new SimpleCommand(invalidCommand, Enumerable.Empty<string>())
                }
            };
            CommandProcessor commandProcessor = new CommandProcessor(shellCommand, stdOut, stdErr);

            commandProcessor.Run();

            outSb.ToString().Should().BeNullOrEmpty();
            errSb.ToString().Trim().Should().Be($"Command not found: {invalidCommand}");
        }

        [TestMethod]
        public void DoesNotRunPipelineWithBuiltIn()
        {
            StringWriter stdOut = OutputStream(out StringBuilder outSb);
            StringWriter stdErr = OutputStream(out StringBuilder errSb);
            ShellCommand shellCommand = new ShellCommand()
            {
                CommandList = new List<SimpleCommand>
                {
                    new SimpleCommand(catCommand, new List<string> { }),
                    new SimpleCommand("cd", Enumerable.Empty<string>())
                }
            };
            CommandProcessor commandProcessor = new CommandProcessor(shellCommand, stdOut, stdErr);

            commandProcessor.Run();

            outSb.ToString().Should().BeNullOrEmpty();
            errSb.ToString().Trim().Should().Be($"Cannot run commands together: {catCommand},cd");
        }

        [TestMethod]
        public void PipelineTest()
        {
            StringWriter stdOut = OutputStream(out StringBuilder outSb);
            StringWriter stdErr = OutputStream(out StringBuilder errSb);
            string file = TempFileWithContent(loremipsum);
            ShellCommand shellCommand = new ShellCommand()
            {
                CommandList = new List<SimpleCommand>
                {
                    new SimpleCommand(catCommand, new List<string> { file }),
                    new SimpleCommand(wcCommand, new List<string> { "-w" }),
                }
            };
            CommandProcessor commandProcessor = new CommandProcessor(shellCommand, stdOut, stdErr);

            commandProcessor.Run();

            outSb.ToString().Should().Be($"104{Environment.NewLine}");
            errSb.ToString().Should().BeNullOrEmpty();
        }

        private static string TempFileWithContent(string content)
        {
            string tempFile = Path.GetTempFileName();

            using (var writer = new StreamWriter(tempFile, false))
            {
                writer.Write(content);
            }

            return tempFile;
        }

        private static StringWriter OutputStream(out StringBuilder sb)
        {
            sb = new StringBuilder();
            StringWriter outStream = new StringWriter(sb);
            return outStream;
        }
    }
}
