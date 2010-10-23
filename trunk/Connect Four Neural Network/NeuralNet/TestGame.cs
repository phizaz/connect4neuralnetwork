using System;
using System.Collections.Generic;
using NeuralNet;

class HelloWorld
{
	static void Main()
	{
		Bot bot1 = new Bot(1);
		Bot bot2 = new Bot(2);
		ConnectFourBoard board = new ConnectFourBoard();

		for (;;)
		{
			if (board.isGameOver())
				break;

			int column = bot1.SelectMove(board);
			Console.WriteLine("bot1 picks column " + column);
			board.addChecker(column, 1);

			if (board.isGameOver())
				break;

			column = bot2.SelectMove(board);
			Console.WriteLine("bot2 picks column " + column);
			board.addChecker(column, 2);
		}

		int color = board.getWinner();
		if (color != 0)
		{
			Console.WriteLine("winner is " + color);
		}
		else
		{
			Console.WriteLine("tie game");
		}
	}
}
