using EASendMail;
using StockWatch.Domain;

namespace StockWatch
{
    public static class EmailSender {

        /// <summary>
        /// Sends an email using the <see cref="Configuration"/> of the program
        /// </summary>
        /// <param name="configuration"> The <see cref="Configuration"/> of the program </param>
        /// <param name="subject"> The subject of the email </param>
        /// <param name="messageBody"> The message body of the email </param>
        public static void SendEmail(Configuration configuration, string subject, string messageBody) {
            // Setting up SMTP email server
            SmtpServer oServer = new SmtpServer(configuration.SmtpServer) {
                User = configuration.OutlookEmail,
                Password = configuration.OutlookPassword,
                Port = 587,
                ConnectType = SmtpConnectType.ConnectSSLAuto,
            };
                
            // Setting up email content
            SmtpMail oMail = new SmtpMail("TryIt") {
                From = configuration.OutlookEmail,
                To = configuration.OutputEmail,
                Subject = subject,
                TextBody = messageBody,
            };

            // Setting up SMTP client            
            SmtpClient oSmtp = new SmtpClient();

            // Sending email
            oSmtp.SendMail(oServer, oMail);
        }    
    }
}
