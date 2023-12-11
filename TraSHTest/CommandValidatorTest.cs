// MIT License - Copyright (c) Vishnu Vijayan
// This file is subject to the terms and conditions defined in
// LICENSE, which is part of this source code package

namespace TraSHTest
{
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using FluentAssertions;
    using TraSH.Builtins;
    using TraSH;
    using System;

    [TestClass]
    public sealed class CommandValidatorTest
    {
        private static readonly string CatCommand = @"C:\Program Files (x86)\GnuWin32\bin\cat.exe";

        [TestMethod]
        public void ValidatesBuiltInCommand()
        {
            foreach(var cmd in BuiltInCommands.Map.Keys)
            {
                CommandValidator.Validate(cmd).Should().BeTrue();
            }
        }

        [DataRow("C:\\Windows\\System32\\cmd.exe")]
        [DataRow("cmd.exe")]
        [DataRow("cmd")]
        [TestMethod]
        public void ReturnsTrueWithValidExternalCommand(string command)
        {
            CommandValidator.Validate(command).Should().BeTrue();
        }

        [TestMethod]
        public void ReturnsFalseWithInvalidExternalCommand()
        {
            CommandValidator.Validate(Guid.NewGuid().ToString()).Should().BeFalse();
        }
    }
}
