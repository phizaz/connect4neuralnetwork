using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NeuralNet
{
	public enum NeuronType { Input, Hidden, Output, Constant }

	[Serializable]
	public class Neuron
	{
		public NeuronType Type;
		public double Value;
		public double ErrorTerm;
		public List<Neuron> Downstream = new List<Neuron>();
		public List<Weight> DownstreamWeights = new List<Weight>();
		public List<Weight> WeightChange = new List<Weight>();
		public List<Neuron> Upstream = new List<Neuron>();
		public List<Weight> UpstreamWeights = new List<Weight>();

		private  Neuron() { }
		public  Neuron(NeuronType type) { Type = type; }

		public void Attach(Neuron neuron, Weight weight)
		{
			Downstream.Add(neuron);
			DownstreamWeights.Add(weight);
			neuron.Upstream.Add(this);
			neuron.UpstreamWeights.Add(weight);
			WeightChange.Add(new Weight(0));
		}

		/// <summary>
		/// Calculates output value from sigmoid-function(x*w). Usually used for forward feeding.
		/// </summary>
		public void UpdateOutputValue()
		{
			double sum = 0;
			for (int i = 0; i < Upstream.Count; ++i)
				sum += Upstream[i].Value * UpstreamWeights[i].Value;
			Value = ComputeSigmoid(sum);
		}

		private double ComputeSigmoid(double y)
		{
			return 1.0 / (1 + Math.Exp(-y));
		}

		/// <summary>
		/// Calculates error term.  Used by backward propogation.
		/// </summary>
		public void UpdateErrorTerm(double label = 0)
		{
			if (Type == NeuronType.Output)
				ErrorTerm = Value * (1 - Value) * (label - Value);
			else if (Type == NeuronType.Hidden)
			{
				double sum = 0;
				for (int i = 0; i < Downstream.Count; ++i)
					sum += Downstream[i].ErrorTerm * DownstreamWeights[i].Value;
				ErrorTerm = Value * (1 - Value) * sum;
			}
		}

		/// <summary>
		/// Update all weights using error term from backward propogation.
		/// </summary>
		public void UpdateDownstreamWeights(double learningRate, double momentum)
		{
			for (int i = 0; i < Downstream.Count; ++i)
			{
				WeightChange[i].Value = learningRate * Downstream[i].ErrorTerm * Downstream[i].Value + momentum * WeightChange[i].Value;
				DownstreamWeights[i].Value += WeightChange[i].Value;
			}
		}



		#region PropogateHelper
		public bool Fed = false;
		public static void UnfeedAll(List<Neuron> neurons)
		{
			foreach (Neuron neuron in neurons)
				neuron.Fed = false;
		}
		#endregion




	}

}
