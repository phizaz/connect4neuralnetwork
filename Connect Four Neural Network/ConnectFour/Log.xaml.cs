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

namespace ConnectFour
{
    /// <summary>
    /// Interaction logic for Log.xaml
    /// </summary>
    public partial class Log : Window
    {
        Game game;
        public Log()
        {
            InitializeComponent();
            game = new Game(this);
        }

        public void WriteLine(string text)
        {
            tbLog.AppendText(text + "\r\n");
            tbLog.ScrollToEnd();
        }
        public void WriteLine()
        {
            tbLog.AppendText("\r\n");
        }

        public void Write(string text)
        {
            tbLog.AppendText(text);
            tbLog.ScrollToEnd();
        }

        private void btnPlay_Click(object sender, RoutedEventArgs e)
        {
            game.Play();
        }
    }
}
