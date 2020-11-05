using EIP_System.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Newtonsoft.Json;

namespace EIP_System.Controllers
{
    public class ProjectDetailController : Controller
    {
        EIP_DBEntities db=new EIP_DBEntities();
        int a;
        public ActionResult Index(int prjId)
        {
            a = prjId;
            TempData["prjId"] = prjId;
            return View(prjId);
        }
        public ActionResult GetaData()
        {
            int b = a;

            TempData.Keep();
            var prjId = TempData["prjId"] as int?;

            var list = from p in db.tProjectDetails
                       where p.fProjectId == prjId           //todo:改當前案號  
                       select new
                       {
                           prjDetailId = p.fProjectDetailId,
                           levelName = p.tLevel.fLevelName,
                           taskName = p.fTaskName,
                           empId = p.fEmployeeId,
                           empName = p.tEmployee.fName,
                           status = p.fStatus,
                           startTime = p.fStartTime.Value.Year + "/" + p.fStartTime.Value.Month + "/" + p.fStartTime.Value.Day,
                           deadline = p.fDeadline.Value.Year + "/" + p.fDeadline.Value.Month + "/" + p.fDeadline.Value.Day,
                           remark = p.fRemarks
                       };
            return Json(new { data = list }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult testfortable() 
        {
            return View();
        }

        //public ActionResult Index(int prjId)
        //{
        //    return View();
        //}

        //public ActionResult GetaData()
        //{
        //    var list = from p in db.tProjectDetails
        //               where p.fProjectId == 1090101           //todo:改當前案號  
        //               select new
        //               {
        //                   prjDetailId = p.fProjectDetailId,
        //                   levelName = p.tLevel.fLevelName,
        //                   taskName = p.fTaskName,
        //                   empId = p.fEmployeeId,
        //                   empName = p.tEmployee.fName,
        //                   status = p.fStatus,
        //                   startTime = p.fStartTime.Value.Year + "/" + p.fStartTime.Value.Month + "/" + p.fStartTime.Value.Day,
        //                   deadline = p.fDeadline.Value.Year + "/" + p.fDeadline.Value.Month + "/" + p.fDeadline.Value.Day,
        //                   remark = p.fRemarks
        //               };
        //    return Json(new { data = list }, JsonRequestBehavior.AllowGet);
        //}

        [HttpPost]
        public ActionResult GetEdit(int fId) 
        {
            var task =from p in db.tProjectDetails
                      where p.fProjectDetailId==fId
                      select new
                      {
                          prjDetailId = p.fProjectDetailId,
                          levelName = p.tLevel.fLevelName,
                          taskName = p.fTaskName,
                          empId = p.fEmployeeId,
                          empName = p.tEmployee.fName,
                          status = p.fStatus,
                          startTime = p.fStartTime,
                          deadline = p.fDeadline,
                          remark = p.fRemarks
                      };

            return Json(new { data = task.ToList() }, JsonRequestBehavior.AllowGet) ;
        }





        //======================專案工作項目==========================//
        //todo:新刪修 需更新tProject fProgress進度欄位

        public string Test()
        {
            return "";
        }

        //========新增任務========//
        public ActionResult Create(int? prjId)
        {
            return View();
        }

        [HttpPost]
        public string Create(tProjectDetail target)
        {
            TempData.Keep();
            var prjId = TempData["prjId"] as int?;

            tProjectDetail prjDetail = new tProjectDetail();
            prjDetail.fProjectId = target.fProjectId;                 //todo:改當前案號 
            prjDetail.fLevelId = target.fLevelId;
            prjDetail.fTaskName = target.fTaskName;
            prjDetail.fEmployeeId = 100;                              //todo:改下拉式選單的員工(tmember)
            prjDetail.fStatus = target.fStatus;
            prjDetail.fStartTime = target.fStartTime;
            prjDetail.fDeadline = target.fDeadline;
            prjDetail.fRemarks = target.fRemarks;
            db.tProjectDetails.Add(prjDetail);
            db.SaveChanges();

            updateProgress(prjDetail.fProjectId);

            return "success";
        }

        [HttpPost]
        public string Edit(tProjectDetail target)
        {
            int id = target.fProjectDetailId;
            var prjDetail = db.tProjectDetails.Where(p => p.fProjectDetailId == id).FirstOrDefault();

            prjDetail.fLevelId = target.fLevelId;
            prjDetail.fTaskName = target.fTaskName;
            prjDetail.fEmployeeId = 100;                              //todo:改下拉式選單的員工(tmember)
            prjDetail.fStatus = target.fStatus;
            prjDetail.fStartTime = target.fStartTime;
            prjDetail.fDeadline = target.fDeadline;
            prjDetail.fRemarks = target.fRemarks;

            db.SaveChanges();
            return "success";
        }

        public JsonResult Delete(int fId)
        {
            //封包刪除及更新進度條

            //檢查是否存在工時紀錄
            var records = db.tTimeRecords.Where(p => p.fProjectDetailId == fId).ToList();
            if (records.Count() > 0) 
            {
                return Json("error", JsonRequestBehavior.AllowGet);
            }

            var task = db.tProjectDetails.Where(p => p.fProjectDetailId == fId).FirstOrDefault();

            int id = task.tProject.fProjectId;

            db.tProjectDetails.Remove(task);
            db.SaveChanges();

            updateProgress(id);

            return Json("success", JsonRequestBehavior.AllowGet);
        }

        public string GetLevels(string prjId)
        {
            int _prjId = Convert.ToInt32(prjId);

            //var levels = db.tLevels.Where(p => p.fProjectId == _prjId);
            var levels = from i in db.tLevels
                         where i.fProjectId == _prjId
                         select new { i.fLevelName, i.fLevelId };
            string jsonString = JsonConvert.SerializeObject(levels);
            return jsonString;
        }


        //更新進度條
        public void updateProgress(int fId) 
        {
            //找出此案號所有的任務
            var prjDetailList = db.tProjectDetails.Where(p => p.fProjectId == fId).ToList();
            
            //分母
            double sum = prjDetailList.Count();
            //分子
            double finished = prjDetailList.Where(p => p.fStatus == "驗收完成").Count();
            //進度
            double Progress = finished / sum;

            var project = db.tProjects.Where(p => p.fProjectId == fId).FirstOrDefault();
            project.fProgress = Progress;
            db.SaveChanges();
        }
    }
}