using System;
using System.Collections.Generic;
using System.Configuration;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace TDSStudios.TomScript.UI
{
    /// <summary>
    /// Interaction logic for SettingsWindow.xaml
    /// </summary>
    public partial class SettingsWindow : Window
    {
        SplashWindow splashWindow;

        public SettingsWindow(SplashWindow splashWindow)
        {
            InitializeComponent();
            InitializeComponent();

            (this.splashWindow = splashWindow).Hide();
        }

        private void PythonExecutableLocationTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {

        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            splashWindow.Show();
        }
    }
}
