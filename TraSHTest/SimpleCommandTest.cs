using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using TraSH.Model;

namespace TraSHTest
{
    [TestClass]
    public class SimpleCommandTest
    {
        [TestMethod]
        public void InitializeEmptyTest()
        {
            var sc = new SimpleCommand();

            sc.Command.Should().BeNullOrEmpty();
            sc.Arguments.Should().BeEmpty();
        }

        [TestMethod]
        public void InitializeCommandAndArgsTest()
        {
            var sc = new SimpleCommand(
                "git",
                new List<string> { "reset", "--hard" });

            sc.Command.Should().Be("git");
            sc.Arguments.Should().BeEquivalentTo(new List<string> { "reset", "--hard" }, opt => opt.WithStrictOrdering());
        }

        [TestMethod]
        public void ToStringTest()
        {
            var sc = new SimpleCommand(
                "git",
                new List<string> { "reset", "--hard" });
            string expectedString = "git reset --hard";

            string actualString = sc.ToString();

            actualString.Should().Be(expectedString);
        }
    }
}
