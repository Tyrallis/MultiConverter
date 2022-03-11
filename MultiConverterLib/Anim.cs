using MultiConverter.Lib.Converters;
using MultiConverter.Lib.Converters.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MultiConverterLib
{
    public class Anim
    {
        public AFM2 afm2;
        public SKA1 afsa;
        public SKB1 afsb;

        public Anim(ByteBuffer bb)
        {
            int offset = 0;
            int header = bb.readInt(ref offset);
            while (header != 0)
            {
                switch (header)
                {
                    case 0x324D4641: //AFM2
                        afm2 = new AFM2(ref bb, ref offset);
                        break;
                    case 0x41534641: //AFSA
                        afsa = new SKA1(ref bb, ref offset);
                        break;
                    case 0x42534641: //AFSB
                        afsb = new SKB1(ref bb, ref offset);
                        break;
                }
                if (offset + 4 >= bb.buffer.Count)
                    break;
                header = bb.readInt(ref offset);
            }
        }

        
    }

    public class AFM2
    {
        public AFM2(ref ByteBuffer bb, ref int offset)
        {
            int size = bb.readInt(ref offset);
            ByteBuffer tempBB = new ByteBuffer(bb.readBytes(size, ref offset));

        }
    }
}
