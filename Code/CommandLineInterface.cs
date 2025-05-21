using static Animalopoly.Writing;
using static Animalopoly.PlayerClass;
using static Animalopoly.CardClass;
using static Animalopoly.TileClasses;

namespace Animalopoly
{
    class CommandLineInterface
    {
        const int TILEWIDTH = 12;
        const int TILEHEIGHT = 5;
        public static void WriteBoard(Player[] players, Animal[] animals)
        {
            string boardString = "";
            boardString += ("┌");
            for (int i = 0; i < 8; i++)
            {
                boardString += (new string('─', TILEWIDTH));
                boardString += ("┬");
            }
            boardString += "\b┐" + "\n";

            // First tile-row
            // First row
            for (int i = 0; i < 8; i++)
            {
                boardString += ("│");
                if (players[0].getBankruptStatus() != 2 && players[0].getPos() == i)
                {
                    boardString += $"[{colourNames[0]}]{players[0].getName()}[white]";
                }
                else
                {
                    boardString += (" ");
                }
                boardString += (new string(' ', TILEWIDTH - 2));
                if (players[1].getBankruptStatus() != 2 && players[1].getPos() == i)
                {
                    boardString += $"[{colourNames[1]}]{players[1].getName()}[white]";
                }
                else
                {
                    boardString += (" ");
                }
            }
            boardString += "│" + "\n";

            // Top half
            for (int row = 0; row < Math.Floor((double)(TILEHEIGHT - 2) / 2); row++)
            {
                for (int i = 0; i < 8; i++)
                {
                    boardString += ("│");
                    boardString += (new string(' ', TILEWIDTH));
                }
                boardString += "│" + "\n";
            }
            // Name row
            for (int i = 0; i < 8; i++)
            {
                boardString += ("│");
                boardString += (new string(' ', (int)Math.Floor((float)((TILEWIDTH - animals[i].GetName().Length) / 2.0))));
                boardString += animals[i].GetAnimalText();
                boardString += (new string(' ', (int)Math.Ceiling((float)((TILEWIDTH - animals[i].GetName().Length) / 2.0))));
            }
            boardString += "│" + "\n";
            // Bottom half
            for (int row = 0; row < Math.Floor((double)(TILEHEIGHT - 2) / 2); row++)
            {
                for (int i = 0; i < 8; i++)
                {
                    boardString += ("│");
                    boardString += (new string(' ', TILEWIDTH));
                }
                boardString += "│" + "\n";
            }
            // Bottom row
            for (int i = 0; i < 8; i++)
            {
                boardString += ("│");
                if (players[2].getBankruptStatus() != 2 && players[2].getPos() == i)
                {
                    boardString += $"[{colourNames[2]}]{players[2].getName()}[white]";
                }
                else
                {
                    boardString += (" ");
                }
                boardString += (new string(' ', TILEWIDTH - 2));
                if (players[3].getBankruptStatus() != 2 && players[3].getPos() == i)
                {
                    boardString += $"[{colourNames[3]}]{players[3].getName()}[white]";
                }
                else
                {
                    boardString += (" ");
                }
            }
            boardString += "│" + "\n";

            boardString += ("├");
            boardString += (new string('─', TILEWIDTH));
            boardString += ("┼");
            for (int i = 0; i < 6; i++)
            {
                boardString += (new string('─', TILEWIDTH));
                boardString += ("┴");
            }
            boardString += ("\b┼");
            boardString += (new string('─', TILEWIDTH));
            boardString += ("┤");

            for (int row = 0; row < 5; row++)
            {
                boardString += "\n";
                // Top player row
                // Left
                boardString += ("│");
                if (players[0].getBankruptStatus() != 2 && players[0].getPos() == 25 - row)
                {
                    boardString += $"[{colourNames[0]}]{players[0].getName()}[white]";
                }
                else
                {
                    boardString += (" ");
                }
                boardString += (new string(' ', TILEWIDTH - 2));
                if (players[1].getBankruptStatus() != 2 && players[1].getPos() == 25 - row)
                {
                    boardString += $"[{colourNames[1]}]{players[1].getName()}[white]";
                }
                else
                {
                    boardString += (" ");
                }
                boardString += ("│");
                // Middle
                boardString += (new string(' ', 6 * (TILEWIDTH + 1) - 1));
                // Right
                boardString += ("│");
                if (players[0].getBankruptStatus() != 2 && players[0].getPos() == 8 + row)
                {
                    boardString += $"[{colourNames[0]}]{players[0].getName()}[white]";
                }
                else
                {
                    boardString += (" ");
                }
                boardString += (new string(' ', TILEWIDTH - 2));
                if (players[1].getBankruptStatus() != 2 && players[1].getPos() == 8 + row)
                {
                    boardString += $"[{colourNames[1]}]{players[1].getName()}[white]";
                }
                else
                {
                    boardString += (" ");
                }
                boardString += "│" + "\n";
                // Middle lines
                // Top half
                for (int line = 0; line < Math.Floor((double)(TILEHEIGHT - 2) / 2); line++)
                {
                    boardString += ("│");
                    boardString += (new string(' ', TILEWIDTH));
                    boardString += ("│");
                    boardString += (new string(' ', 6 * (TILEWIDTH + 1) - 1));
                    boardString += ("│");
                    boardString += (new string(' ', TILEWIDTH));
                    boardString += "│" + "\n";
                }
                // Name line
                boardString += ("│");
                boardString += (new string(' ', (int)Math.Floor((float)((TILEWIDTH - animals[25 - row].GetName().Length) / 2.0))));
                boardString += animals[25 - row].GetAnimalText();
                boardString += (new string(' ', (int)Math.Ceiling((float)((TILEWIDTH - animals[25 - row].GetName().Length) / 2.0))));
                boardString += ("│");

                boardString += (new string(' ', 6 * (TILEWIDTH + 1) - 1));

                boardString += ("│");
                boardString += (new string(' ', (int)Math.Floor((float)((TILEWIDTH - animals[8 + row].GetName().Length) / 2.0))));
                boardString += animals[8 + row].GetAnimalText();
                boardString += (new string(' ', (int)Math.Ceiling((float)((TILEWIDTH - animals[8 + row].GetName().Length) / 2.0))));
                boardString += "│" + "\n";
                // Bottom half
                for (int line = 0; line < Math.Floor((double)(TILEHEIGHT - 2) / 2); line++)
                {
                    boardString += ("│");
                    boardString += (new string(' ', TILEWIDTH));
                    boardString += ("│");
                    boardString += (new string(' ', 6 * (TILEWIDTH + 1) - 1));
                    boardString += ("│");
                    boardString += (new string(' ', TILEWIDTH));
                    boardString += "│" + "\n";
                }
                // Bottom row
                // Left
                boardString += ("│");
                if (players[2].getBankruptStatus() != 2 && players[2].getPos() == 25 - row)
                {
                    boardString += $"[{colourNames[2]}]{players[2].getName()}[white]";
                }
                else
                {
                    boardString += (" ");
                }
                boardString += (new string(' ', TILEWIDTH - 2));
                if (players[3].getBankruptStatus() != 2 && players[3].getPos() == 25 - row)
                {
                    boardString += $"[{colourNames[3]}]{players[3].getName()}[white]";

                }
                else
                {
                    boardString += (" ");
                }
                boardString += ("│");
                // Middle
                boardString += (new string(' ', 6 * (TILEWIDTH + 1) - 1));
                // Right
                boardString += ("│");
                if (players[2].getBankruptStatus() != 2 && players[2].getPos() == 8 + row)
                {
                    boardString += $"[{colourNames[2]}]{players[2].getName()}[white]";
                }
                else
                {
                    boardString += (" ");
                }
                boardString += (new string(' ', TILEWIDTH - 2));
                if (players[3].getBankruptStatus() != 2 && players[3].getPos() == 8 + row)
                {
                    boardString += $"[{colourNames[3]}]{players[3].getName()}[white]";
                }
                else
                {
                    boardString += (" ");
                }
                boardString += "│" + "\n";

                boardString += ("├");
                boardString += (new string('─', TILEWIDTH));
                boardString += ("┤");
                for (int i = 0; i < 6; i++)
                {
                    boardString += (new string(' ', TILEWIDTH + 1));
                }
                boardString += ("\b├");
                boardString += (new string('─', TILEWIDTH));
                boardString += ("┤");
            }
            // Bottom row
            boardString += (new string('\b', 110));
            boardString += ("├");
            boardString += (new string('─', TILEWIDTH));
            boardString += ("┼");
            for (int i = 0; i < 6; i++)
            {
                boardString += (new string('─', TILEWIDTH));
                boardString += ("┬");
            }
            boardString += ("\b┼");
            boardString += (new string('─', TILEWIDTH));
            boardString += "┤" + "\n";

            // Final row
            // First line
            for (int i = 0; i < 8; i++)
            {
                boardString += ("│");
                if (players[0].getBankruptStatus() != 2 && players[0].getPos() == 20 - i)
                {
                    boardString += $"[{colourNames[0]}]{players[0].getName()}[white]";
                }
                else
                {
                    boardString += (" ");
                }
                boardString += (new string(' ', TILEWIDTH - 2));
                if (players[1].getBankruptStatus() != 2 && players[1].getPos() == 20 - i)
                {
                    boardString += $"[{colourNames[1]}]{players[1].getName()}[white]";
                }
                else
                {
                    boardString += (" ");
                }
            }
            boardString += "│" + "\n";

            // Top half
            for (int row = 0; row < Math.Floor((double)(TILEHEIGHT - 2) / 2); row++)
            {
                for (int i = 0; i < 8; i++)
                {
                    boardString += ("│");
                    boardString += (new string(' ', TILEWIDTH));
                }
                boardString += "│" + "\n";
            }
            // Name row
            for (int i = 0; i < 8; i++)
            {
                boardString += ("│");
                boardString += (new string(' ', (int)Math.Floor((float)((TILEWIDTH - animals[20 - i].GetName().Length) / 2.0))));
                boardString += animals[20 - i].GetAnimalText();
                boardString += (new string(' ', (int)Math.Ceiling((float)((TILEWIDTH - animals[20 - i].GetName().Length) / 2.0))));
            }
            boardString += "│" + "\n";
            // Bottom half
            for (int row = 0; row < Math.Floor((double)(TILEHEIGHT - 2) / 2); row++)
            {
                for (int i = 0; i < 8; i++)
                {
                    boardString += ("│");
                    boardString += (new string(' ', TILEWIDTH));
                }
                boardString += "│" + "\n";
            }
            // Bottom row
            for (int i = 0; i < 8; i++)
            {
                boardString += ("│");
                if (players[2].getBankruptStatus() != 2 && players[2].getPos() == 20 - i)
                {
                    boardString += $"[{colourNames[2]}]{players[2].getName()}[white]";
                }
                else
                {
                    boardString += (" ");
                }
                boardString += (new string(' ', TILEWIDTH - 2));
                if (players[3].getBankruptStatus() != 2 && players[3].getPos() == 20 - i)
                {
                    boardString += $"[{colourNames[3]}]{players[3].getName()}[white]";
                }
                else
                {
                    boardString += (" ");
                }
            }
            boardString += "│" + "\n";

            boardString += ("└");
            boardString += (new string('─', TILEWIDTH));
            boardString += ("┴");
            for (int i = 0; i < 6; i++)
            {
                boardString += (new string('─', TILEWIDTH));
                boardString += ("┴");
            }
            boardString += ("\b┴");
            boardString += (new string('─', TILEWIDTH));
            boardString += "┘" + "\n";

            Write(boardString);
        }
    }
}