using bsy.ViewModels.Mesaj;
using Nexmo.Api;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using Vonage;
using Vonage.Request;

namespace bsy.Controllers
{
    public class MesajController : Controller
    {
        // GET: Mesaj
        public ActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Index(SMSMesaji mesaj)
        {
            return View();

            var credentials = Credentials.FromApiKeyAndSecret(
                   "fe019b51",
                  "t8YQjjZe8nmEUj4c"
             );

            var vonageClient = new VonageClient(credentials);

            Vonage.Messaging.SendSmsRequest sms =
            new Vonage.Messaging.SendSmsRequest
            {
                To = "905514260656",
                From = "Kadir Güleç",
                Text = mesaj.ContentMsg
            };


            var response = vonageClient.SmsClient.SendAnSms(sms);

            return View();
        }

    }
}