// MIT License - Copyright (c) Vishnu Vijayan
// This file is subject to the terms and conditions defined in
// LICENSE, which is part of this source code package

namespace TraSH
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Threading.Tasks;
    using TraSH.Builtins;
    using TraSH.Buffer;

    public class LineEditor
    {
        public EventHandler<string> LineReceived;

        private readonly HistoryManager historyManager;
        private readonly IReadOnlyDictionary<ConsoleKey, Action<ConsoleKeyInfo>> consoleKeyMap;
        private readonly IReadOnlyDictionary<ConsoleKey, Action<ConsoleKeyInfo>> consoleKeyCtrlMap;
        private IConsoleBuffer consoleBuffer;
        private IEnumerator<string> suggestionCache;

        private bool useSuggestionCache;
        private string bufferCache;

        public LineEditor()
        {
            this.historyManager = new HistoryManager();
            this.consoleKeyMap = new Dictionary<ConsoleKey, Action<ConsoleKeyInfo>>
            {
                { ConsoleKey.Enter, this.HandleEnter },
                { ConsoleKey.Backspace, this.HandleBackspace },
                { ConsoleKey.Delete, this.HandleDelete },
                { ConsoleKey.LeftArrow, this.HandleLeftArrow },
                { ConsoleKey.RightArrow, this.HandleRightArrow },
                { ConsoleKey.UpArrow, this.HandleUpArrow },
                { ConsoleKey.DownArrow, this.HandleDownArrow },
                { ConsoleKey.Home,  this.HandleHome },
                { ConsoleKey.End,  this.HandleEnd },
                { ConsoleKey.LeftWindows, this.IgnoreCharacter },
                { ConsoleKey.RightWindows, this.IgnoreCharacter }
            };

            this.consoleKeyCtrlMap = new Dictionary<ConsoleKey, Action<ConsoleKeyInfo>>
            {
                { ConsoleKey.E, this.HandleEnd },
                { ConsoleKey.V, this.HandlePaste },
                { ConsoleKey.D, this.HandleExit },
                { ConsoleKey.U, this.HandleClearLine },
                { ConsoleKey.K, this.HandleClearAfterCursor }
            };

            this.consoleBuffer = this.NewConsoleBuffer();
            this.useSuggestionCache = false;
        }

        public void Start()
        {
            Task.Run(this.historyManager.Start);

            Console.CancelKeyPress += delegate (object sender, ConsoleCancelEventArgs e)
            {
                e.Cancel = true;
                Console.WriteLine();
                this.consoleBuffer = this.NewConsoleBuffer();
            };

            while (true)
            {
                ConsoleKeyInfo c = Console.ReadKey(true);
                if (this.consoleKeyMap.ContainsKey(c.Key))
                {
                    this.consoleKeyMap[c.Key](c);
                }
                else
                {
                    this.HandleCharacter(c);
                }
            }
        }

        private void HandleEnter(ConsoleKeyInfo c)
        {
            this.consoleBuffer.MoveCursorEnd();
            Console.WriteLine();

            if (!this.consoleBuffer.IsEmpty)
            {
                string newline = this.consoleBuffer.GetContent();
                this.historyManager.Add(newline);
                this.LineReceived?.Invoke(this, newline);
            }

            this.consoleBuffer = this.NewConsoleBuffer();
        }

        private void HandleBackspace(ConsoleKeyInfo c)
        {
            this.consoleBuffer.Backspace(1);
        }

        private void HandleDelete(ConsoleKeyInfo c)
        {
            this.consoleBuffer.Delete(1);
        }

        private void HandleLeftArrow(ConsoleKeyInfo c)
        {
            this.consoleBuffer.MoveCursorLeft(1);
        }

        private void HandleRightArrow(ConsoleKeyInfo c)
        {
            this.consoleBuffer.MoveCursorRight(1);
        }

        private void HandleUpArrow(ConsoleKeyInfo c)
        {
            if (!this.useSuggestionCache)
            {
                this.bufferCache = this.consoleBuffer.GetContent();
                this.suggestionCache = this.historyManager.AutoComplete(this.bufferCache).GetEnumerator();
                this.useSuggestionCache = true;
            }

            if (!this.suggestionCache.MoveNext())
            {
                this.suggestionCache = this.historyManager.AutoComplete(this.bufferCache).GetEnumerator();
            }
            string suggestion = this.suggestionCache.Current;
            if (!string.IsNullOrEmpty(suggestion))
            {
                suggestion = suggestion.Substring(this.bufferCache.Length, suggestion.Length - this.bufferCache.Length);
            }

            this.consoleBuffer.Clear();
            this.consoleBuffer.Write(this.bufferCache + suggestion);
        }

        private void HandleDownArrow(ConsoleKeyInfo c)
        {

        }

        private void HandleHome(ConsoleKeyInfo c)
        {
            this.consoleBuffer.MoveCursorHome();
        }

        private void HandleEnd(ConsoleKeyInfo c)
        {
            this.consoleBuffer.MoveCursorEnd();
        }

        private void HandlePaste(ConsoleKeyInfo c)
        {
            string clipboardText = TextCopy.Clipboard.GetText();
            this.consoleBuffer.Write(clipboardText);
        }

        private void HandleExit(ConsoleKeyInfo c)
        {
            if (this.consoleBuffer.IsEmpty)
            {
                Console.WriteLine();
                BuiltInCommands.Exit.Execute(Enumerable.Empty<string>());
            }
            else if (this.consoleBuffer.CursorPosition.X < this.consoleBuffer.Size.X)
            {
                this.HandleDelete(c);
            }
        }

        private void HandleClearLine(ConsoleKeyInfo c)
        {
            this.consoleBuffer.Clear();
        }

        private void HandleClearAfterCursor(ConsoleKeyInfo c)
        {
            this.consoleBuffer.Delete(this.consoleBuffer.Size.X - this.consoleBuffer.CursorPosition.X);
        }

        private void HandleCharacter(ConsoleKeyInfo c)
        {
            if (c.IsControlActive() && this.consoleKeyCtrlMap.ContainsKey(c.Key))
            {
                this.consoleKeyCtrlMap[c.Key](c);
            }
            else
            {
                this.useSuggestionCache = false;
                this.consoleBuffer.PutChar(c.KeyChar);
            }
        }

        private void IgnoreCharacter(ConsoleKeyInfo c) { }

        private IConsoleBuffer NewConsoleBuffer()
        {
            return new SingleLineConsoleBuffer(this.GetPrompt());
        }

        private string GetPrompt()
        {
            return $"{Environment.MachineName}:{new DirectoryInfo(Environment.CurrentDirectory).Name}> ";
        }
    }
}
