using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SchoolyardUI
{
    public class DrawingSurface : System.Windows.Forms.Control
    {
        public bool BeingDrawnTo = false;
        protected override void OnPaint(System.Windows.Forms.PaintEventArgs e)
        {
            if(!BeingDrawnTo)
            {
                base.OnPaint(e);
            }
        }
        protected override void OnPaintBackground(System.Windows.Forms.PaintEventArgs pevent)
        {
            if (!BeingDrawnTo)
            {
                base.OnPaintBackground(pevent);
            }
        }
    }
}
