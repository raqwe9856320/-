using EIP_System.Models;
using EIP_System.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace EIP_System.Controllers
{
    public class SignoffController : Controller
    {
        EIP_DBEntities db = new EIP_DBEntities();

        static List<VMsignoff> list;

        // GET: Signoff
        public ActionResult SignoffIndex()
        {
            list = (new VMsignoff()).getList(db.tSignoffs.OrderByDescending(m => m.fId).ToList());

            return View();
        }
        public ActionResult getAllData()
        {

            return Json(new { data = list.Where(m => m.isagreed == null) }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult getSignoffData()
        {

            return Json(new { data = list.Where(m => m.isagreed != null) }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult getRow(int? id)
        {
            VMsignoff row = list.Where(m => m.id == id).FirstOrDefault();

            return Json(row, JsonRequestBehavior.AllowGet);
        }

        public ActionResult Editpass(int id, int agree) 
        {
            //資料庫更新
            tSignoff signoff = db.tSignoffs.Where(m => m.fId == id).FirstOrDefault();
            signoff.fIsAgreed = agree;  //通過 不通過
            signoff.fPassdate = DateTime.Now;   //通過日期

            //更新員工假別紀錄


            //list更新
            var target = list.Where(m => m.id == id).FirstOrDefault();
            target.isagreed = agree;
            target.passdate = DateTime.Now.ToString("yyyy-MM-dd hh:mm");

            db.SaveChanges();

            return Json("success", JsonRequestBehavior.AllowGet);
        }
    }
}