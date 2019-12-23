namespace TraSH.Buffer
{
    using System;
    using System.Collections.Generic;

    public interface IConsoleBuffer
    {
        bool IsEmpty { get; }

        void Write(IEnumerable<char> s);

        void PutChar(char c);

        void MoveCursorLeft(int count);

        void MoveCursorRight(int count);

        void MoveCursorHome();

        void MoveCursorEnd();

        string GetContent();
    }
}
