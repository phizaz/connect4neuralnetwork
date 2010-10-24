using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NeuralNet;

namespace ConnectFour
{

    class Trainer
    {
        Network Network;
        Simulator Simulator = new Simulator();

        private Trainer(){}
        public Trainer(Network network)
        {
            Network = network;
        }

        public void Train(Func<Board> regimen)
        {
            while (!Network.IsTrained)
            {
                List<Example> trace = Simulator.Play(regimen(), Network);
                Critique(trace);
                Network.TrainNetwork(trace);
            }
        }

        /// <summary>
        /// Takes as input the history/trace and estimates label based on successor board state (ie Neural Network)
        /// </summary>
        public void Critique(List<Example> trace)
        {
            for (int i = 0; i < trace.Count - 1; ++i)
            {
                Network.PropogateInput(trace[i + 1]);
                trace[i].Labels = trace[i + 1].Predictions;
            }
            trace[trace.Count - 1].Labels = trace[trace.Count - 1].Predictions;
        }


    }
}
