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
		Network Network;

		public NeuralNetBot(Checker myColor, Network network) : base(myColor)
		{
			Network = network;
		}

		protected override double EvaluateBoard(Board board)
		{
            Debug.Assert(Network.Inputs.Count == board.Rows * board.Columns);

            Example example = Transform.ToNormalizedExample(board, MyColor);
            Network.PropogateInput(example);
			return example.Predictions[0];
		}
	}
}
