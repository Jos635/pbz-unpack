using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Pbz_extractor
{
    public static class Tool
    {
        public static DateTime intToDate(long i)
        {
            return new DateTime(1970, 1, 1, 0, 0, 0, 0).AddSeconds(i);
        }

        public static string usefulSize(long s)
        {
            if (s > 1024 * 1024 * 4)
            {
                return String.Format("{0:n}mb", s / 1024 / 1024);
            }
            else if (s > 4096)
            {
                return String.Format("{0:n}kb", s / 1024);
            }
            else
            {
                return s + "b";
            }
        }

        public static byte[] ToNullString(string s, int len)
        {
            return ASCIIEncoding.ASCII.GetBytes(s + new String('\0', len - s.Length));
        }
    }
}
