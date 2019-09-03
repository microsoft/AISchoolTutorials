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
        private static readonly string TrainDataPath = Path.Combine(Environment.CurrentDirectory, "data", "chocolate-data-multiple-linear-regression.txt");
        
        static void Main(string[] args)
        {
            var mlContext = new MLContext();

            var reader = mlContext.Data.CreateTextLoader(
                columns: new TextLoader.Column[]
                {
                    new TextLoader.Column("Weight", DataKind.Single, 0),
                    new TextLoader.Column("CocoaPercent", DataKind.Single, 1),
                    new TextLoader.Column("Cost", DataKind.Single, 2),
                    new TextLoader.Column("Label", DataKind.Single, 3)
                },
                // First line of the file is a header, not a data row
                hasHeader: true
            );

            var trainingData = reader.Load(TrainDataPath);
            var preview = trainingData.Preview();
            Console.WriteLine($"******************************************");
            Console.WriteLine($"Loaded training data: {preview.ToString()}");
            Console.WriteLine($"******************************************");

            var pipeline =
                // Features to include in the prediction
                mlContext.Transforms.Concatenate("Features", "Weight", "CocoaPercent", "Cost")
                // Specify the regression trainer
                .Append(mlContext.Regression.Trainers.PoissonRegression());
            
            // Train the model
            var model = pipeline.Fit(trainingData);

            // The model's feature weight coefficients
            var regressionModel = model.LastTransformer.Model;
            var weights = regressionModel.Weights;
            var intercept = regressionModel.Bias;
            Console.WriteLine($"Coefficients: Weight={weights[0]:0.##}, CocoaPercent={weights[1]:0.##}, Cost={weights[2]:0.##}");

            /* Build the graph */
            var featureColumn = reader.GetOutputSchema().GetColumnOrNull("CocoaPercent").Value;
            ChartGeneratorUtil.PlotRegressionChart(new PlotChartGeneratorModel
            {
                Title = "Chocolate Consumer Happiness Prediction",
                LabelX = featureColumn.Name,
                LabelY = "Customer Happiness",
                ImageName = "FeatureToHappiness.png",
                PointsList = new List<PlotChartPointsList>
                {
                    new PlotChartPointsList { Points = ChartGeneratorUtil.GetChartPointsFromFile(TrainDataPath, featureColumn.Index, 3).ToList()}
                },
                MinLimitX = ChartGeneratorUtil.GetMinColumnValueFromFile(TrainDataPath, featureColumn.Index),
                MaxLimitX = ChartGeneratorUtil.GetMaxColumnValueFromFile(TrainDataPath, featureColumn.Index) + 0.25,
                MaxLimitY = ChartGeneratorUtil.GetMaxColumnValueFromFile(TrainDataPath, 3) + 5
            });

            // Compute the quality metrics of our prediction model
            var predictions = model.Transform(trainingData);
            PrintMetrics(mlContext, predictions);

            Console.ReadKey();
        }


        private static void PrintMetrics(MLContext mlContext, IDataView predictions)
        {
            // Compute the quality metrics of our prediction model
            var metrics = mlContext.Regression.Evaluate(predictions, "Label", "Score");

            Console.WriteLine();
            Console.WriteLine($"Model quality metrics evaluation");
            // RSquared is another evaluation metric of the regression models. RSquared takes values between 0 and 1. The closer its value is to 1, the better the model is
            Console.WriteLine($"R2 Score: {metrics.RSquared:0.##}");
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
