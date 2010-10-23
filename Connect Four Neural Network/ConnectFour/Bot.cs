using System;
using System.Collections.Generic;
using NeuralNet;
using System.Diagnostics;
using System.Linq;

namespace ConnectFour
{
	/// <summary>
	/// Plays the game of ConnectFour using a neural network.
	/// </summary>
	public class Bot
	{
		Network Network;
		public Checker MyColor;

		public Bot(Checker myColor)
		{
			MyColor = myColor;
		}

		double EvaluateBoard(Board board)
		{
            if (Network == null)
                Network = new Network(board.Rows * board.Columns, 100, 1, null);
            else
                Debug.Assert(Network.Inputs.Count == board.Rows * board.Columns);

            Example example = new Example(board.Cells.Cast<Checker>().Select(checker => (checker == Checker.Empty ? 0.0 : checker == MyColor ? 1.0 : -1.0)).ToList());
            Network.PropogateInput(example);
			return example.Predictions[0];
		}

		public int SelectMove(Board board)
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
			return bestX;
		}
	}
}
