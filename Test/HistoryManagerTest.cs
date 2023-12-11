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
    using TraSH;

    [TestClass]
    public class HistoryManagerTest
    {
        private static readonly List<string> mockHistory = new List<string>
        {
            "git rest --hard",
            @"cd C:\Users\testuser\Documents\Projects\TraSH\TraSH\bin\Debug\netcoreapp2.2",
            "ls -al",
            "git commit -m \"Initial commit\"",
            "git status",
        };

        [TestMethod]
        public void TestFileCreated()
        {
            string filepath = Path.Join(Path.GetTempPath(), Guid.NewGuid().ToString());
            var hm = new HistoryManager(filepath);

            hm.Start().Wait();

            File.Exists(filepath).Should().BeTrue();

            File.Delete(filepath);
        }

        [TestMethod]
        public void TestHistoryLoaded()
        {
            string filepath = Path.GetTempFileName();
            using (TextWriter tw = new StreamWriter(filepath))
            {
                mockHistory.ForEach(s => tw.WriteLine(s));
            }
            var hm = new HistoryManager(filepath);

            hm.Start().Wait();
            IEnumerable<string> actualHistory = hm.GetHistory();

            actualHistory.Should().BeEquivalentTo(mockHistory, options => options.WithStrictOrdering());

            File.Delete(filepath);
        }

        [TestMethod]
        public void TestNewLineSavedToFile()
        {
            string filepath = Path.GetTempFileName();
            var hm = new HistoryManager(filepath);

            hm.Start().Wait();
            hm.Add("newcommand");
            IEnumerable<string> actualHistory = hm.GetHistory();
            IEnumerable<string> fileHistory = hm.GetFileHistory();

            actualHistory.Should().BeEquivalentTo(new List<string> { "newcommand" });
            fileHistory.Should().BeEquivalentTo(new List<string> { "newcommand" });

            File.Delete(filepath);
        }

        [TestMethod]
        public void TestAddUniqueLine()
        {
            string filepath = Path.GetTempFileName();
            List<string> expectedHistory = mockHistory.ToList();
            expectedHistory.Add("newcommand");
            using (TextWriter tw = new StreamWriter(filepath))
            {
                mockHistory.ForEach(s => tw.WriteLine(s));
            }
            var hm = new HistoryManager(filepath);

            hm.Start().Wait();
            hm.Add("newcommand");
            IEnumerable<string> actualHistory = hm.GetHistory();
            IEnumerable<string> fileHistory = hm.GetFileHistory();

            actualHistory.Should().BeEquivalentTo(expectedHistory);
            fileHistory.Should().BeEquivalentTo(expectedHistory);

            File.Delete(filepath);
        }

        [TestMethod]
        public void TestAddDuplicateLine()
        {
            string filepath = Path.GetTempFileName();
            List<string> expectedHistory = mockHistory.ToList();
            expectedHistory.Add("ls -al");
            var hm = new HistoryManager(filepath);

            hm.Start().Wait();
            mockHistory.ForEach(s => hm.Add(s));
            hm.Add("ls -al");
            IEnumerable<string> actualHistory = hm.GetHistory();
            IEnumerable<string> fileHistory = hm.GetFileHistory();

            actualHistory.Should().BeEquivalentTo(expectedHistory, options => options.WithStrictOrdering());
            fileHistory.Should().BeEquivalentTo(expectedHistory, options => options.WithStrictOrdering());

            File.Delete(filepath);
        }

        [TestMethod]
        public void IgnoresEmptyLine()
        {
            string filepath = Path.GetTempFileName();
            var hm = new HistoryManager(filepath);

            hm.Start().Wait();
            hm.Add("c1");
            hm.Add("  ");
            hm.Add("c2");
            IEnumerable<string> actualHistory = hm.GetHistory();
            IEnumerable<string> fileHistory = hm.GetFileHistory();

            actualHistory.Should().BeEquivalentTo(new List<string> { "c1", "c2" });
            fileHistory.Should().BeEquivalentTo(new List<string> { "c1", "c2" });

            File.Delete(filepath);
        }

        [TestMethod]
        public void TestMultilineLine()
        {
            string filepath = Path.GetTempFileName();
            List<string> expectedHistory = new List<string> {
                "echo \"line 1\\n line 2\\n line 3\"",
                "echo \"line 4\\n line 5\""
            };
            using (TextWriter tw = new StreamWriter(filepath))
            {
                tw.WriteLine("echo \"line 1\\n line 2\\n line 3\"");
            }
            var hm = new HistoryManager(filepath);

            hm.Start().Wait();
            hm.Add("echo \"line 4\n line 5\"");
            IEnumerable<string> actualHistory = hm.GetHistory();
            IEnumerable<string> fileHistory = hm.GetFileHistory();

            actualHistory.Should().BeEquivalentTo(expectedHistory, options => options.WithStrictOrdering());
            fileHistory.Should().BeEquivalentTo(expectedHistory, options => options.WithStrictOrdering());

            File.Delete(filepath);
        }

        [TestMethod]
        public void TestAutocompletePrefixExists()
        {
            string filepath = Path.GetTempFileName();
            List<string> expectedSuggestions = mockHistory.Where(q => q.StartsWith("git", StringComparison.Ordinal)).ToList();
            var hm = new HistoryManager(filepath);

            hm.Start().Wait();
            mockHistory.ForEach(hm.Add);
            IEnumerable<string> actualSuggestions = hm.AutoComplete("git");

            actualSuggestions.Should().BeEquivalentTo(expectedSuggestions);

            File.Delete(filepath);
        }

        [TestMethod]
        public void TestAutocompletePrefixDoesNotExist()
        {
            string filepath = Path.GetTempFileName();
            var hm = new HistoryManager(filepath);

            hm.Start().Wait();
            mockHistory.ForEach(hm.Add);
            IEnumerable<string> actualSuggestions = hm.AutoComplete("shouldnotexist");

            actualSuggestions.Should().BeEquivalentTo(Enumerable.Empty<string>());

            File.Delete(filepath);
        }

        [TestMethod]
        public void TestAutocompleteNoPrefix()
        {
            string filepath = Path.GetTempFileName();
            List<string> expectedHistory = mockHistory.ToList();
            expectedHistory.Reverse();

            var hm = new HistoryManager(filepath);
            mockHistory.ForEach(hm.Add);
            IEnumerable<string> actualSuggestions = hm.AutoComplete(string.Empty);

            actualSuggestions.Should().BeEquivalentTo(expectedHistory, options => options.WithStrictOrdering());
        }
    }
}
