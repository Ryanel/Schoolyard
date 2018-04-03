namespace SchoolyardUI
{
    partial class MainWindow
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
            this.components = new System.ComponentModel.Container();
            this.globalTimer = new System.Windows.Forms.Timer(this.components);
            this.openFileDialog = new System.Windows.Forms.OpenFileDialog();
            this.menuStrip = new System.Windows.Forms.MenuStrip();
            this.fileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.openToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.viewToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.debuggerToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.sfmlThread = new System.ComponentModel.BackgroundWorker();
            this.renderSurface = new SchoolyardUI.DrawingSurface();
            this.menuStrip.SuspendLayout();
            this.SuspendLayout();
            // 
            // globalTimer
            // 
            this.globalTimer.Enabled = true;
            this.globalTimer.Interval = 16;
            this.globalTimer.Tick += new System.EventHandler(this.MainTimer);
            // 
            // openFileDialog
            // 
            this.openFileDialog.Filter = "Gameboy ROMS|*.gb";
            // 
            // menuStrip
            // 
            this.menuStrip.BackColor = System.Drawing.SystemColors.Control;
            this.menuStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.fileToolStripMenuItem,
            this.viewToolStripMenuItem});
            this.menuStrip.Location = new System.Drawing.Point(0, 0);
            this.menuStrip.Name = "menuStrip";
            this.menuStrip.Size = new System.Drawing.Size(479, 24);
            this.menuStrip.TabIndex = 1;
            this.menuStrip.Text = "menuStrip1";
            // 
            // fileToolStripMenuItem
            // 
            this.fileToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.openToolStripMenuItem});
            this.fileToolStripMenuItem.Name = "fileToolStripMenuItem";
            this.fileToolStripMenuItem.Size = new System.Drawing.Size(37, 20);
            this.fileToolStripMenuItem.Text = "File";
            // 
            // openToolStripMenuItem
            // 
            this.openToolStripMenuItem.Name = "openToolStripMenuItem";
            this.openToolStripMenuItem.Size = new System.Drawing.Size(103, 22);
            this.openToolStripMenuItem.Text = "Open";
            this.openToolStripMenuItem.Click += new System.EventHandler(this.OpenToolStripMenuItem_Click);
            // 
            // viewToolStripMenuItem
            // 
            this.viewToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.debuggerToolStripMenuItem});
            this.viewToolStripMenuItem.Name = "viewToolStripMenuItem";
            this.viewToolStripMenuItem.Size = new System.Drawing.Size(44, 20);
            this.viewToolStripMenuItem.Text = "View";
            // 
            // debuggerToolStripMenuItem
            // 
            this.debuggerToolStripMenuItem.Name = "debuggerToolStripMenuItem";
            this.debuggerToolStripMenuItem.Size = new System.Drawing.Size(126, 22);
            this.debuggerToolStripMenuItem.Text = "Debugger";
            this.debuggerToolStripMenuItem.Click += new System.EventHandler(this.debuggerToolStripMenuItem_Click);
            // 
            // sfmlThread
            // 
            this.sfmlThread.DoWork += new System.ComponentModel.DoWorkEventHandler(this.smflDisplayThread_DoWork);
            // 
            // renderSurface
            // 
            this.renderSurface.Dock = System.Windows.Forms.DockStyle.Fill;
            this.renderSurface.Location = new System.Drawing.Point(0, 24);
            this.renderSurface.Name = "renderSurface";
            this.renderSurface.Size = new System.Drawing.Size(479, 431);
            this.renderSurface.TabIndex = 2;
            // 
            // MainWindow
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(479, 455);
            this.Controls.Add(this.renderSurface);
            this.Controls.Add(this.menuStrip);
            this.MainMenuStrip = this.menuStrip;
            this.Name = "MainWindow";
            this.Text = "Schoolyard";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.MainWindow_FormClosing);
            this.Load += new System.EventHandler(this.MainWindow_Load);
            this.menuStrip.ResumeLayout(false);
            this.menuStrip.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.Timer globalTimer;
        private System.Windows.Forms.OpenFileDialog openFileDialog;
        private System.Windows.Forms.MenuStrip menuStrip;
        private System.Windows.Forms.ToolStripMenuItem fileToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem openToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem viewToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem debuggerToolStripMenuItem;
        private System.ComponentModel.BackgroundWorker sfmlThread;
        private DrawingSurface renderSurface;
    }
}

