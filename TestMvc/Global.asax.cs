using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;
using Web.Core.IOC;

namespace TestMvc
{
    /// <summary>
    /// 程序的入口，且只执行一次(所以可以用来做单利)
    /// </summary>
    public class MvcApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);
            //ControllerBuilder.Current.SetControllerFactory(new DefaultControllerFactory());
            ControllerBuilder.Current.SetControllerFactory(new UnityControllerFactory());
        }
    }
}
