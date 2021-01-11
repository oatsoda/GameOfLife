using System;
using System.Threading;

namespace GameOfLife.ConsoleApp
{
    class Program
    {      
        private const int GRID_COLS = 40;
        private const int GRID_ROWS = 30;
        private const int SEED_LIVE_SQUARES = 400;

        const int _MAX_TICKS = 80;
        const int _TICK_INTERVAL = 400;

        static (int x, int y) s_CursorPos;

        static void Main(string[] args)
        {
            Console.OutputEncoding = System.Text.Encoding.UTF8;
            Console.ForegroundColor = ConsoleColor.Blue;
            Console.CursorVisible = false;

            var game = new Game(GRID_COLS, GRID_ROWS, SEED_LIVE_SQUARES, false);
            DrawGame(game);

            s_CursorPos = (0, game.Rows + 2);
            ResetCursor();
            Console.WindowHeight = s_CursorPos.y + 10;

            Console.ReadLine();

            for (int t = 1; t <= _MAX_TICKS; t++)
            {
                var changes = game.Tick();
                //Console.Beep(1100, 200);

                foreach (var change in changes)
                    DrawSquare(change);

                ResetCursor();
                Console.WriteLine(t.ToString().PadRight(Console.WindowWidth));
                if (game.IsDeserted)
                {
                    Console.WriteLine($"DEAD AFTER {t} TICKS :(");
                    return;
                }

                Thread.Sleep(_TICK_INTERVAL);
            }
                    
            Console.WriteLine($"SURVIVED {_MAX_TICKS} TICKS.");
        }

        static void DrawGame(Game game)
        {
            foreach ((int x, int y) square in game.GetGridSquares())
            {
                if (!game.Grid[square.x,square.y])
                    continue;

                DrawSquare((square.x,square.y,true));
            }            
        }

        static void DrawSquare((int x, int y, bool alive) square)
        {
            Console.SetCursorPosition(square.x * 2, square.y);
            Console.Write(square.alive ? "▒▒" : "  ");
        }

        static void ResetCursor() => Console.SetCursorPosition(s_CursorPos.x, s_CursorPos.y);
    }
}
