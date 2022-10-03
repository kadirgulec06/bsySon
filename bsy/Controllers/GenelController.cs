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
            User user = (User)Session["USER"];

            if (user.gy.butunTurkiye)
            {
                IEnumerable<SelectListItem> ilceler = SozlukHelper.sehrinIlceleri(context, sehirID);
                var query = from ix in ilceler
                            select new { text = ix.Text, value = ix.Value };
                return Json(query, JsonRequestBehavior.AllowGet);
            }

            var tumIlceler = (from ix in context.tblSozluk
                           where ix.Turu == SozlukHelper.ilceKodu &&
                                 ix.BabaID == sehirID
                           select new
                           {
                               ilceID = ix.id,
                               ilceADI = ix.Aciklama
                           }).ToList();

            List<long> gorevIlceleri = KullaniciHelper.gorevIlceleri(context, user.gy);
            var sonuc = from ix in tumIlceler
                        join gi in gorevIlceleri on ix.ilceID equals gi
                        select new
                        {
                            text = ix.ilceADI,
                            value = ix.ilceID.ToString()
                        };

            return Json(sonuc, JsonRequestBehavior.AllowGet);

        }

        public JsonResult IlceninMahalleleri(long ilceID)
        {
            User user = (User)Session["USER"];

            if (user.gy.butunTurkiye)
            {
                IEnumerable<SelectListItem> mahalleler = SozlukHelper.IlceninMahalleleri(context, ilceID, 0);
                var query = from ix in mahalleler
                            select new { text = ix.Text, value = ix.Value };
                return Json(query, JsonRequestBehavior.AllowGet);
            }

            var tumMahalleler = (from ix in context.tblSozluk
                              where ix.Turu == SozlukHelper.mahalleKodu &&
                                    ix.BabaID == ilceID
                              select new
                              {
                                  mahalleID = ix.id,
                                  mahalleADI = ix.Aciklama
                              }).ToList();

            List<long> gorevMahalleleri = KullaniciHelper.gorevMahalleleri(context, user.gy);
            var sonuc = from ix in tumMahalleler
                        join gi in gorevMahalleleri on ix.mahalleID equals gi
                        select new
                        {
                            text = ix.mahalleADI,
                            value = ix.mahalleID.ToString()
                        };

            return Json(sonuc, JsonRequestBehavior.AllowGet);

        }
    }
}