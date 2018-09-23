using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using MessageBuffers;
using System.Drawing;
using System.Diagnostics;

namespace Pbz_extractor
{
    class PbResource
    {
        public int index, offset, size;
        public long crc;
        public byte[] data;
        public PbResource(int index, int offset, int size, uint crc)
        {
            this.index = index;
            this.crc = crc;
            this.offset = offset;
            this.size = size;
        }

        public void Print()
        {
            Console.WriteLine("Resource {0}, at location {1} to {2}", index, offset, offset + size);
            //Console.WriteLine("CRC: {0}, recalculated: {1} (Binary: {2})", crc, Crc.crc32(data), BitConverter.ToString(BitConverter.GetBytes(crc)));
            //Console.ReadKey();
        }

        public int width, height, scanline_length;
        public int u1;
        public int u2;
        #region Saving
        public MessageReader LoadInfo()
        {
            MessageReader r = new MessageReader(data);

            scanline_length = r.readUShort();

            u1 = r.readUShort();
            u2 = r.readInt();

            width = r.readUShort();
            height = r.readUShort();

            Console.WriteLine("Size {0}x{1}; (Bit Depth {4}) U1: {2} U2: {3}", width, height, u1, u2, scanline_length);

            return r;
        }

        public void SaveBitmap(string file)
        {
            Console.WriteLine("Saving bitmap '{0}'", file);
            MessageReader r = LoadInfo();

            int shift = scanline_length * 8 - width;
            int skip_left = 0;
            using (Bitmap b = new Bitmap(width, height))
            {
                int x = 0, y = 0;
                foreach (byte t in r.remainingBytes)
                {
                    for (int i = 0; i < 8; i += 1)
                    {
                        if (skip_left > 0)
                        {
                            skip_left--;
                        }
                        else
                        {
                            b.SetPixel(x, y, (((t >> i) & 1) == 1 ? Color.White : Color.Black));
                            x++;
                            if (x >= width)
                            {
                                x = 0;
                                y++;
                                skip_left = shift;
                                if (y >= height) break;
                            }
                        }
                    }
                    if (y >= height) break;
                }
                b.Save(file + ".png");
            }
            Console.WriteLine("Done saving bitmap");
        }

        public void SaveFont(string path)
        {
            MessageReader r = new MessageReader(data);

            int u1a = r.readByte();
            int u1b = r.readByte();
            int tableSize = r.readUShort();
            int u2 = r.readUShort();

            Console.WriteLine("Processing font file...");
            Console.WriteLine("Table size: {0}, unknown values: {1} {3}, {2}", tableSize, u1a, u2, u1b);

            List<PbChar> chars = new List<PbChar>();

            for (int i = 0; i < tableSize; i++)
            {
                int chr = r.readUShort();
                int offset = r.readUShort();
                chars.Add(new PbChar(offset, chr));
            }

            Console.WriteLine("\n\nSaving FONT to DIRECTORY '{0}'\n", path);
            if (u2 == 13398)
            {
                int beginPos = r.pos;
                MessageReader d = new MessageReader(r.remainingBytes);
                //Console.WriteLine("Begin for...");

                int maxHeight = 0;

                for (int n = 0; n < chars.Count; n += 1)
                {
                    PbChar c = chars[n];
                    while (d.pos < c.offset * 4) { d.readByte(); /*Console.WriteLine("Skipping byte...");*/ }

                    int w = d.readByte(), h = d.readByte();
                    sbyte xMargin = (sbyte)d.readByte(), yMargin = (sbyte)d.readByte();

                    Console.WriteLine("Font char '{4}': Size: {0} {1}, unknown = {2} {3}; Last2: [{5}]x[{6}]", w, h, xMargin, yMargin, (char)c.index, w + xMargin, h + yMargin); //{3} = Y padding?

                    int p1 = d.readByte();
                    int p2 = d.readByte();
                    int p3 = d.readByte();
                    int p4 = d.readByte();

                    Console.WriteLine("4 more unknown's: [{0}, {1}, {2}, {3}]", p1, p2, p3, p4);

                    Debug.Assert(p1 == 0);
                    Debug.Assert(p2 == 0);
                    Debug.Assert(p3 == 0);

                    if (w * h > 0)
                    {
                        maxHeight = Math.Max(maxHeight, h + yMargin);

                        byte[] imagedata = d.readBytes((int)Math.Ceiling(((double)(w * h)) / 8.0));

                        using (Bitmap b = new Bitmap(w, h))
                        {
                            int x = 0, y = 0;
                            foreach (byte t in imagedata)
                            {
                                for (int i = 0; i < 8; i += 1)
                                {
                                    b.SetPixel(x, y, (((t >> i) & 1) == 1 ? Color.White : Color.Black));
                                    x++;
                                    if (x >= w)
                                    {
                                        x = 0;
                                        y++;
                                        if (y >= h) break;
                                    }
                                }
                            }

                            b.Save(path + "image-" + c.index + ".png");
                        }
                    }
                }
                
                Console.WriteLine();
                Console.WriteLine("Comparing u1b [{0}] versus maximum height [{1}]", u1b, maxHeight);
                Console.WriteLine("Bytes left: {0}", BitConverter.ToString(d.remainingBytes));
                //Console.ReadKey();
            }

            
        }

        public void SaveRaw(string file)
        {
            File.WriteAllBytes(file, data);
        }
        #endregion

        #region Updating

        public void UpdatePng(string file)
        {
            LoadInfo();
            using (Bitmap b = new Bitmap(file))
            {
                if (b.Width != width || b.Height != height)
                {
                    throw new RepackException(String.Format("File {0} is not of the same size as the original file ({1} x {2})", file, width, height));
                }

                MessageWriter w = new MessageWriter();

                w.writeUShort((ushort)scanline_length);

                w.writeUShort((ushort)u1);
                w.writeInt(u2);

                w.writeUShort((ushort)width);
                w.writeUShort((ushort)height);

                byte[] imagedata = new byte[scanline_length * height];
                for (int x = 0; x < width; x++)
                {
                    for (int y = 0; y < height; y++)
                    {
                        Color c = b.GetPixel(x, y);
                        int val = (c.R + c.G + c.B) / 3;
                        if (val >= 0x80)
                        {
                            imagedata[(x / 8) + y * scanline_length] |= (byte)(1 << ((x % 8)));
                        }
                    }
                }
                w.writeBytes(imagedata);

                data = w.bytes;
                Console.WriteLine("Old crc: {0}, new crc: {1}", crc, Crc.crc32(data));
                crc = Crc.crc32(data);
                size = data.Length;
                /*byte[] newdata = w.bytes;

                File.WriteAllBytes(file + ".newdat", newdata);

                Console.WriteLine("Length {0} vs {1}", data.Length, newdata.Length);

                for (int i = 0; i < newdata.Length; i++)
                {
                    if(data[i] != newdata[i])
                    {
                        Console.WriteLine("Byte {0} is not the same! {1} {2}", i, imagedata[i], newdata[i]);
                    }
                }*/
                Console.WriteLine("Successfully packed file");
                //Console.ReadKey();
            }
        }

        #endregion
    }
}
