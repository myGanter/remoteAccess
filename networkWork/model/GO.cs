﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.IO;
using System.Net;
using System.Text.RegularExpressions;
using System.Runtime.Serialization.Formatters.Binary;

namespace networkWork.model
{
    public static class GO
    {
        //"http://ganter-001-site1.etempurl.com" без паттерна
        //"https://www.whatismyip.org" с паттерном "<a href=\"/my-ip-address\">(.*)</a></h3>"
        public static string parceIP(string webSite, string pattern = null)
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
            return null;
        }

        public static void writeNewDomein(string domein)
        {
            if (File.Exists("domainInfo.bin"))
                File.Delete("domainInfo.bin");
            using (FileStream fs = new FileStream("domainInfo.bin", FileMode.OpenOrCreate))
            {
                new BinaryFormatter().Serialize(fs, domein);
            }
        }

        public static string getDomain()
        {
            if (File.Exists("domainInfo.bin"))
            {
                string result = null;
                using (FileStream fs = new FileStream("domainInfo.bin", FileMode.Open))
                {
                    result = (string)new BinaryFormatter().Deserialize(fs);
                }
                return result;
            }                                   
            else
                throw new Exception("No configuration file found");
        }

        public static string setNewIp(string passwd)
        {
            string domain = getDomain();
            string IP = parceIP("https://www.whatismyip.org", "<a href=\"/my-ip-address\">(.*)</a></h3>");
            WebRequest request = WebRequest.Create(domain);
            request.Method = "POST";
            string data = "ip=" + IP + "&passwd=" + passwd;
            byte[] byteArray = System.Text.Encoding.UTF8.GetBytes(data);
            request.ContentType = "application/x-www-form-urlencoded";
            request.ContentLength = byteArray.Length;

            using (Stream dataStream = request.GetRequestStream())
            {
                dataStream.Write(byteArray, 0, byteArray.Length);
            }

            WebResponse response = request.GetResponseAsync().Result;
            string result = null;

            using (Stream stream = response.GetResponseStream())
            {
                using (StreamReader reader = new StreamReader(stream))
                {
                    result = reader.ReadToEnd();
                }
            }

            return result;
        }
    }
}
