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
using System.Windows.Navigation;
using System.Windows.Shapes;

using System.Runtime.InteropServices;
using System.Windows.Interop;
using System.Windows.Media.Animation;
using System.Threading;
using System.Threading.Tasks;
using GlassExtension;

using NeuralNet;

using System.ComponentModel;

namespace ConnectFour
{
    public enum GameMode { HumanVHuman, HumanVComputer, ComputerVComputer }

    /// <summary>
    /// Interaction logic for GameViewer.xaml
    /// </summary>
    public partial class GameViewer : Window
    {
        public GameMode Mode;
        public Board CurrentBoard;
        public Bot Bot;

        public Log Log = new Log();

        public GameViewer()
        {
            InitializeComponent();
            Restart(GameMode.HumanVComputer);
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            Glass.ExtendGlassFrame(this);
        }


        private void HumanVHuman_MouseUp(object sender, MouseButtonEventArgs e)
        {
            Restart(GameMode.HumanVHuman);
        }

        private void HumanVComputer_MouseUp(object sender, MouseButtonEventArgs e)
        {
            Restart(GameMode.HumanVComputer);
        }

        private void ComputerVComputer_MouseUp(object sender, MouseButtonEventArgs e)
        {
            Restart(GameMode.ComputerVComputer);
            Simulator simulator = new Simulator(this);
            simulator.Play();
            Log.Show();
        }

        private Checker Checker = Checker.Blue;
        private void Border_MouseUp(object sender, MouseEventArgs e)
        {
            int column = (int)(sender as Border).GetValue(Grid.ColumnProperty);
            if (IsColumnFull(column))
                return;

            if (Mode == GameMode.HumanVComputer)
            {
                double score;
                int column2;
                CurrentBoard.AddChecker(Checker, column);
                Bot.SelectMove(CurrentBoard, out column2, out score);
                CurrentBoard.AddChecker(ToggleChecker(Checker), column2);
                BatchAddCheckers(Checker, new List<int> { column, column2 }, TimeSpan.FromMilliseconds(50), false);
            }
            else
            {
                AddChecker(Checker, column);
                Checker = ToggleChecker(Checker);
            }
        }

        public void Restart(GameMode mode)
        {
            gridBoard.Children.RemoveRange(7, gridBoard.Children.Count - 7);
            Mode = mode;
            Checker = Checker.Blue;
            CurrentBoard = new Board();
            Bot = new Bot(Checker.Green, new Network(CurrentBoard.Rows * CurrentBoard.Columns, 100, 1, null));
        }

        public Checker ToggleChecker(Checker checker)
        {
            return (checker == Checker.Green ? Checker.Blue : Checker.Green);
        }


        public void AddChecker(Checker checker, int column, bool updateBoard = true)
        {
            BatchAddCheckers(checker, new List<int> { column }, TimeSpan.Zero, updateBoard); 
        }

        public void BatchAddCheckers(Checker start, List<int> columnHistory, TimeSpan delayBetweenMoves, bool updateBoard = true)
        {
            Storyboard story = new Storyboard();
            Checker checker = start;
            int i=0;
            foreach (int column in columnHistory)
            {
                Image image = new Image();
                image.Stretch = Stretch.Uniform;
                image.Source = (checker == Checker.Blue ? new BitmapImage(new Uri("/Icons/orbz_water.ico", UriKind.Relative)) : new BitmapImage(new Uri("/Icons/orbz_spirit.ico", UriKind.Relative)));
                image.SetValue(Grid.ColumnProperty, column);
                int? minRow = gridBoard.Children.OfType<Image>().Where(e => (int)e.GetValue(Grid.ColumnProperty) == column).Select(e => (int?)e.GetValue(Grid.RowProperty)).Min();
                if (minRow.HasValue && minRow == 0)
                    throw new Exception("Cannot add checker to full column");
                int row = (int)(minRow.HasValue ? minRow - 1 : 5);
                image.SetValue(Grid.ZIndexProperty, -1);
                image.SetValue(Grid.RowProperty, row);
                image.Opacity = 0;
                image.Height = gridBoard.RowDefinitions[0].ActualHeight;
                gridBoard.Children.Add(image);
                if (updateBoard)
                    CurrentBoard.AddChecker(checker, column);

                DoubleAnimation fadeIn = new DoubleAnimation(0, 1, TimeSpan.FromMilliseconds(0));
                ThicknessAnimation animation = new ThicknessAnimation(new Thickness(0, -300, 0, 0), new Thickness(0, 0, 0, 0), TimeSpan.FromMilliseconds(500));
                animation.EasingFunction = new BounceEase() { Bounces = 3, Bounciness = 5, EasingMode = System.Windows.Media.Animation.EasingMode.EaseOut };
                fadeIn.BeginTime = animation.BeginTime = TimeSpan.FromMilliseconds((i++) * delayBetweenMoves.TotalMilliseconds); 
                Storyboard.SetTarget(animation, image);
                Storyboard.SetTargetProperty(animation, new PropertyPath(Image.MarginProperty));
                Storyboard.SetTarget(fadeIn, image);
                Storyboard.SetTargetProperty(fadeIn, new PropertyPath(Image.OpacityProperty));
                story.Children.Add(animation);
                story.Children.Add(fadeIn);
                story.Completed += new EventHandler(story_Completed);

                checker = ToggleChecker(checker);
            }
            story.Begin();
        }

        public bool IsColumnFull(int column)
        {
            int? minRow = gridBoard.Children.OfType<Image>().Where(e => (int)e.GetValue(Grid.ColumnProperty) == column).Select(e => (int?)e.GetValue(Grid.RowProperty)).Min();
            return (minRow.HasValue && minRow == 0);
        }

        void story_Completed(object sender, EventArgs e)
        {
            Storyboard story = (Storyboard)(sender as ClockGroup).Timeline;
            foreach (var child in story.Children)
            {
                Image image = (Image)Storyboard.GetTarget(child);
                image.ClearValue(Image.HeightProperty);
            }
        }

        private void ToggleLog_MouseUp(object sender, MouseButtonEventArgs e)
        {
            if (Log.IsVisible)
                Log.Hide();
            else
                Log.Show();
        }

        private void ToggleSettings_MouseUp(object sender, MouseButtonEventArgs e)
        {

        }

 


    }






}
