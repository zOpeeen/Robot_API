using Robot_API.Structs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace Robot_API.ShopPages
{
    internal static class PaymentValidate
    {
        internal static async Task ValidatePaymentAsync(GameList.Game selectedGame, HttpClient httpClient)
        {
            Console.Clear();
            Console.Write("Digite o código do pagamento para validar: ");
            string codigo = Console.ReadLine().Trim();

            if (string.IsNullOrEmpty(codigo))
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("Código inválido.");
                Console.ResetColor();
                Console.WriteLine("Pressione qualquer tecla para voltar...");
                Console.ReadKey();
                return;
            }

            await CheckStatusByCode(codigo, httpClient, selectedGame);

            Console.WriteLine("Pressione qualquer tecla para sair...");
            Console.ReadKey();

            Environment.Exit(0);
        }

        internal static async Task CheckStatusByCode(string codigo, HttpClient httpClient, GameList.Game selectedGame)
        {
            Console.WriteLine($"\nConsultando status para o código: {codigo}\n");

            var paymentStatusRequest = new HttpRequestMessage(HttpMethod.Get, $"/payment/status/{codigo}");
            var statusResponse = await httpClient.SendAsync(paymentStatusRequest);
            var statusResponseBody = await statusResponse.Content.ReadAsStringAsync();

            if (!statusResponse.IsSuccessStatusCode)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine($"Erro {statusResponse.StatusCode} ao consultar status do pagamento:");
                Console.WriteLine(statusResponseBody);
                Console.ResetColor();
                return;
            }

            var statusData = JsonSerializer.Deserialize<PaymentStatusList.PaymentStatusResponse>(statusResponseBody, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

            if (statusData == null || !statusData.Success)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("Falha ao obter status do pagamento.");
                Console.ResetColor();
                return;
            }

            Console.WriteLine($"Status do pagamento: {statusData.Data.Status}");
            if (statusData.Data.Status == "approved")
            {
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine($"Chave do jogo: {statusData.Data.Key}");
                if (!string.IsNullOrEmpty(selectedGame.DownloadUrl))
                {
                    Console.WriteLine($"Download do produto: {selectedGame.DownloadUrl}");
                }
                Console.ResetColor();
            }
            else if (statusData.Data.Status == "rejected")
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("Pagamento rejeitado.");
                Console.ResetColor();
            }
            else
            {
                Console.WriteLine("Pagamento pendente. Aguarde a confirmação.");
            }
        }
    }
}
