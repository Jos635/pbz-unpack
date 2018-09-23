using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Pbz_extractor
{
    class PebbleResourceInfo
    {
        public long timestamp;
        public long crc;
        public string friendlyVersion;
        public string name;
        public long size;


        public void Print()
        {
            Console.WriteLine("Resource Info");
            Console.WriteLine("Timestamp: {0}", Tool.intToDate(timestamp));
            Console.WriteLine("CRC: {0} ({1})", crc, BitConverter.ToString(BitConverter.GetBytes(crc)));
            Console.WriteLine("Friendly Version: {0}", friendlyVersion);
            Console.WriteLine("Name: {0}", name);
            Console.WriteLine("Size: {0}", Tool.usefulSize(size));
        }
    }
}
