using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;

namespace NeuralNet
{

	[Serializable]
	public class Network
	{
		public List<Neuron> Hiddens { get { return Neurons.Where(n => n.Type == NeuronType.Hidden).ToList(); } }
		public List<Neuron> Inputs { get { return Neurons.Where(n => n.Type == NeuronType.Input).ToList(); } }
		public List<Neuron> Outputs { get { return Neurons.Where(n => n.Type == NeuronType.Output).ToList(); } }
		public List<Neuron> Constants { get { return Neurons.Where(n => n.Type == NeuronType.Constant).ToList(); }}
		public List<Neuron> Neurons = new List<Neuron>();
		public NetworkParameters Parameters = new NetworkParameters();
		public Termination Termination;
        public bool IsTrained { get { return Termination.IsNetworkTrained; } }

		private Network() { }  
		public Network(int inputs, int hiddens, int outputs, Termination termination, NetworkParameters parameters = null) : this (inputs, new List<int>(){hiddens}, outputs, termination, parameters) {}
		public Network(int inputs, List<int> hiddens, int outputs, Termination termination, NetworkParameters parameters = null, int constants = 1)
		{
			if (parameters == null)
				parameters = new NetworkParameters();
			parameters.Assert();

			List<List<Neuron>> levels = new List<List<Neuron>>();
			levels.Add(Enumerable.Range(0, inputs).Select(i => new Neuron(NeuronType.Input)).ToList());
			foreach (int hidden in hiddens)
				levels.Add(Enumerable.Range(0, hidden).Select(i => new Neuron(NeuronType.Hidden)).ToList());
			levels.Add(Enumerable.Range(0, outputs).Select(i => new Neuron(NeuronType.Output)).ToList());

			// Connects to all other neurons.. except input ones (this is used as the additive "constant" value attached to each sigmoid unit.)
			List<Neuron> connectToAll = Enumerable.Range(0, constants).Select(i => new Neuron(NeuronType.Constant) { Value = 1 }).ToList();

			for (int i = 0; i + 1 < levels.Count; ++i)
			{
				foreach (Neuron n in levels[i])
					foreach (Neuron m in levels[i + 1])
						n.Attach(m, new Weight(parameters.InitialWeightInterval));

				foreach (Neuron c in connectToAll)
					foreach (Neuron m in levels[i + 1])
						c.Attach(m, new Weight(parameters.InitialWeightInterval));
			}

			Neurons = levels.Aggregate<List<Neuron>>((aggregated, level) => { aggregated.AddRange(level); return aggregated; }).ToList();
			Neurons.AddRange(connectToAll);
		}

		public void LearnOneExample(Example example)
		{
			PropogateInput(example);
			PropogateErrors(example);
			UpdateWeights();
		}

		public bool TrainNetwork(List<Example> examples, int iterations = 1)
		{
            AssertValidForTraining(examples);

			if (Termination.IsNetworkTrained)
				return true;
			for (int i = 0; i < iterations; ++i)
			{
				foreach (Example example in examples)
				{
					LearnOneExample(example);
					if (Termination.IsNetworkTrained)
						return true;
				}
				Termination.CompleteIteration();
			}
			return false;
		}

        /// <summary>
        /// Propogates input through network (FeedForward).  It will update/populate example's predictions list.
        /// </summary>
		public void PropogateInput(Example example)
		{
            AssertValidFeatures(example);

            Neuron.UnfeedAll(Neurons);

            int feature = 0;
            foreach (Neuron neuron in Inputs)
            {
                neuron.Value = example.Features[feature++];
                neuron.Fed = true;
            }
            foreach (Neuron neuron in Neurons)
                recur_FeedForward(neuron);

            example.Predictions = Outputs.Select(o => o.Value).ToList();
		}

		private void recur_FeedForward(Neuron neuron)
		{
			if (neuron.Fed)
				return;

			foreach (Neuron n in neuron.Upstream)
				recur_FeedForward(n);
			neuron.UpdateOutputValue(); // Safe to call since all upstream values have been updated.
			neuron.Fed = true;
		}

		private void PropogateErrors(Example example)
		{
            AssertValidLabels(example);

			Neuron.UnfeedAll(Neurons);

			int label = 0;
			foreach (Neuron neuron in Outputs)
			{
				neuron.UpdateErrorTerm(example.Labels[label++]);
				neuron.Fed = true;
			}

			foreach (Neuron neuron in Neurons)
				recur_PropogateErrors(neuron);
		}

		private void recur_PropogateErrors(Neuron neuron)
		{
			if (neuron.Fed)
				return;

			foreach (Neuron n in neuron.Downstream)
				recur_PropogateErrors(n);
			neuron.UpdateErrorTerm(); // Safe to call since all downstream errorterms have been updated.
			neuron.Fed = true;
		}

		private void UpdateWeights()
		{
			foreach (Neuron neuron in Neurons)
				neuron.UpdateDownstreamWeights(Parameters.LearningRate, Parameters.Momentum);
		}


  
        private void AssertValidFeatures(Example example)
        {
            Debug.Assert(example.Features.Count == Inputs.Count, "The number of features must match the number of Input Neurons.\r\n" + "Features: " + example.Features.Count + " Inputs: " + Inputs.Count);
        }
        private void AssertValidLabels(Example example)
        {
            Debug.Assert(example.Labels.Count == Outputs.Count, "The number of labels must match the number of Output Neurons.\r\n" + "Labels: " + example.Features.Count + " Outputs: " + Inputs.Count);
        } 
        private void AssertValidForTraining(Example example)
        {
            AssertValidFeatures(example);
            AssertValidLabels(example);
        }
        private void AssertValidForTraining(List<Example> examples)
        {
            examples.ForEach(e => AssertValidForTraining(e));
        }		

	}

	[Serializable]
	public class NetworkParameters
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
