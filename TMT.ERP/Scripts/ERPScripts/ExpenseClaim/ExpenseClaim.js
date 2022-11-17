$(document).ready(function ($) {
    if ($('#chkIncludeTax').prop('checked')) {
        $('#Exclude').show();
        $('#Include').show();
    } else {
        $('#Exclude').hide();
        $('#Include').hide();
    }


    $("#addNewLine").click(function () {
        addTableRow();
        return false;
    });

    function addTableRow() {
        // clone the last row in the table
        var $tr = jQuery('div#myDiv table#tblExpenseClaimDetail').find("tbody tr:last").clone(true);
        jQuery('div#myDiv table#tblExpenseClaimDetail').find("tbody tr:last").after($tr);
        CalTotal();
    };

    $("#chkIncludeTax").change(function () {
        
        if (!$('#chkIncludeTax').prop('checked')) {
            $('#Exclude').hide();
            $('#Include').hide();
            $("#tblExpenseClaimDetail tr").each(function () {
                var tbody = $(this).closest('tr');
                $('#ddlTaxRateID', tbody).attr("disabled", "disabled");
                $('#ddlTaxRateID', tbody).val(0);

            });
            CalTax();
            CalTotal();
        } else {
            $('#Exclude').show();
            $('#Include').show();
            $("#tblExpenseClaimDetail tr").each(function () {
                var tbody = $(this).closest('tr');
                $('#ddlTaxRateID', tbody).removeAttr('disabled');
            });
            CalTax();
            CalTotal();
        }
    });

    $(".accountChange").change(function () {
        ChangeAccount(this);
    });

    function ChangeAccount(element) {
        
        var tbody = $(element).closest('tr');
        var AccountID = $("#ddlAccountID", tbody).val();
        if (AccountID > 0) {
            $.ajax({
                type: "POST",
                contentType: "application/json; charset=utf-8",
                dataType: "json",
                url: "/Services/AccountService.asmx/GetTaxRateByAccountID",
                data: JSON.stringify({ AccountID: AccountID }),
                success: function (data) {
                    try {
                        var account = JSON.parse(data.d);
                        $('#ddlTaxRateID', tbody).val(account.TaxRate);
                        $('.taxvalue', tbody).val(account.TaxRateValue);
                        //CalAmount(element)
                        CalSubTotal();
                        CalTax();
                        CalTotal();

                    } catch (ex) { }
                }
            }).error(function (event, jqXHR, ajaxSettings, thrownError) {
                alert('Error when loading users');
            });
        } else {
            $('#ddlTaxRateID', tbody).val('');
            $('.taxvalue', tbody).val(0);
            // CalAmount(element);
            CalTax();
            CalSubTotal();
            CalTotal();
        }
    }
    $(".Tax").change(function () {
        // else if ($('#TaxInclude').is(":checked")) {
        //    if ($('#TaxExclude').is(":checked")) {
      /*  if ($("#TaxInclude").attr("checked")) {
            $('#TaxInclude').removeAttr('checked');
            $('#TaxExclude').attr('checked');

        }
        else if( $('#TaxExclude').attr('checked')) {
            $('#TaxExclude').removeAttr('checked');
            $('#TaxInclude').attr('checked');
        } */
        CalTax();
        CalTotal();
    });

    $(".taxrate").change(function () {
        ChangeTax(this);
    });

    function ChangeTax(element) {
        var tr = $(element).closest('tr');
        var taxID = $("#ddlTaxRateID", tr).val();
        if (taxID != "") {
            if (taxID > 0) {
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
            }
        }
        else {
            $('.taxvalue', tr).val("0");
            CalTax();
            CalTotal();
        }
    }

    function CalAmount(element) {
        var tr = $(element).closest('tr');
        $(".amount", tr).val(function () {
            var qty = 0;
            
            if ($('.qty', tr).val() != "")
                qty = $('.qty', tr).asNumber();

            var unitprice = 0;
            if ($('.unitprice', tr).val() != "")
                unitprice = $('.unitprice', tr).asNumber();
            var retValue = qty * unitprice;
            return retValue;
        }).formatCurrency();
    }

    function CalSubTotal() {
        $(".subtotal").text(function () {
            var value = 0;
            $(".amount").each(function () {
                if ($(this).val() != "")
                    value += $(this).asNumber();
            });
            return value;
        }).formatCurrency();
    }

    function CalTotal() {

        $("#txtExpenseClaimTotal").text(function () {
            var subtotal = 0;
            if ($('.subtotal').text() != "")
                subtotal = $('.subtotal').asNumber();
            var tax = 0;
            if ($('#txtTax').text() != "")
                tax = $('#txtTax').asNumber();

            if ($('#chkIncludeTax').prop('checked')) //Inclusive
                return subtotal;
            else //Exclusive
                return subtotal + tax;
        }).formatCurrency();
    }



    function CalTax() {
        
        $("#txtTax").text(function () {
            var value = 0;
            $("#tblExpenseClaimDetail tr").each(function () {
                var tr = $(this).closest('tr');
                var rate = 0;
                if ($('#ddlTaxRateID', tr).prop("disabled") == true) {
                    $('.taxvalue', tr).val(0);
                }
                var taxvalue = $(".taxvalue", tr).asNumber();
                if (taxvalue > 0)
                    rate = 1 + taxvalue / 100;
                var amount = $(".amount", tr).asNumber();


                if ($('#TaxExclude').is(":checked")) {
                    if (taxvalue > 0 & amount > 0)
                        value += taxvalue * amount / 100;
                }
                else if ($('#TaxInclude').is(":checked")) {
                    if (taxvalue > 0 & amount > 0)
                        value += amount - (amount / rate);
                }
            });
            return value;
        }).formatCurrency();
    }

    $(".qty").change(function () {
        CalAmount(this)
        CalSubTotal();
        CalTax();
        CalTotal();
    });

    $(".unitprice").change(function () {
        CalAmount(this)
        CalSubTotal();
        CalTax();
        CalTotal();
    });

});

