using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HQF.Tutorial.MMF
{
    /// <summary>
    ///  2byte head Length + header+ body
    /// </summary>
    //unsafe class RpcMMFResponsePacket
    //{
    //    private RpcResponse _response;
    //    private int _sequence;
    //    public int Sequence { get { return _sequence; } }
    //    public RpcResponse Response { get { return _response; } }

    //    public RpcMMFResponsePacket(byte[] data)
    //    {
    //        if (data.Length <= 2)
    //            throw new ArgumentException();
    //        short headLen;
    //        fixed (byte* p = data)
    //        {
    //            headLen = *(short*)p;
    //        }

    //        MemoryStream hms = new MemoryStream(data, 2, headLen);
    //        var header = ProtoBufSerializer.Deserialize<RpcMMFResponseHeader>(hms);
    //        int bodyLen = data.Length - 2 - headLen;

    //        RpcBodyBuffer body = null;
    //        if (bodyLen > 0)
    //        {
    //            MemoryStream bs = new MemoryStream(data, 2 + headLen, bodyLen, false);
    //            body = new RpcBodyBuffer(bs, bodyLen);
    //        }
    //        else
    //            body = RpcBodyBuffer.EmptyBody;

    //        _sequence = header.Sequence;
    //        _response = new RpcResponse((RpcErrorCode)header.ResponseCode, body);
    //    }

    //    public static void WriteResponse(RpcMMFMessageQueue queue, IRpcMMFSendingPacket packet)
    //    {
    //        RpcMMFResponseHeader header = packet.ResponseHeader;
    //        byte[] buffer = RpcMMFHelper.GetRpcPacket<RpcMMFResponseHeader>(header, packet.BodyBuffer);
    //        queue.Enqueue(buffer);
    //    }
    //}
}
