using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using StockWatch.Domain;
using System.Net.Http;
using System.Net.Http.Json;

namespace StockWatch
{
    public class Program {
        static void Main(string[] args) {
            Console.WriteLine("[StockWatch]: Bem vindo ao programa de observação de ativos");
            Configuration? configuration = new Configuration();

            var serializer = new JsonSerializer();
            using(var streamReader = new StreamReader("config.json"))
            using(var textReader = new JsonTextReader(streamReader)) {
                configuration = serializer.Deserialize<Configuration>(textReader);
            }

            if(configuration == null) {
                Console.Write("[StockWatch]: ERRO - Erro ao ler arquivo de configurações, por favor verifique o arquivo config.json");
                return;
            }

            HttpClient httpClient = new HttpClient();

            try {
                var test = httpClient.GetAsync("https://brapi.dev/api/quote/MGLU3?range=1d&interval=1d&fundamental=true&dividends=true").Result;
                var jsonString = test.Content.ReadAsStringAsync().Result;
                BrapiApiResponse brapiApiResponse = JsonConvert.DeserializeObject<BrapiApiResponse>(jsonString);
                Console.WriteLine($"[StockWatch]: O valor atual do ativo é de R$ {brapiApiResponse.Results.FirstOrDefault().RegularMarketPrice}");
            } 
            catch(Exception ex) {
                Console.WriteLine($"[StockWatch]: ERRO - Erro ao acessar API de ativos");
            }

            //try {
            //    EmailSender.SendEmail(configuration, "Test Email", "Hey this is a test email from stock watch");
            //    Console.Write($"[StockWatch]: email enviado para o destino {configuration.OutputEmail})");
            //}
            //catch (Exception ex) {
            //    Console.WriteLine($"[StockWatch]: ERRO - Erro ao enviar email: {ex.GetBaseException().Message}");
            //}                  
        }
    }

   
}
