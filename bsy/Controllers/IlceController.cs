using bsy.Filters;
using bsy.Models;
using bsy.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic;
using System.Web;
using System.Web.Mvc;
using bsy.ViewModels.Ilce;
using System.Transactions;
using System.Data.Entity;

namespace bsy.Controllers
{
    [OturumAcikMI]
    [Yetkili(Roles = "YONETICI")]
    public class IlceController : Controller
    {
        bsyContext context = new bsyContext();
        // GET: Ilce
        public ActionResult Index()
        {
            Session["sehirID"] = 0;

            ViewBag.IlkGiris = 1;
            ViewBag.sehirID = 0;

            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Index(long sehirID = 0)
        {
            ViewBag.IlkGiris = 0;
            ViewBag.sehirID = sehirID;

            Session["sehirID"] = sehirID;

            return View();
        }

        public ActionResult ListeIlceler(string sidx, string sord, int page, int rows, byte ilkGiris = 0, long sehirID = 0)
        {
            // filtre parametrelerini hazırla

            string ilce = "";
            string sehir = "";
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
            }

            int rapor = 2;
            if (rapor == 0)
            {
                Session["filtreSEHIR"] = sehir.ToUpper();
                Session["filtreILCE"] = ilce.ToUpper();
            }
            else if (rapor == 1)
            {
                sehir = (string)Session["filtreSEHIR"];
                ilce = (string)Session["filtreILCE"];
            }

            int pageIndex = Convert.ToInt32(page) - 1;

            int pageSize = rows;

            //long sehirID = (long)Session["sehirID"];
            //pageSize = 5;
            var query = (from ic in context.tblIlceler
                         join sx in context.tblSozluk on ic.id equals sx.id
                         join sy in context.tblSozluk on ic.sehirID equals sy.id
                         where
                            (ic.sehirID == sehirID || sehirID == 0) &&
                            (sx.Aciklama + "").Contains(ilce) &&
                            (sy.Aciklama + "").Contains(sehir)
                         select new
                         {
                             ic.id,
                             //ilceKODU = sx.Kodu,
                             ilceADI = sx.Aciklama,
                             ic.sehirID,
                             //sehirKodu = sy.Kodu,
                             sehirADI = sy.Aciklama,
                             ic.Aciklama
                         });

            int totalRecords = query.Count();
            int totalPages = (int)Math.Ceiling((float)totalRecords / (float)pageSize);

            var resultSetAfterOrderandPaging = query.OrderBy("sehirADI, ilceADI").Skip(pageIndex * pageSize).Take(pageSize);

            var resultSet = (from icx in resultSetAfterOrderandPaging
                             select new
                             {
                                 icx.id,
                                 //icx.ilceKODU,
                                 icx.ilceADI,
                                 icx.sehirID,
                                 icx.sehirADI,
                                 //icx.sehirKodu,
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
                                 kr.sehirID.ToString(),
                                 //kr.sehirKodu,
                                 kr.sehirADI,
                                 //kr.ilceKODU,
                                 kr.ilceADI,                                 
                                 kr.Aciklama,
                                 kr.Degistir.ToString(),
                                 kr.Sil.ToString()
                       }
                  }).ToArray()
            };

            return Json(jsonData, JsonRequestBehavior.AllowGet);
        }

        public ActionResult YeniIlce(long id = 0)
        {
            List<Mesaj> mesajlar = new List<Mesaj>();
            Mesaj m = null;

            SOZLUK soz = null;
            ILCE ilce = null;
            try
            {
                soz = context.tblSozluk.Find(id);
                ilce = context.tblIlceler.Find(id);
            }
            catch (Exception ex)
            {

            }

            if (soz == null)
            {
                soz = new SOZLUK();
                soz.Turu = SozlukHelper.ilceKodu;
                ilce = new ILCE();
            }

            IlceVM ilceVM = new IlceVM();
            ilceVM = ilceHazirla(soz, ilce);

            Session["sehirID"] = 0;

            return View(ilceVM);
        }

        private IlceVM ilceHazirla(SOZLUK soz, ILCE ilce)
        {
            IlceVM ilceVM = new IlceVM();

            soz.BabaID = ilce.sehirID;
            ilceVM.sozluk = soz;
            ilceVM.ilce = ilce;

            ilceVM.sehirler = SozlukHelper.sozlukKalemleriListesi(context, "SEHIR", 0);
            ilceVM.sehirADI = SozlukHelper.sozlukAciklama(context, ilce.sehirID);

            return ilceVM;
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult YeniIlce(IlceVM yeniIlceVM, string btnSubmit)
        {
            IlceVM eskiIlceVM = null;

            List<Mesaj> mesajlar = new List<Mesaj>();
            Mesaj m = null;

            //yeniSozluk.Turu = SozlukHelper.rolTuru;
            if (!ModelState.IsValid)
            {
                return View(yeniIlceVM);
            }

            using (TransactionScope scope = new TransactionScope(TransactionScopeOption.Required))
            {
                try
                {
                    eskiIlceVM = eskiIlceYarat(yeniIlceVM.sozluk.id);
                    //yeniAO = YeniOzetHesapla(yeniAO, mao);
                    eskiIlceVM = VMYeniToEski(eskiIlceVM, yeniIlceVM);

                    bool yeniIlce = false;
                    if (eskiIlceVM.sozluk.id == 0)
                    {
                        yeniIlce = true;
                        var ilceKaydi = (from sz in context.tblSozluk
                                         join ic in context.tblIlceler on sz.id equals ic.id
                                         select new { sz.id, sz.Aciklama, ic.sehirID }).FirstOrDefault();
                        if (ilceKaydi != null && eskiIlceVM.sozluk.Aciklama == ilceKaydi.Aciklama && eskiIlceVM.ilce.sehirID == ilceKaydi.sehirID)
                        {
                            m = new Mesaj("hata", "Bu ilçe kaydı daha önce oluşturulmuş, tekrar eklenemez");
                            mesajlar.Add(m);
                            Session["MESAJLAR"] = mesajlar;
                            return View(yeniIlceVM);
                        }
                        else
                        {
                            context.tblSozluk.Add(eskiIlceVM.sozluk);
                            context.SaveChanges();
                            eskiIlceVM.ilce.id = eskiIlceVM.sozluk.id; // SozlukHelper.sozlukID(context, eskiIlceVM.sozluk);
                            context.tblIlceler.Add(eskiIlceVM.ilce);

                            m = new Mesaj("tamam", "İlçe Kaydı Eklenmiştir.");
                        }
                    }
                    else
                    {
                        context.Entry(eskiIlceVM.sozluk).State = EntityState.Modified;
                        context.Entry(eskiIlceVM.ilce).State = EntityState.Modified;

                        m = new Mesaj("tamam", "İlçe Kaydı Güncellenmiştir.");
                    }

                    try
                    {
                        context.SaveChanges();
                        scope.Complete();
                    }
                    catch (Exception e)
                    {
                        m = new Mesaj("hata", "İlçe kaydı güncelleneMEdi=>" + GenelHelper.exceptionMesaji(e));
                    }
                }
                catch (Exception ex)
                {
                    m = new Mesaj("hata", "İlçe kaydı güncelleneMEdi=>" + GenelHelper.exceptionMesaji(ex));
                }
            }

            mesajlar.Add(m);
            Session["MESAJLAR"] = mesajlar;

            Response.Redirect(Request.Url.Scheme + "://" + Request.Url.Authority + Request.ApplicationPath + "/Ilce/Index", false);
            return Content("OK");

        }

        private IlceVM eskiIlceYarat(long id)
        {
            IlceVM ilceVM = new IlceVM();

            SOZLUK eskiSozluk = context.tblSozluk.Find(id);
            if (eskiSozluk == null)
            {
                eskiSozluk = new SOZLUK();
            }

            ILCE eskiIlce = context.tblIlceler.Find(id);
            if (eskiIlce == null)
            {
                eskiIlce = new ILCE();
            }

            eskiSozluk.Turu = SozlukHelper.ilceKodu;

            ilceVM.ilce = eskiIlce;
            ilceVM.sozluk = eskiSozluk;
            ilceVM.sehirADI = SozlukHelper.sozlukAciklama(context, eskiIlce.sehirID);

            return ilceVM;

        }
        private IlceVM VMYeniToEski(IlceVM eskiVM, IlceVM yeniVM)
        {
            eskiVM.sozluk = SozlukYeniToEski(eskiVM.sozluk, yeniVM.sozluk);
            eskiVM.ilce = IlceYeniToEski(eskiVM.ilce, yeniVM.ilce);
            eskiVM.sozluk.BabaID = yeniVM.ilce.sehirID;
            eskiVM.sehirADI = yeniVM.sehirADI;

            return eskiVM;
        }

        private SOZLUK SozlukYeniToEski(SOZLUK eskiSozluk, SOZLUK yeniSozluk)
        {
            eskiSozluk.id = yeniSozluk.id;
            eskiSozluk.Turu = SozlukHelper.ilceKodu;
            //eskiSozluk.Kodu = yeniSozluk.Kodu;
            eskiSozluk.Aciklama = yeniSozluk.Aciklama;

            return eskiSozluk;
        }

        private ILCE IlceYeniToEski(ILCE eskiIlce, ILCE yeniIlce)
        {
            eskiIlce.id = yeniIlce.id;
            eskiIlce.sehirID = yeniIlce.sehirID;
            eskiIlce.Aciklama = yeniIlce.Aciklama;

            return eskiIlce;
        }


        public ActionResult IlceSil(long idSil)
        {
            long id = idSil;

            List<Mesaj> mesajlar = new List<Mesaj>();
            Mesaj m = null;

            ILCE ilce = context.tblIlceler.Find(id);
            context.Entry(ilce).State = EntityState.Deleted;

            SOZLUK sozluk = context.tblSozluk.Find(id);
            context.Entry(sozluk).State = EntityState.Deleted;

            try
            {
                context.SaveChanges();
                m = new Mesaj("tamam", "İlçe Kaydı Silinmiştir.");
            }
            catch (Exception e)
            {
                m = new Mesaj("hata", "İlçe Kaydı Silinemedi=>" + GenelHelper.exceptionMesaji(e));
            }

            mesajlar.Add(m);
            Session["MESAJLAR"] = mesajlar;

            Response.Redirect(Request.Url.Scheme + "://" + Request.Url.Authority + Request.ApplicationPath + "/Ilce/Index", false);
            return Content("OK");

        }

    }
}