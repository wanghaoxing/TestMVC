using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Ruanmou.Bussiness.Interface;
using Ruanmou.Framework.Extension;
using Ruanmou.Framework.Log;
using Web.Core.IOC;
using Microsoft.Practices.Unity;
using Ruanmou.EF.Model;
using Ruanmou.Framework.Encrypt;

namespace TestMvc.Utility
{
    public static class UserManage
    {
        public static Logger logger =Logger.CreateLogger(typeof(UserManage));

        public static LoginResult UserLogin(this HttpContextBase context, string name = "", string pwd = "",
            string verify = "")
        {
            if(string.IsNullOrEmpty(verify)||context.Session["CheckCode"]==null||!context.Session["CheckCode"].ToString().Equals(verify))
                return LoginResult.WrongVerify;
            var service = DIFactory.GetContainer().Resolve<IUserMenuService>();
            var user = service.UserLogin(name);
            if (user == null)
            {
                return LoginResult.NoUser;
            }
            else if (!user.Password.Equals(MD5Encrypt.Encrypt(pwd)))
            {
                return LoginResult.WrongPwd;
            }
            else if (user.State == (int) UserState.Frozen)
            {
                return LoginResult.Frozen;
            }
            else
            {
                return LoginResult.Success;
            }
        }
    }
    public enum LoginResult
    {
        /// <summary>
        /// 登录成功
        /// </summary>
        [Remark("登录成功")]
        Success = 0,
        /// <summary>
        /// 用户不存在
        /// </summary>
        [RemarkAttribute("用户不存在")]
        NoUser = 1,
        /// <summary>
        /// 密码错误
        /// </summary>
        [RemarkAttribute("密码错误")]
        WrongPwd = 2,
        /// <summary>
        /// 验证码错误
        /// </summary>
        [RemarkAttribute("验证码错误")]
        WrongVerify = 3,
        /// <summary>
        /// 账号被冻结
        /// </summary>
        [RemarkAttribute("账号被冻结")]
        Frozen = 4
    }
}