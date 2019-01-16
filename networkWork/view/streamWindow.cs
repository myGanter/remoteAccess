using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Net.Sockets;
using System.Drawing;

namespace networkWork.view
{
    public interface streamWindow
    {
        Socket client { get; set; }
        void draw(Image img);

        event Action<Socket, Action<Image>> closeWindow;
        event Action<Socket, string, string> buttonTask;
    }
}
