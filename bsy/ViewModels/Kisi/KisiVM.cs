using bsy.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace bsy.ViewModels.Kisi
{
    public class KisiVM
    {
        public KisiVM()
        {
            kayitVar = 0;
            kunye = new Kunye();
            kisi = new KISI();
            kisiHane = new KISIHANE();
        }
        public int kayitVar { get; set; }
        public Kunye kunye { get; set; }
        public KISI kisi { get; set; }
        public KISIHANE kisiHane { get; set; }
        public KisiListeleri kisiListeleri { get; set; }

    }
}