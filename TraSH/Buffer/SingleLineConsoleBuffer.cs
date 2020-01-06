// MIT License - Copyright (c) Vishnu Vijayan
// This file is subject to the terms and conditions defined in
// LICENSE, which is part of this source code package

namespace TraSH.Buffer
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;

    public class SingleLineConsoleBuffer : IConsoleBuffer
    {
        private readonly StringBuilder buffer;
        private readonly string prompt;
        private int cursorPos;

        public SingleLineConsoleBuffer(string prompt)
        {
            this.prompt = prompt;
            this.cursorPos = 0;
            this.buffer = new StringBuilder();

            this.Write(this.prompt);

            this.IsEmpty = true;
        }

        public bool IsEmpty { get; private set; }

        public Coordinate2 Size { get => new Coordinate2(this.buffer.Length, 0); }

        public Coordinate2 CursorPosition { get => new Coordinate2(this.cursorPos, 0); }

        public string GetContent()
        {
            StringBuilder tempBuffer = new StringBuilder(this.buffer.ToString());
            tempBuffer.Remove(0, this.prompt.Length);

            return tempBuffer.ToString();
        }

        public void Clear()
        {
            int contentLength = this.buffer.Length - this.prompt.Length;

            this.MoveCursorHome();
            this.buffer.Remove(this.cursorPos, this.buffer.Length - this.cursorPos);
            this.Write(new string(' ', contentLength));
            this.MoveCursorHome();
            this.buffer.Remove(this.cursorPos, this.buffer.Length - this.cursorPos);
        }

        public void MoveCursorEnd()
        {
            int bufferEnd = this.buffer.Length % Console.BufferWidth;
            int bufferHeight = this.buffer.Length / Console.BufferWidth + 1;

            Console.SetCursorPosition(bufferEnd, Console.CursorTop + bufferHeight - this.CursorLine - 1);
            this.cursorPos = this.buffer.Length;
        }

        public void MoveCursorHome()
        {
            int promptLength = this.prompt.Length;

            int adjustedCursorLine = this.CursorLine;
            if (Console.CursorLeft == Console.BufferWidth - 1 && this.cursorPos % Console.BufferWidth == 0)
            {
                adjustedCursorLine--;
            }

            Console.SetCursorPosition(promptLength, Console.CursorTop - adjustedCursorLine);
            this.cursorPos = promptLength;
        }

        public void MoveCursorLeft(int count)
        {
            for (int i = 0; i < count; i++)
            {
                this.MoveCursorLeft();
            }
        }

        public void MoveCursorRight(int count)
        {
            for (int i = 0; i < count; i++)
            {
                this.MoveCursorRight();
            }
        }

        public void PutChar(char c)
        {
            this.InsertStringAtCursor(c.ToString());
            this.IsEmpty = false;
        }

        public void Write(IEnumerable<char> s)
        {
            this.InsertStringAtCursor(string.Join("", s.ToList()));
            this.IsEmpty = false;
        }

        public void Delete(int count)
        {
            for (int i = 0; i < count; i++)
            {
                this.DeleteCharacter();
            }
        }

        public void Backspace(int count)
        {
            int promptLength = this.prompt.Length;
            int boundedCount = Math.Min(count, this.cursorPos - promptLength);
            this.MoveCursorLeft(boundedCount);
            this.Delete(boundedCount);
        }

        private int CursorLine { get => this.cursorPos / Console.BufferWidth; }

        private void MoveCursorLeft()
        {
            int promptLength = this.prompt.Length;

            if (this.cursorPos > promptLength)
            {
                if (Console.CursorLeft == 0 && this.CursorLine > 0)
                {
                    Console.SetCursorPosition(Console.BufferWidth - 1, Console.CursorTop - 1);
                }
                else
                {
                    Console.CursorLeft -= 1;
                }

                this.cursorPos--;
            }

            System.Diagnostics.Debug.WriteLine($"{DateTime.Now.ToString("hh:mm:ss fffffff")} <- {this.cursorPos}");
        }

        private void MoveCursorRight()
        {
            int bufferHeight = this.buffer.Length / Console.BufferWidth + 1;

            if (this.cursorPos < this.buffer.Length)
            {
                if (Console.CursorLeft == Console.BufferWidth - 1 && this.CursorLine < bufferHeight - 1)
                {
                    Console.SetCursorPosition(0, Console.CursorTop + 1);
                }
                else
                {
                    Console.CursorLeft += 1;
                }
                this.cursorPos++;
            }

            System.Diagnostics.Debug.WriteLine($"{DateTime.Now.ToString("hh:mm:ss fffffff")} -> {this.cursorPos}");
        }

        private void InsertStringAtCursor(string s)
        {
            if (string.IsNullOrEmpty(s))
            {
                return;
            }

            string remainingBuffer = this.buffer.RemainingSubstring(this.cursorPos);
            bool shouldAdvanceCursor = Console.CursorLeft == Console.BufferWidth - 1 && s.Length <= 1;

            this.buffer.Insert(this.cursorPos, s);

            Console.Write(s);
            this.cursorPos += s.Length;

            if (shouldAdvanceCursor)
            {
                Console.CursorLeft = 0;
                Console.CursorTop += 1;
            }

            int oldCursorLeft = Console.CursorLeft;
            Console.Write(remainingBuffer);
            Console.CursorTop -= (remainingBuffer.Length + oldCursorLeft - 1) / Console.BufferWidth;
            Console.CursorLeft = oldCursorLeft;
        }

        private void DeleteCharacter()
        {
            if (this.cursorPos < this.buffer.Length)
            {
                string emptySpace = "  ";
                string replacementBuffer = this.buffer.RemainingSubstring(this.cursorPos + 1) + emptySpace;

                this.buffer.Remove(this.cursorPos, 1);
                this.buffer.Append(emptySpace);

                Console.Write(replacementBuffer);
                this.cursorPos += replacementBuffer.Length;

                this.MoveCursorLeft(replacementBuffer.Length);
                this.buffer.Remove(this.buffer.Length - emptySpace.Length, emptySpace.Length);
            }
        }
    }
}
