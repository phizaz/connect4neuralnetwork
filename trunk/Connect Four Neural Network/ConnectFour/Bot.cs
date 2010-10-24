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

		public Bot(Checker myColor, Network network)
		{
            Debug.Assert(myColor != Checker.Empty);
			MyColor = myColor;
            Network = network;
		}

		double EvaluateBoard(Board board)
		{
            Debug.Assert(Network.Inputs.Count == board.Rows * board.Columns);

            Example example = Transform.ToNormalizedExample(board, MyColor);
            Network.PropogateInput(example);
			return example.Predictions[0];
		}

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
