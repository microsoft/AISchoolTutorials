using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Microsoft.ML;
using Microsoft.Data.DataView;
using Microsoft.ML.Data;
using Microsoft.ML.Calibrators;
using Microsoft.ML.Trainers;
using myMLNET.Common.Models;
using myMLNET.Common.Utils;

namespace myMLNET
{
    class MultipleLinearRegression
    {
        /* Add data path code */
        
        static void Main(string[] args)
        {
            /* Add ML Context and TextLoader */

            /* Print out data summary */

            /* Create pipeline */
            
            /* Train the model */

            /* Get the graph data */
            
            /* Build the graph */

            /* Calculate metrics */

            /* Get the predictions */

            /* Show Graph 3D */
            // TODO: Add Graph, update instructions

            Console.ReadKey();
        }


        private static void PrintMetrics(MLContext mlContext, IDataView predictions)
        {

        }

        public class ChocolateInput
        {
            public float Weight { get; set; }
            public float CocoaPercent { get; set; } 
            public float Cost { get; set; } 
        }

        public class ChocolateOutput
        {
            [ColumnName("Score")]
            public float CustomerHappiness { get; set; }
        }
    }
}
