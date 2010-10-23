using System;

namespace NeuralNet
{
	/// Represents the current state of the game of ConnectFour
	///
	class ConnectFourBoard
	{
		int[] cells;
		public const int NUM_ROWS = 6;
		public const int NUM_COLUMNS = 7;

		public ConnectFourBoard()
		{
			cells = new int[NUM_ROWS*NUM_COLUMNS];
		}

		// column should be an integer in [0-6]
		public void addChecker(int column, int color)
		{
			//Debug.Assert(column >= 0 && column < NUM_COLUMNS);
			if (getCell(0,column) != 0)
			{
				//this column already filled
				//TODO- throw an exception
			}
			else
			{
				for (int row = 1; row < NUM_ROWS; row++)
				{
					if (getCell(row,column) != 0)
					{
						setCell(row-1, column, color);
						return;
					}
				}
				setCell(NUM_ROWS-1, column, color);
			}
		}

		// column should be an integer in [0-6]
		public int removeChecker(int column)
		{
			for (int row = 0; row < NUM_ROWS; row++)
			{
				int color = getCell(row, column);
				if (color != 0)
				{
					setCell(row, column, 0);
					return color;
				}
			}
			return 0;
		}

		public int getCell(int row, int column)
		{
			return cells[row * NUM_COLUMNS + column];
		}

		// column should be an integer in [0-6]
		public bool isColumnFull(int column)
		{
			return getCell(0, column) != 0;
		}

		/// y,x- the cell to start checking in
		/// dy,dx- the direction to look in
		/// returns the number of matching cells
		///
		int checkSequence(int y, int x, int dy, int dx)
		{
			int c = getCell(y,x);
			if (c == 0)
				return 0;

			for (int num = 1;; num++)
			{
				y += dy;
				x += dx;
				if (y < 0 || y >= NUM_ROWS)
					return num;
				if (x < 0 || x >= NUM_COLUMNS)
					return num;
				if (getCell(y,x) != c)
					return num;
			}
		}

		/// Check if there is a winner, if so, return the "color"
		/// of the winner.
		///
		public int getWinner()
		{
			for (int y = 0; y < NUM_ROWS; y++)
			{
				for (int x = 0; x < NUM_COLUMNS; x++)
				{
					// check for Four-in-a-row in
					// various directions

					if (checkSequence(y,x,-1,1) >= 4)
						return getCell(y,x);
					if (checkSequence(y,x,0,1) >= 4)
						return getCell(y,x);
					if (checkSequence(y,x,1,1) >= 4)
						return getCell(y,x);
					if (checkSequence(y,x,1,0) >= 4)
						return getCell(y,x);
				}
			}
			// if reached here, then no winner
			return 0;
		}

		public bool isGameOver()
		{
			if (getWinner() != 0)
				return true;

			for (int x = 0; x < NUM_COLUMNS; x++)
			{
				if (!isColumnFull(x))
					return false;
			}
			// if reached here, then all columns are full
			return true;
		}

		void setCell(int row, int column, int color)
		{
			cells[row * NUM_COLUMNS + column] = color;
		}
	}
}
