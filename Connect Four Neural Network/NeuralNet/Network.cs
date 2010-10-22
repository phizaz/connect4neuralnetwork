using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;

namespace NeuralNet
{

	
	class Network
	{
		public List<Neuron> Hiddens { get { return Neurons.Where(n => n.Type == NeuronType.Hidden).ToList(); } }
		public List<Neuron> Inputs { get { return Neurons.Where(n => n.Type == NeuronType.Input).ToList(); } }
		public List<Neuron> Outputs { get { return Neurons.Where(n => n.Type == NeuronType.Output).ToList(); } }
		public List<Neuron> Constants { get { return Neurons.Where(n => n.Type == NeuronType.Constant).ToList(); }}
		public List<Neuron> Neurons = new List<Neuron>();
		public NetworkParameters Parameters = new NetworkParameters();
		public Termination Termination;

		private Network() { }
		public Network(int inputs, int hiddens, int outputs, Termination termination, NetworkParameters parameters = null) : this (inputs, new List<int>(){hiddens}, outputs, termination, parameters) {}
		public Network(int inputs, List<int> hiddens, int outputs, Termination termination, NetworkParameters parameters = null, int constants = 1)
		{
			if (parameters == null)
				parameters = new NetworkParameters();
			parameters.Assert();

			List<List<Neuron>> levels = new List<List<Neuron>>();
			levels.Add(Enumerable.Range(0, inputs - 1).Select(i => new Neuron(NeuronType.Input)).ToList());
			foreach (int hidden in hiddens)
				levels.Add(Enumerable.Range(0, hidden - 1).Select(i => new Neuron(NeuronType.Hidden)).ToList());
			levels.Add(Enumerable.Range(0, outputs - 1).Select(i => new Neuron(NeuronType.Output)).ToList());

			// Connects to all other neurons.. except input ones (this is used as the additive "constant" value attached to each sigmoid unit.)
			List<Neuron> connectToAll = Enumerable.Range(0, constants - 1).Select(i => new Neuron(NeuronType.Constant) { Value = 1 }).ToList();

			for (int i = 0; i + 1 < levels.Count; ++i)
			{
				foreach (Neuron n in levels[i])
					foreach (Neuron m in levels[i + 1])
					{
						n.Attach(m, new Weight(parameters.InitialWeightInterval));
						foreach (Neuron c in connectToAll)
							c.Attach(m, new Weight(parameters.InitialWeightInterval));
					}
			}

			Neurons = levels.Aggregate<List<Neuron>>((aggregated, level) => { aggregated.AddRange(level); return aggregated; }).ToList();
			Neurons.AddRange(connectToAll);
		}

		public bool TrainNetwork(List<Example> examples, int iterations = 1)
		{
			if (Termination.IsNetworkTrained)
				return true;
			for (int i = 0; i < iterations; ++i)
			{
				foreach (Example example in examples)
				{
					PropogateInput(example);
					PropogateErrors(example);
					UpdateWeights();

					if (Termination.IsNetworkTrained)
						return true;
				}
				Termination.CompleteIteration();
			}
			return false;
		}


		public void PropogateInput(Example example)
		{
			Neuron.UnfeedAll(Neurons);

			int feature = 0;
			foreach (Neuron neuron in Inputs)
			{
				neuron.Value = example.Features[feature++];
				neuron.Fed = true;
			}
			foreach (Neuron neuron in Neurons)
				FeedForward(neuron);
		}

		private void FeedForward(Neuron neuron)
		{
			if (neuron.Fed)
				return;

			foreach (Neuron n in neuron.Upstream)
				FeedForward(n);
			neuron.UpdateOutputValue(); // Safe to call since all upstream values have been updated.
			neuron.Fed = true;
		}

		private void PropogateErrors(Example example)
		{
			Neuron.UnfeedAll(Neurons);

			int label = 0;
			foreach (Neuron neuron in Outputs)
			{
				neuron.UpdateErrorTerm(example.Labels[label++]);
				neuron.Fed = true;
			}

			foreach (Neuron neuron in Neurons)
				PropogateErrors(neuron);
		}

		private void PropogateErrors(Neuron neuron)
		{
			if (neuron.Fed)
				return;

			foreach (Neuron n in neuron.Downstream)
				PropogateErrors(n);
			neuron.UpdateErrorTerm(); // Safe to call since all downstream errorterms have been updated.
			neuron.Fed = true;
		}

		private void UpdateWeights()
		{
			foreach (Neuron neuron in Neurons)
				neuron.UpdateDownstreamWeights(Parameters.LearningRate, Parameters.Momentum);
		}

				

	}

	class NetworkParameters
	{
		public double LearningRate = .05;
		public double Momentum = 0;
		public double LearningRateDecay = 0;
		public double MomentumDecay = 0;
		public Tuple<double, double> InitialWeightInterval = Tuple.Create(-.05, .05);
		
		public void Assert()
		{
			Debug.Assert(LearningRate > 0);
			Debug.Assert(Momentum >= 0);
			Debug.Assert(LearningRateDecay >= 0);
			Debug.Assert(MomentumDecay >= 0);
		}
	}

}
