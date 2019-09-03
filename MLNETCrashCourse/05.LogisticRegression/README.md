# Logistic Regression

Logistic regression predicts binary (yes/no) events. For example, we may want to predict if someone will arrive at work on time, or if a person shopping will buy a product.

This exercise will demonstrate simple logistic regression: predicting an outcome from only one feature.

> **Note:** Make sure that in the solution explorer you right click on **05.LogisticRegression** and select **Set as StartUp project**

## Step 1 - Looking at our data

We want to place a bet on the outcome of the next football (soccer) match. It is the final of a competition, so there will not be a draw. We have historical data about our favourite team playing in matches such as this. Complete the exercise below to preview our data.

This data shows the average goals per match of our team for that season in the left column. In the right column it lists a 1 if our team won the competition or a 0 if they did not. Lets have a look at our data:

1. Open `Program.cs` in this project by double clicking on it in the explorer.

    ![Open project](images/open-program.png)

1. First we need to define the path of the data file that we are going to use in the exercises. **Locate the comment that reads `/* Add data path code */` and replace with the following code**:

    ```csharp
    private static readonly string TrainDataPath = Path.Combine(Environment.CurrentDirectory,"data", "football-data.txt");
    ```

1. The next step is to add the following code the create the [MLContext](https://docs.microsoft.com/en-us/dotnet/api/microsoft.ml.mlcontext?view=ml-dotnet) and the [TextLoader](https://docs.microsoft.com/en-us/dotnet/api/microsoft.ml.data.textloader?view=ml-dotnet) to read the training data, this is explained with more detail in the [Linear Regression](../02.LinearRegression/README.md).  **Locate the comment that reads `/* Add ML Context and TextLoader */` and replace with the following code**:

    ```csharp
    var mlContext = new MLContext();

    var reader = mlContext.Data.CreateTextLoader(
        columns: new TextLoader.Column[]
        {
            new TextLoader.Column("AverageGoalsPerMatch", DataKind.Single, 0),
            new TextLoader.Column("Label", DataKind.Boolean, 1)
        },
        // First line of the file is a header, not a data row
        hasHeader: true
    );
    ```

    > **Note:** You can also copy the complete solution from [Complete Examples](../00.CompleteExamples/05.LogisticRegression/Program.cs).

1. Finally we are going load to data into an `IDataView` using the `TextLoader` we created. Then we can use the `IDataView` method `Preview` get a summary of the data and write it out to the console. **Locate the comment that reads `/* Print out data summary */` and replace with the following code**:

    ```csharp
    var trainingData = reader.Load(TrainDataPath);
    var preview = trainingData.Preview();
    Console.WriteLine($"******************************************");
    Console.WriteLine($"Loaded training data: {preview.ToString()}");
    Console.WriteLine($"******************************************");
    ```
1. Save the file.
    > **Alert:** Make sure you change the debug setting to `NET Core Launch (console) (05.LogisticRegression)` in Visual Studio Code like it is explained in the [Introduction to ML.NET](../01.IntroductionToMLNET/README.md)

1. Run the project by pressing F5.
1. Observe the summary about the loaded training data in the console window that launches. Open the file at [data](/data/football-data.txt) to review the training data in detail.

## Step 2 - Visualize Data

Let's graph the data so we have a better idea of what's going on here. Complete the exercise below to make an x-y scatter plot.

1. Locate the comment that reads `/* Build the basic graph */` and replace it with the following code:

    ```csharp
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
    ```
1. Save the file.
1. Run the project by pressing F5.
1. Analyze the data displayed in the graph. We can see from this graph that generally, when our team has a good score average, they tend to win the competition.

## Step 3 - Building a model and running a prediction

How can we predict whether the team will win this season? Let's apply AI to this problem, by making a logisitic regression model using this data and then graph it. This will tell us whether we will likely win this season.

1. The first thing you will need to do is to create a pipeline. In this case we are creating a pipeline using the `LogisticRegression` algorithm,  a well-known statistical method for determining the contribution of multiple factors to a pair of outcomes. Locate the comment that reads `/* Create pipeline */` and replace with the following code:

    ```csharp
    var pipeline = 
            mlContext.Transforms.Concatenate("Features", "AverageGoalsPerMatch")
            .Append(mlContext.BinaryClassification.Trainers.LogisticRegression(
                new Microsoft.ML.Trainers.LogisticRegression.Options
                {
                    ShowTrainingStats = true
                }
            ));
    ```

1. The next step is to train our model by passing our training data to the method `Fit`. Locate the line that reads `/* Train the model */` and replace it with the following code:

    ```csharp
    // Train the model
    var model = pipeline.Fit(trainingData);
    ```

1. Use the model to work out the loss. Paste the following code snippet after the previous line where we create the model:
	```csharp
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
	```

1. Locate the comment that reads `/* Build the Competition Win Likelihood graph */` and replace it with the following code:
    ```csharp
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
    ```
1. Save the file.
1. Run the project by pressing F5.
1. Take a look at the graph, we now have a line fit to our data. This red line is our logistic regression model.

## Step 4 - Get predictions from your model

We can read the model above like so:

* Take the average number of goals per match for the current year. Let's say it is 2.5.
* Find 2.5 on the x-axis.
* What value (on the y axis) does the line have at x=2.5?
* If this value is above 0.5, then the model thinks our team will win this year. If it is less than 0.5, it thinks our team will lose.
* Because this line is just a mathematical function (equation) we don't have to do this visually.

In the exercise below, **choose the number of goals you want to evaluate**.

The code will calculate the probability that our team will win with your chosen number of goals in the match.

1. Let's use the model to get a prediction. In this case we do this by calling the method `CreatePredictionEngine` to generate our final prediction engine. Then we can pass in the `AverageGoalsPerMatch` to predict the chances to win using our model. Locate the comment that reads `/* Get the predictions */` and replace it with the following code:

    ```csharp
    // Use the trained model for one-time prediction
    var predictionEngine = model.CreatePredictionEngine<FootballInput, FootballOutput>(mlContext);

    // Obtain the prediction
    var goalsPerMatch = <add Value>
    var prediction = predictionEngine.Predict(new FootballInput
    {
        AverageGoalsPerMatch = goalsPerMatch
    });

    Console.WriteLine($"*************************************");
    Console.WriteLine($"Probability of winning this year: { prediction.WonCompetition * 100 }%");
    Console.WriteLine($"*************************************");
    ```

1. Replace the `goalsPerMatch` value in the code above with the number of goals in a match this year. Use any number **from 0 to 3**, i.e `2.5f`.
1. Locate the comment that reads `/* Build the Probability of winning this year graph */` and replace it with the following code:
    ```csharp
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
    ```
1. Save the file.
1. Run the project by pressing F5.
1. Analyze the data displayed in the graph. We can see from this graph that generally, when our team has a good score average, they tend to win the competition.
