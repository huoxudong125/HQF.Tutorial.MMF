namespace HQF.Tutorial.MMF
{
    internal class RpcMMFConfiguration
    {
        private static volatile RpcMMFConfiguration _current=null;
        private static readonly object LockObj=new object();

        private RpcMMFConfiguration()
        {
      
           
        }

        public static RpcMMFConfiguration Current
        {
            get
            {
                lock (LockObj)
                {
                    if (_current == null)
                    {
                        _current = new RpcMMFConfiguration() { MMFSize = 10 }; 
                    }
                   
                }
              
                return _current;
            }
        }

        public int MMFSize { get; private set; }
    }
}