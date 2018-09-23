using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MessageBuffers;
using System.IO;

namespace Pbz_extractor
{
    class PbPack
    {
        public List<PbResource> resources = new List<PbResource>();
        string file;
        byte[] unknown;
        string version;
        public PbPack(string file)
        {
            this.file = file;
        }

        public void Load(string path, PebbleResourceMap map)
        {
            Console.WriteLine("Extracting resource file");
            MessageReader r = new MessageReader(File.ReadAllBytes(file));

            int resourceNum = r.readInt();
            unknown = r.readBytes(8);
            version = Encoding.ASCII.GetString(r.readBytes(16)).Replace("\0", "");

            Console.WriteLine("{0} resources; Version '{1}'", resourceNum, version);
            Console.WriteLine("Unknown data: {0}", BitConverter.ToString(unknown));

            MessageReader rTable = new MessageReader(r.readBytes(4096));
            for (int i = 0; i < resourceNum; i++)
            {
                PbResource n = new PbResource(rTable.readInt(),
                    rTable.readInt(), rTable.readInt(), rTable.readUInt());
                resources.Add(n);
            }

            byte[] remaining = r.remainingBytes;
            //Console.WriteLine(BitConverter.ToString(remaining));
            foreach (PbResource b in resources)
            {
                b.data = new byte[b.size];
                Buffer.BlockCopy(remaining, b.offset, b.data, 0, b.size);

                b.Print();
            }
        }

        public void Extract(string path, PebbleResourceMap map)
        {
            foreach (PbResource b in resources)
            {
                try
                {
                    string type = "png";
                    try
                    {
                        PebbleResource res = map.media[b.index - 1];
                        type = res.type;
                    }
                    catch { }

                    switch (type)
                    {
                        case "png":
                        case "png-trans":
                            b.SaveBitmap(path + "resource" + b.index);
                            break;
                        case "font":
                            if (!Directory.Exists(path + "resource" + b.index + "\\")) Directory.CreateDirectory(path + "resource" + b.index + "\\");
                            b.SaveFont(path + "resource" + b.index + "\\");
                            break;
                        default:
                            Console.WriteLine("Unknown resource type");
                            try
                            {
                                b.SaveBitmap(path + "resource" + b.index);
                            }
                            catch (Exception e) { Console.WriteLine(e); }
                            break;
                    }
                    b.SaveRaw(path + "resource" + b.index + ".dat");
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                }
            }
        }

        public void Intract(string path, PebbleResourceMap map, AppBinary a)
        {
            foreach (PbResource b in resources)
            {
                try
                {
                    string type = "png";
                    try
                    {
                        PebbleResource res = map.media[b.index - 1];
                        type = res.type;
                    }
                    catch { }

                    uint old_crc = (uint)b.crc;

                    string file = "";
                    switch (type)
                    {
                        case "png":
                        case "png-trans":
                            file = path + "resource" + b.index + ".png";
                            break;
                        case "font":
                            file = path + "resource" + b.index + "\\";
                            break;
                        default:
                            break;
                    }

                    if (!File.Exists(file))
                    {
                        Console.WriteLine("Error: missing file {0}", file);
                    }

                    switch (type)
                    {
                        case "png":
                        case "png-trans":
                            b.UpdatePng(file);
                            break;
                        case "font":
                            file = path + "resource" + b.index + "\\";
                            break;
                    }

                    uint new_crc = (uint)b.crc;
                    a.findReplace(BitConverter.GetBytes(old_crc), BitConverter.GetBytes(new_crc));
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                }
            }

            uint old_pb_crc = (uint)Crc.crc32(File.ReadAllBytes(this.file));

            MessageWriter w = new MessageWriter();
            w.writeInt(resources.Count);
            w.writeBytes(unknown);
            
            w.writeBytes(Tool.ToNullString(version, 16));

            foreach (PbResource r in resources)
            {
                w.writeInt(r.index);
                w.writeInt(r.offset);
                w.writeInt(r.size);
                w.writeUInt((uint)r.crc);
            }

            //Make sure the data starts at 0x101c
            while (w.Length < 0x101c)
            {
                w.writeByte(0);
            }

            foreach (PbResource r in resources)
            {
                w.writeBytes(r.data);
            }

            File.WriteAllBytes(this.file, w.bytes);

            uint new_pb_crc = (uint)Crc.crc32(w.bytes);
            a.findReplace(BitConverter.GetBytes(old_pb_crc), BitConverter.GetBytes(new_pb_crc));

            Console.WriteLine("Old CRC: {0}, new: {1}", old_pb_crc, new_pb_crc);
        }
    }
}
