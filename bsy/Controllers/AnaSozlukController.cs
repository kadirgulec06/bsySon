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
using bsy.ViewModels.AnaSozluk;

namespace bsy.Controllers
{
    [OturumAcikMI]
    [Yetkili(Roles = "YONETICI")]
    public class AnaSozlukController : Controller
    {
        bsyContext context = new bsyContext();
        // GET: AnaSozluk
        public ActionResult Index()
        {
            Session["turSecildi"] = 0;
            Session["tur"] = "";

            ViewBag.IlkGiris = 1;

            AnaSozlukVM sozVM = sozlukHazirla(0, "");

            return View(sozVM);
        }

        public ActionResult AnaSozluk(string Tur)
        {
            Session["turSecildi"] = 1;
            Session["tur"] = Tur;

            ViewBag.IlkGiris = 0;

            AnaSozlukVM sozVM = sozlukHazirla(0, Tur);
            sozVM.Tur = Tur;

            return View(sozVM);
        }

        private AnaSozlukVM sozlukHazirla(long id, string tur)
        {
            AnaSozlukVM sozVM = new AnaSozlukVM();
            ANASOZLUK soz = context.tblAnaSozluk.Find(id);
            if (soz == null)
            {
                soz = new ANASOZLUK();
            }

            sozVM.sozluk = soz;
            sozVM.Tur = tur;
            sozVM.sozluk.Turu = tur;
            sozVM.turler = SozlukHelper.sozlukTurleriListesi(context, soz.Turu, false);

            return sozVM;
        }
        public ActionResult YeniAnaSozluk(long id = 0, string Tur = "")
        {
            List<Mesaj> mesajlar = new List<Mesaj>();
            Mesaj m = null;

            AnaSozlukVM sozVM = sozlukHazirla(id, Tur);

            return View(sozVM);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult YeniAnaSozluk(AnaSozlukVM yeniAnaSozluk, string btnSubmit)
        {
            List<Mesaj> mesajlar = new List<Mesaj>();
            Mesaj m = null;

            if (!ModelState.IsValid)
            {
                return View(yeniAnaSozluk);
            }

            bool yeniAnaSozlukIslemi = false;
            using (TransactionScope scope = new TransactionScope(TransactionScopeOption.Required))
            {
                try
                {
                    AnaSozlukVM eskiAnaSozluk = sozlukHazirla(yeniAnaSozluk.sozluk.id, yeniAnaSozluk.Tur);

                    //yeniAO = YeniOzetHesapla(yeniAO, mao);
                    eskiAnaSozluk = AnaSozlukYeniToEski(eskiAnaSozluk, yeniAnaSozluk);
                    
                    if (eskiAnaSozluk.sozluk.id == 0)
                    {
                        yeniAnaSozlukIslemi = true;
                        ANASOZLUK soz = context.tblAnaSozluk
                            .Where(sz => sz.Turu == eskiAnaSozluk.sozluk.Turu && 
                                   sz.Aciklama == eskiAnaSozluk.sozluk.Aciklama )
                                   .FirstOrDefault();
                        if (soz != null)
                        {
                            m = new Mesaj("hata", "Bu Sözlük kaydı daha önce oluşturulmuş, tekrar eklenemez");
                            mesajlar.Add(m);
                            Session["MESAJLAR"] = mesajlar;
                            return View(yeniAnaSozluk);
                        }
                        else
                        {
                            context.tblAnaSozluk.Add(eskiAnaSozluk.sozluk);
                            m = new Mesaj("tamam", "Sözlük Kaydı Eklenmiştir.");
                        }
                    }
                    else
                    {
                        context.Entry(eskiAnaSozluk.sozluk).State = EntityState.Modified;
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
                Response.Redirect(Request.Url.Scheme + "://" + Request.Url.Authority + Request.ApplicationPath + "/AnaSozluk/AnaSozluk?Tur=" + tur, false);
            }
            else
            {
                Response.Redirect(Request.Url.Scheme + "://" + Request.Url.Authority + Request.ApplicationPath + "/AnaSozluk/Index", false);
            }

            return Content("OK");

        }

        private AnaSozlukVM AnaSozlukYeniToEski(AnaSozlukVM eskiAnaSozluk, AnaSozlukVM yeniAnaSozluk)
        {
            eskiAnaSozluk.sozluk.id = yeniAnaSozluk.sozluk.id;
            eskiAnaSozluk.Tur = yeniAnaSozluk.Tur;
            eskiAnaSozluk.sozluk.Turu = yeniAnaSozluk.sozluk.Turu;
            //eskiAnaSozluk.Kodu = yeniAnaSozluk.Kodu;
            eskiAnaSozluk.sozluk.Aciklama = yeniAnaSozluk.sozluk.Aciklama;
            eskiAnaSozluk.sozluk.EkBilgi = yeniAnaSozluk.sozluk.EkBilgi;

            return eskiAnaSozluk;
        }
        public ActionResult AnaSozlukSil(long idSil)
        {
            long id = idSil;

            List<Mesaj> mesajlar = new List<Mesaj>();
            Mesaj m = null;

            ANASOZLUK sozluk = context.tblAnaSozluk.Find(id);
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
                Response.Redirect(Request.Url.Scheme + "://" + Request.Url.Authority + Request.ApplicationPath + "/AnaSozluk/AnaSozluk?Tur=" + tur, false);
            }
            else
            {
                Response.Redirect(Request.Url.Scheme + "://" + Request.Url.Authority + Request.ApplicationPath + "/AnaSozluk/Index", false);
            }

            return Content("OK");

        }


        public ActionResult ListeAnaSozluk(string sidx, string sord, int page, int rows, byte ilkGiris = 0, string tur = "")
        {
            int pageIndex = Convert.ToInt32(page) - 1;

            int pageSize = rows;

            //pageSize = 5;
            var query = (from s in context.tblAnaSozluk
                         where 
                            (s.Turu == tur || tur == "")
                         select new
                         {
                             s.id,
                             s.Turu,
                             s.Aciklama,
                             s.EkBilgi
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
                                 s.EkBilgi,
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
                                 s.EkBilgi,
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

            ANASOZLUK soz = null;
            try
            {
                soz = context.tblAnaSozluk.Find(id);
            }
            catch (Exception ex)
            {

            }

            if (soz == null)
            {
                soz = new ANASOZLUK();
                soz.Turu = SozlukHelper.rolTuru;
            }

            return View(soz);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult YeniRol(ANASOZLUK yeniAnaSozluk, string btnSubmit)
        {
            List<Mesaj> mesajlar = new List<Mesaj>();
            Mesaj m = null;

            yeniAnaSozluk.Turu = SozlukHelper.rolTuru;
            if (!ModelState.IsValid)
            {
                return View(yeniAnaSozluk);
            }

            using (TransactionScope scope = new TransactionScope(TransactionScopeOption.Required))
            {
                try
                {
                    ANASOZLUK eskiAnaSozluk = context.tblAnaSozluk.Find(yeniAnaSozluk.id);
                    if (eskiAnaSozluk == null)
                    {
                        eskiAnaSozluk = new ANASOZLUK();
                    }

                    //yeniAO = YeniOzetHesapla(yeniAO, mao);
                    eskiAnaSozluk = RolYeniToEski(eskiAnaSozluk, yeniAnaSozluk);

                    bool yeniRol = false;
                    if (eskiAnaSozluk.id == 0)
                    {
                        yeniRol = true;
                        ANASOZLUK soz = context.tblAnaSozluk.Where(sz => sz.Turu == SozlukHelper.rolTuru && sz.Aciklama == eskiAnaSozluk.Aciklama).FirstOrDefault();
                        if (soz != null)
                        {
                            m = new Mesaj("hata", "Bu rol kaydı daha önce oluşturulmuş, tekrar eklenemez");
                            mesajlar.Add(m);
                            Session["MESAJLAR"] = mesajlar;
                            return View(yeniAnaSozluk);
                        }
                        else
                        {
                            context.tblAnaSozluk.Add(eskiAnaSozluk);
                            m = new Mesaj("tamam", "Rol Kaydı Eklenmiştir.");
                        }
                    }
                    else
                    {
                        context.Entry(eskiAnaSozluk).State = EntityState.Modified;
                        m = new Mesaj("tamam", "Rol Kaydı Güncellenmiştir.");
                    }

                    try
                    {
                        context.SaveChanges();
                        if (yeniRol)
                        {
                            //eskiAnaSozluk.Kodu = AnaSozlukHelper.rolTuruHazirla(eskiAnaSozluk.id);
                            context.Entry(eskiAnaSozluk).State = EntityState.Modified;
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

            Response.Redirect(Request.Url.Scheme + "://" + Request.Url.Authority + Request.ApplicationPath + "/AnaSozluk/Rol", false);
            return Content("OK");

        }

        private ANASOZLUK RolYeniToEski(ANASOZLUK eskiAnaSozluk, ANASOZLUK yeniAnaSozluk)
        {
            eskiAnaSozluk.id = yeniAnaSozluk.id;
            eskiAnaSozluk.Turu = SozlukHelper.rolTuru;
            //eskiAnaSozluk.Kodu = AnaSozlukHelper.rolTuruBul(yeniAnaSozluk.id, eskiAnaSozluk.Kodu);
            eskiAnaSozluk.Aciklama = yeniAnaSozluk.Aciklama;

            return eskiAnaSozluk;
        }
        public ActionResult RolSil(long idSil)
        {
            long id = idSil;

            List<Mesaj> mesajlar = new List<Mesaj>();
            Mesaj m = null;

            ANASOZLUK sozluk = context.tblAnaSozluk.Find(id);
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

            Response.Redirect(Request.Url.Scheme + "://" + Request.Url.Authority + Request.ApplicationPath + "/AnaSozluk/Rol", false);
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
            var query = (from s in context.tblAnaSozluk
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

            ANASOZLUK soz = null;
            try
            {
                soz = context.tblAnaSozluk.Find(id);
            }
            catch (Exception ex)
            {

            }

            if (soz == null)
            {
                soz = new ANASOZLUK();
                soz.Turu = SozlukHelper.sehirKodu;
                //soz.Kodu = AnaSozlukHelper.sehirKodu;
            }

            return View(soz);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult YeniSehir(ANASOZLUK yeniAnaSozluk, string btnSubmit)
        {
            List<Mesaj> mesajlar = new List<Mesaj>();
            Mesaj m = null;
            yeniAnaSozluk.Turu = SozlukHelper.sehirKodu;

            if (!ModelState.IsValid)
            {
                return View(yeniAnaSozluk);
            }

            using (TransactionScope scope = new TransactionScope(TransactionScopeOption.Required))
            {
                try
                {
                    ANASOZLUK eskiAnaSozluk = context.tblAnaSozluk.Find(yeniAnaSozluk.id);
                    if (eskiAnaSozluk == null)
                    {
                        eskiAnaSozluk = new ANASOZLUK();
                    }

                    //yeniAO = YeniOzetHesapla(yeniAO, mao);
                    eskiAnaSozluk = SehirYeniToEski(eskiAnaSozluk, yeniAnaSozluk);

                    bool yeniSehir = false;
                    if (eskiAnaSozluk.id == 0)
                    {
                        yeniSehir = true;
                        ANASOZLUK soz = context.tblAnaSozluk.Where(sz => sz.Turu == SozlukHelper.sehirKodu && sz.Aciklama == eskiAnaSozluk.Aciklama).FirstOrDefault();
                        if (soz != null)
                        {
                            m = new Mesaj("hata", "Bu Şehir kaydı daha önce oluşturulmuş, tekrar eklenemez");
                            mesajlar.Add(m);
                            Session["MESAJLAR"] = mesajlar;
                            return View(yeniAnaSozluk);
                        }
                        else
                        {
                            context.tblAnaSozluk.Add(eskiAnaSozluk);
                            m = new Mesaj("tamam", "Şehir Kaydı Eklenmiştir.");
                        }
                    }
                    else
                    {
                        context.Entry(eskiAnaSozluk).State = EntityState.Modified;
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

            Response.Redirect(Request.Url.Scheme + "://" + Request.Url.Authority + Request.ApplicationPath + "/AnaSozluk/Sehir", false);
            return Content("OK");

        }

        private ANASOZLUK SehirYeniToEski(ANASOZLUK eskiAnaSozluk, ANASOZLUK yeniAnaSozluk)
        {
            eskiAnaSozluk.id = yeniAnaSozluk.id;
            eskiAnaSozluk.Turu = SozlukHelper.sehirKodu;
            //eskiAnaSozluk.Kodu = yeniAnaSozluk.Kodu;
            eskiAnaSozluk.Aciklama = yeniAnaSozluk.Aciklama;

            return eskiAnaSozluk;
        }
        public ActionResult SehirSil(long idSil)
        {
            long id = idSil;

            List<Mesaj> mesajlar = new List<Mesaj>();
            Mesaj m = null;

            ANASOZLUK sozluk = context.tblAnaSozluk.Find(id);
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

            Response.Redirect(Request.Url.Scheme + "://" + Request.Url.Authority + Request.ApplicationPath + "/AnaSozluk/Sehir", false);
            return Content("OK");

        }

    }
}