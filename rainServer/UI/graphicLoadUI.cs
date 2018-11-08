using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace rainServer.UI
{
    public partial class graphicLoadUI : UserControl
    {

        private Color[] seriesColor;
        public Color[] SeriesColor
        {
            get { return seriesColor; }
            set
            {
                seriesColor = value;
                initialSeries();
            }
        }     

        private Bitmap bmp;
        private Graphics g;
        private int cW = 60;
        private int cH;
        private List<int>[] points;

        public graphicLoadUI()
        {
            InitializeComponent();
        }

        public void reDraw(params int[] values)
        {
            if (seriesColor == null || values.Length != seriesColor.Length)
                throw new Exception("This number of elements in values is not ALLOWED");

            int max = 0;
            for (int i = 0; i < points.Length; i++)
            {
                points[i].RemoveAt(0);
                points[i].Add(values[i]);

                int mmax = points[i].Max();
                max = max < mmax ? mmax : max;
            }              

            g.Clear(BackColor);

            for (int i = 0; i < points.Length; i++)
            {
                for (int j = 0; j < points[i].Count - 1; j++)
                {
                    g.DrawLine
                            (
                                new Pen(seriesColor[i]),
                                j * bmp.Width / cW,
                                bmp.Height - (max == 0 ? 0 : points[i][j] * bmp.Height / max),
                                (j + 1) * bmp.Width / cW,
                                bmp.Height - (max == 0 ? 0 : points[i][j + 1] * bmp.Height / max)
                            );
                }
                g.DrawLine
                        (
                            new Pen(seriesColor[i]),
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
                double sizer = (double)max / cH;
                g.DrawString(((int)(i * (sizer == 0 ? 1 : sizer))).ToString(), new Font(FontFamily.GenericMonospace, 10), Brushes.Gray, 0, bmp.Height - i * size - 17);
            }

            g.DrawString("Bytes", new Font(FontFamily.GenericMonospace, 10), Brushes.Gray, 0, 0);
            g.DrawString("60 second", new Font(FontFamily.GenericMonospace, 10), Brushes.Gray, bmp.Width - 80, bmp.Height - 17);

            pictureBox1.Image = bmp;
        }

        private void graphicLoadUI_Load(object sender, EventArgs e)
        {
            bmp = new Bitmap(pictureBox1.Width, pictureBox1.Height);
            g = Graphics.FromImage(bmp);
            cH = bmp.Height / 40;

            //initialSeries();
        }

        private void graphicLoadUI_Resize(object sender, EventArgs e)
        {
            if (pictureBox1.Width > 0 && pictureBox1.Height > 0)
            {
                bmp = new Bitmap(pictureBox1.Width, pictureBox1.Height);
                g = Graphics.FromImage(bmp);
                cH = bmp.Height / 40;
                cH = cH < 1 ? 1 : cH;
            }
        }

        private void initialSeries()
        {
            if (seriesColor != null)
            {
                points = new List<int>[seriesColor.Length];
                for (int i = 0; i < seriesColor.Length; i++)
                {
                    points[i] = new List<int>();
                    for (int j = 0; j < cW; j++)
                        points[i].Add(0);
                }
            }
        }     
    }
}
