using Antlr4.Runtime;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using System.Linq;
using TraSH;
using TraSH.Gen;
using TraSH.Model;
using static TraSH.Gen.ShellParser;

namespace TraSHTest
{
    [TestClass]
    public class ShellVisitorTest
    {
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
