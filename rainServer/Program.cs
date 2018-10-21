﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

using networkWork.model;
using networkWork.presenter;

namespace rainServer
{
    static class Program
    {
        /// <summary>
        /// Главная точка входа для приложения.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            taskStream.listenCount = 10;
            taskStream.port = 6666;
            Form1 f1 = new Form1();
            videoStream vS = new videoStream(9999999);
            mainPresenter presenter = new mainPresenter(vS, f1);

            Application.Run(f1);
        }
    }
}
