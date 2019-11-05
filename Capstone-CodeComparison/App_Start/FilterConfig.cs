using System.Web;
using System.Web.Mvc;

namespace Capstone_CodeComparison
{
    public class FilterConfig
    {
        public static void RegisterGlobalFilters(GlobalFilterCollection filters)
        {
            filters.Add(new HandleErrorAttribute());
        }
    }
}
