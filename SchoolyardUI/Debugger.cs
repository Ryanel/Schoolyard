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
        Gameboy target;
        public Debugger(Gameboy target)
        {
            InitializeComponent();
            this.target = target;
        }

        private void Debugger_Load(object sender, EventArgs e)
        {
            Size toResize = Size;
            toResize.Width = 256;
            Size = toResize;
        }

        
        private void DebugTimer_Tick(object sender, EventArgs e)
        {
            
        }

        private ulong lastInstructions = 0;
        private ulong lastFrame = 0;


        private void PerSecondTimer_Tick(object sender, EventArgs e)
        {
            ulong instructionsDelta = target.cpu.instructionsExecuted - lastInstructions;
            lastInstructions = target.cpu.instructionsExecuted;

            ulong frameDelta = target.ppu.framesRendered - lastFrame;
            lastFrame = target.ppu.framesRendered;

            ipsLabel.Text = "IPS: " + instructionsDelta;
            fpsLabel.Text = "FPS: " + frameDelta;
        }
    }
}
