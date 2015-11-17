using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace HQF.Tutorial.MMF
{
    /// <summary>
    /// 每个通道两个MMF队列，一个发送，一个接收
    /// </summary>
    class RpcMMFMessageQueue
    {
        private const int NotityTimes = 1;
        private MMFMessageQueue _repQueue;
        private MMFMessageQueue _rspQueue;
        private Thread _thread;
        private EventWaitHandle _reqWait;
        private EventWaitHandle _rspWait;
        private RpcMMFMode _mode;
        private int _count;

        public Action<RpcMMFMessageQueue, byte[]> ReceiveData;

        public RpcMMFMessageQueue(string mmfName, RpcMMFMode mode)
        {
            _mode = mode;
            string reqName = string.Format("rpc_mmf_{0}_req", mmfName);
            string rspName = string.Format("rpc_mmf_{0}q_rsp", mmfName);

            _repQueue = new MMFMessageQueue(reqName, RpcMMFConfiguration.Current.MMFSize);
            _rspQueue = new MMFMessageQueue(rspName, RpcMMFConfiguration.Current.MMFSize);

            _reqWait = RpcMMFHelper.GetOrCreateWaitHandle(reqName + "_wait");
            _rspWait = RpcMMFHelper.GetOrCreateWaitHandle(rspName + "_wait");

            _thread = new Thread(DequeueProc);
            _thread.IsBackground = true;
            _thread.Start();
        }

        public void Enqueue(byte[] data)
        {
            Enqueue(data, 0, data.Length);
        }

        public void Enqueue(byte[] data, int offsize, int length)
        {
            try
            {
                var queue = _mode == RpcMMFMode.RpcClient ? _repQueue : _rspQueue;
                if (queue.TryAppend(data, offsize, length) == MMFMessageQueue.QueueResult.FULL)
                {
                    throw new RpcMMFException("MMF Queue Full");
                }
            }
            catch (OverflowException)
            {
                throw new RpcMMFException("MMF Queue Full");
            }

            if (Interlocked.Increment(ref _count) == NotityTimes)
            {
                _count = 0;
                if (_mode == RpcMMFMode.RpcClient)
                    _reqWait.Set();
                else
                    _rspWait.Set();
            }
        }

        private void DequeueProc()
        {
            while (true)
            {
                byte[] data;
                var queue = _mode == RpcMMFMode.RpcServer ? _repQueue : _rspQueue;

                if (queue.TryDequeue(out data) == MMFMessageQueue.QueueResult.EMPTY)
                {
                    if (_mode == RpcMMFMode.RpcServer)
                        _reqWait.WaitOne(1);
                    else
                        _rspWait.WaitOne(1);
                }
                else
                {
                    if (ReceiveData != null)
                        ReceiveData(this, data);
                }
            }
        }
    }
}
