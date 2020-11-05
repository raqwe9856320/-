using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Mvc;
using EIP_System.Models;

namespace EIP_System.Controllers
{
    public class EmployeeController : Controller
    {
        EIP_DBEntities db = new EIP_DBEntities();
        // GET: Employee
        public ActionResult Index()
        {
            return View();
        }
        public ActionResult Getdata()
        {
            //using (EIP_DBEntities db = new EIP_DBEntities())
            //{
            //    //List<tEmployee> empList = db.tEmployees;

            //}
                var hello = from p in db.tEmployees
                            select new
                            {
                                fEmployeeId = p.fEmployeeId,
                                fName = p.fName,
                                fIdent = p.fIdent,
                                fPassword = p.fPassword.Substring(0, 1) + "****" + p.fPassword.Substring(p.fPassword.Length - 1, 1),                                
                                fDepartment = p.fDepartment,
                                fTitle = p.fTitle,
                                fEmail = p.fEmail,
                                //fBirth=p.fBirth.Year+"-"+p.fBirth.Month+"-"+ p.fBirth.Day,
                                fBirth = p.fBirth.Year + "-" + p.fBirth.Month + "-" + p.fBirth.Day,
                                fPhonePersonal = p.fPhonePersonal,
                                fState = p.fState,
                                fAuth = p.fAuth,
                            };
            var f = hello.ToList();
                return Json(new { data = hello.ToList() }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult Create(tEmployee target)
        {            
            if (target.fEmployeeId > 0)
            {
                //修改
                DateTime Now = DateTime.Now;
                tEmployee updateEmp = db.tEmployees.Where(emp => emp.fEmployeeId == target.fEmployeeId).FirstOrDefault();
                //updateEmp.fEmployeeId = target.fEmployeeId;
                updateEmp.fName = target.fName;
                updateEmp.fIdent = target.fIdent;
                updateEmp.fPassword = target.fPassword;
                updateEmp.fDepartment = target.fDepartment;
                updateEmp.fTitle = target.fTitle;
                updateEmp.fEmail = target.fEmail;
                updateEmp.fBirth = target.fBirth;
                updateEmp.fPhonePersonal = target.fPhonePersonal;
                updateEmp.fHireDate = target.fHireDate;
                updateEmp.fState = target.fState;
                updateEmp.fAuth = Convert.ToInt32(target.fAuth);
                updateEmp.fSalaryMonth = target.fSalaryMonth;
                updateEmp.fSalaryHour = target.fSalaryMonth / 30 / 8;
                updateEmp.fPhoneHouse = target.fPhoneHouse;
                updateEmp.fCountry = target.fCountry;
                updateEmp.fBirthPlace = target.fBirthPlace;
                updateEmp.fEducation = target.fEducation;
                updateEmp.fAddressNow = target.fAddressNow;
                updateEmp.fAddressOrigin = target.fAddressOrigin;
                updateEmp.fEngyName = target.fEngyName;
                updateEmp.fEngyPhone = target.fEngyPhone;
                TimeSpan fOld = Now.Subtract(updateEmp.fHireDate);
                updateEmp.fOld = Math.Round(fOld.TotalDays, 1);
                updateEmp.fFireDate = target.fFireDate;
                updateEmp.fBackDate = target.fBackDate;

                db.SaveChanges();
            }
            else
            {
                //身分證驗證(資料倒入)
                
                if (target.fName == null)
                {
                    return Json("empty", JsonRequestBehavior.AllowGet);
                }
                else
                {
                    string idcheck = target.fIdent;
                    bool regId = Regex.IsMatch(idcheck, @"^[A-Za-z]{1}[1-2]{1}[0-9]{8}$");
                    //密碼驗證(資料倒入)///////
                    string pwdcheck = target.fPassword;
                    bool regPwd = Regex.IsMatch(pwdcheck, @"^.*(?=.{6,})(?=.*\d)(?=.*[a-zA-Z]).*$");
                    if(regId && regPwd)
                    {
                        //新增
                        DateTime Now = DateTime.Now;
                        tEmployee emp = new tEmployee();
                        //emp.fEmployeeId = target.fEmployeeId;
                        emp.fName = target.fName;
                        emp.fName = target.fName;
                        emp.fIdent = target.fIdent;
                        emp.fPassword = target.fPassword;
                        emp.fDepartment = target.fDepartment;
                        emp.fTitle = target.fTitle;
                        emp.fEmail = target.fEmail;
                        emp.fBirth = target.fBirth;
                        emp.fPhonePersonal = target.fPhonePersonal;
                        emp.fHireDate = target.fHireDate;
                        emp.fState = "試用期";
                        emp.fAuth = Convert.ToInt32(target.fAuth);
                        emp.fSalaryMonth = target.fSalaryMonth;
                        emp.fSalaryHour = target.fSalaryMonth / 30 / 8;
                        emp.fPhoneHouse = target.fPhoneHouse;
                        emp.fCountry = target.fCountry;
                        emp.fBirthPlace = target.fBirthPlace;
                        emp.fEducation = target.fEducation;
                        emp.fAddressNow = target.fAddressNow;
                        emp.fAddressOrigin = target.fAddressOrigin;
                        emp.fEngyName = target.fEngyName;
                        emp.fEngyPhone = target.fEngyPhone;
                        TimeSpan fOld = Now.Subtract(emp.fHireDate);
                        emp.fOld = Math.Round(fOld.TotalDays, 1);
                        emp.fFireDate = target.fFireDate;
                        emp.fBackDate = target.fBackDate;
                        db.tEmployees.Add(emp);
                        db.SaveChanges();
                    }
                    else
                    {
                    return Json("error", JsonRequestBehavior.AllowGet);
                    }                
                }
            }

            return Json("success", JsonRequestBehavior.AllowGet);
        }
        [HttpPost]
        public JsonResult GetEdit(int id)
        {
            var Monday = from p in db.tEmployees.Where(m => m.fEmployeeId == id)
                         select new
                         {
                             p.fEmployeeId,
                             p.fName,
                             p.fIdent,
                             p.fPassword,
                             p.fDepartment,
                             p.fTitle,
                             p.fEmail,
                             p.fBirth,
                             p.fPhonePersonal,
                             p.fHireDate,
                             p.fState,
                             p.fAuth,
                             p.fCountry,
                             p.fBirthPlace,
                             p.fSalaryMonth,
                             p.fEducation,
                             p.fPhoneHouse,
                             p.fAddressNow,
                             p.fAddressOrigin,
                             p.fEngyName,
                             p.fEngyPhone,
                         };
            //var emp = db.tEmployees.Where(m => m.fEmployeeId == id).FirstOrDefault();


            return Json(new { data = Monday.ToList() }, JsonRequestBehavior.AllowGet);
        }

        public JsonResult Delete(int id)
        {
            var del = db.tEmployees.Where(m => m.fEmployeeId == id).FirstOrDefault();
            db.tEmployees.Remove(del);
            db.SaveChanges();

            return Json("success", JsonRequestBehavior.AllowGet);

        }
    }
}