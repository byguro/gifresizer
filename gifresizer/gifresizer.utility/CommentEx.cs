﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace gifresizer.utility
{
    internal struct CommentEx
    {

        internal List<string> CommentDatas;
        internal byte[] GetBuffer()
        {
            List<byte> list = new List<byte>();
            list.Add(GifExtensions.ExtensionIntroducer);
            list.Add(GifExtensions.CommentLabel);
            foreach (string coment in CommentDatas)
            {
                char[] commentCharArray = coment.ToCharArray();
                list.Add((byte)commentCharArray.Length);
                foreach (char c in commentCharArray)
                {
                    list.Add((byte)c);
                }
            }
            list.Add(GifExtensions.Terminator);
            return list.ToArray();
        }
    }
}
