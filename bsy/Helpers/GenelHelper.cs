using bsy.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Security.Cryptography;
using System.Text;
using System.Web;

namespace bsy.Helpers
{
    public static class GenelHelper
    {
        /*
        SmtpClient client = new SmtpClient();
        client.Port = 587;
                    client.Host = "smtp.gmail.com";
                    client.EnableSsl = true;
                    client.Timeout = 10000;
                    client.DeliveryMethod = SmtpDeliveryMethod.Network;
                    client.UseDefaultCredentials = false;
                    client.Credentials = new System.Net.NetworkCredential("mail@gmail.com", "password");
                    MailMessage mm = new MailMessage(Text, Text, "Text", Text);
        mm.BodyEncoding = UTF8Encoding.UTF8;
                    mm.DeliveryNotificationOptions = DeliveryNotificationOptions.OnFailure;
                    client.Send(mm);
                    MessageBox.Show("Mesajiniz gonderildi:)");
            */

        public static bool sendMail(string to, string toName, string konu, string mesaj)
        {
            return true;

            var fromAddress = new MailAddress("kadirgulec59@gmail.com", "BSY Portalı");
            var toAddress = new MailAddress(to, toName);
            const string fromPassword = "homhrjgdsnimmsyz";
            //const string fromPassword = "Kato36Zato34Nato36.34";
            string subject = konu;
            string body = mesaj;

            var smtp = new SmtpClient
            {                
                Timeout = 10000,
                Host = "smtp.gmail.com",
                Port = 587,  //465
                EnableSsl = true,
                DeliveryMethod = SmtpDeliveryMethod.Network,
                UseDefaultCredentials = false,
                Credentials = new NetworkCredential(fromAddress.Address, fromPassword)
            };
            using (var message = new MailMessage(fromAddress, toAddress)
            {
                Subject = subject,
                Body = body,
                BodyEncoding = UTF8Encoding.UTF8,
                DeliveryNotificationOptions = DeliveryNotificationOptions.OnFailure
            })
            {
                smtp.Send(message);
            }
            return true;
        }

        public static bool dizideVar(string aranan, string[] dizi)
        {
            foreach (string eleman in dizi)
            {
                if (eleman == aranan)
                {
                    return true;
                }
            }

            return false;
        }
        public static string exceptionMesaji(Exception ex)
        {
            string mesaj = ex.Message;
            Exception ie = ex.InnerException;
            while (ie != null)
            {
                mesaj = mesaj + "\\r\\n" + ie.Message;
                ie = ie.InnerException;
            }

            return mesaj;
        }

    }
}