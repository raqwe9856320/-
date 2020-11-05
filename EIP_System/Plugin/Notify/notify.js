//小鈴鐺刷新
function bell(list) {
    document.getElementById("bell-dropdown-item").innerHTML = "";
    if (list.length === 0) {
        document.getElementById("bell-dropdown-item").innerHTML =
            '<li class="nav-item">' +
            '<a class="dropdown-item" >' +
            '<span>' +
            '沒有訊息'
        '</span>' +
            '</a >' +
            '</li >';
    }
    else {
        let listlength = 0
        if (list.length > 5)
            listlength = list.length - 5
        for (let i = list.length - 1; i >= listlength; i--) {
            document.getElementById("bell-dropdown-item").innerHTML +=
                '<li class="nav-item">' +
                '<a class="dropdown-item" >' +
                '<span>' +
                '<span>' + list[i].fTitle + '</span>' +
                '<span class="time">3 mins ago+</span>' +
                '</span>' +
                '<span class="message">' + list[i].fContent + '</span>' +
                '</a >' +
                '</li >'
            if (list[i].fType === 0) { bellring += 1 }
        }
        document.getElementById("bell-dropdown-item").innerHTML +=
            '<li class="nav-item"><div class="text-center" ><a class="dropdown-item"><strong>查看所有通知</strong><i class="fa fa-angle-right"></i></a></div ></li >'
        if (bellring != 0) {
            $("#bg-green-counts").css({ "display": "inline-block" })
            document.getElementById("bg-green-counts").innerHTML = bellring
        }
        else
            $("#bg-green-counts").css({ "display": "none" })
    }
}

