﻿using System;
using System.IO;

namespace gifresizer.utility
{
    internal class LZWDecoder
    {
        protected static readonly int MaxStackSize = 4096;
        protected Stream stream;
        internal LZWDecoder(Stream stream)
        {
            this.stream = stream;
        }
        internal byte[] DecodeImageData(int width, int height, int dataSize)
        {
            int NullCode = -1;
            int pixelCount = width * height;
            byte[] pixels = new byte[pixelCount];
            int codeSize = dataSize + 1;
            int clearFlag = 1 << dataSize;
            int endFlag = clearFlag + 1;
            int available = endFlag + 1;

            int code = NullCode;
            int old_code = NullCode;
            int code_mask = (1 << codeSize) - 1;
            int bits = 0;


            int[] prefix = new int[MaxStackSize];
            int[] suffix = new int[MaxStackSize];
            int[] pixelStatck = new int[MaxStackSize + 1];

            int top = 0;
            int count = 0;
            int bi = 0;
            int i = 0;

            int data = 0;
            int first = 0;
            int inCode = NullCode;

            for (code = 0; code < clearFlag; code++)
            {
                prefix[code] = 0;
                suffix[code] = (byte)code;
            }

            byte[] buffer = null;
            while (i < pixelCount)
            {
                try
                {
                    //pixelCount = width * width
                    if (top == 0)
                    {
                        if (bits < codeSize)
                        {
                            if (count == 0)
                            {
                                buffer = ReadData();
                                count = buffer.Length;
                                if (count == 0)
                                {
                                    break;
                                }
                                bi = 0;
                            }
                            data += buffer[bi] << bits;
                            bits += 8;
                            bi++;
                            count--;
                            continue;
                        }
                        code = data & code_mask;
                        data >>= codeSize;
                        bits -= codeSize;

                        if (code > available || code == endFlag)
                        {
                            break;
                        }
                        if (code == clearFlag)
                        {
                            codeSize = dataSize + 1;
                            code_mask = (1 << codeSize) - 1;
                            available = clearFlag + 2;
                            old_code = NullCode;
                            continue;
                        }
                        if (old_code == NullCode)
                        {
                            pixelStatck[top++] = suffix[code];
                            old_code = code;
                            first = code;
                            continue;
                        }
                        inCode = code;
                        if (code == available)
                        {
                            pixelStatck[top++] = (byte)first;
                            code = old_code;
                        }
                        while (code > clearFlag)
                        {
                            pixelStatck[top++] = suffix[code];
                            code = prefix[code];
                        }
                        first = suffix[code];
                        if (available > MaxStackSize)
                        {
                            break;
                        }
                        pixelStatck[top++] = suffix[code];
                        if (available >= prefix.Length) available = prefix.Length-1;
                        if (available >= suffix.Length) available = suffix.Length-1;
                        prefix[available] = old_code;
                        suffix[available] = first;
                        available++;
                        if (available == code_mask + 1 && available < MaxStackSize)
                        {
                            codeSize++;
                            code_mask = (1 << codeSize) - 1;
                        }
                        old_code = inCode;
                    }
                    top--;
                    pixels[i++] = (byte)pixelStatck[top];
                }
                catch (Exception ex) {

                }
            }
            return pixels;
        }

        byte[] ReadData()
        {
            int blockSize = Read();
            return ReadByte(blockSize);
        }
        byte[] ReadByte(int len)
        {
            byte[] buffer = new byte[len];
            stream.Read(buffer, 0, len);
            return buffer;
        }
        int Read()
        {
            return stream.ReadByte();
        }
    }
}
