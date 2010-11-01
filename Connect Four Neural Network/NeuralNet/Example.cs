using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;

namespace NeuralNet
{
    [Serializable]
    public class Example
    {
        public List<double> Features = new List<double>();
        public List<double> Labels = new List<double>();
        public List<double> Predictions = new List<double>();

        private Example() { }

        /// <summary>
        /// Creates an example to be used when utilizing neural net.
        /// </summary>
        public Example(List<double> features = null, List<double> labels = null, List<double> predictions = null)
        {
            if (features != null) Features = features;
            if (labels != null) Labels = labels;
            if (predictions != null) Predictions = predictions;
        }

        public override string ToString()
        {
            String buf = "";
            for (int x = 0; x < 7; x++)
            {
                buf += "[";
                for (int y = 0; y < 6; y++)
                {
                    double d = Features[y * 7 + x];
                    buf += d > 0 ? "X" : d < 0 ? "O" : "-";
                }
                buf += "]";
            }
            if (Labels.Count > 0)
                buf += " : " + Labels[0];
            return buf;
        }
    }
}
