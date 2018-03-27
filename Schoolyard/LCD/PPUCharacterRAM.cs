using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Schoolyard.LCD
{
    public class PPUCharacterRAM : Memory.RAM
    {
        public PPUCharacterRAM(string name, ushort addressBase, ushort size) : base(name, addressBase,size) {}
    }
}
