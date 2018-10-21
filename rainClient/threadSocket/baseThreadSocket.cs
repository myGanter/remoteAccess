using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Net.Sockets;

namespace rainClient.threadSocket
{
    abstract class baseThreadSocket
    {
        protected byte[] buffer;
        protected Socket soc;

        public baseThreadSocket(byte[] buffer)
        {
            this.buffer = buffer;
        }

        public abstract void startProces(string ip, int port);
    }
}
