using System;
using System.Collections.Generic;
using KNearestNeighbor;
using NeuralNet;
using System.Diagnostics;
using System.Linq;

namespace ConnectFour
{
	/// <summary>
	/// Plays the game of ConnectFour using a K-nearest-neighbor algorithm.
	/// </summary>
	[Serializable]
	public class KnnBot : Bot
	{
		KNearestNeighborLearner learner;

		public KnnBot(Checker myColor) : base(myColor)
		{
			learner = new KNearestNeighborLearner(5, 1000);
		}

		protected override double EvaluateBoard(Board board)
		{
			Example example = MakeExample(board, MyColor);
			learner.PropogateInput(example);
			return example.Predictions[0];
		}

		public override Example MakeExample(Board board, Checker color)
		{
			return Transform.ToNormalizedExample(board, color);
		}

		public override void LearnOneExample(Example example)
		{
			learner.LearnOneExample(example);
		}
	}
}
