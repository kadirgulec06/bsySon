using bsy.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace bsy.Helpers
{
    public static class KisiHelper
    {
        public static long kisiHanesi(bsyContext ctx, long kisiID)
        {
            KISIHANE kh = ctx.tblKisiHane.Where(kx => kx.KisiID == kisiID).OrderByDescending(bx => bx.BasTar).FirstOrDefault();

            if (kh == null)
            {
                return 0;
            }

            return kh.HaneID;
        }
    }
}