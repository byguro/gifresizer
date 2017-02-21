using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace gifresizer.utility
{
    internal class LogicalScreenDescriptor
    {
        private short _width;
        internal short Width
        {
            get { return _width; }
            set { _width = value; }
        }

        private short _height;

        internal short Height
        {
            get { return _height; }
            set { _height = value; }
        }


        private byte _packed;

        internal byte Packed
        {
            get { return _packed; }
            set { _packed = value; }
        }

        private byte _bgIndex;
        internal byte BgColorIndex
        {
            get { return _bgIndex; }
            set { _bgIndex = value; }
        }


        private byte _pixelAspect;
        internal byte PixcelAspect
        {
            get { return _pixelAspect; }
            set { _pixelAspect = value; }
        }
        private bool _globalColorTableFlag;
        internal bool GlobalColorTableFlag
        {
            get { return _globalColorTableFlag; }
            set { _globalColorTableFlag = value; }
        }

        private byte _colorResoluTion;
        internal byte ColorResoluTion
        {
            get { return _colorResoluTion; }
            set { _colorResoluTion = value; }
        }

        private int _sortFlag;

        internal int SortFlag
        {
            get { return _sortFlag; }
            set { _sortFlag = value; }
        }

        private int _globalColorTableSize;
        internal int GlobalColorTableSize
        {
            get { return _globalColorTableSize; }
            set { _globalColorTableSize = value; }
        }

        internal byte[] GetBuffer()
        {
            byte[] buffer = new byte[7];
            Array.Copy(BitConverter.GetBytes(_width), 0, buffer, 0, 2);
            Array.Copy(BitConverter.GetBytes(_height), 0, buffer, 2, 2);
            int m = 0;
            if (_globalColorTableFlag)
            {
                m = 1;
            }
            byte pixel = (byte)(Math.Log(_globalColorTableSize, 2) - 1);
            _packed = (byte)(pixel | (_sortFlag << 4) | (_colorResoluTion << 5) | (m << 7));
            buffer[4] = _packed;
            buffer[5] = _bgIndex;
            buffer[6] = _pixelAspect;
            return buffer;
        }
    }
}
