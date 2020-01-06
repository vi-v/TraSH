// MIT License - Copyright (c) Vishnu Vijayan
// This file is subject to the terms and conditions defined in
// LICENSE, which is part of this source code package

namespace TraSH
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Text;
    using System.Text.RegularExpressions;
    using System.Threading.Tasks;
    using Gma.DataStructures.StringSearch;
    using VDS.Common.Tries;

    public class HistoryManager
    {
        private readonly string filepath;
        private readonly PatriciaTrie<int> autocompleteTrie;
        private List<string> historyList;
        private int numCommands;

        public HistoryManager() : this(Path.Join(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), ".trash_history"))
        {
        }

        public HistoryManager(string filepath)
        {
            this.filepath = filepath;
            this.historyList = new List<string>();
            this.autocompleteTrie = new PatriciaTrie<int>();
            this.numCommands = 0;
        }

        public Task Start()
        {
            return Task.Factory.StartNew(() =>
            {
                if (!File.Exists(this.filepath))
                {
                    FileStream historyFile = File.Create(this.filepath);
                    historyFile.Close();
                }

                using (StreamReader fileReader = new StreamReader(this.filepath))
                {
                    string line;
                    while ((line = fileReader.ReadLine()) != null)
                    {
                        this.numCommands++;
                        this.historyList.Add(line);
                        this.autocompleteTrie.Add(line, this.numCommands);
                    }
                }
            });
        }

        public IEnumerable<string> GetHistory()
        {
            return this.historyList.AsEnumerable();
        }

        public IEnumerable<string> GetFileHistory()
        {
            return File.ReadAllLines(this.filepath);
        }

        public void Add(string newLine)
        {
            this.numCommands++;
            string escapedLine = this.Escape(newLine);

            using (StreamWriter fileWriter = new StreamWriter(this.filepath, append: true))
            {
                this.historyList.Add(escapedLine);
                fileWriter.WriteLine(escapedLine);
            }

            this.autocompleteTrie.Add(escapedLine, this.numCommands);
        }

        public IEnumerable<string> AutoComplete(string prefix)
        {
            if (string.IsNullOrEmpty(prefix))
            {
                return this.GetHistory()
                   .Reverse()
                   .Distinct();
            }

            return this.autocompleteTrie
                .Retrieve(prefix)
                .ToList()
                .OrderByDescending(ln => ln)
                .Select(ln => this.historyList[ln - 1])
                .Distinct();
        }

        private string Escape(string input)
        {
            return Regex.Replace(input, @"\r\n?|\n", "\\n");
        }
    }
}
