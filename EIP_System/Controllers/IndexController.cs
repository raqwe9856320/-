using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Helpers;
using System.Web.Mvc;
using EIP_System.Models;

namespace 期末專題.Controllers
{
    public class IndexController : Controller
    {
        // GET: Index
        public  ActionResult Index()
        {
            HttpCookie cookie = Request.Cookies["id"];
            try { string.IsNullOrEmpty(Session["acc" + cookie.Value] as string); }
            catch
            {
                ViewBag.info = "請輸入帳號密碼";
                return RedirectToAction("Login", "login");
            }
            int auth =Convert.ToInt32(Session["auth"+cookie.Value]);
            if (auth>1)
                ViewBag.a = "true";
            else
                ViewBag.a = "false";
            ViewBag.acc = Session["name"+ cookie.Value].ToString();
            return View();
        }
        [HttpPost]
        public string ShowBoard()
        {
            EIP_DBEntities db = new EIP_DBEntities();
            var list = from b in db.tBillboards
                       select new 
                       {
                           b.fId,
                           b.fTitle,
                           b.fContent,
                           b.fPostTime,
                           b.fType,
                           b.fEmployeeId
                       };
            string json = JsonConvert.SerializeObject(list);
            return json;
        }
        [HttpPost]
        public string ShowBell()
        {
            HttpCookie cookie = Request.Cookies["id"];
            loginController lc = new loginController();
            int fid = Convert.ToInt32(cookie.Value);
            EIP_DBEntities db = new EIP_DBEntities();
            var list = from b in db.tNotifies
                       where b.fEmployeeId == fid 
                       select new
                       {
                        b.fType,
                        b.fTitle,
                        b.fContent,
                        b.fEmployeeId
                       };
            string json = JsonConvert.SerializeObject(list);
            return json;
        }
        [HttpPost]
        public string ShowCalendar(string id)
        {
            HttpCookie cookie = Request.Cookies["id"];
            int fid = Convert.ToInt32(id);
            string json = "";
            EIP_DBEntities db = new EIP_DBEntities();
            IQueryable list;
            if (string.IsNullOrEmpty(id))
            {
                int empid = Convert.ToInt32(cookie.Value);
                list = from b in db.tCalendars
                       where b.fEmployeeId == empid
                       select new { 
                       b.fId,
                       b.fContent,
                       b.fStart,
                       b.fEnd,
                       b.fTitle,
                       } ;
                json = JsonConvert.SerializeObject(list);
                return json;
            }
            list = from b in db.tCalendars
                   where b.fId == fid
                   select new
                   {
                       b.fId,
                       b.fContent,
                       b.fStart,
                       b.fEnd,
                       b.fTitle,
                   };
            json = JsonConvert.SerializeObject(list);
            return json;
        }
        [HttpPost]
        public string InsertBoard(string id,string title,string content,string type) 
        {
            string json = "";
            HttpCookie cookie = Request.Cookies["id"];
            tBillboard tb = new tBillboard();
            IQueryable list;
            EIP_DBEntities db = new EIP_DBEntities();
            if (string.IsNullOrEmpty(id)) 
            {
                tb.fContent = content;
                tb.fTitle = title;
                tb.fPostTime = DateTime.Now.ToString();
                tb.fEmployeeId = Convert.ToInt32(cookie.Value);
                db.tBillboards.Add(tb);
                db.SaveChanges();
                list = from b in db.tBillboards
                           select new
                           {
                               b.fTitle,
                               b.fContent,
                               b.fPostTime,
                               b.fEmployeeId,
                               b.fType
                           };
                json = JsonConvert.SerializeObject(list);
                return json;
            }
            int fid = Convert.ToInt32(id);
            var listu = (db.tBillboards.Where(x => x.fId == fid)).FirstOrDefault();
            listu.fContent = content;
            listu.fTitle = title;
            listu.fPostTime = DateTime.Now.ToString();
            db.SaveChanges();
            list = from b in db.tBillboards
                   select new
                   {
                       b.fTitle,
                       b.fContent,
                       b.fPostTime,
                       b.fEmployeeId,
                       b.fType
                   };
            json = JsonConvert.SerializeObject(list);
            return json;
        }
        [HttpPost]
        public string InsertBell(string title, string content)
        {
            HttpCookie cookie = Request.Cookies["id"];
            int fid = Convert.ToInt32(cookie.Value);
            tNotify tn = new tNotify();
            tn.fContent = content;
            tn.fTitle = title;
            tn.fType = 0;
            tn.fEmployeeId = 100;
            EIP_DBEntities db = new EIP_DBEntities();
            db.tNotifies.Add(tn);
            db.SaveChanges();
            var list = from n in db.tNotifies
                       where n.fEmployeeId == fid
                       select new 
                       {
                           n.fTitle,
                           n.fContent,
                           n.fType
                       };
            string json = JsonConvert.SerializeObject(list);
            return json;
        }
        [HttpPost]
        public string UpdateBell(string type)
        {
            HttpCookie cookie = Request.Cookies["id"];
            int fid = Convert.ToInt32(cookie.Value);
            EIP_DBEntities db = new EIP_DBEntities();
            var listt = from n in db.tNotifies
                        where n.fType == 0 && n.fEmployeeId == fid
                        select n;
                        
            foreach (var item in listt) 
            {
                item.fType =Convert.ToInt32(type);
            }
            db.SaveChanges();
            var list = from n in db.tNotifies
                       where n.fEmployeeId == fid
                       select new 
                       {
                           n.fType,
                           n.fTitle,
                           n.fContent
                       };
            string json = JsonConvert.SerializeObject(list);
            return json;
        }
        [HttpPost]
        public string InsertCalendar(string id, string start, string end,string title,string content)
        {
            HttpCookie cookie = Request.Cookies["id"];
            int empid = Convert.ToInt32(cookie.Value);
            EIP_DBEntities db = new EIP_DBEntities();
            string json = "";
            IQueryable list;
            if (string.IsNullOrEmpty(id)) 
            {
                tCalendar tc = new tCalendar();
                tc.fTitle = title;
                tc.fContent = content;
                tc.fStart = start;
                tc.fEnd = end;
                tc.fEmployeeId = empid;
                db.tCalendars.Add(tc);
                db.SaveChanges();
                list = from c in db.tCalendars
                       where c.fEmployeeId == tc.fEmployeeId
                       select new {
                           c.fId,
                           c.fTitle,
                           c.fStart,
                           c.fEnd
                           };
                json = JsonConvert.SerializeObject(list);
                return json;
            }
            //現有行事曆
            int fid = Convert.ToInt32(id);
            var listu = (db.tCalendars.Where(x => x.fId == fid)).FirstOrDefault() ;
            listu.fStart = start;
            listu.fEnd = end;
            listu.fTitle = title;
            listu.fContent = content;
            db.SaveChanges();
            list = from c in db.tCalendars
                   where c.fEmployeeId == empid
                       select new 
                       {
                           c.fId,
                           c.fContent,
                           c.fTitle,
                           c.fStart,
                           c.fEnd

                       };
            json = JsonConvert.SerializeObject(list);
            return json;
        }
        public string RemoveCalendar(string id) 
        {
            HttpCookie cookie = Request.Cookies["id"];
            int empid = Convert.ToInt32(cookie.Value);
            int fid = Convert.ToInt32(id);
            string json = "";
            EIP_DBEntities db = new EIP_DBEntities();
            var del = (from c in db.tCalendars
                       where c.fId == fid
                       select c).FirstOrDefault();
            db.tCalendars.Remove(del);
            db.SaveChanges();
            var list = from b in db.tCalendars
                   where b.fEmployeeId == empid
                   select new
                   {
                       b.fId,
                       b.fContent,
                       b.fStart,
                       b.fEnd,
                       b.fTitle,
                   };
            json = JsonConvert.SerializeObject(list);
            return json;
        }
        public ActionResult Logout() 
        {
            if (Request.Cookies["id"] != null)
            {
                Response.Cookies["id"].Expires = DateTime.Now.AddDays(-1);
            }
            Session.Abandon();
            return RedirectToAction("Login","login");
        }
    }
}