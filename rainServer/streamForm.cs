using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using networkWork.view;

namespace rainServer
{
    public partial class streamForm : Form, streamWindow
    {
        public Socket client { get; set; }

        public event Action<Socket, Action<Image>> closeWindow;
        public event Action<Socket, string, string> buttonTask;

        private bool key = true;
        private int clientW;
        private int clientH;

        public streamForm()
        {
            InitializeComponent();
            KeyDown += Form2_KeyDown;
        }

        public void draw(Image img)
        {
            Image pastImg = pictureBox1.BackgroundImage;            

            clientW = img.Width;
            clientH = img.Height;
            pictureBox1.BackgroundImage = img;

            pastImg.Dispose();
        }

        private void Form2_FormClosing(object sender, FormClosingEventArgs e)
        {
            closeWindow?.Invoke(client, draw);
        }

        private string normolizeCoordinate(MouseEventArgs e)
        {
            keyInfo pressKey = keyInfo.MouseLeft;
            switch (e.Button)
            {
                case MouseButtons.Left:
                    pressKey = keyInfo.MouseLeft;
                    break;
                case MouseButtons.Right:
                    pressKey = keyInfo.MouseRight;
                    break;
                case MouseButtons.Middle:
                    pressKey = keyInfo.MouseWheel;
                    break;
            }

            return $"{(int)pressKey} {clientW * e.X / pictureBox1.Width} {clientH * e.Y / pictureBox1.Height}";
        }

        private void pictureBox1_MouseClick(object sender, MouseEventArgs e)
        {
            Task.Run(() => {
                System.Threading.Thread.Sleep(300);
                if (key)
                {
                    buttonTask?.Invoke(client, $"{keyMode.mouse} oneClick", normolizeCoordinate(e));
                }                
            });          
        }

        private void pictureBox1_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            key = false;
            buttonTask?.Invoke(client, $"{keyMode.mouse} doubleClick", normolizeCoordinate(e));
            System.Threading.Thread.Sleep(300);
            key = true;
        }

        private void Form2_KeyDown(object sender, KeyEventArgs e)
        {
            buttonTask?.Invoke(client, $"{keyMode.keyBoard}", $"{(int)e.KeyCode}");
        }
    }
}
