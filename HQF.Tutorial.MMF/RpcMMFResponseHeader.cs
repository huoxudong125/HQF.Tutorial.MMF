using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ProtoBuf;

namespace HQF.Tutorial.MMF
{
    [ProtoContract]
    public abstract class RpcMMFResponseHeader
    {
        [ProtoMember(1, IsRequired = true)]
        public int Sequence;

        [ProtoMember(2, IsRequired = true)]
        public int ResponseCode;

        [ProtoMember(3, IsRequired = true)]
        public int BodyLength;
    }
}
