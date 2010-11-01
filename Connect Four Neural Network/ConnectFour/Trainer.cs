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
                Network.TrainNetwork(trace);
            }
        }
    }
}
