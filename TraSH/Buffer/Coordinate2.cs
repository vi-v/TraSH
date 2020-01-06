// MIT License - Copyright (c) Vishnu Vijayan
// This file is subject to the terms and conditions defined in
// LICENSE, which is part of this source code package

namespace TraSH.Buffer
{
    using System;
    using System.Collections.Generic;
    using System.Text;

    public sealed class Coordinate2
    {
        public Coordinate2(int x, int y)
        {
            this.X = x;
            this.Y = y;
        }

        public int X { get; private set; }

        public int Y { get; private set; }
    }
}
