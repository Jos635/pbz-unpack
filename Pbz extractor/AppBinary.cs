using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using MessageBuffers;

namespace Pbz_extractor
{
    class AppBinary
    {
        byte[] data;
        string magic;
        int version, sdk_version, app_version, size;
        uint entry_point, crc;
        string name, company;
        uint icon_resource_id, symbol_table, flags, reloc_list_offset, num_relocs;
        byte[] uuid;
        string path;
        public AppBinary(string path)
        {
            this.path = path;
            data = File.ReadAllBytes(path);
            MessageReader r = new MessageReader(data);
            magic = ASCIIEncoding.ASCII.GetString(r.readBytes(8)).Replace("\0", "");
            version = r.readUShort();
            sdk_version = r.readUShort();
            app_version = r.readUShort();
            size = r.readUShort();

            entry_point = r.readUInt();
            crc = r.readUInt();

            name = ASCIIEncoding.ASCII.GetString(r.readBytes(32)).Replace("\0", "");
            company = ASCIIEncoding.ASCII.GetString(r.readBytes(32)).Replace("\0", "");

            icon_resource_id = r.readUInt();
            symbol_table = r.readUInt();
            flags = r.readUInt();
            reloc_list_offset = r.readUInt();
            num_relocs = r.readUInt();

            uuid = r.readBytes(16);

            File.WriteAllBytes(path.Replace(".bin", ".raw"), r.remainingBytes);
        }

        public void findReplace(byte[] oldInt, byte[] newInt)
        {
            Console.WriteLine("Finding and replacing...");
            for (int i = data.Length - 4; i > 0; i -= 1)
            {
                for (int j = 0; j < 4; j++)
                {
                    if (oldInt[j] != data[i + j])
                    {
                        goto NotEqual;
                    }
                }

                //Found has point
                Console.WriteLine("Found block at positon #" + i + ". Now replacing CRC");
                Buffer.BlockCopy(newInt, 0, data, i, 4);
                break;

            NotEqual:
                continue;
            }
        }

        public void Print()
        {
            Console.WriteLine("\n\nAPP {0}: {1} made by {2}", magic, name, company);
            Console.WriteLine("Version {0}, SDK version {1}, App Version {2}", version, sdk_version, app_version);
            Console.WriteLine("Size: {0}", size);
            Console.WriteLine("Entry point: {0}", entry_point);
            Console.WriteLine("Resource Icon ID: {0}, Symbol Table: {1}", icon_resource_id, symbol_table);
            Console.WriteLine("Flags: {0}, Reloc List Offset {1}, Reloc Count {2}", flags, reloc_list_offset, num_relocs);
        }

        public void Save()
        {
            Console.WriteLine("Old PBAPP crc: {0} vs new {1}", Crc.crc32(File.ReadAllBytes(path)), Crc.crc32(data));
            File.WriteAllBytes(path, data);
        }
    }
}
