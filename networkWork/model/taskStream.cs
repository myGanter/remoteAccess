using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.IO;
using System.Net;
using System.Net.Sockets;

namespace networkWork.model
{
    public static class taskStream
    {
        public static int port = 6666;
        public static int listenCount = 10;
        public static event Action<string, string> message;

        public static Task sendTask(int clientsCount, string ip, string task, string atribute) => Task.Run(() => 
        {
            Socket server = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            Socket client = null;          

            try
            {
                server.Bind(new IPEndPoint(IPAddress.Any, port));
                server.Listen(listenCount);
                int count = 1;
                do
                {
                    client?.Close();
                    if (count > clientsCount)
                        throw new Exception("Active user with this IP is not found :(");
                    client = server.Accept();
                    count++;
                }
                while (((IPEndPoint)client.RemoteEndPoint).Address.ToString() != ip);

                using (MemoryStream mS = new MemoryStream())
                {
                    using (BinaryWriter bW = new BinaryWriter(mS))
                    {
                        bW.Write(task);
                        bW.Write(atribute);
                        client.Send(mS.ToArray());
                    }
                }

                message?.Invoke("Completed!", "Task stream");
            }
            catch (Exception e)
            {
                message?.Invoke(e.Message, "Error!");
            }
            finally
            {
                server.Dispose();                
                client?.Dispose();
            }
        });
    }
}
