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
        private static readonly string TrainDataPath = Path.Combine(Environment.CurrentDirectory,"data", "football-data.txt"); 
 
        static void Main(string[] args)
        {
            // Create MLContext to be shared across the model creation workflow objects
            MLContext mlContext = new MLContext();

            // Define a reader: specify the data columns, types, and where to find them in the text file
            var reader = mlContext.Data.CreateTextLoader(
                columns: new TextLoader.Column[]
                {
                    new TextLoader.Column("AverageGoalsPerMatch", DataKind.Single, 0),
                    new TextLoader.Column("Label", DataKind.Boolean, 1)
                },
                // First line of the file is a header, not a data row
                hasHeader: true
            );

            // Read the training data
            var trainingData = reader.Load(TrainDataPath);
            var preview = trainingData.Preview();
            Console.WriteLine($"******************************************");
            Console.WriteLine($"Loaded training data: {preview}");
            Console.WriteLine($"******************************************");

            /* Build the basic graph */
            ChartGeneratorUtil.PlotRegressionChart(new PlotChartGeneratorModel
            {
                Title = string.Empty,
                LabelX = "Average number of goals scored per match",
                LabelY = "Competition Win",
                ImageName = "CompetitionWin.png",
                PointsList = new List<PlotChartPointsList>
                {
                  new PlotChartPointsList { Points = ChartGeneratorUtil.GetChartPointsFromFile(TrainDataPath, 0, 1, 1, 1).ToList(), Color = CommonConstants.PPLplotColorRed },
                  new PlotChartPointsList { Points = ChartGeneratorUtil.GetChartPointsFromFile(TrainDataPath, 0, 1, 1, 0).ToList()}
                },
                MaxLimitY = 1.25,
                MinLimitY = -0.25,
                MinLimitX = -0.25,
                MaxLimitX = ChartGeneratorUtil.GetMaxColumnValueFromFile(TrainDataPath, 0) + 0.25,
                DrawRegressionLine = false
            });

            // Apply standard ML.NET normalization to the raw data
            var pipeline = 
                mlContext.Transforms.Concatenate("Features", "AverageGoalsPerMatch")
                .Append(mlContext.BinaryClassification.Trainers.LogisticRegression(
                    new Microsoft.ML.Trainers.LogisticRegression.Options
                    {
                        ShowTrainingStats = true
                    }
                ));

            // Train the model
            var model = pipeline.Fit(trainingData);

            // View training stats
            var linearModel = model.LastTransformer.Model.SubModel as LinearBinaryModelParameters;

            // This works out the loss
            var coefficient = linearModel.Weights.FirstOrDefault();
            var intercept = linearModel.Bias;
            var step = 3 / (double)300;
            var testX = Enumerable.Range((int)0, 300).Select(v => (v * step) + 0).ToList();
            var loss = new List<double>();
            foreach (var x in testX)
            {
                loss.Add(Sigmoid(x * coefficient + intercept));
            }

            // Get an array of the average data points
            var lossPoints = GetAvgChartPointsFromData(testX, loss);

            /* Build the Competition Win Likelihood graph */
            ChartGeneratorUtil.PlotRegressionChart(new PlotChartGeneratorModel
            {
                Title = string.Empty,
                LabelX = "Average number of goals per match",
                LabelY = "Competition Win Likelihood",
                ImageName = "CompetitionWinLikelihood.png",
                PointsList = new List<PlotChartPointsList>
                {
                    new PlotChartPointsList { Points = ChartGeneratorUtil.GetChartPointsFromFile(TrainDataPath, 0, 1, 1, 1).ToList(), Color = CommonConstants.PPLplotColorRed },
                    new PlotChartPointsList { Points = ChartGeneratorUtil.GetChartPointsFromFile(TrainDataPath, 0, 1, 1, 0).ToList() }
                },
                MaxLimitY = 1.25,
                MinLimitY = -0.25,
                MinLimitX = -0.25,
                MaxLimitX = ChartGeneratorUtil.GetMaxColumnValueFromFile(TrainDataPath, 0) + 0.25,
                DrawRegressionLine = true,
                RegressionPointsList = new PlotChartPointsList { Points = lossPoints.ToList(), Color = CommonConstants.PPLplotColorBlack }
            });

            // Use the trained model for one-time prediction
            var predictionEngine = model.CreatePredictionEngine<FootballInput, FootballOutput>(mlContext);

            // Obtain the prediction
            var goalsPerMatch = 2.422870462f;
            var prediction = predictionEngine.Predict(new FootballInput
            {
                AverageGoalsPerMatch = goalsPerMatch
            });

            Console.WriteLine($"*************************************");
            Console.WriteLine($"Probability of winning this year: { prediction.WonCompetition * 100 }%");
            Console.WriteLine($"*************************************");

            /* Build the Probability of winning this year graph */
            ChartGeneratorUtil.PlotRegressionChart(new PlotChartGeneratorModel
            {
                Title = "Probability of winning this year",
                LabelX = "Average number of goals per match",
                LabelY = "Competition Win Likelihood",
                ImageName = "ProbabilityOfWinningThisYear.png",
                PointsList = new List<PlotChartPointsList>
                        {
                          new PlotChartPointsList { Points = ChartGeneratorUtil.GetChartPointsFromFile(TrainDataPath, 0, 1, 1, 1).ToList(), Color = CommonConstants.PPLplotColorRed },
                          new PlotChartPointsList { Points = ChartGeneratorUtil.GetChartPointsFromFile(TrainDataPath, 0, 1, 1, 0).ToList()}
                        },
                MaxLimitY = 1.25,
                MinLimitY = -0.25,
                MinLimitX = -0.25,
                MaxLimitX = ChartGeneratorUtil.GetMaxColumnValueFromFile(TrainDataPath, 0) + 0.25,
                DrawRegressionLine = true,
                DashedPoint = new PlotChartPoint { X = goalsPerMatch, Y = prediction.WonCompetition },
                RegressionPointsList = new PlotChartPointsList { Points = lossPoints.ToList(), Color = CommonConstants.PPLplotColorBlack }
            });
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
