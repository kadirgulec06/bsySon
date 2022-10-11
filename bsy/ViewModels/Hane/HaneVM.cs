using bsy.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace bsy.ViewModels.Hane
{
    public class HaneVM
    {
        public HaneVM()
        {
            kayitVar = 0;
            kunye = new Kunye();
            hane = new HANE();
            haneListeleri = new HaneListeleri();
        }
        public int kayitVar { get; set; }
        public Kunye kunye { get; set; }
        public HANE hane { get; set; }
        public HaneListeleri haneListeleri { get; set; }

    }
}