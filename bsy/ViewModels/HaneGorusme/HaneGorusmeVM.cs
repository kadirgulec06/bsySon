﻿using bsy.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace bsy.ViewModels.HaneGorusme
{
    public class HaneGorusmeVM
    {
        public int yeniGorusme { get; set; }
        public int tabIndex { get; set; }
        public Kunye kunye { get; set; }
        public HaneListeleri haneListeleri { get; set; }
        public HANEGORUSME haneGorusme { get; set; }
    }
}