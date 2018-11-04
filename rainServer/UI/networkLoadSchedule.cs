using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Drawing;
using System.Windows.Forms;

namespace rainServer.UI
{
    class networkLoadSchedule
    {
        private PictureBox pic;
        private Bitmap bmp;
        private Graphics g;
        private int cW = 60;
        private int cH;
        private List<List<int>> points = new List<List<int>>();
        private List<Color> series = new List<Color>();

        public networkLoadSchedule(PictureBox pic, Color series1, Color series2)
        {
            this.pic = pic;
            series.Add(series1);
            series.Add(series2);
            bmp = new Bitmap(pic.Width, pic.Height);
            g = Graphics.FromImage(bmp);
            cH = bmp.Height / 40;

            for (int i = 0; i < 2; i++)
            {
                points.Add(new List<int>());
                for (int j = 0; j < cW; j++)
                    points[i].Add(0);
            }
        }

        public void reDraw(int send, int received)
        {
            points[0].RemoveAt(0);
            points[0].Add(received);
            points[1].RemoveAt(0);
            points[1].Add(send);

            int max = points[0].Max();
            int p1max = points[1].Max();
            max = max < p1max ? p1max : max;

            g.Clear(Color.Black);

            for (int i = 0; i < points.Count; i++)
            {
                for (int j = 0; j < points[i].Count - 1; j++)
                {
                    g.DrawLine
                            (
                                new Pen(series[i]),
                                j * bmp.Width / cW,
                                bmp.Height - (max == 0 ? 0 : points[i][j] * bmp.Height / max),
                                (j + 1) * bmp.Width / cW,
                                bmp.Height - (max == 0 ? 0 : points[i][j + 1] * bmp.Height / max)
                            );
                }
                g.DrawLine
                        (
                            new Pen(series[i]),
                            (points[i].Count - 2) * bmp.Width / cW,
                            bmp.Height - (max == 0 ? 0 : points[i][(points[i].Count - 2)] * bmp.Height / max),
                            ((points[i].Count - 2) + 1) * bmp.Width / cW,
                            bmp.Height - (max == 0 ? 0 : points[i][(points[i].Count - 2) + 1] * bmp.Height / max)
                        );
            }

            int size = bmp.Height / cH;
            for (int i = 0; i < cH; i++)
            {
                g.DrawLine(Pens.Green, 0, i * size, bmp.Width, i * size);
                g.DrawString((i * (max / (bmp.Height / size))).ToString(), new Font(FontFamily.GenericMonospace, 10), Brushes.Gray, 0, bmp.Height - i * size);
            }

            g.DrawString("Bytes", new Font(FontFamily.GenericMonospace, 10), Brushes.Gray, 0, 0);
            g.DrawString("60 second", new Font(FontFamily.GenericMonospace, 10), Brushes.Gray, bmp.Width - 80, bmp.Height - 17);

            pic.Image = bmp;
        }

        public void reSize()
        {
            if (pic.Width > 0 && pic.Height > 0)
            {
                bmp = new Bitmap(pic.Width, pic.Height);
                g = Graphics.FromImage(bmp);
                cH = bmp.Height / 40;
                cH = cH < 1 ? 1 : cH;
            }
        }
    }
}
