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
        // https://dotnet.microsoft.com/learn/machinelearning-ai/ml-dotnet-get-started-tutorial/intro
        private static readonly string TrainDataPath = Path.Combine(Environment.CurrentDirectory, "data", "traffic_by_hour.csv");

        static void Main(string[] args)
        {
            // Create MLContext to be shared across the model creation workflow objects
            MLContext mlContext = new MLContext();

            // Read the training data
            var data = mlContext.Data.LoadFromTextFile<TrafficData>(TrainDataPath, separatorChar: '\t', hasHeader: false);

            // Split dataset in two parts: TrainingDataset (80%) and TestDataset (20%)
            TrainCatalogBase.TrainTestData dataSplit = mlContext.Regression.TrainTestSplit(data, testFraction: 0.2);
            var trainingData = dataSplit.TrainSet;
            var preview = trainingData.Preview();
            Console.WriteLine($"******************************************");
            Console.WriteLine($"Loaded training data: {preview}");
            Console.WriteLine($"******************************************");

            var testData = dataSplit.TestSet;
            preview = testData.Preview();
            Console.WriteLine($"******************************************");
            Console.WriteLine($"Loaded test data: {preview}");
            Console.WriteLine($"******************************************");

            // Get an array of the average data points
            var avgPoints = GetAvgChartPointsFromData(mlContext.Data.CreateEnumerable<TrafficData>(trainingData, reuseRowObject: true));

            // Generate graph with training data
            ChartGeneratorUtil.PlotRegressionChart(new PlotChartGeneratorModel
            {
                Title = "Internet traffic over the day",
                LabelX = "Time of day",
                LabelY = "Internet traffic (Gbps)",
                ImageName = "InternetTrafficOverTheDay.png",
                PointsList = new List<PlotChartPointsList>
                {
                  new PlotChartPointsList { Points = ChartGeneratorUtil.GetChartPointsFromFile(TrainDataPath, 0, 1, hasHeader: false).ToList(), Color = CommonConstants.PPLplotColorBlue, PaintDots = false},
                  new PlotChartPointsList { Points = ChartGeneratorUtil.GetChartPointsFromFile(TrainDataPath, 0, 2, hasHeader: false).ToList(), Color = CommonConstants.PPLplotColorGreen, PaintDots = false},
                  new PlotChartPointsList { Points = ChartGeneratorUtil.GetChartPointsFromFile(TrainDataPath, 0, 3, hasHeader: false).ToList(), Color = CommonConstants.PPLplotColorRed, PaintDots = false},
                  new PlotChartPointsList { Points = ChartGeneratorUtil.GetChartPointsFromFile(TrainDataPath, 0, 4, hasHeader: false).ToList(), Color = CommonConstants.PPLplotColorBlack, PaintDots = false},
                  new PlotChartPointsList { Points = ChartGeneratorUtil.GetChartPointsFromFile(TrainDataPath, 0, 5, hasHeader: false).ToList(), Color = CommonConstants.PPLplotColorRed2, PaintDots = false},
                  new PlotChartPointsList { Points = ChartGeneratorUtil.GetChartPointsFromFile(TrainDataPath, 0, 6, hasHeader: false).ToList(), Color = CommonConstants.PPLplotColorRed3, PaintDots = false},
                  new PlotChartPointsList { Points = avgPoints.ToList(), Color = CommonConstants.PPLplotColorBlue}
                },
                MaxLimitX = 24,
                MaxLimitY = 70,
                DrawRegressionLine = false
            });

            // Create the pipeline
            var pipeline =
                // Specify the Poisson regression trainer
                mlContext.Transforms.Concatenate("Features", "Time", "AverageMeasure")
                .Append(mlContext.Regression.Trainers.PoissonRegression());

            // Train the model
            var model = pipeline.Fit(trainingData);

            // Use the trained model to predict the internet traffic
            var predictionEngine = model.CreatePredictionEngine<TrafficData, TrafficPrediction>(mlContext);
            
            // This represents the time 12:30
            var time = 12.5f;

            // Obtain the prediction
            var prediction = predictionEngine.Predict(new TrafficData
            {
                Time = time,
                HistoricalMeasures = new float[] { 43.5f, 45.3f, 41.9f, 40.3f, 31.5f, 44.6f }
            });

            Console.WriteLine($"At t={time}, predicted internet traffic is {prediction.InternetTraffic} Gbps.");
            Console.ReadKey();
        }

        public static IEnumerable<PlotChartPoint> GetAvgChartPointsFromData(
          IEnumerable<TrafficData> data)
        {
            return data
                .Select(x => new PlotChartPoint()
                {
                    X = x.Time,
                    Y = x.AverageMeasure
                });
        }

        public class TrafficData
        {
            [LoadColumn(0)]
            [ColumnName("Time")]
            public float Time { get; set; }

            [LoadColumn(1, 6)]
            [VectorType(6)]
            [ColumnName("HistoricalMeasures")]
            public float[] HistoricalMeasures { get; set; }

            [LoadColumn(7)]
            [ColumnName("Label")]
            public float InternetTraffic { get; set; }

            [LoadColumn(8)]
            [ColumnName("AverageMeasure")]
            public float AverageMeasure
            {
                set { }
                get
                {
                    return (HistoricalMeasures == null || HistoricalMeasures.Count() == 0) ? 0 : HistoricalMeasures.Average();
                }
            }
        }

        public class TrafficPrediction
        {
            [ColumnName("Score")]
            public float InternetTraffic { get; set; }
        }
    }
}
