using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Microsoft.ML;
using Microsoft.ML.Data;
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
            /* Add ML Context and TextLoader */

            /* Print out data summary */

            /* Build the classification plot for leaf features graph */

            /* Build the classification plot for trunk features graph */

            /* Create pipeline */
            
            /* Train the model */

            /* Get the predictions */

            // Build the SVM plot for leaf features graph

            // Build the SVM plot for trunk features graph

            Console.ReadKey();
        }

        public class TreeInput
        {
            [LoadColumn(0)]
            public float LeafWidth { get; set; } 

            [LoadColumn(1)]
            public float LeafLength { get; set; }

            [LoadColumn(2)]
            public float TrunkGirth { get; set; }

            [LoadColumn(3)]
            public float TrunkHeight { get; set; }

            [LoadColumn(4)]
            public bool Label { get; set; }
        }

        public class TreeOutput
        {
            [ColumnName("PredictedLabel")]
            public bool TreeType { get; set; }
            public float Score { get; set; }
            public float Probability { get; set; }
        }
    }
}
