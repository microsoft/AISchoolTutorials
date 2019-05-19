using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Microsoft.Data.DataView;
using Microsoft.ML;
using Microsoft.ML.Data;
using Microsoft.ML.Trainers;
using myMLNET.Common;
using myMLNET.Common.Models;
using myMLNET.Common.Utils;

namespace myMLNET
{
    class Program
    {
        /* Add data path code */
        
        static void Main(string[] args)
        {
            /* Add ML Context and Text File Loader */

            /* Print out data summary */

            /* Get the average values */
            
            /* Build the graph */

            /* Create pipeline */
            
            /* Train the model */

            /* Get the predictions */

            Console.ReadKey();
        }


        public static IEnumerable<PlotChartPoint> GetAvgChartPointsFromData(
          IEnumerable<TrafficData> data)
        {
            return null;
        }

        public class TrafficData
        {
            
        }

        public class TrafficPrediction
        {
            [ColumnName("Score")]
            public float InternetTraffic { get; set; }
        }
    }
}
