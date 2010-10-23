using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ConnectFour
{
    class Game
    {
        Log Log = new Log();

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
                Log.WriteLine("winner is " + (winner == allen.MyColor ? "allen" : "jason"));
            else
            {
                Log.WriteLine("tie game");
            }
        }
    }
}
