using bsy.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace bsy.ViewModels.Hane
{
    public class HaneVM
    {
        public Kunye kunye { get; set; }
        public HANE hane { get; set; }
    }
}