using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NeuralNet;

namespace ConnectFour
{
    class Simulator
    {
        int TotalGames, JasonWon, AllenWon, Ties, Turns, TotalTurns;
        GameViewer Viewer { get; set; }
        Log Log = new Log();


        /// <summary>
        /// Simulate a game until completion.
        /// </summary>
        /// <param name="board">Starting board that the bots will play on.  This need not be empty!</param>
        /// <param name="network">Neural network that provides the AI for gameplay.</param>
        /// <returns>Trace of game sequence, each board state stored as a Neural Net Example</returns>
        public List<Example> Play(Board board = null, Network network = null, LogSetting setting = LogSetting.Ignore, GameViewer viewer = null)
        {
            if (board == null)
                board = new Board();
            if (network == null)
                network = new Network(board.Rows * board.Columns, 100, 1, null);
            Viewer = viewer;
            if (Viewer != null)
                Log = Viewer.Log;
            Log.Setting = setting;

            Bot allen = new Bot(Checker.Blue, network); // <-- you know he will win :)
            Bot jason = new Bot(Checker.Green, network);

            List<Example> trace = new List<Example>();

            Turns = 0;
            Bot current = allen;
            while (!board.IsGameOver)
            {
                int column;
                double score;
                current.SelectMove(board, out column, out score);
                Log.WriteLine(String.Format("{0} picks column {1}   (Score: {2:f2})", (current == allen ? "Allen" : "Jason"), column, score));
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
                example.Predictions.Add(score);
                trace.Add(example);

                current = (current == allen ? jason : allen);
                ++Turns;
            }

            if (Viewer != null)
                Viewer.BatchAddCheckers(board.MoveHistory, TimeSpan.FromMilliseconds(10));

            TotalTurns += Turns;

            Checker winner;
            if (board.TryGetWinner(out winner))
            {
                if (winner == allen.MyColor)
                {
                    Log.WriteLine("WINNER:  Allen");
                    ++AllenWon;
                }
                else
                {
                    Log.WriteLine("WINNER:  Jason");
                    ++JasonWon;
                }
            }
            else
            {
                Log.WriteLine("TIE");
                ++Ties;
            }

            ++TotalGames;
            Log.WriteLine(string.Format("Turns: {0} ({1:f2})", Turns, (double)TotalTurns / TotalGames));
            Log.WriteLine(string.Format("Allen: {0}({1:f2}) Jason: {2}({3:f2}) Ties {4}({5:f2})   TOTAL: {6}", AllenWon, (double)AllenWon / TotalGames, JasonWon, (double)JasonWon / TotalGames, Ties, (double)Ties / TotalGames, TotalGames));
            Log.WriteLine();

            return trace;
        }
    }
}
