using System;
using System.Collections.Generic;
using System.IO;

namespace gifresizer.utility
{
    internal class LZWEncoder
    {
        protected static readonly int MaxStackSize = 4096;
        protected static readonly byte NULLCODE = 0;
        byte colorDepth;
        byte initDataSize;
        byte[] indexedPixel;

        internal LZWEncoder(byte[] indexedPixel, byte colorDepth)
        {
            this.indexedPixel = indexedPixel;
            this.colorDepth = (byte)Math.Max((byte)2, colorDepth);
            initDataSize = this.colorDepth;
        }

        internal void Encode(Stream os)
        {
            bool first = true;
            int prefix = NULLCODE;
            int suffix = NULLCODE;
            string entry = string.Format("{0},{1}", prefix, suffix);

            int ClearFlag = (1 << colorDepth);
            int EndFlag = ClearFlag + 1;

            Dictionary<string, int> CodeTable = new Dictionary<string, int>();

            int releaseCount = 0;

            byte codeSize = (byte)(colorDepth + 1);
            int availableCode = EndFlag + 1;
            int mask_Code = (1 << codeSize) - 1;

            BitEncoder bitEncoder = new BitEncoder(codeSize);

            os.WriteByte(colorDepth);
            bitEncoder.Add(ClearFlag);
            while (releaseCount < indexedPixel.Length)
            {
                if (first)
                {
                    suffix = indexedPixel[releaseCount++];
                    if (releaseCount == indexedPixel.Length)
                    {
                        bitEncoder.Add(suffix);
                        bitEncoder.Add(EndFlag);
                        bitEncoder.End();
                        os.WriteByte((byte)(bitEncoder.Length));
                        os.Write(bitEncoder.OutList.ToArray(), 0, bitEncoder.Length);
                        bitEncoder.OutList.Clear();
                        break;
                    }
                    first = false;
                    continue;
                }
                prefix = suffix;
                suffix = indexedPixel[releaseCount++];
                entry = string.Format("{0},{1}", prefix, suffix);

                if (!CodeTable.ContainsKey(entry))
                {
                    bitEncoder.Add(prefix);

                    CodeTable.Add(entry, availableCode++);

                    if (availableCode > (MaxStackSize - 3))
                    {
                        CodeTable.Clear();
                        colorDepth = initDataSize;
                        codeSize = (byte)(colorDepth + 1);
                        availableCode = EndFlag + 1;
                        mask_Code = (1 << codeSize) - 1;

                        bitEncoder.Add(ClearFlag);
                        bitEncoder.inBit = codeSize;
                    }
                    else if (availableCode > (1 << codeSize))
                    {
                        colorDepth++;
                        codeSize = (byte)(colorDepth + 1);
                        bitEncoder.inBit = codeSize;
                        mask_Code = (1 << codeSize) - 1;
                    }
                    if (bitEncoder.Length >= 255)
                    {
                        os.WriteByte((byte)255);
                        os.Write(bitEncoder.OutList.ToArray(), 0, 255);
                        if (bitEncoder.Length > 255)
                        {
                            byte[] left_buffer = new byte[bitEncoder.Length - 255];
                            bitEncoder.OutList.CopyTo(255, left_buffer, 0, left_buffer.Length);
                            bitEncoder.OutList.Clear();
                            bitEncoder.OutList.AddRange(left_buffer);
                        }
                        else
                        {
                            bitEncoder.OutList.Clear();
                        }
                    }
                }

                else
                {
                    suffix = (int)CodeTable[entry];
                }

                if (releaseCount == indexedPixel.Length)
                {
                    bitEncoder.Add(suffix);
                    bitEncoder.Add(EndFlag);
                    bitEncoder.End();
                    if (bitEncoder.Length > 255)
                    {
                        byte[] left_buffer = new byte[bitEncoder.Length - 255];
                        bitEncoder.OutList.CopyTo(255, left_buffer, 0, left_buffer.Length);
                        bitEncoder.OutList.Clear();
                        bitEncoder.OutList.AddRange(left_buffer);
                        os.WriteByte((byte)left_buffer.Length);
                        os.Write(left_buffer, 0, left_buffer.Length);
                    }
                    else
                    {
                        os.WriteByte((byte)(bitEncoder.Length));
                        os.Write(bitEncoder.OutList.ToArray(), 0, bitEncoder.Length);
                        bitEncoder.OutList.Clear();
                    }
                    break;
                }
            }
        }
    }
}
