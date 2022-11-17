var lastsel;

$(function () {
    
    $("#grid").jqGrid({        
        colNames: ['Item Code', ' Ref', 'To', 'Date', 'Due Date'],
        colModel: [
                        {
                            name: 'Code', index: 'Code', sortable: false,
                            align: 'left', width: '100',
                            editable: true, edittype: 'text'
                        },
                        {
                            name: 'Reference', index: 'Reference', sortable: true,
                            align: 'left', width: '200',
                            editable: true, edittype: 'text'
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
        pager: jQuery('#pager'),
        sortname: 'Code',
        rowNum: 10,
        rowList: [10, 20, 25, 50],
        sortorder: "",
        height: 225,
        viewrecords: true,
        rownumbers: true,
        caption: 'Sale',
        imgpath: '/Content/jqGridCss/smoothness/images',
        width: 927,
        url: "/SaleInvoice/ListInvoice",
        editurl: "/SaleInvoice/InvoiceCRUDAction",
        datatype: 'json',
        mtype: 'GET',
        onCellSelect: function (rowid, iCol, aData) {

            if (rowid && rowid !== lastsel) {

                if (lastsel)
                    jQuery('#grid').jqGrid('restoreRow', lastsel);
                jQuery('#grid').jqGrid('editRow', rowid, true);
                lastsel = rowid;
            }
        }
    })
    jQuery("#grid").jqGrid('navGrid', '#pager',
    { edit: false, add: true, save:true, cancel:true, del: true, search: false, refresh: true },
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