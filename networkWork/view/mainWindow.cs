using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Net.Sockets;

namespace networkWork.view
{
    public delegate Task send(int clientsCount, string ip, string task, string atribute);

    public interface mainWindow
    {
        void connectionClient(Socket newClient, string ip, DateTime connectedTime);
        void shutdownClient(Socket Client, string ip, DateTime disconnectedTime);
        void message(string mes, string header);
        void showNetworkLoad(int send, int received);

        event Action<Socket, streamWindow> streamStart;    
        event send sendInfo;
        event Action<ipMode, string> ipEvent;
        event Action<compileMode, string, bool, bool, string> compile;
        event Action<string> chooseNetwork;
        event Action<Socket, string> StartSaveFraim;
        event Action<Socket> StopSaveFraim;
    }
}
