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

        public ConsoleBuffer(Func<string> getLeftPrompt) : this(getLeftPrompt, () => string.Empty)
        {
        }

        public ConsoleBuffer(Func<string> getLeftPrompt, Func<string> getRightPrompt)
        {
            this.getLeftPrompt = getLeftPrompt;
            this.getRightPrompt = getRightPrompt;
            this.buffer = new List<StringBuilder> { new StringBuilder() };
            this.oldBufferWidth = Console.BufferWidth;

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
            if (Console.BufferWidth != this.oldBufferWidth)
            {
                this.oldBufferWidth = Console.BufferWidth;
                StringBuilder tempBuffer = new StringBuilder();
                this.buffer.ForEach(b => tempBuffer.Append(b));
                this.buffer.Clear();
                this.ClearConsoleLine();


                for (int i = 0; i < tempBuffer.Length; i++)
                {
                    char c = tempBuffer[i];

                    if (i % Console.BufferWidth == 0)
                    {
                        this.buffer.Add(new StringBuilder());
                    }

                    this.buffer[Console.CursorTop].Append(c);
                    Console.Write(c);
                }
            }

            if (Console.CursorLeft >= Console.BufferWidth - 1)
            {
                this.buffer.Add(new StringBuilder());
            }
        }

        private void ClearConsoleLine()
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
