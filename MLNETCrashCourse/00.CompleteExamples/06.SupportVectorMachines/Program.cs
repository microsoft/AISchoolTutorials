using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Microsoft.ML;
using Microsoft.ML.Data;
using myMLNET.Common;
using myMLNET.Common.Models;
using myMLNET.Common.Utils;

namespace _06.SupportVectorMachines
{
    class Program
    {
        private static readonly string TrainDataPath = Path.Combine(Environment.CurrentDirectory, "data", "trees.txt");
 
        static void Main(string[] args)
        {
            // Create MLContext to be shared across the model creation workflow objects
            MLContext mlContext = new MLContext();

            // Read the training data
            var trainingData = mlContext.Data.LoadFromTextFile<TreeInput>(TrainDataPath, separatorChar: '\t', hasHeader: true);

            // Read the training data
            var preview = trainingData.Preview();
            Console.WriteLine($"******************************************");
            Console.WriteLine($"Loaded training data: {preview}");
            Console.WriteLine($"******************************************");

            /* Build the classification plot for leaf features graph */
            ChartGeneratorUtil.PlotRegressionChart(new PlotChartGeneratorModel
            {
                Title = "Classification plot for leaf features",
                LabelX = "leaf width",
                LabelY = "leaf length",
                ImageName = "ClassificationPlotForLeafFeatures.png",
                PointsList = new List<PlotChartPointsList>
                    {
                        new PlotChartPointsList { Points = ChartGeneratorUtil.GetChartPointsFromFile(TrainDataPath, 0, 1, 4, 1).ToList(), Color = CommonConstants.PPLplotColorGreen },
                        new PlotChartPointsList { Points = ChartGeneratorUtil.GetChartPointsFromFile(TrainDataPath, 0, 1, 4, 0).ToList(), Color = CommonConstants.PPLplotColorBlue }
                    },
                MaxLimitX = ChartGeneratorUtil.GetMaxColumnValueFromFile(TrainDataPath, 0) + 0.25,
                MaxLimitY = ChartGeneratorUtil.GetMaxColumnValueFromFile(TrainDataPath, 1) + 0.25,
                DrawRegressionLine = false
            });

            /* Build the classification plot for trunk features graph */
            ChartGeneratorUtil.PlotRegressionChart(new PlotChartGeneratorModel
            {
                Title = "Classification plot for trunk features",
                LabelX = "trunk girth",
                LabelY = "trunk height",
                ImageName = "ClassificationPlotForTrunkFeatures.png",
                PointsList = new List<PlotChartPointsList>
                    {
                        new PlotChartPointsList { Points = ChartGeneratorUtil.GetChartPointsFromFile(TrainDataPath, 2, 3, 4, 1).ToList(), Color = CommonConstants.PPLplotColorGreen },
                        new PlotChartPointsList { Points = ChartGeneratorUtil.GetChartPointsFromFile(TrainDataPath, 2, 3, 4, 0).ToList(), Color = CommonConstants.PPLplotColorBlue }
                    },
                MaxLimitX = ChartGeneratorUtil.GetMaxColumnValueFromFile(TrainDataPath, 0) + 0.25,
                MaxLimitY = ChartGeneratorUtil.GetMaxColumnValueFromFile(TrainDataPath, 1) + 0.25
            });

            // Apply standard ML.NET normalization to the raw data
            var pipeline = 
                // Specify the support vector machine trainer
                mlContext.Transforms.Concatenate("Features", "LeafWidth", "LeafLength", "TrunkGirth", "TrunkHeight")
                .Append(mlContext.BinaryClassification.Trainers.LinearSupportVectorMachines());

            // Train the model
            var model = pipeline.Fit(trainingData);

            // Use the trained model for one-time prediction
            var predictionEngine = model.CreatePredictionEngine<TreeInput, TreeOutput>(mlContext);

            // Obtain the prediction
            var prediction = predictionEngine.Predict(new TreeInput
            {
                LeafWidth = 5.13E+00f,
                LeafLength = 6.18E+00f,
                TrunkGirth = 8.26E+00f,
                TrunkHeight = 8.74E+00f
            });

            Console.WriteLine($"*************************************");
            Console.WriteLine("Tree type {0}", prediction.TreeType ? "0" : "1");
            Console.WriteLine($"Score: {prediction.Score}");
            Console.WriteLine($"Probability: {prediction.Probability}");
            Console.WriteLine($"*************************************");

            // Build the SVM plot for leaf features graph
            ChartGeneratorUtil.PlotRegressionChart(new PlotChartGeneratorModel
            {
                Title = "SVM plot for leaf features",
                LabelX = "leaf width",
                LabelY = "leaf length",
                ImageName = "SVMPlotForLeafFeatures.png",
                PointsList = new List<PlotChartPointsList>
                    {
                        new PlotChartPointsList { Points = ChartGeneratorUtil.GetChartPointsFromFile(TrainDataPath, 0, 1, 4, 1).ToList(), Color = CommonConstants.PPLplotColorGreen },
                        new PlotChartPointsList { Points = ChartGeneratorUtil.GetChartPointsFromFile(TrainDataPath, 0, 1, 4, 0).ToList(), Color = CommonConstants.PPLplotColorBlue }
                    },
                MaxLimitX = ChartGeneratorUtil.GetMaxColumnValueFromFile(TrainDataPath, 0) + 0.25,
                MaxLimitY = ChartGeneratorUtil.GetMaxColumnValueFromFile(TrainDataPath, 1) + 0.25,
                DrawRegressionLine = false,
                DrawVectorMachinesAreas = true,
            });

            // Build the SVM plot for trunk features graph
            ChartGeneratorUtil.PlotRegressionChart(new PlotChartGeneratorModel
            {
                Title = "SVM plot for trunk features",
                LabelX = "trunk girth",
                LabelY = "trunk height",
                ImageName = "SVMPlotForTrunkFeatures.png",
                PointsList = new List<PlotChartPointsList>
                    {
                        new PlotChartPointsList { Points = ChartGeneratorUtil.GetChartPointsFromFile(TrainDataPath, 2, 3, 4, 1).ToList(), Color = CommonConstants.PPLplotColorGreen },
                        new PlotChartPointsList { Points = ChartGeneratorUtil.GetChartPointsFromFile(TrainDataPath, 2, 3, 4, 0).ToList(), Color = CommonConstants.PPLplotColorBlue }
                    },
                MaxLimitX = ChartGeneratorUtil.GetMaxColumnValueFromFile(TrainDataPath, 0) + 0.25,
                MaxLimitY = ChartGeneratorUtil.GetMaxColumnValueFromFile(TrainDataPath, 1) + 0.25,
                DrawRegressionLine = false,
                DrawVectorMachinesAreas = true,
            });
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
