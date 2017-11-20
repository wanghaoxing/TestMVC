using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace TestMvc
{
    public class RouteConfig
    {
        public static void RegisterRoutes(RouteCollection routes)
        {
            //iis6历史原因
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");

//            //路由扩展1，按顺序注册
//            routes.MapRoute(
//                name: "About",
//                url: "about",
//                defaults: new { controller = "First", action = "String", id = UrlParameter.Optional }//静态路由
//            );
//            routes.MapRoute(
//    name: "TestStatic",
//    url: "Test/Static",
//    defaults: new { controller = "First", action = "Index", id = UrlParameter.Optional }//静态路由
//                );

//            routes.MapRoute(
//name: "TestStatic2",
//url: "Test/{action}",
//defaults: new { controller = "First"}//替换控制器
//    );

//            routes.MapRoute(
// "Regex",
//"{controller}/{action}_{Year}_{Month}_{Day}",
// new { controller = "First",id=UrlParameter.Optional },
//new{ Year=@"\d{4}",Month=@"\d{2}",Day=@"\d{2}" }//正则
//);

//            routes.MapRoute(
//    name: "Default",
//    url: "{controller}/{action}/{id}",
//    defaults: new { controller = "Home", action = "Next", id = UrlParameter.Optional },
//    namespaces:new string[] { "TestMvc.Controllers" }
//);//命名空间

            routes.MapRoute(
                name: "Default",
                url: "{controller}/{action}/{id}",
                defaults: new { controller = "Home", action = "Index", id = UrlParameter.Optional }//命名参数
            );
        }
    }
}
