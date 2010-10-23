using System;
using System.Collections.Generic;

namespace NeuralNet
{
	/// Plays the game of Connect Four using a neural network.
	///
	class Bot
	{
		Network network;
		int my_color;

		const int NUM_INPUT_NODES = ConnectFourBoard.NUM_ROWS * ConnectFourBoard.NUM_COLUMNS;
		const int NUM_HIDDEN_NODES = 100;

		public Bot(int _my_color)
		{
			// construct a neural network with 42 input nodes,
			// one output node, and some number of hidden nodes.
			network = new Network(NUM_INPUT_NODES,
				NUM_HIDDEN_NODES, 1, null);
			my_color = _my_color;
		}

		double EvaluateBoard(ConnectFourBoard board)
		{
			double [] cells = new double[NUM_INPUT_NODES];
			for (int y = 0; y < ConnectFourBoard.NUM_ROWS; y++)
			{
				for (int x = 0; x < ConnectFourBoard.NUM_COLUMNS; x++)
				{
					int tmp = board.getCell(y,x);
					cells[y * ConnectFourBoard.NUM_COLUMNS + x]
						= tmp == 0 ? 0.0 :
						tmp == my_color ? 1.0 : -1.0;
				}
			}
			List<double> inputValues = new List<double>(cells);
			List<double> outputValues = network.Apply(inputValues);
			return outputValues[0];
		}

		public int SelectMove(ConnectFourBoard board)
		{
			int best_x = 0;
			double best_v = Double.NegativeInfinity;
			for (int x = 0; x < ConnectFourBoard.NUM_COLUMNS; x++)
			{
				if (!board.isColumnFull(x))
				{
					board.addChecker(x, 1);
					double v = EvaluateBoard(board);
					board.removeChecker(x);
					if (v > best_v)
					{
						best_x = x;
						best_v = v;
					}
				}
			}
			return best_x;
		}
	}
}
