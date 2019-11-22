﻿namespace TraSH
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading;

    public class LineEditor
    {
        public EventHandler<string> LineReceived;
        private readonly StringBuilder buffer;
        private readonly Dictionary<ConsoleKey, Action<ConsoleKeyInfo>> consoleKeyMap;
        private readonly Cursor cursor;

        public LineEditor()
        {
            this.buffer = new StringBuilder();
            this.consoleKeyMap = new Dictionary<ConsoleKey, Action<ConsoleKeyInfo>>
            {
                { ConsoleKey.Enter, this.HandleEnter },
                { ConsoleKey.Backspace, this.HandleBackspace },
                { ConsoleKey.LeftArrow, this.HandleLeftArrow },
                { ConsoleKey.RightArrow, this.HandleRightArrow },
                { ConsoleKey.UpArrow, this.HandleUpArrow },
                { ConsoleKey.DownArrow, this.HandleDownArrow },
            };

            this.cursor = new Cursor(this.buffer, this.GetPrompt().Length);
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

                for (int i = this.cursor.RelativePosition; i < this.buffer.Length; i++)
                {
                    Console.Write(this.buffer[i]);
                }

                Console.Write(" ");

                for (int i = 0; i <= delCount; i++)
                {
                    this.cursor.MoveLeft();
                }
            }
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

        private void HandleCharacter(ConsoleKeyInfo c)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.Write(c.KeyChar);
            Console.ResetColor();
            this.buffer.Append(c.KeyChar);
        }

        private string GetPrompt()
        {
            return "TraSH> ";
        }

        private class Cursor
        {
            private readonly StringBuilder buffer;
            private readonly int promptLength;

            public Cursor(StringBuilder buffer, int promptLength)
            {
                this.promptLength = promptLength;
                this.buffer = buffer;
            }

            public int RelativePosition { get => Console.CursorLeft - this.promptLength; }

            public int Position { get => Console.CursorLeft; }

            public void MoveLeft()
            {
                if (this.RelativePosition > 0 && this.RelativePosition > 0)
                {
                    Console.SetCursorPosition(this.Position - 1, Console.CursorTop);
                }
            }

            public void MoveRight()
            {
                if (this.RelativePosition < this.buffer.Length)
                {
                    Console.SetCursorPosition(this.Position + 1, Console.CursorTop);
                }
            }
        }
    }
}