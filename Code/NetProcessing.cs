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
        PImage img;
        int smallPoint, largePoint;
        const int TILEWIDTH = 100;
        const int TILEHEIGHT = 100;

        public override void Setup()
        {
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
            int x = 0;
            int y = 0;
            int direction = 0;
            foreach (Animal animal in locations)
            {
                // Draw tile
                Rect(x, y, TILEWIDTH, TILEHEIGHT);

                // Move to next position
                x += DIRECTIONS[direction].Item1;
                y += DIRECTIONS[direction].Item2;

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
                    x -= DIRECTIONS[direction].Item1;
                    y -= DIRECTIONS[direction].Item2;
                    direction += 1;
                    x += DIRECTIONS[direction].Item1;
                    y += DIRECTIONS[direction].Item2;
                }
            }
        }
    }
}