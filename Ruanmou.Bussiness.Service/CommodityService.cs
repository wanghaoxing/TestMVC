using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Ruanmou.Bussiness.Interface;
using System.Linq.Expressions;
using System.Data.Entity;
using Ruanmou.EF.Model;

namespace Ruanmou.Bussiness.Service
{
    public class CommodityService : BaseService, ICommodityService
    {
        #region Identity
        private DbSet<Commodity> _CommodityDbSet = null;
        public CommodityService(DbContext dbContext)
            : base(dbContext)
        {
            this._CommodityDbSet = dbContext.Set<Commodity>();
        }
        #endregion

        public override void Dispose()
        {
            Console.WriteLine("已释放");
            base.Dispose();
        }
    }
}
