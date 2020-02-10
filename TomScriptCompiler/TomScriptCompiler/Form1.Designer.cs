namespace TomScriptCompiler
{
    partial class Form1
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Form1));
            this.addSourceBtn = new System.Windows.Forms.Button();
            this.sourcePathBox = new System.Windows.Forms.TextBox();
            this.browseSourceBtn = new System.Windows.Forms.Button();
            this.sourceList = new System.Windows.Forms.CheckedListBox();
            this.compileBtn = new System.Windows.Forms.Button();
            this.outputDirBox = new System.Windows.Forms.TextBox();
            this.outputDirBrowseBtn = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.exeFileCheckBox = new System.Windows.Forms.CheckBox();
            this.SuspendLayout();
            // 
            // addSourceBtn
            // 
            this.addSourceBtn.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.addSourceBtn.Location = new System.Drawing.Point(672, 9);
            this.addSourceBtn.Name = "addSourceBtn";
            this.addSourceBtn.Size = new System.Drawing.Size(100, 25);
            this.addSourceBtn.TabIndex = 2;
            this.addSourceBtn.Text = "+ Add Souce File";
            this.addSourceBtn.UseVisualStyleBackColor = true;
            this.addSourceBtn.Click += new System.EventHandler(this.addSourceBtn_Click);
            // 
            // sourcePathBox
            // 
            this.sourcePathBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.sourcePathBox.Location = new System.Drawing.Point(80, 12);
            this.sourcePathBox.Name = "sourcePathBox";
            this.sourcePathBox.Size = new System.Drawing.Size(480, 20);
            this.sourcePathBox.TabIndex = 3;
            // 
            // browseSourceBtn
            // 
            this.browseSourceBtn.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.browseSourceBtn.Location = new System.Drawing.Point(566, 9);
            this.browseSourceBtn.Name = "browseSourceBtn";
            this.browseSourceBtn.Size = new System.Drawing.Size(100, 25);
            this.browseSourceBtn.TabIndex = 4;
            this.browseSourceBtn.Text = "Browse";
            this.browseSourceBtn.UseVisualStyleBackColor = true;
            this.browseSourceBtn.Click += new System.EventHandler(this.browseSourceBtn_Click);
            // 
            // sourceList
            // 
            this.sourceList.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.sourceList.CheckOnClick = true;
            this.sourceList.FormattingEnabled = true;
            this.sourceList.IntegralHeight = false;
            this.sourceList.Location = new System.Drawing.Point(12, 38);
            this.sourceList.Name = "sourceList";
            this.sourceList.Size = new System.Drawing.Size(760, 149);
            this.sourceList.TabIndex = 5;
            // 
            // compileBtn
            // 
            this.compileBtn.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.compileBtn.Location = new System.Drawing.Point(12, 222);
            this.compileBtn.Name = "compileBtn";
            this.compileBtn.Size = new System.Drawing.Size(760, 23);
            this.compileBtn.TabIndex = 6;
            this.compileBtn.Text = "Start Compilation";
            this.compileBtn.UseVisualStyleBackColor = true;
            this.compileBtn.Click += new System.EventHandler(this.compileBtn_Click);
            // 
            // outputDirBox
            // 
            this.outputDirBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.outputDirBox.Location = new System.Drawing.Point(92, 196);
            this.outputDirBox.Name = "outputDirBox";
            this.outputDirBox.Size = new System.Drawing.Size(468, 20);
            this.outputDirBox.TabIndex = 7;
            // 
            // outputDirBrowseBtn
            // 
            this.outputDirBrowseBtn.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.outputDirBrowseBtn.Location = new System.Drawing.Point(566, 193);
            this.outputDirBrowseBtn.Name = "outputDirBrowseBtn";
            this.outputDirBrowseBtn.Size = new System.Drawing.Size(100, 25);
            this.outputDirBrowseBtn.TabIndex = 8;
            this.outputDirBrowseBtn.Text = "Browse";
            this.outputDirBrowseBtn.UseVisualStyleBackColor = true;
            this.outputDirBrowseBtn.Click += new System.EventHandler(this.outputDirBrowseBtn_Click);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(12, 15);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(62, 13);
            this.label1.TabIndex = 9;
            this.label1.Text = "Script Path:";
            // 
            // label2
            // 
            this.label2.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(12, 199);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(74, 13);
            this.label2.TabIndex = 10;
            this.label2.Text = "Output Folder:";
            // 
            // exeFileCheckBox
            // 
            this.exeFileCheckBox.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.exeFileCheckBox.AutoSize = true;
            this.exeFileCheckBox.Location = new System.Drawing.Point(672, 198);
            this.exeFileCheckBox.Name = "exeFileCheckBox";
            this.exeFileCheckBox.Size = new System.Drawing.Size(107, 17);
            this.exeFileCheckBox.TabIndex = 11;
            this.exeFileCheckBox.Text = "Create .exe file(s)";
            this.exeFileCheckBox.UseVisualStyleBackColor = true;
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(784, 261);
            this.Controls.Add(this.exeFileCheckBox);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.outputDirBrowseBtn);
            this.Controls.Add(this.outputDirBox);
            this.Controls.Add(this.compileBtn);
            this.Controls.Add(this.sourceList);
            this.Controls.Add(this.browseSourceBtn);
            this.Controls.Add(this.sourcePathBox);
            this.Controls.Add(this.addSourceBtn);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MinimumSize = new System.Drawing.Size(500, 200);
            this.Name = "Form1";
            this.Text = "TomScript Compiler";
            this.Load += new System.EventHandler(this.Form1_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.Button addSourceBtn;
        private System.Windows.Forms.TextBox sourcePathBox;
        private System.Windows.Forms.Button browseSourceBtn;
        private System.Windows.Forms.CheckedListBox sourceList;
        private System.Windows.Forms.Button compileBtn;
        private System.Windows.Forms.TextBox outputDirBox;
        private System.Windows.Forms.Button outputDirBrowseBtn;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.CheckBox exeFileCheckBox;
    }
}