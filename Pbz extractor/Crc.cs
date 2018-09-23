using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Pbz_extractor
{
    class Crc
    {
        public const long CRC_POLY = 0x04C11DB7;

        public static long process_word(byte[] data, long crc = 0xffffffff)
        {
            uint d = BitConverter.ToUInt32(data, 0);
            crc = crc ^ d;

            for (int i = 0; i < 32; i ++)
            {
                if ((crc & 0x80000000) != 0)
                {
                    crc = (crc << 1) ^ CRC_POLY;
                }else{
                    crc = (crc << 1);
                }
            }

            return crc & 0xffffffff;
        }

        public static long process_buffer(byte[] buf, long c = 0xffffffff)
        {
            int word_count = buf.Length / 4;
            if (buf.Length % 4 != 0)
            {
                word_count += 1;
            }

            long crc = c;
            for (int i = 0; i < word_count; i ++)
            {
                byte[] process = new byte[4];
                Buffer.BlockCopy(buf, i * 4, process, 0, Math.Min(4, buf.Length - i  * 4));
                crc = process_word(process, crc);
            }

            return crc;
        }

        public static long crc32(byte[] data)
        {
            return process_buffer(data);
        }
    }
}
