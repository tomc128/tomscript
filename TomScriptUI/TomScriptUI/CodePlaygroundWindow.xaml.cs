using Microsoft.WindowsAPICodePack.Dialogs;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using TDSStudios.TomScript.Core;

namespace TDSStudios.TomScript.UI
{
    /// <summary>
    /// Interaction logic for CodePlaygroundWindow.xaml
    /// </summary>
    public partial class CodePlaygroundWindow : Window
    {
        private TomScriptCompiler compiler;
        private SplashWindow splashWindow;

        public CodePlaygroundWindow(SplashWindow splashWindow)
        {
            InitializeComponent();

            (this.splashWindow = splashWindow).Hide();

            compiler = new TomScriptCompiler();
        }

        private void CompileButton_Click(object sender, RoutedEventArgs e)
        {
            CompileCode();
        }

        private void CompileCode()
        {
            var output = compiler.Compile(sourceTextBox.Text);
            outputTextBox.Text = output;
        }

        private async void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            //using var dialog = new CommonOpenFileDialog
            //{
            //    Title = "Save Compiled TomScript Code",
            //    Multiselect = false,
            //    DefaultFileName = "output.py",
            //};

            using var dialog = new CommonSaveFileDialog
            {
                Title = "Save Compiled TomScript Code",
                DefaultFileName = "TomScript Output.py",
                DefaultExtension = ".py",
                AlwaysAppendDefaultExtension = true
            };

            dialog.Filters.Add(new CommonFileDialogFilter("Python Files", "*.py"));
            dialog.Filters.Add(new CommonFileDialogFilter("All Files", "*.*"));

            var result = dialog.ShowDialog();

            if (result == CommonFileDialogResult.Ok)
            {
                await File.WriteAllTextAsync(dialog.FileName, outputTextBox.Text);
            }
        }

        Task waitTask;

        private async void SourceTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (automaticCompilationCheckBox.IsChecked == false) return;

            waitTask = WaitForAutoCompilation(sourceTextBox.Text);
        }

        private async Task WaitForAutoCompilation(string textAtStart)
        {
            await Task.Delay(500);

            if (textAtStart == sourceTextBox.Text)
            {
                CompileCode();
            }
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            splashWindow.Show();
        }
    }
}
