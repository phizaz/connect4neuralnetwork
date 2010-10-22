using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;

namespace NeuralNet
{
	class Example
	{
		public List<double> Features = new List<double>();
		public List<double> Labels = new List<double>();

		private Example(){}
		public Example(List<double> features, List<double> labels, Network network)
		{
			Debug.Assert(features.Count == network.Inputs.Count, "The number of features must match the number of Input Neurons.\r\n" + "Features: " + features.Count + " Inputs: " + network.Inputs.Count);
			Debug.Assert(labels.Count == network.Outputs.Count, "The number of labels must match the number of Output Neurons.\r\n" + "Labels: " + features.Count + " Outputs: " + network.Inputs.Count);
			Features = features;
			Labels = labels;
		}
	}
}
