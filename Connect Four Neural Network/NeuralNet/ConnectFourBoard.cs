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

		void setCell(int row, int column, int color)
		{
			cells[row * NUM_COLUMNS + column] = color;
		}
	}
}
