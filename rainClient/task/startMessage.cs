using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace rainClient.task
{
    class startMessage : baseTask
    {
        private string caption { get; set; }

        public startMessage(string caption)
        {
            this.caption = caption;            
        }

        public override void startProces(string atribute) => MessageBox.Show(atribute, caption);

    }
}
