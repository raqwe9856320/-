using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using EIP_System.Models;
using EIP_System.ViewModels;

namespace EIP_System.Controllers
{
    public class OverTimeController : Controller
    {
        EIP_DBEntities db = new EIP_DBEntities();
        public static int fakeid = 100;
        // GET: OverTime
        public ActionResult Index()
        {
            return View();
        }
        //取得加班紀錄
        [HttpPost]
        public ActionResult getOverTimeRecord()
        {
            List<VMCOvertime> list = new List<VMCOvertime>();
            foreach (tOvertime item in db.tOvertimes.ToList())
            {
                tSignoff temp = db.tSignoffs.Where(m => m.fOvertimeId == item.fId).FirstOrDefault();


                VMCOvertime overtime = new VMCOvertime();
                overtime.fId = item.fId;
                overtime.fEmployeeId = item.fEmployeeId;
                overtime.fSort = item.fSort;
                overtime.fSubmitDate = item.fSubmitDate;
                overtime.fActiveDate = item.fActiveDate;
                overtime.fTimeCount = item.fTimeCount;
                overtime.fReason = item.fReason;
                overtime.isAgree = temp.fIsAgreed;
                list.Add(overtime);

            }
            return Json(new { data = list }, JsonRequestBehavior.AllowGet);            
        }
        public ActionResult CreateOverTime()
        {
            return View();

        }
        [HttpPost]
        public ActionResult CreateOverTime(tOvertime o, tSignoff s)
        {
            //撈資料庫當月
            double TimeCountForMonth = (from a in db.tOvertimes
                                        where a.fActiveDate.Year == DateTime.Now.Year
                                        && a.fActiveDate.Month == DateTime.Now.Month
                                        && a.fEmployeeId == fakeid
                                        select a.fTimeCount).DefaultIfEmpty(0).Sum();
            ////撈資料庫上個月
            //double TimeCountLastMonth = (from a in db.tOvertimes.AsEnumerable()
            //                              where a.fActiveDate.Year == DateTime.Now.Year
            //                              && a.fActiveDate.Month == DateTime.Now.AddMonths(-1).Month
            //                              && a.fEmployeeId == fakeid
            //                              select a.fTimeCount).Sum();
            ////撈資料庫上上個月
            //double TimeCountBeforeLastMonth = (from a in db.tOvertimes.AsEnumerable()
            //                                   where a.fActiveDate.Year == DateTime.Now.Year
            //                                   && a.fActiveDate.Month == DateTime.Now.AddMonths(-2).Month
            //                                   && a.fEmployeeId == fakeid
            //                                   select a.fTimeCount).Sum();
            //三個月加班合計
            double TimeCountThreeMonth = (from a in db.tOvertimes.AsEnumerable()
                                          where a.fActiveDate.Year == DateTime.Now.Year

                                          && a.fActiveDate.Month == DateTime.Now.AddMonths(-2).Month
                                          || a.fActiveDate.Month == DateTime.Now.AddMonths(-1).Month
                                          || a.fActiveDate.Month == DateTime.Now.Month

                                          && a.fEmployeeId == fakeid
                                          select a.fTimeCount).DefaultIfEmpty(0).Sum();
            //第一層判斷他目前是不是已經超過加班上限了
            //if (TimeCountForMonth > 54)
            //{
            //    TempData["message"] = "你太累瞜~~當月加班時數超過上限";
            //    return View();
            //}
            if (TimeCountThreeMonth > 138)
            {
                TempData["message"] = "你太累瞜~~三個月內累計加班時數超過上限";
                return View();
            }

            if (o.fTimeCount > 4)
            {
                TempData["message"] = "你太累瞜~~當日加班時數超過上限";
                return View();
            }
            //假設系統撈出來的資料目前都沒有超過上限
            //新增時共有兩個Table，一個是加班申請，一個是簽核表插入
            s.fOvertimeId = o.fId;//簽核表編號=加班申請fid
            s.fApplyClass = "加班申請";//簽核表種類=加班申請
            o.fSubmitDate = DateTime.Now;//申請日期等於Now
            o.fActiveDate = o.fActiveDate;
            s.fStartdate = o.fActiveDate;//簽核表申請日期=現在加班申請日期
            //s.tEmployee.fEmployeeId = fakeid;//簽核表寫入員工編號
            o.fEmployeeId = fakeid;//加班表寫入員工編號
            s.fSupervisorId = 106;//主管編號
            s.fIsAgreed = null;//寫入是否同意，預設為null(待審核)
            s.fEnddate = o.fActiveDate;
            o.fTimeCount = Convert.ToDouble(o.fTimeCount);//選單輸入的文字轉成加班的數字並存回加班表
            //很遺憾的，可能有超時狀況，進入細項判斷式
            if (o.fTimeCount + TimeCountForMonth > 54 || o.fTimeCount + TimeCountThreeMonth > 138)
            {
                //這個是申請當月的月份
                int NowMonth = s.fStartdate.Month;
                int LastMonth = s.fStartdate.Month - 1;
                int BeforeLastMonth = s.fStartdate.Month - 2;


                //申請加班開始日期的月份已經請的時數
                double now = (from a in db.tOvertimes
                              where a.fActiveDate.Year == DateTime.Now.Year
                              && a.fActiveDate.Month == NowMonth
                              && a.fEmployeeId == fakeid
                              select a.fTimeCount).DefaultIfEmpty(0).Sum();


                //申請加班開始日期的上個月已經請的時數
                double Last = (from a in db.tOvertimes
                               where a.fActiveDate.Year == DateTime.Now.Year
                               && a.fActiveDate.Month == LastMonth
                               && a.fEmployeeId == fakeid
                               select a.fTimeCount).DefaultIfEmpty(0).Sum();

                //申請加班開始日期的上上個月已經請的時數
                double BeforeLast = (from a in db.tOvertimes
                                     where a.fActiveDate.Year == DateTime.Now.Year
                                     && a.fActiveDate.Month == BeforeLastMonth
                                     && a.fEmployeeId == fakeid
                                     select a.fTimeCount).DefaultIfEmpty(0).Sum();

                //申請加班當月的時數已經超過上限
                if (now + o.fTimeCount > 54)
                {
                    TempData["message"] = "你太累瞜~~當月加班時數超過上限";
                    return View();
                }
                //申請加班的前三個月總時數已經超過上限
                else if (Last + BeforeLast + o.fTimeCount > 138)
                {
                    TempData["message"] = "你太累瞜~~累計三個月內加班時數超過上限";
                    return View();
                }
                //沒事，他有可能是申請上個月的加班，但是因為這個月已經滿了，才會進來這個鬼地方
                else
                {
                    //他可以出去判斷式了
                    db.tOvertimes.Add(o);//加入t物件(加班表)
                    db.tSignoffs.Add(s);//加入s物件(簽核表)
                    db.SaveChanges();//存檔
                    return RedirectToAction("CreateOverTime");
                }
            }
            //他當月既沒有超過上限，三個月內也沒有超過上限
            db.tOvertimes.Add(o);//加入t物件(加班表)
            db.tSignoffs.Add(s);//加入s物件(簽核表)
            db.SaveChanges();//存檔
            return RedirectToAction("CreateOverTime");

        }

    }
}