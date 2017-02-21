using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace gifresizer.utility
{
    internal class ImageDescriptor
    {
        internal short XOffSet;

        internal short YOffSet;

        internal short Width;

        internal short Height;

        internal byte Packed;

        internal bool LctFlag;

        internal bool InterlaceFlag;

        internal bool SortFlag;

        internal int LctSize;

        internal byte[] GetBuffer()
        {
            List<byte> list = new List<byte>();
            list.Add(GifExtensions.ImageDescriptorLabel);
            list.AddRange(BitConverter.GetBytes(XOffSet));
            list.AddRange(BitConverter.GetBytes(YOffSet));
            list.AddRange(BitConverter.GetBytes(Width));
            list.AddRange(BitConverter.GetBytes(Height));
            byte packed = 0;
            int m = 0;
            if (LctFlag)
            {
                m = 1;
            }
            int i = 0;
            if (InterlaceFlag)
            {
                i = 1;
            }
            int s = 0;
            if (SortFlag)
            {
                s = 1;
            }
            byte pixel = (byte)(Math.Log(LctSize, 2) - 1);
            packed = (byte)(pixel | (s << 5) | (i << 6) | (m << 7));
            list.Add(packed);
            return list.ToArray();
        }
    }
}
