using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Win32.SafeHandles;

namespace HQF.Tutorial.MMF
{
    public unsafe class MMFMessageQueue
    {
        private volatile void* _shm;
        private volatile int* _lck;
        private volatile int* _head;
        private volatile int* _tail;

        private LoopMemoryStream _ms;
        private int _size;
        private int _realSize;
        private string _mmfName;

        public enum QueueResult
        {
            EMPTY,
            SUCCESS,
            FULL,
        }

        public int Count { get; set; }

        public MMFMessageQueue(string mmfName, int size)
        {
            _mmfName = mmfName;
            _size = size;
            _realSize = _size - sizeof(int*) * 3;

            _shm = GetOrCreateMMFView();
            _lck = (int*)_shm;
            _ms = new LoopMemoryStream((byte*)_shm + sizeof(int*), _size - sizeof(int*));
            //_ms.ClearData();//打开的同时清理

            _head = _ms.Head;
            _tail = _ms.Tail;
        }

        private void* GetOrCreateMMFView()
        {
            IntPtr mmf = Win32API.OpenFileMapping(Win32API.FileMapAccess.FileMapAllAccess, false, _mmfName);
            if (mmf == IntPtr.Zero)
            {
                mmf = Win32API.CreateFileMapping(new SafeFileHandle(new IntPtr(-1), true), Win32API.FileMapProtection.PageReadWrite, _size, _mmfName);
                if (mmf == IntPtr.Zero)
                    throw new Win32Exception();
            }

            IntPtr mvf = Win32API.MapViewOfFile(mmf, Win32API.FileMapAccess.FileMapWrite | Win32API.FileMapAccess.FileMapRead, 0, _size);
            if (mvf == IntPtr.Zero)
                throw new Win32Exception();

            return mvf.ToPointer();
        }

        //SpinWait 每20次会有一次系统时间片切换
        //清理数据（挂的时候数据一致性是问题，全部删掉）
        //然后相当于获取锁往下执行
        //测试发现Count=10w时，wait时间为5s
        private void TryEnterLock()
        {
            SpinWait wait = new SpinWait();
            int head = *_head;
            int tail = *_tail;
            int count = 0;

            while (Interlocked.CompareExchange(ref *_lck, 1, 0) != 0)
            {
                wait.SpinOnce();

                count++;
                if (head != *_head || tail != *_tail)
                {
                    head = *_head;
                    tail = *_tail;
                    count = 0;
                }

                if (count > 100000)
                {
                    Console.WriteLine("ClearData");
                    _ms.ClearData();
                    break;
                }
            }
        }

        private void ExitLock()
        {
            *_lck = 0;
        }
        public QueueResult TryAppend(byte[] data)
        {
            return TryAppend(data, 0, data.Length);
        }

        public QueueResult TryAppend(byte[] data, int offsize, int length)
        {
            int realsize = 4 + length;
            if (realsize > _realSize)
                throw new OverflowException();

            TryEnterLock();

            if (_ms.AvailLen < realsize)
            {
                ExitLock();
                return QueueResult.FULL;
            }
            else
            {
                _ms.Write(data, 0, length);
                ExitLock();
                return QueueResult.SUCCESS;
            }
        }

        public QueueResult TryDequeue(out byte[] result)
        {
            result = null;

            TryEnterLock();

            if (_ms.IsEmpty)
            {
                ExitLock();
                return QueueResult.EMPTY;
            }
            else
            {
                result = _ms.Read();
                ExitLock();
                return QueueResult.SUCCESS;
            }
        }
    }
}
