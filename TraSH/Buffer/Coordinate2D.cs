namespace TraSH.Buffer
{
    using System;
    using System.Collections.Generic;
    using System.Text;

    public sealed class Coordinate2D
    {
        public Coordinate2D(int x, int y)
        {
            this.X = x;
            this.Y = y;
        }

        public int X { get; private set; }

        public int Y { get; private set; }
    }
}
