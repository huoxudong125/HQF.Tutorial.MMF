using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ProtoBuf;

namespace HQF.Tutorial.MMF
{
    [ProtoContract]
    public class RpcMMFRequestHeader
    {
        [ProtoMember(1, IsRequired = true)]
        public int Sequence;

        [ProtoMember(2)]
        public string ContextUri;

        [ProtoMember(3)]
        public string FromComputer;

        [ProtoMember(4)]
        public string FromService;

        [ProtoMember(5)]
        public string Service;

        [ProtoMember(6)]
        public string Method;

        public unsafe void Serialize(Stream stream)
        {
            byte[] b = new byte[4];
            fixed (byte* p = b)
                *(int*)p = Sequence;
            stream.Write(b, 0, 4);
            WriteStr(stream, ContextUri);
            WriteStr(stream, FromComputer);
            WriteStr(stream, FromService);
            WriteStr(stream, Service);
            WriteStr(stream, Method);
        }

        private static unsafe void WriteStr(Stream stream, string str)
        {

            byte len = 0;
            if (!string.IsNullOrEmpty(str))
                len = (byte)str.Length;

            if (len > 0)
            {
                byte[] data = UTF8Encoding.UTF8.GetBytes(str);
                if (data.Length > byte.MaxValue)
                    throw new NotSupportedException();

                len = (byte)data.Length;
                stream.WriteByte(len);
                stream.Write(data, 0, len);
            }
            else
                stream.WriteByte(0);
        }

        private static unsafe string ReadStr(Stream stream)
        {
            byte len = (byte)stream.ReadByte();
            if (len == 0)
                return null;
            byte[] data = new byte[len];
            stream.Read(data, 0, len);

            return UTF8Encoding.UTF8.GetString(data);
        }

        public MemoryStream Serialize()
        {
            MemoryStream ms = new MemoryStream();
            Serialize(ms);
            return ms;
        }

        public static unsafe RpcMMFRequestHeader Deserialize(Stream stream)
        {
            RpcMMFRequestHeader header = new RpcMMFRequestHeader();
            byte[] b = new byte[4];
            stream.Read(b, 0, 4);
            fixed (byte* p = b)
                header.Sequence = *(int*)p;

            header.ContextUri = ReadStr(stream);
            header.FromComputer = ReadStr(stream);
            header.FromService = ReadStr(stream);
            header.Service = ReadStr(stream);
            header.Method = ReadStr(stream);
            return header;
        }
    }
}
