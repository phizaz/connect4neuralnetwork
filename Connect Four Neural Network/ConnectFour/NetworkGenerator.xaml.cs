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

namespace ConnectFour
{
    /// <summary>
    /// Interaction logic for NetworkGenerator.xaml
    /// </summary>
    public partial class NetworkGenerator : Window
    {
        private ObservableDataSource<Point> Validation = new ObservableDataSource<Point>();
        private ObservableDataSource<Point> Training = new ObservableDataSource<Point>();

        public NetworkGenerator()
        {
            InitializeComponent();

        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            Glass.ExtendGlassFrame(this);
            //Validation.SetXYMapping(p => p);
            //Training.SetXYMapping(p => p);
            plotter.AddLineGraph(Validation, new Pen(Brushes.DarkBlue, 2.0), new PenDescription("Validation Error"));
            plotter.AddLineGraph(Training, new Pen(Brushes.DarkGreen, 2.0), new PenDescription("Training Error"));
            for (int i = 0; i < 1000; ++i)
            {
                AddValidationError(i, i);
            }
        }

        public void AddValidationError(int iteration, double error)
        {
            Validation.AppendAsync(this.Dispatcher, new Point(iteration, error));
        }

        public void AddTrainingError(int iteration, double error)
        {
            Training.AppendAsync(this.Dispatcher, new Point(iteration, error));
        }

        private void plotter_MouseWheel(object sender, MouseWheelEventArgs e)
        {
            plotter.Viewport.Zoom(e.Delta / 10.0);
        }

 

    }
}
