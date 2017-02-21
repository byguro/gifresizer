using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace gifresizer.utility
{
    internal struct PlainTextEx
    {

        /// <summary>
        /// Block Size 
        /// </summary>
        internal static readonly byte BlockSize = 0X0C;

        /// <summary>
        /// Text Glid Left Posotion 
        /// </summary>
        internal short XOffSet;

        /// <summary>
        /// Text Glid Top Posotion 
        /// </summary>
        internal short YOffSet;

        /// <summary>
        /// Text Glid Width 
        /// </summary>
        internal short Width;

        /// <summary>
        /// Text Glid Height 
        /// </summary>
        internal short Height;

        /// <summary>
        /// Character Cell Width 
        /// </summary>
        internal byte CharacterCellWidth;

        /// <summary>
        /// Character Cell Height
        /// </summary>
        internal byte CharacterCellHeight;

        /// <summary>
        /// Text Foreground Color Index 
        /// </summary>
        internal byte ForegroundColorIndex;

        /// <summary>
        /// Text Blackground Color Index
        /// </summary>
        internal byte BgColorIndex;

        /// <summary>
        /// Plain Text Data
        /// </summary>
        internal List<string> TextDatas;


        internal byte[] GetBuffer()
        {
            List<byte> list = new List<byte>();
            list.Add(GifExtensions.ExtensionIntroducer);
            list.Add(GifExtensions.PlainTextLabel);
            list.Add(BlockSize);
            list.AddRange(BitConverter.GetBytes(XOffSet));
            list.AddRange(BitConverter.GetBytes(YOffSet));
            list.AddRange(BitConverter.GetBytes(Width));
            list.AddRange(BitConverter.GetBytes(Height));
            list.Add(CharacterCellWidth);
            list.Add(CharacterCellHeight);
            list.Add(ForegroundColorIndex);
            list.Add(BgColorIndex);
            if (TextDatas != null)
            {
                foreach (string text in TextDatas)
                {
                    list.Add((byte)text.Length);
                    foreach (char c in text)
                    {
                        list.Add((byte)c);
                    }
                }
            }
            list.Add(GifExtensions.Terminator);
            return list.ToArray();
        }
    }
}
