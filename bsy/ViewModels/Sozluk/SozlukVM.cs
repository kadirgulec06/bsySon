using bsy.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace bsy.ViewModels.Sozluk
{
    public class SozlukVM
    {
        public SOZLUK sozluk { get; set; }
        public string Tur { get; set;}
        public IEnumerable<SelectListItem> turler { get; set; }

    }
}