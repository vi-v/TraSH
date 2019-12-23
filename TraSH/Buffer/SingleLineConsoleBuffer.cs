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

        public SingleLineConsoleBuffer(Func<string> getLeftPrompt)
        {
            this.getLeftPrompt = getLeftPrompt;
            this.relativeCursorTop = 0;
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
            return;
        }

        public void MoveCursorRight(int count)
        {
            return;
        }

        public void PutChar(char c)
        {
            Console.Write(c);

            int cursorPos = Console.CursorLeft - 1;

            //if (Console.CursorLeft == Console.BufferWidth - 1)
            //{
            //    this.relativeCursorTop++;
            //}

            int insertIndex = this.relativeCursorTop * (Console.BufferWidth - 1) + cursorPos;
            this.buffer.Insert(insertIndex, c);

            this.IsEmpty = false;
        }

        public void Write(IEnumerable<char> s)
        {
            foreach (char c in s)
            {
                this.PutChar(c);
            }
        }
    }
}
