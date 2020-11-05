using EIP_System.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace EIP_System.ViewModels
{
    public class VMsignoff
    {
        public int id { get; set; }
        public string name { get; set; }
        public string catelog { get; set; }
        public string applyclass { get; set; }
        public string reason { get; set; }
        public string applydate { get; set; }
        public string activedate { get; set; }
        public string enddate { get; set; }
        public string expireddate { get; set; }
        public string passdate { get; set; }
        public string supervisor { get; set; }
        public int? isagreed { get; set; }
        public int? revoke { get; set; }


        public VMsignoff convert(tSignoff tSignoff)
        {
            VMsignoff vmsignoff = new VMsignoff();
            //大項目判斷
            string catelogName = "";
            string name = "";
            string reason = "";
            string applydate = "";
            string expireddate = "";
            if (tSignoff.fLeaveId != null)
            {
                catelogName = "請假申請";
                name = tSignoff.tLeave.tEmployee.fName;
                reason = tSignoff.tLeave.fReason;
                applydate = tSignoff.tLeave.fApplyDate.ToString("yyyy-MM-dd hh:mm");
                expireddate = tSignoff.tLeave.fActiveDate.ToString("yyyy-MM-dd hh:mm");
            }
            else if (tSignoff.fOvertimeId != null)
            {
                catelogName = "加班申請";
                name = tSignoff.tOvertime.tEmployee.fName;
                reason = tSignoff.tOvertime.fReason;
                applydate = tSignoff.tOvertime.fSubmitDate.ToString("yyyy-MM-dd hh:mm");
                expireddate = tSignoff.tOvertime.fActiveDate.ToString("yyyy-MM-dd hh:mm");
            }
            else //(item.fAlpplypunchId == null)
            {
                catelogName = "補打卡申請";
            }

            vmsignoff.id = tSignoff.fId;
            vmsignoff.name = name;
            vmsignoff.catelog = catelogName;
            vmsignoff.applyclass = tSignoff.fApplyClass;
            vmsignoff.reason = reason;
            vmsignoff.applydate = applydate;
            vmsignoff.activedate = tSignoff.fStartdate.ToString("yyyy-MM-dd hh:mm");
            vmsignoff.enddate = tSignoff.fEnddate.ToString("yyyy-MM-dd hh:mm");
            vmsignoff.expireddate = expireddate;
            vmsignoff.passdate = (tSignoff.fPassdate != null) ? ((DateTime)tSignoff.fPassdate).ToString("yyyy-MM-dd hh:mm") : "";
            vmsignoff.supervisor = tSignoff.tEmployee.fName;
            vmsignoff.isagreed = tSignoff.fIsAgreed;

            return vmsignoff;
        }

        public List<VMsignoff> getList(List<tSignoff> signofflist)
        {
            List<VMsignoff> list = new List<VMsignoff>();
            
            foreach (var item in signofflist)
            {
                list.Add(new VMsignoff().convert(item));
            }

            return list;
        }
    }
}