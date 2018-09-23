using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Pbz_extractor
{
    class PebbleApplication
    {
        public uint reqFwVer, timestamp, crc;
        public string name;
        public uint size;

        public void Print()
        {
            Console.WriteLine("Pebble Application, '{0}'", name);
            Console.WriteLine("Timestamp: {0}", Tool.intToDate(timestamp));
            Console.WriteLine("CRC: {0}", crc);
            Console.WriteLine("Firmware Required: {0}", reqFwVer);
            Console.WriteLine("Application File Size: {0}", Tool.usefulSize(size));
        }
    }
}
