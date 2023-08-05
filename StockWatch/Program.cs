using Newtonsoft.Json;

namespace StockWatch {
    class Program {
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

            try {
                EmailSender.SendEmail(configuration, "Test Email", "Hey this is a test email from stock watch");
                Console.Write($"[StockWatch]: email enviado para o destino {configuration.OutputEmail})");
            }
            catch (Exception ex) {
                Console.WriteLine($"[StockWatch]: ERRO - Erro ao enviar email: {ex.GetBaseException().Message}");
            }                  
        }
    }
}
