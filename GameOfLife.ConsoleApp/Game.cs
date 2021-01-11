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
                var n = CountLiveNeighbours(x,y);

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
            foreach (var (x, y, alive) in changes)
                Grid[x, y] = alive;

            return changes;
        }

        public IEnumerable<(int x, int y)> GetGridSquares()
        {
            for (int x = 0; x < Cols; x++)
                for (int y = 0; y < Rows; y++)
                    yield return (x, y);
        }

        public bool IsDeserted => GetGridSquares().All(square => !Grid[square.x, square.y]);

        private int CountLiveNeighbours(int x, int y)
        {
            var t = 0;

            for (int xN = x-1; xN <= x+1; xN++)
            {
                var targetX = xN;

                if (targetX < 0 || targetX >= Cols)
                {
                    if (!m_WrapGrid)
                        continue;

                    targetX = targetX < 0 ? (Cols - 1) : 0; // Wrap
                }

                for (int yN = y-1; yN <= y+1; yN++)
                {
                    var targetY = yN;

                    if (targetX == x && targetY == y)
                        continue;

                    if (targetY < 0 || targetY >= Rows)
                    {
                        if (!m_WrapGrid)
                            continue;
                        
                        targetY = targetY < 0 ? (Rows - 1) : 0; // Wrap
                    }

                    if (Grid[targetX, targetY])
                        t++;
                }

            }

            return t;
        }
    }
}
