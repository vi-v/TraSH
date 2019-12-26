namespace TraSH.Buffer
{
    using System;
    using System.Collections.Generic;

    public interface IConsoleBuffer
    {
        bool IsEmpty { get; }

        Coordinate2 Size { get; }

        Coordinate2 CursorPosition { get; }

        void Write(IEnumerable<char> s);

        void PutChar(char c);

        void Delete(int count);

        void Backspace(int count);

        void MoveCursorLeft(int count);

        void MoveCursorRight(int count);

        void MoveCursorHome();

        void MoveCursorEnd();

        string GetContent();

        void Clear();
    }
}
