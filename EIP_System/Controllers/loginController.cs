using EIP_System.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace 期末專題.Controllers
{
    public class loginController : Controller
    {
        // GET: login
        public ActionResult Login()
        {
            return View();
        }
        
        [HttpPost]
        public ActionResult Login(FormCollection fc)
        {
            EIP_DBEntities db = new EIP_DBEntities();
            try
            {
                string a = fc["acc"];
                var em = (from e in db.tEmployees
                          where e.fIdent == a
                          select new
                          {
                              e.fEmployeeId,
                              e.fIdent,
                              e.fAuth,
                              e.fName,
                              e.fPassword
                          }).FirstOrDefault();
                if (fc["acc"].ToString() == em.fIdent && fc["pw"].ToString() == em.fPassword)
                {
                    Session["name"+em.fEmployeeId] = em.fName;
                    HttpCookie cookie = new HttpCookie("id");
                    cookie.Value = em.fEmployeeId.ToString();
                    Response.Cookies.Add(cookie);

                    Session["auth"+em.fEmployeeId] = em.fAuth;
                    return RedirectToAction("Index", "Index");
                }
            }
            catch 
            {
                ViewBag.info = "帳號密碼錯誤";
                return View();
            }
            return View();
        }
    }
}