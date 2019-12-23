namespace TraSH.Buffer
{
    using System;
    using System.Collections.Generic;
    using System.Text;

    public class SingleLineConsoleBuffer : IConsoleBuffer
    {
        private readonly Func<string> getLeftPrompt;
        private readonly StringBuilder buffer;
        private int relativeCursorTop;
        private int cursorPos;

        public SingleLineConsoleBuffer(Func<string> getLeftPrompt)
        {
            this.getLeftPrompt = getLeftPrompt;
            this.relativeCursorTop = 0;
            this.cursorPos = 0;
            this.buffer = new StringBuilder();

            foreach (char c in this.getLeftPrompt())
            {
                this.PutChar(c);
            }

            this.IsEmpty = true;
        }

        public bool IsEmpty { get; private set; }

        public string GetContent()
        {
            StringBuilder tempBuffer = new StringBuilder(this.buffer.ToString());
            tempBuffer.Remove(0, this.getLeftPrompt().Length);

            return tempBuffer.ToString();
        }

        public void MoveCursorEnd()
        {
            int bufferEnd = this.buffer.Length % Console.BufferWidth;
            int cursorLine = this.cursorPos / Console.BufferWidth;
            int bufferHeight = this.buffer.Length / Console.BufferWidth + 1;

            Console.SetCursorPosition(bufferEnd, Console.CursorTop + bufferHeight - cursorLine - 1);
            this.cursorPos = this.buffer.Length;
        }

        public void MoveCursorHome()
        {
            int cursorLine = this.cursorPos / Console.BufferWidth;
            int promptLength = this.getLeftPrompt().Length;

            Console.SetCursorPosition(promptLength, Console.CursorTop - cursorLine);
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
            this.cursorPos++;
            this.IsEmpty = false;
        }

        public void Write(IEnumerable<char> s)
        {
            foreach (char c in s)
            {
                this.PutChar(c);
            }
        }

        private void MoveCursorLeft()
        {
            int promptLength = this.getLeftPrompt().Length;
            int cursorLine = this.cursorPos / Console.BufferWidth;

            if (this.cursorPos > promptLength)
            {
                if (Console.CursorLeft == 0 && cursorLine > 0)
                {
                    Console.SetCursorPosition(Console.BufferWidth - 1, Console.CursorTop - 1);
                }
                else
                {
                    Console.CursorLeft -= 1;
                }

                this.cursorPos--;
            }
        }

        private void MoveCursorRight()
        {
            int cursorLine = this.cursorPos / Console.BufferWidth;
            int bufferHeight = this.buffer.Length / Console.BufferWidth + 1;

            if (this.cursorPos < this.buffer.Length)
            {
                if (Console.CursorLeft == Console.BufferWidth - 1 && cursorLine < bufferHeight - 1)
                {
                    Console.SetCursorPosition(0, Console.CursorTop + 1);
                }
                else
                {
                    Console.CursorLeft += 1;
                }
                this.cursorPos++;
            }
        }

        private void InsertStringAtCursor(string s)
        {
            string remainingBuffer = this.buffer.ToString(this.cursorPos, this.buffer.Length - this.cursorPos);
            this.buffer.Insert(this.cursorPos, s);

            Console.Write(s);
            Console.Write(remainingBuffer);

            this.cursorPos += remainingBuffer.Length;
            this.MoveCursorLeft(remainingBuffer.Length);
        }
    }
}
