var lastsel;

$(function () {
    
    $("#gridSaleInvoice_Detail").jqGrid({
        colNames: ['Item', ' Description', 'Qty', 'Unit Price', 'Disc %', 'Account', 'Tax Rate', 'Untitled', 'Amount'],
        colModel: [
                        {
                            name: 'Code', index: 'Code', sortable: false,
                            align: 'left', width: '100',sorttype:'text',
                            editable: true, edittype: 'text'
                        },
                        {
                            name: 'Reference', index: 'Reference', sortable: true,
                            align: 'left', width: '200',
                            editable: false, edittype: 'text'
                        },
                        {
                            name: 'ContactName', index: 'ContactName', sortable: true,
                            align: 'left', width: '200',
                            editable: false, edittype: 'text'
                        },
                        {
                            name: 'CreatedDate', index: 'CreatedDate', sortable: false, formatter: "date", formatoptions: { newformat: "m/d/Y" },
                            align: 'left', width: '100', sorttype: 'text',
                            editable: false, edittype: 'text'
                        },
                        {
                            name: 'DueDate', index: 'DueDate', sortable: false, formatter: "date", formatoptions: { newformat: "m/d/Y"},
                            align: 'left', width: '100',
                            editable: false, edittype: 'text'
                        }
        ],
        pager: jQuery('#good_pager'),
        sortname: 'Code',
        rowNum: 10,
        rowList: [10, 20, 25, 50],
        sortorder: "",
        height: 225,
        viewrecords: true,
        rownumbers: true,
        caption: 'Good list',
        imgpath: '/Content/jqGridCss/smoothness/images',
        width: 927,
        url: "/SaleInvoice/ListInvoiceDetail",
        //editurl: "/SaleInvoice/InvoiceCRUDAction",
        datatype: 'json',
        mtype: 'GET',
        onCellSelect: function (rowid, iCol, aData) {

            if (rowid && rowid !== lastsel) {

                if (lastsel)
                    jQuery('#gridSaleInvoice_Detail').jqGrid('restoreRow', lastsel);
                jQuery('#gridSaleInvoice_Detail').jqGrid('editRow', rowid, true);
                lastsel = rowid;
            }
        }
    })
    jQuery("#gridSaleInvoice_Detail").jqGrid('navGrid', '#good_pager',
    { edit: true, add: true, del: false, search: false, refresh: true },
            {
                closeOnEscape: true, reloadAfterSubmit: true,
                closeAfterEdit: true, left: 400, top: 300
            },
            {
                closeOnEscape: true, reloadAfterSubmit: true,
                closeAfterAdd: true, left: 450, top: 300, width: 520
            },
            { closeOnEscape: true, reloadAfterSubmit: true, left: 450, top: 300 });

});