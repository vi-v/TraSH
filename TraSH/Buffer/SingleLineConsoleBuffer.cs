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
            this.buffer.Insert(this.cursorPos, c);
            this.cursorPos++;
            this.IsEmpty = false;

            Console.Write(c);
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

            int cursorLine = this.buffer.Length / Console.BufferWidth;
            if (this.cursorPos > promptLength)
            {
                if (Console.CursorLeft == 0 && cursorLine > 0)
                {
                    Console.SetCursorPosition(Console.BufferWidth - 1, Console.CursorTop - 1);
                    this.cursorPos--;
                }
                else
                {
                    Console.CursorLeft -= 1;
                    this.cursorPos--;
                }
            }
        }

        private void MoveCursorRight()
        {
            int cursorLine = this.buffer.Length / Console.BufferWidth;
        }
    }
}
