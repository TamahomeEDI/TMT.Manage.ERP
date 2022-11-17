using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using TMT.ERP.Commons;
using TMT.ERP.DataAccess.Model;
using TMT.ERP.BusinessLogic.Managers;

namespace TMT.ERP.Controllers
{
    public class LoginController : Controller
    {
        //private readonly ErpEntities _db = new ErpEntities();
        //
        // GET: /Login/

        public ActionResult Index()
        {
            return View();
        }
        public ActionResult Registration()
        {
            return View();
        }

        public ActionResult ChangePassword()
        {
            //var user = _db.Users.Find(id);
            var user = AppContext.RequestVariables.CurrentUser;
            if (user == null)
            {
                Session.Clear();
                Redirect(Constant.SiteMap.LOGIN_PAGE);
            }
            return View(user);
        }
        [HttpPost]
        public string ChangePass(int id, string oldPass, string newpass)
        {
            string result = "";
            try
            {
                var manager = new GenericManager<User>();
                var user = manager.FindById(id);
                if (user != null)
                {
                    if (user.Password == Password.Encode(oldPass))
                    {
                        user.Password = Password.Encode(newpass);
                        manager.Save();
                        result = "Your password had been successfully changed! <br>Click <a href='" + Url.Content(Constant.SiteMap.HOME_PAGE) + "'>here</a> to go back to the Home Page.";
                    }
                    else
                    {
                        result = "The Old Password was incorrect.";
                    }
                }
                else
                {
                    result = "User not found.";
                }

                //var user = _db.Users.Find(id);
                //if (user != null)
                //{
                //    user.Password = Password.Encode(newpass);
                    
                //    _db.Entry(user).State = EntityState.Modified;
                //}
                //_db.SaveChanges();
            }            
            catch (Exception ex)
            {
                result = "ERROR: " + ex.ToString();
            }
            return result;
        }

        [HttpPost]
        public string CheckLogin(string username, string password)
        {
            string result = "success";
            try
            {
                var pass = Password.Encode(password);
                var manager = new GenericManager<User>();
                var userInfo = manager.Find(x => x.UserName == username);
                if (userInfo != null)
                {
                    if (userInfo.Password == pass)
                    {
                        if (userInfo.Active == true)
                        {
                            result = "success";
                            userInfo.LastLoginDate = DateTime.Now;
                            manager.Update(userInfo);
                            manager.Save();
                            AppContext.RequestVariables.CurrentUser = userInfo;
                        }
                        else
                        {
                            result = Resources.Resource.Login_Lock;
                        }
                    }
                    else
                    {
                        result = Resources.Resource.Login_InvalidPassword;
                    }
                }
                else
                {
                    result = Resources.Resource.Login_Error;
                }



                //var user = _db.Users.FirstOrDefault(u => u.UserName == username && u.Password == pass);
                //if (user != null)
                //{
                //    if (user.Active == true)
                //    {
                //        result = "success";
                //        AppContext.SessionVariables.CurrentUserID = user.ID;
                //    }
                //    else
                //    {
                //        result = "Account is inactive";
                //    }
                //}
                //else
                //{
                //    if (_db.Users.FirstOrDefault(u => u.UserName == username) != null)
                //    {
                //        result = "Password is invalid";
                //    }
                //    else if (_db.Users.FirstOrDefault(u => u.Password == pass) != null)
                //    {
                //        result = "UserName is invalid";
                //    }
                //    else
                //    {
                //        result = "Username or password is invalid";
                //    }
                //}
            }
            catch (Exception ex)
            {
                result = ex.Message;
            }
            return result;
        }

        [HttpPost]
        public ActionResult Logout()
        {
            AppContext.SessionVariables.Clear();
            return RedirectToAction("Index", "Login");

            //var result = "success"; 
            //try
            //{
            //    AppContext.SessionVariables.Clear();
            //    Response.Redirect(Constant.SiteMap.LOGIN_PAGE, true);
            //}
            //catch (Exception ex)
            //{
            //    result = "ERROR: " + ex.ToString();
            //}
            //return result;
        }

        [HttpPost]
        public string Logout1()
        {
            AppContext.SessionVariables.Clear();
            return Constant.SiteMap.LOGIN_PAGE;
        }

        public ActionResult Profile()
        {
            var user = AppContext.RequestVariables.CurrentUser;
            return View(user);
        }
    }
}
