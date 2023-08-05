using Newtonsoft.Json;
using StockWatch.Domain;

namespace StockWatch
{
    public class Program {
        static void Main(string[] args) {           
            string stockName = args[0];
            float sellValue = float.Parse(args[1]), buyValue = float.Parse(args[2]);
            Stock stock = new Stock(stockName, sellValue, buyValue);
            Console.WriteLine("[StockWatch]: Bem vindo ao programa de observação de ativos");
            Console.WriteLine($"[StockWatch]: O ativo a ser monitorado é: {stockName}");
            Console.WriteLine($"[StockWatch]: Valor de venda: R$ {sellValue}");
            Console.WriteLine($"[StockWatch]: Valor de compra: R$ {buyValue}");

            // Getting configs form config file
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

            Console.WriteLine("[StockWatch]: Pressione qualquer tecla para iniciar o monitoramento do ativo.");
            Console.ReadLine();

            MonitorStock(stock, configuration);

            Console.WriteLine("[StockWatch]: Encerrando StockWatch.");
        }

        /// <summary>
        /// Monitors a <see cref="Stock"/> while the program is running, if the stock value reaches the Sellig or acquistion value, and email
        /// making a recomended action will be sent to the output email in the <see cref="Configuration"/>
        /// </summary>
        /// <param name="stock"> The <see cref="Stock"/> to be monitored </param>
        /// <param name="configuration"> The <see cref="Configuration"/> of the program </param>
        public static void MonitorStock(Stock stock, Configuration configuration) {
            bool error = false;
            float currentStockValue = 0;
            HttpClient httpClient = new HttpClient();

            while(!error) {
                try {
                    var response = httpClient.GetAsync($"https://brapi.dev/api/quote/{stock.Name}?range=1d&interval=1d&fundamental=true&dividends=true").Result;
                    var jsonResult = response.Content.ReadAsStringAsync().Result;
                    BrapiApiResponse brapiApiResponse = JsonConvert.DeserializeObject<BrapiApiResponse>(jsonResult);
                    float newStockValue = brapiApiResponse.Results.FirstOrDefault().RegularMarketPrice;

                    // Update current stock value if the new value is diferent and check if email is necessary
                    if(newStockValue != currentStockValue) {
                        Console.WriteLine($"[StockWatch]: O valor atual do ativo é de R$ {newStockValue} - (Atualizado em {DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss")})");

                        string subject = string.Empty, messageBody = string.Empty, action = string.Empty;

                        // Check if a action will be recomended
                        if(newStockValue >= stock.SellingValue) {
                            action = "Venda";
                            Console.WriteLine($"[StockWatch]: Venda de ativo recomendada, enviando email para {configuration.OutputEmail}");
                            subject = $"Recomendação  de Venda de {stock.Name}";
                            messageBody = $"StockWatch recomenda a venda dos ativos de {stock.Name} baseado em seu valor atual de R$ {newStockValue}.\n\nStockWatch";
                        } else if(newStockValue <= stock.AcquisitionValue) {
                            action = "Compra";
                            subject = $"Recomendação  de Compra de {stock.Name}";
                            messageBody = $"StockWatch recomenda a compra dos ativos de {stock.Name} baseado em seu valor atual de R$ {newStockValue}.\n\nStockWatch";
                        }

                        // If a action was recomended send the email
                        if(!string.IsNullOrEmpty(action)) {
                            EmailSender.SendEmail(configuration, subject, messageBody);
                            Console.WriteLine($"[StockWatch]: Email de recomendação de {action} enviado para {configuration.OutputEmail}");
                        }

                        currentStockValue = newStockValue;
                    }
                } catch(Exception ex) {
                    Console.WriteLine($"[StockWatch]: ERRO - Erro durante monitoramento de ativo: {ex.GetBaseException().Message}\n encerrando monitoramento");
                    // Stop monitoring if a error happens
                    error = true;
                }
            }
        }
    } 
}
