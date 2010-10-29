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

using System.ComponentModel;

namespace ConnectFour
{
    /// <summary>
    /// Interaction logic for GameViewer.xaml
    /// </summary>
    public partial class GameViewer : Window
    {
        private Checker Checker = Checker.Green;

        public Log Log = new Log();

        public GameViewer()
        {
            InitializeComponent();

        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            Glass.ExtendGlassFrame(this);
        }


        private void HumanVHuman_MouseUp(object sender, MouseButtonEventArgs e)
        {
            Reset();
        }

        private void HumanVComputer_MouseUp(object sender, MouseButtonEventArgs e)
        {
            Reset();
        }

        private void ComputerVComputer_MouseUp(object sender, MouseButtonEventArgs e)
        {
            Reset();
            Simulator simulator = new Simulator();
            simulator.Play(viewer: this, setting: LogSetting.Hidden);
            Log.Show();
        }

        private void Border_MouseUp(object sender, MouseEventArgs e)
        {
            int j = (int)(sender as Border).GetValue(Grid.ColumnProperty);
            Checker = Checker == Checker.Green ? Checker.Blue : Checker.Green;
            AddChecker(Checker, j);
        }

        public void Reset()
        {
            gridBoard.Children.RemoveRange(7, gridBoard.Children.Count - 7);

        }

  
        public void AddChecker(Checker checker, int column)
        {

            Image image = new Image();
            image.Stretch = Stretch.Uniform;
            image.Source = (checker == Checker.Blue ? new BitmapImage(new Uri("/Icons/orbz_water.ico", UriKind.Relative)) : new BitmapImage(new Uri("/Icons/orbz_spirit.ico", UriKind.Relative)));
            image.SetValue(Grid.ColumnProperty, column);
            int? minRow = gridBoard.Children.OfType<Image>().Where(e => (int)e.GetValue(Grid.ColumnProperty) == column).Select(e => (int?)e.GetValue(Grid.RowProperty)).Min();
            if (minRow.HasValue && minRow == 0)
                return;
            int row = (int)(minRow.HasValue ? minRow - 1 : 5);
            image.SetValue(Grid.ZIndexProperty, -1);
            image.SetValue(Grid.RowProperty, row);
            image.Height = gridBoard.RowDefinitions[0].ActualHeight;
            gridBoard.Children.Add(image);

            ThicknessAnimation animation = new ThicknessAnimation(new Thickness(0, -300, 0, 0), new Thickness(0, 0, 0, 0), TimeSpan.FromMilliseconds(500));
            animation.EasingFunction = new BounceEase() { Bounces = 3, Bounciness = 5, EasingMode = System.Windows.Media.Animation.EasingMode.EaseOut };
            Storyboard.SetTarget(animation, image);
            Storyboard.SetTargetProperty(animation, new PropertyPath(Image.MarginProperty));
            Storyboard story = new Storyboard();
            story.Children.Add(animation);
            story.Completed += new EventHandler(story_Completed);
            story.Begin();


        }

        public void BatchAddCheckers(List<int> columnHistory, TimeSpan? delayBetweenMoves = null)
        {
            Storyboard story = new Storyboard();
            Checker checker = Checker.Green;
            int i=0;
            foreach (int column in columnHistory)
            {
                checker = checker == Checker.Green ? Checker.Blue : Checker.Green;
                Image image = new Image();
                image.Stretch = Stretch.Uniform;
                image.Source = (checker == Checker.Blue ? new BitmapImage(new Uri("/Icons/orbz_water.ico", UriKind.Relative)) : new BitmapImage(new Uri("/Icons/orbz_spirit.ico", UriKind.Relative)));
                image.SetValue(Grid.ColumnProperty, column);
                int? minRow = gridBoard.Children.OfType<Image>().Where(e => (int)e.GetValue(Grid.ColumnProperty) == column).Select(e => (int?)e.GetValue(Grid.RowProperty)).Min();
                if (minRow.HasValue && minRow == 0)
                    continue;
                int row = (int)(minRow.HasValue ? minRow - 1 : 5);
                image.SetValue(Grid.ZIndexProperty, -1);
                image.SetValue(Grid.RowProperty, row);
                image.Opacity = 0;
                image.Height = gridBoard.RowDefinitions[0].ActualHeight;
                gridBoard.Children.Add(image);

                DoubleAnimation fadeIn = new DoubleAnimation(0, 1, TimeSpan.FromMilliseconds(0));
                ThicknessAnimation animation = new ThicknessAnimation(new Thickness(0, -300, 0, 0), new Thickness(0, 0, 0, 0), TimeSpan.FromMilliseconds(500));
                animation.EasingFunction = new BounceEase() { Bounces = 3, Bounciness = 5, EasingMode = System.Windows.Media.Animation.EasingMode.EaseOut };
                fadeIn.BeginTime = animation.BeginTime = TimeSpan.FromMilliseconds((i++) * (delayBetweenMoves ?? TimeSpan.Zero).TotalMilliseconds); 
                Storyboard.SetTarget(animation, image);
                Storyboard.SetTargetProperty(animation, new PropertyPath(Image.MarginProperty));
                Storyboard.SetTarget(fadeIn, image);
                Storyboard.SetTargetProperty(fadeIn, new PropertyPath(Image.OpacityProperty));
                story.Children.Add(animation);
                story.Children.Add(fadeIn);
                story.Completed += new EventHandler(story_Completed);
            }
            story.Begin();
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

 


    }






}
