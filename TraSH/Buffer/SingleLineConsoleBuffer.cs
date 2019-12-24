namespace TraSH.Buffer
{
    using System;
    using System.Collections.Generic;
    using System.Text;

    public class SingleLineConsoleBuffer : IConsoleBuffer
    {
        private readonly Func<string> getLeftPrompt;
        private readonly StringBuilder buffer;
        private int cursorPos;

        public SingleLineConsoleBuffer(Func<string> getLeftPrompt)
        {
            this.getLeftPrompt = getLeftPrompt;
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
            int bufferHeight = this.buffer.Length / Console.BufferWidth + 1;

            Console.SetCursorPosition(bufferEnd, Console.CursorTop + bufferHeight - this.CursorLine - 1);
            this.cursorPos = this.buffer.Length;
        }

        public void MoveCursorHome()
        {
            int promptLength = this.getLeftPrompt().Length;

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
            foreach (char c in s)
            {
                this.PutChar(c);
            }
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
            int promptLength = this.getLeftPrompt().Length;
            int boundedCount = Math.Min(count, this.cursorPos - promptLength);
            this.MoveCursorLeft(boundedCount);
            this.Delete(boundedCount);
        }

        private int CursorLine { get => this.cursorPos / Console.BufferWidth; }

        private void MoveCursorLeft()
        {
            int promptLength = this.getLeftPrompt().Length;

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
        }

        private void InsertStringAtCursor(string s)
        {
            string remainingBuffer = this.buffer.RemainingSubstring(this.cursorPos);
            
            this.buffer.Insert(this.cursorPos, s);

            Console.Write(s);
            this.cursorPos += s.Length;

            Console.Write(remainingBuffer);
            this.cursorPos += remainingBuffer.Length;

            //this.MoveCursorLeft(remainingBuffer.Length);
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
