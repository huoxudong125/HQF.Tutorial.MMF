using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace HQF.Tutorial.MMF
{
    public unsafe class LoopMemoryStream
    {
        private readonly int PREHEAD = sizeof (int*)*2;
        private readonly byte* _dataArea;
        private readonly byte* _start;
        private readonly byte* _end;
        private readonly int _totalLen;

        private int* _head; //save next available byte offset index
        private int* _tail; //save first data byte offset index

        public LoopMemoryStream(byte* dataArea, int length)
        {
            _dataArea = dataArea;
            _totalLen = length - PREHEAD;

            _head = (int*) _dataArea;
            _tail = (int*) _dataArea + 1;

            _start = dataArea + PREHEAD;
            _end = dataArea + length;
        }

        public int* Head
        {
            get { return _head; }
        }

        public int* Tail
        {
            get { return _tail; }
        }

        public bool IsEmpty
        {
            get { return *_head == *_tail; }
        }

        public int DataLen
        {
            get { return _totalLen - AvailLen; }
        }

        public int AvailLen
        {
            get
            {
                int diff = *_head - *_tail;
                return diff >= 0 ? _totalLen - diff : -diff;
            }
        }

        public void ClearData()
        {
            *_head = 0;
            *_tail = 0;
        }

        public void Write(byte[] data, int offSize, int length)
        {
            if (AvailLen < length + 4)
                throw new ArgumentException();

            WriteInt32(length);
            WriteBytes(data, offSize, length);
        }

        public byte[] Read()
        {
            if (DataLen < 4)
                throw new ArgumentException();

            int len = GetInt32();

            if (DataLen < len)
                throw new ArgumentException();

            return ReadBytes(len);
        }

        public byte[] ReadBytes(int length)
        {
            byte[] data = new byte[length];
            fixed (byte* pd = data)
            {
                if (*_tail > *_head && _totalLen - *_tail - 1 <= length)
                {
                    for (int i = 0; i < length; i++)
                    {
                        *(pd + i) = *(_start + *_tail);

                        if (*_tail == _totalLen - 1)
                            *_tail = 0;
                        else
                            (*_tail)++;
                    }
                }
                else
                {
                    MemCopy(_start + *_tail, pd, length);
                    *_tail += length;
                }
            }

            return data;
        }

        private void WriteBytes(byte[] data, int offSize, int length)
        {
            int end = offSize + length;
            fixed (byte* pd = data)
            {
                if (*_head >= *_tail && _totalLen - *_head - 1 <= length)
                {
                    for (int i = offSize; i < end; i++)
                    {
                        *(_start + *_head) = *(pd + i);
                        if (*_head == _totalLen - 1)
                            *_head = 0;
                        else
                            (*_head)++;
                    }
                }
                else
                {
                    MemCopy(pd + offSize, _start + *_head, length);
                    *_head += length;
                }
            }
        }

        private int GetInt32()
        {
            byte[] lenArr = ReadBytes(4);
            fixed (byte* p = lenArr)
            {
                return *(int*) p;
            }
        }

        private void WriteInt32(int value)
        {
            byte[] lenArr = new byte[4];
            fixed (byte* p = lenArr)
            {
                *(int*) p = value;
            }

            WriteBytes(lenArr, 0, 4);
        }


        [DllImport("msvcrt.dll", SetLastError = false)]
        private static extern IntPtr memcpy(IntPtr dest, IntPtr src, int len);

        /// <summary>
        /// 比MemCopy2 快1/3
        /// </summary>
        private static void MemCopy(byte* src, byte* dest, int len)
        {
            memcpy(new IntPtr(dest), new IntPtr(src), len);
        }

        /// <summary>
        /// 比循环Copy速度快10倍
        /// </summary>
        private static void MemCopy2(byte* src, byte* dest, int len)
        {
            if (len >= 16)
            {
                do
                {
                    *(long*) dest = *(long*) src;
                    *(long*) (dest + 8) = *(long*) (src + 8);
                    dest += 16;
                    src += 16;
                } while ((len -= 16) >= 16);
            }
            if (len > 0)
            {
                if ((len & 8) != 0)
                {
                    *(long*) dest = *(long*) src;
                    dest += 8;
                    src += 8;
                }
                if ((len & 4) != 0)
                {
                    *(int*) dest = *(int*) src;
                    dest += 4;
                    src += 4;
                }
                if ((len & 2) != 0)
                {
                    *(short*) dest = *(short*) src;
                    dest += 2;
                    src += 2;
                }
                if ((len & 1) != 0)
                {
                    byte* d = dest;
                    dest = d + 1;
                    byte* s = src;
                    src = s + 1;
                    *d = *s;
                }
            }
        }
    }
}
