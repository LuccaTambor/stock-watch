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
            string stockName = args[0];
            float sellValue = float.Parse(args[1]);
            float buyValue = float.Parse(args[2]);

            Console.WriteLine($"[StockWatch]: O ativo a ser monitorado é: {stockName}");
            Console.WriteLine($"[StockWatch]: Valor de venda: R$ {sellValue}");
            Console.WriteLine($"[StockWatch]: Valor de compra: R$ {buyValue}");

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
                var response = httpClient.GetAsync($"https://brapi.dev/api/quote/{stockName}?range=1d&interval=1d&fundamental=true&dividends=true").Result;
                var jsonResult = response.Content.ReadAsStringAsync().Result;
                BrapiApiResponse brapiApiResponse = JsonConvert.DeserializeObject<BrapiApiResponse>(jsonResult);
                Console.WriteLine($"[StockWatch]: O valor atual do ativo é de R$ {brapiApiResponse.Results.FirstOrDefault().RegularMarketPrice}");
            } 
            catch(Exception ex) {
                Console.WriteLine($"[StockWatch]: ERRO - Erro ao acessar API de ativos");
            }

            Console.ReadLine();

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
