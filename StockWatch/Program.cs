using System.IO;
using Newtonsoft.Json;

namespace StockWatch {
    class Program {
        static void Main(string[] args) {
            Console.WriteLine("[Bem vindo ao programa de observação de ativos]");
            Configuration configuration = new Configuration();

            var serializer = new JsonSerializer();
            using(var streamReader = new StreamReader("config.json"))
            using(var textReader = new JsonTextReader(streamReader)) {
                configuration = serializer.Deserialize<Configuration>(textReader);
            }

            Console.WriteLine($"Email padrão: {configuration.Email}");
        }
    }
}
