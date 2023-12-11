// MIT License - Copyright (c) Vishnu Vijayan
// This file is subject to the terms and conditions defined in
// LICENSE, which is part of this source code package

namespace TraSH.Buffer
{
    using System.Collections.Generic;

    internal interface IConsoleBuffer
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
