
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Web;
using System.Web.Mvc;
using System.Web.UI.WebControls;
using EIP_System.Models;


namespace TestLayout.Controllers.api
{
    [RoutePrefix("api/clickDayData")]
    public class clickDayDataController : Controller
    {
        // GET: clickDayData

        [Route("clickDaydata/{room}/{day}"), HttpGet]
        public ActionResult clickDaydata(string room, string day)
        {

            EIP_DBEntities db = new EIP_DBEntities();
            var click = DateTime.Parse(day);
            //});
            var dayss = from o in db.tMetting_date
                        from p in db.tEmployees
                        where o.fEmployeeId == p.fEmployeeId && o.fRoom == room && o.fDate == click
                        select new
                        {
                            id = o.fId,
                            date = o.fDate.ToString(),
                            Borrower = p.fName,
                            Reason = o.fReason,
                            starttime = o.fStarttime.ToString(),
                            room = o.fRoom,
                            endtime = o.fEndtime.ToString(),
                        };         
            
            return new ContentResult()
            {
                Content = JsonSerializer.Serialize(dayss),
                ContentEncoding = Encoding.UTF8,
                ContentType = "json"
            };
        }
        [Route("start/{room}"), HttpGet]
        public ActionResult start(string room)
        {
            EIP_DBEntities db = new EIP_DBEntities();
            
            //var days = db.tMetting_date.Where(p => p.fRoom == room).Select(p => new
            //{
            //    id = p.fId,
            //    EmployeeId=p.fEmployeeId,
            //    Borrower ="123" ,
            //    Reason = p.fReason,
            //    starttime = p.fStarttime,
            //    start = p.fDate,
            //    endtime = p.fEndtime,
            //    end = p.fDate
            //});
            var days = db.tMetting_date.Where(c=>c.fRoom==room).Join(db.tEmployees, o => o.fEmployeeId, p => p.fEmployeeId, (o, p) => new
            {
                id = o.fId,
                EmployeeId = o.fEmployeeId,
                Borrower = p.fName,
                Reason = o.fReason,
                starttime = o.fStarttime,
                start = o.fDate,
                endtime = o.fEndtime,
                end = o.fDate
            });
            var day = from d in days.ToList()
                      select new
                      {
                          id = d.id,
                          title = d.Reason,
                          description = "借用人:" + d.Borrower,
                          start = (d.start.Value + d.starttime.Value).ToString("yyyy-MM-ddTHH:mm:ss"),
                          end = (d.end.Value + d.endtime.Value).ToString("yyyy-MM-ddTHH:mm:ss")
                      };

            return new ContentResult()
            {
                Content = JsonSerializer.Serialize(day),
                ContentEncoding = Encoding.UTF8,
                ContentType = "json"
            };
        }
        [Route("startlist/{room}/{user}"), HttpGet]
        public ActionResult startlist(string room, string user)
        {
            EIP_DBEntities db = new EIP_DBEntities();
            int u = Convert.ToInt32(user);
            var userid = db.tEmployees.Where(p => p.fEmployeeId == u).First();
            var userlist = from o in db.tMetting_date
                           from p in db.tEmployees
                           where o.fEmployeeId == p.fEmployeeId && o.fRoom == room && o.fEmployeeId == u
                           select new
                           {
                               id = o.fId,
                               date = o.fDate.ToString(),
                               Borrower = p.fName,
                               reason = o.fReason,
                               room = o.fRoom,
                               starttime = o.fStarttime.ToString(),
                               endtime = o.fEndtime.ToString(),
                           };
            
              var t = from o in db.tEmployees where o.fEmployeeId == u select new { Borrower = o.fName };
            if (userlist.Count() == 0)
            {
                return new ContentResult()
                {
                    Content = JsonSerializer.Serialize(t),
                    ContentEncoding = Encoding.UTF8,
                    ContentType = "json"
                };
            }
            
            return new ContentResult()
            {
                Content = JsonSerializer.Serialize(userlist),
                ContentEncoding = Encoding.UTF8,
                ContentType = "json"
            };
        }

        [Route("changedaylist/{new_day}"), HttpGet]
        public ActionResult changedaylist(DateTime new_day)
        {
            DateTime today = DateTime.Now;
            TimeSpan timestart, fortimes = new TimeSpan(0, 30, 0);
            List<string> starttimelist = new List<string>();
            List<string> endtimelist = new List<string>();

            if (today.Month != new_day.Month || (today.Month == new_day.Month && today.Day != new_day.Day))
                timestart = new TimeSpan(8, 0, 0);
            else if (today.Date == new_day.Date && today.Hour < 8)
                timestart = new TimeSpan(8, 0, 0);
            else if (today.Minute > 30)
                timestart = new TimeSpan(today.Hour + 1, 0, 0);
            else
                timestart = new TimeSpan(today.Hour, 30, 0);
            starttimelist.Add("請選時間");
            endtimelist.Add("請選時間");
            for (int i = 0; i < 18; i++)
            {
                if (timestart.Hours >= 17)
                    break;
                starttimelist.Add(timestart.ToString(@"hh\:mm"));
                endtimelist.Add(timestart.Add(fortimes).ToString(@"hh\:mm"));
                timestart = timestart.Add(fortimes);
            }
            return new ContentResult()
            {
                Content = JsonSerializer.Serialize(new
                {
                    starttimelist = starttimelist,
                    endtimelist = endtimelist
                }),
                ContentEncoding = Encoding.UTF8,
                ContentType = "json"

            };
        }
        [Route("clickevent/{id}"), HttpGet]
        public ActionResult clickevent(string id)
        {
            EIP_DBEntities db = new EIP_DBEntities();
            int i = Convert.ToInt32(id);
            var userlist = db.tMetting_date.Where(p => p.fId == i).Select(p => new
            {
                fid = p.fId,
                date = p.fDate.ToString(),
                reason = p.fReason,
                room = p.fRoom,
                starttime = p.fStarttime.ToString(),
                endtime = p.fEndtime.ToString()
            });
            return new ContentResult()
            {
                Content = JsonSerializer.Serialize(userlist),
                ContentEncoding = Encoding.UTF8,
                ContentType = "json"

            };
        }


    }
}