using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SchoolyardUI
{
    public static class ThreadUtilities
    {
        public static void SleepUntilTargetTime(System.Diagnostics.Stopwatch sw, float targetTime)
        {
            if (sw.Elapsed.TotalMilliseconds >= targetTime) { return; }

            bool hitTime = false;

            while (hitTime == false)
            {
                if (targetTime - sw.Elapsed.TotalMilliseconds > 1f)
                {
                    System.Threading.Thread.Sleep(1);
                }
                else
                {
                    while (sw.Elapsed.TotalMilliseconds < targetTime)
                    {
                    }
                    return;
                }

            }
        }
    }
}
