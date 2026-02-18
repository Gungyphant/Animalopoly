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

        Parameter[] HORIZONTALALIGNS = [LEFT, RIGHT, LEFT, RIGHT];
        Parameter[] VERTICALALIGNS = [TOP, TOP, BOTTOM, BOTTOM];
        int[] HORIZONTALOFFSETS = [2, TILEWIDTH - 2, 2, TILEWIDTH - 2];
        int[] VERTICALOFFSETS = [0, 0, TILEHEIGHT, TILEHEIGHT];

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
        public string HexColour(int i)
        {
            return $"#{System.Drawing.Color.FromName(colourNames[i]).ToArgb() & 0xFFFFFF:X6}";
        }
        public override void Draw()
        {
            if (players != null)
            {
                //HideConsole();
                int x = 0;
                int y = 0;
                int direction = 0;
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
                    if (animalName == "") // There are inexplicable flickers where the name & set disappear; see #12
                    {
                        throw new Exception("Got \"\" for the name of an Animal when drawing GUI");
                    }
                    int animal_name_x = x + TILEWIDTH / 2;
                    int animal_name_y = y + TILEHEIGHT / 2 - (ANIMALNAMESIZE / 2 + SETNAMESIZE / 2) / 2; // Offset it upwards ((0, 0) is top-left so subtracting is up) so that the midpoint between it and the set will be the center of the tile
                    TextSize(ANIMALNAMESIZE);
                    TextAlign(CENTER, CENTER); // TOOD: why not just change textalign?

                    // Tile name outline
                    Fill(0);
                    for (int x_offset = -1; x_offset <= 1; x_offset++)
                    {
                        for (int y_offset = -1; y_offset <= 1; y_offset++)
                        {
                            Text(animalName, x + TILEWIDTH / 2 + x_offset, y + TILEHEIGHT / 2 - (15 / 2 + 10 / 2) / 2 + y_offset);
                        }
                    }

                    // Actual name
                    if (tile_is_animal && animal.GetOwner() is not null)
                    {
                        Fill(HexColour(animal.GetOwner().GetId()));
                    }
                    else
                    {
                        Fill(255);
                    }
                    Text(animalName, animal_name_x, animal_name_y);

                    // Tile set
                    if (tile_is_animal)
                    {
                        TextSize(SETNAMESIZE);
                        Fill("#000000", 192);
                        Text(animal.GetSet(), x + TILEWIDTH / 2, y + TILEHEIGHT / 2 + (15 / 2 + 10 / 2) / 2);
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

                            // Outline
                            Fill(0);
                            for (int x_offset = -1; x_offset <= 1; x_offset++)
                            {
                                for (int y_offset = -1; y_offset <= 1; y_offset++)
                                {
                                    Text(name, name_x + x_offset, name_y + y_offset);
                                }
                            }

                            Fill(HexColour(i));
                            Text(name, name_x, name_y);
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

                        direction += 1;
                        // Do the correct movement
                        x += DIRECTIONS[direction].Item1;
                        y += DIRECTIONS[direction].Item2;
                    }
                }
            }
        }
    }
}