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

        public ConsoleBuffer(Func<string> getLeftPrompt) : this(getLeftPrompt, () => string.Empty)
        {
        }

        public ConsoleBuffer(Func<string> getLeftPrompt, Func<string> getRightPrompt)
        {
            this.getLeftPrompt = getLeftPrompt;
            this.getRightPrompt = getRightPrompt;
            this.buffer = new List<StringBuilder> { new StringBuilder() };

            foreach (char c in this.getLeftPrompt())
            {
                this.PutChar(c);
            }
        }

        public void PutChar(char c)
        {
            this.CheckDimensionsAndUpdateBuffer();
            this.buffer[Console.CursorTop].Append(c);
            Console.Write(c);
        }

        private void CheckDimensionsAndUpdateBuffer()
        {
            if (Console.CursorLeft >= Console.BufferWidth - 1)
            {
                this.buffer.Add(new StringBuilder());
            }
        }
    }
}
