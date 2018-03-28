using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Schoolyard;
namespace SchoolyardUI
{
    public partial class Debugger : Form
    {
        private Gameboy target;

        private ulong lastInstructions = 0;
        private ulong lastFrame = 0;

        public Debugger(Gameboy target)
        {
            InitializeComponent();
            this.target = target;
        }

        private void Debugger_Load(object sender, EventArgs e)
        {
            Size toResize = Size;
            toResize.Width = 173;
            Size = toResize;
        }

        private void DebugTimer_Tick(object sender, EventArgs e)
        {
            // CPU
            labelAF.Text = String.Format("BC: {0:X4}", target.cpu.AF);
            labelBC.Text = String.Format("BC: {0:X4}", target.cpu.BC);
            labelDE.Text = String.Format("BC: {0:X4}", target.cpu.DE);
            labelHL.Text = String.Format("BC: {0:X4}", target.cpu.HL);
            labelSP.Text = String.Format("SP: {0:X4}", target.cpu.SP);
            labelPC.Text = String.Format("PC: {0:X4}", target.cpu.PC);
        }

        private void PerSecondTimer_Tick(object sender, EventArgs e)
        {
            ulong instructionsDelta = target.cpu.instructionsExecuted - lastInstructions;
            lastInstructions = target.cpu.instructionsExecuted;

            ulong frameDelta = target.ppu.framesRendered - lastFrame;
            lastFrame = target.ppu.framesRendered;

            ipsLabel.Text = "IPS: " + instructionsDelta;
            fpsLabel.Text = "FPS: " + frameDelta;
        }

        private void Debugger_FormClosing(object sender, FormClosingEventArgs e)
        {
            e.Cancel = true;
            this.Hide();
        }
    }
}
