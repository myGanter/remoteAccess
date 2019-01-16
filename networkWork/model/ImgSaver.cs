using System;
using System.IO;
using System.Drawing;

namespace networkWork.model
{
    public class ImgSaver
    {
        private string folder;
        public string Folder
        {
            get => folder;
            set
            {
                folder = value;
                NestedFolder = value + FileName.Split(new char[] { '.' })[0] + DateTime.Now.ToString().Replace(':', '.');
            }
        }
        public string NestedFolder { get; private set; }
        private string fileName;
        public string FileName
        {
            get => fileName.Replace("{0}", "");
            set
            {
                var h = value.Split(new char[] { '.' });
                fileName = value.Replace(h[0], h[0] + "{0}");
            }
        }

        public long Count { get; private set; }

        public ImgSaver(string folder, string fileName)
        {
            FileName = fileName;
            Folder = folder;
            Directory.CreateDirectory(NestedFolder);
        }

        public void SaveImg(Image img)
        {            
            string newFile = string.Format(fileName, Count);
            img.Save(NestedFolder + "\\" + newFile);
            Count++;
            img.Dispose();
        }
    }
}
