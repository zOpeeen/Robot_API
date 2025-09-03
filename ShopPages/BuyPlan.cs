using Robot_API.Structs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace Robot_API.ShopPages
{
    internal static class BuyPlan
    {
        internal static async Task SelectPlans(GameList.Game selectedGame, HttpClient httpClient, string compraCodeFile)
        {
            while (true)
            {
                Console.Clear();
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.WriteLine($"Jogo selecionado: {selectedGame.Name} (v{selectedGame.Version})");
                Console.ResetColor();

                var actualDownloadUrl = selectedGame.DownloadUrl;

                Console.WriteLine(new string('-', 50));
                Console.WriteLine("Planos disponíveis (VALORES EM DOLAR):");
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.WriteLine($"[1]  1 dia   - ${selectedGame.Prices.Dia1}");
                Console.WriteLine($"[7]  7 dias  - ${selectedGame.Prices.Dia7}");
                Console.WriteLine($"[30] 30 dias - ${selectedGame.Prices.Dia30}");
                Console.ResetColor();
                Console.WriteLine(new string('-', 50));
                Console.WriteLine("[0] Voltar para a lista de jogos\n");

                Console.Write("Digite a quantidade de dias desejada: ");
                if (!int.TryParse(Console.ReadLine(), out int dias))
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine("Entrada inválida.");
                    Console.ResetColor();
                    Console.WriteLine("Pressione qualquer tecla para continuar...");
                    Console.ReadKey();
                    continue;
                }

                if (dias == 0)
                    break;

                if (dias != 1 && dias != 7 && dias != 30)
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine("Plano inválido.");
                    Console.ResetColor();
                    Console.WriteLine("Pressione qualquer tecla para continuar...");
                    Console.ReadKey();
                    continue;
                }

                Console.Write("Deseja usar cupom de desconto? (s/n): ");
                string usarCupom = Console.ReadLine().Trim().ToLower();
                string cupom = "";
                if (usarCupom == "s")
                {
                    Console.Write("Digite o código do cupom: ");
                    cupom = Console.ReadLine().Trim();
                }

                Console.WriteLine("\nInforme os dados do pagador:");
                Console.Write("Nome completo: ");
                string nomePagador = Console.ReadLine().Trim();
                Console.Write("CPF (somente números e pontuação): ");
                string cpf = Console.ReadLine().Trim();
                Console.Write("Email: ");
                string email = Console.ReadLine().Trim();

                var compraBody = new
                {
                    duration = dias,
                    coupon = string.IsNullOrEmpty(cupom) ? null : cupom,
                    payer = new
                    {
                        name = nomePagador,
                        cpf = cpf,
                        email = email
                    }
                };

                var compraJson = JsonSerializer.Serialize(compraBody, new JsonSerializerOptions { IgnoreNullValues = true });

                var request = new HttpRequestMessage(HttpMethod.Post, $"/purchase/game/{selectedGame.Id}")
                {
                    Content = new StringContent(compraJson, Encoding.UTF8, "application/json")
                };

                var compraResponse = await httpClient.SendAsync(request);
                var compraResponseBody = await compraResponse.Content.ReadAsStringAsync();

                if (!compraResponse.IsSuccessStatusCode)
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine($"Erro {compraResponse.StatusCode} ao iniciar compra:");
                    Console.WriteLine(compraResponseBody);
                    Console.ResetColor();
                    Console.WriteLine("Pressione qualquer tecla para voltar...");
                    Console.ReadKey();
                    break;
                }

                var compraData = JsonSerializer.Deserialize<PaymentList.PurchaseResponse>(compraResponseBody, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

                if (compraData == null || !compraData.Success)
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine("Falha ao processar compra.");
                    Console.ResetColor();
                    Console.WriteLine("Pressione qualquer tecla para voltar...");
                    Console.ReadKey();
                    break;
                }


                Console.Clear();
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine("Compra iniciada com sucesso!\n");
                Console.ResetColor();

                Console.WriteLine($"Jogo: {compraData.Data.Game.Name}");
                Console.WriteLine($"Plano: {dias} dias");
                Console.WriteLine($"Valor dolar: ${compraData.Data.OriginalAmount}");
                Console.WriteLine($"Valor real convertido: R${compraData.Data.Amount}");
                if (compraData.Data.CouponApplied != null)
                {
                    Console.WriteLine($"Cupom aplicado: {compraData.Data.CouponApplied.Code} - Desconto: {compraData.Data.CouponApplied.Discount}{compraData.Data.CouponApplied.DiscountType} ({compraData.Data.CouponApplied.DiscountAmount})");
                }
                Console.WriteLine();

                // Salvar código da compra para futuras consultas
                if (compraData.Data.Payment.Id != default)
                {
                    File.WriteAllText(compraCodeFile, compraData.Data.Payment.Id);
                    Console.ForegroundColor = ConsoleColor.Green;
                    Console.WriteLine($"\nSeu código de compra é: {compraData.Data.Payment.Id}");
                    Console.WriteLine("Guarde este código para consultar seu pagamento futuramente.");
                    Console.ResetColor();
                }
                Console.WriteLine();
                Console.WriteLine();

                if (!string.IsNullOrEmpty(compraData.Data.Payment.Qrcode))
                {
                    Console.WriteLine($"QR Code (URL): {compraData.Data.Payment.Qrcode}");
                }
                else if (!string.IsNullOrEmpty(compraData.Data.Payment.TicketUrl))
                {
                    Console.WriteLine($"Ticket para pagamento: {compraData.Data.Payment.TicketUrl}");
                }
                else if (!string.IsNullOrEmpty(compraData.Data.Payment.QrCodeBase64))
                {
                    Console.WriteLine("QR Code (Base64):");
                    Console.WriteLine(compraData.Data.Payment.QrCodeBase64);
                }

                Console.WriteLine("\nAguarde a confirmação do pagamento. (Pressione Ctrl+C para cancelar)\n");

                // Loop para verificar status do pagamento sem poluir console
                while (true)
                {
                    Console.Write("\rConsultando status do pagamento...                   ");

                    var paymentStatusRequest = new HttpRequestMessage(HttpMethod.Get, $"/payment/status/{compraData.Data.Payment.Id}");
                    var statusResponse = await httpClient.SendAsync(paymentStatusRequest);
                    var statusResponseBody = await statusResponse.Content.ReadAsStringAsync();

                    if (!statusResponse.IsSuccessStatusCode)
                    {
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.WriteLine($"\nErro {statusResponse.StatusCode} ao consultar status do pagamento:");
                        Console.WriteLine(statusResponseBody);
                        Console.ResetColor();
                        Console.WriteLine("Pressione qualquer tecla para tentar novamente...");
                        Console.ReadKey();
                        continue;
                    }

                    var statusData = JsonSerializer.Deserialize<PaymentStatusList.PaymentStatusResponse>(statusResponseBody, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

                    if (statusData == null || !statusData.Success)
                    {
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.WriteLine("\nFalha ao obter status do pagamento.");
                        Console.ResetColor();
                        Console.WriteLine("Pressione qualquer tecla para tentar novamente...");
                        Console.ReadKey();
                        continue;
                    }

                    if (statusData.Data.Status == "approved")
                    {
                        Console.WriteLine("\rPagamento aprovado!                            ");
                        Console.ForegroundColor = ConsoleColor.Green;
                        Console.WriteLine($"Chave do jogo: {statusData.Data.Key}");
                        Console.ResetColor();

                        if (!string.IsNullOrEmpty(selectedGame.DownloadUrl))
                        {
                            Console.WriteLine($"Download do produto: {selectedGame.DownloadUrl}");
                        }

                        // Apagar o arquivo de código pois o pagamento foi concluído
                        if (File.Exists(compraCodeFile))
                            File.Delete(compraCodeFile);

                        break;
                    }
                    else if (statusData.Data.Status == "rejected")
                    {
                        Console.WriteLine("\rPagamento rejeitado.                             ");
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.WriteLine("Tente novamente ou entre em contato com o suporte.");
                        Console.ResetColor();
                        break;
                    }
                    else
                    {
                        // Status pendente, aguardar
                        await Task.Delay(10000);
                    }
                }

                Console.WriteLine("\nPressione qualquer tecla para voltar à lista de jogos...");
                Console.ReadKey();
                break;
            }
        }
    }
}
