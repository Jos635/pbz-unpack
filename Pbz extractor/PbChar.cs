using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Pbz_extractor
{
    class PbChar
    {
        public int offset, index;
        public byte[] data;
        public PbChar(int offset, int index)
        {
            this.offset = offset;
            this.index = index;
        }
    }
}
