
using System;
using System.Net;
using System.Net.Mail;
using System.Security.Cryptography;
using System.Configuration;

namespace InventoryManagementLibrary.Helpers
{
    public static class EmailHelper
    {
        public static void SendEmail(string toEmail, string subject, string body)
        {
            string fromEmail = ConfigurationManager.AppSettings["EmailUsername"];
            string password = ConfigurationManager.AppSettings["EmailPassword"];
            var smtp = new SmtpClient("smtp.gmail.com", 587)
            {
                Credentials = new NetworkCredential(fromEmail, password),
                EnableSsl = true
            };

            var mail = new MailMessage(fromEmail, toEmail, subject, body);
            mail.IsBodyHtml = true;
            smtp.Send(mail);
        }

        public static int GenerateOtp()
        {
            using (var rng = new RNGCryptoServiceProvider())
            {
                byte[] bytes = new byte[4];
                rng.GetBytes(bytes);
                int value = Math.Abs(BitConverter.ToInt32(bytes, 0));
                return (value % 900000) + 100000; 
            }
        }

    }
}
