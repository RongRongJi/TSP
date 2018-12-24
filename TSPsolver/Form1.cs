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
using Microsoft.VisualBasic.PowerPacks;
using ThreadState = System.Threading.ThreadState;
using System.Diagnostics;

namespace TSPsolver
{
    public partial class Form1 : Form
    {

        GeneticAlgorithm ga;
        ShapeContainer shapeContainer_allCityShape;
        List<LineShape> lineShape_Way = new List<LineShape>();
        int counter_City = 0;
        List<OvalShape> ovalShape_City = new List<OvalShape>();
        Graphics gpath;
        Graphics point;

        public Form1()
        {
            InitializeComponent();

            shapeContainer_allCityShape = new ShapeContainer();
            shapeContainer_allCityShape.Location = new Point(0, 0);
            shapeContainer_allCityShape.Size = new Size(Width, Height);
            shapeContainer_allCityShape.TabIndex = 0;
            shapeContainer_allCityShape.TabStop = false;
            //Controls.Add(shapeContainer_allCityShape);
            gpath = this.CreateGraphics();
            this.SetStyle(ControlStyles.AllPaintingInWmPaint, true);
            this.SetStyle(ControlStyles.OptimizedDoubleBuffer, true);
            this.BackColor = Color.White;
        }
        
  
        #region create city

        private void create_City(Point e)
        {
            counter_City++;
            OvalShape newCity = new OvalShape();
            newCity.BackColor = Color.Red;
            newCity.BackStyle = BackStyle.Opaque;
            newCity.Cursor = Cursors.Hand;
            newCity.BorderColor = Color.Red;
            newCity.Location = new Point(e.X, e.Y);
            newCity.Size = new Size(20, 20);

            ovalShape_City.Add(newCity);
            shapeContainer_allCityShape.Shapes.Add(newCity);
            Graphics g = this.CreateGraphics();
            g.FillEllipse(Brushes.Black, e.X, e.Y, 10, 10);
        }
        #endregion


        private void Form_Click(object sender, MouseEventArgs e)
        {
            Point mPosition = new Point(e.X - 10, e.Y - 10);
            foreach (LineShape anyLine in lineShape_Way)
                shapeContainer_allCityShape.Shapes.Remove(anyLine);
            create_City(mPosition);
        }

        private void Start(object sender,EventArgs e)
        {
            Thread runtime = new Thread(new ThreadStart(GA));
            setThreadPriority(runtime);
            runtime.Start();
        }

        #region 遗传算法
        public void GA()
        {
            for (int i = 0; i < ovalShape_City.Count; i++)
            {
                LineShape newLine = new LineShape();
                newLine.BorderColor = Color.Blue;
                newLine.Cursor = Cursors.Default;
                newLine.Enabled = false;
                lineShape_Way.Add(newLine);
                shapeContainer_allCityShape.Shapes.Add(newLine);
            }
            int scale = 60;
            float Pc = 0.8f, Pm = 0.9f;
            int GEN_MAX = 1000;
            ga = new GeneticAlgorithm(scale, ovalShape_City.Count, GEN_MAX, Pc, Pm, ovalShape_City);
            ga.solveInit();
            for (int i = 0; i < GEN_MAX; i++)
            {
                ga.GAStep();
                refreshTour();
            }
        }
        #endregion

        #region 更新连线
        private void refreshTour()
        {
            Point p1, p2;
            gpath.Clear(Color.White);
            for(int i = 0; i < counter_City; i++)
            {
                Graphics g = this.CreateGraphics();
                g.FillEllipse(Brushes.Black, ovalShape_City[i].Location.X-5, ovalShape_City[i].Location.Y-5, 10, 10);
            }
            for (int i = 1; i < counter_City; i++)
            {
                p1 = ovalShape_City[ga.bestTour[i]].Location;
                p2 = ovalShape_City[ga.bestTour[i - 1]].Location;
                Pen p = new Pen(Brushes.Blue);
                gpath.DrawLine(p, p1, p2);
            }
            p1 = ovalShape_City[ga.bestTour[counter_City - 1]].Location;
            p2 = ovalShape_City[ga.bestTour[0]].Location;
            Pen pen = new Pen(Brushes.Blue);
            gpath.DrawLine(pen, p1, p2);
            Thread.Sleep(1);
        }
        #endregion

        #region 多线程操作
        delegate void AddShapeCallBack(LineShape l);
        private void AddLineShape(LineShape l)
        {
            if (shapeContainer_allCityShape.InvokeRequired)
            {
                AddShapeCallBack d = new AddShapeCallBack(AddLineShape);
                Invoke(d, new object[] { l });
            }
            else
            {
                shapeContainer_allCityShape.Shapes.Add(l);
            }
        }

        delegate void RemoveShapeCallBack(LineShape l);
        private void RemoveLineShape(LineShape l)
        {
            if (shapeContainer_allCityShape.InvokeRequired)
            {
                RemoveShapeCallBack d = new RemoveShapeCallBack(RemoveLineShape);
                Invoke(d, new object[] { l });
            }
            else
            {
                shapeContainer_allCityShape.Shapes.Remove(l);
            }
        }

        delegate void SetPointCallBack(int i, Point p1, Point p2);
        private void SetPoint(int i, Point p1, Point p2)
        {
            lineShape_Way[i].X1 = p1.X + 10;
            lineShape_Way[i].X2 = p2.X + 10;
            lineShape_Way[i].Y1 = p1.Y + 10;
            lineShape_Way[i].Y2 = p2.Y + 10;
        }
        #endregion

        private void setThreadPriority(Thread th)
        {
            if (th != null)
            {
                if (th.ThreadState != ThreadState.Aborted &&
                   th.ThreadState != ThreadState.AbortRequested &&
                   th.ThreadState != ThreadState.Stopped &&
                   th.ThreadState != ThreadState.StopRequested)
                {
                    switch (Process.GetCurrentProcess().PriorityClass)
                    {
                        case ProcessPriorityClass.AboveNormal:
                            th.Priority = ThreadPriority.AboveNormal;
                            break;
                        case ProcessPriorityClass.BelowNormal:
                            th.Priority = ThreadPriority.BelowNormal;
                            break;
                        case ProcessPriorityClass.High:
                            th.Priority = ThreadPriority.Highest;
                            break;
                        case ProcessPriorityClass.Idle:
                            th.Priority = ThreadPriority.Lowest;
                            break;
                        case ProcessPriorityClass.Normal:
                            th.Priority = ThreadPriority.Normal;
                            break;
                        case ProcessPriorityClass.RealTime:
                            th.Priority = ThreadPriority.Highest;
                            break;
                    }
                    //
                    // Set Thread Affinity 
                    //
                    Thread.BeginThreadAffinity();
                }
            }
        }
    }


}
