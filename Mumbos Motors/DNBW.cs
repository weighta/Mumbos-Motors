using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mumbos_Motors
{
    public class DNBW
    {
        public string path;
        public int offs;
        public int len;
        public string name;

        public DNBW(string path, int offs, int len)
        {
            this.path = path;
            this.offs = offs;
            this.len = len;

            name = DataMethods.readNullTerminatedString(path, offs + 0x3C);
        }
    }
}
