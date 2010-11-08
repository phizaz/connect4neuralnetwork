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

using ConnectFour.Properties;
using GlassExtension;

namespace ConnectFour
{
    /// <summary>
    /// Interaction logic for Menu.xaml
    /// </summary>
    public partial class Menu : Window
    {
        public Menu()
        {
            InitializeComponent();
            PopulateControls();
        }

        private void PopulateControls()
        {
            sliderAnimationSpeed.Value = Settings.Default.AnimationSpeed;
            sliderDifficulty.Value = Settings.Default.Difficulty;
            sliderDropHeightRatio.Value = Settings.Default.DropHeightRatio;
            sliderMoveDelay.Value = Settings.Default.MoveDelay;
            lblNetworkPath.Content = Settings.Default.NetworkPaths[(int)sliderDifficulty.Value] ?? "Null";
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            GlassExtension.Glass.ExtendGlassFrame(this);
        }
        private void sliderMoveDelay_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            Settings.Default.MoveDelay = (int)sliderMoveDelay.Value;
        }

        private void sliderDropHeightRatio_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            Settings.Default.DropHeightRatio = sliderDropHeightRatio.Value;
        }

        private void sliderAnimationSpeed_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            Settings.Default.AnimationSpeed = (int)sliderAnimationSpeed.Value;
        }

        private void sliderDifficulty_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            Settings.Default.Difficulty = (int)sliderDifficulty.Value;
            lblNetworkPath.Content = Settings.Default.NetworkPaths[(int)sliderDifficulty.Value] ?? "Null";
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            this.Hide();
            e.Cancel = true;
        }

        private void btnDefault_Click(object sender, RoutedEventArgs e)
        {
            Settings.Default.Reset();
            PopulateControls();
        }

        private void btnCreateNetwork_Click(object sender, RoutedEventArgs e)
        {
            (new NetworkGenerator()).Show();
        }


    }
}
