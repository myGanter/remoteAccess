using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

using networkWork.model;

namespace rainServer
{
    public partial class inputForm : Form
    {
        public bool morfoze { get; private set; }
        public string getDomain { get { return textBox1.Text; } }

        public inputForm()
        {
            InitializeComponent();
            morfoze = false;
        }

        private void button2_Click(object sender, EventArgs e)
        {
            try
            {
                MessageBox.Show(GO.parceIP(textBox1.Text), "IP");
            }
            catch
            {
                MessageBox.Show(":(", "Error!");
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            morfoze = true;
            Close();
        }
    }
}
