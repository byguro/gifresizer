using System.Collections;
using System.Collections.Generic;

namespace gifresizer.utility
{
    internal class GifImage
    {
        internal short Width
        {
            get
            {
                return lcd.Width;
            }
        }

        internal short Height
        {
            get
            {
                return lcd.Height;
            }
        }

        string header = "";
        internal string Header
        {
            get { return header; }
            set { header = value; }
        }

        private byte[] gct;
        internal byte[] GlobalColorTable
        {
            get
            {
                return gct;
            }
            set
            {
                gct = value;
            }
        }

        internal Color32[] Palette
        {
            get
            {
                Color32[] act = PaletteHelper.GetColor32s(GlobalColorTable);
                act[lcd.BgColorIndex] = new Color32(0);
                return act;
            }
        }

        Hashtable table = new Hashtable();
        internal Hashtable GlobalColorIndexedTable
        {
            get { return table; }
        }

        List<CommentEx> comments = new List<CommentEx>();
        internal List<CommentEx> CommentExtensions
        {
            get
            {
                return comments;
            }
            set
            {
                comments = value;
            }
        }

        List<ApplicationEx> applictions = new List<ApplicationEx>();
        internal List<ApplicationEx> ApplictionExtensions
        {
            get
            {
                return applictions;
            }
            set
            {
                applictions = value;
            }
        }

        List<PlainTextEx> texts = new List<PlainTextEx>();
        internal List<PlainTextEx> PlainTextEntensions
        {
            get
            {
                return texts;
            }
            set
            {
                texts = value;
            }
        }

        LogicalScreenDescriptor lcd;
        internal LogicalScreenDescriptor LogicalScreenDescriptor
        {
            get
            {
                return lcd;
            }
            set
            {
                lcd = value;
            }
        }
        List<GifFrame> frames = new List<GifFrame>();
        internal List<GifFrame> Frames
        {
            get
            {
                return frames;
            }
            set
            {
                frames = value;
            }
        }
    }
}