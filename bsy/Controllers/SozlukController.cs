using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Linq.Dynamic;
using System.Transactions;
using System.Web;
using System.Web.Mvc;
using bsy.Filters;
using bsy.Helpers;
using bsy.Models;
using bsy.ViewModels.Sozluk;

namespace bsy.Controllers
{
    [OturumAcikMI]
    [Yetkili(Roles = "YONETICI")]
    public class SozlukController : Controller
    {
        bsyContext context = new bsyContext();
        // GET: Sozluk
        public ActionResult Index()
        {
            Session["turSecildi"] = 0;
            Session["tur"] = "";

            ViewBag.IlkGiris = 1;

            SozlukVM sozVM = sozlukHazirla(0, "");

            return View(sozVM);
        }

        public ActionResult Sozluk(string Tur)
        {
            Session["turSecildi"] = 1;
            Session["tur"] = Tur;

            ViewBag.IlkGiris = 0;

            SozlukVM sozVM = sozlukHazirla(0, Tur);
            sozVM.Tur = Tur;

            return View(sozVM);
        }

        private SozlukVM sozlukHazirla(long id, string tur)
        {
            SozlukVM sozVM = new SozlukVM();
            SOZLUK soz = context.tblSozluk.Find(id);
            if (soz == null)
            {
                soz = new SOZLUK();
            }

            sozVM.sozluk = soz;
            sozVM.Tur = tur;
            sozVM.sozluk.Turu = tur;
            sozVM.turler = SozlukHelper.sozlukTurleriListesi(context, soz.Turu, false);

            return sozVM;
        }
        public ActionResult YeniSozluk(long id = 0, string Tur = "")
        {
            List<Mesaj> mesajlar = new List<Mesaj>();
            Mesaj m = null;

            SozlukVM sozVM = sozlukHazirla(id, Tur);

            return View(sozVM);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult YeniSozluk(SozlukVM yeniSozluk, string btnSubmit)
        {
            List<Mesaj> mesajlar = new List<Mesaj>();
            Mesaj m = null;

            if (!ModelState.IsValid)
            {
                return View(yeniSozluk);
            }

            bool yeniSozlukIslemi = false;
            using (TransactionScope scope = new TransactionScope(TransactionScopeOption.Required))
            {
                try
                {
                    SozlukVM eskiSozluk = sozlukHazirla(yeniSozluk.sozluk.id, yeniSozluk.Tur);

                    //yeniAO = YeniOzetHesapla(yeniAO, mao);
                    eskiSozluk = SozlukYeniToEski(eskiSozluk, yeniSozluk);
                    
                    if (eskiSozluk.sozluk.id == 0)
                    {
                        yeniSozlukIslemi = true;
                        SOZLUK soz = context.tblSozluk
                            .Where(sz => sz.Turu == eskiSozluk.sozluk.Turu && 
                                   sz.Aciklama == eskiSozluk.sozluk.Aciklama &&
                                   sz.BabaID == eskiSozluk.sozluk.BabaID).FirstOrDefault();
                        if (soz != null)
                        {
                            m = new Mesaj("hata", "Bu Sözlük kaydı daha önce oluşturulmuş, tekrar eklenemez");
                            mesajlar.Add(m);
                            Session["MESAJLAR"] = mesajlar;
                            return View(yeniSozluk);
                        }
                        else
                        {
                            context.tblSozluk.Add(eskiSozluk.sozluk);
                            m = new Mesaj("tamam", "Sözlük Kaydı Eklenmiştir.");
                        }
                    }
                    else
                    {
                        context.Entry(eskiSozluk.sozluk).State = EntityState.Modified;
                        m = new Mesaj("tamam", "Sözlük Kaydı Güncellenmiştir.");
                    }

                    try
                    {
                        context.SaveChanges();
                        scope.Complete();
                    }
                    catch (Exception e)
                    {
                        m = new Mesaj("hata", "Sözlük kaydı güncelleneMEdi=>" + GenelHelper.exceptionMesaji(e));
                    }
                }
                catch (Exception ex)
                {
                    m = new Mesaj("hata", "Sözlük kaydı güncelleneMEdi=>" + GenelHelper.exceptionMesaji(ex));
                }
            }

            mesajlar.Add(m);
            Session["MESAJLAR"] = mesajlar;

            int turSecildi = 0;
            try
            {
                turSecildi = (int)Session["turSecildi"];
            }
            catch { }

            if (turSecildi == 1)
            {
                string tur = (string)Session["tur"];
                Response.Redirect(Request.Url.Scheme + "://" + Request.Url.Authority + Request.ApplicationPath + "/Sozluk/Sozluk?Tur=" + tur, false);
            }
            else
            {
                Response.Redirect(Request.Url.Scheme + "://" + Request.Url.Authority + Request.ApplicationPath + "/Sozluk/Index", false);
            }

            return Content("OK");

        }

        private SozlukVM SozlukYeniToEski(SozlukVM eskiSozluk, SozlukVM yeniSozluk)
        {
            eskiSozluk.sozluk.id = yeniSozluk.sozluk.id;
            eskiSozluk.Tur = yeniSozluk.Tur;
            eskiSozluk.sozluk.Turu = yeniSozluk.sozluk.Turu;
            //eskiSozluk.Kodu = yeniSozluk.Kodu;
            eskiSozluk.sozluk.Aciklama = yeniSozluk.sozluk.Aciklama;
            eskiSozluk.sozluk.Parametre = yeniSozluk.sozluk.Parametre;
            eskiSozluk.sozluk.BabaID = yeniSozluk.sozluk.BabaID;

            return eskiSozluk;
        }
        public ActionResult SozlukSil(long idSil)
        {
            long id = idSil;

            List<Mesaj> mesajlar = new List<Mesaj>();
            Mesaj m = null;

            SOZLUK sozluk = context.tblSozluk.Find(id);
            context.Entry(sozluk).State = EntityState.Deleted;

            try
            {
                context.SaveChanges();
                m = new Mesaj("tamam", "Sözlük Kaydı Silinmiştir.");
            }
            catch (Exception e)
            {
                m = new Mesaj("hata", "Sözlük Kaydı Silinemedi");
            }

            mesajlar.Add(m);
            Session["MESAJLAR"] = mesajlar;

            int turSecildi = 0;
            try
            {
                turSecildi = (int)Session["turSecildi"];
            }
            catch { }

            if (turSecildi == 1)
            {
                string tur = (string)Session["tur"];
                Response.Redirect(Request.Url.Scheme + "://" + Request.Url.Authority + Request.ApplicationPath + "/Sozluk/Sozluk?Tur=" + tur, false);
            }
            else
            {
                Response.Redirect(Request.Url.Scheme + "://" + Request.Url.Authority + Request.ApplicationPath + "/Sozluk/Index", false);
            }

            return Content("OK");

        }


        public ActionResult ListeSozluk(string sidx, string sord, int page, int rows, byte ilkGiris = 0, string tur = "")
        {
            int pageIndex = Convert.ToInt32(page) - 1;

            int pageSize = rows;

            //pageSize = 5;
            var query = (from s in context.tblSozluk
                         join sb in context.tblSozluk on s.BabaID equals sb.id into sbde
                         from sbdex in sbde.DefaultIfEmpty()
                         where 
                            (s.Turu == tur || tur == "")
                         select new
                         {
                             s.id,
                             s.Turu,
                             s.Aciklama,
                             s.Parametre,
                             s.BabaID,
                             Baba = sbdex != null ? sbdex.Aciklama : ""
                         });

            int totalRecords = query.Count();
            int totalPages = (int)Math.Ceiling((float)totalRecords / (float)pageSize);

            var resultSetAfterOrderandPaging = query.OrderBy("id").Skip(pageIndex * pageSize).Take(pageSize);

            var resultSet = (from s in resultSetAfterOrderandPaging
                             select new
                             {
                                 s.id,
                                 s.Turu,
                                 s.Aciklama,
                                 s.Parametre,
                                 s.Baba,
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
                  from s in resultSet
                  select new
                  {
                      cell = new string[]
                      {
                                 s.id.ToString(),
                                 s.Turu,
                                 s.Aciklama,
                                 s.Parametre,
                                 s.Baba,
                                 s.Degistir.ToString(),
                                 s.Sil.ToString()
                       }
                  }).ToArray()
            };

            return Json(jsonData, JsonRequestBehavior.AllowGet);
        }

        public ActionResult Rol()
        {
            ViewBag.IlkGiris = 1;

            return View();
        }

        public ActionResult YeniRol(long id=0)
        {
            List<Mesaj> mesajlar = new List<Mesaj>();
            Mesaj m = null;

            SOZLUK soz = null;
            try
            {
                soz = context.tblSozluk.Find(id);
            }
            catch (Exception ex)
            {

            }

            if (soz == null)
            {
                soz = new SOZLUK();
                soz.Turu = SozlukHelper.rolKodu;
            }

            return View(soz);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult YeniRol(SOZLUK yeniSozluk, string btnSubmit)
        {
            List<Mesaj> mesajlar = new List<Mesaj>();
            Mesaj m = null;

            yeniSozluk.Turu = SozlukHelper.rolKodu;
            if (!ModelState.IsValid)
            {
                return View(yeniSozluk);
            }

            using (TransactionScope scope = new TransactionScope(TransactionScopeOption.Required))
            {
                try
                {
                    SOZLUK eskiSozluk = context.tblSozluk.Find(yeniSozluk.id);
                    if (eskiSozluk == null)
                    {
                        eskiSozluk = new SOZLUK();
                    }

                    //yeniAO = YeniOzetHesapla(yeniAO, mao);
                    eskiSozluk = RolYeniToEski(eskiSozluk, yeniSozluk);

                    bool yeniRol = false;
                    if (eskiSozluk.id == 0)
                    {
                        yeniRol = true;
                        SOZLUK soz = context.tblSozluk.Where(sz => sz.Turu == SozlukHelper.rolKodu && sz.Aciklama == eskiSozluk.Aciklama).FirstOrDefault();
                        if (soz != null)
                        {
                            m = new Mesaj("hata", "Bu rol kaydı daha önce oluşturulmuş, tekrar eklenemez");
                            mesajlar.Add(m);
                            Session["MESAJLAR"] = mesajlar;
                            return View(yeniSozluk);
                        }
                        else
                        {
                            context.tblSozluk.Add(eskiSozluk);
                            m = new Mesaj("tamam", "Rol Kaydı Eklenmiştir.");
                        }
                    }
                    else
                    {
                        context.Entry(eskiSozluk).State = EntityState.Modified;
                        m = new Mesaj("tamam", "Rol Kaydı Güncellenmiştir.");
                    }

                    try
                    {
                        context.SaveChanges();
                        if (yeniRol)
                        {
                            //eskiSozluk.Kodu = SozlukHelper.RolKoduHazirla(eskiSozluk.id);
                            context.Entry(eskiSozluk).State = EntityState.Modified;
                            context.SaveChanges();
                        }
                        scope.Complete();
                    }
                    catch (Exception e)
                    {
                        m = new Mesaj("hata", "Rol kaydı güncelleneMEdi=>" + GenelHelper.exceptionMesaji(e));
                    }
                }
                catch (Exception ex)
                {
                    m = new Mesaj("hata", "Rol kaydı güncelleneMEdi=>" + GenelHelper.exceptionMesaji(ex));
                }
            }

            mesajlar.Add(m);
            Session["MESAJLAR"] = mesajlar;

            Response.Redirect(Request.Url.Scheme + "://" + Request.Url.Authority + Request.ApplicationPath + "/Sozluk/Rol", false);
            return Content("OK");

        }

        private SOZLUK RolYeniToEski(SOZLUK eskiSozluk, SOZLUK yeniSozluk)
        {
            eskiSozluk.id = yeniSozluk.id;
            eskiSozluk.Turu = SozlukHelper.rolKodu;
            //eskiSozluk.Kodu = SozlukHelper.RolKoduBul(yeniSozluk.id, eskiSozluk.Kodu);
            eskiSozluk.Aciklama = yeniSozluk.Aciklama;

            return eskiSozluk;
        }
        public ActionResult RolSil(long idSil)
        {
            long id = idSil;

            List<Mesaj> mesajlar = new List<Mesaj>();
            Mesaj m = null;

            SOZLUK sozluk = context.tblSozluk.Find(id);
            context.Entry(sozluk).State = EntityState.Deleted;

            try
            {
                context.SaveChanges();
                m = new Mesaj("tamam", "Rol Kaydı Silinmiştir.");
            }
            catch (Exception e)
            {
                m = new Mesaj("hata", "Rol Kaydı Silinemedi");
            }

            mesajlar.Add(m);
            Session["MESAJLAR"] = mesajlar;

            Response.Redirect(Request.Url.Scheme + "://" + Request.Url.Authority + Request.ApplicationPath + "/Sozluk/Rol", false);
            return Content("OK");

        }


        //
        // ŞEHİR Günleme
        //

        public ActionResult ListeSehir(string sidx, string sord, int page, int rows, byte ilkGiris = 0)
        {
            int pageIndex = Convert.ToInt32(page) - 1;

            int pageSize = rows;

            //pageSize = 5;
            var query = (from s in context.tblSozluk
                         where s.Turu == SozlukHelper.sehirKodu
                         select s);

            int totalRecords = query.Count();
            int totalPages = (int)Math.Ceiling((float)totalRecords / (float)pageSize);

            var resultSetAfterOrderandPaging = query.OrderBy("id").Skip(pageIndex * pageSize).Take(pageSize);

            var resultSet = (from s in resultSetAfterOrderandPaging
                             select new
                             {
                                 s.id,
                                 s.Turu,
                                 sehir = s.Aciklama,
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
                  from s in resultSet
                  select new
                  {
                      cell = new string[]
                      {
                                 s.id.ToString(),
                                 s.Turu,
                                 s.sehir,
                                 s.Degistir.ToString(),
                                 s.Sil.ToString()
                       }
                  }).ToArray()
            };
            return Json(jsonData, JsonRequestBehavior.AllowGet);
        }

        public ActionResult Sehir()
        {
            ViewBag.IlkGiris = 1;

            return View();
        }

        public ActionResult YeniSehir(long id = 0)
        {
            List<Mesaj> mesajlar = new List<Mesaj>();
            Mesaj m = null;

            SOZLUK soz = null;
            try
            {
                soz = context.tblSozluk.Find(id);
            }
            catch (Exception ex)
            {

            }

            if (soz == null)
            {
                soz = new SOZLUK();
                soz.Turu = SozlukHelper.sehirKodu;
                //soz.Kodu = SozlukHelper.sehirKodu;
            }

            return View(soz);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult YeniSehir(SOZLUK yeniSozluk, string btnSubmit)
        {
            List<Mesaj> mesajlar = new List<Mesaj>();
            Mesaj m = null;
            yeniSozluk.Turu = SozlukHelper.sehirKodu;

            if (!ModelState.IsValid)
            {
                return View(yeniSozluk);
            }

            using (TransactionScope scope = new TransactionScope(TransactionScopeOption.Required))
            {
                try
                {
                    SOZLUK eskiSozluk = context.tblSozluk.Find(yeniSozluk.id);
                    if (eskiSozluk == null)
                    {
                        eskiSozluk = new SOZLUK();
                    }

                    //yeniAO = YeniOzetHesapla(yeniAO, mao);
                    eskiSozluk = SehirYeniToEski(eskiSozluk, yeniSozluk);

                    bool yeniSehir = false;
                    if (eskiSozluk.id == 0)
                    {
                        yeniSehir = true;
                        SOZLUK soz = context.tblSozluk.Where(sz => sz.Turu == SozlukHelper.sehirKodu && sz.Aciklama == eskiSozluk.Aciklama).FirstOrDefault();
                        if (soz != null)
                        {
                            m = new Mesaj("hata", "Bu Şehir kaydı daha önce oluşturulmuş, tekrar eklenemez");
                            mesajlar.Add(m);
                            Session["MESAJLAR"] = mesajlar;
                            return View(yeniSozluk);
                        }
                        else
                        {
                            context.tblSozluk.Add(eskiSozluk);
                            m = new Mesaj("tamam", "Şehir Kaydı Eklenmiştir.");
                        }
                    }
                    else
                    {
                        context.Entry(eskiSozluk).State = EntityState.Modified;
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

            Response.Redirect(Request.Url.Scheme + "://" + Request.Url.Authority + Request.ApplicationPath + "/Sozluk/Sehir", false);
            return Content("OK");

        }

        private SOZLUK SehirYeniToEski(SOZLUK eskiSozluk, SOZLUK yeniSozluk)
        {
            eskiSozluk.id = yeniSozluk.id;
            eskiSozluk.Turu = SozlukHelper.sehirKodu;
            //eskiSozluk.Kodu = yeniSozluk.Kodu;
            eskiSozluk.Aciklama = yeniSozluk.Aciklama;

            return eskiSozluk;
        }
        public ActionResult SehirSil(long idSil)
        {
            long id = idSil;

            List<Mesaj> mesajlar = new List<Mesaj>();
            Mesaj m = null;

            SOZLUK sozluk = context.tblSozluk.Find(id);
            context.Entry(sozluk).State = EntityState.Deleted;

            try
            {
                context.SaveChanges();
                m = new Mesaj("tamam", "Şehir Kaydı Silinmiştir.");
            }
            catch (Exception e)
            {
                m = new Mesaj("hata", "Şehir Kaydı Silinemedi");
            }

            mesajlar.Add(m);
            Session["MESAJLAR"] = mesajlar;

            Response.Redirect(Request.Url.Scheme + "://" + Request.Url.Authority + Request.ApplicationPath + "/Sozluk/Sehir", false);
            return Content("OK");

        }

    }
}