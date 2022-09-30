using bsy.Filters;
using bsy.Helpers;
using bsy.Models;
using bsy.ViewModels.Sehir;
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
    [Yetkili(Roles = "YONETICI")]
    public class SehirController : Controller
    {
        bsyContext context = new bsyContext();
        // GET: Sehir
        public ActionResult Index()
        {
            Session["bolgeID"] = 0;

            ViewBag.IlkGiris = 1;
            ViewBag.bolgeID = 0;

            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Index(long bolgeID = 0)
        {
            ViewBag.IlkGiris = 0;
            ViewBag.bolgeID = bolgeID;

            Session["bolgeID"] = bolgeID;

            return View();
        }

        public ActionResult ListeSehirler(string sidx, string sord, int page, int rows, byte ilkGiris = 0, long bolgeID = 0)
        {
            // filtre parametrelerini hazırla

            string bolge = "";
            string sehir = "";
            if (Request.Params["_search"] == "true")
            {
                if (Request.Params["BOLGE"] != null)
                {
                    bolge = Request.Params["BOLGE"];
                }

                if (Request.Params["SEHIR"] != null)
                {
                    sehir = Request.Params["SEHIR"];
                }
            }

            int rapor = 2;
            if (rapor == 0)
            {
                Session["filtreBOLGE"] = bolge.ToUpper();
                Session["filtreSEHIR"] = sehir.ToUpper();
            }
            else if (rapor == 1)
            {
                bolge = (string)Session["filtreBOLGE"];
                sehir = (string)Session["filtreSEHIR"];
            }

            int pageIndex = Convert.ToInt32(page) - 1;

            int pageSize = rows;

            //long sehirID = (long)Session["sehirID"];
            //pageSize = 5;
            var query = (from il in context.tblSehirler
                         join sx in context.tblSozluk on il.id equals sx.id
                         join sy in context.tblSozluk on il.bolgeID equals sy.id
                         where
                            (il.bolgeID == bolgeID || bolgeID == 0) &&
                            (sy.Aciklama + "").Contains(bolge) &&
                            (sx.Aciklama + "").Contains(sehir)
                         select new
                         {
                             il.id,
                             il.bolgeID,
                             //bolgeKODU = sy.Kodu,
                             bolgeADI = sy.Aciklama,
                             //sehirKODU = sx.Kodu,
                             sehirADI = sx.Aciklama,
                             il.Aciklama
                         });

            int totalRecords = query.Count();
            int totalPages = (int)Math.Ceiling((float)totalRecords / (float)pageSize);

            var resultSetAfterOrderandPaging = query.OrderBy("bolgeADI, sehirADI").Skip(pageIndex * pageSize).Take(pageSize);

            var resultSet = (from icx in resultSetAfterOrderandPaging
                             select new
                             {
                                 icx.id,
                                 icx.bolgeID,
                                 //icx.bolgeKODU,
                                 icx.bolgeADI,
                                 //icx.sehirKODU,
                                 icx.sehirADI,
                                 icx.Aciklama,
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
                  from kr in resultSet
                  select new
                  {
                      cell = new string[]
                      {
                                 kr.id.ToString(),
                                 kr.bolgeID.ToString(),
                                 //kr.bolgeKODU,
                                 kr.bolgeADI,
                                 //kr.sehirKODU,
                                 kr.sehirADI,
                                 kr.Aciklama,
                                 kr.Degistir.ToString(),
                                 kr.Sil.ToString()
                       }
                  }).ToArray()
            };

            return Json(jsonData, JsonRequestBehavior.AllowGet);
        }

        public ActionResult YeniSehir(long id = 0)
        {
            List<Mesaj> mesajlar = new List<Mesaj>();
            Mesaj m = null;

            SOZLUK soz = null;
            SEHIR sehir = null;
            try
            {
                soz = context.tblSozluk.Find(id);
                sehir = context.tblSehirler.Find(id);
            }
            catch (Exception ex)
            {

            }

            if (soz == null)
            {
                soz = new SOZLUK();
                soz.Turu = SozlukHelper.sehirKodu;
                sehir = new SEHIR();
            }

            SehirVM sehirVM = new SehirVM();
            sehirVM = sehirHazirla(soz, sehir);

            Session["sehirID"] = 0;

            return View(sehirVM);
        }

        private SehirVM sehirHazirla(SOZLUK soz, SEHIR sehir)
        {
            SehirVM sehirVM = new SehirVM();

            soz.BabaID = sehir.bolgeID;
            sehirVM.sozluk = soz;
            sehirVM.sehir = sehir;

            sehirVM.bolgeler = SozlukHelper.sozlukKalemleriListesi(context, "BOLGE", 0);
            sehirVM.bolgeADI = SozlukHelper.sozlukAciklama(context, sehir.bolgeID);

            return sehirVM;
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult YeniSehir(SehirVM yeniSehirVM, string btnSubmit)
        {
            SehirVM eskiSehirVM = null;

            List<Mesaj> mesajlar = new List<Mesaj>();
            Mesaj m = null;

            //yeniSozluk.Turu = SozlukHelper.rolKodu;
            if (!ModelState.IsValid)
            {
                return View(yeniSehirVM);
            }

            using (TransactionScope scope = new TransactionScope(TransactionScopeOption.Required))
            {
                try
                {
                    eskiSehirVM = eskiSehirYarat(yeniSehirVM.sozluk.id);
                    //yeniAO = YeniOzetHesapla(yeniAO, mao);
                    eskiSehirVM = VMYeniToEski(eskiSehirVM, yeniSehirVM);

                    bool yeniSehir = false;
                    if (eskiSehirVM.sozluk.id == 0)
                    {
                        yeniSehir = true;
                        var sehirKaydi = (from sz in context.tblSozluk
                                         join ic in context.tblSehirler on sz.id equals ic.id
                                         select new { sz.id, sz.Aciklama, ic.bolgeID }).FirstOrDefault();
                        if (sehirKaydi != null && eskiSehirVM.sozluk.Aciklama == sehirKaydi.Aciklama && eskiSehirVM.sehir.bolgeID == sehirKaydi.bolgeID)
                        {
                            m = new Mesaj("hata", "Bu Şehir kaydı daha önce oluşturulmuş, tekrar eklenemez");
                            mesajlar.Add(m);
                            Session["MESAJLAR"] = mesajlar;
                            return View(yeniSehirVM);
                        }
                        else
                        {
                            context.tblSozluk.Add(eskiSehirVM.sozluk);
                            context.SaveChanges();
                            eskiSehirVM.sehir.id = eskiSehirVM.sozluk.id; // SozlukHelper.sozlukID(context, eskiSehirVM.sozluk);
                            context.tblSehirler.Add(eskiSehirVM.sehir);

                            m = new Mesaj("tamam", "Şehir Kaydı Eklenmiştir.");
                        }
                    }
                    else
                    {
                        context.Entry(eskiSehirVM.sozluk).State = EntityState.Modified;
                        context.Entry(eskiSehirVM.sehir).State = EntityState.Modified;

                        m = new Mesaj("tamam", "Şehir Kaydı Güncellenmiştir.");
                    }

                    try
                    {
                        context.SaveChanges();
                        scope.Complete();
                    }
                    catch (Exception e)
                    {
                        m = new Mesaj("hata", "Şehir kaydı güncelleneMEdi=>" + GenelHelper.exceptionMesaji(e));
                    }
                }
                catch (Exception ex)
                {
                    m = new Mesaj("hata", "Şehir kaydı güncelleneMEdi=>" + GenelHelper.exceptionMesaji(ex));
                }
            }

            mesajlar.Add(m);
            Session["MESAJLAR"] = mesajlar;

            Response.Redirect(Request.Url.Scheme + "://" + Request.Url.Authority + Request.ApplicationPath + "/Sehir/Index", false);
            return Content("OK");

        }

        private SehirVM eskiSehirYarat(long id)
        {
            SehirVM sehirVM = new SehirVM();

            SOZLUK eskiSozluk = context.tblSozluk.Find(id);
            if (eskiSozluk == null)
            {
                eskiSozluk = new SOZLUK();
            }

            SEHIR eskiSehir = context.tblSehirler.Find(id);
            if (eskiSehir == null)
            {
                eskiSehir = new SEHIR();
            }

            eskiSozluk.Turu = SozlukHelper.sehirKodu;

            sehirVM.sehir = eskiSehir;
            sehirVM.sozluk = eskiSozluk;
            sehirVM.bolgeADI = SozlukHelper.sozlukAciklama(context, eskiSehir.bolgeID);

            return sehirVM;

        }
        private SehirVM VMYeniToEski(SehirVM eskiVM, SehirVM yeniVM)
        {
            eskiVM.sozluk = SozlukYeniToEski(eskiVM.sozluk, yeniVM.sozluk);
            eskiVM.sozluk.BabaID = yeniVM.sehir.bolgeID;
            eskiVM.sehir = SehirYeniToEski(eskiVM.sehir, yeniVM.sehir);
            eskiVM.bolgeADI = yeniVM.bolgeADI;

            return eskiVM;
        }

        private SOZLUK SozlukYeniToEski(SOZLUK eskiSozluk, SOZLUK yeniSozluk)
        {
            eskiSozluk.id = yeniSozluk.id;
            eskiSozluk.Turu = SozlukHelper.sehirKodu;
            //eskiSozluk.Kodu = yeniSozluk.Kodu;
            eskiSozluk.Aciklama = yeniSozluk.Aciklama;

            return eskiSozluk;
        }

        private SEHIR SehirYeniToEski(SEHIR eskiSehir, SEHIR yeniSehir)
        {
            eskiSehir.id = yeniSehir.id;
            eskiSehir.bolgeID = yeniSehir.bolgeID;
            eskiSehir.Aciklama = yeniSehir.Aciklama;

            return eskiSehir;
        }


        public ActionResult SehirSil(long idSil)
        {
            long id = idSil;

            List<Mesaj> mesajlar = new List<Mesaj>();
            Mesaj m = null;

            SEHIR sehir = context.tblSehirler.Find(id);
            context.Entry(sehir).State = EntityState.Deleted;

            SOZLUK sozluk = context.tblSozluk.Find(id);
            context.Entry(sozluk).State = EntityState.Deleted;

            try
            {
                context.SaveChanges();
                m = new Mesaj("tamam", "Şehir Kaydı Silinmiştir.");
            }
            catch (Exception e)
            {
                m = new Mesaj("hata", "Şehir Kaydı Silinemedi=>" + GenelHelper.exceptionMesaji(e));
            }

            mesajlar.Add(m);
            Session["MESAJLAR"] = mesajlar;

            Response.Redirect(Request.Url.Scheme + "://" + Request.Url.Authority + Request.ApplicationPath + "/Sehir/Index", false);
            return Content("OK");

        }

    }
}