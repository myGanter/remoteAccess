using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace rainServer
{
    public partial class paswdInput : Form
    {
        public bool morfoze { get; private set; }
        public string getPasswd { get { return textBox1.Text; } }

        public paswdInput()
        {
            InitializeComponent();
            morfoze = false;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            morfoze = true;
            Close();
        }
    }
}
