using bsy.Helpers;
using bsy.Models;
using bsy.ViewModels.Bolge;
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
    public class BolgeController : Controller
    {
        bsyContext context = new bsyContext();
        // GET: Bolge
        public ActionResult Index()
        {
            ViewBag.IlkGiris = 1;

            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Index(long id = 0)
        {
            ViewBag.IlkGiris = 0;

            return View();
        }

        public ActionResult ListeBolgeler(string sidx, string sord, int page, int rows, byte ilkGiris = 0, long sehirID = 0)
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
            }

            int rapor = 2;
            if (rapor == 0)
            {
                Session["filtreBOLGE"] = bolge.ToUpper();
            }
            else if (rapor == 1)
            {
                bolge = (string)Session["filtreBOLGE"];
            }

            int pageIndex = Convert.ToInt32(page) - 1;

            int pageSize = rows;

            //long sehirID = (long)Session["sehirID"];
            //pageSize = 5;
            var query = (from ic in context.tblBolgeler
                         join sx in context.tblSozluk on ic.id equals sx.id
                         where
                            (sx.Aciklama + "").Contains(bolge)
                         select new
                         {
                             ic.id,
                             bolgeADI = sx.Aciklama,
                             ic.Aciklama
                         });

            int totalRecords = query.Count();
            int totalPages = (int)Math.Ceiling((float)totalRecords / (float)pageSize);

            var resultSetAfterOrderandPaging = query.OrderBy("bolgeADI").Skip(pageIndex * pageSize).Take(pageSize);

            var resultSet = (from icx in resultSetAfterOrderandPaging
                             select new
                             {
                                 icx.id,
                                 //icx.bolgeKODU,
                                 icx.bolgeADI,
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
                                 //kr.bolgeKODU,
                                 kr.bolgeADI,
                                 kr.Aciklama,
                                 kr.Degistir.ToString(),
                                 kr.Sil.ToString()
                       }
                  }).ToArray()
            };

            return Json(jsonData, JsonRequestBehavior.AllowGet);
        }

        public ActionResult YeniBolge(long id = 0)
        {
            List<Mesaj> mesajlar = new List<Mesaj>();
            Mesaj m = null;

            SOZLUK soz = null;
            BOLGE bolge = null;
            try
            {
                soz = context.tblSozluk.Find(id);
                bolge = context.tblBolgeler.Find(id);
            }
            catch (Exception ex)
            {

            }

            if (soz == null)
            {
                soz = new SOZLUK();
                soz.Turu = SozlukHelper.bolgeKodu;
                bolge = new BOLGE();
            }

            BolgeVM bolgeVM = new BolgeVM();
            bolgeVM = bolgeHazirla(soz, bolge);

            return View(bolgeVM);
        }

        private BolgeVM bolgeHazirla(SOZLUK soz, BOLGE bolge)
        {
            BolgeVM bolgeVM = new BolgeVM();

            bolgeVM.sozluk = soz;
            bolgeVM.bolge = bolge;

            return bolgeVM;
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult YeniBolge(BolgeVM yeniBolgeVM, string btnSubmit)
        {
            BolgeVM eskiBolgeVM = null;

            List<Mesaj> mesajlar = new List<Mesaj>();
            Mesaj m = null;

            //yeniSozluk.Turu = SozlukHelper.rolTuru;
            if (!ModelState.IsValid)
            {
                return View(yeniBolgeVM);
            }

            using (TransactionScope scope = new TransactionScope(TransactionScopeOption.Required))
            {
                try
                {
                    eskiBolgeVM = eskiBolgeYarat(yeniBolgeVM.sozluk.id);
                    //yeniAO = YeniOzetHesapla(yeniAO, mao);
                    eskiBolgeVM = VMYeniToEski(eskiBolgeVM, yeniBolgeVM);

                    bool yeniBolge = false;
                    if (eskiBolgeVM.sozluk.id == 0)
                    {
                        yeniBolge = true;
                        var bolgeKaydi = (from sz in context.tblSozluk
                                         join ic in context.tblBolgeler on sz.id equals ic.id
                                         select new { sz.id, sz.Aciklama }).FirstOrDefault();
                        if (bolgeKaydi != null && eskiBolgeVM.sozluk.Aciklama == bolgeKaydi.Aciklama )
                        {
                            m = new Mesaj("hata", "Bu ilçe kaydı daha önce oluşturulmuş, tekrar eklenemez");
                            mesajlar.Add(m);
                            Session["MESAJLAR"] = mesajlar;
                            return View(yeniBolgeVM);
                        }
                        else
                        {
                            context.tblSozluk.Add(eskiBolgeVM.sozluk);
                            context.SaveChanges();
                            eskiBolgeVM.bolge.id = eskiBolgeVM.sozluk.id; // SozlukHelper.sozlukID(context, eskiBolgeVM.sozluk);
                            context.tblBolgeler.Add(eskiBolgeVM.bolge);

                            m = new Mesaj("tamam", "Bölge Kaydı Eklenmiştir.");
                        }
                    }
                    else
                    {
                        context.Entry(eskiBolgeVM.sozluk).State = EntityState.Modified;
                        context.Entry(eskiBolgeVM.bolge).State = EntityState.Modified;

                        m = new Mesaj("tamam", "Bölge Kaydı Güncellenmiştir.");
                    }

                    try
                    {
                        context.SaveChanges();
                        scope.Complete();
                    }
                    catch (Exception e)
                    {
                        m = new Mesaj("hata", "Bölge kaydı güncelleneMEdi=>" + GenelHelper.exceptionMesaji(e));
                    }
                }
                catch (Exception ex)
                {
                    m = new Mesaj("hata", "Bölge kaydı güncelleneMEdi=>" + GenelHelper.exceptionMesaji(ex));
                }
            }

            mesajlar.Add(m);
            Session["MESAJLAR"] = mesajlar;

            Response.Redirect(Request.Url.Scheme + "://" + Request.Url.Authority + Request.ApplicationPath + "/Bolge/Index", false);
            return Content("OK");

        }

        private BolgeVM eskiBolgeYarat(long id)
        {
            BolgeVM bolgeVM = new BolgeVM();

            SOZLUK eskiSozluk = context.tblSozluk.Find(id);
            if (eskiSozluk == null)
            {
                eskiSozluk = new SOZLUK();
            }

            BOLGE eskiBolge = context.tblBolgeler.Find(id);
            if (eskiBolge == null)
            {
                eskiBolge = new BOLGE();
            }

            eskiSozluk.Turu = SozlukHelper.bolgeKodu;

            bolgeVM.bolge = eskiBolge;
            bolgeVM.sozluk = eskiSozluk;

            return bolgeVM;

        }
        private BolgeVM VMYeniToEski(BolgeVM eskiVM, BolgeVM yeniVM)
        {
            eskiVM.sozluk = SozlukYeniToEski(eskiVM.sozluk, yeniVM.sozluk);
            eskiVM.bolge = BolgeYeniToEski(eskiVM.bolge, yeniVM.bolge);

            return eskiVM;
        }

        private SOZLUK SozlukYeniToEski(SOZLUK eskiSozluk, SOZLUK yeniSozluk)
        {
            eskiSozluk.id = yeniSozluk.id;
            eskiSozluk.Turu = SozlukHelper.bolgeKodu;
            eskiSozluk.Aciklama = yeniSozluk.Aciklama;
            eskiSozluk.BabaID = 0;

            return eskiSozluk;
        }

        private BOLGE BolgeYeniToEski(BOLGE eskiBolge, BOLGE yeniBolge)
        {
            eskiBolge.id = yeniBolge.id;
            eskiBolge.Aciklama = yeniBolge.Aciklama;

            return eskiBolge;
        }


        public ActionResult BolgeSil(long idSil)
        {
            long id = idSil;

            List<Mesaj> mesajlar = new List<Mesaj>();
            Mesaj m = null;

            BOLGE bolge = context.tblBolgeler.Find(id);
            context.Entry(bolge).State = EntityState.Deleted;

            SOZLUK sozluk = context.tblSozluk.Find(id);
            context.Entry(sozluk).State = EntityState.Deleted;

            try
            {
                context.SaveChanges();
                m = new Mesaj("tamam", "Bölge Kaydı Silinmiştir.");
            }
            catch (Exception e)
            {
                m = new Mesaj("hata", "Bölge Kaydı Silinemedi=>" + GenelHelper.exceptionMesaji(e));
            }

            mesajlar.Add(m);
            Session["MESAJLAR"] = mesajlar;

            Response.Redirect(Request.Url.Scheme + "://" + Request.Url.Authority + Request.ApplicationPath + "/Bolge/Index", false);
            return Content("OK");

        }
    }
}