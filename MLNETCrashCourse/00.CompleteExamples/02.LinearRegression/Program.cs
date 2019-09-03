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
            MLContext mlContext = new MLContext();

            // Define a reader: specify the data columns, types, and where to find them in the text file
            var reader = mlContext.Data.CreateTextLoader(
                columns: new TextLoader.Column[]
                {
                    new TextLoader.Column("CocoaPercent", DataKind.Single, 1),
                    new TextLoader.Column("Label", DataKind.Single, 4)
                },
                // First line of the file is a header, not a data row
                hasHeader: true
            );

            // Create a preview and print out like a CSV
            var trainingData = reader.Load(TrainDataPath);
            var preview = trainingData.Preview(10);
            Console.WriteLine($"******************************************");
            Console.WriteLine($"Loaded training data: {preview.ToString()}");
            Console.WriteLine($"******************************************");
            foreach (var columnInfo in preview.ColumnView)
            {
                Console.Write($"{columnInfo.Column.Name},");
            }
            Console.WriteLine();
            foreach (var rowInfo in preview.RowView)
            {
                foreach(var row in rowInfo.Values) {
                    Console.Write($"{row.Value},");
                }
                Console.WriteLine();
            }

            /* Create pipeline */
            // Apply standard ML.NET normalization to the raw data
            var pipeline =
                // Specify the PoissonRegression regression trainer
                mlContext.Transforms.Concatenate("Features", "CocoaPercent")
                .Append(mlContext.Regression.Trainers.PoissonRegression());

            /* Train the model */
            var model = pipeline.Fit(trainingData);

            /* Get the prediction */
            // Use the trained model for one-time prediction
            var predictionEngine = model.CreatePredictionEngine<ChocolateInput, ChocolateOutput>(mlContext);

            // Obtain the prediction
            var prediction = predictionEngine.Predict(new ChocolateInput
            {
                CocoaPercent = 65, // trained value 65
            });

            Console.WriteLine($"*************************************");
            Console.WriteLine($"Predicted customer happiness: {prediction.CustomerHappiness:0.##}");
            Console.WriteLine($"*************************************");

            // Generate graph with training data
            ChartGeneratorUtil.PlotRegressionChart(new PlotChartGeneratorModel
            {
                Title = "Chocolate Consumer Happiness Prediction",
                LabelX = "Cocoa Percent",
                LabelY = "Customer Happiness",
                ImageName = "CocoaPercentToHappiness.png",
                PointsList = new List<PlotChartPointsList>
                        {
                        new PlotChartPointsList { Points = ChartGeneratorUtil.GetChartPointsFromFile(TrainDataPath, 1, 4).ToList() }
                        },
                MaxLimitY = ChartGeneratorUtil.GetMaxColumnValueFromFile(TrainDataPath, 4) + 10,
            });
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
