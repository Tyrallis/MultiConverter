/************************************************************************
 *  Author: Callum Hutchinson
 *  Created: 18/02/2022
 *  
 *  Skel file handling
 * 
 * **********************************************************************/


using MultiConverter.Lib.Converters.Base;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace MultiConverter.Lib.Converters
{
    public class Skel
    {
        public SKL1 skl1;
        public SKS1 sks1;
        public SKA1 ska1;
        public SKB1 skb1;
        public SKPD skpd;
        public AFID afid;
        public BFID bfid;

        public Skel(ByteBuffer bb)
        {
            int offset = 0;
            int header = bb.readInt(ref offset);
            while (header != 0)
            {
                switch (header)
                {
                    case 0x314C4B53: //SKL1
                        skl1 = new SKL1(ref bb, ref offset);
                        break;
                    case 0x31534B53: //SKS1
                        sks1 = new SKS1(ref bb, ref offset);
                        break;
                    case 0x31414B53: //SKA1
                        ska1 = new SKA1(ref bb, ref offset);
                        break;
                    case 0x31424B53: //SKB1
                        skb1 = new SKB1(ref bb, ref offset);
                        break;
                    case 0x44504B53: //SKPD
                        skpd = new SKPD(ref bb, ref offset);
                        break;
                    case 0x44494641: //AFID
                        afid = new AFID(ref bb, ref offset);
                        break;
                    case 0x44494642: //BFID
                        bfid = new BFID(ref bb, ref offset);
                        break;
                }
                if (offset + 4 >= bb.buffer.Count)
                    break;
                header = bb.readInt(ref offset);
            }
        }


    }

    public class SKL1
    {
        uint flags;
        M2Array<M2Char> name;
        byte[] _0x0c = new byte[4];

        public SKL1(ref ByteBuffer bb, ref int offset)
        {
            int size = bb.readInt(ref offset);
            ByteBuffer tempBB = new ByteBuffer(bb.readBytes(size, ref offset));
            int newOffset = 0;
            flags = tempBB.readUInt(ref newOffset);
            name = new M2Array<M2Char>(ref tempBB, ref newOffset);
            for (int i = 0; i < 4; i++)
            {
                _0x0c[i] = tempBB.readByte(ref newOffset);
            }
        }
    }

    public class SKS1
    {
        public M2Array<M2Loop> global_loops;
        public M2Array<M2Sequence> sequences;
        public M2Array<M2UShort> sequence_lookups;
        public byte[] _0x18 = new byte[8];

        public SKS1(ref ByteBuffer bb, ref int offset)
        {
            int size = bb.readInt(ref offset) ;
            ByteBuffer tempBB = new ByteBuffer(bb.readBytes(size, ref offset));
            int newOffset = 0;
            global_loops = new M2Array<M2Loop>(ref tempBB, ref newOffset);
            sequences = new M2Array<M2Sequence>(ref tempBB, ref newOffset);
            sequence_lookups = new M2Array<M2UShort>(ref tempBB, ref newOffset);
            for (int i = 0; i < 8; i++)
            {
                _0x18[i] = tempBB.readByte(ref newOffset);
            }
        }
    }

    public class SKA1
    {
        public M2Array<M2Attachment> attachments;
        public M2Array<M2UShort> attachment_lookup_table;

        public SKA1(ref ByteBuffer bb, ref int offset)
        {
            int size = bb.readInt(ref offset);
            ByteBuffer tempBB = new ByteBuffer(bb.readBytes(size, ref offset));
            int newOffset = 0;
            attachments = new M2Array<M2Attachment>(ref tempBB, ref newOffset);
            attachment_lookup_table = new M2Array<M2UShort>(ref tempBB, ref newOffset);
        }
    }

    public class SKB1
    {

        public M2Array<M2CompBone> bones;
        public M2Array<M2UShort> key_bone_lookup;

        public SKB1(ref ByteBuffer bb, ref int offset)
        {
            int size = bb.readInt(ref offset);
            ByteBuffer tempBB = new ByteBuffer(bb.readBytes(size, ref offset));
            int newOffset = 0;
            bones = new M2Array<M2CompBone>(ref tempBB, ref newOffset);
            key_bone_lookup = new M2Array<M2UShort>(ref tempBB, ref newOffset);
        }
    }

    public class SKPD
    {
        byte[] _0x00 = new byte[8];
        uint parent_skel_file_id;
        byte[] _0x0c = new byte[4];

        public SKPD(ref ByteBuffer bb, ref int offset)
        {
            int size = bb.readInt(ref offset);
            ByteBuffer tempBB = new ByteBuffer(bb.readBytes(size, ref offset));
            int newOffset = 0;
            for (int i = 0; i < 8; i++)
            {
                _0x00[i] = tempBB.readByte(ref newOffset);
            }
            parent_skel_file_id = tempBB.readUInt(ref newOffset);
            for (int i = 0; i < 4; i++)
            {
                _0x0c[i] = tempBB.readByte(ref newOffset);
            }
        }
    }

    public class AFID
    {
        ushort anim_id;
        ushort sub_anim_id;
        uint file_id;

        public AFID(ref ByteBuffer bb, ref int offset)
        {
            int size = bb.readInt(ref offset);
            ByteBuffer tempBB = new ByteBuffer(bb.readBytes(size, ref offset));
            int newOffset = 0;
            anim_id = tempBB.readUShort(ref newOffset);
            sub_anim_id = tempBB.readUShort(ref newOffset);
            file_id = tempBB.readUInt(ref newOffset);
        }
    }

    public class BFID
    {
        public int size;
        public uint[] boneFileDataIDs;
        public int numberOfFiles;

        public BFID(ref ByteBuffer bb, ref int offset)
        {
            int size = bb.readInt(ref offset);
            ByteBuffer tempBB = new ByteBuffer(bb.readBytes(size, ref offset));
            int newOffset = 0;
            numberOfFiles = size / 4;
            boneFileDataIDs = new uint[numberOfFiles];
            for (int i = 0; i < numberOfFiles; i++)
            {
                boneFileDataIDs[i] = tempBB.readUInt(ref newOffset);
            }
        }
    }
}
