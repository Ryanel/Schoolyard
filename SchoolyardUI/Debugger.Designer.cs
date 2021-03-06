﻿namespace SchoolyardUI
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
            this.cFlag = new System.Windows.Forms.CheckBox();
            this.hFlag = new System.Windows.Forms.CheckBox();
            this.nFlag = new System.Windows.Forms.CheckBox();
            this.zFlag = new System.Windows.Forms.CheckBox();
            this.labelPC = new System.Windows.Forms.Label();
            this.labelDE = new System.Windows.Forms.Label();
            this.labelSP = new System.Windows.Forms.Label();
            this.labelHL = new System.Windows.Forms.Label();
            this.labelBC = new System.Windows.Forms.Label();
            this.labelAF = new System.Windows.Forms.Label();
            this.ppuGroupBox = new System.Windows.Forms.GroupBox();
            this.updateTiles = new System.Windows.Forms.CheckBox();
            this.ppuTiles = new System.Windows.Forms.PictureBox();
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.fileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.saveStateToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.loadStateToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.closeToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.viewToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.vRAMViewerToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.statusStrip = new System.Windows.Forms.StatusStrip();
            this.ipsLabel = new System.Windows.Forms.ToolStripStatusLabel();
            this.fpsLabel = new System.Windows.Forms.ToolStripStatusLabel();
            this.DebugTimer = new System.Windows.Forms.Timer(this.components);
            this.PerSecondTimer = new System.Windows.Forms.Timer(this.components);
            this.cpuGroupBox.SuspendLayout();
            this.ppuGroupBox.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.ppuTiles)).BeginInit();
            this.menuStrip1.SuspendLayout();
            this.statusStrip.SuspendLayout();
            this.SuspendLayout();
            // 
            // dissassemblyGroupBox
            // 
            this.dissassemblyGroupBox.Location = new System.Drawing.Point(245, 33);
            this.dissassemblyGroupBox.Name = "dissassemblyGroupBox";
            this.dissassemblyGroupBox.Size = new System.Drawing.Size(239, 349);
            this.dissassemblyGroupBox.TabIndex = 0;
            this.dissassemblyGroupBox.TabStop = false;
            this.dissassemblyGroupBox.Text = "Dissassembly";
            // 
            // cpuGroupBox
            // 
            this.cpuGroupBox.Controls.Add(this.cFlag);
            this.cpuGroupBox.Controls.Add(this.hFlag);
            this.cpuGroupBox.Controls.Add(this.nFlag);
            this.cpuGroupBox.Controls.Add(this.zFlag);
            this.cpuGroupBox.Controls.Add(this.labelPC);
            this.cpuGroupBox.Controls.Add(this.labelDE);
            this.cpuGroupBox.Controls.Add(this.labelSP);
            this.cpuGroupBox.Controls.Add(this.labelHL);
            this.cpuGroupBox.Controls.Add(this.labelBC);
            this.cpuGroupBox.Controls.Add(this.labelAF);
            this.cpuGroupBox.Location = new System.Drawing.Point(12, 27);
            this.cpuGroupBox.Name = "cpuGroupBox";
            this.cpuGroupBox.Size = new System.Drawing.Size(134, 112);
            this.cpuGroupBox.TabIndex = 1;
            this.cpuGroupBox.TabStop = false;
            this.cpuGroupBox.Text = "CPU";
            // 
            // cFlag
            // 
            this.cFlag.AutoSize = true;
            this.cFlag.Location = new System.Drawing.Point(95, 61);
            this.cFlag.Name = "cFlag";
            this.cFlag.Size = new System.Drawing.Size(33, 17);
            this.cFlag.TabIndex = 4;
            this.cFlag.Text = "C";
            this.cFlag.UseVisualStyleBackColor = true;
            // 
            // hFlag
            // 
            this.hFlag.AutoSize = true;
            this.hFlag.Location = new System.Drawing.Point(95, 46);
            this.hFlag.Name = "hFlag";
            this.hFlag.Size = new System.Drawing.Size(34, 17);
            this.hFlag.TabIndex = 4;
            this.hFlag.Text = "H";
            this.hFlag.UseVisualStyleBackColor = true;
            // 
            // nFlag
            // 
            this.nFlag.AutoSize = true;
            this.nFlag.Location = new System.Drawing.Point(95, 31);
            this.nFlag.Name = "nFlag";
            this.nFlag.Size = new System.Drawing.Size(34, 17);
            this.nFlag.TabIndex = 4;
            this.nFlag.Text = "N";
            this.nFlag.UseVisualStyleBackColor = true;
            // 
            // zFlag
            // 
            this.zFlag.AutoSize = true;
            this.zFlag.Location = new System.Drawing.Point(95, 16);
            this.zFlag.Name = "zFlag";
            this.zFlag.Size = new System.Drawing.Size(33, 17);
            this.zFlag.TabIndex = 4;
            this.zFlag.Text = "Z";
            this.zFlag.UseVisualStyleBackColor = true;
            // 
            // labelPC
            // 
            this.labelPC.AutoSize = true;
            this.labelPC.Font = new System.Drawing.Font("Consolas", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.labelPC.Location = new System.Drawing.Point(7, 91);
            this.labelPC.Name = "labelPC";
            this.labelPC.Size = new System.Drawing.Size(35, 15);
            this.labelPC.TabIndex = 3;
            this.labelPC.Text = "PC: ";
            // 
            // labelDE
            // 
            this.labelDE.AutoSize = true;
            this.labelDE.Font = new System.Drawing.Font("Consolas", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.labelDE.Location = new System.Drawing.Point(7, 46);
            this.labelDE.Name = "labelDE";
            this.labelDE.Size = new System.Drawing.Size(35, 15);
            this.labelDE.TabIndex = 2;
            this.labelDE.Text = "DE: ";
            // 
            // labelSP
            // 
            this.labelSP.AutoSize = true;
            this.labelSP.Font = new System.Drawing.Font("Consolas", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.labelSP.Location = new System.Drawing.Point(7, 76);
            this.labelSP.Name = "labelSP";
            this.labelSP.Size = new System.Drawing.Size(35, 15);
            this.labelSP.TabIndex = 2;
            this.labelSP.Text = "SP: ";
            // 
            // labelHL
            // 
            this.labelHL.AutoSize = true;
            this.labelHL.Font = new System.Drawing.Font("Consolas", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.labelHL.Location = new System.Drawing.Point(7, 61);
            this.labelHL.Name = "labelHL";
            this.labelHL.Size = new System.Drawing.Size(35, 15);
            this.labelHL.TabIndex = 2;
            this.labelHL.Text = "HL: ";
            // 
            // labelBC
            // 
            this.labelBC.AutoSize = true;
            this.labelBC.Font = new System.Drawing.Font("Consolas", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.labelBC.Location = new System.Drawing.Point(7, 31);
            this.labelBC.Name = "labelBC";
            this.labelBC.Size = new System.Drawing.Size(35, 15);
            this.labelBC.TabIndex = 1;
            this.labelBC.Text = "BC: ";
            // 
            // labelAF
            // 
            this.labelAF.AutoSize = true;
            this.labelAF.Font = new System.Drawing.Font("Consolas", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.labelAF.Location = new System.Drawing.Point(7, 16);
            this.labelAF.Name = "labelAF";
            this.labelAF.Size = new System.Drawing.Size(35, 15);
            this.labelAF.TabIndex = 0;
            this.labelAF.Text = "AF: ";
            // 
            // ppuGroupBox
            // 
            this.ppuGroupBox.Controls.Add(this.updateTiles);
            this.ppuGroupBox.Controls.Add(this.ppuTiles);
            this.ppuGroupBox.Location = new System.Drawing.Point(12, 145);
            this.ppuGroupBox.Name = "ppuGroupBox";
            this.ppuGroupBox.Size = new System.Drawing.Size(134, 237);
            this.ppuGroupBox.TabIndex = 2;
            this.ppuGroupBox.TabStop = false;
            this.ppuGroupBox.Text = "PPU";
            // 
            // updateTiles
            // 
            this.updateTiles.AutoSize = true;
            this.updateTiles.Checked = true;
            this.updateTiles.CheckState = System.Windows.Forms.CheckState.Checked;
            this.updateTiles.Location = new System.Drawing.Point(2, 80);
            this.updateTiles.Name = "updateTiles";
            this.updateTiles.Size = new System.Drawing.Size(122, 17);
            this.updateTiles.TabIndex = 1;
            this.updateTiles.Text = "Update Tile Preview";
            this.updateTiles.UseVisualStyleBackColor = true;
            this.updateTiles.CheckedChanged += new System.EventHandler(this.updateTiles_CheckedChanged);
            // 
            // ppuTiles
            // 
            this.ppuTiles.Location = new System.Drawing.Point(2, 103);
            this.ppuTiles.Name = "ppuTiles";
            this.ppuTiles.Size = new System.Drawing.Size(128, 128);
            this.ppuTiles.TabIndex = 0;
            this.ppuTiles.TabStop = false;
            this.ppuTiles.Paint += new System.Windows.Forms.PaintEventHandler(this.ppuTiles_Paint);
            // 
            // menuStrip1
            // 
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.fileToolStripMenuItem,
            this.viewToolStripMenuItem});
            this.menuStrip1.Location = new System.Drawing.Point(0, 0);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.Size = new System.Drawing.Size(496, 24);
            this.menuStrip1.TabIndex = 3;
            this.menuStrip1.Text = "menuStrip1";
            // 
            // fileToolStripMenuItem
            // 
            this.fileToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.saveStateToolStripMenuItem,
            this.loadStateToolStripMenuItem,
            this.closeToolStripMenuItem});
            this.fileToolStripMenuItem.Name = "fileToolStripMenuItem";
            this.fileToolStripMenuItem.Size = new System.Drawing.Size(37, 20);
            this.fileToolStripMenuItem.Text = "File";
            // 
            // saveStateToolStripMenuItem
            // 
            this.saveStateToolStripMenuItem.Name = "saveStateToolStripMenuItem";
            this.saveStateToolStripMenuItem.Size = new System.Drawing.Size(129, 22);
            this.saveStateToolStripMenuItem.Text = "Save State";
            // 
            // loadStateToolStripMenuItem
            // 
            this.loadStateToolStripMenuItem.Name = "loadStateToolStripMenuItem";
            this.loadStateToolStripMenuItem.Size = new System.Drawing.Size(129, 22);
            this.loadStateToolStripMenuItem.Text = "Load State";
            // 
            // closeToolStripMenuItem
            // 
            this.closeToolStripMenuItem.Name = "closeToolStripMenuItem";
            this.closeToolStripMenuItem.Size = new System.Drawing.Size(129, 22);
            this.closeToolStripMenuItem.Text = "Close";
            // 
            // viewToolStripMenuItem
            // 
            this.viewToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.vRAMViewerToolStripMenuItem});
            this.viewToolStripMenuItem.Name = "viewToolStripMenuItem";
            this.viewToolStripMenuItem.Size = new System.Drawing.Size(44, 20);
            this.viewToolStripMenuItem.Text = "View";
            // 
            // vRAMViewerToolStripMenuItem
            // 
            this.vRAMViewerToolStripMenuItem.Name = "vRAMViewerToolStripMenuItem";
            this.vRAMViewerToolStripMenuItem.Size = new System.Drawing.Size(145, 22);
            this.vRAMViewerToolStripMenuItem.Text = "VRAM Viewer";
            // 
            // statusStrip
            // 
            this.statusStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.ipsLabel,
            this.fpsLabel});
            this.statusStrip.Location = new System.Drawing.Point(0, 386);
            this.statusStrip.Name = "statusStrip";
            this.statusStrip.Size = new System.Drawing.Size(496, 22);
            this.statusStrip.TabIndex = 4;
            this.statusStrip.Text = "statusStrip1";
            // 
            // ipsLabel
            // 
            this.ipsLabel.AutoSize = false;
            this.ipsLabel.Name = "ipsLabel";
            this.ipsLabel.Size = new System.Drawing.Size(64, 17);
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
            this.DebugTimer.Interval = 8;
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
            this.ClientSize = new System.Drawing.Size(496, 408);
            this.Controls.Add(this.statusStrip);
            this.Controls.Add(this.ppuGroupBox);
            this.Controls.Add(this.cpuGroupBox);
            this.Controls.Add(this.dissassemblyGroupBox);
            this.Controls.Add(this.menuStrip1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            this.MainMenuStrip = this.menuStrip1;
            this.Name = "Debugger";
            this.Text = "Debugger";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.Debugger_FormClosing);
            this.Load += new System.EventHandler(this.Debugger_Load);
            this.cpuGroupBox.ResumeLayout(false);
            this.cpuGroupBox.PerformLayout();
            this.ppuGroupBox.ResumeLayout(false);
            this.ppuGroupBox.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.ppuTiles)).EndInit();
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
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
        private System.Windows.Forms.Label labelPC;
        private System.Windows.Forms.Label labelDE;
        private System.Windows.Forms.Label labelSP;
        private System.Windows.Forms.Label labelHL;
        private System.Windows.Forms.Label labelBC;
        private System.Windows.Forms.Label labelAF;
        private System.Windows.Forms.ToolStripMenuItem fileToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem saveStateToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem loadStateToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem closeToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem viewToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem vRAMViewerToolStripMenuItem;
        private System.Windows.Forms.PictureBox ppuTiles;
        private System.Windows.Forms.CheckBox cFlag;
        private System.Windows.Forms.CheckBox hFlag;
        private System.Windows.Forms.CheckBox nFlag;
        private System.Windows.Forms.CheckBox zFlag;
        private System.Windows.Forms.CheckBox updateTiles;
    }
}