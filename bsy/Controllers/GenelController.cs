using bsy.Filters;
using bsy.Helpers;
using bsy.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

/*

Get last record of each group in group by  
  
             from p in PersonOrders
                //where conditions or joins with other tables to be included here
            group p by p.PersonID into grp
            let MaxOrderDatePerPerson = grp.Max(g => g.OrderDate)

            from p in grp
            where p.OrderDate == MaxOrderDatePerPerson
            select p


 */

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
        public JsonResult SehrinIlceleri(long SehirID)
        {
            User user = (User)Session["USER"];

            if (user.gy.butunTurkiye)
            {
                IEnumerable<SelectListItem> ilceler = SozlukHelper.sehrinIlceleri(context, SehirID);
                var query = from ix in ilceler
                            select new { text = ix.Text, value = ix.Value };
                return Json(query, JsonRequestBehavior.AllowGet);
            }

            var tumIlceler = (from ix in context.tblSozluk
                           where ix.Turu == SozlukHelper.ilceKodu &&
                                 ix.BabaID == SehirID
                           select new
                           {
                               IlceID = ix.id,
                               ilceADI = ix.Aciklama
                           }).ToList();

            List<long> gorevIlceleri = KullaniciHelper.gorevIlceleri(context, user.gy);
            var sonuc = from ix in tumIlceler
                        join gi in gorevIlceleri on ix.IlceID equals gi
                        select new
                        {
                            text = ix.ilceADI,
                            value = ix.IlceID.ToString()
                        };

            return Json(sonuc, JsonRequestBehavior.AllowGet);

        }

        public JsonResult IlceninMahalleleri(long IlceID)
        {
            User user = (User)Session["USER"];

            if (user.gy.butunTurkiye)
            {
                IEnumerable<SelectListItem> mahalleler = SozlukHelper.IlceninMahalleleri(context, IlceID, 0);
                var query = from ix in mahalleler
                            select new { text = ix.Text, value = ix.Value };
                return Json(query, JsonRequestBehavior.AllowGet);
            }

            var tumMahalleler = (from ix in context.tblSozluk
                              where ix.Turu == SozlukHelper.mahalleKodu &&
                                    ix.BabaID == IlceID
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