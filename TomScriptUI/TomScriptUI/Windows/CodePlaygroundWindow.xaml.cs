using Microsoft.WindowsAPICodePack.Dialogs;
using System;
using System.Collections.Generic;
using System.Diagnostics;
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
using TDSStudios.TomScript.UI.Util;

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

        private void SourceTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (automaticCompilationCheckBox.IsChecked == false) return;

            WaitForAutoCompilation(sourceTextBox.Text);
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

        private void CopyButton_Click(object sender, RoutedEventArgs e)
        {
            Clipboard.SetText(outputTextBox.Text);
        }

        private void PythonCompileButton_Click(object sender, RoutedEventArgs e)
        {
            if (process != null && !process.HasExited)
            {
                Console.WriteLine("Killing Python...");
                // Kill python
                process.Kill();
            }
            else
            {
                // Run python
                Console.WriteLine("Running Python...");

                pythonConsoleTextBox.Text = "";
                pythonInputTextBox.Text = "";

                pythonCompileButton.Content = "Kill Python";

                File.WriteAllText(PathLocater.TempPythonFileLocation, outputTextBox.Text);

                ExecutePython();
            }
        }


        private void SetEnabledStateForNonPythonTextBoxes(bool isEnabled)
        {
            sourceTextBox.IsEnabled = isEnabled;
            outputTextBox.IsEnabled = isEnabled;
        }


        private void ExecutePython()
        {
            pythonStatusEllipse.Fill = new SolidColorBrush(Colors.Gold);
            pythonStatusEllipse.ToolTip = "Python status: active";
            SetEnabledStateForNonPythonTextBoxes(false);

            var threadStart = new ThreadStart(RunPython);
            var thread = new Thread(threadStart);
            thread.Start();
        }

        private void ReadStream()
        {
            while (process == null)
            {
                Thread.Sleep(100);
            }
            while (!process.HasExited)
            {
                var read = processReader.Read();

                if (read == 0)
                {
                    // Nothing left to read - execution not finished, just wait a little
                    Thread.Sleep(100);
                }
                if (read == -1)
                {
                    // Nothing left to read - execution finished, break
                    break;
                }
                else
                {
                    Dispatcher.Invoke(() =>
                    {
                        pythonConsoleTextBox.Text += (char)read;
                    });
                }
            }

            Console.WriteLine("Python execution finished : 2 exit");
        }


        StreamWriter processWriter;
        StreamReader processReader;
        Process process;
        private void RunPython()
        {
            var startInfo = new ProcessStartInfo
            {
                FileName = App.Settings.PythonExecutableLocation,
                Arguments = $"\"{PathLocater.TempPythonFileLocation}\"",
                UseShellExecute = false,
                RedirectStandardOutput = true,
                RedirectStandardInput = true,
                CreateNoWindow = true
            };

            process = Process.Start(startInfo);

            processWriter = process.StandardInput;
            processReader = process.StandardOutput;

            var readerThreadStart = new ThreadStart(ReadStream);
            var readerThread = new Thread(readerThreadStart);
            readerThread.Start();

            process.WaitForExit();

            processWriter.Flush();
            processWriter.Dispose();

            processReader.Dispose();

            Dispatcher.Invoke(() =>
            {
                pythonStatusEllipse.Fill = new SolidColorBrush(Colors.LimeGreen);
                pythonStatusEllipse.ToolTip = "Python status: not active";
                SetEnabledStateForNonPythonTextBoxes(true);

                pythonCompileButton.Content = "Run Python";
            });

            Console.WriteLine("Python execution finished : 1 exit");
        }

        private void PythonInputTextBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                if (processWriter == null || processWriter.BaseStream == null) return;

                var text = pythonInputTextBox.Text;


                processWriter.Write(text + Environment.NewLine);
                Console.WriteLine($"Writing '{text}'");

                pythonInputTextBox.Text = "";
            }
        }

        private void PythonConsoleTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            pythonConsoleTextBox.ScrollToEnd();
        }
    }
}
