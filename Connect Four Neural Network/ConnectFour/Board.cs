using System;
using System.Diagnostics;
using System.Collections.Generic;

namespace ConnectFour
{
    public enum Checker { Empty, Blue, Green }
    public enum GameResult { Draw, Win, Loss }

    /// <summary>
    /// Represents the current state of the game of ConnectFour
    /// </summary>
	public class Board
	{
        public List<int> MoveHistory = new List<int>();
        public int Rows { get { return Cells.GetLength(0); } }
        public int Columns { get { return Cells.GetLength(1); } }
        public Checker[,] Cells;
        public Checker NextInTurn;

        public Board(int rows=6, int columns=7)
        {
            Cells = new Checker[rows,columns];
            for (int i = 0; i < rows; ++i)
                for (int j = 0; j < columns; ++j)
                    Cells[i, j] = Checker.Empty;
        }

        public Checker this[int row, int col]
        {
            get { return Cells[row, col]; }
            set { Cells[row, col] = value; }
        }

		// column should be an integer in [0-6]
        public void AddChecker(Checker checker, int column)
		{
			Debug.Assert(column >= 0 && column < Columns);
			if (IsColumnFull(column))
			{
				//this column already filled
				//TODO- throw an exception
			}
			else
			{
				for (int row = Rows-1; row >= 0; --row)
				{
					if (Cells[row,column] == Checker.Empty)
					{
						Cells[row, column] = checker;
                        MoveHistory.Add(column);
						return;
					}
				}
			}
		}

		// column should be an integer in [0-6]
		public Checker RemoveChecker(int column)
		{
			for (int row = 0; row < Rows; row++)
			{
				Checker checker = Cells[row, column];
				if (checker != Checker.Empty)
				{
                    Cells[row, column] = Checker.Empty;
                    int lastindex = MoveHistory.FindLastIndex(e => e == column);
                    if (lastindex != -1)
                    MoveHistory.RemoveAt(lastindex);
                    
					return checker;
				}
			}
			return 0;
		}


		// column should be an integer in [0-6]
		public bool IsColumnFull(int column)
		{
            return Cells[0, column] != Checker.Empty;
		}

		/// <summary>
        /// Returns the number of matching cells
		/// </summary>
		/// <param name="y">Column to start checking in</param>
		/// <param name="x">Row to start checking in</param>
		/// <param name="dy">Look in the vertical direction</param>
		/// <param name="dx">Look in the horizontal direction</param>
		/// <returns></returns>
		int CheckSequence(int y, int x, int dy, int dx)
		{
			Checker checker = Cells[y,x];
			if (checker == Checker.Empty)
				return 0;

			for (int num = 1;; num++)
			{
				y += dy;
				x += dx;
				if (y < 0 || y >= Rows)
					return num;
				if (x < 0 || x >= Columns)
					return num;
				if (Cells[y,x] != checker)
					return num;
			}
		}

        /// <summary>
        /// Checks if there is a winner.
        /// </summary>
        public bool TryGetWinner(out Checker winner)
        {
            winner = Checker.Empty;
            for (int y = 0; y < Rows; y++)
            {
                for (int x = 0; x < Columns; x++)
                {
                    // check for Four-in-a-row in
                    // various directions

                    if (CheckSequence(y, x, -1, 1) >= 4)
                        winner = Cells[y, x];
                    if (CheckSequence(y, x, 0, 1) >= 4)
                        winner = Cells[y, x];
                    if (CheckSequence(y, x, 1, 1) >= 4)
                        winner = Cells[y, x];
                    if (CheckSequence(y, x, 1, 0) >= 4)
                        winner = Cells[y, x];
                    if (winner != Checker.Empty)
                        return true;
                }
            }
            // if reached here, then no winner
            return false;

        }

		public bool IsGameOver
		{
            get
            {
                Checker winner;
                if (TryGetWinner(out winner))
                    return true;

                for (int x = 0; x < Columns; x++)
                {
                    if (!IsColumnFull(x))
                        return false;
                }
                // if reached here, then all columns are full
                return true;
            }
		}
	}
}
