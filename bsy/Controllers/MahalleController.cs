using bsy.Filters;
using bsy.Helpers;
using bsy.Models;
using bsy.ViewModels.Mahalle;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Linq.Dynamic;
using System.Transactions;
using System.Web;
using System.Web.Mvc;

namespace bsy.Controllers
{
    [OturumAcikMI]
    [Yetkili(Roles = "YONETICI,SAHAGOREVLISI")]
    public class MahalleController : Controller
    {
        bsyContext context = new bsyContext();
        // GET: Ilce
        public ActionResult Index()
        {
            Session["SehirID"] = 0;
            Session["IlceID"] = 0;

            ViewBag.IlkGiris = 1;
            ViewBag.SehirID = 0;
            ViewBag.IlceID = 0;

            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Index(long SehirID = 0, long IlceID = 0)
        {
            ViewBag.IlkGiris = 0;
            ViewBag.SehirID = SehirID;
            ViewBag.IlceID = IlceID;

            Session["SehirID"] = SehirID;
            Session["IlceID"] = IlceID;

            return View();
        }

        public ActionResult ListeMahalleler(string sidx, string sord, int page, int rows, byte ilkGiris = 0, long SehirID=0, long IlceID = 0)
        {
            // filtre parametrelerini hazırla

            string sehir = "";
            string ilce = "";
            string mahalle = "";

            if (Request.Params["_search"] == "true")
            {
                if (Request.Params["SEHIR"] != null)
                {
                    sehir = Request.Params["SEHIR"];
                }
                if (Request.Params["ILCE"] != null)
                {
                    ilce = Request.Params["ILCE"];
                }

                if (Request.Params["MAHALLE"] != null)
                {
                    mahalle = Request.Params["MAHALLE"];
                }

            }

            int rapor = 2;
            if (rapor == 0)
            {
                Session["filtreSEHIR"] = sehir.ToUpper();
                Session["filtreILCE"] = ilce.ToUpper();
                Session["filtreMAHALLE"] = mahalle.ToUpper();
            }
            else if (rapor == 1)
            {
                sehir = (string)Session["filtreSEHIR"];
                ilce = (string)Session["filtreILCE"];
                mahalle = (string)Session["filtreMAHALLE"];
            }

            int pageIndex = Convert.ToInt32(page) - 1;

            int pageSize = rows;

            //long ilID = (long)Session["ilID"];
            //pageSize = 5;
            var query = (from mh in context.tblMahalleler
                         join sx in context.tblSozluk on mh.id equals sx.id
                         join ic in context.tblIlceler on mh.IlceID equals ic.id
                         join sh in context.tblSozluk on ic.SehirID equals sh.id
                         join sy in context.tblSozluk on mh.IlceID equals sy.id
                         where
                            (mh.IlceID == IlceID || IlceID == 0) &&
                            (ic.SehirID == SehirID || SehirID == 0) &&
                            (sx.Aciklama + "").Contains(mahalle) &&
                            (sy.Aciklama + "").Contains(ilce) &&
                            (sh.Aciklama + "").Contains(sehir)
                         select new
                         {
                             mh.id,
                             mahalleKODU = mh.MahalleKodu,
                             mahalleADI = sx.Aciklama,
                             mh.IlceID,
                             //ilceKODU = sy.Kodu,
                             ilceADI = sy.Aciklama,
                             ic.SehirID,
                             //sehirKodu = sh.Kodu,
                             sehirADI = sh.Aciklama,
                             mh.Aciklama
                         });

            int totalRecords = query.Count();
            int totalPages = (int)Math.Ceiling((float)totalRecords / (float)pageSize);

            var resultSetAfterOrderandPaging = query.OrderBy("sehirADI, ilceADI").Skip(pageIndex * pageSize).Take(pageSize);

            var resultSet = (from mhx in resultSetAfterOrderandPaging
                             select new
                             {
                                 mhx.id,
                                 mhx.mahalleKODU,
                                 mhx.mahalleADI,
                                 mhx.IlceID,
                                 //mhx.ilceKODU,
                                 mhx.ilceADI,
                                 mhx.SehirID,
                                 //mhx.sehirKodu,
                                 mhx.sehirADI,
                                 mhx.Aciklama,
                                 Degistir = 0,
                                 Sil = 0
                             }).ToList();


            //int totalRecords = resultSet.Count();
            //int totalPages = (int)Math.Ceiling((float)totalRecords / (float)pageSize);

            //Data to sent grid
            var jsonData = new
            {
                total = totalPages, //todo: calculate
                page = page,
                records = totalRecords,
                rows = (
                  from mhx in resultSet
                  select new
                  {
                      cell = new string[]
                      {
                                 mhx.id.ToString(),
                                 mhx.IlceID.ToString(),
                                 mhx.SehirID.ToString(),
                                 //mhx.sehirKodu,
                                 mhx.sehirADI,
                                 //mhx.ilceKODU,
                                 mhx.ilceADI,
                                 mhx.mahalleKODU,
                                 mhx.mahalleADI,
                                 mhx.Aciklama,
                                 mhx.Degistir.ToString(),
                                 mhx.Sil.ToString()
                       }
                  }).ToArray()
            };

            return Json(jsonData, JsonRequestBehavior.AllowGet);
        }

        public ActionResult YeniMahalle(long id = 0)
        {
            List<Mesaj> mesajlar = new List<Mesaj>();
            Mesaj m = null;

            SOZLUK soz = null;
            MAHALLE mahalle = null;
            try
            {
                soz = context.tblSozluk.Find(id);
                mahalle = context.tblMahalleler.Find(id);
            }
            catch (Exception ex)
            {

            }

            if (soz == null)
            {
                soz = new SOZLUK();
                soz.Turu = SozlukHelper.mahalleKodu;
                mahalle = new MAHALLE();
            }

            MahalleVM mahalleVM = new MahalleVM();
            mahalleVM = mahalleHazirla(soz, mahalle);

            Session["SehirID"] = 0;

            return View(mahalleVM);
        }

        private MahalleVM mahalleHazirla(SOZLUK soz, MAHALLE mahalle)
        {
            MahalleVM mahalleVM = new MahalleVM();

            soz.BabaID = mahalle.IlceID;
            mahalleVM.sozluk = soz;
            mahalleVM.mahalle = mahalle;

            mahalleVM.sehirler = SozlukHelper.sozlukKalemleriDD(context, "SEHIR", 0, 0);
            mahalleVM.ilceler = SozlukHelper.sozlukKalemleriDD(context, "ILCE", 0, 0);
            mahalleVM.ilceADI = SozlukHelper.sozlukAciklama(context, mahalle.IlceID);

            return mahalleVM;
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult YeniMahalle(MahalleVM yeniMahalleVM, string btnSubmit)
        {
            MahalleVM eskiMahalleVM = null;

            List<Mesaj> mesajlar = new List<Mesaj>();
            Mesaj m = null;

            //yeniSozluk.Turu = SozlukHelper.rolTuru;
            if (!ModelState.IsValid)
            {
                return View(yeniMahalleVM);
            }

            using (TransactionScope scope = new TransactionScope(TransactionScopeOption.Required))
            {
                try
                {
                    eskiMahalleVM = eskiMahalleYarat(yeniMahalleVM.sozluk.id);
                    //yeniAO = YeniOzetHesapla(yeniAO, mao);
                    eskiMahalleVM = VMYeniToEski(eskiMahalleVM, yeniMahalleVM);

                    bool yeniMahalle = false;
                    if (eskiMahalleVM.sozluk.id == 0)
                    {
                        yeniMahalle = true;
                        var mahalleKaydi = (from sz in context.tblSozluk
                                         join mh in context.tblMahalleler on sz.id equals mh.id
                                         select new { sz.id, sz.Aciklama, mh.IlceID }).FirstOrDefault();
                        if (mahalleKaydi != null && eskiMahalleVM.sozluk.Aciklama == mahalleKaydi.Aciklama && eskiMahalleVM.mahalle.IlceID == mahalleKaydi.IlceID)
                        {
                            m = new Mesaj("hata", "Bu mahalle kaydı daha önce oluşturulmuş, tekrar eklenemez");
                            mesajlar.Add(m);
                            Session["MESAJLAR"] = mesajlar;
                            return View(yeniMahalleVM);
                        }
                        else
                        {
                            context.tblSozluk.Add(eskiMahalleVM.sozluk);
                            context.SaveChanges();
                            eskiMahalleVM.mahalle.id = eskiMahalleVM.sozluk.id; // SozlukHelper.sozlukID(context, eskiIlceVM.sozluk);
                            context.tblMahalleler.Add(eskiMahalleVM.mahalle);

                            m = new Mesaj("tamam", "Mahalle Kaydı Eklenmiştir.");
                        }
                    }
                    else
                    {
                        context.Entry(eskiMahalleVM.sozluk).State = EntityState.Modified;
                        context.Entry(eskiMahalleVM.mahalle).State = EntityState.Modified;

                        m = new Mesaj("tamam", "Mahalle Kaydı Güncellenmiştir.");
                    }

                    try
                    {
                        context.SaveChanges();
                        scope.Complete();
                    }
                    catch (Exception e)
                    {
                        m = new Mesaj("hata", "Mahalle kaydı güncelleneMEdi=>" + GenelHelper.exceptionMesaji(e));
                    }
                }
                catch (Exception ex)
                {
                    m = new Mesaj("hata", "Mahalle kaydı güncelleneMEdi=>" + GenelHelper.exceptionMesaji(ex));
                }
            }

            mesajlar.Add(m);
            Session["MESAJLAR"] = mesajlar;

            Response.Redirect(Request.Url.Scheme + "://" + Request.Url.Authority + Request.ApplicationPath + "/Mahalle/Index", false);
            return Content("OK");

        }

        private MahalleVM eskiMahalleYarat(long id)
        {
            MahalleVM mahalleVM = new MahalleVM();

            SOZLUK eskiSozluk = context.tblSozluk.Find(id);
            if (eskiSozluk == null)
            {
                eskiSozluk = new SOZLUK();
            }

            MAHALLE eskiMahalle = context.tblMahalleler.Find(id);
            if (eskiMahalle == null)
            {
                eskiMahalle = new MAHALLE();
            }

            eskiSozluk.Turu = SozlukHelper.mahalleKodu;

            mahalleVM.mahalle = eskiMahalle;
            mahalleVM.sozluk = eskiSozluk;
            mahalleVM.ilceADI = SozlukHelper.sozlukAciklama(context, eskiMahalle.IlceID);

            return mahalleVM;

        }
        private MahalleVM VMYeniToEski(MahalleVM eskiVM, MahalleVM yeniVM)
        {
            eskiVM.sozluk = SozlukYeniToEski(eskiVM.sozluk, yeniVM.sozluk);
            eskiVM.sozluk.BabaID = yeniVM.mahalle.IlceID;
            eskiVM.mahalle = MahalleYeniToEski(eskiVM.mahalle, yeniVM.mahalle);
            eskiVM.ilceADI = yeniVM.ilceADI;

            return eskiVM;
        }

        private SOZLUK SozlukYeniToEski(SOZLUK eskiSozluk, SOZLUK yeniSozluk)
        {
            eskiSozluk.id = yeniSozluk.id;
            eskiSozluk.Turu = SozlukHelper.mahalleKodu;
            //eskiSozluk.Kodu = yeniSozluk.Kodu;
            eskiSozluk.Aciklama = yeniSozluk.Aciklama;

            return eskiSozluk;
        }

        private MAHALLE MahalleYeniToEski(MAHALLE eskiMahalle, MAHALLE yeniMahalle)
        {
            eskiMahalle.id = yeniMahalle.id;
            eskiMahalle.MahalleKodu = yeniMahalle.MahalleKodu;
            eskiMahalle.IlceID = yeniMahalle.IlceID;
            eskiMahalle.Aciklama = yeniMahalle.Aciklama;

            return eskiMahalle;
        }


        public ActionResult MahalleSil(long idSil)
        {
            long id = idSil;

            List<Mesaj> mesajlar = new List<Mesaj>();
            Mesaj m = null;

            MAHALLE mahalle = context.tblMahalleler.Find(id);
            context.Entry(mahalle).State = EntityState.Deleted;

            SOZLUK sozluk = context.tblSozluk.Find(id);
            context.Entry(sozluk).State = EntityState.Deleted;

            try
            {
                context.SaveChanges();
                m = new Mesaj("tamam", "Mahalle Kaydı Silinmiştir.");
            }
            catch (Exception e)
            {
                m = new Mesaj("hata", "Mahalle Kaydı Silinemedi=>" + GenelHelper.exceptionMesaji(e));
            }

            mesajlar.Add(m);
            Session["MESAJLAR"] = mesajlar;

            Response.Redirect(Request.Url.Scheme + "://" + Request.Url.Authority + Request.ApplicationPath + "/Mahalle/Index", false);
            return Content("OK");

        }
    }
}