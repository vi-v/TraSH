namespace TraSH
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Text;
    using System.Threading;

    public class LineEditor
    {
        public EventHandler<string> LineReceived;
        private readonly StringBuilder buffer;
        private readonly IReadOnlyDictionary<ConsoleKey, Action<ConsoleKeyInfo>> consoleKeyMap;
        private readonly Cursor cursor;

        public LineEditor()
        {
            this.buffer = new StringBuilder();
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
                { ConsoleKey.V, this.HandlePaste },
                { ConsoleKey.LeftWindows, this.IgnoreCharacter },
                { ConsoleKey.RightWindows, this.IgnoreCharacter }
            };

            this.cursor = new Cursor(this.buffer, this.GetPrompt);
        }

        public void Start()
        {
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

        public void PrintPrompt()
        {
            Console.ForegroundColor = ConsoleColor.Green;
            Console.Write(this.GetPrompt());
            Console.ResetColor();
        }

        private void HandleEnter(ConsoleKeyInfo c)
        {
            Console.WriteLine();
            if (this.buffer.Length > 0)
            {
                this.LineReceived?.Invoke(this, this.buffer.ToString());
                this.buffer.Clear();
            }
            else
            {
                this.PrintPrompt();
            }
        }

        private void HandleBackspace(ConsoleKeyInfo c)
        {
            if (this.buffer.Length > 0 && this.cursor.RelativePosition > 0)
            {
                int delPosition = this.cursor.RelativePosition - 1;
                int delCount = this.buffer.Length - this.cursor.RelativePosition;
                this.buffer.Remove(delPosition, 1);
                this.cursor.MoveLeft();
                this.ReplaceBufferAndMoveCursor(delCount + 1);
            }
        }

        private void HandleDelete(ConsoleKeyInfo c)
        {
            if (this.buffer.Length > 0 && this.cursor.RelativePosition < this.buffer.Length)
            {
                int delPosition = this.cursor.RelativePosition;
                int delCount = this.buffer.Length - this.cursor.RelativePosition;
                this.buffer.Remove(delPosition, 1);
                this.ReplaceBufferAndMoveCursor(delCount);
            }
        }

        private void ReplaceBufferAndMoveCursor(int delCount)
        {
            for (int i = this.cursor.RelativePosition; i < this.buffer.Length; i++)
            {
                Console.Write(this.buffer[i]);
            }

            Console.Write(" ");
            this.cursor.MoveLeft(delCount);
        }

        private void HandleLeftArrow(ConsoleKeyInfo c)
        {
            this.cursor.MoveLeft();
        }

        private void HandleRightArrow(ConsoleKeyInfo c)
        {
            this.cursor.MoveRight();
        }

        private void HandleUpArrow(ConsoleKeyInfo c)
        {

        }

        private void HandleDownArrow(ConsoleKeyInfo c)
        {

        }

        private void HandleHome(ConsoleKeyInfo c)
        {
            this.cursor.MoveFront();
        }

        private void HandleEnd(ConsoleKeyInfo c)
        {
            this.cursor.MoveLast();
        }

        private void HandlePaste(ConsoleKeyInfo c)
        {
            if ((c.Modifiers & ConsoleModifiers.Control) != 0)
            {
                string clipboardText = TextCopy.Clipboard.GetText();
                this.InsertStringAtCursor(clipboardText);
            }
            else
            {
                this.HandleCharacter(c);
            }
        }

        private void HandleCharacter(ConsoleKeyInfo c)
        {
            this.InsertStringAtCursor(c.KeyChar.ToString());
        }

        private void IgnoreCharacter(ConsoleKeyInfo c) { }

        private void InsertStringAtCursor(string text)
        {
            string remainingBufText = this.buffer.ToString(this.cursor.RelativePosition, this.buffer.Length - this.cursor.RelativePosition);
            this.buffer.Insert(this.cursor.RelativePosition, text);

            Console.ForegroundColor = ConsoleColor.Red;
            Console.Write(text);
            Console.Write(remainingBufText);
            Console.ResetColor();

            this.cursor.MoveLeft(remainingBufText.Length);
        }

        private string GetPrompt()
        {
            return $"{Environment.MachineName}:{new DirectoryInfo(Environment.CurrentDirectory).Name} > ";
        }

        private class Cursor
        {
            private readonly StringBuilder buffer;
            private readonly Func<string> getPromptFunc;

            public Cursor(StringBuilder buffer, Func<string> getPromptFunc)
            {
                this.getPromptFunc = getPromptFunc;
                this.buffer = buffer;
            }

            public int RelativePosition { get => Console.CursorLeft - this.PromptLength; }

            public int Position { get => Console.CursorLeft; }

            public void MoveLeft()
            {
                this.MoveLeft(1);
            }

            public void MoveLeft(int count)
            {
                if (this.RelativePosition > 0)
                {
                    Console.SetCursorPosition(Math.Max(this.Position - count, this.PromptLength), Console.CursorTop);
                }
            }

            public void MoveRight()
            {
                this.MoveRight(1);
            }

            public void MoveRight(int count)
            {
                if (this.RelativePosition < this.buffer.Length)
                {
                    Console.SetCursorPosition(Math.Min(this.Position + count, this.buffer.Length + this.PromptLength), Console.CursorTop);
                }
            }

            public void MoveFront()
            {
                Console.SetCursorPosition(this.PromptLength, Console.CursorTop);
            }

            public void MoveLast()
            {
                Console.SetCursorPosition(this.PromptLength + this.buffer.Length, Console.CursorTop);
            }

            private int PromptLength { get => this.getPromptFunc().Length; }
        }
    }
}
