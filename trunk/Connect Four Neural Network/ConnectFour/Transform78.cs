using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NeuralNet;
using System.Diagnostics;

namespace ConnectFour
{
    /// With this transform, we create a vector of
    /// 42 + 14*2 + 8 = 78 real numbers.
    ///
    public class Transform78 : Transform
    {
		public override Example MakeExample(Board board, Checker color)
		{
			Example e = new Example();
			for (int row = 0; row < board.Rows; row++)
			{
				for (int col = 0; col < board.Columns; col++)
				{
					Checker c1 = board.Cells[row, col];
					e.Features.Add(
						c1 == Checker.Empty ? 0 :
						c1 == color ? 1 :
						-1);
				}
			}

			for (int col = 0; col < board.Columns; col++)
			{
				for (int row = 0; row < 2; row++)
				{
					bool myThreat = checkSequence3(board, color, col, -(row + 1));
					e.Features.Add(myThreat ? 1 : 0);
					bool oppThreat = checkSequence3(board, otherColor(color), col, -(row + 1));
					e.Features.Add(oppThreat ? 1 : 0);
				}
			}

			for (int len = 1; len <= 4; len++)
			{
				e.Features.Add(countSequences(board, color, len));
				e.Features.Add(countSequences(board, otherColor(color), len));
			}
			return e;
		}

  
		private int countSequences(Board b, Checker c, int len)
		{
			int sum = 0;
			for (int y = 0; y < b.Rows; y++)
			{
				for (int x = 0; x < b.Columns; x++)
				{
					sum += countSequences(b, c, len, x, y);
				}
			}
			return sum;
		}

		private int countSequences(Board b, Checker c, int len, int x, int y)
		{
			int count = 0;
			if (checkSequence4(b, c, x, y, 0, 1) >= len)
				count++;
			if (checkSequence4(b, c, x, y, 1, 0) >= len)
				count++;
			if (checkSequence4(b, c, x, y, 1, -1) >= len)
				count++;
			if (checkSequence4(b, c, x, y, 1, 1) >= len)
				count++;
			return count;
		}

		//Checks whether a given spot on the board is a threat to
		//complete a 4-in-a-row (i.e. filling that spot with a certain
		//color checker will make a 4-in-a-row of that color).
		//
		// x - the x-coordinate of the spot to check.
		// y - the y-coordinate of the spot to check,
		//     or -1 for the lowest empty spot in the column,
		//     or -2 for the 2nd-lowest empty spot in the column, etc.
		//
		private bool checkSequence3(Board b, Checker c, int x, int y)
		{
			if (y < 0)
			{
				//find top-most checker in this column
				int row = 0;
				while (row < b.Rows && b.Cells[row,x]==Checker.Empty)
				{
					row++;
				}

				// row is the y-coord of top-most checker
				y = row + y;
				if (y < 0)
					return false;
			}

			return checkSequence3(b,c,x,y,1,0)
				|| checkSequence3(b,c,x,y,1,1)
				|| checkSequence3(b,c,x,y,1,-1)
				|| checkSequence3(b,c,x,y,0,1);
		}

		private bool checkSequence3(Board b, Checker c, int x, int y, int dx, int dy)
		{
			if (b.Cells[y,x] != Checker.Empty)
				return false;

			int count_behind = 0;
			for (int i = 1; i < 4; i++)
			{
				if (b.isValidCoords(y-dy*i, x-dx*i))
				{
					Checker c1 = b.Cells[y-dy*i, x-dx*i];
					if (c1 != c)
						break;
					count_behind++;
				}
			}
			int count_ahead = 0;
			for (int i = 1; i < 4; i++)
			{
				if (b.isValidCoords(y+dy*i, x+dx*i))
				{
					Checker c1 = b.Cells[y+dy*i, x+dx*i];
					if (c1 != c)
						break;
					count_ahead++;
				}
			}
			return count_ahead + count_behind >= 3;
		}

		//returns number of contributing checkers in the specific
		//four-in-a-row position; 0 if there is an opponent checker
		//in the way or the sequence is invalid.
		//
		private int checkSequence4(Board b, Checker c, int x, int y, int dx, int dy)
		{
			int count = 0;
			int i = 0;
			while (b.isValidCoords(y,x) && (b.Cells[y,x] == Checker.Empty || b.Cells[y,x] == c))
			{
				if (b.Cells[y,x] == c)
	
					count++;
				i++;
				if (i >= 4)
					break;
				y += dy;
				x += dx;
			}
			if (i < 4)
				return 0;
			return count;
		}

		private Checker otherColor(Checker c)
		{
			if (c == Checker.Blue)
				return Checker.Green;
			else if (c == Checker.Green)
				return Checker.Blue;
			else
				return c;
		}

    }
}
