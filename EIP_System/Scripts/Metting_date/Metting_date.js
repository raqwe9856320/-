
/**
   * Created by heart on 2017/4/26.
   */

(function () {
    var old_data = null;
    $(document).ready(function () {
        startlist();
        clander_1();
        $("#select_room").change(({ target: { value: v } }) => changeroom(v))
        $("#select_room").change(({ target: { value: v } }) => { startlist(v), $('#list>h3').hide(); })
        $("#close").click(hidden);
        $("#edit").on('mousedown', function (event) {
            if (event.target == event.currentTarget)
                hidden();
        });

        //edit
        $("#fStarttime").change(({ target: { value: v } }) => {
            timelist(v, $('#fEndtime'));
            $('#fEndtime').val("請選時間");            
        });
        $("#fEndtime").change(({ target: { value: v } }) => {
            selectedTime(new Date(`2020-01-01 ${$("#fStarttime").val()}`), new Date(`2020-01-01 ${v}`));
            $("#fStarttime").focusout();
        });    
        $("#fStarttime").attr('data-checktime', '');
        $("#fEndtime").attr('data-checktime', '');
        $("#fDate").attr('data-checkdate', '');
        $('#fDate').change(({ target: { value: v } }) => timelistcreate(v))
        //createshow
})
    function listclick(info) {
        var date = new Date(info).toLocaleDateString("fr-CA");
        $('#clickday').val(date);
        let R = $('#select_room').val();
        $.get(`/api/clickDayData/clickdaydata/${R}/${date}`, {}, function (data) {
            $('#list>h2').text(`搜尋日期:${date}`).css("text-align", 'left');
            $('#create-new').on('click', createshow)
            if (((new Date(date).getMonth() == new Date().getMonth() && new Date(date).getDate() > new Date().getDate()))
                || (new Date(date).getMonth() > new Date().getMonth()))
                $('#list>h3').show();
            else if ((new Date(date).getDate() == new Date().getDate() && new Date(date).getMonth() == new Date().getMonth()) &&
                (new Date().getHours() < 17 || (new Date().getHours() == 17 && new Date().getMinutes() < 30)))
                $('#list>h3').show();
            else
                $('#list>h3').hide();

            let table = $('#home-table');
            table.show();
            table.DataTable().destroy();
            table = table[0];
            table.innerText = '';
            let theadRow = table.createTHead().insertRow(-1);
            theadRow.classList.add('table-head');
            theadRow.insertCell(-1).innerText = '開會日期';
            theadRow.insertCell(-1).innerText = '借用人';
            theadRow.insertCell(-1).innerText = '開會原由';
            theadRow.insertCell(-1).innerText = '會議室';
            theadRow.insertCell(-1).innerText = '開始時間';
            theadRow.insertCell(-1).innerText = '結束時間';
            let tbodyRow = table.createTBody()
            for (let i of data) {
                let tr = tbodyRow.insertRow(-1);
                tr.insertCell(-1).innerText = i.date;
                tr.insertCell(-1).innerText = i.Borrower;
                tr.insertCell(-1).innerText = i.Reason;
                tr.insertCell(-1).innerText = i.room + "會議室";
                tr.insertCell(-1).innerText = i.starttime.match(/^\d{1,2}:\d{1,2}/)[0];
                tr.insertCell(-1).innerText = i.endtime.match(/^\d{1,2}:\d{1,2}/)[0];
            }
            datatable();
            if (new Date(date).getDay() == 0 || new Date(date).getDay() == 6) {
                $('#list>h3').hide();
                $('#home-table').DataTable().destroy();
                $('#home-table').hide();
                $("#list>h2").text("假日無預約").css("text-align", 'center');
            }
        }, 'json');
    }

    function startlist() {
        let R = $('#select_room').val();
        let U = $('#userid').val();
        $.get(`/api/clickDayData//startlist/${R}/${U}`, {}, function (data) {
            let table = $('#home-table');
            table.DataTable().destroy();
            table = table[0];
            table.innerText = '';
            let theadRow = table.createTHead().insertRow(-1);
            theadRow.classList.add('table-head');
            theadRow.insertCell(-1).innerText = '開會日期';
            theadRow.insertCell(-1).innerText = '開會原由';
            theadRow.insertCell(-1).innerText = '會議室';
            theadRow.insertCell(-1).innerText = '開始時間';
            theadRow.insertCell(-1).innerText = '結束時間';
            theadRow.insertCell(-1).innerText = '編輯/刪除';
            $('#list>h2').text(`目前使用者:${data[0].Borrower}`);
            if (data[0].date != null) {
                console.log(data);
                let tbodyRow = table.createTBody();
                for (let i of data) {
                    let tr = tbodyRow.insertRow(-1);
                    tr.insertCell(-1).innerText = i.date;
                    tr.insertCell(-1).innerText = i.reason;
                    tr.insertCell(-1).innerText = i.room + "會議室";
                    tr.insertCell(-1).innerText = i.starttime.match(/^\d{1,2}:\d{1,2}/)[0];
                    tr.insertCell(-1).innerText = i.endtime.match(/^\d{1,2}:\d{1,2}/)[0]
                    let cell = tr.insertCell(-1);
                    let edit = document.createElement('a');
                    cell.appendChild(edit);
                    edit.innerText = '編輯  ';
                    edit.href = `javascript:void(0)`;
                    edit.onclick = function () {
                        $.get(`/api/clickDayData/clickevent/${i.id}`, {}, function (data) {
                            editshow(data)
                        });
                    };
                    let del = document.createElement('a');
                    cell.appendChild(del);
                    del.innerText = '刪除';
                    del.href = `javascript:void(0)`
                    del.onclick = () => confirm('確定刪除嗎?') ? location.href = `/monthtest/Delete?fId=${i.fid}` : 0;
                    deledit();
                 }
            
            }
            datatable();
        }, 'json')
    }

    function deledit() {
        let now = new Date($.ajax({ async: false }).getResponseHeader("Date"))
        $('#list > .table tr').toArray().forEach(function (tr) {
            var startTime = new Date(`${$('td:first-child', tr).text().trim()} ${$('td:nth-child(4)', tr).text().trim()}`);
            if (new Date(now) > startTime) $('td:nth-child(6)', tr).html('');
        })
    };

    function changeroom(v) {
        let room = $('#select_room').val();
        var calendarEl = document.getElementById('calendar_1');
        $.get(`/api/clickDayData/start/${room}`, {}, function (data) {
            $("#room").val($("#select_room").val())
            calendar.removeAllEventSources();
            calendar.addEventSource({
                url: `/api/clickDayData/start/${room}`,
                type: 'get',
                color: 'yellow',
                textColor: 'green'
            });
        }, 'json');

    };

    function datatable() {
        $('#list > .table').DataTable({
            paginate: true,
            lengthMenu: [8, 10, 15],
            colums: [{ 'width': '20%' }, { 'width': '20%' }, { 'width': '20 %' }, { ' width': '20%' }, { 'width': '10%' }, { 'width': '10%' }],
            order: [[0, "desc"]]
        });
    }

    var calendar = null;

    function clander_1() {
        var room = $('#select_room').val()
        var calendarEl = document.getElementById('calendar_1');
        calendar = new FullCalendar.Calendar(calendarEl, {
            initialView: 'dayGridMonth',
            locale: 'zh-TW',
            headerToolbar: {
                left: 'prev,next',
                center: 'title',
                right: 'dayGridMonth,timeGridWeek'
            },
            titleFormat: {
                year: 'numeric',
                month: '2-digit'
            },
            height: 600,
            buttonText: { today: '今天', month: '月', week: '周', listWeek: '列表' },
            fixedWeekCount: false,
            slotMinTime: "08:00:00",
            allDaySlot: false,
            slotEventOverlap: false,
            slotMaxTime: "17:00:00",
            slotDuration: "00:15",
            dayMaxEventRows: 'true',
            selectable: 'true',
            select: function (info) {
                listclick(info.startStr);
            },
            eventDidMount: function (info) {
                $(info.el).tooltip({
                    title: info.event.extendedProps.description,
                    placement: "top",
                    trigger: "hover",
                    container: "body"
                });
                console.log(info.timeText)
            },
            eventSources: [
                {
                    url: `/api/clickDayData/start/${room}`, // use the `url` property
                    color: 'yellow',
                    textColor: 'green'
                }

            ],
            eventTimeFormat: { // like '14:30:00'
                hour: '2-digit',
                minute: '2-digit',
                hour12: false
            },
            eventClick: function (info) {
                if (new Date() < new Date(info.event.startStr)) {
                    console.log(info.event._def.extendedProps.description.split(":")[1])
                    if ($("#list>h2").text().split(":")[1] == info.event._def.extendedProps.description.split(":")[1]) {
                        $.get(`/api/clickDayData/clickevent/${info.event.id}`, {}, function (data) {
                            editshow(data);
                        });
                    }
                    
                }

            }
        });

        calendar.render();
    }

    function hidden() {
        $("#dialog-form").hide();
        $("#edit").hide()
        $("#fStarttime").val("");
        $("#fEndtime").val("");
        $("#fReason").val("");
        $("#fId").val("");
        $("#fRoom").val("");
        $("#fEmployeeId").val("");
        $("#fName").val("");
        document.getElementById("fStarttime").setAttribute('selected-item', '');
        document.getElementById("fEndtime").setAttribute('selected-item', '');
        $('#editfrom').validator('destroy')
    }
    function editshow(data) {
        $("#title").text("變更會議時間")
        $("#editfrom").attr('action', '/Metting_room/edit')
        if (document.getElementById("fId") == null) {
            var fid = document.createElement('input');
            fid.type = 'hidden';
            fid.name = fid.id = "fId";
            document.getElementById("editfrom").appendChild(fid);
        }        
        $("#dialog-form").show();
        $("#edit").show();
        timelistcreate(data[0].date);
        document.getElementById("fStarttime").setAttribute('selected-item', data[0].starttime.match(/^\d{1,2}:\d{1,2}/));
        document.getElementById("fEndtime").setAttribute('selected-item', data[0].endtime.match(/^\d{1,2}:\d{1,2}/));
        document.getElementById("fReason").value = data[0].reason;
        if ((new Date().getHours() === 16 && new Date().getMinutes() < 30) || new Date().getHours() < 16)
            fDate.min = new Date().getFullYear().toString() + "-" + ('0' + (new Date().getMonth() + 1).toString()).slice(-2) + "-" + new Date().getDate().toString()
        else
            fDate.min = new Date().getFullYear().toString() + "-" + ('0' + (new Date().getMonth() + 1).toString()).slice(-2) + "-" + (new Date().getDate() + 1).toString()
        $("#fDate").val(data[0].date);
        document.getElementById("fId").setAttribute('value', data[0].fid);
        $("#send").val("Save")
        isrood();      
    }
    function isrood() {
        $("#fDate").attr('required', 'required');
        $("#fDate").attr('data-pattern-error', '請輸入日期');
        $("#fReason").attr('required', 'required');
        $("#fReason").attr('data-error', '請輸入內容');
        $('#editfrom').validator({
            custom: {
                checktime(v) {
                    var fStarttime = new Date(`2020-01-01 ${$("#fStarttime").val()}`);
                    var fEndtime = new Date(`2020-01-01 ${$("#fEndtime").val()}`);
                    var eventid = $("#fId").val()
                    if (fStarttime.toString() == "Invalid Date" || fEndtime.toString() == "Invalid Date")
                        return "請選正確時間";
                    
                    for (let t of old_data) {
                        console.log(t.starttime, t.endtime);
                        if (
                            t.id != eventid &&
                            fStarttime < new Date(`2020-01-01 ${t.endtime}`) &&
                            fEndtime > new Date(`2020-01-01 ${t.starttime}`)
                        ) {
                            return '時間衝突';
                        }
                    }
                },
                checkdate(v) {
                    var clickdate = new Date($("#fDate").val()).getDay();
                    if (clickdate == 0 || clickdate == 6)
                    {
                        return "請勿選假日時間";
                    }

                }
            }            
        }).on('submit', function (e) {
            if (e.isDefaultPrevented())  // 未驗證通過 則不處理
                return;
            //e.preventDefault(); // 防止原始 form 提交表单
        });
    }
    function createshow() {
        $("#title").text("申請會議室")
        $("#editfrom").attr('action', '/Metting_room/create')
        if (document.getElementById("fId") != null) {
            document.getElementById("fId").remove();
        }  
        $("#dialog-form").show();
        $("#edit").show();
        var date=$("#list>h2").text().split(":")[1]
        timelistcreate(date);
        $("#fDate").val(date)
        $("#fRoom").val($("#select_room").val());
        $("#fEmployeeId").val($("#userid").val());
        $("#fName").val($("#user").val());
        $("#send").val("Create");
        if ((new Date().getHours() === 16 && new Date().getMinutes() < 30) || new Date().getHours() < 16)
            fDate.min = new Date().getFullYear().toString() + "-" + ('0' + (new Date().getMonth() + 1).toString()).slice(-2) + "-" + new Date().getDate().toString()
        else
            fDate.min = new Date().getFullYear().toString() + "-" + ('0' + (new Date().getMonth() + 1).toString()).slice(-2) + "-" + (new Date().getDate() + 1).toString()
        isrood();
    }

    //edit

    function timelist(startTime, endTimeStlecter) {
        if (startTime == null) startTime = '23:59:59';
        let endtime = $('option', endTimeStlecter);

        console.log(startTime)

        startTime = new Date(`2020-01-01 ${startTime}`);

        for (let i = 1; i < endtime.length; i++) {
            if (new Date(`2020-01-01 ${endtime[i].value}`) <= startTime) endtime[i].style.display = 'none';
            else endtime[i].style.display = 'block';
        }
    }

    function clearSelfSelected(startTime, endTime) {
        for (let i = startTime; i < endTime; i.setTime(i.getTime() + 1000 * 60 * 30)) {
            $(`.bar > .box_list[time='${i.toTimeString().match(/^\d{1,2}:\d{1,2}/)[0]}']`).removeClass('used');
        }
    }

    function selectedTime(startTime, endTime) {
        let newTimes = [];
        for (let i = startTime; i < endTime; i.setTime(i.getTime() + 1000 * 60 * 30)) {
            newTimes.push(i.toTimeString().match(/^\d{1,2}:\d{1,2}/)[0]);
        }
        for (let box of document.querySelectorAll('.bar > .box_list')) {
            console.log(box.getAttribute('time'), box.getAttribute('time') in newTimes)
            if (newTimes.includes(box.getAttribute('time'))) {
                box.classList.add('selected');
            } else {
                box.classList.remove('selected');
            }
        }
    }


    function updatetimelist(v) {
        var p = new Promise((resolve, reject) => {
            $.get(`/api/clickDayData/changedaylist/${v}`, {}, function (data, result, status) {
                if (status.status != 200) {
                    reject(data);
                    return;
                }
                resolve(data);
            });
        });
        return p;
    }

    function oldata_data(v) {
        var r = $("#select_room").val()
        var p = new Promise((resolve, reject) => {

            $.get(`/api/clickDayData/clickDaydata/${r}/${v}`, {}, function (data, result, status) {
                if (status.status != 200) {
                    reject(data);
                    return;
                }
                resolve(data);
            });
        });
        return p;
    }

    async function timelistcreate(v) {
        var opt = document.getElementById('fStarttime');
        var ope = document.getElementById('fEndtime');
        var s = []
        var a = await updatetimelist(v);
        let i = 0;
        old_data = await oldata_data(v)
        var tlis = document.getElementById('timelist');
        opt.innerText = ""
        ope.innerText = ""
        tlis.innerText = "";

        for (let item of a.starttimelist) {
            let op = document.createElement('option');
            op.innerText = item;
            if (new Date(`2020-01-01 ${item}`).getTime() == new Date(`2020-01-01 ${opt.getAttribute('selected-item')}`).getTime())
                op.setAttribute('selected', '');
            opt.appendChild(op);
            if (i++ == 0) continue;
            let div = document.createElement('div')
            let ditext = document.createElement('span')
            div.setAttribute('time', item)
            div.classList.add('box_list');
            tlis.appendChild(div);
            ditext.innerText = item;
            div.appendChild(ditext);

        }
        for (let item of a.endtimelist) {
            let op = document.createElement('option');
            op.innerText = item;
            if (new Date(`2020-01-01 ${item}`).getTime() == new Date(`2020-01-01 ${ope.getAttribute('selected-item')}`).getTime())
                op.setAttribute('selected', '');
            ope.appendChild(op);
        }
        for (let item of old_data)
            for (let s = new Date(`2020-01-01 ${item.starttime}`); s < new Date(`2020-01-01 ${item.endtime}`); s.setTime(s.getTime() + 1000 * 60 * 30))
                $(`.bar > .box_list[time='${s.toTimeString().match(/^\d{1,2}:\d{1,2}/)[0]}']`).addClass('used')

        selectedTime(new Date(`2020-01-01 ${$("#fStarttime").val()}`), new Date(`2020-01-01 ${$('#fEndtime').val()}`));
        timelist($('#fStarttime').val(), $('#fEndtime').val())
        clearSelfSelected(new Date(`2020-01-01 ${$("#fStarttime").val()}`), new Date(`2020-01-01 ${$('#fEndtime').val()}`));

    }  
})()