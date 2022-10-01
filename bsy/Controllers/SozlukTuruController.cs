using bsy.Filters;
using bsy.Helpers;
using bsy.Models;
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
    public class SozlukTuruController : Controller
    {
        bsyContext context = new bsyContext();
        // GET: SozlukTuru
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult ListeTur(string sidx, string sord, int page, int rows, byte ilkGiris = 0)
        {
            int pageIndex = Convert.ToInt32(page) - 1;

            int pageSize = rows;

            //pageSize = 5;
            var query = (from st in context.tblSozlukTurleri
                         select st);

            int totalRecords = query.Count();
            int totalPages = (int)Math.Ceiling((float)totalRecords / (float)pageSize);

            var resultSetAfterOrderandPaging = query.OrderBy("id").Skip(pageIndex * pageSize).Take(pageSize);

            var resultSet = (from st in resultSetAfterOrderandPaging
                             select new
                             {
                                 st.id,
                                 st.Tur,
                                 st.Aciklama,
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
                  from st in resultSet
                  select new
                  {
                      cell = new string[]
                      {
                                 st.id.ToString(),
                                 st.Tur,
                                 st.Aciklama,
                                 st.Degistir.ToString(),
                                 st.Sil.ToString()
                       }
                  }).ToArray()
            };
            return Json(jsonData, JsonRequestBehavior.AllowGet);
        }

        public ActionResult Tur()
        {
            ViewBag.IlkGiris = 1;

            return View();
        }

        public ActionResult YeniTur(long id = 0)
        {
            List<Mesaj> mesajlar = new List<Mesaj>();
            Mesaj m = null;

            SOZLUKTURU st = null;
            try
            {
                st = context.tblSozlukTurleri.Find(id);
            }
            catch (Exception ex)
            {

            }

            if (st == null)
            {
                st = new SOZLUKTURU();
            }

            return View(st);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult YeniTur(SOZLUKTURU yeniST, string btnSubmit)
        {
            List<Mesaj> mesajlar = new List<Mesaj>();
            Mesaj m = null;

            if (!ModelState.IsValid)
            {
                return View(yeniST);
            }

            using (TransactionScope scope = new TransactionScope(TransactionScopeOption.Required))
            {
                try
                {
                    SOZLUKTURU eskiST = context.tblSozlukTurleri.Find(yeniST.id);
                    if (eskiST == null)
                    {
                        eskiST = new SOZLUKTURU();
                    }

                    //yeniAO = YeniOzetHesapla(yeniAO, mao);
                    eskiST = TurYeniToEski(eskiST, yeniST);

                    bool yeniTurIslemi = false;
                    if (eskiST.id == 0)
                    {
                        yeniTurIslemi = true;
                        SOZLUKTURU st = context.tblSozlukTurleri.Where(sz => sz.Tur == yeniST.Tur).FirstOrDefault();
                        if (st != null)
                        {
                            m = new Mesaj("hata", "Bu Tür kaydı daha önce oluşturulmuş, tekrar eklenemez");
                            mesajlar.Add(m);
                            Session["MESAJLAR"] = mesajlar;
                            return View(yeniST);
                        }
                        else
                        {
                            context.tblSozlukTurleri.Add(yeniST);
                            m = new Mesaj("tamam", "Tür Kaydı Eklenmiştir.");
                        }
                    }
                    else
                    {
                        context.Entry(eskiST).State = EntityState.Modified;
                        m = new Mesaj("tamam", "Tür Kaydı Güncellenmiştir.");
                    }

                    try
                    {
                        context.SaveChanges();
                        scope.Complete();
                    }
                    catch (Exception e)
                    {
                        m = new Mesaj("hata", "Tür kaydı güncelleneMEdi=>" + GenelHelper.exceptionMesaji(e));
                    }
                }
                catch (Exception ex)
                {
                    m = new Mesaj("hata", "Yür kaydı güncelleneMEdi=>" + GenelHelper.exceptionMesaji(ex));
                }
            }

            mesajlar.Add(m);
            Session["MESAJLAR"] = mesajlar;

            Response.Redirect(Request.Url.Scheme + "://" + Request.Url.Authority + Request.ApplicationPath + "/SozlukTuru/Tur", false);
            return Content("OK");

        }

        private SOZLUKTURU TurYeniToEski(SOZLUKTURU eskiST, SOZLUKTURU yeniST)
        {
            eskiST.id = yeniST.id;
            eskiST.Tur = yeniST.Tur;
            eskiST.Aciklama = yeniST.Aciklama;

            return eskiST;
        }
        public ActionResult TurSil(long idSil)
        {
            long id = idSil;

            List<Mesaj> mesajlar = new List<Mesaj>();
            Mesaj m = null;

            SOZLUKTURU st = context.tblSozlukTurleri.Find(id);
            context.Entry(st).State = EntityState.Deleted;

            try
            {
                context.SaveChanges();
                m = new Mesaj("tamam", "Tür Kaydı Silinmiştir.");
            }
            catch (Exception e)
            {
                m = new Mesaj("hata", "Tür Kaydı Silinemedi");
            }

            mesajlar.Add(m);
            Session["MESAJLAR"] = mesajlar;

            Response.Redirect(Request.Url.Scheme + "://" + Request.Url.Authority + Request.ApplicationPath + "/SozlukTuru/Tur", false);
            return Content("OK");

        }

    }
}