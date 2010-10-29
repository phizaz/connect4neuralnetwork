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
using GlassExtension;

namespace ConnectFour
{
    public enum LogSetting { Hidden, Ignore, Normal }

    /// <summary>
    /// Interaction logic for Log.xaml
    /// </summary>
    public partial class Log : Window
    {
        public LogSetting Setting = LogSetting.Normal;

        public Log(LogSetting setting = LogSetting.Normal)
        {
            InitializeComponent();
            Setting = setting;
            this.Loaded += new RoutedEventHandler(Log_Loaded);
        }

        void Log_Loaded(object sender, RoutedEventArgs e)
        {
            Glass.ExtendGlassFrame(this);
        }

        public void WriteLine(string text)
        {
            Write(text + "\r\n");
        }
        public void WriteLine()
        {
            Write("\r\n");
        }

        public void Write(string text)
        {
            if (Setting == LogSetting.Ignore)
                return;
            if (Setting != LogSetting.Hidden)
                this.Show();
            tbLog.AppendText(text);
            tbLog.ScrollToEnd();
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            this.Hide();
            e.Cancel = true;
        }



    }
}
