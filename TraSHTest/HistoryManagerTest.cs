using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using TraSH;

namespace TraSHTest
{
    [TestClass]
    public class HistoryManagerTest
    {
        private readonly List<string> mockHistory = new List<string>
        {
            @"cd C:\Users\testuser\Documents\Projects\TraSH\TraSH\bin\Debug\netcoreapp2.2",
            "ls -al",
            "git commit -m \"Initial commit\""
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
                this.mockHistory.ForEach(s => tw.WriteLine(s));
            }
            var hm = new HistoryManager(filepath);

            hm.Start().Wait();
            IEnumerable<string> actualHistory = hm.GetHistory();

            actualHistory.Should().BeEquivalentTo(this.mockHistory, options => options.WithStrictOrdering());

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
            IEnumerable<string> fileHistory = new List<string>(File.ReadAllLines(filepath));

            actualHistory.Should().BeEquivalentTo(new List<string> { "newcommand" });
            fileHistory.Should().BeEquivalentTo(new List<string> { "newcommand" });

            File.Delete(filepath);
        }

        [TestMethod]
        public void TestAddUniqueLine()
        {
            string filepath = Path.GetTempFileName();
            List<string> expectedHistory = new List<string>(mockHistory);
            expectedHistory.Add("newcommand");
            using (TextWriter tw = new StreamWriter(filepath))
            {
                this.mockHistory.ForEach(s => tw.WriteLine(s));
            }
            var hm = new HistoryManager(filepath);

            hm.Start().Wait();
            hm.Add("newcommand");
            IEnumerable<string> actualHistory = hm.GetHistory();
            IEnumerable<string> fileHistory = new List<string>(File.ReadAllLines(filepath));

            actualHistory.Should().BeEquivalentTo(expectedHistory);
            fileHistory.Should().BeEquivalentTo(expectedHistory);

            File.Delete(filepath);
        }

        [TestMethod]
        public void TestAddDuplicateLine()
        {
            string filepath = Path.GetTempFileName();
            List<string> expectedHistory = new List<string>
            {
                @"cd C:\Users\testuser\Documents\Projects\TraSH\TraSH\bin\Debug\netcoreapp2.2",
                "git commit -m \"Initial commit\"",
                "ls -al"
            };
            using (TextWriter tw = new StreamWriter(filepath))
            {
                this.mockHistory.ForEach(s => tw.WriteLine(s));
            }
            var hm = new HistoryManager(filepath);

            hm.Start().Wait();
            this.mockHistory.ForEach(s => hm.Add(s));
            hm.Add("ls -al");
            IEnumerable<string> actualHistory = hm.GetHistory();
            IEnumerable<string> fileHistory = new List<string>(File.ReadAllLines(filepath));

            actualHistory.Should().BeEquivalentTo(expectedHistory, options => options.WithStrictOrdering());
            fileHistory.Should().BeEquivalentTo(expectedHistory, options => options.WithStrictOrdering());

            File.Delete(filepath);
        }

        [TestMethod]
        public void TestMaximumLengthExceeded()
        {
            Assert.Fail();
        }

        [TestMethod]
        public void TestMultilineLine()
        {
            Assert.Fail();
        }
    }
}
