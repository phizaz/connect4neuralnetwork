using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;

namespace NeuralNet
{
	public enum TerminationType { ByIteration, ByValidationSet }

	/// <summary>
	/// Encapsulates a set of termination criteria to complete networking propogation.
	/// </summary>
    public class Termination
    {
        TerminationType Type;

        public int CurrentIteration;
        int TotalIterations;

        List<Example> ValidationSet;
        int ValidateSchedule;
        double CurrentError = 1.0;
        Network Snapshot; // Snapshot of network that had best validation error thus far.
        double SnapshotError;
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
                // use current network and forward feed to find values... and calculate errors.
                // TODO
                // take a network snapshot if the error is lower than last network snapshot.
            }
            throw new NotImplementedException();

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
                    throw new NotImplementedException();
                }
                return false;
            }

        }
    }
}
