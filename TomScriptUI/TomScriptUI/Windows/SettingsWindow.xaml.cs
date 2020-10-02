using Microsoft.WindowsAPICodePack.Dialogs;
using System;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using TDSStudios.TomScript.UI.Util;

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

            (this.splashWindow = splashWindow)?.Hide();

            pythonExecutableLocationTextBox.Text = App.Settings.PythonExecutableLocation;
        }

        private void PythonExecutableLocationTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            App.Settings.PythonExecutableLocation = pythonExecutableLocationTextBox.Text;
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            UserSettings.Save(App.Settings);

            splashWindow?.Show();
        }

        private void PythonBrowseButton_Click(object sender, RoutedEventArgs e)
        {
            using var dialog = new CommonOpenFileDialog
            {
                Title = "Select Python.exe Location",
                Multiselect = false,
            };

            dialog.Filters.Add(new CommonFileDialogFilter("python.exe", ".exe"));

            var result = dialog.ShowDialog();

            if (result == CommonFileDialogResult.Ok)
            {
                if (Path.GetFileName(dialog.FileName) != "python.exe")
                {
                    MessageBox.Show("Select python.exe file");
                }
                else
                {
                    pythonExecutableLocationTextBox.Text = dialog.FileName;
                }
            }
        }
    }
}
