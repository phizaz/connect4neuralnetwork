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
            string str = string.Empty;
            for (int i = 0; i < Features.Count; ++i)
            {
                str += Features[i]<0 ? "x" : Features[i] == 0 ? "-" : "o";
                if (i % 7 == 0)
                    str += "\r\n";
            }
            return str;
        }
    }
}
