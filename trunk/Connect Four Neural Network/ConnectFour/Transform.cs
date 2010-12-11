using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NeuralNet;
using System.Diagnostics;

namespace ConnectFour
{
    /// <summary>
    /// Contains the function that transform Connect-Four board states
    /// into a vector of double values to feed into neural network.
    /// </summary>
    public abstract class Transform
    {
	// This controls which transform is being used. To use a different
	// transform, change the following line as needed and rebuild the
	// project.
	static Transform myTransform = new Transform42();

        /// <summary>
        /// Converts board to neurel net training example.  Returns example corresponding to normalized board (current player's checker color corresponds to max value)
        /// </summary>
        /// <param name="lastPlayerToGo">Current Player which corresponds to last checker placed on board.</param>
        public static Example ToNormalizedExample(Board board, Checker lastPlayerToGo)
	{
		return myTransform.MakeExample(board, lastPlayerToGo);
	}

        public static double ToValue(GameResult result)
        {
            switch (result)
            {
                case GameResult.Loss: return 0;
                case GameResult.Win: return 1;
                case GameResult.Draw: return 0.5;
                default: throw new Exception();
            }
        }

        public abstract Example MakeExample(Board board, Checker lastPlayerToGo);
    }

    public class Transform42 : Transform
    {
        public static double ToValue(Checker checker) 
        {
            switch (checker)
            {
                case Checker.Blue: return -1.0; 
                case Checker.Green: return 1.0; 
                case Checker.Empty: return 0.0; 
                default: throw new Exception();
            }
        }

        public static double ToNormalizedValue(Checker checker, Checker lastPlayerToGo)
        {
            if (checker == Checker.Empty)
                return ToValue(checker);
            else if (checker == lastPlayerToGo)
                return Math.Max(ToValue(Checker.Blue), ToValue(Checker.Green));
            else
                return Math.Min(ToValue(Checker.Blue), ToValue(Checker.Green));
        }


        public override Example MakeExample(Board board, Checker lastPlayerToGo)
        {
            Debug.Assert(lastPlayerToGo != Checker.Empty);
            return new Example(board.Cells.Cast<Checker>().Select(c=>Transform42.ToNormalizedValue(c, lastPlayerToGo)).ToList());
        }
  
    }
}
