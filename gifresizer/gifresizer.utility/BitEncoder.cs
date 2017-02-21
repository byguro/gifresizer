using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace gifresizer.utility
{
    internal class BitEncoder
    {
        private int current_Bit = 0;

        internal List<Byte> OutList = new List<byte>();

        internal int Length
        {
            get
            {
                return OutList.Count;
            }
        }
        int current_Val;

        internal int inBit = 8;

        internal BitEncoder(int init_bit)
        {
            this.inBit = init_bit;
        }

        internal void Add(int inByte)
        {

            current_Val |= (inByte << (current_Bit));

            current_Bit += inBit;

            while (current_Bit >= 8)
            {
                byte out_Val = (byte)(current_Val & 0XFF);
                current_Val = current_Val >> 8;
                current_Bit -= 8;
                OutList.Add(out_Val);
            }
        }

        internal void End()
        {
            while (current_Bit > 0)
            {
                byte out_Val = (byte)(current_Val & 0XFF);
                current_Val = current_Val >> 8;
                current_Bit -= 8;
                OutList.Add((byte)out_Val);
            }
        }
    }
}
