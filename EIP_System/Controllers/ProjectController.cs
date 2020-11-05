using EIP_System.Models;
using EIP_System.ViewModel;
using EIP_System.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace EIP_System.Controllers
{
    public class ProjectController : Controller
    {
        EIP_DBEntities db = new EIP_DBEntities();

        //當天日期
        DateTime todayDate = Convert.ToDateTime("00:00:00");


        //======================專案列表(tProject)==========================//
        //==========列表==========//
        // GET: Project
        public ActionResult List()
        {
            var projects = db.tProjects.ToList();
            List<CVM_ProjectTeamMember> list = new List<CVM_ProjectTeamMember>();

            for (int i = 0; i < projects.Count(); i++)
            {
                CVM_ProjectTeamMember prjmeb = new CVM_ProjectTeamMember();
                prjmeb.project = projects[i];
                prjmeb.members = db.tTeamMembers.Where(p => p.fProjectId == prjmeb.project.fProjectId).ToList();
                list.Add(prjmeb);
            }
            return View(list);
        }

        //==========新增==========//
        //todo:輸入完fEmployeeId ajax自動帶出員工名稱，或是輸入員工名稱，自動存入fEmployeeId

        //public ActionResult Create()
        //{

        //    return View();
        //}

        [HttpPost]
        public string Create(tProject target)
        {
            //------新增專案-----------//
            tProject prj = new tProject();
            prj.fProjectId = target.fProjectId;
            prj.fProjectName = target.fProjectName;
            prj.fClient = target.fClient;
            prj.fPrice = target.fPrice;
            prj.fCreatdDate = target.fCreatdDate;
            prj.fDateline = target.fDateline;
            prj.fEmployeeId = target.fEmployeeId;
            prj.fProgress = 0;
            prj.fPaymentStatus = "未收款";

            db.tProjects.Add(prj);
            db.SaveChanges();

            //------新增專案預算書------//
            tBudget budget = new tBudget();
            budget.fProjectId = target.fProjectId;
            db.tBudgets.Add(budget);
            db.SaveChanges();

            //---------新增階段表------//
            //預設為三階段//
            //todo:讓使用者自訂階段數量及名稱

            tLevel level1 = new tLevel();
            level1.fProjectId = prj.fProjectId;
            level1.fLevelName = "規劃";
            level1.fEstimateTime = 0;

            tLevel level2 = new tLevel();
            level2.fProjectId = prj.fProjectId;
            level2.fLevelName = "開發";
            level2.fEstimateTime = 0;

            tLevel level3 = new tLevel();
            level3.fProjectId = prj.fProjectId;
            level3.fLevelName = "測試";
            level3.fEstimateTime = 0;

            db.tLevels.Add(level1);
            db.tLevels.Add(level2);
            db.tLevels.Add(level3);
            db.SaveChanges();

            return "success";
        }


        //==========修改==========//
        public ActionResult Edit(int? fPRJId)
        {
            var project = db.tProjects.Where(p => p.fProjectId == fPRJId).FirstOrDefault();
            return View(project);
        }

        [HttpPost]
        public ActionResult Edit(int? fPRJId, tProject target)
        {
            var project = db.tProjects.Where(p => p.fProjectId == fPRJId).FirstOrDefault();
            project.fPrice = target.fPrice;
            project.fCreatdDate = target.fCreatdDate;
            project.fDateline = target.fDateline;
            project.fEmployeeId = target.fEmployeeId;
            db.SaveChanges();

            return View(project);
        }

        //==========刪除==========//
        public ActionResult Delete(int? fId)
        {
            if (fId == null)
                return RedirectToAction("List");

            //todo:擔心不一致 需要封包
            //------刪除階段(多個)----------//
            var levels = db.tLevels.Where(m => m.fProjectId == fId).ToList();
            if (levels != null)
            {
                for (int i = 0; i < levels.Count; i++)
                {
                    db.tLevels.Remove(levels[i]);
                }
            }

            //------刪除專案預算書(一個)------//
            var budget = db.tBudgets.Where(m => m.fProjectId == fId).FirstOrDefault();
            if (budget != null)
                db.tBudgets.Remove(budget);

            //todo:tProjectDetail create完成後再確認一次刪除
            //-----刪除專案工作項目(多個)-----//
            var prjDetail = db.tProjectDetails.Where(m => m.fProjectId == fId).ToList();
            if (prjDetail != null)
            {
                for(int i =0;i<prjDetail.Count;i++)
                {
                    db.tProjectDetails.Remove(prjDetail[i]);
                }
            }

            //---------刪除專案(一個)--------//
            var prj = db.tProjects.Where(m => m.fProjectId == fId).FirstOrDefault();

            if (prj == null)
                return RedirectToAction("List");

            db.tProjects.Remove(prj);
            db.SaveChanges();

            return RedirectToAction("List");
        }


        //======================專案預算書==========================//

        //==========新增=======專案列表新增時，一併新增專案預算書=====//
        //==========編輯==========//
        public ActionResult EditBudget(int? prjId)
        {
            if (TempData.ContainsKey("prjId"))
            {
                var prjId_fromCreateLevel = TempData["prjId"] as int?;

                CVM_BudgetLevel bl = new CVM_BudgetLevel();
                bl.budget = db.tBudgets.Where(p => p.fProjectId == prjId_fromCreateLevel).FirstOrDefault();
                bl.levels = db.tLevels.Where(m => m.fProjectId == prjId_fromCreateLevel).ToList();
                return View(bl);
            }
            else
            {
                CVM_BudgetLevel bl = new CVM_BudgetLevel();
                bl.budget = db.tBudgets.Where(p => p.fProjectId == prjId).FirstOrDefault();
                bl.levels = db.tLevels.Where(m => m.fProjectId == prjId).ToList();
                return View(bl);
            }
        }

        [HttpPost]
        public ActionResult EditBudget(int? prjId, CVM_BudgetLevel target)
        {
            //---------編輯階預算書--------//
            var budget = db.tBudgets.Where(p => p.fProjectId == prjId).FirstOrDefault();
            budget.fSalaryHour = target.budget.fSalaryHour;
            budget.fManagementFeePct = target.budget.fManagementFeePct;

                
            CVM_BudgetLevel bl = new CVM_BudgetLevel();
            bl.levels = db.tLevels.Where(m => m.fProjectId == prjId).ToList();            //多個時段
            bl.budget = db.tBudgets.Where(m => m.fProjectId == prjId).FirstOrDefault();   //一個預算書
                
            //--------編輯階段表(多個)-----//
            var level = db.tLevels.Where(m => m.fProjectId == prjId).ToList();
            for (int i = 0; i < level.Count(); i++)
            {
                level[i].fEstimateTime = Convert.ToInt32(target.levels[i].fEstimateTime);
            }

            int totaltime = 0;
            for (int i = 0; i < level.Count; i++)
            {
                totaltime += Convert.ToInt32(level[i].fEstimateTime);
            }

            int personnelCost= Convert.ToInt32(totaltime * budget.fSalaryHour);
            int managementFee = Convert.ToInt32(bl.budget.fManagementFeePct * bl.budget.tProject.fPrice);
            int totalBuget = personnelCost + managementFee;

            budget.fBudget = totalBuget;

            db.SaveChanges();
            return View(bl);
        }

        //======================專案階段表==========================//
        //==========新增==========//

        [HttpPost]
        public ActionResult CreateLevel(string prjId, string levelName)
        {
             TempData["prjId"] = Convert.ToInt32(prjId);

            tLevel level = new tLevel();
            level.fProjectId = Convert.ToInt32(prjId);
            level.fLevelName = levelName;
            level.fEstimateTime = 0;

            db.tLevels.Add(level);
            db.SaveChanges();
            return RedirectToAction("EditBudget");
        }
        //==========編輯==========//
        //todo:未建立view
        //public ActionResult EditLevel(int? fId)
        //{
        //    var level = db.tLevels.Where(p => p.fLevelId == fId).FirstOrDefault();
        //    return View(level);
        //}


        public ActionResult EditLevel(string prjId, string levelName,string levelId)
        {
            TempData["prjId"] = Convert.ToInt32(prjId);
            var _prjId = Convert.ToInt32(prjId);

            var level = db.tLevels.Where(p => p.fLevelId == _prjId).FirstOrDefault();
            level.fLevelName = levelName;
            db.SaveChanges();

            return RedirectToAction("EditBudget");
        }

        //public ActionResult EditLevel(int? fId, tLevel target)
        //{
        //    var level = db.tLevels.Where(p => p.fLevelId == fId).FirstOrDefault();
        //    level.fLevelName = target.fLevelName;
        //    db.SaveChanges();

        //    return RedirectToAction("EditBudget");
        //}

        //==========刪除==========//
        public ActionResult DeleteLevel(int? fId, int? prjId)
        {
            int a = Convert.ToInt32(fId);
            int b = Convert.ToInt32(prjId);

            //計下目前案號傳給 list
            TempData["prjId"] = Convert.ToInt32(prjId);

            var level = db.tLevels.Where(p => p.fLevelId == fId).FirstOrDefault();
            db.tLevels.Remove(level);
            db.SaveChanges();

            return RedirectToAction("EditBudget");
        }


    }
}
