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
	[Serializable]
	public class NeuralNetBot : Bot
	{
		public Network Network;
		public Transform transform;

		public NeuralNetBot(Checker myColor, Network network, double? lambda=null) : base(myColor, lambda)
		{
			Network = network;
			transform = network.Inputs.Count == 42 ? (Transform) new Transform42() :
				network.Inputs.Count == 78 ? (Transform) new Transform78() :
				null;
		}

		protected override double EvaluateBoard(Board board)
		{
			Example example = MakeExample(board, MyColor);
			Network.PropogateInput(example);
			return example.Predictions[0];
		}

		public override Example MakeExample(Board board, Checker color)
		{
			return transform.MakeExample(board, color);
		}

		public override void LearnOneExample(Example example)
		{
			Network.LearnOneExample(example);
		}
	}
}
