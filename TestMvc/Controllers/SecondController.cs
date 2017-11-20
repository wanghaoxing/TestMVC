using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Microsoft.Practices.Unity;
using Microsoft.Practices.Unity.Configuration;
using Ruanmou.Bussiness.Interface;
using Ruanmou.Bussiness.Service;
using Ruanmou.EF.Model;
using Ruanmou.Remote.Interface;

namespace TestMvc.Controllers
{
    public class SecondController : Controller
    {
        #region

        private IUserMenuService _iUserMenuService = null;
        private IUserCompanyService _userCompanyService = null;
        private ISearch _search = null;

        public SecondController(IUserMenuService userMenuService, IUserCompanyService userCompanyService, ISearch search)
        {
            _iUserMenuService = userMenuService;
            _userCompanyService = userCompanyService;
            _search = search;
        }

        #endregion

        public ViewResult RazorShow()
        {
            return View();
        }

        [ChildActionOnly] //不能被单独请求
        public ViewResult Render(string name)
        {
            ViewBag.Name = name;
            return View();
            //return PartialView()  //指定分部视图，在_ViewStatrt.cshtml中指定的Layout会无效
        }

        public string FormTest(string UserNameGet, string PasswordPost)
        {
            return UserNameGet + PasswordPost;
        }

        public ViewResult DB()
        {
            {
                //第一种，直接使用ef
                //var dbcontext=new JDContext();
                //IUserMenuService service = new UserMenuService(dbcontext);
                //User user = service.Find<User>(2);
            }

            //{
            //第二种使用untity,用到的时候就去读取
            //    ExeConfigurationFileMap fileMap = new ExeConfigurationFileMap();
            //    fileMap.ExeConfigFilename = Path.Combine(AppDomain.CurrentDomain.BaseDirectory + "CfgFiles\\Unity.Config.xml");//找配置文件的路径
            //    Configuration configuration = ConfigurationManager.OpenMappedExeConfiguration(fileMap, ConfigurationUserLevel.None);
            //    UnityConfigurationSection section = (UnityConfigurationSection)configuration.GetSection(UnityConfigurationSection.SectionName);
            //    IUnityContainer container = new UnityContainer();
            //    section.Configure(container, "ruanmouContainer");

            //IUserMenuService service = container.Resolve<IUserMenuService>(); //new UserMenuService(dbContext);
            //    User user = service.Find<User>(2);
            //}

            {
                //第三种封装到一个类里面只读去一次，注意的是使用using对于延迟加载对象可能提前释放
                //    IUnityContainer container = UnityContainerFactoryManager.GetContainer();

                //    using (IUserMenuService service = container.Resolve<IUserMenuService>())
                //    {
                //        User user = service.Find<User>(2);

                //        //延迟加载

                //        //service.QueryPage<User>(u => u.Id > 2, 12, 1);

                //        return View(user);
                //    }
                //}

                {
                    //第四种
                    User user = this._iUserMenuService.Find<User>(2);
                    return View(user);
                }

            }
        }

        public ViewResult Lucene()
            {
                //IUserMenuService service 
                //Service

                var list = this._search.QueryCommodityPage(1, 30, "书", null, null, null);
                return View();
            }
        

        public class UnityContainerFactoryManager
        {
            private static IUnityContainer container = null;

            static UnityContainerFactoryManager()
            {
                ExeConfigurationFileMap fileMap = new ExeConfigurationFileMap();
                fileMap.ExeConfigFilename =
                    Path.Combine(AppDomain.CurrentDomain.BaseDirectory + "CfgFiles\\Unity.Config.xml"); //找配置文件的路径
                Configuration configuration = ConfigurationManager.OpenMappedExeConfiguration(fileMap,
                    ConfigurationUserLevel.None);
                UnityConfigurationSection section =
                    (UnityConfigurationSection) configuration.GetSection(UnityConfigurationSection.SectionName);
                container = new UnityContainer();
                section.Configure(container, "ruanmouContainer");
            }

            public static IUnityContainer GetContainer()
            {
                return container;
            }
        }
    }
}
