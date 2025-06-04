using static Animalopoly.Code.Writing;
using static Animalopoly.Code.PlayerClass;
using static Animalopoly.Code.CardClass;
using static Animalopoly.Code.TileClasses;
using System.Runtime.InteropServices;

namespace Animalopoly.Code
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
                if (players[0].GetBankruptStatus() < 2 && players[0].GetPos() == i)
                {
                    boardString += $"[{colourNames[0]}]{players[0].GetName()}[white]";
                }
                else
                {
                    boardString += (" ");
                }
                boardString += (new string(' ', TILEWIDTH - 2));
                if (players[1].GetBankruptStatus() < 2 && players[1].GetPos() == i)
                {
                    boardString += $"[{colourNames[1]}]{players[1].GetName()}[white]";
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
                boardString += animals[i].GetFormattedName();
                boardString += (new string(' ', (int)Math.Ceiling((float)((TILEWIDTH - animals[i].GetName().Length) / 2.0))));
            }
            boardString += "│" + "\n";
            // Set row
            for (int row = 0; row < Math.Floor((double)(TILEHEIGHT - 2) / 2); row++)
            {
                for (int i = 0; i < 8; i++)
                {
                    boardString += ("│");
                    boardString += (new string(' ', (int)Math.Floor((float)((TILEWIDTH - animals[i].GetSmallSet().Length) / 2.0))));
                    boardString += $"[dark grey]{animals[i].GetSmallSet()}[white]";
                    boardString += (new string(' ', (int)Math.Ceiling((float)((TILEWIDTH - animals[i].GetSmallSet().Length) / 2.0))));
                }
                boardString += "│" + "\n";
            }
            // Bottom row
            for (int i = 0; i < 8; i++)
            {
                boardString += ("│");
                if (players[2].GetBankruptStatus() < 2 && players[2].GetPos() == i)
                {
                    boardString += $"[{colourNames[2]}]{players[2].GetName()}[white]";
                }
                else
                {
                    boardString += (" ");
                }
                boardString += (new string(' ', TILEWIDTH - 2));
                if (players[3].GetBankruptStatus() < 2 && players[3].GetPos() == i)
                {
                    boardString += $"[{colourNames[3]}]{players[3].GetName()}[white]";
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
                if (players[0].GetBankruptStatus() < 2 && players[0].GetPos() == 25 - row)
                {
                    boardString += $"[{colourNames[0]}]{players[0].GetName()}[white]";
                }
                else
                {
                    boardString += (" ");
                }
                boardString += (new string(' ', TILEWIDTH - 2));
                if (players[1].GetBankruptStatus() < 2 && players[1].GetPos() == 25 - row)
                {
                    boardString += $"[{colourNames[1]}]{players[1].GetName()}[white]";
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
                if (players[0].GetBankruptStatus() < 2 && players[0].GetPos() == 8 + row)
                {
                    boardString += $"[{colourNames[0]}]{players[0].GetName()}[white]";
                }
                else
                {
                    boardString += (" ");
                }
                boardString += (new string(' ', TILEWIDTH - 2));
                if (players[1].GetBankruptStatus() < 2 && players[1].GetPos() == 8 + row)
                {
                    boardString += $"[{colourNames[1]}]{players[1].GetName()}[white]";
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
                boardString += animals[25 - row].GetFormattedName();
                boardString += (new string(' ', (int)Math.Ceiling((float)((TILEWIDTH - animals[25 - row].GetName().Length) / 2.0))));
                boardString += ("│");

                boardString += (new string(' ', 6 * (TILEWIDTH + 1) - 1));

                boardString += ("│");
                boardString += (new string(' ', (int)Math.Floor((float)((TILEWIDTH - animals[8 + row].GetName().Length) / 2.0))));
                boardString += animals[8 + row].GetFormattedName();
                boardString += (new string(' ', (int)Math.Ceiling((float)((TILEWIDTH - animals[8 + row].GetName().Length) / 2.0))));
                boardString += "│" + "\n";
                // Bottom half
                for (int line = 0; line < Math.Floor((double)(TILEHEIGHT - 2) / 2); line++)
                {
                    boardString += ("│");
                    boardString += (new string(' ', (int)Math.Floor((float)((TILEWIDTH - animals[25 - row].GetSmallSet().Length) / 2.0))));
                    boardString += $"[dark grey]{animals[25 - row].GetSmallSet()}[white]";
                    boardString += (new string(' ', (int)Math.Ceiling((float)((TILEWIDTH - animals[25 - row].GetSmallSet().Length) / 2.0))));
                    boardString += ("│");
                    boardString += (new string(' ', 6 * (TILEWIDTH + 1) - 1));
                    boardString += ("│");
                    boardString += (new string(' ', (int)Math.Floor((float)((TILEWIDTH - animals[8 + row].GetSmallSet().Length) / 2.0))));
                    boardString += $"[dark grey]{animals[8 + row].GetSmallSet()}[white]";
                    boardString += (new string(' ', (int)Math.Ceiling((float)((TILEWIDTH - animals[8 + row].GetSmallSet().Length) / 2.0))));
                    boardString += "│" + "\n";
                }
                // Bottom row
                // Left
                boardString += ("│");
                if (players[2].GetBankruptStatus() < 2 && players[2].GetPos() == 25 - row)
                {
                    boardString += $"[{colourNames[2]}]{players[2].GetName()}[white]";
                }
                else
                {
                    boardString += (" ");
                }
                boardString += (new string(' ', TILEWIDTH - 2));
                if (players[3].GetBankruptStatus() < 2 && players[3].GetPos() == 25 - row)
                {
                    boardString += $"[{colourNames[3]}]{players[3].GetName()}[white]";

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
                if (players[2].GetBankruptStatus() < 2 && players[2].GetPos() == 8 + row)
                {
                    boardString += $"[{colourNames[2]}]{players[2].GetName()}[white]";
                }
                else
                {
                    boardString += (" ");
                }
                boardString += (new string(' ', TILEWIDTH - 2));
                if (players[3].GetBankruptStatus() < 2 && players[3].GetPos() == 8 + row)
                {
                    boardString += $"[{colourNames[3]}]{players[3].GetName()}[white]";
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
                if (players[0].GetBankruptStatus() < 2 && players[0].GetPos() == 20 - i)
                {
                    boardString += $"[{colourNames[0]}]{players[0].GetName()}[white]";
                }
                else
                {
                    boardString += (" ");
                }
                boardString += (new string(' ', TILEWIDTH - 2));
                if (players[1].GetBankruptStatus() < 2 && players[1].GetPos() == 20 - i)
                {
                    boardString += $"[{colourNames[1]}]{players[1].GetName()}[white]";
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
                boardString += animals[20 - i].GetFormattedName();
                boardString += (new string(' ', (int)Math.Ceiling((float)((TILEWIDTH - animals[20 - i].GetName().Length) / 2.0))));
            }
            boardString += "│" + "\n";
            // Set row
            for (int row = 0; row < Math.Floor((double)(TILEHEIGHT - 2) / 2); row++)
            {
                for (int i = 0; i < 8; i++)
                {
                    boardString += ("│");
                    boardString += (new string(' ', (int)Math.Floor((float)((TILEWIDTH - animals[20 - i].GetSmallSet().Length) / 2.0))));
                    boardString += $"[dark grey]{animals[20 - i].GetSmallSet()}[white]";
                    boardString += (new string(' ', (int)Math.Ceiling((float)((TILEWIDTH - animals[20 - i].GetSmallSet().Length) / 2.0))));
                }
                boardString += "│" + "\n";
            }
            // Bottom row
            for (int i = 0; i < 8; i++)
            {
                boardString += ("│");
                if (players[2].GetBankruptStatus() < 2 && players[2].GetPos() == 20 - i)
                {
                    boardString += $"[{colourNames[2]}]{players[2].GetName()}[white]";
                }
                else
                {
                    boardString += (" ");
                }
                boardString += (new string(' ', TILEWIDTH - 2));
                if (players[3].GetBankruptStatus() < 2 && players[3].GetPos() == 20 - i)
                {
                    boardString += $"[{colourNames[3]}]{players[3].GetName()}[white]";
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

        public static void Fullscreen() // Original code from the accepted answer to https://learn.microsoft.com/en-us/answers/questions/1630444/
        {
            [DllImport("kernel32.dll", ExactSpelling = true)]
            static extern IntPtr GetConsoleWindow();

            [DllImport("user32.dll")]
            static extern bool ShowWindow(IntPtr hWnd, int nCmdShow);

            const int SW_MAXIMIZE = 3;

            IntPtr handle = GetConsoleWindow();
            ShowWindow(handle, SW_MAXIMIZE);
        }
    }
}