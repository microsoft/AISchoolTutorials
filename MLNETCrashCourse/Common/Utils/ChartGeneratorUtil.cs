using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using PLplot;
using System.Diagnostics;
using myMLNET.Common.Models;

namespace myMLNET.Common.Utils
{
    public static class ChartGeneratorUtil
    {
        public static void PlotRegressionChart(PlotChartGeneratorModel generationModel)
        {
            using (var pl = new PLStream())
            {
                pl.sdev("pngcairo");
                pl.sfnam(generationModel.ImageName);

                // use white background with black foreground
                pl.spal0("cmap0_alternate.pal");

                // Initialize plplot
                pl.init();

                // set axis limits
                pl.env(generationModel.MinLimitX, generationModel.MaxLimitX, generationModel.MinLimitY, generationModel.MaxLimitY,
                  AxesScale.Independent, AxisBox.CustomXYBoxTicksLabels);

                // Set scaling for mail title text 125% size of default
                //pl.schr(0, 1.25);

                // The main title
                pl.lab(generationModel.LabelX, generationModel.LabelY, generationModel.Title);

                // plot using different colors
                // see http://plplot.sourceforge.net/examples.php?demo=02 for palette indices
                pl.col0(1);

                // This code is the symbol to paint
                char code = (char)9;

                double yTotal = 0;
                double xTotal = 0;
                double xyMultiTotal = 0;
                double xSquareTotal = 0;

                var totalNumber = 0;

                generationModel.PointsList.ForEach(pointsList =>
                {
                    double y0 = 0;
                    double x0 = 0;
                    totalNumber += pointsList.Points.Count();

                    // plot using other color
                    pl.col0(pointsList.Color);
                    pointsList.Points.ForEach(point =>
                    {
                        var x = new double[1];
                        var y = new double[1];

                        x[0] = point.X;
                        y[0] = point.Y;

                        if (pointsList.PaintDots)
                        {
                            // Paint a dot
                            pl.poin(x, y, code);
                        }
                        else
                        {
                            if (!generationModel.DrawRegressionLine)
                            {
                                // Draw lines between points
                                pl.join(x0, y0, point.X, point.Y);

                                x0 = point.X;
                                y0 = point.Y;
                            }
                        }
                        

                        xTotal += point.X;
                        yTotal += point.Y;
                        xyMultiTotal += point.X * point.Y;
                        xSquareTotal += point.X * point.X;
                    });
                });

                // Regression Line calculation explanation:
                // https://www.khanacademy.org/math/statistics-probability/describing-relationships-quantitative-data/more-on-regression/v/regression-line-example

                if (generationModel.DrawRegressionLine)
                {
                    if (generationModel.RegressionPointsList != null)
                    {
                        var xArray = generationModel.RegressionPointsList.Points.Select(dp => dp.X).ToArray();
                        var yArray = generationModel.RegressionPointsList.Points.Select(dp => dp.Y).ToArray();

                        pl.col0(generationModel.RegressionPointsList.Color);
                        pl.line(xArray, yArray);
                    }
                    else
                    {
                        double minY = yTotal / totalNumber;
                        double minX = xTotal / totalNumber;
                        double minXY = xyMultiTotal / totalNumber;
                        double minXsquare = xSquareTotal / totalNumber;

                        double m = ((minX * minY) - minXY) / ((minX * minX) - minXsquare);
                        double b = minY - (m * minX);
                        double x1 = generationModel.MinLimitX;
                        //Function for Y1 in the line
                        double y1 = (m * x1) + b;

                        double x2 = generationModel.MaxLimitX;
                        //Function for Y2 in the line
                        double y2 = (m * x2) + b;

                        var xArray = new double[2];
                        var yArray = new double[2];
                        xArray[0] = x1;
                        yArray[0] = y1;
                        xArray[1] = x2;
                        yArray[1] = y2;

                        pl.col0(4);
                        pl.line(xArray, yArray);
                    }
                }

                if (generationModel.DashedPoint != null)
                {
                    pl.col0(CommonConstants.PPLplotColorGreen);
                    pl.width(2);
                    // Horizontal Line should go from (0,DashedPoint.Y) to (DashedPoint.X, DashedPoint.Y)
                    DrawDashedLined(pl, 0, generationModel.DashedPoint.Y, generationModel.DashedPoint.X, generationModel.DashedPoint.Y);
                    // Vertical Line should go from (DashedPoint.X,0) to (DashedPoint.X, DashedPoint.Y)
                    DrawDashedLined(pl, generationModel.DashedPoint.X, 0, generationModel.DashedPoint.X, generationModel.DashedPoint.Y);
                }

                // end page (writes output to disk)
                pl.eop();

                // output version of PLplot
                pl.gver(out var verText);

            } // the pl object is disposed here

            // Open Chart File In Microsoft Photos App (Or default app, like browser for .svg)
            Console.WriteLine("Showing chart...");
            var p = new Process();
            string chartFileNamePath = @".\" + generationModel.ImageName;
            p.StartInfo = new ProcessStartInfo(chartFileNamePath)
            {
                UseShellExecute = true
            };
            p.Start();
        }

        public static IEnumerable<PlotChartPoint> GetChartPointsFromFile(
          string dataLocation,
          int xColumn,
          int yColumn,
          int filterByColumn = -1,
          int filterByValue = 0,
          bool transformXAsPercentage = false,
          bool transformYAsPercentage = false,
          int numMaxRecords = 0,
          bool hasHeader = true)
        {
            var lines = hasHeader ? File.ReadAllLines(dataLocation).Skip(1) : File.ReadAllLines(dataLocation);
            var records = lines
                .Select(x => x.Split('	'))
                .Where(x =>
                {
                  return filterByColumn == -1 || int.Parse(x[filterByColumn]) == filterByValue;
                })
                .Select(x => new PlotChartPoint()
                {
                    X = transformXAsPercentage ? float.Parse(x[xColumn]) * 100: float.Parse(x[xColumn]),
                    Y = transformYAsPercentage ? float.Parse(x[yColumn]) * 100 : float.Parse(x[yColumn])
                });

            return numMaxRecords > 0 ? records.Take(numMaxRecords) : records;
        }

        public static int GetMaxColumnValueFromFile(string dataLocation, int column, bool transformAsPercentage = false, bool hasHeader = true)
        {
            var lines = hasHeader ? File.ReadAllLines(dataLocation).Skip(1) : File.ReadAllLines(dataLocation);
            var maxFloatValue = lines
                  .Select(x => x.Split('	'))
                  .OrderByDescending(x => float.Parse(x[column]))
                  .Select(x => transformAsPercentage ? float.Parse(x[column]) * 100 : float.Parse(x[column]))
                  .First();
            return Convert.ToInt32(maxFloatValue);
        }

        public static int GetMinColumnValueFromFile(string dataLocation, int column, bool transformAsPercentage = false)
        {
            var maxFloatValue = File.ReadAllLines(dataLocation)
                  .Skip(1)
                  .Select(x => x.Split('	'))
                  .OrderBy(x => float.Parse(x[column]))
                  .Select(x => transformAsPercentage ? float.Parse(x[column]) * 100 : float.Parse(x[column]))
                  .First();
            return Convert.ToInt32(maxFloatValue);
        }

        public static int GetAvgColumnValueFromFile(string dataLocation, int column, bool transformAsPercentage = false)
        {
          var maxFloatValue = File.ReadAllLines(dataLocation)
                .Skip(1)
                .Select(x => x.Split('	'))
                .OrderBy(x => float.Parse(x[column]))
                .Select(x => transformAsPercentage ? float.Parse(x[column]) * 100 : float.Parse(x[column]))
                .First();
          return Convert.ToInt32(maxFloatValue);
        }

        private static void DrawDashedLined(PLStream pl, double x1, double y1, double x2, double y2)
        {
            var xArray = new double[2];
            var yArray = new double[2];
            xArray[0] = x1;
            yArray[0] = y1;
            xArray[1] = x2;
            yArray[1] = y2;
            pl.line(xArray, yArray);
        }
    }
}