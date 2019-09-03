using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Microsoft.ML;
using Microsoft.ML.Data;
using Microsoft.ML.Trainers.FastTree;
using myMLNET.Common.Models;
using myMLNET.Common.Utils;

namespace myMLNET
{
    class LinearRegression
    {
        private static readonly string TrainDataPath = Path.Combine(Environment.CurrentDirectory,"data", "chocolate-data.txt");
        
        static void Main(string[] args)
        {
            /* Add ML Context */

            /* Create TextLoader */

            /* Print out data */

            /* Create pipeline */

            /* Train the model */

            /* Get the prediction */

            /* Generate graph */

            Console.ReadKey();
        }

        public class ChocolateInput
        {
            public float CocoaPercent { get; set; }

            public float Weight { get; set; }
        }

        public class ChocolateOutput
        {
            [ColumnName("Score")]
            public float CustomerHappiness { get; set; }
        }
    }
}
