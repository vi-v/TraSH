namespace TraSHTest
{
    using System;
    using System.Collections.Generic;
    using System.Text;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using FluentAssertions;
    using TraSH.Model;
    using System.IO;
    using TraSH;

    [TestClass]
    public class CommandProcessorTest
    {
        [TestMethod]
        public void RunBuiltinCommandTest()
        {
            string dirPath = Path.Join(Path.GetTempPath(), Guid.NewGuid().ToString());
            DirectoryInfo newDir = Directory.CreateDirectory(dirPath);
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
    }
}
