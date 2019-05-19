# Support Vector Machines

Support vector machines (SVMs) let us predict categories. This exercise will demonstrate a simple support vector machine that can predict a category from a small number of features.

Our problem is that we want to be able to categorise which type of tree an new specimen belongs to. To do this, we will use features of three different types of trees to train an SVM.

> **Note:** Make sure that in the solution explorer you right click on **06.SupportVectorMachines** and select **Set as StartUp project**

## Step 1 - Looking at our data

First, we will take a look at the raw data first to see what features we have:

1. Open `Program.cs` in this project by double clicking on it in the explorer.

    ![Open project](images/open-program.png)

1. First we need to define the path of the data file that we are going to use in the exercises. **Locate the comment that reads `/* Add data path code */` and replace with the following code**:

    ```csharp
    private static readonly string TrainDataPath = Path.Combine(Environment.CurrentDirectory, "data", "trees.csv");
    ```

1. The next step is to add the following code the create the [MLContext](https://docs.microsoft.com/en-us/dotnet/api/microsoft.ml.mlcontext?view=ml-dotnet) and the [TextLoader](https://docs.microsoft.com/en-us/dotnet/api/microsoft.ml.data.textloader?view=ml-dotnet) to read the training data, this is explained with more detail in the [Polynomial Regression](../04.PolynomialRegression/README.md).  **Locate the comment that reads `/* Add ML Context and TextLoader */` and replace with the following code**:

    ```csharp
    var mlContext = new MLContext();

    var trainingData = mlContext.Data.LoadFromTextFile<TreeInput>(TrainDataPath, separatorChar: '\t', hasHeader: true);
    ```

    > **Note:** You can also copy the complete solution from [Complete Examples](../00.CompleteExamples/06.SupportVectorMachines/Program.cs).

1. Finally we are going load to data into an `IDataView`. Let's use the `IDataView` method `Preview` to get a summary of the data and write it out to the console. **Locate the comment that reads `/* Print out data summary */` and replace with the following code**:

    ```csharp
    var preview = trainingData.Preview();
    Console.WriteLine($"******************************************");
    Console.WriteLine($"Loaded training data: {preview.ToString()}");
    Console.WriteLine($"******************************************");
    ```
1. Open the file at [data](/data/trees.txt) to review the training data in detail. It looks like we have four features (leaf_width, leaf_length, trunk_girth, trunk_height) and one label (tree_type). Let's plot it.
1. Locate the comment that reads `/* Build the classification plot for leaf features graph */` and replace it with the following code:
    ```csharp
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
    ```
1. Locate the comment that reads `/* Build the classification plot for trunk features graph */` and replace it with the following code:
    ```csharp
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
    ```
1. Save the file.
    > **Alert:** Make sure you change the debug setting to `NET Core Launch (console) (06.SupportVectorMachines)` in Visual Studio Code like it is explained in the [Introduction to ML.NET](../01.IntroductionToMLNET/README.md)

1. Run the project by pressing F5.

## Step 2 - Building a SVM model and running a prediction

Lets make a support vector machine. SVMs are a family algorithm for classification, regression, transduction, novelty detection and semi-supervised learning. The syntax for a support vector machine is composed by **features** and **lables**. Your features set will be called **trainX** and your labels set will be called **trainY**.

1. Let's first the following code to create a pipeline. In this case we are creating a pipeline using the `LinearSupportVectorMachines` trainer and we are only going to use the leaf features. Locate the comment that reads `/* Create pipeline */` and replace with the following code:

    ```csharp
    var pipeline =
        // Specify the support vector machine trainer
        mlContext.Transforms.Concatenate("Features", "LeafWidth", "LeafLength")
        .Append(mlContext.BinaryClassification.Trainers.LinearSupportVectorMachines());
    ```

1. The next step is to train our model by passing our training data to the method `Fit`. Locate the line that reads `/* Train the model */` and replace it with the following code:

    ```csharp
    // Train the model
    var model = pipeline.Fit(trainingData);
    ```

1. Let's use the model to get a prediction using the first two features, the leaf features. In this case we do this by calling the method `CreatePredictionEngine` to generate our final prediction engine. Locate the comment that reads `/* Get the predictions */` and replace it with the following code:

    ```csharp
    var predictionEngine = model.CreatePredictionEngine<TreeInput, TreeOutput>(mlContext);

    // Obtain the prediction
    var prediction = predictionEngine.Predict(new TreeInput
    {
        LeafWidth = 5.13E+00f,
        LeafLength = 6.18E+00f,
    });

    Console.WriteLine($"*************************************");
    Console.WriteLine("Tree type {0}", prediction.TreeType ? "0" : "1");
    Console.WriteLine($"Score: {prediction.Score}");
    Console.WriteLine($"Probability: {prediction.Probability}");
    Console.WriteLine($"*************************************");
    ```

1. Alright, that's the model done. Now locate the comment that reads `/* Build the SVM plot for leaf features graph */` and replace it with the following code:
    ```csharp
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
    ```
1. Locate the comment that reads `/* Build the SVM plot for trunk features graph */` and replace it with the following code:
    ```csharp
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
    ```
1. Save the file.
1. Run the project by pressing F5.
1. The graph shows three colored zones that the SVM has chosen to group the datapoints in. Color, here, means type of tree. As we can see, the zones correspond reasonably well with the actual tree types of our training data. This means that the SVM can group, for its training data, quite well calculate tree type based on leaf features.
1. Click on the **Stop** button to stop debugging. Now let's do the same using trunk features.
1. First we need to include all the variables in our prediction. Look for the `pipeline` variable and replace the `mlContext.Transforms.Concatenate` line with the following code:

    ```csharp
    mlContext.Transforms.Concatenate("Features", "LeafWidth", "LeafLength", "TrunkGirth", "TrunkHeight")
    ```

1. Find the `Obtain the prediction` comment and add the following code to the **TreeInput** content:

    ```csharp
    TrunkGirth = 8.26E+00f,
    TrunkHeight = 8.74E+00f
    ```

1. Save the file.
1. Run the project by pressing F5 and review the graph output.

## Conclusion

And that's it! You've made a simple support vector machine that can predict the type of tree based on the leaf and trunk measurements!
