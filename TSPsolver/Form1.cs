using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;
using System.Threading;

namespace TSPsolver
{
    public partial class Form1 : Form
    {
        private Bitmap origin;
        private double aimValue;
        private String pointFileNmae = "city.tsp";
        private int mapSize;
        DateTime start;
        SimulatedAnnealing sa;

        public Form1()
        {
            InitializeComponent();
            sa = new SimulatedAnnealing(ResultArea);
        }

        private void Start(object sender, EventArgs e) {
            start = DateTime.Now;
            StartButton.Enabled = false;
            mapSize = 0;
            List<Point> pointList = new List<Point>();
            using (StreamReader srp = File.OpenText(pointFileNmae))
            {
                String Line;
                while ((Line = srp.ReadLine()) != null)
                {
                    int x = 0, y = 0, tmp = 0, cnt = 0;
                    for(int i = 0; i < Line.Length; i++)
                    {
                        if (Line[i] >= '0' && Line[i] <= '9')
                        {
                            tmp = tmp * 10 + Line[i] - '0';
                        }
                        else if (cnt != 0 || (cnt == 0 && tmp == pointList.Count + 1))
                        {
                            cnt++;
                            if (cnt == 2) x = tmp;
                            else if (cnt == 3) y = tmp;
                            tmp = 0;
                        }
                        else tmp = 0;
                    }
                    if (tmp != 0) y = tmp;
                    if (x > mapSize) mapSize = x;
                    if (y > mapSize) mapSize = y;
                    pointList.Add(new Point(x, y));
                }
                srp.Close();
            }
            mapSize += 10;

            if (origin != null) origin.Dispose();
            Point[] point = pointList.ToArray();
            PointF[] pointf = new PointF[point.Length];
            float scaleRateX = mapSize / (float)ResultArea.Width;
            float scaleRateY = mapSize / (float)ResultArea.Height;
            for (int i = 0; i < point.Length; i++)
            {
                pointf[i].X = point[i].X / scaleRateX;
                pointf[i].Y = point[i].Y / scaleRateY;
            }

            new Thread((ThreadStart)delegate
            {
                sa.simulateAnnealing(point, pointf);
                Invoke((EventHandler)delegate
                {
                    StartButton.Enabled = true;
                    TimeSpan timeSpan = DateTime.Now - start;
                });
            })
            { IsBackground = true }.Start();
        }
    }

    
}
