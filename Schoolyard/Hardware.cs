using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Schoolyard
{
    public interface IHardware
    {
        void Step(long cycles);
        void Reset();
    }
}
