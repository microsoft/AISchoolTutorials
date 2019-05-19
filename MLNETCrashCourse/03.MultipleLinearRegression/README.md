# Multiple Linear Regression

> **Note:** If you have not already setup your environment please refer to the
> [Introduction to ML.NET Readme here](../01.IntroductionToMLNET/README.md)

From the previous exercise, we know that customers are happier with chocolate bars that are large and have high amounts of cocoa. Customers may feel differently when they have to pay for these bars though.

In this exercise, we will try to find the chocolate bar that best suits customers, taking into account the cocoa content, size, and price.

> **Note:** Make sure that in the solution explorer you right click on **03.MultipleLinearRegression** and select **Set as StartUp project**

## Step 1 - Looking at our data

Firstly, lets have a look at our data. The data is from survey of how happy customers were with chocolate bars they purchased.

1. Open `Program.cs` in this project by double clicking on it in the explorer.

    ![Open project](images/open-program.png)

1. First we need to define the path of the data file that we are going to use in the exercises. **Locate the comment that reads `/* Add data path code */` and replace with the following code**:

    ```csharp
    private static readonly string TrainDataPath = Path.Combine(Environment.CurrentDirectory, "data", "chocolate-data-multiple-linear-regression.txt");
    ```

1. The next step is to add the following code the create the [MLContext](https://docs.microsoft.com/en-us/dotnet/api/microsoft.ml.mlcontext?view=ml-dotnet) and the [TextLoader](https://docs.microsoft.com/en-us/dotnet/api/microsoft.ml.data.textloader?view=ml-dotnet) to read the training data, this is explained with more detail in the [Linear Regression](../02.LinearRegression/README.md).  **Locate the comment that reads `/* Add ML Context and TextLoader */` and replace with the following code**:

    ```csharp
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
    ```

    > **Note:** You can also copy the complete solution from [Complete Examples](../00.CompleteExamples/03.MultipleLinearRegression/Program.cs).

1. Finally we are going load the data into an `IDataView` using the `TextLoader` we created. Then we can use the `IDataView` method `Preview` get a summary of the data and write it out to the console. **Locate the comment that reads `/* Print out data summary */` and replace with the following code**:

    ```csharp
    var trainingData = reader.Load(TrainDataPath);
    var preview = trainingData.Preview();
    Console.WriteLine($"******************************************");
    Console.WriteLine($"Loaded training data: {preview.ToString()}");
    Console.WriteLine($"******************************************");
    ```
1. Save the file.
    > **Alert:** Make sure you change the debug setting to `NET Core Launch (console) (03.MultiLinearRegression)` in Visual Studio Code like it is explained in the [Introduction to ML.NET](../01.IntroductionToMLNET/README.md)

1. Run the project by pressing F5.
1. Observe the summary about the loaded training data in the console window that launches. Open the file at [data](/data/chocolate-data-multiple-linear-regression.txt) to review the training data in detail.

## Step 2 - Perform a simple linear regression

Previously we found that customers like a high percentage of cocoa and heavier bars of chocolate. Large bars of chocolate cost more money, though, which might make customers less inclined to purchase them.

Let's perform a simple linear regression to see the relationship between **customer happiness** and chocolate bar **weight** when the cost of the chocolate was taken into consideration for the survey.

1. The first thing you will need to do is to create a pipeline. In this case we are creating a pipeline using the `PoissonRegression` algorithm, which is a type of Linear regression. For this pipeline we'll only consider the **Weight** feature. Locate the comment that reads `/* Create pipeline */` and replace with the following code:

    ```csharp
    var pipeline =
        // Features to include in the prediction
        mlContext.Transforms.Concatenate("Features", "Weight")
        // Specify the regression trainer
        .Append(mlContext.Regression.Trainers.PoissonRegression());
    ```
    > **Note:** You can also copy the complete solution from [Complete Examples](../00.CompleteExamples/03.MultipleLinearRegression/Program.cs)

1. The next step is to train our model by passing our training data to the method `Fit`. Locate the line that reads `/* Train the model */` and replace it with the following code:

    ```csharp
    // Train the model
    var model = pipeline.Fit(trainingData);
    ```

1. The final step is to use the model to get a the regression coefficient and the slope. We will use these variables to build a graphic and have a better visualization of the data. Locate the comment that reads `/* Get the graph data */` and replace it with the following code:

    ```csharp
    // The model's feature weight coefficients
    var regressionModel = model.LastTransformer.Model;
    var weights = regressionModel.Weights;
    var intercept = regressionModel.Bias;
    ```

1. Locate the comment that reads `/* Build the graph */` and replace it with the following code:

    ```csharp
    var featureColumn = reader.GetOutputSchema().GetColumnOrNull("Weight").Value;
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
    ```
1. Make sure to save the file.
1. Run the project by pressing F5.
1. Let's analyze the graphic output. We can see that customer happiness still increases with larger bars of chocolate. However, many data points (blue) are a long way from our trendline (red). This means that this line doesn't describe the data very well. It is likely that there are other features of the chocolate that are influencing customer happiness.
1. Repeat the above exercise, using `CocoaPercent` in place of `Weight` in the pipeline.
1. Find the `featureColumn` variable and replace the line with the following code:
    ```csharp
    var featureColumn = reader.GetOutputSchema().GetColumnOrNull("CocoaPercent").Value;
    ```
1. Run the code again. You should see a similar trend.

## Step 3 - Check the quality metrics of our prediction model

We can check how well our data fit by getting the R² values. These range between 0 - 1, where 1 is a perfect fit. What is a 'good' or 'bad' fit depends on several things, but for our purposes here numbers below ~0.3 will mean a poor fit.

Our linear model is saved under the name lm.

The linear model for simple linear regression we just ran, *"weight vs. customer happiness"*, is saved under lm. Let's determine the R² value of this model.

1. The first thing you will need to do is to apply the model to the DataView to make *predictions* and `Evaluate` them using the **Regression** trainer provided by ML.NET. Locate the line that reads `/* Calculate metrics */` and replace with the following code:

    ```csharp
    // Compute the quality metrics of our prediction model
    var predictions = model.Transform(trainingData);
    PrintMetrics(mlContext, predictions);
    ```

1. Finally you can get evaluation metrics to check your model, in this case we are going to analyze the `RSquared` (R²) value and write it out to the console. Find the method `PrintMetrics` and add the following code:

    ```csharp
    // Compute the quality metrics of our prediction model
    var metrics = mlContext.Regression.Evaluate(predictions, "Label", "Score");

    Console.WriteLine();
    Console.WriteLine($"Model quality metrics evaluation");
    // RSquared is another evaluation metric of the regression models. RSquared takes values between 0 and 1. The closer its value is to 1, the better the model is
    Console.WriteLine($"R2 Score: {metrics.RSquared:0.##}");
    ```

1. Save the file.
1. Run the project by pressing F5.
1. Observe the details about the loaded training data in the console window that launches. We have a value below 0.3, which means it is a poor fit.


## Step 4 - Compare each feature using Multiple linear regression

The problem with our chocolate bar survey is that the chocolate bar variables aren't controlled; cost, bar weight, and cocoa percent are different for every chocolate bar.

We want to see the relationship between cocoa content and customer happiness, but cost and block weight are also influencing customer happiness.

We could run another survey, giving away chocolate bars that are all the same weight for free (i.e. weight and cost are constant), and ask people how happy they are with the chocolate bar given varying percentages of cocoa. However, this would be expensive and time consuming.

**Alternatively, we can use multiple linear regression.** Multiple linear regression can give us the relationship between each feature and customer happiness. These are provided as coefficients (slopes). Positive numbers indicate a positive relationship (i.e. customer happiness increases as this feature increases), negative numbers indicate a negative relationship (customer happiness decreases as this feature increases). Unlike simple linear regression, these relationships should be independent. That means that our relationship between cocoa content and customer happiness should not be influenced strongly by bar weight or cost.

1. First we need to include all the variables in our prediction. Look for the `pipeline` variable and replace the `mlContext.Transforms.Concatenate` line with the following code:

    ```csharp
    mlContext.Transforms.Concatenate("Features", "Weight", "CocoaPercent", "Cost")
    ```

    > **Note:** You can also copy the complete solution from [Complete Examples](../00.CompleteExamples/03.MultipleLinearRegression/Program.cs).

1. Look for the line `var intercept = regressionModel.Bias;` and add the the following code snippet **after** it:

    ```csharp
    Console.WriteLine($"Coefficients: Weight={weights[0]:0.##}, CocoaPercent={weights[1]:0.##}, Cost={weights[2]:0.##}");
    ```
1. Save the file.
1. Run the project by pressing F5.
1. Observe the details about the loaded training data in the console window that launches. Let's inspect the output:
    * We can see that **Weight** and **CocoaPercent** are positive numbers, telling us they both independently increase customer happiness, but also that **Cost** decreases it.
    * The R² value is also much higher than before. This means model fits much better now.


Note how this is different to our earlier work with simple linear regression. With that, we assumed a large bar with very high amount of cocoa was what customers would want.
