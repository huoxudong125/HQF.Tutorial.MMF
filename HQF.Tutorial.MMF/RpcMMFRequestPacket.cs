using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HQF.Tutorial.MMF
{
    ///// <summary>
    ///// 2byte head Length + header+ body
    ///// </summary>
    //unsafe class RpcMMFRequestPacket
    //{
    //    private RpcRequest _request;
    //    private int _sequence;

    //    public RpcRequest Request { get { return _request; } }
    //    public int Sequence { get { return _sequence; } }

    //    public RpcMMFRequestPacket(byte[] data)
    //    {
    //        if (data.Length <= 2)
    //            throw new ArgumentException();
    //        short headLen;
    //        fixed (byte* p = data)
    //        {
    //            headLen = *(short*)p;
    //        }

    //        MemoryStream hms = new MemoryStream(data, 2, headLen);
    //        var header = ProtoBufSerializer.Deserialize<RpcMMFRequestHeader>(hms);

    //        var request = new RpcRequest();
    //        request.ContextUri = header.ContextUri;
    //        request.FromComputer = header.FromComputer;
    //        request.FromService = header.FromService;
    //        request.Method = header.Method;
    //        request.Service = header.Service;
    //        _sequence = header.Sequence;

    //        int bodyLen = data.Length - 2 - headLen;
    //        if (bodyLen > 0)
    //        {
    //            MemoryStream bms = new MemoryStream(data, 2 + headLen, bodyLen);
    //            request.SetBodyStream(bms, bodyLen);
    //        }
    //        else
    //            request.BodyBuffer = RpcBodyBuffer.EmptyBody;

    //        _request = request;
    //    }

    //    public static void WriteRequest(RpcMMFMessageQueue queue, IRpcMMFSendingPacket packet)
    //    {
    //        RpcMMFRequestHeader header = packet.RequestHeader;
    //        byte[] buffer = RpcMMFHelper.GetRpcPacket<RpcMMFRequestHeader>(header, packet.BodyBuffer);
    //        queue.Enqueue(buffer);
    //    }
    //}
}
