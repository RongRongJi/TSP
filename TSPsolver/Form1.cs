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
        AntColonyAlgorithm aca;
        ShapeContainer shapeContainer_allCityShape;
        int counter_City = 0;
        List<OvalShape> ovalShape_City = new List<OvalShape>();
        bool isCalculating;

        public Form1()
        {
            InitializeComponent();

            shapeContainer_allCityShape = new ShapeContainer();
            shapeContainer_allCityShape.Location = new Point(0, 0);
            shapeContainer_allCityShape.Size = new Size(Width, Height);
            shapeContainer_allCityShape.TabIndex = 0;
            shapeContainer_allCityShape.TabStop = false;
            this.SetStyle(ControlStyles.AllPaintingInWmPaint, true);
            this.SetStyle(ControlStyles.OptimizedDoubleBuffer, true);
            this.SetStyle(ControlStyles.UserPaint, true);
            pictureBox1.BackColor = Color.White;
        }
        
  
        #region 创建城市
        private void create_City(Point e)
        {
            counter_City++;
            OvalShape newCity = new OvalShape();
            newCity.Location = new Point(e.X, e.Y);
            newCity.Size = new Size(20, 20);
            ovalShape_City.Add(newCity);
            shapeContainer_allCityShape.Shapes.Add(newCity);
            Bitmap origin = new Bitmap(pictureBox1.Width, pictureBox1.Height);
            Graphics g = Graphics.FromImage(origin);
            for (int i = 0; i < counter_City; i++)
            {
                g.FillEllipse(Brushes.Black, ovalShape_City[i].Location.X - 5, ovalShape_City[i].Location.Y - 5, 10, 10);
            }
            pictureBox1.BackgroundImage = origin;
        }
        #endregion

        #region 画点
        private void pictureBox1_Click(object sender, MouseEventArgs e)
        {
            if (isCalculating) return;
            Point mPosition = new Point(e.X - 10, e.Y - 10);
            create_City(mPosition);
        }
        #endregion

        #region 开启算法
        private void Start(object sender,EventArgs e)
        {
            if(!radioButton1.Checked && !radioButton2.Checked)
            {
                MessageBox.Show("请选择一种算法！");
                return;
            }
            if (isCalculating) return;
            isCalculating = true;
            //遗传算法
            if (radioButton1.Checked) {
                if (counter_City <= 2)
                {
                    MessageBox.Show("至少设置三个城市！");
                    return;
                }
                Thread runtime = new Thread(new ThreadStart(GA));
                setThreadPriority(runtime);
                runtime.Start();
            }
            //蚁群算法
            else if (radioButton2.Checked)
            {
                if (counter_City <= 2)
                {
                    MessageBox.Show("至少设置三个城市！");return;
                }
                Thread runtime = new Thread(new ThreadStart(ACA));
                setThreadPriority(runtime);
                runtime.Start();
            }
        }
        #endregion

        #region 遗传算法
        public void GA()
        {
            for (int i = 0; i < ovalShape_City.Count; i++)
            {
                LineShape newLine = new LineShape();
                newLine.BorderColor = Color.Blue;
                newLine.Cursor = Cursors.Default;
                newLine.Enabled = false;
                shapeContainer_allCityShape.Shapes.Add(newLine);
            }
            int scale = 60;
            float Pc = 0.95f, Pm = 0.05f;
            int GEN_MAX = SetGAMaxValue();
            progressBar1.Maximum = GEN_MAX;
            ga = new GeneticAlgorithm(scale, ovalShape_City.Count, GEN_MAX, Pc, Pm, ovalShape_City);
            ga.solveInit();
            for (int i = 0; i < GEN_MAX; i++)
            {
                ga.GAStep();
                refreshUI();
                progressBar1.Value = i+1;
                textBox1.Text = "遗传代数："+(i+1) + "/" + GEN_MAX;
                richTextBox1.Text = ga.toString();
            }
            isCalculating = false;
        }
        private int SetGAMaxValue()
        {
            if (counter_City == 0) return 0;
            else if (counter_City <= 5) return 100;
            else if (counter_City <= 15) return 1000;
            else if (counter_City <= 30) return 10000;
            else if (counter_City <= 40) return 51000;
            else if (counter_City <= 60) return 100000;
            else return 1000000;
        }
        #endregion

        #region 遗传算法下更新UI
        private void refreshUI()
        {
            Point p1, p2;
            Bitmap origin = new Bitmap(pictureBox1.Width, pictureBox1.Height);
            Graphics g = Graphics.FromImage(origin);
            for (int i = 0; i < counter_City; i++)
            {
                g.FillEllipse(Brushes.Black, ovalShape_City[i].Location.X - 5, ovalShape_City[i].Location.Y - 5, 10, 10);
            }
            for (int i = 1; i < counter_City; i++)
            {
                p1 = ovalShape_City[ga.bestTour[i]].Location;
                p2 = ovalShape_City[ga.bestTour[i - 1]].Location;
                Pen p = new Pen(Brushes.Blue);
                g.DrawLine(p, p1, p2);
            }
            p1 = ovalShape_City[ga.bestTour[counter_City - 1]].Location;
            p2 = ovalShape_City[ga.bestTour[0]].Location;
            Pen pen = new Pen(Brushes.Blue);
            g.DrawLine(pen, p1, p2);
            pictureBox1.BackgroundImage = origin;
        }
        #endregion

        #region 蚁群算法
        private void ACA()
        {
            for (int i = 0; i < ovalShape_City.Count; i++)
            {
                LineShape newLine = new LineShape();
                newLine.BorderColor = Color.Blue;
                newLine.Cursor = Cursors.Default;
                newLine.Enabled = false;
                shapeContainer_allCityShape.Shapes.Add(newLine);
            }
            int antnum = 10;
            int GEN_MAX = SetACAMaxValue();
            progressBar1.Maximum = GEN_MAX;
            double alpha = 1.0, beta = 5.0, rho = 0.5;
            aca = new AntColonyAlgorithm(counter_City, antnum, GEN_MAX, alpha, beta, rho, ovalShape_City);
            aca.init();
            for(int i = 0; i < GEN_MAX; i++)
            {
                aca.ACAStep();
                refreshLine();
                progressBar1.Value = i + 1;
                textBox1.Text = "蚁群算法代数：" + (i + 1) + "/" + GEN_MAX;
                richTextBox1.Text = aca.toString();
            }
            isCalculating = false;
        }
        private int SetACAMaxValue()
        {
            if (counter_City == 0) return 0;
            else if (counter_City <= 5) return 100;
            else if (counter_City <= 40) return 500;
            else if (counter_City <= 60) return 800;
            else return 1000;
        }
        #endregion

        #region 蚁群算法下更新UI
        private void refreshLine()
        {
            Point p1, p2;
            Bitmap origin = new Bitmap(pictureBox1.Width, pictureBox1.Height);
            Graphics g = Graphics.FromImage(origin);
            for (int i = 0; i < counter_City; i++)
            {
                g.FillEllipse(Brushes.Black, ovalShape_City[i].Location.X - 5, ovalShape_City[i].Location.Y - 5, 10, 10);
            }
            for (int i = 1; i < counter_City; i++)
            {
                p1 = ovalShape_City[aca.bestTour[i]].Location;
                p2 = ovalShape_City[aca.bestTour[i - 1]].Location;
                Pen p = new Pen(Brushes.Blue);
                g.DrawLine(p, p1, p2);
            }
            p1 = ovalShape_City[aca.bestTour[counter_City - 1]].Location;
            p2 = ovalShape_City[aca.bestTour[0]].Location;
            Pen pen = new Pen(Brushes.Blue);
            g.DrawLine(pen, p1, p2);
            pictureBox1.BackgroundImage = origin;
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



        #endregion

        private void clearButton_Click(object sender, EventArgs e)
        {
            if (isCalculating) return;
            ovalShape_City.Clear();
            counter_City = 0;
            Bitmap origin = new Bitmap(pictureBox1.Width, pictureBox1.Height);
            Graphics g = Graphics.FromImage(origin);
            g.Clear(Color.White);
            pictureBox1.BackgroundImage = origin;
        }
    }


}
