﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using Ookii.Dialogs.WinForms;

namespace TomScriptCompiler
{
    public partial class Form1 : Form
    {
        public Form1(string source = null, string outputDir = null, bool compileExe = false)
        {
            InitializeComponent();

            if (source != null)
            {
                sourceList.Items.Add(source, true);
            }
            if (outputDir != null)
            {
                outputDirBox.Text = outputDir;
            }
            exeFileCheckBox.Checked = compileExe;
        }

        private void addSourceBtn_Click(object sender, EventArgs e)
        {
            sourceList.Items.Add(sourcePathBox.Text, true);
            sourcePathBox.Text = "";
        }

        private void browseSourceBtn_Click(object sender, EventArgs e)
        {
            var dialog = new System.Windows.Forms.OpenFileDialog();
            dialog.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
            dialog.Title = "Select TomScript File";
            dialog.Filter = "TomScript files (*.tms)|*.tms|All files (*.*)|*.*";
            dialog.Multiselect = true;

            if (dialog.ShowDialog() == DialogResult.OK)
            {
                if (dialog.FileNames.Length > 1)
                {
                    for (int i = 0; i < dialog.FileNames.Length; i++)
                    {
                        sourceList.Items.Add(dialog.FileNames[i], true);
                    }

                    if (outputDirBox.Text == "") outputDirBox.Text = Path.GetDirectoryName(dialog.FileName);
                }
                else
                {
                    sourcePathBox.Text = dialog.FileName;

                    if (outputDirBox.Text == "") outputDirBox.Text = Path.GetDirectoryName(dialog.FileName);
                }
            }
        }

        private void compileBtn_Click(object sender, EventArgs e)
        {
            if (sourceList.Items.Count == 0 && sourcePathBox.Text != "")
            {
                sourceList.Items.Add(sourcePathBox.Text, true);
                sourcePathBox.Text = "";
            }

            if (sourceList.Items.Count == 0) 
            {
                MessageBox.Show("Please enter one or more valid TomScript source files.");
                return;
            }
            if (outputDirBox.Text == "")
            {
                MessageBox.Show("Please enter a valid output directory.");
                return;
            }

            if (!Directory.Exists(outputDirBox.Text)) Directory.CreateDirectory(outputDirBox.Text);

            var startTime = DateTime.Now;
            for (int i = 0; i < sourceList.Items.Count; i++)
            {
                if (sourceList.GetItemChecked(i))
                {
                    string scriptName = Path.GetFileName(sourceList.Items[i].ToString());
                    Console.WriteLine($"Initiating compilation of {scriptName}...");

                    new Compiler(
                        sourceList.Items[i].ToString(),
                        Path.Combine(outputDirBox.Text, scriptName + ".py"),
                        exeFileCheckBox.Checked ? Path.Combine(outputDirBox.Text, scriptName + ".exe") : null
                    ).Compile();

                    sourceList.SetItemChecked(i, false);
                    Console.WriteLine($"Compilation of {scriptName} completed!\n");
                }
            }
            var endTime = DateTime.Now;
            var timeTaken = endTime - startTime;

            Console.WriteLine($"Compilation of {sourceList.Items.Count} item(s) finished in {timeTaken.TotalMilliseconds}ms!");

            Process.Start(outputDirBox.Text);
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            Console.WriteLine("TomScript Compiler loaded...\n");
        }

        private void outputDirBrowseBtn_Click(object sender, EventArgs e)
        {
            var dialog = new VistaFolderBrowserDialog();

            if (dialog.ShowDialog() == DialogResult.OK)
            {
                outputDirBox.Text = dialog.SelectedPath;
            }
        }
    }
}
