using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Threading;
using System.Windows.Forms;

namespace TSPsolver
{
    public class SimulatedAnnealing : System.Windows.Forms.Form
    {
        Random random = new Random();
        Bitmap origin;
        int areaWidth, areaHeight;
        public System.Windows.Forms.Timer timer = new System.Windows.Forms.Timer();
        int ticker = 0;
        private int[] operate = new int[3];
        const int optInit = 500;
        const double Temperature = 1.2;
        const double delta = 0.99;
        const int markov = 15000;
        const int Limit = markov / 5;
        const int BLimit = markov / 15;
        Point[] point;
        PointF[] pointf;
        PictureBox ResultArea;
        double currentDistant;
        double tmp, de, best;
        double t = Temperature;
        PointF[] bestPathf;
        PointF[] tmpPointf;
        Point[] bestPath;
        Point[] tmpPoint;
        bool isBlock = false;
        bool isChange;
        int l1 = 0, l2 = 0, whichOperate;

        public SimulatedAnnealing(PictureBox ra,Point[] point,PointF[] pointf)
        {
            ResultArea = ra;
            areaWidth = ra.Width;
            areaHeight = ra.Height;
            timer = new System.Windows.Forms.Timer();
            timer.Interval=1;
            timer.Tick += new EventHandler(simulateAnnealing);
            operate[0] = optInit;
            operate[1] = optInit;
            operate[2] = optInit;
            this.point = point;
            this.pointf = pointf;

        }

        //模拟退火法
        public void simulateAnnealing(Object sender,EventArgs eve)
        {
            point.CopyTo(tmpPoint, 0);
            pointf.CopyTo(tmpPointf, 0);
            whichOperate = GetNext(point, pointf);
            tmp = GetDistant(point);
            de = currentDistant - tmp;
            isChange = false;
            if (de > 0)
            {
                operate[whichOperate]++;
                currentDistant = tmp;
                isChange = true;
                l1 = 0;
                l2 = 0;
                if (tmp < best)
                {
                    best = tmp;
                    pointf.CopyTo(bestPathf, 0);
                    point.CopyTo(bestPath, 0);
                }
            }
            else
            {
                if (Math.Exp(de / t) > random.Next() / (double)System.Int32.MaxValue)
                {
                    currentDistant = tmp;
                    isChange = true;
                }
                else
                {
                    tmpPoint.CopyTo(point, 0);
                    tmpPointf.CopyTo(pointf, 0);
                }
                l1++;
            }
            if (isChange)
            {
                isBlock = true;
                new Thread((ThreadStart)delegate {
                    try
                    {
                        if (origin != null) origin.Dispose();
                        origin = new Bitmap(areaWidth, areaHeight);
                        using (Graphics g = Graphics.FromImage(origin))
                        {
                            using (GraphicsPath gpath = new GraphicsPath())
                            {
                                gpath.AddPolygon(pointf);
                                g.DrawPath(new Pen(Color.Black)
                                {
                                    Width = 2
                                }, gpath);
                                ResultArea.Image = origin;
                            }
                        }
                    }
                    catch (Exception e)
                    {
                        MessageBox.Show(e.Message);
                        return;
                    }
                    isBlock = false;
                }).Start();
            }
            if (l1 > Limit)
            {
                l2++;
            }
            ticker++;
            if (ticker >= markov)
            {
                if (markov==ticker && l2 > BLimit)
                {
                    new Thread((ThreadStart)delegate {
                        try
                        {
                            if (origin != null) origin.Dispose();
                            origin = new Bitmap(areaWidth, areaHeight);
                            using (Graphics g = Graphics.FromImage(origin))
                            {
                                using (GraphicsPath gpath = new GraphicsPath())
                                {
                                    gpath.AddPolygon(bestPathf);
                                    g.DrawPath(new Pen(Color.Black) { Width = 2 }, gpath);
                                    ResultArea.Image = origin;
                                }
                            }
                        }
                        catch (Exception e)
                        {
                            MessageBox.Show(e.Message);
                        }
                        timer.Enabled = false;
                    }).Start();
                }
                bestPath.CopyTo(point, 0);
                bestPathf.CopyTo(pointf, 0);
                t *= delta;
                ticker = 0;
                isBlock = false;
            }
        }



        //获得当前遍历路径长
        private double GetDistant(Point[] point) {
            double tmp = Math.Sqrt(Math.Pow(point[0].X - point[point.Length - 1].X, 2)
                + Math.Pow(point[0].Y - point[point.Length - 1].Y, 2));
            for(int i = 1; i < point.Length; i++)
            {
                tmp += Math.Sqrt(Math.Pow(point[i - 1].X - point[i].X, 2) 
                    + Math.Pow(point[i - 1].Y - point[i].Y, 2));
            }
            return tmp;
        }

        //交换函数
        private void Swap<T>(ref T a, ref T b)
        {
            T tmp = a;
            a = b;
            b = tmp;
        }

        //状态转移函数，包含3种操作
        private int GetNext(Point[] point, PointF[] pointf)
        {
            int x, y;
            do
            {
                x = random.Next(point.Length);
                y = random.Next(point.Length);
            } while (x == y);
            double randomRate = random.Next() / (double)System.Int32.MaxValue;
            double sum = operate[0] + operate[1] + operate[2];

            //操作1：将区间[X,Y]中的点翻转
            if(randomRate < operate[0] / sum)
            {
                if (x > y) Swap(ref x, ref y);
                while (x < y)
                {
                    Swap(ref point[x], ref point[y]);
                    Swap(ref pointf[x], ref pointf[y]);
                    x++;y--;
                }
                return 0;
            }
            //操作2：交换两点
            else if (randomRate < (operate[0] + operate[1]) / sum)
            {
                Swap(ref point[x], ref point[y]);
                Swap(ref pointf[x], ref pointf[y]);
                return 1;
            }
            //操作3：将一点放到另一点后面
            else
            {
                if (x > y) Swap(ref x, ref y);
                x++;
                while (x < y)
                {
                    Swap(ref point[x], ref point[y]);
                    Swap(ref pointf[x], ref pointf[y]);
                    x++;
                }
                return 2;
            }
        }

        //点位置初始化函数
        public void InitPosition(ref PointF[] pointf,ref Point[] point)
        {
            List<int> list = new List<int>();
            Point[] tmp = new Point[point.Length];
            PointF[] tmpf = new PointF[pointf.Length];
            int index;
            list.Clear();
            for (int i = 0; i < point.Length; i++)
                list.Add(i);
            for(int i = 0; i < point.Length; i++)
            {
                index = random.Next(point.Length - i);
                tmp[i] = point[list[index]];
                tmpf[i] = pointf[list[index]];
                list.RemoveAt(index);
            }
            tmp.CopyTo(point, 0);
            tmpf.CopyTo(pointf, 0);
            currentDistant = GetDistant(point);
            bestPathf = new PointF[pointf.Length];
            bestPath = new Point[point.Length];
            tmpPoint = new Point[point.Length];
            tmpPointf = new PointF[pointf.Length];
            isBlock = false;
            best = currentDistant;
            ticker = 0;
        }
    }
}
