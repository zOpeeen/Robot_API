using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace Robot_API.Structs
{
    internal static class GameList
    {

        public class Game
        {
            public int Id { get; set; }
            public string? Name { get; set; }
            public string? Version { get; set; }
            public string? Status { get; set; }
            public string? Icon { get; set; }
            public string? Gender { get; set; }
            public string? DownloadUrl { get; set; }
            public Prices? Prices { get; set; }
            public int MaxKeys { get; set; }
            public int SoldKeys { get; set; }
        }

        public class Prices
        {
            public float Dia1 { get; set; }
            public float Dia7 { get; set; }
            public float Dia30 { get; set; }

            public static Prices FromJson(JsonElement json)
            {
                float TryGet(JsonElement element, string key)
                {
                    return element.TryGetProperty(key, out var value) ? value.GetSingle() : 0f;
                }

                return new Prices
                {
                    Dia1 = TryGet(json, "1"),
                    Dia7 = TryGet(json, "7"),
                    Dia30 = TryGet(json, "30")
                };
            }
        }

        public class GameConverter : System.Text.Json.Serialization.JsonConverter<Game>
        {
            public override Game Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
            {
                var jsonDoc = JsonDocument.ParseValue(ref reader);
                var root = jsonDoc.RootElement;

                string GetStringOrDefault(string propName)
                    => root.TryGetProperty(propName, out var prop) ? prop.GetString() : "";

                int GetIntOrDefault(string propName)
                    => root.TryGetProperty(propName, out var prop) && prop.TryGetInt32(out var val) ? val : 0;

                // Tentar obter prices, pode não existir
                Prices prices = new Prices();
                if (root.TryGetProperty("prices", out var pricesElement))
                {
                    prices = Prices.FromJson(pricesElement);
                }
                else
                {
                    Console.WriteLine($"Jogo \"{GetStringOrDefault("name")}\" está sem preços.");
                }

                return new Game
                {
                    Id = GetIntOrDefault("id"),
                    Name = GetStringOrDefault("name"),
                    Version = GetStringOrDefault("version"),
                    Status = GetStringOrDefault("status"),
                    Icon = GetStringOrDefault("icon"),
                    Gender = GetStringOrDefault("gender"),
                    DownloadUrl = GetStringOrDefault("downloadUrl"),
                    Prices = prices,
                    MaxKeys = GetIntOrDefault("maxKeys"),
                    SoldKeys = GetIntOrDefault("soldKeys")
                };
            }


            public override void Write(Utf8JsonWriter writer, Game value, JsonSerializerOptions options)
            {
                throw new NotImplementedException();
            }
        }
    }
}
