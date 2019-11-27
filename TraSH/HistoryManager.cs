﻿namespace TraSH
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Text;
    using System.Text.RegularExpressions;
    using System.Threading.Tasks;

    public class HistoryManager
    {
        private readonly string filepath;
        private readonly OrderedHashSet<string> historySet;

        public HistoryManager()
        {
            this.filepath = Path.Join(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), ".trash_history");
        }

        public HistoryManager(string filepath)
        {
            this.filepath = filepath;
            this.historySet = new OrderedHashSet<string>();
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
                        this.historySet.Add(line);
                    }
                }
            });
        }

        public IEnumerable<string> GetHistory()
        {
            return this.historySet;
        }

        public IEnumerable<string> GetFileHistory()
        {
            return File.ReadAllLines(this.filepath);
        }

        public void Add(string newLine)
        {
            string escapedLine = this.Escape(newLine);

            bool shouldWriteAll = false;
            if (this.historySet.Contains(escapedLine))
            {
                this.historySet.Remove(escapedLine);
                File.WriteAllText(this.filepath, string.Empty);
                shouldWriteAll = true;
            }

            using (StreamWriter fileWriter = new StreamWriter(this.filepath, append: true))
            {
                if (shouldWriteAll)
                {
                    this.historySet.ToList().ForEach(s => fileWriter.WriteLine(this.Escape(s)));
                }

                this.historySet.Add(escapedLine);
                fileWriter.WriteLine(escapedLine);
            }
        }

        private string Escape(string input)
        {
            return Regex.Replace(input, @"\r\n?|\n", "\\n");
        }
    }
}
