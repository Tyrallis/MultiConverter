/************************************************************************
 *  Author: Callum Hutchinson
 *  Created: 18/02/2022
 *  
 *  Custom structure for handling a series of bytes
 *  Tailored to allow easier use with M2Arrays when trying to add write the 
 *  data to a file
 *  
 *  M2Array needs the ability to reserve a specified size at the current 
 *  offset while allowing the objects deeper in the structure to continue appending. 
 *  As the data works back up the tree the headers get filled in at the specified locations
 *  
 *  insertAtPosition does most of the work here by increasing the size of the array if the it 
 *  is too small, it just appends a required number of filler bytes, then adding the provided data 
 *  in it's place
 * 
 * **********************************************************************/

using System;
using System.Collections.Generic;
using System.Text;

namespace MultiConverter.Lib.Converters.Base
{
    public class ByteBuffer
    {

        public List<byte> buffer = new List<byte>();
        public int currentIndex = 0;
        public int headerSize = 0;
        public int additional = 0;

        public ByteBuffer(byte[] bytes)
        {
            buffer.AddRange(bytes);
        }

        public ByteBuffer()
        {
        }

        public void addByte(byte b)
        {
            buffer.Add(b);
            currentIndex++;
        }

        public void insertAtPosition(byte[] temp, int offset)
        {
            if(buffer.Count < temp.Length + offset)
            {
                int difference = temp.Length + offset - buffer.Count ;
                for(int i =0; i < difference; i++)
                {
                    buffer.Add(0);
                }
            }

            for (int i = 0; i < temp.Length; i++)
            {
                buffer[offset + i] = temp[i];
            }
        }

        public void addByteWithoutIncrease(byte b)
        {
            buffer.Add(b);
        }

        public int addBytes(byte[] bytes)
        {
            int indexBefore = currentIndex; ;
            for (int i = 0; i < bytes.Length; i++)
            {
                addByte(bytes[i]);
            }
            return indexBefore;
        }

        public void addBytesWithoutIncrease(byte[] bytes)
        {
            for (int i = 0; i < bytes.Length; i++)
            {
                addByteWithoutIncrease(bytes[i]);
            }
        }

        public int addInt(int i)
        {
            int indexBefore = currentIndex;
            addBytes(BitConverter.GetBytes(i));
            return indexBefore;
        }

        public int addUInt(uint i)
        {
            int indexBefore = currentIndex;
            addBytes(BitConverter.GetBytes(i));
            return indexBefore;
        }

        public int addShort(short i)
        {
            int indexBefore = currentIndex;
            addBytes(BitConverter.GetBytes(i));
            return indexBefore;
        }

        public int addUShort(ushort i)
        {
            int indexBefore = currentIndex;
            addBytes(BitConverter.GetBytes(i));
            return indexBefore;
        }

        public int addFloat(float i)
        {
            int indexBefore = currentIndex;
            addBytes(BitConverter.GetBytes(i));
            return indexBefore;
        }

        public byte[] readBytes(int size, ref int offset)
        {
            byte[] temp = new byte[size];
            for (int i = 0; i < size; i++)
            {
                temp[i] = readByte(ref offset);
            }
            return temp;
        }

        public int readInt(ref int offset)
        {
            int index = offset == -1 ? currentIndex : offset;
            int result = BitConverter.ToInt32(buffer.GetRange(index, 4).ToArray());
            if (offset == -1)
            {
                currentIndex += 4;
            }
            else
            {
                offset += 4;
            }
            return result;
        }

        public int readInt()
        {
            int temp = -1;
            return readInt(ref temp);
        }

        public uint readUInt(ref int offset)
        {
            int index = offset == -1 ? currentIndex : offset;
            uint result = BitConverter.ToUInt32(buffer.GetRange(index, 4).ToArray());
            if (offset == -1)
            {
                currentIndex += 4;
            }
            else
            {
                offset += 4;
            }
            return result;
        }

        public uint readUInt()
        {
            int temp = -1;
            return readUInt(ref temp);
        }

        public byte readByte(ref int offset)
        {
            int index = offset == -1 ? currentIndex : offset;
            byte result = buffer[index];
            if (offset == -1)
            {
                currentIndex++;
            }
            else
            {
                offset++;
            }
            return result;
        }

        public byte readByte()
        {
            int temp = -1;
            return readByte(ref temp);
        }

        public short readShort(ref int offset)
        {
            int index = offset == -1 ? currentIndex : offset;
            short result = BitConverter.ToInt16(buffer.GetRange(index, 2).ToArray());
            if (offset == -1)
            {
                currentIndex += 2;
            }
            else
            {
                offset += 2;
            }
            return result;
        }

        public short readShort()
        {
            int temp = -1;
            return readShort(ref temp);
        }

        public ushort readUShort(ref int offset)
        {
            int index = offset == -1 ? currentIndex : offset;
            ushort result = BitConverter.ToUInt16(buffer.GetRange(index, 2).ToArray());
            if (offset == -1)
            {
                currentIndex += 2;
            }
            else
            {
                offset += 2;
            }
            return result;
        }

        public ushort readUShort()
        {
            int temp = -1;
            return readUShort(ref temp);
        }

        public float readFloat(ref int offset)
        {
            int index = offset == -1 ? currentIndex : offset;
            float result = BitConverter.ToSingle(buffer.GetRange(index, 4).ToArray());
            if (offset == -1)
            {
                currentIndex += 4;
            }
            else
            {
                offset += 4;
            }
            return result;
        }

        public float readFloat()
        {
            int temp = -1;
            return readFloat(ref temp);
        }
    }
}
