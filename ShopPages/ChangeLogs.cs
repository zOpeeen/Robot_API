using Robot_API.Structs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace Robot_API.ShopPages
{
    internal static class ChangeLogs
    {
        internal static async Task ShowChangeLogs(HttpClient httpClient)
        {
            Console.Clear();

            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine("+-------------------------------------------------------------+");
            Console.WriteLine("|                        CHANGE LOGS                          |");
            Console.WriteLine("+-------------------------------------------------------------+");
            Console.ResetColor();

            try
            {
                var response = await httpClient.GetAsync("/games/changelog");
                var body = await response.Content.ReadAsStringAsync();

                if (!response.IsSuccessStatusCode)
                {
                    var error = JsonSerializer.Deserialize<ChangeLogsList.ApiError>(body, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine($"Erro ao buscar changelogs: {error?.Message ?? "Erro desconhecido"}");
                    Console.ResetColor();
                    return;
                }

                var changelogResponse = JsonSerializer.Deserialize<ChangeLogsList.ChangeLogResponse>(body, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

                if (changelogResponse == null || changelogResponse.Data == null || changelogResponse.Data.Count == 0)
                {
                    Console.WriteLine("Nenhum changelog encontrado.");
                    return;
                }

                foreach (var log in changelogResponse.Data.OrderByDescending(l => l.CreatedAt))
                {
                    Console.ForegroundColor = ConsoleColor.Yellow;
                    Console.WriteLine($"[{log.CreatedAt:dd/MM/yyyy HH:mm}]");
                    Console.ResetColor();

                    Console.WriteLine($"{log.Message}\n");
                }
            }
            catch (Exception ex)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine($"Erro ao obter change logs: {ex.Message}");
                Console.ResetColor();
            }

            Console.WriteLine("Pressione qualquer tecla para voltar ao menu...");
            Console.ReadKey();
        }
    }
}
