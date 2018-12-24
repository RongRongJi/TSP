using Microsoft.VisualBasic.PowerPacks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TSPsolver
{
    public class GeneticAlgorithm
    {
        #region 遗传算法变量
        private int scale;
        private int cityNum;
        private int MAX_GEN;
        public double[,] distance;
        private int bestT;
        private double bestLength;
        public int[] bestTour;
        public int[,] oldPopulation;
        private int[,] newPopulation;
        private double[] fitness;
        private float[] Pi;
        private float Pc;
        private float Pm;
        private int t;
        private Random random;
        #endregion

        public GeneticAlgorithm(int s,int n,int g,float c,float m,List<OvalShape> ovalShapes)
        {
            scale = s;cityNum = n;MAX_GEN = g;Pc = c;Pm = m;
            bestLength = Int32.MaxValue;
            bestTour = new int[cityNum + 1];
            bestT = 0;t = 0;
            newPopulation = new int[scale,cityNum];
            oldPopulation = new int[scale,cityNum];
            fitness = new double[scale];
            Pi = new float[scale];
            random = new Random((int)DateTime.Now.Ticks/10000);
            setDistance(ovalShapes);
        }

        public void setDistance(List<OvalShape> ovalShapes)
        {
            int[] x = new int[cityNum];
            int[] y = new int[cityNum];
            for(int i = 0; i < cityNum; i++)
            {
                x[i] = ovalShapes[i].Location.X;
                y[i] = ovalShapes[i].Location.Y;
            }
            distance = new double[cityNum, cityNum];
            for(int i = 0; i < cityNum - 1; i++)
            {
                distance[i, i] = 0;
                for(int j = i + 1; j < cityNum; j++)
                {
                    double rij = Math.Sqrt(((x[i] - x[j]) * (x[i] - x[j]) + (y[i] - y[j]) * (y[i] - y[j])) / 10.0);
                    distance[i, j] = rij;
                    distance[j, i] = rij;
                }
            }
            distance[cityNum - 1, cityNum - 1] = 0;
        }

        public void initGroup()
        {
            int i, j, k;
            for (k = 0; k < scale; k++)
            {
                oldPopulation[k, 0] = random.Next(65535) % cityNum;
                for (i = 1; i < cityNum;)
                {
                    oldPopulation[k, i] = random.Next(65535) % cityNum;
                    for (j = 0; j < i; j++)
                    {
                        if (oldPopulation[k, i] == oldPopulation[k, j]) break;
                    }
                    if (j == i) i++;
                }
            }
        }

        public double evalute(int[] chromosome)
        {
            double len = 0;
            for (int i = 1; i < cityNum; i++) len += distance[chromosome[i - 1], chromosome[i]];
            len += distance[chromosome[cityNum - 1],chromosome[0]];
            return len;
        }

        public void countRate()
        {
            int k;
            double sumFitness = 0;
            double[] tmpf = new double[scale];
            for (k = 0; k < scale; k++)
            {
                tmpf[k] = 10.0 / fitness[k];
                sumFitness += tmpf[k];
            }
            Pi[0] = (float)(tmpf[0] / sumFitness);
            for (k = 1; k < scale; k++)
            {
                Pi[k] = (float)(tmpf[k] / sumFitness + Pi[k - 1]);
            }
        }

        public void selectBestGh()
        {
            int k, i, maxid = 0;
            double maxevaluation = fitness[0];
            for (k = 1; k < scale; k++)
            {
                if (maxevaluation > fitness[k])
                {
                    maxevaluation = fitness[k];
                    maxid = k;
                }
            }
            if (bestLength > maxevaluation)
            {
                bestLength = maxevaluation;
                bestT = t;
                for (i = 0; i < cityNum; i++)
                {
                    bestTour[i] = oldPopulation[maxid, i];
                }
            }
            copyGh(0, maxid);
        }

        public void copyGh(int k,int t)
        {
            for(int i = 0; i < cityNum; i++)
            {
                newPopulation[k, i] = oldPopulation[t, i];
            }
        }

        public void select()
        {
            int k, i, selectId;
            float ran1;
            for (k = 1; k < scale; k++)
            {
                ran1 = (float)(random.Next(65535) % 1000 / 1000.0);
                for (i = 0; i < scale; i++)
                {
                    if (ran1 <= Pi[i]) break;
                }
                selectId = i;
                copyGh(k, selectId);
            }
        }

        public void NormalCrossover()
        {
            selectBestGh();
            select();
            float r;
            for (int k = 0; k < scale; k = k + 2)
            {
                r = (float)random.NextDouble();
                if (r < Pc) OXCross1(k, k + 1);
                else
                {
                    r = (float)random.NextDouble();
                    if (r < Pm)
                    {
                        OnCVariation(k);
                    }
                    r = (float)random.NextDouble();
                    if (r < Pm)
                    {
                        OnCVariation(k + 1);
                    }
                }
            }
        }

        public void NormalWithoutCross()
        {
            int k;
            selectBestGh();
            select();
            float r;
            for (k = 1; k + 1 < scale / 2; k = k + 2)
            {
                r = (float)random.NextDouble();
                if (r < Pc)
                {
                    OXCross1(k, k + 1);
                }
                else
                {
                    r = (float)random.NextDouble();
                    if (r < Pm) OnCVariation(k);
                    r = (float)random.NextDouble();
                    if (r < Pm) OnCVariation(k + 1);
                }
            }
            if (k == scale / 2 - 1)
            {
                r = (float)random.NextDouble();
                if (r < Pm) OnCVariation(k);
            }
        }

        public void OXCross(int k1,int k2)
        {
            int i, j, k, flag;
            int ran1, ran2, tmp;
            int[] Gh1 = new int[cityNum];
            int[] Gh2 = new int[cityNum];
            ran1 = random.Next(65535) % cityNum;
            ran2 = random.Next(65535) % cityNum;
            while (ran1 == ran2)
            {
                ran2 = random.Next(65535) % cityNum;
            }
            if (ran1 > ran2)
            {
                tmp = ran1;
                ran1 = ran2;
                ran2 = tmp;
            }
            flag = ran2 - ran1 + 1;
            for (i = 0, j = ran1; i < flag; i++, j++)
            {
                Gh1[i] = newPopulation[k2, j];
                Gh2[i] = newPopulation[k1, j];
            }
            for (k = 0, j = flag; j < cityNum;)
            {
                Gh1[j] = newPopulation[k1, k++];
                for (i = 0; i < flag; i++)
                {
                    if (Gh1[i] == Gh1[j]) break;
                }
                if (i == flag) j++;
            }
            for (k = 0, j = flag; j < cityNum;)
            {
                Gh2[j] = newPopulation[k2, k++];
                for (i = 0; i < flag; i++)
                {
                    if (Gh2[i] == Gh2[j]) break;
                }
                if (i == flag) j++;
            }
            for (i = 0; i < cityNum; i++)
            {
                newPopulation[k1, i] = Gh1[i];
                newPopulation[k2, i] = Gh2[i];
            }
        }

        public void OXCross1(int k1,int k2)
        {
            int i, j, k, flag;
            int ran1, ran2, tmp;
            int[] Gh1 = new int[cityNum];
            int[] Gh2 = new int[cityNum];
            ran1 = random.Next(65535) % cityNum;
            ran2 = random.Next(65535) % cityNum;
            while (ran1 == ran2)
            {
                ran2 = random.Next(65535) % cityNum;
            }
            if (ran1 > ran2)
            {
                tmp = ran1;
                ran1 = ran2;
                ran2 = tmp;
            }
            for (i = 0, j = ran2; j<cityNum; i++, j++)
            {
                Gh2[i] = newPopulation[k1, j];
            }
            flag = i;
            for (k = 0, j = flag; j < cityNum;)
            {
                Gh2[j] = newPopulation[k2, k++];
                for (i = 0; i < flag; i++)
                {
                    if (Gh2[i] == Gh2[j]) break;
                }
                if (i == flag) j++;
            }
            flag = ran1;
            for (k = 0, j = 0; k < cityNum;)
            {
                Gh1[j] = newPopulation[k1, k++];
                for (i = 0; i < flag; i++)
                {
                    if (newPopulation[k2, i] == Gh1[j]) break;
                }
                if (i == flag) j++;
            }
            flag = cityNum - ran1;
            for (i = 0, j = flag; j < cityNum; j++, i++)
            {
                Gh1[j] = newPopulation[k2, i];
            }
            for (i = 0; i < cityNum; i++)
            {
                newPopulation[k1, i] = Gh1[i];
                newPopulation[k2, i] = Gh2[i];
            }
        }

        public void OnCVariation(int k)
        {
            int ran1, ran2, tmp;
            int count = random.Next(65535) % cityNum;
            for(int i = 0; i < count; i++)
            {
                ran1 = random.Next(65535) % cityNum;
                ran2 = random.Next(65535) % cityNum;
                while (ran1 == ran2)
                {
                    ran2 = random.Next(65535) % cityNum;
                }
                tmp = newPopulation[k, ran1];
                newPopulation[k, ran1] = newPopulation[k, ran2];
                newPopulation[k, ran2] = tmp;
            }
        }

        public void solveInit()
        {
            int i, k;
            initGroup();
            for (k = 0; k < scale; k++)
            {
                int[] tmp = new int[cityNum];
                for (int j = 0; j < cityNum; j++) tmp[j] = oldPopulation[k, j];
                fitness[k] = evalute(tmp);
            }
            countRate();
        }

        public void GAStep()
        {
            NormalCrossover();
            for(int k = 0; k < scale; k++)
            {
                for(int i = 0; i < cityNum; i++)
                {
                    oldPopulation[k, i] = newPopulation[k, i];
                }
            }
            for(int k = 0; k < scale; k++)
            {
                int[] tmp = new int[cityNum];
                for (int j = 0; j < cityNum; j++) tmp[j] = oldPopulation[k, j];
                fitness[k] = evalute(tmp);
            }
            countRate();
        }
    }
}
