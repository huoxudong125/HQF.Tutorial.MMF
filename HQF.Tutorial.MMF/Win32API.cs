using System;
using System.IO;
using System.Runtime.InteropServices;
using Microsoft.Win32.SafeHandles;

namespace HQF.Tutorial.MMF
{
    public sealed class Win32API
    {
        [Flags]
        public enum FileMapAccess : uint
        {
            FileMapCopy = 0x0001,
            FileMapWrite = 0x0002,
            FileMapRead = 0x0004,
            FileMapAllAccess = 0x001f,
            fileMapExecute = 0x0020
        }

        [Flags]
        public enum FileMapProtection : uint
        {
            PageReadonly = 0x02,
            PageReadWrite = 0x04,
            PageWriteCopy = 0x08,
            PageExecuteRead = 0x20,
            PageExecuteReadWrite = 0x40,
            SectionCommit = 0x8000000,
            SectionImage = 0x1000000,
            SectionNoCache = 0x10000000,
            SectionReserve = 0x4000000
        }

        [DllImport("Kernel32", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern IntPtr CreateFileMapping(IntPtr hFile, IntPtr lpAttributes, FileMapProtection flProtect,
            int dwMaxSizeHi, int dwMaxSizeLow, string lpName);

        public static IntPtr CreateFileMapping(FileStream File,
            FileMapProtection flProtect, long ddMaxSize, string lpName)
        {
            var Hi = (int) (ddMaxSize/int.MaxValue);
            var Lo = (int) (ddMaxSize%int.MaxValue);
            return CreateFileMapping(File.SafeFileHandle.DangerousGetHandle(), IntPtr.Zero, flProtect, Hi, Lo, lpName);
        }

        public static IntPtr CreateFileMapping(SafeFileHandle handle,
            FileMapProtection flProtect, long ddMaxSize, string lpName)
        {
            var Hi = (int) (ddMaxSize/int.MaxValue);
            var Lo = (int) (ddMaxSize%int.MaxValue);
            return CreateFileMapping(handle.DangerousGetHandle(), IntPtr.Zero, flProtect, Hi, Lo, lpName);
        }

        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern IntPtr OpenFileMapping(FileMapAccess DesiredAccess, bool bInheritHandle, string lpName);

        public static unsafe IntPtr Memcpy(byte* dest, byte* src, int len)
        {
            return memcpy(new IntPtr(dest), new IntPtr(src), len);
        }

        [DllImport("msvcrt.dll", SetLastError = false)]
        public static extern IntPtr memcpy(IntPtr dest, IntPtr src, int len);

        [DllImport("Kernel32", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern IntPtr MapViewOfFile(IntPtr hFileMapping, FileMapAccess dwDesiredAccess,
            int dwFileOffsetHigh, int dwFileOffsetLow, int dwNumberOfBytesToMap);

        public static IntPtr MapViewOfFile(IntPtr hFileMapping, FileMapAccess dwDesiredAccess, long ddFileOffset,
            int dwNumberOfBytesToMap)
        {
            var Hi = (int) (ddFileOffset/int.MaxValue);
            var Lo = (int) (ddFileOffset%int.MaxValue);
            return MapViewOfFile(hFileMapping, dwDesiredAccess, Hi, Lo, dwNumberOfBytesToMap);
        }

        [DllImport("kernel32.dll")]
        public static extern bool FlushViewOfFile(IntPtr lpBaseAddress,
            int dwNumberOfBytesToFlush);

        [DllImport("kernel32")]
        public static extern bool UnmapViewOfFile(IntPtr lpBaseAddress);

        [DllImport("kernel32", SetLastError = true)]
        public static extern bool CloseHandle(IntPtr hFile);

        [DllImport("kernel32.dll")]
        public static extern void GetSystemInfo([MarshalAs(UnmanagedType.Struct)] ref SYSTEM_INFO lpSystemInfo);

        [StructLayout(LayoutKind.Sequential)]
        public struct SYSTEM_INFO
        {
            public _PROCESSOR_INFO_UNION uProcessorInfo;
            public uint dwPageSize;
            public IntPtr lpMinimumApplicationAddress;
            public IntPtr lpMaximumApplicationAddress;
            public IntPtr dwActiveProcessorMask;
            public uint dwNumberOfProcessors;
            public uint dwProcessorType;
            public uint dwAllocationGranularity;
            public ushort dwProcessorLevel;
            public ushort dwProcessorRevision;
        }

        [StructLayout(LayoutKind.Explicit)]
        public struct _PROCESSOR_INFO_UNION
        {
            [FieldOffset(0)] public uint dwOemId;
            [FieldOffset(0)] public ushort wProcessorArchitecture;
            [FieldOffset(2)] public ushort wReserved;
        }
    }
}