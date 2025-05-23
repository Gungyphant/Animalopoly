using ScottPlot;
using static Animalopoly.PlayerClass;
using static Animalopoly.Writing;

namespace Animalopoly
{
    class Graphing
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
            public void GenerateGraph(string filepathForImage)
            {
                Directory.CreateDirectory(filepathForImage[..filepathForImage.LastIndexOf('/')]);
                ScottPlot.Plot graph = new();
                foreach (Player player in points.Keys)
                {
                    List<int> xData = new List<int>();
                    List<int> yData = new List<int>();
                    foreach (Tuple<int, int> point in points[player])
                    {
                        xData.Add(point.Item1);
                        yData.Add(point.Item2);
                    }
                    var plot = graph.Add.Scatter(xData, yData);
                    plot.LegendText = Convert.ToString(player.getName());
                    plot.Color = ConsoleColorToScottPlotColour[colours[player.getId()]];
                }
                graph.ShowLegend();
                graph.SavePng(filepathForImage, 400, 300);
            }
        }
    }
}