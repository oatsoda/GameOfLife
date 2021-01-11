using System;
using System.Collections.Generic;
using System.Linq;

namespace GameOfLife.ConsoleApp
{
    public class Game
    {
        private readonly bool m_WrapGrid;

        public bool[,] Grid { get; }

        public int Cols { get; }
        public int Rows { get; }

        public Game(int cols, int rows, int seedLiveSquares, bool wrapGrid)
        {
            Cols = cols;
            Rows = rows;
            m_WrapGrid = wrapGrid;
            Grid = new bool[cols,rows];

            var rdm = new Random();

            for (int i = 0; i < seedLiveSquares; i++)
            {
                int x, y;
                do
                {
                    x = rdm.Next(0, cols);
                    y = rdm.Next(0, rows);
                }
                while (Grid[x,y]);

                Grid[x,y] = true;
            }
        }

        public List<(int x, int y, bool alive)> Tick()
        {
            // Calc changes
            var changes = new List<(int x, int y, bool alive)>();
            
            foreach (var (x, y) in GetGridSquares())
            {
                var n = LiveNeighbours(x,y);

                /*
                Any live cell with fewer than two live neighbours dies, as if by underpopulation.
                Any live cell with two or three live neighbours lives on to the next generation.
                Any live cell with more than three live neighbours dies, as if by overpopulation.
                Any dead cell with exactly three live neighbours becomes a live cell, as if by reproduction.
                */
                var alive = Grid[x,y];

                if (!alive)
                {
                    if (n == 3)
                        changes.Add((x,y,true));
                    continue;
                }

                if (n == 2 || n == 3)
                    continue;

                changes.Add((x,y,false));
            }

            // Apply changes
            foreach (var change in changes)
                Grid[change.x, change.y] = change.alive;

            return changes;
        }

        public IEnumerable<(int x, int y)> GetGridSquares()
        {
            for (int x = 0; x < Cols; x++)
            {
                for (int y = 0; y < Rows; y++)
                {
                    yield return (x, y);
                }
            }
        }

        public bool IsDeserted => GetGridSquares().All(square => !Grid[square.x, square.y]);

        private int LiveNeighbours(int x, int y)
        {
            var t = 0;

            for (int xN = x-1; xN <= x+1; xN++)
            {
                if (xN < 0 || xN >= Cols)
                    continue;

                for (int yN = y-1; yN <= y+1; yN++)
                {
                    if (yN < 0 || yN >= Rows || (xN == x && yN == y))
                        continue;

                    if (Grid[xN,yN])
                        t++;
                }

            }

            return t;
        }
    }
}
