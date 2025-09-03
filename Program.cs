using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using System.IO;
using Robot_API.Structs;
using Robot_API.ShopPages;

class Program
{
    internal static GameList.Game? selectedGame;

    internal static string purchaseCodeFile = "ultimo_codigo_compra.txt";

    internal static HttpClient _HttpClient()
    {
        var username = "your_username";
        var password = "your_password";
        var baseUrl = "https://api.robotproject.com.br";
        var authToken = Convert.ToBase64String(Encoding.UTF8.GetBytes($"{username}:{password}"));

        var httpClient = new HttpClient { BaseAddress = new Uri(baseUrl) };
        httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", authToken);
        return httpClient;
    }

    static async Task Main()
    {
        try
        {
            while (true)
            {
                Console.Clear();

                string linhaTopo = "+--------------------------------------+";
                string linhaMeio = "+--------------------------------------+";

                Console.ForegroundColor = ConsoleColor.Cyan;
                Console.WriteLine(linhaTopo);
                Console.WriteLine("|          === MENU PRINCIPAL ===       |");
                Console.WriteLine(linhaMeio);
                Console.ResetColor();

                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.WriteLine("| 1 | Loja                            |");
                Console.WriteLine("| 2 | Change Logs                    |");
                Console.WriteLine("| 3 | Validar pagamento              |");
                Console.WriteLine("| 0 | Sair                          |");
                Console.ResetColor();

                Console.ForegroundColor = ConsoleColor.Cyan;
                Console.WriteLine(linhaMeio);
                Console.ResetColor();

                Console.Write("Escolha uma opção: ");

                string opc = Console.ReadLine().Trim();

                using var httpClient = _HttpClient();

                switch (opc)
                {
                    case "1":
                        await ShopPage.AsyncStore(httpClient,false);
                        break;
                    case "2":
                        await ChangeLogs.ShowChangeLogs(httpClient);
                        break;
                    case "3":
                        await ShopPage.AsyncStore(httpClient,true);
                        break;
                    case "0":
                        Console.WriteLine("Encerrando...");
                        return;
                    default:
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.WriteLine("Opção inválida. Tente novamente.");
                        Console.ResetColor();
                        Console.WriteLine("Pressione qualquer tecla para continuar...");
                        Console.ReadKey();
                        break;
                }
            }
        }
        catch (Exception ex)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine($"Erro inesperado: {ex.Message}");
            Console.ResetColor();
        }
    }
}
