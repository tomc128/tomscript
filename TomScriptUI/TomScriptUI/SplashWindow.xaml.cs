using System;
using System.Collections.Generic;
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
    /// Interaction logic for SplashWindow.xaml
    /// </summary>
    public partial class SplashWindow : Window
    {
        public SplashWindow()
        {
            InitializeComponent();
        }

        private void CodePlaygroundButton_Click(object sender, RoutedEventArgs e)
        {
            var window = new CodePlaygroundWindow(this);
            window.Show();
        }

        private void BatchCompilerButton_Click(object sender, RoutedEventArgs e)
        {
            var window = new BatchCompilerWindow(this);
            window.Show();
        }
    }
}
