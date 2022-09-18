using bsy.Filters;
using bsy.Helpers;
using bsy.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace bsy.Controllers
{
    [OturumAcikMI]
    public class GenelController : Controller
    {
        bsyContext context = new bsyContext();

        // GET: Genel
        public ActionResult Index()
        {
            return View();
        }

        public JsonResult SehrinIlceleri(long sehirID)
        {
            IEnumerable<SelectListItem> ilceler = SozlukHelper.sehrinIlceleri(context, sehirID);
            var query = from ix in ilceler
                        select new { text = ix.Text, value = ix.Value };

            return Json(query, JsonRequestBehavior.AllowGet);
        }

        public JsonResult IlceninMahalleleri(long ilceID)
        {
            IEnumerable<SelectListItem> mahalleler = SozlukHelper.IlceninMahalleleri(context, ilceID);
            var query = from ix in mahalleler
                        select new { text = ix.Text, value = ix.Value };

            return Json(query, JsonRequestBehavior.AllowGet);
        }

    }
}