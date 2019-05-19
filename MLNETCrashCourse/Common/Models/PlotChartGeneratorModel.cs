using System.Collections.Generic;

namespace myMLNET.Common.Models
{
    public class PlotChartGeneratorModel
    {
        public string ImageName { get; set; } = "PlotChart.png";

        public string Title { get; set; }

        public string LabelX { get; set; } = "References";

        public string LabelY { get; set; } = "Values";

        public double MinLimitX { get; set; } = 0;

        public double MaxLimitX { get; set; } = 100;

        public double MinLimitY { get; set; } = 0;

        public double MaxLimitY { get; set; } = 100;

        public List<PlotChartPointsList> PointsList { get; set; } = new List<PlotChartPointsList>();

        public bool DrawRegressionLine { get; set; } = true;

        public bool DrawVectorMachinesAreas { get; set; } = true;

        public PlotChartPoint DashedPoint { get; set; } = null;

        public PlotChartPointsList RegressionPointsList { get; set; }
    }

    public class PlotChartPointsList
    {
        public int Color { get; set; } = CommonConstants.PPLplotColorBlue;

        public bool PaintDots { get; set; } = true;

        public List<PlotChartPoint> Points { get; set; } = new List<PlotChartPoint>();
    }

    public class PlotChartPoint
    {
        public double X { get; set; }

        public double Y { get; set; }
    }
}
