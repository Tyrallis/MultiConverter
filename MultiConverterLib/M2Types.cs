/************************************************************************
 *  Author: Callum Hutchinson
 *  Created: 18/02/2022
 *  
 *  Some of the types used in the WoW/M2 file format
 * 
 * **********************************************************************/


using MultiConverter.Lib.Converters.Base;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace MultiConverter.Lib.Converters
{
    public class M2Array<T> : IM2Array
    {
        public uint size;
        public uint ofs;
        public T[] values;

        public M2Array() { }

        public M2Array(ref ByteBuffer bb, ref int offset)
        {
            Init(ref bb, ref offset);
        }

        public void Init(ref ByteBuffer bb, ref int offset)
        {
            size = bb.readUInt(ref offset);
            values = new T[size];
            ofs = bb.readUInt(ref offset);
            int currentOffset = (int)ofs;
            for(int i =0; i < size; i++)
            {
                T a = Activator.CreateInstance<T>();
                ((IM2Array)a).Init(ref bb, ref currentOffset);
                values[i] = a;
            }
        }

        public int GetSize()
        {
            return (int)(sizeof(uint) + sizeof(uint));
        }

        public byte[] ToBytes(ref ByteBuffer data)
        {
            ByteBuffer result = new ByteBuffer();
            if(size != 0)
            {
                int dataOffset = data.currentIndex;
                int localOffset = dataOffset;
                int valueSize = ((IM2Array)values[0]).GetSize();
                int sizeOfData = (int)(valueSize * size);
                data.currentIndex += sizeOfData;
                for(int i =0; i < size; i++)
                {
                    data.insertAtPosition(((IM2Array)values[i]).ToBytes(ref data), localOffset);
                    localOffset += valueSize;
                }
                result.addUInt(size);
                result.addInt(dataOffset + data.additional);
            }
            else
            {
                result.addInt(0x00000000);
                result.addInt(0x00000000);
            }
            return result.buffer.ToArray();
        }
    }

    public class M2TrackBase
    {
        public ushort interpolation_type;
        public ushort global_sequence;
        public M2Array<M2Array<M2UInt32>> timestamps;

        public M2TrackBase() { }

        public M2TrackBase(ref ByteBuffer bb, ref int offset)
        {
            interpolation_type = bb.readUShort(ref offset);
            global_sequence = bb.readUShort(ref offset);
            timestamps = new M2Array<M2Array<M2UInt32>>(ref bb, ref offset);
        }

        public int GetSize()
        {
            return sizeof(ushort) + sizeof(ushort) + timestamps.GetSize();
        }

        public byte[] ToBytes(ref ByteBuffer data)
        {
            ByteBuffer result = new ByteBuffer();
            result.addUShort(interpolation_type);
            result.addUShort(global_sequence);
            result.addBytes(timestamps.ToBytes(ref data));
            return result.buffer.ToArray();
        }
    }

    public class M2Track<T> : M2TrackBase
    {
        public M2Array<M2Array<T>> values;

        public M2Track(ref ByteBuffer bb, ref int offset) : base(ref bb, ref offset)
        {
            values = new M2Array<M2Array<T>>(ref bb, ref offset);
        }

        public new int GetSize()
        {
            return base.GetSize() + values.GetSize();
        }

        public new byte[] ToBytes(ref ByteBuffer data)
        {
            ByteBuffer result = new ByteBuffer();
            result.addBytes(base.ToBytes(ref data));
            result.addBytes(values.ToBytes(ref data));
            return result.buffer.ToArray();
        }
    }

    public class M2UInt32 : IM2Array
    {
        public uint value;

        public M2UInt32() { }

        public M2UInt32(ref ByteBuffer bb, ref int offset)
        {
            Init(ref bb, ref offset);
        }

        public void Init(ref ByteBuffer bb, ref int offset)
        {
            value = bb.readUInt(ref offset);
        }

        public int GetSize()
        {
            return sizeof(uint);
        }
        public byte[] ToBytes(ref ByteBuffer data)
        {
            return BitConverter.GetBytes(value);
        }
    }

    public class CArgb : IM2Array
    {
        public byte A;
        public byte R;
        public byte G;
        public byte B;

        public CArgb() { }

        public CArgb(ref ByteBuffer bb, ref int offset)
        {
            Init(ref bb, ref offset);
        }

        public void Init(ref ByteBuffer bb, ref int offset)
        {
            A = bb.readByte(ref offset);
            R = bb.readByte(ref offset);
            G = bb.readByte(ref offset);
            B = bb.readByte(ref offset);
        }

        public int GetSize()
        {
            return sizeof(byte) * 4;
        }

        public byte[] ToBytes(ref ByteBuffer data)
        {
            ByteBuffer result = new ByteBuffer();
            result.addByte(A);
            result.addByte(R);
            result.addByte(G);
            result.addByte(B);
            return result.buffer.ToArray();
        }
    }

    public class C4Plane : IM2Array
    {
        public C3Vector Normal;
        public float Distance;

        public C4Plane() { }
        public C4Plane(ref ByteBuffer bb, ref int offset)
        {
            Init(ref bb, ref offset);
        }

        public void Init(ref ByteBuffer bb, ref int offset)
        {
            Normal = new C3Vector(ref bb, ref offset);
            Distance = bb.readFloat(ref offset);
        }

        public int GetSize()
        {
            return Normal.GetSize() + sizeof(float);
        }

        public byte[] ToBytes(ref ByteBuffer data)
        {
            ByteBuffer result = new ByteBuffer();
            result.addBytes(Normal.ToBytes(ref data));
            result.addFloat(Distance);
            return result.buffer.ToArray();
        }
    }

    public class CAaBox : IM2Array
    {
        public C3Vector Min;
        public C3Vector Max;

        public CAaBox() { }
        public CAaBox(ref ByteBuffer bb, ref int offset)
        {
            Init(ref bb, ref offset);
        }

        public void Init(ref ByteBuffer bb, ref int offset)
        {
            Min = new C3Vector(ref bb, ref offset);
            Max = new C3Vector(ref bb, ref offset);
        }

        public int GetSize()
        {
            return Min.GetSize() + Max.GetSize();
        }

        public byte[] ToBytes(ref ByteBuffer data)
        {
            ByteBuffer result = new ByteBuffer();
            result.addBytes(Min.ToBytes(ref data));
            result.addBytes(Max.ToBytes(ref data));
            return result.buffer.ToArray();
        }
    }

    public class C3Vector : IM2Array
    {
        public float X;
        public float Y;
        public float Z;

        public C3Vector() { }
        public C3Vector(ref ByteBuffer bb, ref int offset)
        {
            Init(ref bb, ref offset);
        }

        public void Init(ref ByteBuffer bb, ref int offset)
        {
            X = bb.readFloat(ref offset);
            Y = bb.readFloat(ref offset);
            Z = bb.readFloat(ref offset);
        }

        public int GetSize()
        {
            return sizeof(float) * 3;
        }

        public byte[] ToBytes(ref ByteBuffer data)
        {
            ByteBuffer result = new ByteBuffer();
            result.addFloat(X);
            result.addFloat(Y);
            result.addFloat(Z);
            return result.buffer.ToArray();
        }
    }

    public class M2CompQuat : IM2Array
    {
        public short x, y, z, w;
        public M2CompQuat() { }
        public M2CompQuat(ref ByteBuffer bb, ref int offset)
        {
            Init(ref bb, ref offset);
        }

        public void Init(ref ByteBuffer bb, ref int offset)
        {
            x = bb.readShort(ref offset);
            y = bb.readShort(ref offset);
            z = bb.readShort(ref offset);
            w = bb.readShort(ref offset);
        }
        public int GetSize()
        {
            return sizeof(short) * 4;
        }

        public byte[] ToBytes(ref ByteBuffer data)
        {
            ByteBuffer result = new ByteBuffer();
            result.addShort(x);
            result.addShort(y);
            result.addShort(z);
            result.addShort(w);
            return result.buffer.ToArray();
        }
    }

    public class M2Range : IM2Array
    {
        public uint minimum;
        public uint maximum;

        public M2Range() { }
        public M2Range(ref ByteBuffer bb, ref int offset)
        {
            Init(ref bb, ref offset);
        }

        public void Init(ref ByteBuffer bb, ref int offset)
        {
            minimum = bb.readUInt(ref offset);
            maximum = bb.readUInt(ref offset);
        }

        public int GetSize()
        {
            return sizeof(uint) + sizeof(uint);
        }
        public byte[] ToBytes(ref ByteBuffer data)
        {
            ByteBuffer result = new ByteBuffer();
            result.addUInt(minimum);
            result.addUInt(maximum);
            return result.buffer.ToArray();
        }

    }

    public class M2Bounds : IM2Array
    {
        public CAaBox extent;
        public float radius;

        public M2Bounds() { }
        public M2Bounds(ref ByteBuffer bb, ref int offset)
        {
            Init(ref bb, ref offset);
        }

        public void Init(ref ByteBuffer bb, ref int offset)
        {
            extent = new CAaBox(ref bb, ref offset);
            radius = bb.readFloat(ref offset);
        }

        public int GetSize()
        {
            return extent.GetSize() + sizeof(float);
        }

        public byte[] ToBytes(ref ByteBuffer data)
        {
            ByteBuffer result = new ByteBuffer();
            result.addBytes(extent.ToBytes(ref data));
            result.addFloat(radius);
            return result.buffer.ToArray();
        }
    }

    public class M2Byte : IM2Array
    {
        public byte value;

        public M2Byte() { }
        public M2Byte(ref ByteBuffer bb, ref int offset)
        {
            Init(ref bb, ref offset);
        }

        public void Init(ref ByteBuffer bb, ref int offset)
        {
            value = bb.readByte(ref offset);
        }

        public int GetSize()
        {
            return sizeof(byte);
        }
        public byte[] ToBytes(ref ByteBuffer data)
        {
            return BitConverter.GetBytes(value);
        }
    }

    public class M2Char : IM2Array
    {
        public char value;

        public M2Char() { }
        public M2Char(ref ByteBuffer bb, ref int offset)
        {
            Init(ref bb, ref offset);
        }

        public void Init(ref ByteBuffer bb, ref int offset)
        {
            value = (char)bb.readByte(ref offset);
        }

        public int GetSize()
        {
            return sizeof(char);
        }

        public byte[] ToBytes(ref ByteBuffer data)
        {
            return BitConverter.GetBytes(value);
        }
    }

    public class M2Loop : IM2Array
    {
        public uint timestamp;

        public M2Loop() { }
        public M2Loop(ref ByteBuffer bb, ref int offset)
        {
            Init(ref bb, ref offset);
        }

        public void Init(ref ByteBuffer bb, ref int offset)
        {
            timestamp = bb.readUInt(ref offset);
        }

        public int GetSize()
        {
            return sizeof(uint);
        }

        public byte[] ToBytes(ref ByteBuffer data)
        {
            return BitConverter.GetBytes(timestamp);
        }
    }

    public class M2Sequence : IM2Array
    {
        ushort id;
        ushort variationIndex;
        uint duration;
        float moveSpeed;
        uint flags;
        short frequency;
        ushort padding;
        M2Range replay;
        uint blendTime; //For Wotlk
        ushort blendTimeIn; //For Above
        ushort blendTimeOut; //^^
        M2Bounds bounds;
        short variationNext;
        ushort aliasNext;

        public M2Sequence() { }
        public M2Sequence(ref ByteBuffer bb, ref int offset)
        {
            Init(ref bb, ref offset);
        }

        public void Init(ref ByteBuffer bb, ref int offset)
        {
            id = bb.readUShort(ref offset);
            variationIndex = bb.readUShort(ref offset);
            duration = bb.readUInt(ref offset);
            moveSpeed = bb.readFloat(ref offset);
            flags = bb.readUInt(ref offset);
            frequency = bb.readShort(ref offset);
            padding = bb.readUShort(ref offset);
            replay = new M2Range(ref bb, ref offset);

            //if file == wotlk
            //blendTime = br.ReadUInt32();
            //else
            blendTimeIn = bb.readUShort(ref offset);
            blendTimeOut = bb.readUShort(ref offset);

            bounds = new M2Bounds(ref bb, ref offset);
            variationNext = bb.readShort(ref offset);
            aliasNext = bb.readUShort(ref offset);
        }

        public int GetSize()
        {
            return sizeof(ushort) + sizeof(ushort) + sizeof(uint) + sizeof(float) +
                sizeof(uint) + sizeof(short) + sizeof(ushort) + replay.GetSize() + sizeof(uint) +
                bounds.GetSize() + sizeof(short) + sizeof(ushort);
        }

        public byte[] ToBytes(ref ByteBuffer data)
        {
            ByteBuffer result = new ByteBuffer();
            result.addUShort(id);
            result.addUShort(variationIndex);
            result.addUInt(duration);
            result.addFloat(moveSpeed);
            result.addUInt(flags);
            result.addShort(frequency);
            result.addUShort(padding);
            result.addBytes(replay.ToBytes(ref data));
            result.addUInt(0x0);
            result.addBytes(bounds.ToBytes(ref data));
            result.addShort(variationNext);
            result.addUShort(aliasNext);
            return result.buffer.ToArray();
        }
    }

    public class M2UShort : IM2Array
    {
        public ushort value;

        public M2UShort() { }
        public M2UShort(ref ByteBuffer bb, ref int offset)
        {
            Init(ref bb, ref offset);
        }

        public void Init(ref ByteBuffer bb, ref int offset)
        {
            value = bb.readUShort(ref offset);
        }

        public int GetSize()
        {
            return sizeof(ushort);
        }

        public byte[] ToBytes(ref ByteBuffer data)
        {
            return BitConverter.GetBytes(value);
        }
    }

    public class M2CompBone : IM2Array
    {
        int key_bone_id;
        int flags;
        short parent_bone;
        ushort submesh_id;
        ushort uDistToFurthDesc;
        ushort uZRatioOfChain;
        M2Track<C3Vector> translation;
        M2Track<M2CompQuat> rotation;
        M2Track<C3Vector> scale;
        C3Vector pivot;

        public M2CompBone() { }
        public M2CompBone(ref ByteBuffer bb, ref int offset)
        {
            Init(ref bb, ref offset);
        }

        public void Init(ref ByteBuffer bb, ref int offset)
        {
            key_bone_id = bb.readInt(ref offset);
            flags = bb.readInt(ref offset);
            parent_bone = bb.readShort(ref offset);
            submesh_id = bb.readUShort(ref offset);
            uDistToFurthDesc = bb.readUShort(ref offset);
            uZRatioOfChain = bb.readUShort(ref offset);
            translation = new M2Track<C3Vector>(ref bb, ref offset);
            rotation = new M2Track<M2CompQuat>(ref bb, ref offset);
            scale = new M2Track<C3Vector>(ref bb, ref offset);
            pivot = new C3Vector(ref bb, ref offset);
        }

        public int GetSize()
        {
            return sizeof(int) + sizeof(int) + sizeof(short) + sizeof(ushort) + sizeof(ushort) + sizeof(ushort)
                + translation.GetSize() + rotation.GetSize() + scale.GetSize() + pivot.GetSize();
        }

        public byte[] ToBytes(ref ByteBuffer data)
        {
            ByteBuffer result = new ByteBuffer();
            result.addInt(key_bone_id);
            result.addInt(flags);
            result.addShort(parent_bone);
            result.addUShort(submesh_id);
            result.addUShort(uDistToFurthDesc);
            result.addUShort(uZRatioOfChain);
            result.addBytes(translation.ToBytes(ref data));
            result.addBytes(rotation.ToBytes(ref data));
            result.addBytes(scale.ToBytes(ref data));
            result.addBytes(pivot.ToBytes(ref data));
            return result.buffer.ToArray();
        }
    }

    public class M2Attachment : IM2Array
    {
        public uint id;
        ushort bone;
        ushort unknown;
        C3Vector position;
        M2Track<M2Byte> animated_attached;

        public M2Attachment() { }

        public M2Attachment(ref ByteBuffer bb, ref int offset)
        {
            Init(ref bb, ref offset);
        }

        public void Init(ref ByteBuffer bb, ref int offset)
        {
            id = bb.readUInt(ref offset);
            bone = bb.readUShort(ref offset);
            unknown = bb.readUShort(ref offset);
            position = new C3Vector(ref bb, ref offset);
            animated_attached = new M2Track<M2Byte>(ref bb, ref offset);
        }

        public int GetSize()
        {
            return sizeof(uint) + sizeof(ushort) + sizeof(ushort) + position.GetSize() + animated_attached.GetSize();
        }

        public byte[] ToBytes(ref ByteBuffer data)
        {
            ByteBuffer result = new ByteBuffer();
            result.addUInt(id);
            result.addUShort(bone);
            result.addUShort(unknown);
            result.addBytes(position.ToBytes(ref data));
            result.addBytes(animated_attached.ToBytes(ref data));
            return result.buffer.ToArray();
        }
    }

    public interface IM2Array
    {
        public void Init(ref ByteBuffer bb, ref int offset);

        public int GetSize();

        public byte[] ToBytes(ref ByteBuffer data);
    }
}
