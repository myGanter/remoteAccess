using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace rainClient.task
{
    class keyBoardTask : baseTask
    {
        public override void startProces(string atribute)
        {
            Console.WriteLine($"key {atribute}");
            GO.keybd_event(byte.Parse(atribute), 0, 0 | 0, 0);
            GO.keybd_event(byte.Parse(atribute), 0, 0 | 2, 0);
        }
    }
}
