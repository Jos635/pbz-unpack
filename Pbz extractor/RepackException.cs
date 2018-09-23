using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Pbz_extractor
{
    class RepackException : Exception
    {
        public RepackException(string error) : base(error)
        {

        }
    }
}
