using Newtonsoft.Json;
using EIP_System.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using EIP_System.ViewModels;


namespace EIP_System.Controllers
{
    public class TimeRecordController : Controller
    {
        EIP_DBEntities db = new EIP_DBEntities();

        //todo:為什麼要 list
        //static List<CVM_TimeRecord> Prjlist;

        //當天日期
        DateTime todayDate = Convert.ToDateTime("00:00:00");

        public string GetProjectName(string prjId)
        {
            int _prjID = Convert.ToInt32(prjId);
            var prj = db.tProjects.Where(p => p.fProjectId == _prjID).FirstOrDefault();

            if (prj == null)
                return "查無此專案";

            return prj.fProjectName;
        }

        public string GetLevels(string prjId)
        {
            int _prjID = Convert.ToInt32(prjId);

            var levels = from i in db.tLevels
                         where i.fProjectId== _prjID
                         select new { i.fLevelName,i.fLevelId };

            string jsonString = JsonConvert.SerializeObject(levels);
            return jsonString;
        }

        public string GetTask(string levelId)
        {
            int _levelId = Convert.ToInt32(levelId);

            var prjaDetail = from p in db.tProjectDetails
                             where p.fLevelId== _levelId
                             select new { p.fProjectDetailId, p.fTaskName };

            string jsonString = JsonConvert.SerializeObject(prjaDetail);
            return jsonString;
        }

        public ActionResult Index()
        {
            return View();
        }

        //todo: 取得登入員工的當月紀錄
        public ActionResult Getdata()
        {
            var recordList = from p in db.tTimeRecords
                             where p.fEmployeeId == 100
                             select new
                             {
                                 prjDetailId =p.fProjectDetailId,
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

            return Json(new { data = recordList }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult Create(tTimeRecord target)
        {
            //todo:若新增時輸入資料錯誤 驗證
            //var prj = db.tProjects.Where(p => p.fProjectId == target.fProjectId).FirstOrDefault();
            //if (prj == null)
            //    return RedirectToAction("Index");

            //新增
            //todo:改成當前員編
            tTimeRecord record = new tTimeRecord();
            record.fDate = todayDate;
            record.fEmployeeId = 100;                         
            record.fProjectId = target.fProjectId;
            record.fProjectDetailId = target.fProjectDetailId;
            record.fTime = target.fTime;
            //record.fApprove = "否";

            db.tTimeRecords.Add(record);
            db.SaveChanges();
            return Json("success", JsonRequestBehavior.AllowGet);
        }


        public JsonResult Delete(int id)
        {
            var del = db.tTimeRecords.Where(p => p.fTimeRecordId == id).FirstOrDefault();
            db.tTimeRecords.Remove(del);
            db.SaveChanges();

            return Json("success", JsonRequestBehavior.AllowGet);
        }

        public ActionResult Search(string starTime, string endTime)
        {
            int year = Convert.ToInt32(starTime);
            int month = Convert.ToInt32(endTime);

            var list = from p in db.tTimeRecords
                       where p.fDate.Year == year && p.fDate.Month == month && p.fEmployeeId == 100
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
            return Json(new { data = list }, JsonRequestBehavior.AllowGet);
        }

    }
}