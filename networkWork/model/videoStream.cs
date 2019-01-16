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
    public class videoStream
    {
        private Socket server;
        private Dictionary<Socket, List<Action<Image>>> ActiveStreams;
        private int bufferSize;
        private int port;
        public event Action<Socket, string, DateTime> connectionClientEvent;
        public event Action<Socket, string, DateTime> shutdownClientEvent;

        public videoStream(int bufferSize, int port = 1234)
        {
            server = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            ActiveStreams = new Dictionary<Socket, List<Action<Image>>>();
            this.port = port;
            this.bufferSize = bufferSize;
        }

        public void sendTask(Socket client, string task, string atribute)
        {
            Task.Run(() =>
            {
                try
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
                }
                catch { }
            });
        }

        private void startStreaming(Socket client) => Task.Run(() =>
        {
            byte[] buffer = new byte[bufferSize];

            using (MemoryStream mS = new MemoryStream(buffer))
            {
                while (ActiveStreams.ContainsKey(client) && ActiveStreams[client].Count > 0)
                {
                    try
                    {
                        client.Receive(buffer);
                        Image img = Image.FromStream(mS);
                        foreach (var i in ActiveStreams[client])
                            i.Invoke((Image)img.Clone());
                        img.Dispose();
                    }
                    catch
                    { }
                    System.Threading.Thread.Sleep(100);
                }
            }
        });        

        public void AddActionForSocket(Socket client, Action<Image> metod)
        {
            if (ActiveStreams.ContainsKey(client))
            {
                ActiveStreams[client].Add(metod); 
                if (ActiveStreams[client].Count == 1)
                    startStreaming(client);
            }         
            
        }

        public void RemoveActionForSocket(Socket client, Action<Image> metod)
        {
            if (!ActiveStreams.ContainsKey(client))
                return;            
            ActiveStreams[client].Remove(metod);
        }

        public Task listenSocets(int listenCount) => Task.Run(() => 
        {
            System.Threading.Thread.Sleep(300);
            server.Bind(new IPEndPoint(IPAddress.Any, port));
            server.Listen(listenCount);

            deleteDisconnectedClients();

            for (; ; )
            {
                Socket client = server.Accept();
                ActiveStreams.Add(client, new List<Action<Image>>());
                connectionClientEvent?.Invoke(client, ((IPEndPoint)client.RemoteEndPoint).Address.ToString(), DateTime.Now);
            }
        });

        private Task deleteDisconnectedClients() => Task.Run(() => 
        {
            for (; ; )
            {
                foreach (var i in ActiveStreams)
                {
                    if (!SocketConnected(i.Key))
                    {
                        shutdownClientEvent?.Invoke(i.Key, ((IPEndPoint)i.Key.RemoteEndPoint).Address.ToString(), DateTime.Now);
                        i.Key.Close();
                        ActiveStreams.Remove(i.Key);
                        break;
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
