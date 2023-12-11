// MIT License - Copyright (c) Vishnu Vijayan
// This file is subject to the terms and conditions defined in
// LICENSE, which is part of this source code package

namespace TraSH.Test
{
    using FluentAssertions;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using System.Collections.Generic;
    using System.Diagnostics;
    using TraSH.Model;

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

        [TestMethod]
        public void AsProcessTest()
        {
            var sc = new SimpleCommand(
                "git",
                new List<string> { "reset", "--hard" });

            Process proc = sc.AsProcess();

            proc.StartInfo.FileName.Should().Be("git");
            proc.StartInfo.ArgumentList.Should().BeEquivalentTo(new List<string> { "reset", "--hard" }, opt => opt.WithStrictOrdering());
            proc.StartInfo.RedirectStandardInput.Should().BeTrue();
            proc.StartInfo.RedirectStandardOutput.Should().BeTrue();
            proc.StartInfo.RedirectStandardError.Should().BeTrue();
            proc.StartInfo.UseShellExecute.Should().BeFalse();
        }
    }
}
