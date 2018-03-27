using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Schoolyard.LCD
{
    public class PPURegisters : Memory.RAM
    {
        public PPURegisters(string name, ushort addressBase, ushort size) : base(name, addressBase,size) {}
    }
}
