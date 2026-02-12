using MsgPack.Serialization;
using ScottPlot;
using ScottPlot.Plottables;
using System.IO;
using static Animalopoly.Code.PlayerClass;
using static Animalopoly.Code.Writing;

namespace Animalopoly.Code
{
    public class Graphing
    {
        public static Dictionary<ConsoleColor, ScottPlot.Color> ConsoleColorToScottPlotColour = new Dictionary<ConsoleColor, ScottPlot.Color>()
        {
            {ConsoleColor.Blue, Colors.Blue},
            {ConsoleColor.Red, Colors.Red},
            {ConsoleColor.Green, Colors.Lime},
            {ConsoleColor.Yellow, Colors.Yellow},
            {ConsoleColor.White, Colors.White},
        };
        public class Grapher
        {
            [MessagePackMember(0)]
            private Dictionary<Player, List<Tuple<int, int>>> points;
            public Grapher()
            {
                points = new Dictionary<Player, List<Tuple<int, int>>>(); // Player: [(x, y)]
            }
            public void LogMoney(Player player, int turn, int money)
            {
                if (!points.ContainsKey(player))
                {
                    points[player] = new List<Tuple<int, int>>();
                }
                points[player].Add(new Tuple<int, int>(turn, money));
            }
            public void GenerateGraph(string filepathForImage, bool quiet = false)
            {
                if (!quiet)
                {
                    Write("Generating money graph...");
                }
                Directory.CreateDirectory(filepathForImage[..filepathForImage.LastIndexOf('/')]);
                ScottPlot.Plot graph = new();
                int maxX = 0;
                int maxY = 0;
                int minY = 0;
                foreach (Player player in points.Keys)
                {
                    List<int> xData = new List<int>();
                    List<int> yData = new List<int>();
                    foreach (Tuple<int, int> point in points[player])
                    {
                        int x = point.Item1;
                        int y = point.Item2;
                        if (x > maxX) maxX = x;
                        if (y > maxY) maxY = y;
                        if (y < minY) minY = y;
                        xData.Add(x);
                        yData.Add(y);
                    }
                    Scatter plot = graph.Add.Scatter(xData, yData);
                    plot.LegendText = Convert.ToString(player.GetName());
                    plot.Color = ConsoleColorToScottPlotColour[colours[player.GetId()]];
                    plot.LineWidth = 5;
                }
                // Generate dashed horizontal lines
                foreach ((int y, string name) in new Tuple<int, string>[2] { new Tuple<int, string>(0, "Bankrupt"), new Tuple<int, string>(3750, "Starting money") })
                {
                    int[] yData = Enumerable.Repeat(y, maxX).ToArray();
                    Signal line = graph.Add.Signal(yData);
                    line.LegendText = name;
                    line.LinePattern = LinePattern.DenselyDashed;
                    line.LineWidth = 5;
                }
                graph.ShowLegend();
                graph.Axes.Bottom.TickGenerator = new ScottPlot.TickGenerators.NumericFixedInterval(1);
                graph.Axes.Left.TickGenerator = new ScottPlot.TickGenerators.NumericFixedInterval(375);
                graph.Axes.SetLimits(0, maxX, minY, maxY);
                graph.SavePng(filepathForImage, 1920, 1080); // 1080p
                if (!quiet)
                {
                    WriteLine($"{new string('\b', 100)}Money graph saved to {filepathForImage}");
                }
            }
        }
    }
}