using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace gifresizer.utility
{
    internal class GifFrame
    {
        #region private fields
        private ImageDescriptor _imgDes;
        private System.Drawing.Bitmap _img;
        private int _colorSize = 3;
        private byte[] _lct;
        private GraphicEx _graphicEx;
        private byte[] _buffer;
        #endregion

        #region internal property
        public Color32 BgColor
        {
            get
            {
                Color32[] act = PaletteHelper.GetColor32s(LocalColorTable);
                return act[GraphicExtension.TranIndex];
            }
        }
        internal ImageDescriptor ImageDescriptor
        {
            get { return _imgDes; }
            set { _imgDes = value; }
        }

        internal Color32[] Palette
        {
            get
            {
                Color32[] act = PaletteHelper.GetColor32s(LocalColorTable);
                if (GraphicExtension != null && GraphicExtension.TransparencyFlag)
                {
                    act[GraphicExtension.TranIndex] = new Color32(0);
                }
                return act;
            }
        }

        internal System.Drawing.Bitmap Image
        {
            get { return _img; }
            set { _img = value; }
        }
        internal int ColorDepth
        {
            get
            {
                return _colorSize;
            }
            set
            {
                _colorSize = value;
            }
        }

        /// <summary>
        /// (Local Color Table)
        /// RGBRGB......
        /// </summary>
        internal byte[] LocalColorTable
        {
            get { return _lct; }
            set { _lct = value; }
        }

        internal GraphicEx GraphicExtension
        {
            get { return _graphicEx; }
            set { _graphicEx = value; }
        }
        internal short Delay
        {
            get { return _graphicEx.Delay; }
            set { _graphicEx.Delay = value; }
        }

        internal byte[] IndexedPixel
        {
            get { return _buffer; }
            set { _buffer = value; }
        }
        #endregion
    }
}
