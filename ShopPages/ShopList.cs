using Robot_API.Structs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Robot_API.ShopPages
{
    internal static class ShopList
    {
        internal static void PrintGamesList(List<GameList.Game> games)
        {
            string linha = "+-----+--------------------------+-----------+--------------+------------------------+";
            Console.WriteLine(linha);
            Console.WriteLine("|  #  | Nome                     | Versão    | Status       | Slots (Privado)        |");
            Console.WriteLine(linha);

            for (int i = 0; i < games.Count; i++)
            {
                var game = games[i];
                bool isPrivado = game.MaxKeys != 0 && game.SoldKeys != 0;

                string statusLabel = game.Status.ToLower() == "on" ? "Online" : "Maintenance";
                string slotsStr = isPrivado ? $"[{game.SoldKeys}/{game.MaxKeys}]" : "-";

                string nome = game.Name.Length > 24
                    ? game.Name.Substring(0, 21) + "..."
                    : game.Name.PadRight(24);

                string versao = game.Version.PadRight(9);
                string status = statusLabel.PadRight(12);
                string slots = slotsStr.PadRight(22);

                Console.Write("| ");
                Console.ForegroundColor = ConsoleColor.Cyan;
                Console.Write($"{(i + 1),-3}");
                Console.ResetColor();
                Console.Write(" | ");

                Console.ForegroundColor = ConsoleColor.White;
                Console.Write(nome);
                Console.ResetColor();
                Console.Write(" | ");

                Console.ForegroundColor = ConsoleColor.Gray;
                Console.Write(versao);
                Console.ResetColor();
                Console.Write(" | ");

                Console.ForegroundColor = statusLabel == "Online" ? ConsoleColor.Green : ConsoleColor.Red;
                Console.Write(status);
                Console.ResetColor();
                Console.Write(" | ");

                Console.ForegroundColor = isPrivado ? ConsoleColor.Yellow : ConsoleColor.DarkGray;
                Console.Write(slots);
                Console.ResetColor();

                Console.WriteLine(" |");
            }

            Console.WriteLine(linha);
        }
    }
}
