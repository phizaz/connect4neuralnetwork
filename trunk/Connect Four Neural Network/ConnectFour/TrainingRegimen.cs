using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ConnectFour
{
    public static class TrainingRegimen
    {
        public static Func<Board> Empty = () =>
        {
            return new Board();
        };

        public static Func<Board> Random = () =>
        {
            Random rand = new Random();
            Board board = new Board();
            int checkers = rand.Next(board.Rows * board.Columns + 1);
            Checker checker = rand.Next(2) == 0 ? Checker.Blue : Checker.Green;
            int column;
            for (int i = 0; i < checkers; ++i)
            {
                do
                    column = rand.Next(board.Columns);
                while (board.IsColumnFull(column));
                board.AddChecker(checker, column);
                checker = checker == Checker.Green ? Checker.Blue : Checker.Green;
            }
            return board;
        };
  
    }
}
