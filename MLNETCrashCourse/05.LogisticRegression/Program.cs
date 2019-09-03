using System;
using System.IO;
using System.Linq;
using Microsoft.ML;
using Microsoft.ML.Data;
using Microsoft.ML.Calibrators;
using Microsoft.ML.Trainers;
using System.Collections.Generic;
using myMLNET.Common.Utils;
using myMLNET.Common.Models;
using myMLNET.Common;

namespace myMLNET
{
    class Program
    {
        /* Add data path code */
 
        static void Main(string[] args)
        {
            /* Add ML Context and TextLoader */

            /* Print out data summary */

            /* Build the basic graph */

            /* Create pipeline */
            
            /* Train the model */

            /* Build the Competition Win Likelihood graph */

            /* Get the predictions */

            /* Build the Probability of winning this year graph */

            Console.ReadKey();
        }

        public static double Sigmoid(double x)
        {
            return 1 / (1 + Math.Exp(-x));
        }

        public static IEnumerable<PlotChartPoint> GetAvgChartPointsFromData(IEnumerable<double> x, IEnumerable<double> y)
        {
            var points = new List<PlotChartPoint>();
            for (int i = 0; i < x.Count(); i++)
            {
                points.Add(new PlotChartPoint()
                {
                    X = x.ElementAt(i),
                    Y = y.ElementAt(i)
                });
            }

            return points;
        }

        public class FootballInput
        {
            public float AverageGoalsPerMatch { get; set; } 
        }

        public class FootballOutput
        {
            [ColumnName("Score")]
            public float WonCompetition { get; set; }
        }
    }
}
