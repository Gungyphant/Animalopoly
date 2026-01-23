using NetProcessing;
using System.Linq;
using static Animalopoly.Code.CardClass;
using static Animalopoly.Code.CommandLineInterface;
using static Animalopoly.Code.Graphing;
using static Animalopoly.Code.NetProcessingUI;
using static Animalopoly.Code.PlayerClass;
using static Animalopoly.Code.TileClasses;
using static Animalopoly.Code.Writing;
using static Animalopoly.Code.Program;

namespace Animalopoly.Code
{
    class NetProcessingUI : Sketch
    {
        static string[] tileColours = ["#0000ff", "#00ff00", "#ff0000", "#ffff00"];

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
            if (players != null)
            {
                //HideConsole();
                int x = 0;
                int y = 0;
                int direction = 0;
                foreach ((int id, Animal animal) in locations.Select((value, index) => (index, value)))
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
                    if (animal.GetName() == "") // There are inexplicable flickers where the name & set disappear; see #12
                    {
                        throw new Exception("Got \"\" for the name of an Animal when drawing GUI");
                    }
                    Text(animal.GetName(), x + TILEWIDTH / 2, y + TILEHEIGHT / 2 - (15 / 2 + 10 / 2) / 2);

                    // Tile set
                    TextSize(10);
                    Fill("#000000", 192);
                    Text(animal.GetSet(), x + TILEWIDTH / 2, y + TILEHEIGHT / 2 + (15 / 2 + 10 / 2) / 2);


                    // Players
                    TextSize(25);
                    if (players[0] != null && players[0].GetBankruptStatus() < 2 && players[0].GetPos() == id)
                    {
                        TextAlign(LEFT, TOP);

                        string name = Convert.ToString(players[0].GetName());
                        int name_x = x + 2;
                        int name_y = y;

                        // Outline
                        Fill(0);
                        for (int x_offset = -1; x_offset <= 1; x_offset++)
                        {
                            for (int y_offset = -1; y_offset <= 1; y_offset++)
                            {
                                Text(name, name_x + x_offset, name_y + y_offset);
                            }
                        }

                        Fill(tileColours[0]);
                        Text(name, name_x, name_y);
                    }
                    if (players[1] != null && players[1].GetBankruptStatus() < 2 && players[1].GetPos() == id)
                    {
                        TextAlign(RIGHT, TOP);

                        string name = Convert.ToString(players[1].GetName());
                        int name_x = x + TILEWIDTH - 2;
                        int name_y = y;

                        // Outline
                        Fill(0);
                        for (int x_offset = -1; x_offset <= 1; x_offset++)
                        {
                            for (int y_offset = -1; y_offset <= 1; y_offset++)
                            {
                                Text(name, name_x + x_offset, name_y + y_offset);
                            }
                        }

                        Fill(tileColours[1]);
                        Text(name, name_x, name_y);
                    }
                    if (players[2] != null && players[2].GetBankruptStatus() < 2 && players[2].GetPos() == id)
                    {
                        TextAlign(LEFT, BOTTOM);

                        string name = Convert.ToString(players[2].GetName());
                        int name_x = x + 2;
                        int name_y = y + TILEHEIGHT;

                        // Outline
                        Fill(0);
                        for (int x_offset = -1; x_offset <= 1; x_offset++)
                        {
                            for (int y_offset = -1; y_offset <= 1; y_offset++)
                            {
                                Text(name, name_x + x_offset, name_y + y_offset);
                            }
                        }

                        Fill(tileColours[2]);
                        Text(name, name_x, name_y);
                    }
                    if (players[3] != null && players[3].GetBankruptStatus() < 2 && players[3].GetPos() == id)
                    {
                        TextAlign(RIGHT, BOTTOM);

                        string name = Convert.ToString(players[3].GetName());
                        int name_x = x + TILEWIDTH - 2;
                        int name_y = y + TILEHEIGHT;

                        // Outline
                        Fill(0);
                        for (int x_offset = -1; x_offset <= 1; x_offset++)
                        {
                            for (int y_offset = -1; y_offset <= 1; y_offset++)
                            {
                                Text(name, name_x + x_offset, name_y + y_offset);
                            }
                        }

                        Fill(tileColours[3]);
                        Text(name, name_x, name_y);
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