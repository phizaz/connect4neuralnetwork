using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using NeuralNet;

namespace ConnectFour
{
	/// <summary>
	/// Plays the game of ConnectFour using an unspecified algorithm.
	/// </summary>
	[Serializable]
	public abstract class Bot
	{
		public Checker MyColor;
		public double Lambda;
		static Random RANDOM = new Random();

		protected Bot(Checker myColor)
		{
            Debug.Assert(myColor != Checker.Empty);
			MyColor = myColor;
			Lambda = 0.01;
		}

		protected abstract double EvaluateBoard(Board board);
		public abstract Example MakeExample(Board board, Checker color);
		public abstract void LearnOneExample(Example example);


		public void SelectMove(Board board, out int column, out double score)
		{
            int bestX = 0;

			double[] columnEvaluations = new double[board.Columns];
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
						bestV = v;
                        bestX = x;
					}
					columnEvaluations[x] = v;
				}
				else
				{
					columnEvaluations[x] = Double.NegativeInfinity;
				}
			}

			//we pick a move randomly, using a probability distribution
			//such that the moves with the "best" board positions have a
			//higher probability of being selected

			//higher values of Lambda mean choices will have more equal
			//probability, even if they had different scores

			//low values of Lambda will have the opposite effect
			//Lambda should be positive number

			Lambda = Math.Max(Lambda, 0.00001);
			Lambda = Math.Min(Lambda, 10000);

			double sum = 0.0;
			double[] weights = new double[columnEvaluations.Length];
			for (int i = 0; i < columnEvaluations.Length; i++)
			{
				// the closer this column's evaluation to the "best", the
				// greater weight it will have
				double w = 1 / (Lambda + (bestV - columnEvaluations[i]));
				weights[i] = w;
				sum += w;
			}

			double r = RANDOM.NextDouble() * sum;
			int c;
			for (c = 0; c + 1 < weights.Length; c++)
			{
				r -= weights[c];
				if (r <= 0)
					break;
			}

			column = c;
			score = columnEvaluations[c];
		}
	}
}
