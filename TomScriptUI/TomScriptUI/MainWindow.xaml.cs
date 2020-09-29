using Microsoft.WindowsAPICodePack.Dialogs;
using System;
using System.IO;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using TDSStudios.TomScript.Core;

namespace TDSStudios.TomScript.UI
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void InputBrowseButton_Click(object sender, RoutedEventArgs e)
        {
            using var dialog = new CommonOpenFileDialog
            {
                Title = "Select TomScript Source File",
                Multiselect = false,
            };

            dialog.Filters.Add(new CommonFileDialogFilter("TomScript Source Files", ".tms"));
            dialog.Filters.Add(new CommonFileDialogFilter("All Files", ".*"));

            var result = dialog.ShowDialog();

            if (result == CommonFileDialogResult.Ok)
            {
                inputPathBox.Text = dialog.FileName;
            }
        }

        private void OutputBrowseButton_Click(object sender, RoutedEventArgs e)
        {
            using var dialog = new CommonOpenFileDialog
            {
                Title = "Select Output Folder",
                IsFolderPicker = true,
                Multiselect = false,
            };

            var result = dialog.ShowDialog();

            if (result == CommonFileDialogResult.Ok)
            {
                outputPathBox.Text = dialog.FileName;
            }
        }

        private void AddFileButton_Click(object sender, RoutedEventArgs e)
        {
            var itemToAdd = inputPathBox.Text;

            if (string.IsNullOrEmpty(itemToAdd))
            {
                snackbar.MessageQueue.Enqueue("Enter a file path");
                return;
            }

            if (!File.Exists(itemToAdd))
            {
                snackbar.MessageQueue.Enqueue("Enter a file that exists");
                return;
            }

            var item = new Label
            {
                Content = itemToAdd,
            };

            fileListView.Items.Add(item);

            inputPathBox.Text = "";

            snackbar.MessageQueue.Enqueue("File added", "Undo", () => DeleteLastFileFromList());
        }


        private void FileListView_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Delete)
            {
                if (fileListView.SelectedIndex == -1) return;

                string fileName = (fileListView.SelectedItem as Label).Content.ToString();

                snackbar.MessageQueue.Enqueue("Removed file", "Undo", () => AddFileToList(fileName));

                fileListView.Items.Remove(fileListView.SelectedItem);
            }
        }

        private async void CompileButton_Click(object sender, RoutedEventArgs e)
        {
            progressBar.Visibility = Visibility.Visible;
            await DoCompilation();
            progressBar.Visibility = Visibility.Hidden;
        }

        private async Task DoCompilation()
        {
            var compiler = new TomScriptCompiler();

            foreach (var item in fileListView.Items)
            {
                try
                {
                    string sourceFileName = (item as Label).Content.ToString();
                    string sourceCode = await File.ReadAllTextAsync(sourceFileName);

                    var compiled = compiler.Compile(sourceCode);

                    string outputFileName = Path.Combine(outputPathBox.Text, Path.GetFileName(sourceFileName) + ".py");
                    await File.WriteAllTextAsync(outputFileName, compiled);
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error during compilation: {ex}");
                }
            }
        }

        private void AddFileToList(string fileName) => fileListView.Items.Add(fileName);
        private void DeleteLastFileFromList() => fileListView.Items.RemoveAt(fileListView.Items.Count - 1);

    }
}
