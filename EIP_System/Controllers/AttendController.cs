using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using EIP_System.Models;
using EIP_System.ViewModels;

namespace EIP_System.Controllers
{
    public class AttendController : Controller
    {
        //目前登入員工
        static int EmployeeId = 100;
        static string Department = "設計部";

        EIP_DBEntities db = new EIP_DBEntities();

        /// <summary>
        /// 考勤資訊頁面
        /// </summary>
        /// <returns></returns>
        // GET: Attend
        public ActionResult AttendIndex()
        {
           return View();
        }
        //取得請假紀錄
        [HttpPost]
        public ActionResult getLeaverecord()
        {
            List<VMsignoff> list =
                (new VMsignoff()).getList(db.tSignoffs.OrderByDescending(m => m.fId).Take(10).ToList());


            return Json(new { data = list }, JsonRequestBehavior.AllowGet);
        }
        
        /// <summary>
        /// 請假系統
        /// </summary>
        /// <returns></returns>
        public ActionResult CreateLeave()
        {
            //建立ViewModel
            VMCreateLeave vmCreateLeave = new VMCreateLeave();

            //取VM所需資料

            //取得該部門所有員工
            List<VMEmployee> emplist = 
                (new VMEmployee()).getlist(db.tEmployees.Where(m => m.fDepartment == Department).ToList());

            //該名員工
            vmCreateLeave.employee = emplist.Where(m => m.id == EmployeeId).FirstOrDefault();

            //該員工的請假統計
            vmCreateLeave.leavecountList = 
                db.tLeavecounts.Where(m => m.fEmployeeId == EmployeeId).ToList();

            //該部門所有員工
            vmCreateLeave.employeelist = emplist;

            //假別列表
            vmCreateLeave.leavesortList = db.tleavesorts.ToList();

            return View(vmCreateLeave);
        }
        [HttpPost]
        public ActionResult CreateLeave(VMCreateLeave vMCLeave)
        {
            //請假
            tLeave tLeave = new tLeave();
            
            tLeave.fEmployeeId = vMCLeave.employee.id;
            tLeave.fSort = vMCLeave.leavesort;
            tLeave.fApplyDate = DateTime.Now;
            tLeave.fActiveDate = vMCLeave.start;
            tLeave.fEndDate = vMCLeave.end;
            tLeave.fTimeCount = vMCLeave.timecount;
            tLeave.fReason = vMCLeave.reason;

            db.tLeaves.Add(tLeave);
            db.SaveChanges();


            //簽核表
            tSignoff tSignoff = new tSignoff();
            tSignoff.fLeaveId = int.Parse(db.tLeaves
                .OrderByDescending(p => p.fId)
                .Select(r => r.fId)
                .First().ToString());
            tSignoff.fSupervisorId = Convert.ToInt32(vMCLeave.supervisorId);
            tSignoff.fApplyClass = vMCLeave.leavesort;
            tSignoff.fStartdate = DateTime.Now;
            tSignoff.fEnddate = vMCLeave.start;

            db.tSignoffs.Add(tSignoff);
            db.SaveChanges();

            return RedirectToAction("AttendIndex");
        }

        /// <summary>
        /// 個人考勤查詢系統
        /// </summary>
        /// <returns></returns>
        public ActionResult UserLeaveinfo()
        {
            //該員工請假統計
            //var leavecount = from lc in db.tLeavecounts
            //                 join ls in db.tleavesorts
            //                 on lc.fSortId equals ls.fSortId
            //                 select new
            //                 {
            //                     id = lc.fId,
            //                     name = lc.tEmployee.fName,
            //                     leavesort = ls.fLeavename,
            //                     alltime = lc.fAlltime,
            //                     usedtime = lc.fUesdtime,
            //                     remaintime = lc.fRemaintime,
            //                     startdate = lc.fStartdate,
            //                     enddate = lc.fEnddate
            //                 };

            //return Json(leavecount.ToList(),JsonRequestBehavior.AllowGet);

            return View();
        }
        public ActionResult getLeaveinfo()
        {
            var leaveinfoList = from l in db.tLeaves.AsEnumerable()
                                join s in db.tSignoffs.AsEnumerable()
                                on l.fId equals s.fLeaveId
                                orderby l.fApplyDate descending
                                select new
                                {
                                    id = l.fId,
                                    leavesort = l.fSort,
                                    applydate = l.fApplyDate.ToString("yyyy-MM-dd"),
                                    startdate = l.fActiveDate.ToString("yyyy-MM-dd"),
                                    enddate = l.fEndDate.ToString("yyyy-MM-dd"),
                                    timecount = l.fTimeCount,
                                    reason = l.fReason,
                                    expired = l.fActiveDate.ToString("yyyy-MM-dd"),
                                    supervisor = s.tEmployee.fName,
                                    isagreed = s.fIsAgreed,
                                };

            return Json(new { data = leaveinfoList.ToList()},JsonRequestBehavior.AllowGet);
        }
        public ActionResult getLeavesRow(int id)
        {
            var leaveinfoRow = from l in db.tLeaves.AsEnumerable()
                                join s in db.tSignoffs.AsEnumerable()
                                on l.fId equals s.fLeaveId
                                where l.fId == id
                                orderby l.fApplyDate descending
                                select new
                                {
                                    id = l.fId,
                                    leavesort = l.fSort,
                                    applydate = l.fApplyDate.ToString("yyyy-MM-dd"),
                                    startdate = l.fActiveDate.ToString("yyyy-MM-dd"),
                                    enddate = l.fEndDate.ToString("yyyy-MM-dd"),
                                    timecount = l.fTimeCount,
                                    reason = l.fReason,
                                    expired = l.fActiveDate.ToString("yyyy-MM-dd"),
                                    supervisor = s.tEmployee.fName,
                                    isagreed = s.fIsAgreed,
                                    status = l.fStatus
                                };

            return Json(leaveinfoRow.ToList(), JsonRequestBehavior.AllowGet);
        }
        public ActionResult submitRevoke(int id)
        {
            //移除資料
            db.tLeaves.Remove(db.tLeaves.Where(m => m.fId == id).FirstOrDefault());
            db.tSignoffs.Remove(db.tSignoffs.Where(m => m.fLeaveId == id).FirstOrDefault());

            db.SaveChanges();

            return Json("success",JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 打卡系統
        /// </summary>
        /// <returns></returns>
        public ActionResult CreatePunchTime()
        {
            return View();
        }
    }
}