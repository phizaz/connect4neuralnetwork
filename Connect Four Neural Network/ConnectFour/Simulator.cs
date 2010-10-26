using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NeuralNet;

namespace ConnectFour
{
    class Simulator
    {
        Log Logger = new Log();
        int TotalGames, JasonWon, AllenWon, Ties, Turns, TotalTurns;
        bool LogProgress = false;

        public Simulator()
        {
            Logger.btnPlay.Click += new System.Windows.RoutedEventHandler(btnPlay_Click);
        }

        void btnPlay_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            Play(logProgress:true);
        }
            
        /// <summary>
        /// Simulate a game until completion.
        /// </summary>
        /// <param name="board">Starting board that the bots will play on.  This need not be empty!</param>
        /// <param name="network">Neural network that provides the AI for gameplay.</param>
        /// <returns>Trace of game sequence, each board state stored as a Neural Net Example</returns>
        public List<Example> Play(Board board = null, Network network=null, bool logProgress=false)
        {
            LogProgress = logProgress;
            if (logProgress)
                Logger.Show();

            if (board == null)
                board = new Board();
            if (network == null)
                network = new Network(board.Rows * board.Columns, 100, 1, null);

            Bot allen = new Bot(Checker.Blue, network); // <-- you know he will win :)
            Bot jason = new Bot(Checker.Green, network);

            List<Example> trace = new List<Example>();

            Turns=0;
            Bot current = allen;
            while(!board.IsGameOver)
            {
                int column;
                double score;
                current.SelectMove(board, out column, out score);
                Log((current == allen ? "Allen" : "Jason")
			+ " picks column " + column
			+ " (score " + score + ")");
                board.AddChecker(current.MyColor, column);

                Example example = Transform.ToNormalizedExample(board, current.MyColor);
		if (board.IsGameOver)
		{
			Checker winner1;
            		if (board.TryGetWinner(out winner1))
				score = winner1 == current.MyColor ? 1 : 0;
			else
				score = .5;
		}
                example.Labels.Add(score);
                trace.Add(example);
                Log(ExampleToString(example));
                
                current = (current == allen ? jason : allen);
                ++Turns;
            }
            TotalTurns += Turns;

            Checker winner;
            if (board.TryGetWinner(out winner))
            {
                if (winner == allen.MyColor)
                {
                    Log("WINNER:  Allen");
                    ++AllenWon;
                }
                else
                {
                    Log("WINNER:  Jason");
                    ++JasonWon;
                }
            }
            else
            {
                Log("TIE");
                ++Ties;
            }
            
            ++TotalGames;
            Log(string.Format("Turns: {0} ({1:f2})", Turns, (double)TotalTurns / TotalGames));
            Log(string.Format("Allen: {0}({1:f2}) Jason: {2}({3:f2}) Ties {4}({5:f2})   TOTAL: {6}", AllenWon, (double)AllenWon / TotalGames, JasonWon, (double)JasonWon / TotalGames, Ties, (double)Ties / TotalGames, TotalGames));
            Log();

            return trace;
        }

        public void Log(string message="")
        {
            if (LogProgress)
                Logger.WriteLine(message);
        }

	static String ExampleToString(Example example)
	{
		String buf = "";
		for (int x = 0; x < 7; x++)
		{
			buf += "[";
			for (int y = 0; y < 6; y++)
			{
				double d = example.Features[y * 7 + x];
				buf += d > 0 ? "X" : d < 0 ? "O" : "-";
			}
			buf += "]";
		}
		buf += " : " + example.Labels[0];
		return buf;
	}
    }
}
