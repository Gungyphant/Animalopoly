using NetProcessing;
using static Animalopoly.Code.CardClass;
using static Animalopoly.Code.CommandLineInterface;
using static Animalopoly.Code.Graphing;
using static Animalopoly.Code.NetProcessingUI;
using static Animalopoly.Code.PlayerClass;
using static Animalopoly.Code.TileClasses;
using static Animalopoly.Code.Writing;

namespace Animalopoly.Code
{
    class NetProcessingUI : Sketch
    {
        static string[] tileColours = ["#0000ff", "#ff0000", "#00ff00", "#ffff00"];

        const int TILEWIDTH = 100;
        const int TILEHEIGHT = 100;

        public override void Setup()
        {
            if (8 * TILEWIDTH < 100 || 7 * TILEHEIGHT < 100)
            {
                throw new Exception($"Size cannot set window sizes less than 100px; tried to call Size({8 * TILEWIDTH}, {7 * TILEHEIGHT})");
            }
            Size(8 * TILEWIDTH, 7 * TILEHEIGHT);
            Background(255);
        }

        readonly static Tuple<int, int>[] DIRECTIONS = 
        [
            new Tuple<int, int>(TILEWIDTH, 0),   // Right
            new Tuple<int, int>(0, TILEHEIGHT),  // Down
            new Tuple<int, int>(-TILEWIDTH, 0),  // Left
            new Tuple<int, int>(0, -TILEHEIGHT), // Up
        ];
        public override void Draw()
        {
            //HideConsole();
            int x = 0;
            int y = 0;
            int direction = 0;
            foreach (Animal animal in locations)
            {
                // Draw tile
                // Tile square
                Fill(animal.GetSetColour());
                Rect(x, y, TILEWIDTH, TILEHEIGHT);
                // Tile name
                if (animal.GetOwner() is null)
                {
                    Fill(0, 0, 0);
                }
                else
                {
                    Fill(tileColours[animal.GetOwner().GetId()]);
                }
                TextSize(15);
                TextAlign(CENTER, CENTER);
                if (animal.GetName() == "") // There are inexpilcable flickers where the name & set disappear; see #12
                {
                    throw new Exception("Got \"\" for the name of an Animal when drawing GUI");
                }
                Text(animal.GetName(), x + TILEWIDTH/2, y + TILEHEIGHT/2 - (15/2 + 10/2)/2);
                // Tile set
                TextSize(10);
                Fill("#000000", 192);
                Text(animal.GetSet(), x + TILEWIDTH / 2, y + TILEHEIGHT / 2 + (15/2 + 10/2)/2);

                // Move to next position
                x += DIRECTIONS[direction].Item1;
                y += DIRECTIONS[direction].Item2;

                // Check if gone too far
                if (
                    x >= 8 * TILEWIDTH  // Right edge
                    ||
                    x < 0               // Left edge
                    ||
                    y >= 7 * TILEHEIGHT // Bottom edge
                    ||
                    y < 0               // Top edge
                    )
                {
                    // Move back
                    x -= DIRECTIONS[direction].Item1;
                    y -= DIRECTIONS[direction].Item2;

                    direction += 1;
                    // Do the correct movement
                    x += DIRECTIONS[direction].Item1;
                    y += DIRECTIONS[direction].Item2;
                }
            }
        }
    }
}