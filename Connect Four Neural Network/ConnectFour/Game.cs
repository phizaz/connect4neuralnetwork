using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ConnectFour
{
    class Game
    {
        Log Log;
        int TotalGames = 0, JasonWon = 0, AllenWon = 0, Ties = 0;

        public Game(Log log)
        {
            Log = log;
        }
            
        public void Play()
        {
            Log.Show();
            Bot allen = new Bot(Checker.Blue); // <-- you know he will win :)
            Bot jason = new Bot(Checker.Green);
            Board board = new Board();

            while(true)
            {
                if (board.IsGameOver)
                    break;

                int column = allen.SelectMove(board);
                Log.WriteLine("allen picks column " + column);
                board.AddChecker(allen.MyColor, column);

                if (board.IsGameOver)
                    break;

                column = jason.SelectMove(board);
                Log.WriteLine("jason picks column " + column);
                board.AddChecker(jason.MyColor, column);
            }

            Checker winner;
            if (board.TryGetWinner(out winner))
            {
                if (winner == allen.MyColor)
                {
                    Log.WriteLine("winner is allen");
                    ++AllenWon;
                }
                else
                {
                    Log.WriteLine("winner is jason");
                    ++JasonWon;
                }
            }
            else
            {
                Log.WriteLine("tie game");
                ++Ties;
            }
            ++TotalGames;
            Log.WriteLine(string.Format("Allen: {0}({1:f2}) Jason: {2}({3:f2}) Ties {4}({5:f2})   TOTAL: {6}", AllenWon, (double)AllenWon / TotalGames, JasonWon, (double)JasonWon / TotalGames, Ties, (double)Ties / TotalGames, TotalGames));
            Log.WriteLine();
        }
    }
}
