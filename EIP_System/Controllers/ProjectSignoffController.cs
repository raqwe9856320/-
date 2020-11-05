using EIP_System.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace EIP_System.Controllers
{
    public class ProjectSignoffController : Controller
    {
        EIP_DBEntities db = new EIP_DBEntities();

        // GET: ProjectSignoff
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult Getdata()
        {
            //取得未通過紀錄
            var recordlist = from p in db.tTimeRecords
                             where p.fApprove == null
                             select new
                             {
                                 timeRecordId = p.fTimeRecordId,
                                 date = p.fDate.Year + "/" + p.fDate.Month + "/" + p.fDate.Day,
                                 employeeId = p.fEmployeeId,
                                 employeeName = p.tEmployee.fName,
                                 projectId = p.fProjectId,
                                 projectName = p.tProject.fProjectName,
                                 levelName = p.tProjectDetail.tLevel.fLevelName,
                                 taskName = p.tProjectDetail.fTaskName,
                                 time = p.fTime,
                                 approve = p.fApprove
                             };

            return Json(new { data = recordlist }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult Edit(int recordId, int approve)
        {
            //修改審核狀態
            var record = db.tTimeRecords.Where(p => p.fTimeRecordId == recordId).FirstOrDefault();

            if (approve == 1)
            {
                record.fApprove = "同意";
            }

            if (approve == 0)
            {
                record.fApprove = "不同意";
            }

            db.SaveChanges();

            return Json("success", JsonRequestBehavior.AllowGet);
        }

        //public ActionResult Edit(int recordId,string approve)
        //{
        //    //修改審核狀態
        //    var record = db.tTimeRecords.Where(p => p.fTimeRecordId == recordId).FirstOrDefault();

        //    if(approve== "agree")
        //    {
        //        record.fApprove = "同意";
        //    }

        //    if (approve == "noagree")
        //    {
        //        record.fApprove = "不同意";
        //    }

        //    db.SaveChanges();

        //    return Json("success", JsonRequestBehavior.AllowGet);
        //}

        //public ActionResult Edit(int recordId)
        //{
        //    //修改審核狀態
        //    var record = db.tTimeRecords.Where(p => p.fTimeRecordId == recordId).FirstOrDefault();
        //    record.fApprove = "同意";
        //    db.SaveChanges();

        //    return Json("success", JsonRequestBehavior.AllowGet);
        //}
    }
}