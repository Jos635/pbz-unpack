using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Pbz_extractor
{
    class PebbleDebug
    {
        public PebbleResourceMap resourceMap;
        
        

        public void Print()
        {
            Console.WriteLine("Debug Information");
            resourceMap.Print();
            //resources.Print();
        }
    }
}
