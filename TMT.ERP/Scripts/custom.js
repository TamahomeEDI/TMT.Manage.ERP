var custom = {
    Init: function () {
        $('#cbCheckAll').checkAll();
     
        $("input[type=text]").bind("input propertychange", function () {
            CheckDangerousScript(this);
        });

        $("textarea").on("input propertychange", function () {
            CheckDangerousScript(this);
        });


        $('.datepicker').attr('readonly', true);
        
        $(".datepicker").datepicker({ dateFormat: 'dd-M-yy' });
        //xét giá trị lớn nhất cho Create date
        $('.dtFrom').attr('readonly', true);
        $('.dtFrom').datepicker({
            dateFormat: 'dd-M-yy',
            onClose: function (selectedDate) {
                $(".dtTo").datepicker("option", "minDate", selectedDate);
            }
        });
        //xét giá trị nhỏ nhất cho due date
        $('.dtTo').attr('readonly', true);
        $('.dtTo').datepicker({
            dateFormat: 'dd-M-yy',
            onClose: function (selectedDate) {
                $(".dtFrom").datepicker("option", "maxDate", selectedDate);
            }
        });
        
        $('.dt_To').datepicker({
            dateFormat: 'dd-M-yy',
            beforeShow: function () {
                //get date startDate is set to
                var txt = $('#aStartDate').text().trim();

                var startDate = Date.parseExact(txt, "dd-MMM-yyyy");
                
                //if a date was selected else do nothing
                if (startDate != null) {
                    startDate.setMonth(startDate.getMonth());
                    $(this).datepicker('option', 'minDate', startDate);
                }
            }
        });

        //$(".datepicker").datepicker({ dateFormat: 'dd-M-y', minDate: "dateToday" });  setup date không cho chọn nhỏ hơn ngày hiện tại
        //changeMonth: true, cho phép thay đổi tháng
        //changeYear: true, cho phép thay đổi năm
        //numberOfMonths: 1,
        //yearRange: ":2016", cho phép chọn đến năm

        $(".accordion").accordion();
        $(".numeric").numeric();
        //xóa ký tự nếu di chuyển hoặc copy ký tự vào kiểu số
        $(".numeric").on("input propertychange", function () {
            if (this.value != "") {
                return this.value = $(this).val().replace(/[^0-9\.]/g, '');
            }            
        });

        $(".readonly").attr('readonly', true);

        //$(".colCheckValid").change(function () {//bôi mầu cho grid nếu có
        //    if ($(this).val() == "") {
        //        $(this).parent().parent().find(".required").addClass('checkvalid');
        //        $(this).parent().parent().find(".required").removeClass("required");
        //    }
        //    if ($(this).parent().parent().find(".checkvalid").length > 0) {
        //        $(this).parent().parent().find(".checkvalid").addClass("required");
        //        $(this).parent().parent().find(".checkvalid").removeClass("checkvalid");
        //    }
        //});

        //$("#tabs").tabs();
        //$("#tabs").tabs({
        //    activate: function (event, ui) {
        //        ui.newPanel.find("#awaitApprovePurchase").trigger('applyWidgets');
        //    }
        //});
        /*  var tablesorterOptions = {
              theme: 'blue',
              widgets: ["zebra"]
          };
  
          $("#tabs").tabs({
              create: function (event, ui) {
                  var $t = ui.panel.find('table');
                  if ($t.length) {
                      $t.tablesorter(tablesorterOptions);
                  }
              },
              activate: function (event, ui) {
                  var $t = ui.newPanel.find('table');
                  if ($t.length) {
                      if ($t[0].config) {
                          $t.trigger('applyWidgets');
                      } else {
                          $t.tablesorter(tablesorterOptions);
                      }
                  }
              }
          });*/

    },
    /******************Le Lam begin  add here ******************************/
    /*Create dialog
    _dialog: div which show dialog
    _title: Show dialog title 
    _with: width of div tag
    _height: height of div tag
    _url: Url of dialog
    _openStatus: Open or close status of dialog
    */
    CreatePopup: function (_dialog, _title, _width, _height, _url, _openStatus) {
        $(_dialog).dialog({
            title: _title,
            modal: true,
            autoOpen: _openStatus,
            open: function () {                
                $(this).load(_url);
            },
            width: _width,
            height: _height,
            minWidth: _width,
            minHeight: _height,
            resizable: false,
            overlay: true,
            position: ['middle', 250],
            close: function () {
                location.reload();
            }
        });
    },
    /******************Le Lam end add here ********************************/

    /*MinhNQ begin*/
    Create_Popup: function (_dialog, _title, _width, _height, _url, _openStatus) {
        $(_dialog).dialog({
            title: _title,
            modal: true,
            autoOpen: _openStatus,
            open: function () {
                $(this).load(_url);
            },
            width: _width,
            height: _height,
            minWidth: _width,
            minHeight: _height,
            position: ['middle', 200],
            resizable: false,
            overlay: true
        });
    },
    /*MinhNQ end*/

    ShowBug: function (iStatus) {
        if (iStatus != 0) {
            var bSetDefault = true;
            $.each($(".colCheckValid"), function () {                
                if ($(this).val() != "") {                    
                    var stup = $(this).parent().parent().find(".checkvalid");
                    $.each(stup, function () {
                        if ($(this).val() == "") {
                            
                            $(this).addClass("required error");
                            $(this).removeClass("checkvalid");
                        }
                    });
                    bSetDefault = false;
                }                
            });
            if (bSetDefault && $(".colCheckValid").parent().parent().parent().find("tr:first").find("#ddlGoodID").val() == "") {
                //first element is empty
                $(".colCheckValid").parent().parent().parent().parent().find("tbody > tr:gt(0)").remove();
                var stup = $(".colCheckValid").parent().parent().find(".checkvalid");
                $.each(stup, function () {                    
                    if ($(this).val() == "") {
                        $(this).addClass("required error");
                        $(this).removeClass("checkvalid");
                    }
                });
            }
        }
        else
        {
            $.each($(".colCheckValid").parent().parent().find(".required.error"), function () {
                $(this).addClass("checkvalid");
                $(this).removeClass("required error");
            });
        }
        //if (($(".colCheckValid").val() == "") || (iStatus == 0)) {

        //}

        //$(".colCheckValid").change(function (e) {//bôi mầu cho grid nếu có
        //    e.preventDefault();
        //    if ((iStatus == 2) && ($(".colCheckValid").val() != "")) {
        //        if ($(".colCheckValid").parent().parent().find(".checkvalid").length > 0) {
        //            $(".colCheckValid").parent().parent().find(".checkvalid").addClass("required");
        //            $(".colCheckValid").parent().parent().find(".checkvalid").removeClass("checkvalid");
        //        }
        //    }
        //    else {
        //        $(this).parent().parent().find(".required").addClass("checkvalid");
        //        $(this).parent().parent().find(".required").removeClass("required error");
        //    }
        //});
        
    }
}

function CheckDangerousScript(elm) {    
    var indexSpection1 = $(elm).val().indexOf('<');
    var indexSpection2 = $(elm).val().indexOf('>');
    var indexSpection3 = $(elm).val().indexOf('[');
    var indexSpection4 = $(elm).val().indexOf(']');
    var indexSpection5 = $(elm).val().indexOf('{');
    var indexSpection6 = $(elm).val().indexOf('}');    
    if (((indexSpection1 > -1) && (indexSpection2 > -1)) || ((indexSpection3 > -1) && (indexSpection4 > -1)) || ((indexSpection5 > -1) && (indexSpection6 > -1))) {        
        $(elm).val("");
        $(elm).focus();
    }
}

function IsEmpty(obj) {
    return (typeof obj === "undefined" || obj == null || obj.length === 0);
}

$(document).ready(function () {
    $("#backtotop").click(function () {
        $('html, body').animate({ scrollTop: 0 }, 'slow');
    });
});

var roleArray = [];
var controlInRoleArray;

function setControlShowHide(userId, form) {
    
    var isAdmin = false;
    roleArray = GetRoleFromUserID(userId);
    controlInRoleArray = GetControlInRole(form);
    for (var i = 0; i < roleArray.length; i++) {
        if (roleArray[i] == 'Admin') {
            isAdmin = true;
        }
    }
    if (!isAdmin) {
        //  var obj1 = jQuery.parseJSON(controlInRoleArray);
        if (controlInRoleArray instanceof Array) {
            $.each(controlInRoleArray, function () {
                var controlID = this.controlID;
                var data = this.data;
                setPermission4Control(data, controlID);
            });
        }
    }
}

function setPermission4Control(data, controlID) {
    
    for (var i = 0; i < roleArray.length; i++) {
        if (data instanceof Array) {
            $.each(data, function () {
                
                var rolename = this.rolename;
                var viewtype = this.viewtype;
                if (rolename == roleArray[i]) {
                    //hidden
                    if (viewtype == 1) {
                        $('#' + controlID).attr('disabled', 'disabled');
                    }
                    else if (viewtype == 2) {
                        $('#' + controlID).css("display", "block");
                    }
                    else if (viewtype == 0) {
                        $('#' + controlID).css("display", "none");
                    }
                }
            });
        }
    }
}

function GetRoleFromUserID(userId) {

    var roleArray = [];
    $.ajax({
        type: "POST",
        url: "/Services/UserService.asmx/GetRoleByUserID",
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        async: false,
        data: JSON.stringify({
            userID: userId
        }),

        success: function (data) {
            roleArray = JSON.parse(data.d);
        }
    });
    return roleArray;
}

function GetControlInRole(form) {

    var controlInRoleArray;
    $.ajax({
        type: "POST",
        url: "/Services/UserService.asmx/GetControlInRole",
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        async: false,
        data: JSON.stringify({
            functionName: form
        }),

        success: function (data) {
            controlInRoleArray = JSON.parse(data.d);
        }
    });
    return controlInRoleArray;
}

function CreatePopupMessage(_controlID, _message, _title, _url) {
    $('#' + _controlID).empty();
    $('#' + _controlID).append('<label style="width: 100%; font-weight: bold; font-size: 12px; margin-top: 10px; margin-bottom: 10px;">' + _message + ' </label>');
    $('#' + _controlID).dialog({
        title: _title,
        modal: true,
        height: 160,
        width: 400,
        position: ['middle', 250],
        autoOpen: false,
        buttons: {
            'OK': function () {
                window.location.href = _url;
                $(this).dialog('close');
            }
        }
    });
    $('#' + _controlID).dialog('open');
}

function CreatePopupMessage(_controlID, _message, _title, _height, _width, _url) {
    $('#' + _controlID).empty();
    $('#' + _controlID).append('<p style="width: 100%; font-weight: bold; font-size: 12px; height:0px !important; text-align: center;">' + _message + ' </p>');
    $('#' + _controlID).dialog({
        title: _title,
        modal: true,
        height: _height,
        width: _width,
        autoOpen: false,
        position: ['middle', 250],
        buttons: {
            'OK': function () {
                $(this).dialog('close');
            }
        },
        close: function () {
            window.location.href = _url;
            //$(document).unbind('click');
        }
    });
    $('#' + _controlID).dialog('open');
}

function ConfirmPopupMessage(_controlID, _message, _title, _height, _width) {
    var retConfirm = false;
    $('#' + _controlID).empty();
    $('#' + _controlID).append('<p style="width: 100%; font-weight: bold; font-size: 12px; height:0px !important; text-align: center;">' + _message + ' </p>');
    $('#' + _controlID).dialog({
        title: _title,
        modal: true,
        height: _height,
        width: _width,
        autoOpen: false,
        position: ['middle', 250],
        buttons: {
            'OK': function () {
                $(this).dialog('close');
                retConfirm = true;
            },
            'Cancel': function () {
                $(this).dialog('close');
                retConfirm = false;
            }
        }
    });
    $('#' + _controlID).dialog('open');
    return retConfirm;
}

//function ConfirmPopupMessage(_controlID, _message, _title, _height, _width) {
//    var defer = $.Deferred();
//    $('#' + _controlID).empty();
//    $('#' + _controlID).append('<p style="width: 100%; font-weight: bold; font-size: 12px; height:0px !important; text-align: center;">' + _message + ' </p>');
//    $('#' + _controlID).dialog({
//        title: _title,
//        modal: true,
//        height: _height,
//        width: _width,
//        autoOpen: false,
//        buttons: {
//            'OK': function () {
//                defer.resolve("true");
//                $(this).dialog('close');
//            },
//            'Cancel': function () {
//                defer.resolve("false");
//                $(this).dialog('close');
//            }
//        }
//    });
//    $('#' + _controlID).dialog('open');
//    return defer.promise();
//}


function isNumber(evt) {
    evt = (evt) ? evt : window.event;
    var charCode = (evt.which) ? evt.which : evt.keyCode;
    if (charCode > 31 && (charCode < 48 || charCode > 57)) {
        return false;
    }
    return true;
}
//obtion 0: nextmonth, 1: nextday, 2: currentmonth
// billDate : 15-Jan-2014
function getDueDate(billDate, due, obtion) {
    
    var dueDate = "";
    var dateString = [];
    var strNextMonthYear = ""
    if (billDate != "") {
        dateString = billDate.split('-');
        switch (obtion) {
            //next month
            case "0":
                strNextMonthYear = getNextMonth(dateString[1], dateString[2]);
                dueDate = due < 10 ? '0' + due + '-' + strNextMonthYear : due + '-' + strNextMonthYear;
                break;
                //next bill date
            case "1":
                dueDate = AddDate(billDate, due)
                break;
                //current month
            case "2":
                dueDate = due < 10? '0' + due + '-' + dateString[1] + '-' + dateString[2]: due + '-' + dateString[1] + '-' + dateString[2];
                break;
            default:
                break;
        }
    }
    return dueDate;
}

function getNextMonth(month, year) {
    
    var nextMonth = 0;
    var strNextMonthYear = "";
    var months = new Array('Jan', 'Feb', 'Mar', 'Apr', 'May', 'Jun', 'Jul', 'Aug', 'Sep', 'Oct', 'Nov', 'Dec');
    for (var i = 0; i < months.length; i++) {
        var strMonth = months[i];
        if (strMonth == month) {
            nextMonth = i + 1;
            if (nextMonth == 12) {
                nextMonth = 0;
                year = year + 1;
            }
            break;
        }
    }
    strNextMonthYear = months[nextMonth] + '-' + year;
    return strNextMonthYear;
}
//return 1, 2, 3 , 4.....
function getMonth(month) {
    var Month = "";
    var months = new Array('Jan', 'Feb', 'Mar', 'Apr', 'May', 'Jun', 'Jul', 'Aug', 'Sep', 'Oct', 'Nov', 'Dec');
    for (var i = 0; i < months.length; i++) {
        var strMonth = months[i];
        if (strMonth == month) {
            Month = i + 1;
            if (Month < 10)
                Month = '0' + i;
            break;
        }
    }
    return Month;
}
//return Jan, Feb, Mar, Apr ....
function getStrMonth(numMonth) {
    var strMonth = "";
    var months = new Array('Jan', 'Feb', 'Mar', 'Apr', 'May', 'Jun', 'Jul', 'Aug', 'Sep', 'Oct', 'Nov', 'Dec');
    strMonth = months[numMonth - 1];
    return strMonth;
}
//return 01, 02 ,03 from dd/MM/yyyy
function getDay(strDate) {
    var dateString = [];
    dateString = strDate.split('-');
    if (dateString != "") {
        return parseInt(dateString[0]);
    }
    return 0;
}

function AddDate(billDate, addDays) {
    
    var strReturnDate = "";
    var dateString = [];
    dateString = billDate.split('-');
    var remainDays_month;
    var nYear = parseInt(dateString[2]);
    var nMonth = parseInt(getMonth(dateString[1]));
    var nDate = parseInt(dateString[0]);
    var remainDays = parseInt(addDays);
 
    if (remainDays > 0) {
        remainDays_month = DaysOfMonth(nYear, nMonth) - nDate;
        if (remainDays > remainDays_month) {
            remainDays = remainDays - remainDays_month;
            nDate = remainDays;
            if (nMonth < 12) { nMonth = nMonth + 1; }
            else {
                nMonth = 1;
                nYear = nYear + 1;
            };
        }
        else {
            nDate = nDate + remainDays;
            remainDays = 0;
        };
        strReturnDate = nDate + '-' + getStrMonth(nMonth) + '-' + nYear;
    }
    return strReturnDate;
};

function DaysOfMonth(nYear, nMonth) {
    switch (nMonth) {
        case 1:     // January
            return 31; break;
        case 2:     // February
            if ((nYear % 4) == 0) {
                return 29;
            }
            else {
                return 28;
            };
            break;
        case 3:     // March
            return 31; break;
        case 4:     // April
            return 30; break;
        case 5:     // May
            return 31; break;
        case 6:     // June
            return 30; break;
        case 7:     // July
            return 31; break;
        case 8:     // August
            return 31; break;
        case 9:     // September
            return 30; break;
        case 10:     // October
            return 31; break;
        case 11:     // November
            return 30; break;
        case 12:     // December
            return 31; break;
    }
}

