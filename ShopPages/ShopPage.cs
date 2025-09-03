using Robot_API.Structs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace Robot_API.ShopPages
{
    internal static class ShopPage
    {
        internal static async Task AsyncStore(HttpClient httpClient, bool validar_pagamento)
        {
            var response = await httpClient.GetAsync("/games");
            var responseBody = await response.Content.ReadAsStringAsync();

            if (!response.IsSuccessStatusCode)
            {
                Console.WriteLine($"Erro {response.StatusCode} ao obter lista de jogos:");
                Console.WriteLine(responseBody);
                return;
            }

            var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
            options.Converters.Add(new GameList.GameConverter());

            var games = JsonSerializer.Deserialize<List<GameList.Game>>(responseBody, options);

            if (games == null || games.Count == 0)
            {
                Console.WriteLine("Nenhum jogo disponível.");
                return;
            }

            while (true)
            {
                Console.Clear();

                ShopList.PrintGamesList(games);

                Console.Write("\nDigite o número do jogo que deseja (ou 0 para sair): ");
                if (!int.TryParse(Console.ReadLine(), out int gameIndex) || gameIndex < 0 || gameIndex > games.Count)
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine("Entrada inválida.");
                    Console.ResetColor();
                    Console.WriteLine("Pressione qualquer tecla para continuar...");
                    Console.ReadKey();
                    continue;
                }

                if (gameIndex == 0)
                {
                    Console.WriteLine("Encerrando...");
                    break;
                }

                Program.selectedGame = games[gameIndex - 1];

                if (!validar_pagamento)
                    await BuyPlan.SelectPlans(Program.selectedGame, httpClient, Program.purchaseCodeFile);
                else
                    await PaymentValidate.ValidatePaymentAsync(Program.selectedGame, httpClient);
            }
        }
    }
}
