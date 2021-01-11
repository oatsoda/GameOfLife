using System;
using System.Threading;

namespace GameOfLife.ConsoleApp
{
    class Program
    {      
        //private const int GRID_COLS = 50;
        //private const int GRID_ROWS = 35;
        private const int SEED_LIVE_SQUARES_PERC = 23;

        const int _MAX_TICKS = 500;
        const int _TICK_INTERVAL = 250;

        private const int BOTTOM_MARGIN = 10;

        static (int x, int y) s_CursorPos;

        static void Main(string[] args)
        {
            Console.Clear();
            Console.OutputEncoding = System.Text.Encoding.UTF8;
            Console.ForegroundColor = ConsoleColor.Blue;
            Console.CursorVisible = false;
            //Console.SetWindowSize(Console.LargestWindowWidth - 5, Console.LargestWindowHeight - 5);
            Console.SetWindowPosition(0,0);

            var GRID_ROWS = Console.WindowHeight - BOTTOM_MARGIN;
            var GRID_COLS = (Console.WindowWidth / 2) - 1;

            var seedLiveSquares = (int)((GRID_ROWS * GRID_COLS) * (SEED_LIVE_SQUARES_PERC / 100D));

            var game = new Game(GRID_COLS, GRID_ROWS, seedLiveSquares, true);
            DrawGame(game);

            s_CursorPos = (0, game.Rows + 2);
            ResetCursor();
            Console.WriteLine($"Seeded with {SEED_LIVE_SQUARES_PERC}% [{seedLiveSquares}]. Press Enter to start.");

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
