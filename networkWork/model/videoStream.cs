using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Net;
using System.Net.Sockets;
using System.IO;
using System.Drawing;

namespace networkWork.model
{
    public delegate void connectionClient(Socket newClient, string ip, DateTime connectedTime);
    public delegate void shutdownClient(Socket Client, string ip, DateTime disconnectedTime);
    public delegate void imgClient(Image img);

    public class videoStream
    {
        private Socket server;
        private List<Socket> clients;
        private List<bool> streams;
        private MemoryStream mS;
        private byte[] buffer;
        private int port;
        public event connectionClient connectionClientEvent;
        public event shutdownClient shutdownClientEvent;

        public videoStream(int bufferSize, int port = 1234)
        {
            server = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            clients = new List<Socket>();
            streams = new List<bool>();
            buffer = new byte[bufferSize];
            this.port = port;
            mS = new MemoryStream(buffer);
        }

        public void sendTask(Socket client, string task, string atribute)
        {
            try
            {
                Task.Run(() =>
                {
                    using (MemoryStream mS = new MemoryStream())
                    {
                        using (BinaryWriter bW = new BinaryWriter(mS))
                        {
                            bW.Write(task);
                            bW.Write(atribute);
                            client.Send(mS.ToArray());
                        }
                    }
                });
            }
            catch
            { }
        }

        public int startStreaming(Socket client, imgClient metod)
        {
            streams.Add(true);
            Task.Run(() =>
            {
                if (!clients.Contains(client))
                    return;
                int ID = streams.Count - 1;
                while (streams[ID])
                {
                    try
                    {
                        client.Receive(buffer);
                        metod.Invoke(Image.FromStream(mS));
                    }
                    catch
                    { }
                    System.Threading.Thread.Sleep(100);
                }
            });

            return streams.Count - 1;
        }

        public void stopStreaming(int id) => streams[id] = false;

        public Task listenSocets(int listenCount) => Task.Run(() => 
        {
            System.Threading.Thread.Sleep(300);
            server.Bind(new IPEndPoint(IPAddress.Any, port));
            server.Listen(listenCount);

            deleteDisconnectedClients();

            for (; ; )
            {
                Socket client = server.Accept();
                clients.Add(client);
                connectionClientEvent?.Invoke(client, ((IPEndPoint)client.RemoteEndPoint).Address.ToString(), DateTime.Now);
            }
        });

        private Task deleteDisconnectedClients() => Task.Run(() => 
        {
            for (; ; )
            {
                for (int i = 0; i < clients.Count; i++)
                {
                    if (!SocketConnected(clients[i]))
                    {
                        shutdownClientEvent?.Invoke(clients[i], ((IPEndPoint)clients[i].RemoteEndPoint).Address.ToString(), DateTime.Now);
                        clients.RemoveAt(i);
                    }
                }

                System.Threading.Thread.Sleep(1000);
            }
        });

        private bool SocketConnected(Socket s)
        {
            bool part1 = s.Poll(2000, SelectMode.SelectRead);
            bool part2 = (s.Available == 0);
            if (part1 && part2)
                return false;
            else
                return true;
        }
    }
}
