using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Pbz_extractor
{
    class PebbleResource
    {
        public string defName;
        public string type;
        public string file;

        public void Print(int n)
        {
            Console.WriteLine("{3}: {0} ({1}) in file {2}", defName, type, file, n + 1);
        }
    }
}
