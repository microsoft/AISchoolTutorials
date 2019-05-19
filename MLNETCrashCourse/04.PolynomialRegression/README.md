# Polynomial Regression

Sometimes our data doesn't have a linear relationship, but we still want to predict an outcome.

Suppose we want to predict how satisfied people might be with a piece of fruit, we would expect satisfaction would be low if the fruit was under ripened or over ripened. Satisfaction would be high in between under ripe and overripe.

This is not something linear regression will help us with, so we can turn to polynomial regression to help us make predictions for these more complex non-linear relationships!

> **Note:** Make sure that in the solution explorer you right click on **04.PolynomialRegression** and select **Set as StartUp project**

## Step 1 - Looking at our data

In this exercise we will look at a dataset analyzing internet traffic over the course of the day. Observations were made every hour over the course of several days. Suppose we want to predict the level of traffic we might see at any time during the day, how might we do this?

Let's start by reviewing our data:

1. Open `Program.cs` in this project by double clicking on it in the explorer.

    ![Open project](images/open-program.png)

1. First we need to define the path of the data file that we are going to use in the exercises. **Locate the comment that reads `/* Add data path code */` and replace with the following code**:

    ```csharp
    private static readonly string TrainDataPath = Path.Combine(Environment.CurrentDirectory,"data", "traffic_by_hour.csv"); 
    ```

1. The next step is to add the following code the create the [MLContext](https://docs.microsoft.com/en-us/dotnet/api/microsoft.ml.mlcontext?view=ml-dotnet) and the [TextLoader](https://docs.microsoft.com/en-us/dotnet/api/microsoft.ml.data.textloader?view=ml-dotnet) to read the training data. We'll use the [LoadFromTextFile](https://docs.microsoft.com/en-us/dotnet/api/microsoft.ml.textloadersavercatalog.loadfromtextfile?view=ml-dotnet#Microsoft_ML_TextLoaderSaverCatalog_LoadFromTextFile_Microsoft_ML_DataOperationsCatalog_System_String_Microsoft_ML_Data_TextLoader_Column___System_Char_System_Boolean_System_Boolean_System_Boolean_System_Boolean_) method to read the data from the file, the columns are defined in the **TrafficData** model. **Locate the comment that reads `/* Add ML Context and Text File Loader */` and replace with the following code**: 

    ```csharp
    MLContext mlContext = new MLContext();

    // Read the training data
    var data = mlContext.Data.LoadFromTextFile<TrafficData>(TrainDataPath, separatorChar: '\t', hasHeader: false);
    ```

    > **Note:** You can also copy the complete solution from [Complete Examples](../00.CompleteExamples/04PolynomialRegression.cs).

1. Find the class `TrafficData` at the bottom of the file and add the following code snipet:

    ```csharp
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
    ```

    > **Note:** The [LoadColumn](https://docs.microsoft.com/en-us/dotnet/api/microsoft.ml.data.loadcolumnattribute?view=ml-dotnet) attribute specifies your properties' column indices, it is only required when loading data from a file.

1. Finally we are going split the loaded data in two parts: one dataset for training and another one for testing the model. Then we can use the `IDataView` method `Preview` to get a summary of the data and write it out to the console. **Locate the comment that reads `/* Print out data summary */` and replace with the following code**: 

    ```csharp
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
    ```
1. Save the file.
    > **Alert:** Make sure you change the debug setting to `NET Core Launch (console) (04.PolynomialRegression)` in Visual Studio Code like it is explained in the [Introduction to ML.NET](../01.IntroductionToMLNET/README.md)

1. Run the project by pressing F5.
1. Observe the summary about the loaded training data in the console window that launches. Open the file at [data](/data/traffic_by_hour.csv) to review the training data in detail, notice that the first column has the **hour** and we have 6 additional columns with **measures** for each hour.

## Step 2 - Visualize the data

Now lets visualize the data.

1. First we need to define the general settings of the graph, for this purpose we'll use an utility class provided with this code. **Locate the comment that reads `/* Build the graph */` and replace it with the following code**:

    ```csharp
    ChartGeneratorUtil.PlotRegressionChart(new PlotChartGeneratorModel
    {
        Title = "Internet traffic over the day",
        LabelX = "Time of day",
        LabelY = "Internet traffic (Gbps)",
        ImageName = "InternetTrafficOverTheDay.png",
        PointsList = new List<PlotChartPointsList>
        {
            // Add chart points here
        },
        MaxLimitX = 24,
        MaxLimitY = 70,
        DrawRegressionLine = false
    });
    ```

1. Next we'll define the data points that we'll use to plot the graph. For this example we have 6 different sets of hourly measures. **Locate the comment that reads `Add chart points here` and replace it with the following code**:

    ```csharp
    new PlotChartPointsList { Points = ChartGeneratorUtil.GetChartPointsFromFile(TrainDataPath, 0, 1, hasHeader: false).ToList(), Color = CommonConstants.PPLplotColorBlue, PaintDots = false},
    new PlotChartPointsList { Points = ChartGeneratorUtil.GetChartPointsFromFile(TrainDataPath, 0, 2, hasHeader: false).ToList(), Color = CommonConstants.PPLplotColorGreen, PaintDots = false},
    new PlotChartPointsList { Points = ChartGeneratorUtil.GetChartPointsFromFile(TrainDataPath, 0, 3, hasHeader: false).ToList(), Color = CommonConstants.PPLplotColorRed, PaintDots = false},
    new PlotChartPointsList { Points = ChartGeneratorUtil.GetChartPointsFromFile(TrainDataPath, 0, 4, hasHeader: false).ToList(), Color = CommonConstants.PPLplotColorBlack, PaintDots = false},
    new PlotChartPointsList { Points = ChartGeneratorUtil.GetChartPointsFromFile(TrainDataPath, 0, 5, hasHeader: false).ToList(), Color = CommonConstants.PPLplotColorRed2, PaintDots = false},
    new PlotChartPointsList { Points = ChartGeneratorUtil.GetChartPointsFromFile(TrainDataPath, 0, 6, hasHeader: false).ToList(), Color = CommonConstants.PPLplotColorRed3, PaintDots = false},
    // Add average data points
    ```

1. Save the file.
1. Run the project by pressing F5.
1. Observe the graph output.

## Step 3 - Look at the mean values for each hour

This all looks a bit busy, let's see if we can draw out a clearer pattern by taking the average values for each hour.

1. Find the class `TrafficData` at the bottom of the file and add the following code snippet to calculate the average internet traffic:

    ```csharp
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
    ```

1. The next step is to prepare the data to get the average values. **Locate the comment that reads `/* Get the average values */` and replace it with the following code**:

    ```csharp
    // Get an array of the average data points
    var avgPoints = GetAvgChartPointsFromData(mlContext.Data.CreateEnumerable<TrafficData>(trainingData, reuseRowObject: true));
    ```

1. Find the method `GetAvgChartPointsFromData` and replace its content with the following code:

    ```csharp
    return data
        .Select(x => new PlotChartPoint()
        {
            X = x.Time,
            Y = x.AverageMeasure
        });
    ```

1. Find the code where we build the graph. **Locate the comment that reads `// Add average data points` and replace it with the following code**:

    ```csharp
    new PlotChartPointsList { Points = avgPoints.ToList(), Color = CommonConstants.PPLplotColorBlue}
    ```

1. Save the file.
1. Run the project by pressing F5.
1. Observe the graph output.

This alone could help us make a prediction if we wanted to know the expected traffic exactly on the hour.

But, we'll need to be a bit more clever if we want to make a good prediction of times in between.

## Step 4 - Use a model to make a prediction

Let's use the midpoints in between the hours to analyse the relationship between the **time of day** and the **amount of internet traffic**.

We specify a feature *𝑥* (time of day) and our label *𝑦* (the amount of traffic).

1. The first thing you will need to do is to create a pipeline. Polynomial regression is considered to be a special case of multiple linear regression, so in this case we are creating the pipeline using the `PoissonRegression` algorithm, the one we used for [Multiple Linear Regression](../03.MultipleLinearRegression/README.md). Locate the comment that reads `/* Create pipeline */` and replace with the following code:

    ```csharp
    // Create the pipeline
    var pipeline =
        // Specify the Poisson regression trainer
        mlContext.Transforms.Concatenate("Features", "Time", "AverageMeasure")
        .Append(mlContext.Regression.Trainers.PoissonRegression());
    ```
    > **Note:** You can also copy the complete solution from [Complete Examples](../00.CompleteExamples/04.PolynomialRegression/Program.cs)

1. The next step is to train our model by passing our training data to the method `Fit`. Locate the line that reads `/* Train the model */` and replace it with the following code:

    ```csharp
    // Train the model
    var model = pipeline.Fit(trainingData);
    ```

1. Now let's try using this model to make a prediction for a time between 00 and 24. Locate the comment that reads `/* Get the predictions */` and replace it with the following code:

    ```csharp
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
    ```

## Conclusion

And there we have it! You have made a polynomial regression model and used it for analysis! This models gives us a prediction for the level of internet traffic we should expect to see at any given time of day.