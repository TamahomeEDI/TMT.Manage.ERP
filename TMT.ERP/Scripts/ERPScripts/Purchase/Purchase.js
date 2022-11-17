$(document).ready(function ($) {
    debugger;
    //hide table search detail
    $("#searchPurchase").hide();
    //if (typeof (_vlDashboard) != "undefined") {
    $('#loading').hide();
    var _vlDashboard = $('#vlDashboard').val();
   LoadContent(_vlDashboard);
    switch (_vlDashboard) {
        case '0':
            removeAttrID();
            $('.item2').attr('id', 'current');
            break;
        case '1':
            removeAttrID();
            $('.item3').attr('id', 'current');
            break;
        case '2':
            removeAttrID();
            $('.item4').attr('id', 'current');
            break;
        case '3':
            removeAttrID();
            $('.item5').attr('id', 'current');
            break;
        case '4':
            removeAttrID();
            $('.item6').attr('id', 'current');
            break;
        case '5':
            removeAttrID();
            $('.item1').attr('id', 'current');
            break;
        }
    //End 
    //Load content by tab
    function LoadContent(id) {
        debugger;
       
        switch (id) {
            case '5':
                tab = "#tabs-1";
                jQuery.ajax({
                    type: 'GET',
                    contentType: 'application/html',
                    url: '/Purchase/All',
                    success: function (response) {
                        $('#loading').hide();
                        DelDataOtherTab(tab);
                        $(tab).html(response);
                        ajaxifyTextSearchButtonHide();
                        ajaxifyTextSearchButton();
                        ajaxifySearchPurchaseButton();
                        ajaxifyDatePicker();                                            
                    }
                });
                break;
            case '0':
                tab = "#tabs-2";              
                jQuery.ajax({
                    type: 'GET',
                    contentType: 'application/html',
                    url: '/Purchase/Draft',
                    success: function (response) {
                        DelDataOtherTab(tab);                      
                        $(tab).html(response);
                        ajaxifyTextSearchButtonHide();
                       // ajaxifyTextSearchButton();
                       // ajaxifySearchPurchaseButton();
                       // ajaxifyDatePicker();                       
                    }
                });
                break;
            case '1':
                tab = "#tabs-3";
                jQuery.ajax({
                    type: 'GET',
                    contentType: 'application/html',
                    url: '/Purchase/AwaitingApprove',
                    success: function (response) {
                        DelDataOtherTab(tab);
                        $(tab).html(response);
                        ajaxifyTextSearchButtonHide();
                        //ajaxifyTextSearchButton();
                        //ajaxifySearchPurchaseButton();
                        //ajaxifyDatePicker();
                    }

                });
                break;
            case '2':
                //pass url3
                tab = "#tabs-4";
                jQuery.ajax({
                    type: 'GET',
                    contentType: 'application/html',
                    url: '/Purchase/AwaitingPayment',
                    success: function (response) {
                        DelDataOtherTab(tab);
                        $(tab).html(response);
                        ajaxifyTextSearchButtonHide();
                        //ajaxifyTextSearchButton();
                        //ajaxifySearchPurchaseButton();
                       // ajaxifyDatePicker();
                    }
                });
                break;
            case '3':
                //pass url3
                tab = "#tabs-5";
                jQuery.ajax({
                    type: 'GET',
                    contentType: 'application/html',
                    url: '/Purchase/Paid',
                    success: function (response) {
                        DelDataOtherTab(tab);
                        $(tab).html(response);
                        ajaxifyTextSearchButtonHide();
                       // ajaxifyTextSearchButton();
                       // ajaxifySearchPurchaseButton();
                       // ajaxifyDatePicker();
                    }
                });
                break;
            case '4':
                tab = "#tabs-6";
                jQuery.ajax({
                    type: 'GET',
                    contentType: 'application/html',
                    url: '/Purchase/Repeating',
                    success: function (response) {
                        DelDataOtherTab(tab);
                        $(tab).html(response);
                        ajaxifyTextSearchButtonHide();
                       // ajaxifyTextSearchButton();
                       // ajaxifySearchPurchaseButton();
                       // ajaxifyDatePicker();
                    }
                });
                break;
            default:
                break;

        }
        return;
    }
    //end
    //End load

    function resortTable() {
        $("#awaitApprovePurchase").tablesorter({
            widthFixed: true,
            widgets: ['zebra'],
            headers: {
                0: {
                    sorter: false
                }
            }
        }).tablesorterPager({
            container: $("#pager")
        });
    }

    function removeAttrID() {
        $('.item1').removeAttr('id');
        $('.item2').removeAttr('id');
        $('.item3').removeAttr('id');
        $('.item4').removeAttr('id');
        $('.item5').removeAttr('id');
        $('.item6').removeAttr('id');
    }

    // Set action listener on tab
    jQuery('ul li').click(function (e) {
       
        $(this).find("a").each(function () {           
            if ($(this).attr('href') == "#tabs-1") {
                removeAttrID();
                $('.item1').attr('id', 'current');
                $('#loading').show();
                LoadContent('5');
            }

            if ($(this).attr('href') == "#tabs-2") {
                removeAttrID();
                $('.item2').attr('id', 'current');
                LoadContent('0');
            }

            if ($(this).attr('href') == "#tabs-3") {
                //pass url3
                removeAttrID();
                $('.item3').attr('id', 'current');
                LoadContent('1');

                $("#awaitApprovePurchase").ajaxStop(function () {
                    $("#awaitApprovePurchase").tablesorter();
                });

            }

            if ($(this).attr('href') == "#tabs-4") {
                //pass url3
                removeAttrID();
                $('.item4').attr('id', 'current');
                LoadContent('2');
            }

            if ($(this).attr('href') == "#tabs-5") {
                //pass url3
                removeAttrID();
                $('.item5').attr('id', 'current');
                LoadContent('3');
            }

            if ($(this).attr('href') == "#tabs-6") {
                removeAttrID();
                $('.item6').attr('id', 'current');
                LoadContent('4');
            }

        });
    });


    function DelDataOtherTab(tab) {
          switch (tab) {
              case '#tabs-1':
             $('div#tabs-1 div').remove();
             $('div#tabs-2 div').remove();
             $('div#tabs-3 div').remove();
             $('div#tabs-4 div').remove();
             $('div#tabs-5 div').remove();
             $('div#tabs-6 div').remove();
             break;
         case '#tabs-2':
             $('div#tabs-1 div').remove();
             $('div#tabs-3 div').remove();
             $('div#tabs-4 div').remove();
             $('div#tabs-5 div').remove();
             $('div#tabs-6 div').remove();
             break;
        case '#tabs-3':
            $('div#tabs-1 div').remove();
            $('div#tabs-2 div').remove();
            $('div#tabs-4 div').remove();
            $('div#tabs-5 div').remove();
            $('div#tabs-6 div').remove();
            break;
        case '#tabs-4':
            $('div#tabs-1 div').remove();
            $('div#tabs-2 div').remove();
            $('div#tabs-3 div').remove();
            $('div#tabs-5 div').remove();
            $('div#tabs-6 div').remove();
            break;
        case '#tabs-5':
            $('div#tabs-1 div').remove();
            $('div#tabs-2 div').remove();
            $('div#tabs-3 div').remove();
            $('div#tabs-4 div').remove();
            $('div#tabs-6 div').remove();
            break;
        case '#tabs-6':
            $('div#tabs-1 div').remove();
            $('div#tabs-2 div').remove();
            $('div#tabs-3 div').remove();
            $('div#tabs-4 div').remove();
            $('div#tabs-5 div').remove();
            break;
         default:
             break;
             return;
     } 
  }

    function ajaxifyTextSearchButtonHide() {
        $("#searchPurchase").hide();
    }
    function ajaxifyDatePicker() {
        $(".datepicker").datepicker();
    }

    //  $(document).on('click', "#btnTextSearch", function () {
    function ajaxifyTextSearchButton() {
        $("#btnTextSearch").click(function () {
            $("#searchPurchase").show();
        });
    }




    //call action search for tab all
    function ajaxifySearchPurchaseButton() {
        $("#btnSearchPurchase").click(function () {
            var data2Send = { 'tab': tab, 'referenceName': $("#referenceName").val(), 'CreatedDate': $("#CreatedDate").val(), 'DueDate': $("#DueDate").val() };
            var referenceName = $("#referenceName").val();
            var createdDate = $("#CreatedDate").val();
            var dueDate = $("#DueDate").val();
            data2Send = JSON.stringify(data2Send);
            jQuery.ajax({
                type: 'POST',
                url: '/Purchase/SearchPurchase',
                data: data2Send,
                contentType: 'application/json; charset=utf-8',

                success: function (response) {
                    alert(response);               
                    // result = $($.parseHTML(response)).filter("#allpurchase");
                    $(tab).html(response);
                    $("#referenceName").val(referenceName);
                    $("#CreatedDate").val(createdDate);
                    $("#DueDate").val(dueDate);
                }
            });
        });
    }

    


    function RemoveRow(element) {
        debugger;
        var tr = $(element).closest('tr');
        tr.remove();
        CalSubTotal();
        CalTotal();
    }

    // trigger event when button is clicked
    $("#addNewLine_Purchase").unbind('click').click(function (e) {
        debugger;
        // add new row to table using addTableRow function
        addTableRow();
        e.preventDefault();
        //return false;
    });
    $("#addNewLineRP").unbind('click').click(function (e) {
        debugger;
        // add new row to table using addTableRow function
        addTableRow();
        e.preventDefault();
        //return false;
    });
    $("#addNewLineCN").unbind('click').click(function (e) {
        debugger;
        // add new row to table using addTableRow function
        addTableRow();
        e.preventDefault();
        //return false;
    });
    
    // function to add a new row to a table by cloning the last row and
    // incrementing the name and id values by 1 to make them unique
    function addTableRow() {
        debugger;
        // clone the last row in the table
        var emptyText = " ";
        var $tr = jQuery('div#form_body table#tblPurchaseDetail').find("tbody tr:last").clone(true);
        $tr.find("input,select").val('');
       // var $tr = $(table).find("tbody tr:last").clone();

     /*   $tr.attr("id", function () {
            var parts = this.id.match(/(\D+)(\d+)$/);
            return parts[1] + ++parts[2];
        });
        // get the name attribute for the input and select fields
        $tr.find("input,select").attr("name", function () {
            // break the field name and it's number into two parts
            var parts = this.id.match(/(\D+)(\d+)$/);
            // create a unique name for the new field by incrementing
            // the number for the previous field by 1
            return parts[1] + ++parts[2];
            // repeat for id attributes
        }).attr("id", function () {
            var parts = this.id.match(/(\D+)(\d+)$/);
            return parts[1] + ++parts[2];
        });
        //$tr.find("a").attr("id", function () {
        //    var parts = this.id.match(/(\D+)(\d+)$/);
        //    return parts[1] + ++parts[2];
        //}); 
        // append the new row to the table
        $(table).find("tbody tr:last").after($tr);*/

        jQuery('div#form_body table#tblPurchaseDetail').find("tbody tr:last").after($tr);
        CalSubTotal();
        CalTax();
        CalTotal();
    };

    function RemoveRow(element) {
        var tr = $(element).closest('tr');
        tr.remove();
        CalSubTotal();
        CalTax();
        CalTotal();
        CalDiscToTal();
    }


    $("#chkVAT").change(function () {
        debugger;       
       // var include = @Resources.Resource.PO_Include_Purchase_Tax;
       // var exclude = Resources.Resource.PO_Total_Purchase_Tax;
        if ($('#chkVAT').prop('checked')) //Inclusive
            $('#taxlbl').text('Include Tax');
        else
            $('#taxlbl').text('Total Tax');
        CalTax();
        CalTotal();
    });

    $(".goodchange").change(function () {
        ChangeGood(this);
    });


    function ChangeGood(element) {
        //
        var good = $(element).val();
        var tbody = $(element).closest('tr');
        var goodID = $("#ddlGoodID", tbody).val();
        if (goodID != "") {
        if (goodID > 0) {
            $.ajax({
                type: "POST",
                url: "/Services/GoodService.asmx/GetGoodByIDForPurchase",
                data: JSON.stringify({
                    goodID: goodID
                }),
                contentType: "application/json; charset=utf-8",
                dataType: "json",
                success: function (data) {
                    try {
                        var good = JSON.parse(data.d);
                        $('.description', tbody).val(good.Decription);
                        $('.qty', tbody).val("1");
                        $('.unitprice', tbody).val(good.UnitPrice).formatCurrency();
                        //$('.unitprice', tbody).val(function (good) {
                        //    var amount = good.UnitPrice;
                        //    return amount;
                        //}).formatCurrency();
                        $('.disc', tbody).val("");
                        $('.account', tbody).val(good.Accounts);
                        $('.taxrate', tbody).val(good.TaxRate);
                        $('.taxvalue', tbody).val(good.TaxRateVal);
                        CalAmount(element);
                        CalSubTotal();
                        CalTax();
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

        else {
            $('.description', tbody).val("");
            $('.qty', tbody).val("");
            $('.unitprice', tbody).val("");
            $('.disc', tbody).val("");
            CalAmount(element);
            CalTax();
            CalSubTotal();
            CalTotal();
        }
    }

    function ChangeTax(element) {
        debugger;
        var tr = $(element).closest('tr');
        var taxID = $("#ddlTaxRateID", tr).val();
        if (taxID != "" && taxID > 0) {
           // if (taxID > 0) {
                $.ajax({
                    type: "POST",
                    url: "/Services/GoodService.asmx/GetTaxByID",
                    data: JSON.stringify({
                        taxID: taxID
                    }),
                    contentType: "application/json; charset=utf-8",
                    dataType: "json",
                    success: function (data) {
                        try {
                            var tax = JSON.parse(data.d);
                            $('.taxvalue', tr).val(tax.Rate);
                            CalTax();
                            CalTotal();
                        } catch (ex) { }
                    }
                }).error(function (event, jqXHR, ajaxSettings, thrownError) {
                    alert('Error when loading data');
                });
           // }
        }
        else {
            $('.taxvalue', tr).val("0");
            CalTax();
            CalTotal();
        }
    }

    function CalDiscToTal() {
        $('#discSpan').hide();
        $("#discSpanText").text(function () {
            var value = 0;
            $("#tblPurchaseDetail tr").each(function () {
                var tr = $(this).closest('tr');
                var disc = 0;
                if ($('.disc', tr).val() != "")
                    disc = $('.disc', tr).asNumber();
                var unitprice = 0;
                if ($('.unitprice', tr).val() != "")
                    unitprice = $('.unitprice', tr).asNumber();
                var quantity = 0;
                quantity = $('.qty', tr).asNumber();
                if (disc > 0 & unitprice > 0)
                    $('#discSpan').show();
                value += unitprice * quantity * disc / 100;
            });
            return value;
        }).formatCurrency();
    }


    function CalAmount(element) {
        var tr = $(element).closest('tr');
        $(".amount", tr).val(function () {
            var qty = 0;
            debugger;
            if ($('.qty', tr).val() != "")
                qty = $('.qty', tr).asNumber();

            var unitprice = 0;
            if ($('.unitprice', tr).val() != "")
                unitprice = $('.unitprice', tr).asNumber();

            var disc = 0;
            if ($('.disc', tr).val() != "")
                disc = $('.disc', tr).asNumber();

            var retValue = qty * unitprice;
            if (disc > 0)
                retValue = retValue - retValue * disc / 100;
            return retValue;
        }).formatCurrency();
    }


    function CalSubTotal() {
        $(".subtotal").text(function () {
            var value = 0;
            $(".amount").each(function () {
                if ($(this).val() != "")
                  //  value += parseInt($(this).val());
                    value += $(this).asNumber();
            });
            return value;
        }).formatCurrency();
    }

    function CalTotal() {

        $("#txtPurchaseTotal").text(function () {
            var subtotal = 0;
            if ($('.subtotal').text() != "")
                subtotal = $('.subtotal').asNumber();
            var tax = 0;
            if ($('#txtTax').text() != "")
                tax = $('#txtTax').asNumber();

            if ($('#chkVAT').prop('checked')) //Inclusive
                return subtotal;
            else //Exclusive
                return subtotal + tax;
        }).formatCurrency();
    }

    function CalTax() {
        $("#txtTax").text(function () {
            var value = 0;
            if ($('#chkVAT').prop('checked')) //Inclusive
            {
                $("#tblPurchaseDetail tr").each(function () {
                    var tr = $(this).closest('tr');
                    var taxvalue = $(".taxvalue", tr).asNumber();
                    var qty = 0;
                    if ($('.qty', tr).val() != "")
                        qty = $('.qty', tr).asNumber();

                    var unitprice = 0;
                    if ($('.unitprice', tr).val() != "")
                        unitprice = $('.unitprice', tr).asNumber();

                    var disc = 0;
                    if ($('.disc', tr).val() != "")
                        disc = $('.disc', tr).asNumber();

                    if (taxvalue > 0 & qty > 0 & unitprice > 0)
                        value += qty * ((unitprice - (unitprice * disc / 100)) / ((100 + taxvalue) / 100)) * taxvalue / 100;
                    //value += taxvalue * amount / 100;
                });

            }//Exclusive
            else {
                debugger;
                $("#tblPurchaseDetail tr").each(function () {
                    var tr = $(this).closest('tr');
                    var taxvalue = $(".taxvalue", tr).asNumber();
                    var amount = $(".amount", tr).asNumber();

                    if (taxvalue > 0 & amount > 0)
                        value += taxvalue * amount / 100;
                });
            }
            return value;
        }).formatCurrency();
    }

    $(".taxrate").change(function () {
        ChangeTax(this);
    });

    $(".disc").change(function () {
        CalDiscToTal();
        CalAmount(this);
        CalSubTotal();
        CalTax();
        CalTotal();
    });

    $(".qty").change(function () {
        CalAmount(this);
        CalSubTotal();       
        CalDiscToTal();
        CalTax();
        CalTotal();
    });

    $(".unitprice").change(function () {
        CalAmount(this);
        CalSubTotal();      
        CalDiscToTal();
        CalTax();
        CalTotal();
    });

  /*  function SavePurchase() {
        //alert(BuildJSON());
        debugger;
        if ($('#txtSupplier').val() == "") {
            alert("Input supplier, please");
            $('#txtSupplier').focus();
            return false;
        }

        var contactID = $('#ContactID').val();
        var createDate = $('#CreatedDate').val();
        var dueDate = $('#DueDate').val();
        var reference = $('#Reference').val();
        var currencyID = $('#CurrencyID').val();
        var tax = $('#TaxID').val();

        var createdEmployeeID = 10;

        var txtSubTotal = $('#txtSubTotal').val();
        var totalTax = $('#txtTax').val();
        var totalMoney = $('#txtPurchaseTotal').val();

        var status = 1;

        var purchaseDetail = BuildJSON();
        $.ajax({
            type: "POST",
            url: "/Services/SavePurchaseService.asmx/SavePurchase",
            data: JSON.stringify({
                purchaseID: 0,
                contactID: contactID,               
                createDate: createDate,
                dueDate: dueDate,
                reference: reference,
                currencyID: currencyID,
                createdEmployeeID: createdEmployeeID,
                tax: totalTax,
                totalMoney: totalMoney,
                status: status,
                purchaseDetail: purchaseDetail,
            }),
            contentType: "application/json; charset=utf-8",
            dataType: "json",
            success: function (data) {
                debugger;
                var retValue = JSON.parse(data.d);
                if (!isNaN(retValue))
                    window.location.reload();
            },

            error: function (data) {
                alert('Error in save invoice\n' + data);
            }
        })
    }

    function BuildJSON() {
        var arrayData = [];
        $("#tblPurchaseDetail tr").each(function () {
            var tr = $(this).closest('tr');
            var goodID = $("#ddlGoodID", tr).val();
            if (goodID != "-1" && parseInt(goodID) > 0) {
                var item = {
                    "goodId": goodID,
                    "quantity": $("#txtQty", tr).val(),
                    "price": $("#txtUnitPrice", tr).val(),
                    "taxRateID": $("#ddlTaxRateID", tr).val(),
                    "accountID": $("#ddlAccountID", tr).val(),
                    "totalMoney": $("#txtAmount", tr).val(),
                    "description": $("#txtDescription", tr).val(),
                };
                arrayData.push(item);
            }
        });
        return JSON.stringify(arrayData);
    } */


    function ajaxifyActiveTab() {
        $("table").trigger("update");
        var tablesorterOptions = {
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
                alert($t.html);
                $t.trigger('applyWidgets');
                $t.tablesorter(tablesorterOptions);
                //if ($t.length) {
               // if ($t[0].config) {
               //     $t.trigger('applyWidgets');
               // } else {
                 //   $t.tablesorter(tablesorterOptions);
               // }
                //  }
            }
        });
    }
   
});

