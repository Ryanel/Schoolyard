namespace SchoolyardUI
{
    partial class Debugger
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
            this.dissassemblyGroupBox = new System.Windows.Forms.GroupBox();
            this.cpuGroupBox = new System.Windows.Forms.GroupBox();
            this.ppuGroupBox = new System.Windows.Forms.GroupBox();
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.statusStrip = new System.Windows.Forms.StatusStrip();
            this.ipsLabel = new System.Windows.Forms.ToolStripStatusLabel();
            this.fpsLabel = new System.Windows.Forms.ToolStripStatusLabel();
            this.DebugTimer = new System.Windows.Forms.Timer(this.components);
            this.PerSecondTimer = new System.Windows.Forms.Timer(this.components);
            this.statusStrip.SuspendLayout();
            this.SuspendLayout();
            // 
            // dissassemblyGroupBox
            // 
            this.dissassemblyGroupBox.Location = new System.Drawing.Point(245, 33);
            this.dissassemblyGroupBox.Name = "dissassemblyGroupBox";
            this.dissassemblyGroupBox.Size = new System.Drawing.Size(239, 430);
            this.dissassemblyGroupBox.TabIndex = 0;
            this.dissassemblyGroupBox.TabStop = false;
            this.dissassemblyGroupBox.Text = "Dissassembly";
            // 
            // cpuGroupBox
            // 
            this.cpuGroupBox.Location = new System.Drawing.Point(12, 27);
            this.cpuGroupBox.Name = "cpuGroupBox";
            this.cpuGroupBox.Size = new System.Drawing.Size(216, 138);
            this.cpuGroupBox.TabIndex = 1;
            this.cpuGroupBox.TabStop = false;
            this.cpuGroupBox.Text = "CPU";
            // 
            // ppuGroupBox
            // 
            this.ppuGroupBox.Location = new System.Drawing.Point(12, 171);
            this.ppuGroupBox.Name = "ppuGroupBox";
            this.ppuGroupBox.Size = new System.Drawing.Size(216, 292);
            this.ppuGroupBox.TabIndex = 2;
            this.ppuGroupBox.TabStop = false;
            this.ppuGroupBox.Text = "PPU";
            // 
            // menuStrip1
            // 
            this.menuStrip1.Location = new System.Drawing.Point(0, 0);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.Size = new System.Drawing.Size(496, 24);
            this.menuStrip1.TabIndex = 3;
            this.menuStrip1.Text = "menuStrip1";
            // 
            // statusStrip
            // 
            this.statusStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.ipsLabel,
            this.fpsLabel});
            this.statusStrip.Location = new System.Drawing.Point(0, 477);
            this.statusStrip.Name = "statusStrip";
            this.statusStrip.Size = new System.Drawing.Size(496, 22);
            this.statusStrip.TabIndex = 4;
            this.statusStrip.Text = "statusStrip1";
            // 
            // ipsLabel
            // 
            this.ipsLabel.AutoSize = false;
            this.ipsLabel.Name = "ipsLabel";
            this.ipsLabel.Size = new System.Drawing.Size(96, 17);
            this.ipsLabel.Text = "IPS: 0";
            this.ipsLabel.TextAlign = System.Drawing.ContentAlignment.TopLeft;
            // 
            // fpsLabel
            // 
            this.fpsLabel.AutoSize = false;
            this.fpsLabel.Name = "fpsLabel";
            this.fpsLabel.Size = new System.Drawing.Size(56, 17);
            this.fpsLabel.Text = "FPS: 0";
            this.fpsLabel.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // DebugTimer
            // 
            this.DebugTimer.Enabled = true;
            this.DebugTimer.Tick += new System.EventHandler(this.DebugTimer_Tick);
            // 
            // PerSecondTimer
            // 
            this.PerSecondTimer.Enabled = true;
            this.PerSecondTimer.Interval = 1000;
            this.PerSecondTimer.Tick += new System.EventHandler(this.PerSecondTimer_Tick);
            // 
            // Debugger
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(496, 499);
            this.Controls.Add(this.statusStrip);
            this.Controls.Add(this.ppuGroupBox);
            this.Controls.Add(this.cpuGroupBox);
            this.Controls.Add(this.dissassemblyGroupBox);
            this.Controls.Add(this.menuStrip1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            this.MainMenuStrip = this.menuStrip1;
            this.Name = "Debugger";
            this.Text = "Debugger";
            this.Load += new System.EventHandler(this.Debugger_Load);
            this.statusStrip.ResumeLayout(false);
            this.statusStrip.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.GroupBox dissassemblyGroupBox;
        private System.Windows.Forms.GroupBox cpuGroupBox;
        private System.Windows.Forms.GroupBox ppuGroupBox;
        private System.Windows.Forms.MenuStrip menuStrip1;
        private System.Windows.Forms.StatusStrip statusStrip;
        private System.Windows.Forms.ToolStripStatusLabel ipsLabel;
        private System.Windows.Forms.ToolStripStatusLabel fpsLabel;
        private System.Windows.Forms.Timer DebugTimer;
        private System.Windows.Forms.Timer PerSecondTimer;
    }
}