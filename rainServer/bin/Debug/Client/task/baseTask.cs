using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace rainClient.task
{
    abstract class baseTask
    {
        public static baseTask createTask(string task)
        {
            string[] comand = task.Split(' ');

            switch (comand[0])
            {
                case "removeHostDNS":
                    return new removeDNS();

                case "startProcess":
                    return new cmdTask(comand[1], comand[2], comand[3]);

                case "sendMessage":
                    return new startMessage(comand[1]);

                case "mouse":
                    return new mouseTask(comand[1]);

                case "keyBoard":
                    return new keyBoardTask();
            }

            throw new Exception();
        }

        public abstract void startProces(string atribute);       
    }
}
