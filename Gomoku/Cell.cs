﻿namespace Gomoku
{
    public struct Cell
    {
        public int X { get; set; }
        public int Y { get; set; }

        public Cell(int X, int Y)
        {
            this.X = X;
            this.Y = Y;
        }
    }
}