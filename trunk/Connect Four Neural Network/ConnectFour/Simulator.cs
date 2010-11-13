using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NeuralNet;
using ConnectFour.Properties;

namespace ConnectFour
{
    class Simulator
    {
        int TotalGames, JasonWon, AllenWon, Ties, Turns, TotalTurns;
        GameViewer Viewer { get; set; }
        
        public Simulator(GameViewer viewer = null)
        {
            Viewer = viewer;
        }



        Checker NextPlayer(Board board)
        {
            int numBlue=0, numGreen=0;
            for (int i = 0; i < board.Rows; ++i)
                for (int j = 0; j < board.Columns; ++j)
                    if (board[i, j] == Checker.Blue)
                        ++numBlue;
                    else if (board[i, j] == Checker.Green)
                        ++numGreen;
            return (numBlue > numGreen ? Checker.Green : Checker.Blue);
        }

        /// <summary>
        /// Simulate a game until completion.
        /// </summary>
        /// <param name="board">Starting board that the bots will play on.  This need not be empty!</param>
        /// <param name="network">Neural network that provides the AI for gameplay.</param>
        /// <returns>Trace of game sequence, each board state stored as a Neural Net Example</returns>
        public List<Example> Play(Board board = null, Network network = null)
        {
            if (board == null)
                board = new Board();
            if (network == null)
                network = new Network("default", board.Rows * board.Columns, 100, 1, null);

            Bot allen = new NeuralNetBot(Checker.Blue, network); // <-- you know he will win :)
            Bot jason = new NeuralNetBot(Checker.Green, network);

            List<Example> trace = new List<Example>();

            Turns = 0;
            Bot current = allen.MyColor == NextPlayer(board) ? allen : jason;
            while (!board.IsGameOver)
            {
                int column;
                double score;
                current.SelectMove(board, out column, out score);
                Log(String.Format("{0} picks column {1}   (Score: {2:f2})", (current == allen ? "Allen" : "Jason"), column, score));
                board.AddChecker(current.MyColor, column);

                Example example = Transform.ToNormalizedExample(board, current.MyColor);
                example.Predictions.Add(score);
                trace.Add(example);

                current = (current == allen ? jason : allen);
                ++Turns;
            }

            if (Viewer != null)
                Viewer.BatchAddCheckers(allen.MyColor, board.MoveHistory, TimeSpan.FromMilliseconds(Settings.Default.MoveDelay));

            TotalTurns += Turns;

            Checker winner;
            if (board.TryGetWinner(out winner))
            {
                trace[trace.Count - 1].Predictions[0] = Transform.ToValue(GameResult.Win);
                trace[trace.Count - 2].Predictions[0] = Transform.ToValue(GameResult.Loss);
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
                trace[trace.Count - 1].Predictions[0] = trace[trace.Count - 2].Predictions[0] = Transform.ToValue(GameResult.Draw);
                Log("TIE");
                ++Ties;
            }

            ++TotalGames;
            Log(string.Format("Turns: {0} ({1:f2})", Turns, (double)TotalTurns / TotalGames));
            Log(string.Format("Allen: {0}({1:f2}) Jason: {2}({3:f2}) Ties {4}({5:f2})   TOTAL: {6}", AllenWon, (double)AllenWon / TotalGames, JasonWon, (double)JasonWon / TotalGames, Ties, (double)Ties / TotalGames, TotalGames));
            Log("");

            // Critic: Takes as input the history/trace and estimates label based on successor board state (Successor meaning next time current player goes -- every two moves!).  Assume all features and predictions values are populated already.
            for (int i = 0; i < trace.Count - 2; ++i)
            {
                trace[i].Labels = trace[i + 2].Predictions; 
            }
            trace[trace.Count - 2].Labels = trace[trace.Count - 2].Predictions;
            trace[trace.Count - 1].Labels = trace[trace.Count - 1].Predictions;

            return trace;
        }

        void Log(string msg)
        {
            if (Viewer != null)
                Viewer.Log.WriteLine(msg);
        }
    }
}
