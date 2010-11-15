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

using System.IO;
using ConnectFour.Properties;
using GlassExtension;
using Microsoft.Win32;

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
            sliderDropSpeed.Value = Settings.Default.DropSpeed;
            sliderDifficulty.Value = Settings.Default.Difficulty;
            sliderDropHeightRatio.Value = Settings.Default.DropHeightRatio;
            sliderMoveDelay.Value = Settings.Default.MoveDelay;
            sliderFadeTo.Value = Settings.Default.FadeTo;
            sliderFadeSpeed.Value = Settings.Default.FadeSpeed;
            UpdateNetworkPathLabel();
        }

        public void UpdateNetworkPathLabel()
        {
            lblNetworkPath.Content = Settings.Default.CurrentNetworkPath != null ? System.IO.Path.GetFileNameWithoutExtension(Settings.Default.CurrentNetworkPath) : "Null";
            btnClear.Visibility = Settings.Default.CurrentNetworkPath != null ? Visibility.Visible : Visibility.Hidden;
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

        private void btnLoadNetwork_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog open = new OpenFileDialog();
            open.Filter = "Neural Network|*.net";
            if (open.ShowDialog().Value)
            {
                Settings.Default.CurrentNetworkPath = open.FileName;
                UpdateNetworkPathLabel();
            }

        }

        bool forceClose = false;
        public void ForceClose()
        {
            forceClose = true;
            Close();
        }


        #region Events
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

        private void sliderDropSpeed_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            Settings.Default.DropSpeed = (int)sliderDropSpeed.Value;
        }

        private void sliderFadeTo_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            Settings.Default.FadeTo = (double)sliderFadeTo.Value;
        }

        private void sliderFadeSpeed_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            Settings.Default.FadeSpeed = (int)sliderFadeSpeed.Value;
        }


        private void sliderDifficulty_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            Settings.Default.Difficulty = (int)sliderDifficulty.Value;
            UpdateNetworkPathLabel();
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            this.Hide();
            if (!forceClose)
                e.Cancel = true;
        }  

        private void btnClearPath_MouseUp(object sender, MouseButtonEventArgs e)
        {
            Settings.Default.CurrentNetworkPath = null;
            UpdateNetworkPathLabel();
        }




        #endregion



     

    

    }
}
