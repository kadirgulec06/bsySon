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

namespace bsy.Controllers
{
    //[OturumAcikMI]
    [Yetkili(Roles = "YONETICI")]
    public class SozlukController : Controller
    {
        bsyContext context = new bsyContext();
        // GET: Sozluk
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult ListeRol(string sidx, string sord, int page, int rows, byte ilkGiris = 0)
        {
            int pageIndex = Convert.ToInt32(page) - 1;

            int pageSize = rows;

            //pageSize = 5;
            var query = (from s in context.tblSozluk
                         where s.Turu == SozlukHelper.rolKodu
                         select s);

            int totalRecords = query.Count();
            int totalPages = (int)Math.Ceiling((float)totalRecords / (float)pageSize);

            var resultSetAfterOrderandPaging = query.OrderBy("id").Skip(pageIndex * pageSize).Take(pageSize);

            var resultSet = (from s in resultSetAfterOrderandPaging
                             select new
                             {
                                 s.id,
                                 s.Turu,
                                 s.Kodu,
                                 rol=s.Aciklama,
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
                                 s.Kodu,
                                 s.rol,                                 
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
                            eskiSozluk.Kodu = SozlukHelper.RolKoduHazirla(eskiSozluk.id);
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
            eskiSozluk.Kodu = SozlukHelper.RolKoduBul(yeniSozluk.id, eskiSozluk.Kodu);
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
                                 s.Kodu,
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
                                 s.Kodu,
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
                soz.Kodu = SozlukHelper.sehirKodu;
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
            eskiSozluk.Kodu = yeniSozluk.Kodu;
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