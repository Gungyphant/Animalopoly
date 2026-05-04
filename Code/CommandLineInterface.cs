using static Animalopoly.Code.Writing;
using static Animalopoly.Code.PlayerClass;
using static Animalopoly.Code.TileClasses;

namespace Animalopoly.Code
{
    class CommandLineInterface
    {
        const int TILE_WIDTH = 12;
        const int TILE_HEIGHT = 5;
        public static void WriteBoard(Player[] players, Tile[] tiles)
        { // Writes the CLI board to console
            string boardString = "";
            boardString += ("┌");
            for (int i = 0; i < 8; i++)
            {
                boardString += (new string('─', TILE_WIDTH));
                boardString += ("┬");
            }
            boardString += "\b┐" + "\n";

            // First tile-row
            // First row
            for (int i = 0; i < 8; i++)
            {
                boardString += ("│");
                if ((players[0].GetBankruptStatus() == BankruptcyStatus.Normal || players[0].GetBankruptStatus() == BankruptcyStatus.Warned) && players[0].GetPos() == i)
                {
                    boardString += $"[{colourNames[0]}]{players[0].GetName()}[white]";
                }
                else
                {
                    boardString += (" ");
                }
                boardString += (new string(' ', TILE_WIDTH - 2));
                if ((players[1].GetBankruptStatus() == BankruptcyStatus.Normal || players[1].GetBankruptStatus() == BankruptcyStatus.Warned) && players[1].GetPos() == i)
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
            for (int row = 0; row < Math.Floor((double)(TILE_HEIGHT - 2) / 2); row++)
            {
                for (int i = 0; i < 8; i++)
                {
                    boardString += ("│");
                    boardString += (new string(' ', TILE_WIDTH));
                }
                boardString += "│" + "\n";
            }
            // Name row
            for (int i = 0; i < 8; i++)
            {
                boardString += ("│");
                boardString += (new string(' ', (int)Math.Floor((float)((TILE_WIDTH - tiles[i].GetName().Length) / 2.0))));
                boardString += tiles[i].GetFormattedName();
                boardString += (new string(' ', (int)Math.Ceiling((float)((TILE_WIDTH - tiles[i].GetName().Length) / 2.0))));
            }
            boardString += "│" + "\n";
            // Set row
            for (int row = 0; row < Math.Floor((double)(TILE_HEIGHT - 2) / 2); row++)
            {
                for (int i = 0; i < 8; i++)
                {
                    boardString += ("│");
                    if (tiles[i] is Animal animal_i)
                    {
                        boardString += (new string(' ', (int)Math.Floor((float)((TILE_WIDTH - animal_i.GetSmallSet().Length) / 2.0))));
                        boardString += $"[dark grey]{animal_i.GetSmallSet()}[white]";
                        boardString += (new string(' ', (int)Math.Ceiling((float)((TILE_WIDTH - animal_i.GetSmallSet().Length) / 2.0))));
                    }
                    else
                    {
                        boardString += new string(' ', TILE_WIDTH);
                    }
                }
                boardString += "│" + "\n";
            }
            // Bottom row
            for (int i = 0; i < 8; i++)
            {
                boardString += ("│");
                if ((players[2].GetBankruptStatus() == BankruptcyStatus.Normal || players[2].GetBankruptStatus() == BankruptcyStatus.Warned) && players[2].GetPos() == i)
                {
                    boardString += $"[{colourNames[2]}]{players[2].GetName()}[white]";
                }
                else
                {
                    boardString += (" ");
                }
                boardString += (new string(' ', TILE_WIDTH - 2));
                if ((players[3].GetBankruptStatus() == BankruptcyStatus.Normal || players[3].GetBankruptStatus() == BankruptcyStatus.Warned) && players[3].GetPos() == i)
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
            boardString += (new string('─', TILE_WIDTH));
            boardString += ("┼");
            for (int i = 0; i < 6; i++)
            {
                boardString += (new string('─', TILE_WIDTH));
                boardString += ("┴");
            }
            boardString += ("\b┼");
            boardString += (new string('─', TILE_WIDTH));
            boardString += ("┤");

            for (int row = 0; row < 5; row++)
            {
                boardString += "\n";
                // Top player row
                // Left
                boardString += ("│");
                if ((players[0].GetBankruptStatus() == BankruptcyStatus.Normal || players[0].GetBankruptStatus() == BankruptcyStatus.Warned) && players[0].GetPos() == 25 - row)
                {
                    boardString += $"[{colourNames[0]}]{players[0].GetName()}[white]";
                }
                else
                {
                    boardString += (" ");
                }
                boardString += (new string(' ', TILE_WIDTH - 2));
                if ((players[1].GetBankruptStatus() == BankruptcyStatus.Normal || players[1].GetBankruptStatus() == BankruptcyStatus.Warned) && players[1].GetPos() == 25 - row)
                {
                    boardString += $"[{colourNames[1]}]{players[1].GetName()}[white]";
                }
                else
                {
                    boardString += (" ");
                }
                boardString += ("│");
                // Middle
                boardString += (new string(' ', 6 * (TILE_WIDTH + 1) - 1));
                // Right
                boardString += ("│");
                if ((players[0].GetBankruptStatus() == BankruptcyStatus.Normal || players[0].GetBankruptStatus() == BankruptcyStatus.Warned) && players[0].GetPos() == 8 + row)
                {
                    boardString += $"[{colourNames[0]}]{players[0].GetName()}[white]";
                }
                else
                {
                    boardString += (" ");
                }
                boardString += (new string(' ', TILE_WIDTH - 2));
                if ((players[1].GetBankruptStatus() == BankruptcyStatus.Normal || players[1].GetBankruptStatus() == BankruptcyStatus.Warned) && players[1].GetPos() == 8 + row)
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
                for (int line = 0; line < Math.Floor((double)(TILE_HEIGHT - 2) / 2); line++)
                {
                    boardString += ("│");
                    boardString += (new string(' ', TILE_WIDTH));
                    boardString += ("│");
                    boardString += (new string(' ', 6 * (TILE_WIDTH + 1) - 1));
                    boardString += ("│");
                    boardString += (new string(' ', TILE_WIDTH));
                    boardString += "│" + "\n";
                }
                // Name line
                boardString += ("│");
                boardString += (new string(' ', (int)Math.Floor((float)((TILE_WIDTH - tiles[25 - row].GetName().Length) / 2.0))));
                boardString += tiles[25 - row].GetFormattedName();
                boardString += (new string(' ', (int)Math.Ceiling((float)((TILE_WIDTH - tiles[25 - row].GetName().Length) / 2.0))));
                boardString += ("│");

                boardString += (new string(' ', 6 * (TILE_WIDTH + 1) - 1));

                boardString += ("│");
                boardString += (new string(' ', (int)Math.Floor((float)((TILE_WIDTH - tiles[8 + row].GetName().Length) / 2.0))));
                boardString += tiles[8 + row].GetFormattedName();
                boardString += (new string(' ', (int)Math.Ceiling((float)((TILE_WIDTH - tiles[8 + row].GetName().Length) / 2.0))));
                boardString += "│" + "\n";
                // Bottom half
                for (int line = 0; line < Math.Floor((double)(TILE_HEIGHT - 2) / 2); line++)
                {
                    boardString += ("│");
                    if (tiles[25 - row] is Animal animal_25_minus_row)
                    {
                        boardString += (new string(' ', (int)Math.Floor((float)((TILE_WIDTH - animal_25_minus_row.GetSmallSet().Length) / 2.0))));
                        boardString += $"[dark grey]{animal_25_minus_row.GetSmallSet()}[white]";
                        boardString += (new string(' ', (int)Math.Ceiling((float)((TILE_WIDTH - animal_25_minus_row.GetSmallSet().Length) / 2.0))));
                    }
                    else
                    {
                        boardString += new string(' ', TILE_WIDTH);
                    }
                    boardString += ("│");
                    boardString += (new string(' ', 6 * (TILE_WIDTH + 1) - 1));
                    boardString += ("│");
                    if (tiles[8 + row] is Animal animal_8_plus_row)
                    {
                        boardString += (new string(' ', (int)Math.Floor((float)((TILE_WIDTH - animal_8_plus_row.GetSmallSet().Length) / 2.0))));
                        boardString += $"[dark grey]{animal_8_plus_row.GetSmallSet()}[white]";
                        boardString += (new string(' ', (int)Math.Ceiling((float)((TILE_WIDTH - animal_8_plus_row.GetSmallSet().Length) / 2.0))));
                    }
                    else
                    {
                        boardString += new string(' ', TILE_WIDTH);
                    }
                    boardString += "│" + "\n";
                }
                // Bottom row
                // Left
                boardString += ("│");
                if ((players[2].GetBankruptStatus() == BankruptcyStatus.Normal || players[2].GetBankruptStatus() == BankruptcyStatus.Warned) && players[2].GetPos() == 25 - row)
                {
                    boardString += $"[{colourNames[2]}]{players[2].GetName()}[white]";
                }
                else
                {
                    boardString += (" ");
                }
                boardString += (new string(' ', TILE_WIDTH - 2));
                if ((players[3].GetBankruptStatus() == BankruptcyStatus.Normal || players[3].GetBankruptStatus() == BankruptcyStatus.Warned) && players[3].GetPos() == 25 - row)
                {
                    boardString += $"[{colourNames[3]}]{players[3].GetName()}[white]";

                }
                else
                {
                    boardString += (" ");
                }
                boardString += ("│");
                // Middle
                boardString += (new string(' ', 6 * (TILE_WIDTH + 1) - 1));
                // Right
                boardString += ("│");
                if ((players[2].GetBankruptStatus() == BankruptcyStatus.Normal || players[2].GetBankruptStatus() == BankruptcyStatus.Warned) && players[2].GetPos() == 8 + row)
                {
                    boardString += $"[{colourNames[2]}]{players[2].GetName()}[white]";
                }
                else
                {
                    boardString += (" ");
                }
                boardString += (new string(' ', TILE_WIDTH - 2));
                if ((players[3].GetBankruptStatus() == BankruptcyStatus.Normal || players[3].GetBankruptStatus() == BankruptcyStatus.Warned) && players[3].GetPos() == 8 + row)
                {
                    boardString += $"[{colourNames[3]}]{players[3].GetName()}[white]";
                }
                else
                {
                    boardString += (" ");
                }
                boardString += "│" + "\n";

                boardString += ("├");
                boardString += (new string('─', TILE_WIDTH));
                boardString += ("┤");
                for (int i = 0; i < 6; i++)
                {
                    boardString += (new string(' ', TILE_WIDTH + 1));
                }
                boardString += ("\b├");
                boardString += (new string('─', TILE_WIDTH));
                boardString += ("┤");
            }
            // Bottom row
            boardString += (new string('\b', 110));
            boardString += ("├");
            boardString += (new string('─', TILE_WIDTH));
            boardString += ("┼");
            for (int i = 0; i < 6; i++)
            {
                boardString += (new string('─', TILE_WIDTH));
                boardString += ("┬");
            }
            boardString += ("\b┼");
            boardString += (new string('─', TILE_WIDTH));
            boardString += "┤" + "\n";

            // Final row
            // First line
            for (int i = 0; i < 8; i++)
            {
                boardString += ("│");
                if ((players[0].GetBankruptStatus() == BankruptcyStatus.Normal || players[0].GetBankruptStatus() == BankruptcyStatus.Warned) && players[0].GetPos() == 20 - i)
                {
                    boardString += $"[{colourNames[0]}]{players[0].GetName()}[white]";
                }
                else
                {
                    boardString += (" ");
                }
                boardString += (new string(' ', TILE_WIDTH - 2));
                if ((players[1].GetBankruptStatus() == BankruptcyStatus.Normal || players[1].GetBankruptStatus() == BankruptcyStatus.Warned) && players[1].GetPos() == 20 - i)
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
            for (int row = 0; row < Math.Floor((double)(TILE_HEIGHT - 2) / 2); row++)
            {
                for (int i = 0; i < 8; i++)
                {
                    boardString += ("│");
                    boardString += (new string(' ', TILE_WIDTH));
                }
                boardString += "│" + "\n";
            }
            // Name row
            for (int i = 0; i < 8; i++)
            {
                boardString += ("│");
                boardString += (new string(' ', (int)Math.Floor((float)((TILE_WIDTH - tiles[20 - i].GetName().Length) / 2.0))));
                boardString += tiles[20 - i].GetFormattedName();
                boardString += (new string(' ', (int)Math.Ceiling((float)((TILE_WIDTH - tiles[20 - i].GetName().Length) / 2.0))));
            }
            boardString += "│" + "\n";
            // Set row
            for (int row = 0; row < Math.Floor((double)(TILE_HEIGHT - 2) / 2); row++)
            {
                for (int i = 0; i < 8; i++)
                {
                    boardString += ("│");
                    if (tiles[20 - i] is Animal animal_20_minus_i)
                    {
                        boardString += (new string(' ', (int)Math.Floor((float)((TILE_WIDTH - animal_20_minus_i.GetSmallSet().Length) / 2.0))));
                        boardString += $"[dark grey]{animal_20_minus_i.GetSmallSet()}[white]";
                        boardString += (new string(' ', (int)Math.Ceiling((float)((TILE_WIDTH - animal_20_minus_i.GetSmallSet().Length) / 2.0))));
                    }
                    else
                    {
                        boardString += new string(' ', TILE_WIDTH);
                    }
                }
                boardString += "│" + "\n";
            }
            // Bottom row
            for (int i = 0; i < 8; i++)
            {
                boardString += ("│");
                if ((players[2].GetBankruptStatus() == BankruptcyStatus.Normal || players[2].GetBankruptStatus() == BankruptcyStatus.Warned) && players[2].GetPos() == 20 - i)
                {
                    boardString += $"[{colourNames[2]}]{players[2].GetName()}[white]";
                }
                else
                {
                    boardString += (" ");
                }
                boardString += (new string(' ', TILE_WIDTH - 2));
                if ((players[3].GetBankruptStatus() == BankruptcyStatus.Normal || players[3].GetBankruptStatus() == BankruptcyStatus.Warned) && players[3].GetPos() == 20 - i)
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
            boardString += (new string('─', TILE_WIDTH));
            boardString += ("┴");
            for (int i = 0; i < 6; i++)
            {
                boardString += (new string('─', TILE_WIDTH));
                boardString += ("┴");
            }
            boardString += ("\b┴");
            boardString += (new string('─', TILE_WIDTH));
            boardString += "┘" + "\n";

            Write(boardString);
        }

        public static void Fullscreen() // Original code from the accepted answer to https://learn.microsoft.com/en-us/answers/questions/1630444/
        {
            return; // TODO: Fullscreen causes the console to not be movable or minimizable
        //[DllImport("kernel32.dll", ExactSpelling = true)]
        //    static extern IntPtr GetConsoleWindow();

        //[DllImport("user32.dll")]
        //    static extern bool ShowWindow(IntPtr hWnd, int nCmdShow);

        //    const int SW_MAXIMIZE = 3;

        //    IntPtr handle = GetConsoleWindow();
        //    ShowWindow(handle, SW_MAXIMIZE);
        }
    }
}