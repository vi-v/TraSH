// MIT License - Copyright (c) Vishnu Vijayan
// This file is subject to the terms and conditions defined in
// LICENSE, which is part of this source code package

namespace TraSH.Test
{
    using FluentAssertions;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using TraSH;

    [TestClass]
    public class ExtensionsTest
    {
        [TestMethod]
        public void TestRemainingString()
        {
            string testString = "now this is podracing";
            string expectedString = "podracing";
            var sb = new StringBuilder(testString);

            string actualString = sb.RemainingSubstring(12);

            actualString.Should().Be(expectedString);
        }
    }
}
