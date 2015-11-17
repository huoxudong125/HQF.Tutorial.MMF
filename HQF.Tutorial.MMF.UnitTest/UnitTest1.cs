using System;
using System.Linq;
using Xunit;
using Xunit.Abstractions;

namespace HQF.Tutorial.MMF.UnitTest
{
    public unsafe class UnitTest1
    {

        private readonly ITestOutputHelper output;

        public UnitTest1(ITestOutputHelper output)
        {
            this.output = output;
        }

        [Fact]
        public void TestMethod1()
        {
            Random rand = new Random();
            fixed (byte* p = new byte[1024*1024])
            {
                LoopMemoryStream stream = new LoopMemoryStream(p, 1024*1024);
                CodeTimer.Time("test", 200, () =>
                {
                    for (int i = 0; i < 10000; i++)
                    {
                        byte[] data = new byte[rand.Next(0, 4096)];
                        stream.Write(data, 0, data.Length);
                        if (stream.Read().Length != data.Length)
                            throw new ArgumentException();
                    }
                });

                output.WriteLine("Finished the test");
            }
        }
    }
}
