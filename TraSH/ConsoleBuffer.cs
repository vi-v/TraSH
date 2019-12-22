namespace TraSH
{
    using System;
    using System.Collections.Generic;
    using System.Text;

    public class ConsoleBuffer
    {
        private readonly Func<string> getLeftPrompt;
        private readonly Func<string> getRightPrompt;
        private readonly List<StringBuilder> buffer;
        private int oldBufferWidth;
        private int relativeTop;
        private int absoluteTop;

        public ConsoleBuffer(Func<string> getLeftPrompt) : this(getLeftPrompt, () => string.Empty)
        {
        }

        public ConsoleBuffer(Func<string> getLeftPrompt, Func<string> getRightPrompt)
        {
            this.getLeftPrompt = getLeftPrompt;
            this.getRightPrompt = getRightPrompt;
            this.buffer = new List<StringBuilder> { new StringBuilder() };
            this.oldBufferWidth = Console.BufferWidth;
            this.relativeTop = 0;
            this.absoluteTop = Console.CursorTop;

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
            this.buffer[this.relativeTop].Append(c);
            Console.Write(c);
            this.IsEmpty = false;
        }

        public void MoveCursorLeft(int count = 1)
        {
            this.CheckDimensionsAndUpdateBuffer();
            int promptLength = this.getLeftPrompt().Length;
            int promptHeight = promptLength / Console.BufferWidth;
            int promptWidth = promptLength % Console.BufferWidth;

            int cursorTop = this.relativeTop;
            int targetPosition = -1 * count - (this.buffer[cursorTop].Length - Console.CursorLeft);
            int targetLine = this.buffer.Count - 1;
            for (int i = cursorTop; targetPosition < 0 && i >= 0; i--)
            {
                targetLine = i;
                targetPosition += this.buffer[i].Length;
            }

            if (targetLine <= promptHeight)
            {
                targetLine = promptHeight;
                targetPosition = Math.Max(targetPosition, promptWidth);
            }

            this.relativeTop = targetLine;
            Console.SetCursorPosition(targetPosition, this.absoluteTop + targetLine);
        }

        public void Reset()
        {
            this.buffer.Clear();
            this.buffer.Add(new StringBuilder());

            foreach (char c in this.getLeftPrompt())
            {
                this.PutChar(c);
            }
            this.IsEmpty = true;
        }

        public string ToString(bool includePrompt = false)
        {
            StringBuilder tempBuffer = new StringBuilder();
            this.buffer.ForEach(b => tempBuffer.Append(b));

            if (!includePrompt)
            {
                tempBuffer.Remove(0, this.getLeftPrompt().Length);
            }

            return tempBuffer.ToString();
        }

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

                    this.buffer[this.relativeTop].Append(c);
                    Console.Write(c);
                }
            }

            if (Console.CursorLeft >= Console.BufferWidth - 1)
            {
                this.relativeTop++;
                this.buffer.Add(new StringBuilder());
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
