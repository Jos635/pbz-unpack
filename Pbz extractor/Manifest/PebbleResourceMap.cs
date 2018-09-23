using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Pbz_extractor
{
    class PebbleResourceMap
    {
        public List<PebbleResource> media;
        public string friendlyVersion;
        public string versionDefName;

        public void Print()
        {
            Console.WriteLine("Resource Map, {0} resources (media)", media.Count);
            Console.WriteLine("Friendly Version: {0}", friendlyVersion);
            Console.WriteLine("Version Def Name: {0}", versionDefName);
            int i = 0;
            foreach (PebbleResource r in media)
            {
                r.Print(i);
                i++;
            }
        }
    }
}
