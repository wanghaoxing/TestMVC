using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Web;
using System.Web.Mvc;
using PagedList;
using Ruanmou.Bussiness.Interface;
using Ruanmou.EF.Model;
using Ruanmou.Framework.ExtendExpression;
using Ruanmou.Framework.Log;
using Ruanmou.Framework.Models;
using Ruanmou.Remote.Interface;
using Ruanmou.Remote.Model;
using Web.Core.Models;

namespace TestMvc.Controllers
{
    public class ThirdController : Controller
    {
        private ISearch iCommoditySearch = null;
        private ICommodityService iCommodityService = null;
        private ICategoryService iCategoryService = null;
        private int pageSize = 10;
        private Logger logger = Logger.CreateLogger(typeof(ThirdController));
        public ThirdController(ISearch commoditySearch, ICategoryService categoryService, ICommodityService commodityService)
        {
            this.iCommoditySearch = commoditySearch;
            this.iCategoryService = categoryService;
            this.iCommodityService = commodityService;
        }
        #region 数据库管理
        /// <summary>
        /// 首页列表查询展示
        /// </summary>
        /// <param name="searchString"></param>
        /// <param name="firstCategory"></param>
        /// <param name="secondCategory"></param>
        /// <param name="thirdCategory"></param>
        /// <returns></returns>
        public ViewResult Index(string searchString, int orderBy = -1, int firstCategory = -1, int secondCategory = -1, int thirdCategory = -1, int pageIndex = 1)
        {
            ViewBag.FirstCategory = BuildCategoryList(iCategoryService.CacheAllCategory().Where(c => c.CategoryLevel == 1));
            ViewBag.SecondCategory = BuildCategoryList(null);
            ViewBag.ThirdCategory = BuildCategoryList(null);
            ViewBag.SearchString = searchString;


            //string orderBy = Request.QueryString["orderBy"];
            //string searchString = Request.QueryString["searchString"];

            Expression<Func<Commodity, bool>> lambdaWhere = null;

            #region categoryId
            int id = thirdCategory == -1 ?
                       secondCategory == -1 ?
                  firstCategory == -1 ? -1 : firstCategory : secondCategory : thirdCategory;
            if (id != -1)
            {
                Category category = iCategoryService.CacheAllCategory().FirstOrDefault(c => c.Id == id);
                if (category != null)
                {
                    List<int> categoryIdList = iCategoryService.CacheAllCategory().Where(c => (c.ParentCode.StartsWith(category.Code) || c.Id == category.Id)).Select(c => c.Id).ToList();
                    lambdaWhere = lambdaWhere.And<Commodity>(c => categoryIdList.Contains(c.CategoryId ?? 0));
                }
            }
            #endregion categoryId

            #region searchString
            if (!string.IsNullOrWhiteSpace(searchString))
            {
                lambdaWhere = lambdaWhere.And<Commodity>(b => b.Title.Contains(searchString));
            }
            #endregion

            #region order
            bool isAsc = true;
            Expression<Func<Commodity, decimal>> lambdaOrderby = null;
            if (orderBy == -1)//未排序
            {
                ViewBag.OrderBy = 1;
                lambdaOrderby = t => t.Id;//默认按聚集索引排序
            }
            else if (orderBy == 0)//价格
            {
                ViewBag.OrderBy = 1;
                lambdaOrderby = t => t.Price ?? 0;
            }
            else
            {
                ViewBag.OrderBy = 0;//价格
                isAsc = false;
                lambdaOrderby = t => t.Price ?? 0;
            }

            #endregion order
            PageResult<Commodity> commodityList = this.iCommodityService.QueryPage(lambdaWhere, pageSize, pageIndex, lambdaOrderby, isAsc);
            StaticPagedList<Commodity> pageList = new StaticPagedList<Commodity>(commodityList.DataList, pageIndex, pageSize, commodityList.TotalCount);
            return View(pageList);
        }

        #region Create
        [HttpGet]
        public ActionResult Create()
        {
            ViewBag.categoryList = BuildCategoryListWithCode();
            return View();
        }

        /// <summary>
        /// Action前加[HttpPost]表示只有以Post方法请求Create Action的时候才会调用这个Action。
        ///
        ///Action的参数是以Commodity实例传递的。也就是说Create.cshtml提交的4个值被赋值给work然后把worker传递给Create作为参数。而这个参数前面的[Bind(Include = ProductId, CategoryId, Title, Price, Url, ImageUrl")]是为了防止过多提交(overposting)攻击的。从Create.cshtml的代码可以知道，这个页面只会提交4个值。而黑客可以有办法通过这个页面提/交更多的值给/当/前Action，而这些多出来的值也会存在worker实例中被添加到数据库，这无疑是危险的。因此[Bind(Include = "")]就限定了不管你提交多少值，我这个Action里只/接受ProductId, CategoryId, Title, Price, Url, ImageUrl。保证了页面的安全性。
        ///第7行ModelState.IsValid表示提交的数据是否有效。比如对于一个类型为数字的属性必须提交一个数字才算是有效。如果提交的数据有效则保存数据并且将页面跳转回Index.cshtml。
        ///第16行ModelState.AddModelError()函数可以给Model添加一条错误信息，函数的第一个参数是key，用于查找这个错误信息，第二个参数是错误信息的具体内容。这个错误信息可以在View中通过Html.ValidationMessage("unableToSave")来访问到。
        ///
        /// 
        /// 页面上的Html.AntiForgeryToken()会给访问者一个默认名为__RequestVerificationToken的cookie
        /// 为了验证一个来自form post，还需要在目标action上增加[ValidateAntiForgeryToken]特性，它是一个验证过滤器，
        /// 它主要检查
        /// (1)请求的是否包含一个约定的AntiForgery名的cookie
        /// (2)请求是否有一个Request.Form["约定的AntiForgery名"]，约定的AntiForgery名的cookie和Request.Form值是否匹配
        /// </summary>
        /// <param name="commodity"></param>
        /// <returns></returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "ProductId, CategoryId, Title, Price, Url, ImageUrl")]Commodity commodity)
        {
            string title1 = this.HttpContext.Request.Params["title"];
            string title2 = this.HttpContext.Request.QueryString["title"];
            string title3 = this.HttpContext.Request.Form["title"];
            if (ModelState.IsValid)//
            {
                Commodity newCommodity = iCommodityService.Insert(commodity);
                return RedirectToAction("Index");
            }
            else
            {
                throw new Exception("ModelState未通过检测");
            }
        }

        [HttpPost]
        public ActionResult AjaxCreate([Bind(Include = "ProductId, CategoryId, Title, Price, Url, ImageUrl")]Commodity commodity)
        {
            Commodity newCommodity = iCommodityService.Insert(commodity);
            AjaxResult ajaxResult = new AjaxResult()
            {
                Result = DoResult.Success,
                PromptMsg = "插入成功"
            };
            return Json(ajaxResult);
        }
        #endregion Create

        #region Details Edit Delete
        /// <summary>
        /// 详情
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet]
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                throw new Exception("need commodity id");
            }
            Commodity commodity = iCommodityService.Find<Commodity>(id ?? -1);
            if (commodity == null)
            {
                throw new Exception("Not Found Commodity");
            }
            return View(commodity);
        }

        [HttpGet]
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                throw new Exception("need commodity id");
            }
            Commodity commodity = iCommodityService.Find<Commodity>(id ?? -1);
            if (commodity == null)
            {
                throw new Exception("Not Found");
            }
            ViewBag.categoryList = BuildCategoryListWithCode();
            return View(commodity);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,ProductId, CategoryId, Title, Price,Url,ImageUrl")] Commodity commodity)
        {
            if (ModelState.IsValid)
            {
                Commodity commodityDB = iCommodityService.Find<Commodity>(commodity.Id);
                commodityDB.ProductId = commodity.ProductId;
                commodityDB.CategoryId = commodity.CategoryId;
                commodityDB.Title = commodity.Title;
                commodityDB.Price = commodity.Price;
                commodityDB.Url = commodity.Url;
                commodityDB.ImageUrl = commodity.ImageUrl;
                iCommodityService.Update(commodityDB);
                return RedirectToAction("Index");
            }
            else
                throw new Exception("ModelState未通过检测");
        }
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                throw new Exception("Not Found");
            }
            Commodity commodity = iCommodityService.Find<Commodity>(id ?? -1);
            if (commodity == null)
            {
                throw new Exception("Not Found");
            }
            else
            {
                iCommodityService.Delete(commodity);
            }
            return RedirectToAction("Index");
        }

        /// <summary>
        /// ajax删除
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpPost]
        public JsonResult AjaxDelete(int id)
        {
            iCommodityService.Delete<Commodity>(id);
            AjaxResult ajaxResult = new AjaxResult()
            {
                Result = DoResult.Success,
                PromptMsg = "删除成功"
            };
            return Json(ajaxResult);
        }
        #endregion Details Edit Delete
        #endregion 数据库管理
        #region 搜索

        public ActionResult SearchIndex(string searchString, int firstCategory = -1, int secondCategory = -1,
            int thirdCategory = -1)
        {
            ViewBag.FirstCategory = BuildCategoryList(iCategoryService.CacheAllCategory().Where(c => c.CategoryLevel == 1));
            ViewBag.SecondCategory = BuildCategoryList(null);
            ViewBag.ThirdCategory = BuildCategoryList(null);
            return View();
        }
        public ActionResult SearchPartialList(string searchString, int pageIndex = 1, int firstCategory = -1, int secondCategory = -1, int thirdCategory = -1)
        {
            int id = thirdCategory == -1 ?
                        secondCategory == -1 ?
                   firstCategory == -1 ? -1 : firstCategory : secondCategory : thirdCategory;
            if (id == -1 && string.IsNullOrWhiteSpace(searchString))
            {
                searchString = "男装";
            }
            List<int> categoryIdList = null;
            if (id != -1)
            {
                Category category = iCategoryService.CacheAllCategory().FirstOrDefault(c => c.Id == id);
                if (category != null)
                    categoryIdList = iCategoryService.CacheAllCategory().Where(c => (c.ParentCode.StartsWith(category.Code) || c.Id == category.Id)).Select(c => c.Id).ToList();
            }
            PageResult<CommodityModel> remoteCommodityList = iCommoditySearch.QueryCommodityPage(pageIndex, pageSize, searchString, categoryIdList, null, null);
            StaticPagedList<CommodityModel> pageList = new StaticPagedList<CommodityModel>(remoteCommodityList.DataList, pageIndex, pageSize, remoteCommodityList.TotalCount);
            return PartialView(pageList);
        }
        #endregion
        public JsonResult CategoryDropdown(int id = -1)
        {
            Category category = iCategoryService.CacheAllCategory().FirstOrDefault(c => c.Id == id);
            AjaxResult ajaxResult = new AjaxResult();
            if (category != null)
            {
                var categoryList = iCategoryService.CacheAllCategory().Where(c => c.ParentCode.Equals(category.Code));
                ajaxResult.RetValue = BuildCategoryList(categoryList);
                ajaxResult.Result = DoResult.Success;
            }
            else
            {
                ajaxResult.Result = DoResult.Failed;
                ajaxResult.PromptMsg = "类型查询失败";
            }
            return Json(ajaxResult);
        }
        #region PrivateMethod
        /// <summary>
        /// 拼装下拉框
        /// </summary>
        /// <param name="categoryList"></param>
        /// <returns></returns>
        private IEnumerable<SelectListItem> BuildCategoryList(IEnumerable<Category> categoryList)
        {
            List<SelectListItem> selectList = new List<SelectListItem>()
            {
                new SelectListItem()
                {
                    Selected=true,
                    Text="--请选择--",
                    Value="-1"
                }
            };
            if (categoryList != null && categoryList.Count() > 0)
            {
                selectList.AddRange(categoryList.Select(c => new SelectListItem()
                {
                    Selected = false,
                    Text = c.Name,
                    Value = c.Id.ToString()
                }));
            }
            return selectList;
        }
        /// <summary>
        /// Value为id_code
        /// </summary>
        /// <returns></returns>
        private IEnumerable<SelectListItem> BuildCategoryListWithCode()
        {
            var categoryList = iCategoryService.GetChildList();
            if (categoryList.Count() > 0)
            {
                return categoryList.Select(c => new SelectListItem()
                {
                    Text = c.Name,
                    Value = string.Format("{0}_{1}", c.Id, c.Code)
                });
            }
            else return null;
        }
        #endregion
    }
}