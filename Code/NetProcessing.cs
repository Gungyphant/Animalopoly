using NetProcessing;
using System.Linq;
using System.Xml.Linq;
using static Animalopoly.Code.CardClass;
using static Animalopoly.Code.CommandLineInterface;
using static Animalopoly.Code.Graphing;
using static Animalopoly.Code.NetProcessingUI;
using static Animalopoly.Code.PlayerClass;
using static Animalopoly.Code.Program;
using static Animalopoly.Code.TileClasses;
using static Animalopoly.Code.Writing;

namespace Animalopoly.Code
{
    class NetProcessingUI : Sketch
    {
        //static string[] tileColours = ["#0000ff", "#00ff00", "#ff0000", "#ffff00"];

        const int TILEWIDTH = 100;
        const int TILEHEIGHT = 100;
        const int ANIMALNAMESIZE = 15;
        const int SETNAMESIZE = 10;

        const int DIEWIDTH = 50;
        const int DIEHEIGHT = 50;

        readonly Parameter[] HORIZONTALALIGNS = [LEFT, RIGHT, LEFT, RIGHT];
        readonly Parameter[] VERTICALALIGNS = [TOP, TOP, BOTTOM, BOTTOM];
        readonly int[] HORIZONTALOFFSETS = [2, TILEWIDTH - 2, 2, TILEWIDTH - 2];
        readonly int[] VERTICALOFFSETS = [0, 0, TILEHEIGHT, TILEHEIGHT];

        public override void Setup()
        { // Initialises Net.Processing
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
        public static string HexColour(int i)
        { // Gets the Hex code for player i's colour
            return $"#{System.Drawing.Color.FromName(colourNames[i]).ToArgb() & 0xFFFFFF:X6}";
        }

        private static void Circle(int x, int y, int extent) // In newer version of Processing, but not in Net.Processing
        {
            Ellipse(x, y, extent, extent);
        }

        // {{top left, top center, top right}, {middle left, ... bottom center, bottom right}} for each number
        // technically, top center and bottom center are unnecessary as they are false for all faces, however this allows easy extensibility for alternate faces
        readonly bool[][,] PIPS = new bool[][,]
        {
                new bool[,] { { false, false, false },  {false,  true, false },  { false, false, false } },
                new bool[,] { {  true, false, false },  {false, false, false },  { false, false,  true } },
                new bool[,] { {  true, false, false },  {false,  true, false },  { false, false,  true } },
                new bool[,] { {  true, false,  true },  {false, false, false },  {  true, false,  true } },
                new bool[,] { {  true, false,  true },  {false,  true, false },  {  true, false,  true } },
                new bool[,] { {  true, false,  true },  { true, false,  true },  {  true, false,  true } },
        };

        private void Die(int x, int y, int number)
        { // Draws a Die showing number centered on (x, y)
            if (number > 0) // 0 = no dice shown
            {
                Fill("#FFFFFF");
                RectMode(CENTER);

                Rect(x, y, DIEWIDTH, DIEHEIGHT, 10);
                for (int pip_x = 0; pip_x < 3; pip_x++)
                {
                    int pip_x_offset = DIEWIDTH * (pip_x - 1) / 4;
                    for (int pip_y = 0; pip_y < 3; pip_y++)
                    {
                        if (PIPS[number - 1][pip_y, pip_x])
                        {
                            int pip_y_offset = DIEHEIGHT * (pip_y - 1) / 4;
                            Fill("#000000");
                            Circle(x + pip_x_offset, y + pip_y_offset, DIEHEIGHT / 10);
                        }
                    }
                }

                RectMode(CORNER);
            }
        }

        private void OutlinedText(string text, int x, int y, string textColour, string outlineColour = "#000000", int thickness = 1)
        { // Draws text with a black outline
            // Outline
            Fill(outlineColour);
            for (int x_offset = -thickness; x_offset <= thickness; x_offset++)
            {
                for (int y_offset = -thickness; y_offset <= thickness; y_offset++)
                {
                    Text(text, x + x_offset, y + y_offset);
                }
            }
            // Text
            Fill(textColour);
            Text(text, x, y);
        }

        public override void Draw()
        { // Called each frame to generate the GUI
            if (players != null) // Crash prevention
            {
                //HideConsole();
                int x = 0;
                int y = 0;
                int direction = 0;
                // Tiles
                foreach ((int id, Tile tile) in locations.Select((value, index) => (index, value)))
                {
                    // Draw tile
                    bool tile_is_animal = tile is Animal;
                    Animal? animal = tile as Animal;
                    // Tile square
                    if (tile_is_animal)
                    {
                        Fill(animal.GetSetColour());
                    }
                    else
                    {
                        Fill("#FFFFFF");
                    }
                    Rect(x, y, TILEWIDTH, TILEHEIGHT);

                    // Tile name
                    string animalName = tile.GetName();
                    if (animalName == "") // There are flickers where the name & set disappear; see #12
                    {
                        throw new Exception("Got \"\" for the name of an Animal when drawing GUI");
                    }
                    int animal_name_x = x + TILEWIDTH / 2;
                    int animal_name_y = y + TILEHEIGHT / 2 - (ANIMALNAMESIZE / 2 + SETNAMESIZE / 2) / 2; // Offset it upwards ((0, 0) is top-left so subtracting is up) so that the midpoint between it and the set will be the center of the tile
                    TextSize(ANIMALNAMESIZE);
                    TextAlign(CENTER, CENTER); // TOOD: why not just change textalign?

                    // Tile name
                    string animalNameColour;
                    if (tile_is_animal && animal.GetOwner() is not null)
                    {
                        animalNameColour = HexColour(animal.GetOwner().GetId());
                    }
                    else
                    {
                        animalNameColour = "#FFFFFF";
                    }
                    OutlinedText(animalName, animal_name_x, animal_name_y, animalNameColour);

                    // Tile set
                    if (tile_is_animal)
                    {
                        TextSize(SETNAMESIZE);
                        Fill("#000000", 192);
                        Text(animal.GetSet(), x + TILEWIDTH / 2, y + TILEHEIGHT / 2 + (ANIMALNAMESIZE / 2 + SETNAMESIZE / 2) / 2);
                    }

                    // Tile ID
                    TextAlign(CENTER, BOTTOM);
                    Text(Convert.ToString(id), x + TILEWIDTH / 2, y + TILEHEIGHT - 2);


                    // Players
                    TextSize(25);
                    for (int i = 0; i < players.Length; i++)
                    {
                        Player player = players[i];
                        if (player != null && player.GetBankruptStatus() < 2 && player.GetPos() == id)
                        {
                            TextAlign(HORIZONTALALIGNS[i], VERTICALALIGNS[i]);

                            string name = Convert.ToString(players[i].GetName());
                            int name_x = x + HORIZONTALOFFSETS[i];
                            int name_y = y + VERTICALOFFSETS[i];

                            OutlinedText(name, name_x, name_y, HexColour(i));
                        }
                    }

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

                        // Turn
                        direction += 1;

                        // Do the correct movement
                        x += DIRECTIONS[direction].Item1;
                        y += DIRECTIONS[direction].Item2;
                    }
                }

                // Dice
                (int shownDie1, int shownDie2) = GetDice();
                Die(4 * TILEWIDTH + -TILEWIDTH / 2, 7 * TILEHEIGHT / 2, shownDie1);
                Die(4 * TILEWIDTH + TILEWIDTH / 2, 7 * TILEHEIGHT / 2, shownDie2);
            }
        }
    }
}