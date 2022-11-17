$(document).ready(function ($) {

    $('#sPageSize').html($('#hdfPageSize').val());
    $('#cbCheckAllDraft').click(function () {
        $("input[name=chkStockMovement]").prop('checked', $(this).prop('checked'));
    });
    //var tabNum = $('#hidTabNum').val();
    //if (tabNum != null && tabNum != "") {
    //    LoadContent(tabNum);
    //} else {
    //    LoadContent('0');
    //}
    //jQuery('ul.nav li').click(function (e) {
    //    $(this).find("a").each(function () {
    //        if ($(this).attr('href') == "#tabs-1") {
    //            $(this).parent().attr('class', 'active');
    //            $('#aDone').parent().attr('id', '');

    //            //$(this).parent().addClass('active');
    //            $('#aDone').parent().removeClass('active');
    //            //$('#aDone').parent().addClass('selected');
    //            LoadContent('0');
    //        }

    //        if ($(this).attr('href') == "#tabs-2") {
    //            $(this).parent().attr('class', 'active');
    //            $('#aDraft').parent().attr('id', '');

    //            //$(this).parent().addClass('active');
    //            $('#aDraft').parent().removeClass('active');
    //            //$('#aDraft').parent().addClass('selected');
    //            LoadContent('1');
    //        }  
    //    });
    //});

    function LoadContent(i) {
        switch (i) {
            case '0':
                tab = "#tabs-1";
                jQuery.ajax({
                    type: 'GET',
                    contentType: 'application/html',
                    url: '/StockMovement/GetStockMovement?type=0',
                    success: function (response) {
                        DelDataOtherTab(tab);
                        $(tab).html(response);
                    }
                });
                break;
            case '1':
                tab = "#tabs-2";
                jQuery.ajax({
                    type: 'GET',
                    contentType: 'application/html',
                    url: '/StockMovement/GetStockMovement?type=1',
                    success: function (response) {
                        DelDataOtherTab(tab);
                        $(tab).html(response);
                    }
                });
                break;
            default:
                break;
        }
    }

    function DelDataOtherTab(tab) {
        switch (tab) {
            case '#tabs-1':              
                $('div#tabs-2 div').remove();            
                break;
            case '#tabs-2':
                $('div#tabs-1 div').remove();
                break;
            default:
                break;
        }
    }

    $("#addNewLineEdit_TS").click(function () {
        addTableRow();
        e.preventDefault();
    });

    //$("#addNewLineNew_TS").click(function () {
    //    addTableRow();
    //    e.preventDefault();
    //});

    function addTableRow() {
        // clone the last row in the table
        var $tr = jQuery('div#myDiv table#tblStockMovementDetails').find("tbody tr:last").clone(true);
        jQuery('div#myDiv table#tblStockMovementDetails').find("tbody tr:last").after($tr);
        CalTotal();
    };

    function RemoveRow(element) {
        var tr = $(element).closest('tr');
        tr.remove();
        CalTotal();
    }

    $("#delRow").click(function () {
        RemoveRow(this);
        return false;
    });

    $(".ddlgoodchange").change(function () {
        debugger;
        ChangeGoodInStock(this);
    });

    $(".stockchange").change(function () {
        ChangeStock(this);
    });

    function ChangeStock(element) {
        debugger;
       // var tbody = $(element).closest('tr');
        var stockID = $("#ddlStockID").val();
        if (stockID > 0) {
            $.ajax({
                type: 'GET',
                contentType: 'application/html',
                url: "/StockMovement/GetGoodByStockID",
                data: { StockID: stockID },
                success: function (data) {
                    try {
                        $('#tblStockMovement').empty();
                        $('#div#myDiv div#tblStockMovement').remove();
                        $('#tblStockMovement').html(data);
                       // ajaxifyJavascript();

                    } catch (ex) { }
                }
            }).error(function (event, jqXHR, ajaxSettings, thrownError) {
                alert('Error when loading users');
            });
        }
    }

    function ChangeGoodInStock(element) {
        var good = $(element).val();
        var tbody = $(element).closest('tr');
        var goodID = $("#ddlGoodID", tbody).val();
        var stockID = $("#ddlStockID").val();
        if (goodID > 0) {
            $.ajax({
                type: "POST",
                url: "/Services/GoodService.asmx/GetGoodByStockID",
                data: JSON.stringify({
                    goodID: goodID,
                    stockID: stockID
                }),
                contentType: "application/json; charset=utf-8",
                dataType: "json",
                success: function (data) {
                    try {
                        var good = JSON.parse(data.d);
                        $('.description', tbody).val(good.Decription);                       
                        $('.uom', tbody).val(good.umoCode);
                        $('.umoID', tbody).val(good.umoID);
                        $('.qty', tbody).val(good.quantity);
                        $('#totalAmount').val(good.quantity);
                        CalTotal();

                    } catch (ex) { }
                }
            }).error(function (event, jqXHR, ajaxSettings, thrownError) {
                alert('Error when loading users');
            });
        }
        else if (goodID == 0) {
            alert("Add new inventory");
        }
    }
    function CalTotal() {
        $("#txtStockMovementTotal").text(function () {
            var value = 0;
            $(".qty").each(function () {
                if ($(this).val() != "")
                    //  value += parseInt($(this).val());
                    value += $(this).asNumber();
            });
            return value;
        }).asNumber();
    }

    $(".qty").change(function () {
        debugger;
        CalTotal();
    });

  
   
});

