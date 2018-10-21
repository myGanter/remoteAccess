using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Net.Sockets;

namespace networkWork.view
{
    public interface mainWindow
    {
        void connectionClient(Socket newClient, string ip, DateTime connectedTime);
        void shutdownClient(Socket Client, string ip, DateTime disconnectedTime);
        void message(string mes, string header);

        event Action<Socket, streamWindow> streamStart;
        event Action<int, string, string, string> sendInfo;
        event Action<ipMode, string> ipEvent;
    }
}
