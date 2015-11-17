using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace HQF.Tutorial.MMF
{
    class RpcMMFHelper
    {
        public static EventWaitHandle GetOrCreateWaitHandle(string name)
        {
            EventWaitHandle set = TryCreateWaitHandle(name);
            if (set == null)
                set = TryOpenWaitHandle(name);

            if (set == null)
                throw new Exception(string.Format("can't open or create eventWaitHandle:{0}", name));

            return set;
        }

        //public unsafe static byte[] GetRpcPacket<T>(T header, RpcBodyBuffer bodyBuffer)
        //{
        //    MemoryStream hms = ProtoBufSerializer.Serialize<T>(header);
        //    short headLen = (short)hms.Length;
        //    int bodyLen = bodyBuffer == null ? 0 : bodyBuffer.GetSize();
        //    int totalLen = 2 + headLen + bodyLen;

        //    byte[] buffer = new byte[totalLen];
        //    fixed (byte* p = buffer)
        //    {
        //        *(short*)p = headLen;
        //        hms.Read(buffer, 2, headLen);

        //        if (bodyLen > 0)
        //        {
        //            byte[] body = bodyBuffer.GetByteArray();
        //            fixed (byte* src = body)
        //            {
        //                Win32API.Memcpy(p + 2 + headLen, src, bodyLen);
        //            }
        //        }
        //    }
        //    return buffer;
        //}

        private static EventWaitHandle TryOpenWaitHandle(string name)
        {
            EventWaitHandle set = null;
            try
            {
                set = (EventWaitHandle)EventWaitHandle.OpenExisting(name);
            }
            catch (WaitHandleCannotBeOpenedException ex) { }

            return set;
        }

        private static EventWaitHandle TryCreateWaitHandle(string name)
        {
            EventWaitHandle set = null;
            try
            {
                set = new EventWaitHandle(false, EventResetMode.AutoReset, name);
            }
            catch (WaitHandleCannotBeOpenedException ex) { }

            return set;
        }
    }
}
