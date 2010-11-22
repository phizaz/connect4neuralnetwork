using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NeuralNet;

namespace ConnectFour
{
    public class Trainer
    {
        Network Network;
        Simulator Simulator = new Simulator();
        NetworkGenerator gui;

        private Trainer(){}
        public Trainer(Network network, NetworkGenerator gui)
        {
            Network = network;
            this.gui = gui;
        }

        public void Train(Func<Board> regimen)
        {
            while (!Network.IsTrained && gui.Status != TrainStatus.Paused)
            {
                List<Example> trace = Simulator.Play(regimen(), Network);
                Network.TrainNetwork(trace);
                gui.Dispatcher.BeginInvoke(new Action<Network>(n => gui.UpdateProgressLabels(n)), gui.Network);
            }
        }

    }
}
