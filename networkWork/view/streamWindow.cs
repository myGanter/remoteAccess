using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Net.Sockets;
using networkWork.model;

namespace networkWork.view
{
    public interface streamWindow
    {
        int streamId { get; set; }
        Socket client { get; set; }
        imgClient drawing { get; }
        event Action<int> closeWindow;
        event Action<Socket, string, string> buttonTask;
    }
}
