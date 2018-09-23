using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Pbz_extractor
{
    class PebbleFirmware
    {
        public string name;
        public long timestamp, crc;
        public string hwrev, type;
        public long size;

        public void Print()
        {
            Console.WriteLine("Pebble Firmware, '{0}'", name);
            Console.WriteLine("Timestamp: {0}", Tool.intToDate(timestamp));
            Console.WriteLine("CRC: {0}", crc);
            Console.WriteLine("Hardware Revision: {0}", hwrev);
            Console.WriteLine("Firmware Type: {0}", type);
            Console.WriteLine("Firmware File Size: {0}", Tool.usefulSize(size));
        }
    }
}
