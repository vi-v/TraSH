// MIT License - Copyright (c) Vishnu Vijayan
// This file is subject to the terms and conditions defined in
// LICENSE, which is part of this source code package

namespace TraSH.Test
{
    using Antlr4.Runtime;
    using FluentAssertions;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using System.Collections.Generic;
    using System.Linq;
    using TraSH;
    using TraSH.Gen;
    using TraSH.Model;
    using static TraSH.Gen.ShellParser;

    [TestClass]
    public class ShellVisitorTest
    {
        [TestMethod]
        public void ShellCommandOnlyPipesTest()
        {
            ShellParser shellParser = MakeParser("git reset --hard | echo");
            ShellCommandContext context = shellParser.shellCommand();
            ShellVisitor visitor = new ShellVisitor();

            ParserResult result = visitor.Visit(context);
            ShellCommand shellCommand = result.ShellCommandValue;

            result.IsShellCommand.Should().BeTrue();
            shellCommand.IsBackground.Should().BeFalse();
            shellCommand.CommandList.Should().HaveCount(2);
        }

        [TestMethod]
        public void ShellCommandSingleCommandTest()
        {
            ShellParser shellParser = MakeParser("git reset --hard");
            ShellCommandContext context = shellParser.shellCommand();
            ShellVisitor visitor = new ShellVisitor();

            ParserResult result = visitor.Visit(context);
            ShellCommand shellCommand = result.ShellCommandValue;

            result.IsShellCommand.Should().BeTrue();
            shellCommand.IsBackground.Should().BeFalse();
            shellCommand.CommandList.Should().HaveCount(1);
            shellCommand.CommandList[0].ToString().Should().Be("git reset --hard");
        }

        [TestMethod]
        public void PipeListOneCommandTest()
        {
            ShellParser shellParser = MakeParser("git reset --hard");
            PipeListContext context = shellParser.pipeList();
            ShellVisitor visitor = new ShellVisitor();

            ParserResult result = visitor.Visit(context);
            List<SimpleCommand> actualPipeList = result.PipeListValue;

            result.IsPipeList.Should().BeTrue();
            actualPipeList.Should().HaveCount(1);
            actualPipeList[0].ToString().Should().Be("git reset --hard");
        }

        [TestMethod]
        public void PipeListTwoCommandTest()
        {
            ShellParser shellParser = MakeParser("git reset --hard | echo");
            PipeListContext context = shellParser.pipeList();
            ShellVisitor visitor = new ShellVisitor();

            ParserResult result = visitor.Visit(context);
            List<SimpleCommand> actualPipeList = result.PipeListValue;

            result.IsPipeList.Should().BeTrue();
            actualPipeList.Should().HaveCount(2);
            actualPipeList[0].ToString().Should().Be("git reset --hard");
            actualPipeList[1].ToString().Should().Be("echo");
        }

        [TestMethod]
        public void SimpleCommandWithArgsTest()
        {
            ShellParser shellParser = MakeParser("git reset --hard");
            SimpleCommandContext context = shellParser.simpleCommand();
            ShellVisitor visitor = new ShellVisitor();

            ParserResult result = visitor.Visit(context);
            SimpleCommand actualCommand = result.SimpleCommandValue;

            result.IsSimpleCommand.Should().BeTrue();
            actualCommand.Command.Should().Be("git");
            actualCommand.Arguments.Should().BeEquivalentTo(new List<string> { "reset", "--hard" }, opt => opt.WithStrictOrdering());
        }

        [TestMethod]
        public void SimpleCommandNoArgsTest()
        {
            ShellParser shellParser = MakeParser("git");
            SimpleCommandContext context = shellParser.simpleCommand();
            ShellVisitor visitor = new ShellVisitor();

            ParserResult result = visitor.Visit(context);
            SimpleCommand actualCommand = result.SimpleCommandValue;

            result.IsSimpleCommand.Should().BeTrue();
            actualCommand.Command.Should().Be("git");
            actualCommand.Arguments.Should().BeEmpty();
        }

        [TestMethod]
        public void CmdTest()
        {
            ShellParser shellParser = MakeParser("cmdword");
            CmdContext context = shellParser.cmd();
            ShellVisitor visitor = new ShellVisitor();

            ParserResult result = visitor.Visit(context);

            result.IsCmd.Should().BeTrue();
            result.CmdValue.Should().Be("cmdword");
        }

        [TestMethod]
        public void ArgListOneWordTest()
        {
            ShellParser shellParser = MakeParser("wordarg");
            ArgsContext context = shellParser.args();
            ShellVisitor visitor = new ShellVisitor();

            ParserResult result = visitor.Visit(context);

            result.IsArgList.Should().BeTrue();
            result.ArgListValue.Should().BeEquivalentTo(new List<string> { "wordarg" }, opts => opts.WithStrictOrdering());
        }

        [TestMethod]
        public void ArgListManyWordsTest()
        {
            ShellParser shellParser = MakeParser("these are some args");
            ArgsContext context = shellParser.args();
            ShellVisitor visitor = new ShellVisitor();

            ParserResult result = visitor.Visit(context);

            result.IsArgList.Should().BeTrue();
            result.ArgListValue.Should().BeEquivalentTo(new List<string> { "these", "are", "some", "args" }, opts => opts.WithStrictOrdering());
        }

        [TestMethod]
        public void ArgListMixedArgsTest()
        {
            ShellParser shellParser = MakeParser("these are 'some args'");
            ArgsContext context = shellParser.args();
            ShellVisitor visitor = new ShellVisitor();

            ParserResult result = visitor.Visit(context);

            result.IsArgList.Should().BeTrue();
            result.ArgListValue.Should().BeEquivalentTo(new List<string> { "these", "are", "some args" }, opts => opts.WithStrictOrdering());
        }

        [TestMethod]
        public void ArgWordTest()
        {
            ShellParser shellParser = MakeParser("wordarg");
            ArgContext context = shellParser.arg();
            ShellVisitor visitor = new ShellVisitor();

            ParserResult result = visitor.Visit(context);

            result.IsArg.Should().BeTrue();
            result.ArgValue.Should().Be("wordarg");
        }

        [TestMethod]
        public void ArgStringPipeTest()
        {
            ShellParser shellParser = MakeParser("\"12345 | echo asdf\"");
            ArgContext context = shellParser.arg();
            ShellVisitor visitor = new ShellVisitor();

            ParserResult result = visitor.Visit(context);

            result.IsArg.Should().BeTrue();
            result.ArgValue.Should().Be("12345 | echo asdf");
        }

        [TestMethod]
        public void ArgStringDoubleQuoteTest()
        {
            ShellParser shellParser = MakeParser("\"string arg\"");
            ArgContext context = shellParser.arg();
            ShellVisitor visitor = new ShellVisitor();

            ParserResult result = visitor.Visit(context);

            result.IsArg.Should().BeTrue();
            result.ArgValue.Should().Be("string arg");
        }

        [TestMethod]
        public void ArgStringSingleQuoteTest()
        {
            ShellParser shellParser = MakeParser("'string arg'");
            ArgContext context = shellParser.arg();
            ShellVisitor visitor = new ShellVisitor();

            ParserResult result = visitor.Visit(context);

            result.IsArg.Should().BeTrue();
            result.ArgValue.Should().Be("string arg");
        }

        private static ShellParser MakeParser(string text)
        {
            AntlrInputStream inputStream = new AntlrInputStream(text);
            ShellLexer shellLexer = new ShellLexer(inputStream);
            CommonTokenStream commonTokenStream = new CommonTokenStream(shellLexer);
            ShellParser shellParser = new ShellParser(commonTokenStream);

            return shellParser;
        }
    }
}
