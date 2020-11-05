using EIP_System.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace EIP_System.ViewModels
{
    public class VMCreateLeave
    {

        //該員工資訊
        public VMEmployee employee { get; set; }
        //該員工請假時數表
        public List<tLeavecount> leavecountList { get; set; }
        //該部門所有員工
        public List<VMEmployee> employeelist;
        //假別列表
        public List<tleavesort> leavesortList { get; set; }

        //請假資訊
        [Required]
        [DisplayName("假別")]
        public string leavesort { get; set; }

        [DisplayName("起始時間")]
        [DisplayFormat(ApplyFormatInEditMode = true)]
        public DateTime start { get; set; }

        [DisplayName("結束時間")]
        [DisplayFormat(ApplyFormatInEditMode = true)]
        public DateTime end { get; set; }

        [DisplayName("時數計算(單位/小時)")]
        public double timecount { get; set; }

        [DisplayName("事由")]
        public string reason { get; set; }

        [Required]
        [DisplayName("審核主管")]
        public string supervisorId { get; set; }


    }
}