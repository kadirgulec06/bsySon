using bsy.Filters;
using System.Web;
using System.Web.Mvc;

namespace bsy
{
    public class FilterConfig
    {
        public static void RegisterGlobalFilters(GlobalFilterCollection filters)
        {
            filters.Add(new HandleErrorAttribute());
            filters.Add(new ortakGenelFiltre(), 100);
        }
    }
}
