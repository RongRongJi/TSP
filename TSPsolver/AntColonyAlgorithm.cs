using Microsoft.VisualBasic.PowerPacks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TSPsolver
{
    public class AntColonyAlgorithm
    {
        private Ant[] ants;
        private int antNum;
        private int cityNum;
        private int MAX_GEN;
        private double[,] pheromone;
        private double[,] distance;
        private double bestLength;
        public int[] bestTour;
        private List<OvalShape> ovalShapes;

        private double alpha, beta, rho;
        public AntColonyAlgorithm(int citynum,int antnum,int maxgen,double a,double b,double r,List<OvalShape> shapes)
        {
            cityNum = citynum;
            antNum = antnum;
            ants = new Ant[antnum];
            MAX_GEN = maxgen;
            alpha = a;
            beta = b;
            rho = r;
            ovalShapes = shapes;
        }

        private void setDistance(List<OvalShape> ovalShapes)
        {
            int[] x = new int[cityNum];
            int[] y = new int[cityNum];
            for (int i = 0; i < cityNum; i++)
            {
                x[i] = ovalShapes[i].Location.X;
                y[i] = ovalShapes[i].Location.Y;
            }
            distance = new double[cityNum, cityNum];
            for (int i = 0; i < cityNum - 1; i++)
            {
                distance[i, i] = 0;
                for (int j = i + 1; j < cityNum; j++)
                {
                    double rij = Math.Sqrt(((x[i] - x[j]) * (x[i] - x[j]) + (y[i] - y[j]) * (y[i] - y[j])) / 10.0);
                    distance[i, j] = rij;
                    distance[j, i] = rij;
                }
            }
            distance[cityNum - 1, cityNum - 1] = 0;
        }

        public void init()
        {
            setDistance(ovalShapes);
            pheromone = new double[cityNum, cityNum];
            for(int i = 0; i < cityNum; i++)
            {
                for(int j = 0; j < cityNum; j++)
                {
                    pheromone[i, j] = 0.1;
                }
            }
            bestLength = Int32.MaxValue;
            bestTour = new int[cityNum + 1];
            for(int i = 0; i < antNum; i++)
            {
                ants[i] = new Ant(cityNum);
                ants[i].init(distance, alpha, beta);
            }
        }

        public void updatePheromone()
        {
            for (int i = 0; i < cityNum; i++)
                for (int j = 0; j < cityNum; j++)
                    pheromone[i, j] = pheromone[i, j] * (1 - rho);
            for(int i = 0; i < cityNum; i++)
            {
                for(int j = 0; j < cityNum; j++)
                {
                    for(int k = 0; k < antNum; k++)
                    {
                        pheromone[i, j] += ants[k].GetDelta()[i, j];
                    }
                }
            }
        }

        public void ACAStep()
        {
            for(int i = 0; i < antNum; i++)
            {
                for(int j = 1; j < cityNum; j++)
                {
                    ants[i].selectNextCity(pheromone);
                }
                ants[i].GetTabu().Add(ants[i].GetFirstCity());
                if (ants[i].GetTourLength() < bestLength)
                {
                    bestLength = ants[i].GetTourLength();
                    for(int k = 0; k < cityNum + 1; k++)
                    {
                        bestTour[k] = ants[i].GetTabu()[k];
                    }
                }
                for(int j = 0; j < cityNum; j++)
                {
                    ants[i].GetDelta()[ants[i].GetTabu()[j], ants[i].GetTabu()[j + 1]]
                        = (1.0 / ants[i].GetTourLength());
                    ants[i].GetDelta()[ants[i].GetTabu()[j + 1], ants[i].GetTabu()[j]]
                        = (1.0 / ants[i].GetTourLength());
                }
            }
            updatePheromone();
            for(int i = 0; i < antNum; i++)
            {
                ants[i].init(distance, alpha, beta);
            }
        }

        public string toString()
        {
            string result = "蚁群规模:" + antNum + "\n";
            result += "城市数量:" + cityNum + "\n";
            result += "控制参数α:" + alpha + "\n";
            result += "控制参数β:" + beta + "\n";
            result += "控制参数ρ:" + rho + "\n";
            result += "当前最佳路程:" + bestLength + "\n";
            return result;
        }
    }
}
