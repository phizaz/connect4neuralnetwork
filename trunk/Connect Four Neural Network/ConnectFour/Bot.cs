using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace ConnectFour
{
	/// <summary>
	/// Plays the game of ConnectFour using an unspecified algorithm.
	/// </summary>
	public abstract class Bot
	{
		public Checker MyColor;

		protected Bot(Checker myColor)
		{
            Debug.Assert(myColor != Checker.Empty);
			MyColor = myColor;
		}

		protected abstract double EvaluateBoard(Board board);

		public void SelectMove(Board board, out int column, out double score)
		{
			int bestX = 0;
			double bestV = Double.NegativeInfinity;
			for (int x = 0; x < board.Columns; x++)
			{
				if (!board.IsColumnFull(x))
				{
                    board.AddChecker(MyColor, x);
					double v = EvaluateBoard(board);
					board.RemoveChecker(x);
					if (v > bestV)
					{
						bestX = x;
						bestV = v;
					}
				}
			}
			column = bestX;
            score = bestV;
		}
	}
}
