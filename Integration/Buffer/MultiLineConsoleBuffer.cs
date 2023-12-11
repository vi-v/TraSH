// MIT License - Copyright (c) Vishnu Vijayan
// This file is subject to the terms and conditions defined in
// LICENSE, which is part of this source code package

namespace TraSH.Buffer
{
    using System;
    using System.Collections.Generic;
    using System.Text;

    internal sealed class MultiLineConsoleBuffer
    {
        private readonly Func<string> getLeftPrompt;
        private readonly Func<string> getRightPrompt;
        private readonly List<StringBuilder> buffer;
        private readonly int initialCursorTop;
        private int oldBufferWidth;

        public MultiLineConsoleBuffer(Func<string> getLeftPrompt) : this(getLeftPrompt, () => string.Empty)
        {
        }

        public MultiLineConsoleBuffer(Func<string> getLeftPrompt, Func<string> getRightPrompt)
        {
            this.getLeftPrompt = getLeftPrompt;
            this.getRightPrompt = getRightPrompt;
            this.buffer = new List<StringBuilder> { new StringBuilder() };
            this.initialCursorTop = Console.CursorTop;
            this.oldBufferWidth = Console.BufferWidth;

            foreach (char c in this.getLeftPrompt())
            {
                this.PutChar(c);
            }
            this.IsEmpty = true;
        }

        public bool IsEmpty { get; private set; }

        public void Write(IEnumerable<char> s)
        {
            foreach (char c in s)
            {
                this.PutChar(c);
            }
        }

        public void PutChar(char c)
        {
            this.CheckDimensionsAndUpdateBuffer();

            if (Console.CursorLeft == Console.BufferWidth - 1 && this.RelativeCursorTop == this.buffer.Count - 1)
            {
                this.buffer.Add(new StringBuilder());
            }

            this.buffer[this.RelativeCursorTop].Append(c);
            Console.Write(c);

            this.IsEmpty = false;
        }

        public void MoveCursorLeft(int count = 1)
        {
            this.CheckDimensionsAndUpdateBuffer();
            int promptLength = this.getLeftPrompt().Length;
            int promptHeight = promptLength / Console.BufferWidth;
            int promptWidth = promptLength % Console.BufferWidth;

            //var ct = Console.CursorTop;
            //var wt = Console.WindowTop;
            //Console.WriteLine($"wt:{wt} ct:{ct}");
            //int cursorTop = Console.CursorTop - Console.WindowTop;
            //int targetLine = cursorTop;
            //int targetPosition = -1 * count - (this.buffer[cursorTop].Length - Console.CursorLeft);
            //for (int i = cursorTop; targetPosition < 0 && i >= 0; i--)
            //{
            //    targetLine = i;
            //    targetPosition += this.buffer[i].Length;
            //}

            //if (targetLine <= promptHeight)
            //{
            //    targetLine = promptHeight;
            //    targetPosition = Math.Max(targetPosition, promptWidth);
            //}

            //Console.SetCursorPosition(targetPosition, Console.WindowTop + targetLine);

            if (this.RelativeCursorTop > promptHeight)
            {
                if (Console.CursorLeft == 0)
                {
                    Console.CursorTop -= 1;
                    Console.CursorLeft = Console.BufferWidth;
                }

                Console.CursorLeft -= 1;
            }
            else
            {
                if (Console.CursorLeft > promptWidth)
                {
                    Console.CursorLeft -= 1;
                }
            }
        }

        public void MoveCursorRight(int count) { }

        public override string ToString()
        {
            StringBuilder tempBuffer = new StringBuilder();
            this.buffer.ForEach(b => tempBuffer.Append(b));

            return tempBuffer.ToString();
        }

        public string GetContent()
        {
            StringBuilder tempBuffer = new StringBuilder(this.ToString());
            tempBuffer.Remove(0, this.getLeftPrompt().Length);

            return tempBuffer.ToString();
        }

        private int RelativeCursorTop { get => Console.CursorTop - this.initialCursorTop; }

        private void CheckDimensionsAndUpdateBuffer()
        {
            if (Console.BufferWidth != this.oldBufferWidth)
            {
                this.oldBufferWidth = Console.BufferWidth;
                StringBuilder tempBuffer = new StringBuilder();
                this.buffer.ForEach(b => tempBuffer.Append(b));
                this.buffer.Clear();
                this.ClearExternalBuffer();

                for (int i = 0; i < tempBuffer.Length; i++)
                {
                    char c = tempBuffer[i];

                    if (i % Console.BufferWidth == 0)
                    {
                        this.buffer.Add(new StringBuilder());
                    }

                    this.buffer[this.RelativeCursorTop].Append(c);
                    Console.Write(c);
                }
            }
        }

        private void ClearExternalBuffer()
        {
            for (int i = 0; i < Console.BufferHeight; i++)
            {
                Console.SetCursorPosition(0, i);
                Console.Write(new string(' ', Console.BufferWidth));
            }
            Console.SetCursorPosition(0, 0);
        }
    }
}
