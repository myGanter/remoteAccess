using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.IO;
using System.Net;
using rainClient.task;
using System.Net.Sockets;

namespace rainClient.threadSocket
{
    class taskSocket : baseThreadSocket
    {
        public taskSocket(byte[] buffer) : base(buffer) { }

        public override void startProces(string ip, int port)
        {
            soc = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            MemoryStream mS = new MemoryStream(buffer);

            try
            {
                soc.Connect(IPAddress.Parse(ip), port);
                soc.Receive(buffer);
                using (BinaryReader sr = new BinaryReader(mS))
                {
                    baseTask.createTask(sr.ReadString()).startProces(sr.ReadString());
                }
            }
            catch
            {
                Console.WriteLine("taskSocket error");
            }
            finally
            {
                soc.Close();
                mS.Close();
                GC.Collect();
            }         
        }
    }
}
