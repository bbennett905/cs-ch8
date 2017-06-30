namespace csch8
{
    partial class MainForm
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
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.fileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.loadToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.quitToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.settingsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.cPUToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.interpreterToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.dynamicRecompilerToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.graphicsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.setColorsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.primaryToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.secondaryToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.showFPSToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.debugToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.historyToolStripMenuItem2 = new System.Windows.Forms.ToolStripMenuItem();
            this.pc_label = new System.Windows.Forms.Label();
            this.opcode_label = new System.Windows.Forms.Label();
            this.pauseButton = new System.Windows.Forms.Button();
            this.fpsSelector = new System.Windows.Forms.NumericUpDown();
            this.menuStrip1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.fpsSelector)).BeginInit();
            this.SuspendLayout();
            // 
            // menuStrip1
            // 
            this.menuStrip1.ImageScalingSize = new System.Drawing.Size(20, 20);
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.fileToolStripMenuItem,
            this.settingsToolStripMenuItem,
            this.debugToolStripMenuItem});
            this.menuStrip1.Location = new System.Drawing.Point(0, 0);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.Padding = new System.Windows.Forms.Padding(8, 2, 0, 2);
            this.menuStrip1.Size = new System.Drawing.Size(853, 28);
            this.menuStrip1.TabIndex = 0;
            this.menuStrip1.Text = "menuStrip1";
            // 
            // fileToolStripMenuItem
            // 
            this.fileToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.loadToolStripMenuItem,
            this.quitToolStripMenuItem});
            this.fileToolStripMenuItem.Name = "fileToolStripMenuItem";
            this.fileToolStripMenuItem.Size = new System.Drawing.Size(44, 24);
            this.fileToolStripMenuItem.Text = "&File";
            // 
            // loadToolStripMenuItem
            // 
            this.loadToolStripMenuItem.Name = "loadToolStripMenuItem";
            this.loadToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.L)));
            this.loadToolStripMenuItem.Size = new System.Drawing.Size(212, 26);
            this.loadToolStripMenuItem.Text = "&Load ROM...";
            this.loadToolStripMenuItem.Click += new System.EventHandler(this.LoadToolStripMenuItem_Click);
            // 
            // quitToolStripMenuItem
            // 
            this.quitToolStripMenuItem.Name = "quitToolStripMenuItem";
            this.quitToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)(((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.Shift) 
            | System.Windows.Forms.Keys.Q)));
            this.quitToolStripMenuItem.Size = new System.Drawing.Size(212, 26);
            this.quitToolStripMenuItem.Text = "&Quit";
            this.quitToolStripMenuItem.Click += new System.EventHandler(this.QuitToolStripMenuItem_Click);
            // 
            // settingsToolStripMenuItem
            // 
            this.settingsToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.cPUToolStripMenuItem,
            this.graphicsToolStripMenuItem});
            this.settingsToolStripMenuItem.Name = "settingsToolStripMenuItem";
            this.settingsToolStripMenuItem.Size = new System.Drawing.Size(74, 24);
            this.settingsToolStripMenuItem.Text = "&Settings";
            // 
            // cPUToolStripMenuItem
            // 
            this.cPUToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.interpreterToolStripMenuItem,
            this.dynamicRecompilerToolStripMenuItem});
            this.cPUToolStripMenuItem.Name = "cPUToolStripMenuItem";
            this.cPUToolStripMenuItem.Size = new System.Drawing.Size(181, 26);
            this.cPUToolStripMenuItem.Text = "&CPU";
            // 
            // interpreterToolStripMenuItem
            // 
            this.interpreterToolStripMenuItem.Checked = true;
            this.interpreterToolStripMenuItem.CheckState = System.Windows.Forms.CheckState.Checked;
            this.interpreterToolStripMenuItem.Name = "interpreterToolStripMenuItem";
            this.interpreterToolStripMenuItem.Size = new System.Drawing.Size(222, 26);
            this.interpreterToolStripMenuItem.Text = "&Interpreter";
            this.interpreterToolStripMenuItem.Click += new System.EventHandler(this.InterpreterToolStripMenuItem_Click);
            // 
            // dynamicRecompilerToolStripMenuItem
            // 
            this.dynamicRecompilerToolStripMenuItem.Enabled = false;
            this.dynamicRecompilerToolStripMenuItem.Name = "dynamicRecompilerToolStripMenuItem";
            this.dynamicRecompilerToolStripMenuItem.Size = new System.Drawing.Size(222, 26);
            this.dynamicRecompilerToolStripMenuItem.Text = "&Dynamic Recompiler";
            this.dynamicRecompilerToolStripMenuItem.Click += new System.EventHandler(this.DynamicRecompilerToolStripMenuItem_Click);
            // 
            // graphicsToolStripMenuItem
            // 
            this.graphicsToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.setColorsToolStripMenuItem,
            this.toolStripSeparator1,
            this.showFPSToolStripMenuItem});
            this.graphicsToolStripMenuItem.Name = "graphicsToolStripMenuItem";
            this.graphicsToolStripMenuItem.Size = new System.Drawing.Size(181, 26);
            this.graphicsToolStripMenuItem.Text = "&Graphics";
            // 
            // setColorsToolStripMenuItem
            // 
            this.setColorsToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.primaryToolStripMenuItem,
            this.secondaryToolStripMenuItem});
            this.setColorsToolStripMenuItem.Name = "setColorsToolStripMenuItem";
            this.setColorsToolStripMenuItem.Size = new System.Drawing.Size(181, 26);
            this.setColorsToolStripMenuItem.Text = "Set &Colors";
            // 
            // primaryToolStripMenuItem
            // 
            this.primaryToolStripMenuItem.Name = "primaryToolStripMenuItem";
            this.primaryToolStripMenuItem.Size = new System.Drawing.Size(162, 26);
            this.primaryToolStripMenuItem.Text = "&Primary...";
            this.primaryToolStripMenuItem.Click += new System.EventHandler(this.PrimaryToolStripMenuItem_Click);
            // 
            // secondaryToolStripMenuItem
            // 
            this.secondaryToolStripMenuItem.Name = "secondaryToolStripMenuItem";
            this.secondaryToolStripMenuItem.Size = new System.Drawing.Size(162, 26);
            this.secondaryToolStripMenuItem.Text = "S&econdary...";
            this.secondaryToolStripMenuItem.Click += new System.EventHandler(this.SecondaryToolStripMenuItem_Click);
            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            this.toolStripSeparator1.Size = new System.Drawing.Size(178, 6);
            // 
            // showFPSToolStripMenuItem
            // 
            this.showFPSToolStripMenuItem.CheckOnClick = true;
            this.showFPSToolStripMenuItem.Name = "showFPSToolStripMenuItem";
            this.showFPSToolStripMenuItem.Size = new System.Drawing.Size(181, 26);
            this.showFPSToolStripMenuItem.Text = "Show &FPS";
            // 
            // debugToolStripMenuItem
            // 
            this.debugToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.historyToolStripMenuItem2});
            this.debugToolStripMenuItem.Name = "debugToolStripMenuItem";
            this.debugToolStripMenuItem.Size = new System.Drawing.Size(66, 24);
            this.debugToolStripMenuItem.Text = "&Debug";
            // 
            // historyToolStripMenuItem2
            // 
            this.historyToolStripMenuItem2.Name = "historyToolStripMenuItem2";
            this.historyToolStripMenuItem2.Size = new System.Drawing.Size(208, 26);
            this.historyToolStripMenuItem2.Text = "S&how Debug Info...";
            this.historyToolStripMenuItem2.Click += new System.EventHandler(this.HistoryToolStripMenuItem2_Click);
            // 
            // pc_label
            // 
            this.pc_label.AutoSize = true;
            this.pc_label.Location = new System.Drawing.Point(740, 7);
            this.pc_label.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.pc_label.Name = "pc_label";
            this.pc_label.Size = new System.Drawing.Size(80, 17);
            this.pc_label.TabIndex = 1;
            this.pc_label.Text = "PC: 0xFFFF";
            // 
            // opcode_label
            // 
            this.opcode_label.AutoSize = true;
            this.opcode_label.Location = new System.Drawing.Point(649, 7);
            this.opcode_label.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.opcode_label.Name = "opcode_label";
            this.opcode_label.Size = new System.Drawing.Size(81, 17);
            this.opcode_label.TabIndex = 2;
            this.opcode_label.Text = "Op: 0xFFFF";
            // 
            // pauseButton
            // 
            this.pauseButton.Location = new System.Drawing.Point(567, 0);
            this.pauseButton.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.pauseButton.Name = "pauseButton";
            this.pauseButton.Size = new System.Drawing.Size(75, 30);
            this.pauseButton.TabIndex = 3;
            this.pauseButton.Text = "Pause";
            this.pauseButton.UseVisualStyleBackColor = true;
            this.pauseButton.Click += new System.EventHandler(this.PauseButton_Click);
            // 
            // fpsSelector
            // 
            this.fpsSelector.AllowDrop = true;
            this.fpsSelector.InterceptArrowKeys = false;
            this.fpsSelector.Location = new System.Drawing.Point(501, 2);
            this.fpsSelector.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.fpsSelector.Maximum = new decimal(new int[] {
            120,
            0,
            0,
            0});
            this.fpsSelector.Name = "fpsSelector";
            this.fpsSelector.Size = new System.Drawing.Size(57, 22);
            this.fpsSelector.TabIndex = 4;
            this.fpsSelector.Value = new decimal(new int[] {
            60,
            0,
            0,
            0});
            this.fpsSelector.ValueChanged += new System.EventHandler(this.FPSSelector_Changed);
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(853, 423);
            this.Controls.Add(this.fpsSelector);
            this.Controls.Add(this.pauseButton);
            this.Controls.Add(this.opcode_label);
            this.Controls.Add(this.pc_label);
            this.Controls.Add(this.menuStrip1);
            this.DoubleBuffered = true;
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MainMenuStrip = this.menuStrip1;
            this.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.MaximizeBox = false;
            this.Name = "MainForm";
            this.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.ShowIcon = false;
            this.Text = "Chip-8";
            this.KeyDown += new System.Windows.Forms.KeyEventHandler(this.MainForm_KeyDown);
            this.KeyUp += new System.Windows.Forms.KeyEventHandler(this.MainForm_KeyUp);
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.fpsSelector)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.MenuStrip menuStrip1;
        private System.Windows.Forms.ToolStripMenuItem fileToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem loadToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem quitToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem settingsToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem cPUToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem interpreterToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem dynamicRecompilerToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem graphicsToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem setColorsToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
        private System.Windows.Forms.ToolStripMenuItem showFPSToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem primaryToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem secondaryToolStripMenuItem;
        private System.Windows.Forms.Label pc_label;
        private System.Windows.Forms.Label opcode_label;
        private System.Windows.Forms.Button pauseButton;
        private System.Windows.Forms.ToolStripMenuItem debugToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem historyToolStripMenuItem2;
        private System.Windows.Forms.NumericUpDown fpsSelector;
    }
}

