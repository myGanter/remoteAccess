using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace rainClient.task
{
    class removeDNS : baseTask
    {
        public override void startProces(string atribute) => GO.writeNewDomein(atribute);        
    }
}
