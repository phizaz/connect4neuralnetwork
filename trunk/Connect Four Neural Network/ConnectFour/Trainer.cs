using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NeuralNet;

namespace ConnectFour
{
    public class Trainer
    {
        Simulator Simulator = new Simulator();
        NetworkGenerator gui;

        private Trainer(){}
        public Trainer(NetworkGenerator gui)
        {
            this.gui = gui;
        }

        public void Train(Func<Board> regimen)
        {
            gui.Network.TrainTime.Start();
            while (!gui.Network.IsTrained && gui.Status != TrainStatus.Paused)
            {
                List<Example> trace = Simulator.Play(regimen(), gui.Network);
                gui.Network.TrainNetwork(trace);
                gui.Dispatcher.BeginInvoke(new Action<Network>(n => gui.UpdateProgressLabels(n)), gui.Network);
            }
            gui.Network.TrainTime.Stop();
        }

    }
}
