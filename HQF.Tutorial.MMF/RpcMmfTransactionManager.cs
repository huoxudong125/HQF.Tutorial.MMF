using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace HQF.Tutorial.MMF
{
    //static class RpcMmfTransactionManager
    //{
    //    private static Thread _thread;
    //    private static ConcurrentDictionary<int, RpcMMFClientTransaction> _dict;
    //    private static Stopwatch _watch;
    //    private static object _sync = new object();
    //    public static void Initialize()
    //    {
    //        if (_dict == null)
    //            lock (_sync)
    //            {
    //                _watch = new Stopwatch();
    //                _watch.Start();

    //                _dict = new ConcurrentDictionary<int, RpcMMFClientTransaction>(16, 64 * 1024);
    //                _thread = new Thread(MonitorProc);
    //                _thread.IsBackground = true;
    //                _thread.Start();
    //            }
    //    }

    //    public static void BeginTransaction(RpcMMFClientTransaction tx)
    //    {
    //        tx.Tickets = _watch.ElapsedMilliseconds;

    //        if (!_dict.TryAdd(tx.Sequence, tx))
    //        {
    //            tx.SendFailed(RpcErrorCode.SendFailed, null);
    //            _tracing.ErrorFmt("sequence key same,{0}", ObjectHelper.DumpObject(tx.RequestHeader));
    //        }
    //    }

    //    public static void EndTransaction(int seq, RpcResponse response)
    //    {

    //        RpcMMFClientTransaction tx;
    //        if (_dict.TryRemove(seq, out tx))
    //        {
    //            tx.Callback(response);
    //        }
    //        else
    //        {
    //            _tracing.ErrorFmt("out of band sequence:{0},{1}", seq, ObjectHelper.DumpObject(response));
    //        }
    //    }

    //    private static void MonitorProc()
    //    {
    //        while (true)
    //        {
    //            try
    //            {
    //                long currentTickets = _watch.ElapsedMilliseconds;
    //                foreach (var kv in _dict)
    //                {
    //                    if (kv.Value.Tickets + (long)kv.Value.Timeout < currentTickets)
    //                    {
    //                        RpcResponse rsp = new RpcResponse(RpcErrorCode.TransactionTimeout, null);
    //                        EndTransaction(kv.Key, rsp);

    //                        _tracing.Error("transation timeout");
    //                    }
    //                }
    //            }
    //            catch (ThreadAbortException)
    //            {
    //                Thread.ResetAbort();
    //            }
    //            catch (Exception ex)
    //            {
    //                _tracing.ErrorFmt(ex, "MonitorProc Error");
    //            }

    //            Thread.Sleep(1000);
    //        }
    //    }
    //}
}
