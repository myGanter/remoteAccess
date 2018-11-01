using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace rainClient.task
{
    class mouseTask : baseTask
    {
        private string pressMode { get; set; }

        public mouseTask(string pressMode)
        {
            this.pressMode = pressMode;
        }

        public override void startProces(string atribute)
        {
            string[] atr = atribute.Split(' ');
            Console.WriteLine(String.Format("key {0} X:{1} Y:{2}", atr[0], atr[1], atr[2]));

            GO.SetCursorPos(int.Parse(atr[1]), int.Parse(atr[2]));

            switch (atr[0])
            {
                case "1":
                    mousePress(0x8000 | 0x0002 | 0x0004);
                    break;
                case "2":
                    mousePress(0x8000 | 0x0008 | 0x0010);
                    break;
                case "4":
                    mousePress(0x8000 | 0x0020 | 0x0040);
                    break;
            }           
        }

        private void mousePress(uint mode)
        {
            for (int i = 0; i < (pressMode == "oneClick" ? 1 : 2); i++)
                GO.mouse_event(mode, 0, 0, 0, 0);
        }
    }
}
