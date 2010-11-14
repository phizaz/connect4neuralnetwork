using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

using Microsoft.Research.DynamicDataDisplay;
using Microsoft.Research.DynamicDataDisplay.DataSources;
using GlassExtension;
using NeuralNet;
using MySerializer;
using System.Threading;

namespace ConnectFour
{
    /// <summary>
    /// Interaction logic for NetworkGenerator.xaml
    /// </summary>
    public partial class NetworkGenerator : Window
    {
        private ObservableDataSource<Point> ValidationPlot = new ObservableDataSource<Point>();
        private ObservableDataSource<Point> TrainingPlot = new ObservableDataSource<Point>();
        public Network Network;

        public NetworkGenerator()
        {
            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            Glass.ExtendGlassFrame(this);
            //Validation.SetXYMapping(p => p);
            //Training.SetXYMapping(p => p);
            plotter.AddLineGraph(ValidationPlot, new Pen(Brushes.DarkBlue, 2.0), new PenDescription("Validation Error"));
            plotter.AddLineGraph(TrainingPlot, new Pen(Brushes.DarkGreen, 2.0), new PenDescription("Training Error"));
            New();
        }

        public void New()
        {
            tbName.Text = "";
            tbInputs.Text = "42";
            tbHiddens.Text = "100";
            tbOutputs.Text = "1";
            NetworkParameters parameters = new NetworkParameters();
            tbLearningRate.Text = parameters.LearningRate.ToString();
            tbLearningRateDecay.Text = parameters.LearningRateDecay.ToString();
            tbMomentum.Text = parameters.Momentum.ToString();
            tbMomentumDecay.Text = parameters.MomentumDecay.ToString();
            tbInitialWeightMin.Text = parameters.InitialWeightInterval.Item1.ToString();
            tbInitialWeightMax.Text = parameters.InitialWeightInterval.Item2.ToString();
            cbTerminationType.Text = "ByValidationSet";
            tbIterations.Text = "20000";
            tbValidateCycle.Text = "500";
            ValidationPlot.Collection.Clear();
            TrainingPlot.Collection.Clear();
        }


        public void PopulateControls(Network network)
        {
            tbName.Text = network.Name; 
            tbInputs.Text = network.Inputs.Count.ToString();
            tbHiddens.Text = network.HiddensLayout.Aggregate(string.Empty, (total, i) => total += i.ToString() + " ").Trim();
            tbOutputs.Text = network.Outputs.Count.ToString();
            tbLearningRate.Text = network.Parameters.LearningRate.ToString();
            tbLearningRateDecay.Text = network.Parameters.LearningRateDecay.ToString();
            tbMomentum.Text = network.Parameters.Momentum.ToString();
            tbMomentumDecay.Text = network.Parameters.MomentumDecay.ToString();
            tbInitialWeightMin.Text = network.Parameters.InitialWeightInterval.Item1.ToString();
            tbInitialWeightMax.Text = network.Parameters.InitialWeightInterval.Item2.ToString();
            cbTerminationType.Text = network.Termination.Type.ToString();
            tbIterations.Text = network.Termination.TotalIterations.ToString();
            tbValidateCycle.Text = network.Termination.ValidateCycle.ToString();
            ValidationPlot.Collection.Clear();
            TrainingPlot.Collection.Clear();

            tbName.IsEnabled = tbInputs.IsEnabled = tbHiddens.IsEnabled = tbOutputs.IsEnabled = tbInitialWeightMax.IsEnabled = tbInitialWeightMin.IsEnabled = false;
        }


        public void AddValidationError(int iteration, double error)
        {
            ValidationPlot.AppendAsync(this.Dispatcher, new Point(iteration, error));
        }

        public void AddTrainingError(int iteration, double error)
        {
            TrainingPlot.AppendAsync(this.Dispatcher, new Point(iteration, error));
        }

        private void cbTerminationType_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            tbIterations.IsEnabled = cbTerminationType.SelectedIndex == 1;
            tbValidateCycle.IsEnabled = cbTerminationType.SelectedIndex == 0;
        }

        private void btnNew_Click(object sender, RoutedEventArgs e)
        {
            New();
        }

        private void btnLoad_Click(object sender, RoutedEventArgs e)
        {
            Microsoft.Win32.OpenFileDialog open = new Microsoft.Win32.OpenFileDialog();
            open.Filter = "Neural Network|*.net";
            if (open.ShowDialog().Value)
            {
                try
                {
                    Network = (Network)Serializer.Deserialize(open.FileName);
                    PopulateControls(Network);
                }
                catch { MessageBox.Show("Could deserialize " + open.FileName.ToString(), "Error"); }
            }
        }

        private void btnStart_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                Termination termination;
                if (cbTerminationType.Text == "ByValidationSet")
                    termination = Termination.ByValidationSet(DataParser.ValidationSet, ToInt(tbValidateCycle, x => x > 0, "Validation cycle must be > 0"));
                else if (cbTerminationType.Text == "ByIteration")
                    termination = Termination.ByIteration(ToInt(tbIterations, x => x > 0, "Iterations must be > 0"));
                else
                    throw new Exception("Did not recognize termination type: " + cbTerminationType.Text);

                NetworkParameters parameters = new NetworkParameters()
                {
                    InitialWeightInterval = new Tuple<double, double>(ToDouble(tbInitialWeightMin), ToDouble(tbInitialWeightMax)),
                    LearningRate = ToDouble(tbLearningRate, x => x > 0, "Learning rate must be > 0"),
                    LearningRateDecay = ToDouble(tbLearningRateDecay),
                    Momentum = ToDouble(tbMomentum),
                    MomentumDecay = ToDouble(tbMomentumDecay)
                };

                Network = new Network(
                    ToString(tbName, s => s.Trim() != string.Empty, "Network name cannot be empty."),
                    ToInt(tbInputs, x => x > 0, "Number of input nodes must be > 0"),
                    tbHiddens.Text.Split(new char[] { ' ', ',', ';' }, StringSplitOptions.RemoveEmptyEntries).Select(x => ToInt(x, tbHiddens, xx => xx > 0, "Number of hidden nodes must be > 0 per layer")).ToList<int>(),
                    ToInt(tbOutputs, x => x > 0, "Number of output nodes must be > 0"),
                    termination,
                    parameters);

                Trainer trainer = new Trainer(Network);
                trainer.Train(TrainingRegimen.Empty);
            }
            catch (Exception x)
            {
                MessageBox.Show(x.Message, "Error");
            }
        }

        public int ToInt(TextBox tb, Func<int, bool> func = null, string errorMessage=null)
        {
            return ToInt(tb.Text, tb, func, errorMessage);
        }
        public int ToInt(string s, TextBox tb, Func<int, bool> func = null, string errorMessage = null)
        {
            int x;
            if (!int.TryParse(s.Trim(), out x))
                throw new Exception("Could not parse integer: " + tb.Name + " " + s);
            if (func != null && !func(x))
                throw new Exception(errorMessage ?? "Invalid value: " + tb.Name + " " + x);
            return x;
        }

        public double ToDouble(TextBox tb, Func<double, bool> func = null, string errorMessage=null)
        {
            double x;
            if (!double.TryParse(tb.Text.Trim(), out x))
                throw new Exception("Could not parse double: " + tb.Name + " " + tb.Text);
            if (func != null && !func(x))
                throw new Exception(errorMessage ?? "Invalid value: " + tb.Name + " " + x);
            return x;
        }
        public string ToString(TextBox tb, Func<string, bool> func = null, string errorMessage = null)
        {
            if (func != null && !func(tb.Text))
                throw new Exception(errorMessage ?? "Invalid value: " + tb.Name + " " + tb.Text);
            return tb.Text.Trim();
        }

    }
}
