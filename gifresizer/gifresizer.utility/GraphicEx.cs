using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace gifresizer.utility
{
    internal class GraphicEx : ExData
    {
        #region private fields
        byte _packed;
        short _delay;
        byte _tranIndex;
        bool _transFlag;
        int _disposalMethod;
        #endregion

        /// <summary>
        /// Block Size 
        /// </summary>
        internal static readonly byte BlockSize = 4;

        internal bool TransparencyFlag
        {
            get { return _transFlag; }
            set { _transFlag = value; }
        }

        internal int DisposalMethod
        {
            get { return _disposalMethod; }
            set { _disposalMethod = value; }
        }
        internal byte Packed
        {
            get
            {
                return _packed;
            }
            set
            {
                _packed = value;
            }
        }
        internal short Delay
        {
            get
            {
                return _delay;
            }
            set
            {
                _delay = value;
            }
        }
        /// <summary>
        /// Transparent Color Index
        /// </summary>
        internal byte TranIndex
        {
            get
            {
                return _tranIndex;
            }
            set
            {
                _tranIndex = value;
            }
        }
        internal GraphicEx()
        {
        }

        internal byte[] GetBuffer()
        {
            List<byte> list = new List<byte>();
            list.Add(GifExtensions.ExtensionIntroducer);
            list.Add(GifExtensions.GraphicControlLabel);
            list.Add(BlockSize);
            int t = 0;
            if (_transFlag)
            {
                t = 1;
            }
            _packed = (byte)((_disposalMethod << 2) | t);
            list.Add(_packed);
            list.AddRange(BitConverter.GetBytes(_delay));
            list.Add(_tranIndex);
            list.Add(GifExtensions.Terminator);
            return list.ToArray();
        }
    }
}
