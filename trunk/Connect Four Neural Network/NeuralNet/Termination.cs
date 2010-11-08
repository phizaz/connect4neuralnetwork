using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
using System.IO;

using MySerializer;

namespace NeuralNet
{
	public enum TerminationType { ByIteration, ByValidationSet }

	/// <summary>
	/// Encapsulates a set of termination criteria to complete networking propogation.
	/// </summary>
    [Serializable]
    public class Termination
    {
        TerminationType Type;

        public int CurrentIteration;
        int TotalIterations;

        List<Example> ValidationSet;
        int ValidateSchedule;
        Network Snapshot; // Snapshot of network that had best validation error thus far.
        double SnapshotError = double.MaxValue;
        Network Network;

        private Termination() { }
        public static Termination ByIteration(int iterations)
        {
            Debug.Assert(iterations > 0);
            return new Termination() { Type = TerminationType.ByIteration, TotalIterations = iterations };
        }

        public static Termination ByValidationSet(Network network, List<Example> validationSet, int validateSchedule)
        {
            Debug.Assert(validateSchedule > 0 && validationSet != null && validationSet.Count > 0);
            return new Termination() { Type = TerminationType.ByValidationSet, ValidateSchedule = validateSchedule, Network = network };
        }

        public void CompleteIteration()
        {
            ++CurrentIteration;

            // Check if we need to run through the validation set.
            if (Type == TerminationType.ByValidationSet && CurrentIteration % ValidateSchedule == 0)
            {
                double error = Validate();
                if (error < SnapshotError)
                {
                    // The below is probably horribly inefficient.... what can we do to make it faster?
                    Network.TrueError = error;
                    if (!Directory.Exists(Network.Name))
                        Directory.CreateDirectory(Network.Name);
                    Serializer.Serialize(Network, Path.Combine(Network.Name, Network.Name + "_" + error.ToString()));
                    Snapshot = (Network)Serializer.Deserialize(Path.Combine(Network.Name, Network.Name + "_" + error.ToString()));
                }
            }

        }

        /// <summary>
        /// Tests a network by first computing a prediction (foward feeding), then comparing these predictions to the correct labels of examples in the validation set.
        /// </summary>
        /// <returns>Root mean squared error of all predictions from labels</returns>
        public double Validate()
        {
            double meanSquaredError = 0;
            double n = 0;
            foreach (Example example in ValidationSet)
            {
                Network.PropogateInput(example);
                for (int i = 0; i < example.Predictions.Count; ++i)
                {
                    meanSquaredError += Math.Pow((example.Predictions[i] - example.Labels[i]), 2);
                    ++n;
                }
            }
            meanSquaredError /= n;
            return Math.Sqrt(meanSquaredError);
        }

        public bool IsNetworkTrained
        {
            get
            {
                if (Type == TerminationType.ByIteration)
                {
                    if (CurrentIteration == TotalIterations)
                        return true;
                    return false;
                }
                else if (Type == TerminationType.ByValidationSet)
                {
                    //throw new NotImplementedException(); Go forever.... until user manually stops.  
                }
                return false;
            }

        }
    }
}
