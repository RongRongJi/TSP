using Microsoft.VisualBasic.PowerPacks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TSPsolver
{
    public class Ant
    {
        private List<int> tabu;         //禁忌表
        private List<int> allowedCities;//允许搜索的城市
        private double[,] delta;        //信息素变化矩阵
        public double[,] distance;
        private double alpha;
        private double beta;

        private double tourLength;
        private int cityNum;
        private int firstCity;
        private int currentCity;
        private Random random;

        public Ant(int city)
        {
            cityNum = city;
            tourLength = 0;
            random = new Random((int)DateTime.Now.Ticks / 10000);
        }

       

        public void init(double[,] distance,double a, double b)
        {
            alpha = a;
            beta = b;
            allowedCities = new List<int>();
            tabu = new List<int>();
            delta = new double[cityNum, cityNum];
            this.distance = distance;
            for (int i = 0; i < cityNum; i++)
            {
                allowedCities.Add(i);
                for (int j = 0; j < cityNum; j++)
                {
                    delta[i, j] = 0.0;
                }
            }
            firstCity = random.Next(cityNum);
            foreach (int i in allowedCities)
            {
                if (i == firstCity)
                {
                    allowedCities.Remove(i);
                    break;
                }
            }
            tabu.Add(firstCity);
            currentCity = firstCity;
        }

        public void selectNextCity(double[,] pheromone)
        {
            double[] p = new double[cityNum];
            double sum = 0.0;
            foreach (int i in allowedCities)
            {
                sum += Math.Pow(pheromone[currentCity, i], alpha) * Math.Pow(1.0 / distance[currentCity, i], beta);
            }
            for (int i = 0; i < cityNum; i++)
            {
                bool flag = false;
                foreach (int j in allowedCities)
                {
                    if (i == j)
                    {
                        p[i] = (Math.Pow(pheromone[currentCity, i], alpha)
                            * Math.Pow(1.0 / distance[currentCity, i], beta)) / sum;
                        flag = true;
                        break;
                    }
                }
                if (!flag) p[i] = 0.0;
            }
            double selectP = random.NextDouble();
            int selectCity = 0;
            double sum1 = 0.0;
            for (int i = 0; i < cityNum; i++)
            {
                sum1 += p[i];
                if (sum1 >= selectP)
                {
                    selectCity = i; break;
                }
            }
            foreach (int i in allowedCities)
            {
                if (i == selectCity)
                {
                    allowedCities.Remove(i);
                    break;
                }
            }
            tabu.Add(selectCity);
            currentCity = selectCity;
        }

        private double calculateTourLength()
        {
            double len = 0;
            for (int i = 0; i < cityNum; i++)
            {
                len += distance[tabu[i], tabu[i + 1]];
            }
            return len;
        }

        public List<int> GetAllowedCitiest() { return allowedCities; }
        public void SetAllowedCitest(List<int> cities) { allowedCities = cities; }
        public double GetTourLength() { return calculateTourLength(); }
        public void SetTourLength(double len) { tourLength = len; }
        public int GetCityNum() { return cityNum; }
        public void SetCityNum(int num) { cityNum = num; }
        public List<int> GetTabu() { return tabu; }
        public void SetTabu(List<int> tabu) { this.tabu = tabu; }
        public double[,] GetDelta() { return delta; }
        public void SetDelta(double[,] delta) { this.delta = delta; }
        public int GetFirstCity() { return firstCity; }
        public void SetFirstCity(int first) { firstCity = first; }
        
    }

}
