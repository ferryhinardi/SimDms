$(document).ready(function () {
    var options = {
        title: "Maintain Invoice",
        xtype: "panels",
        toolbars: [
            { name: "btnNew", text: "New", icon: "icon-file" },
            { name: "btnBrowse", text: "Browse", icon: "icon-search" }
        ],
        panels: [
            {
                title: "Invoice Information",
                items: [
                        {
                            text: "Branch",
                            type: "controls",
                            items: [
                                { name: "BranchCode", cls: "span2", placeHolder: "Branch Code", readonly: true },
                                { name: "BranchName", cls: "span6", placeHolder: "Branch Name", readonly: true }                        
                            ]
                        },

                        { name: "InvoiceNo", text: "Invoice No", cls: "span4", type: "popup", btnName: "btnInvoiceNo" },
                        { name: "InvoiceDate", text: "Invoice Date", cls: "span4", readonly: true, type: "date"},
                        

                        { name: "JobOrderNo", text: "Job Order No", cls: "span4", readonly: true },
                        { name: "JobOrderDate", text: "Job Order Date", cls: "span4", readonly: true, type: "date"},
                        
                        { name: "PoliceRegNo", text: "Police Registration No", cls: "span4", readonly: true },
                        { name: "BasicModel", text: "Basic Model", cls: "span4", readonly: true },

                        { name: "ChassisNo", text: "Chassis No", cls: "span4", readonly: true },
                        { name: "EngineNo", text: "Engine No", cls: "span4", readonly: true },
                                        
                        { name: "Odometer", text: "Odometer", cls: "span4", readonly: true }
                ]
            },
            {
                xtype: "tabs",
                name: "tabpage1",
                pnlname: "pnlDetail",
                items: [
                    { name: "Customer", text: "Customer Information" },
                    { name: "TaskPart", text: "Part / Job Detail Information" },
                    { name: "TotalService", text: "Service Amount & AR Information" },
                ]
            },
            {
                cls: "tabpage1 Customer",               
                title: "Customer Information",
                items: [
                        {
                            text: "Customer Bill",
                            type: "controls",
                            items: [
                                { name: "CustomerCode", text: "Customer Bill", cls: "span2", type: "popup", btnName: "btnCustomerCode" },
                                { name: "CustomerName", cls: "span6", readonly: true },
                            ]
                        },
                        { name: "Address1", text: "Address", readonly: true },
                        { name: "Address2", readonly: true },
                        { name: "Address3", readonly: true },
                        { name: "Address4", readonly: true },
                            {
                                text: "City Code",
                                type: "controls",
                                items: [
                                        { name: "CityCode", text: "City Code", cls: "span2", readonly: true },
                                        { name: "CityDesc", cls: "span6", readonly: true },
                                ]
                            },
                        { name: "PhoneNo", text: "Phone No", cls: "span4", readonly: true },
                        { name: "TOPDesc", text: "TOP", cls: "span4", readonly: true },
                        { name: "NPWPNo", text: "NPWP No", cls: "span4", readonly: true },
                        {
                            name: "NPWPDate", text: "NPWP Date", cls: "span4", readonly: true, type: "date",
                            mRender: function (data, type, full) {
                                return moment(data).format('DD MMM YYYY');
                            }
                        },

                        { name: "LaborDiscPct", text: "Labor Disc", cls: "span2 number", readonly: true },
                        { name: "PartsDiscPct", text: "Part Disc", cls: "span2 number", readonly: true },
                        { name: "MaterialDiscPct", text: "Material Disc", cls: "span2 number", readonly: true },
                ]                
            },            
            {
                cls: "tabpage1 TaskPart",
                title: "Part / Job Detail Information",
                xtype: "table",
                pnlname: "pnlTaskPart",
                tblname: "tblTaskPart",
                items: [
                        { name: "OrderGroup", cls: "hide"},
                        { name: "PartJobNo", text: "Part / Job", cls: "span4", type: "popup", btnName: "btnPartJob" },
                        { name: "QtyNK", text: "Qty / NK", cls: "span2 number" },

                        { name: "Price", text: "@Price", cls: "span2 number" },
                        { name: "DiscPct", text: "Discount", cls: "span2 number" },

                        { name: "PriceNet", text: "Net Price", cls: "span2 number" },
                        { name: "PartName", text: "Note", cls: "span4"},
                        {
                            type: "buttons", items: [
                                { name: "btnSaveDtl", text: "Save", icon: "icon-save" },
                                { name: "btnCancelDtl", text: "Cancel", icon: "icon-undo" },
                                //{ name: "btnRefreshDtl", text: "Refresh", icon: "icon-refresh" }
                            ]
                        }
                ],
                columns: [
                    { name: "OrderGroup", cls: "hide" },
                    { text: "Edit", type: "edit", width: 50 },
                    { name: "RecNo", text: "No" },
                    { name: "PartJobNo", text: "Task/Part" },
                    { name: "QtyNK", text: "Qty/NK", cls: "right" },
                    { name: "Price", text: "@Price", cls: "right" },
                    { name: "DiscPct", text: "Discount", cls: "right" },
                    { name: "PriceNet", text: "Net Price", cls: "right" },
                    { name: "PartName", text: "Note" },
                ]
            },
            {
                cls: "tabpage1 TotalService",
                title: "Total Service Amount",               
                items: [
                    { name: "LaborDppAmt", text: "DPP - Labor", cls: "span3 number", readonly: true },
                    { name: "TotalDppAmt", text: "Total DPP", cls: "span3 number", readonly: true },

                    { name: "MaterialDppAmt", text: "DPP - Material", cls: "span3 number", readonly: true },
                    { name: "TotalPpnAmt", text: "Total PPN", cls: "span3 number", readonly: true },

                    { name: "PartsDppAmt", text: "DPP - Part", cls: "span3 number", readonly: true },
                    { name: "TotalSrvAmt", text: "Total Service Amount", cls: "span3 number", readonly: true },
                ]
            },
            {
                cls: "tabpage1 TotalService",
                title: "Acc. Receive Information",
                items: [
                    { name: "StatusAR", text: "AR Status", cls: "span3", readonly: true },
                    { name: "ReceiveAmt", text: "Receive Amount", cls: "span3 indent number", readonly: true },
                    { name: "BlockAmt", text: "Block Amount", cls: "span3 number", readonly: true },
                    { name: "DebetAmt", text: "Debet Amount", cls: "span3 number", readonly: true },
                    { name: "CreditAmt", text: "Credit Amount", cls: "span3 indent number", readonly: true },
                ]
            }
        ]
    }

    var widget = new SimDms.Widget(options);
    widget.default = {};
    widget.render(function () {
        widget.post("sv.api/maintaininv/default", function (result) {          
            widget.default = result;            
            widget.populate(result);
            $('.tabs').hide();
        });
    });

    widget.lookup.onDblClick(function (e, data, name) {
        widget.populate($.extend({}, widget.default, data));
        widget.populateTable({ selector: "#tblTaskPart", url: "sv.api/maintaininv/taskpartlist?invoiceno=" + data.InvoiceNo });
        $('.tabs').show();
        widget.lookup.hide();

        if (name == 'CustCodeBillList') {
            widget.post("sv.api/maintaininv/GetCustomerInfo", { custCode: data.CustomerCode, invoiceNo: $('#InvoiceNo').val() }, function (result) {
                widget.populate(result.info);
            });
        }
    });

    widget.onTableClick(function (icon, row) {
        SetMandatory($('#InvoiceNo').val(), row[0]);
        switch (icon) {
            case "edit":
                $("#pnlTaskPart").slideDown();
                $("#btnAddDtl").parent().hide();
                $("#tblTaskPart td .icon").removeClass("link");

                var data = {
                    OrderGroup: row[0],
                    PartJobNo: row[3],
                    QtyNK: row[4],
                    Price: row[5],
                    DiscPct: row[6],
                    PriceNet: row[7],
                    PartName: row[8],
                }
                widget.populate(data, "#pnlTaskPart")
                break;            
            default:
                break;
        }
    });

    $("#btnSaveDtl").on("click", function () {
        var data = $("#pnlTaskPart").serializeObject();
        var param = '?invoiceNo=' + $('#InvoiceNo').val();
        param += '&custCodeBill=' + $('#CustomerCode').val();
        param +=  '&totSrvAmt=' + $('#TotalSrvAmt').val();
        widget.post("sv.api/MaintainInv/Save" + param, data, function (result) {
            if (result.success)
            {
                listDetail();
                widget.populateTable({ selector: "#tblTaskPart", url: "sv.api/maintaininv/taskpartlist?invoiceno=" + $('#InvoiceNo').val() });
                widget.post("sv.api/MaintainInv/GetInvoiceData", { invoiceNo: $('#InvoiceNo').val() }, function (result2) {
                    widget.populate(result2.invoice);
                });
            }
        });
    });

    $("#btnCancelDtl").on("click", function () { listDetail(); });

    function listDetail() {
        $("#pnlTaskPart").slideUp();
        $("#tblTaskPart td .icon").addClass("link");
        $("#btnAddDtl").parent().show();
    }

    $("#btnNew").on("click", function () {
        widget.clearForm();
        widget.populate(widget.default);
        widget.populateTable({ selector: "#tblTaskPart", url: "sv.api/maintaininv/taskpartlist?invoiceno=" + $('#InvoiceNo').val() });
        $('.tabs').hide();
    });

    $("#btnBrowse,#btnInvoiceNo").on("click", function () {
        listDetail();
        widget.lookup.init({
            name: "CustList",
            title: "Job Order List",
            source: "sv.api/grid/invoicesmaintaininv",
            sortings: [[0, "desc"]],
            columns: [
                { mData: "InvoiceNo", sTitle: "Invoice No", sWidth: "110px" },
                {
                    mData: "InvoiceDate", sTitle: "Invoice Date", sWidth: "130px",
                    mRender: function (data, type, full) {
                        return moment(data).format('DD MMM YYYY');
                    }
                },
                { mData: "FPJNo", sTitle: "Invoice Tax No" },
                 {
                     mData: "FPJDate", sTitle: "Invoice Tax Date", sWidth: "130px",
                     mRender: function (data, type, full) {
                         return moment(data).format('DD MMM YYYY');
                     }
                 },
                { mData: "JobOrderNo", sTitle: "Job Order No" },

                {
                    mData: "JobOrderDate", sTitle: "Job Order Date", sWidth: "130px",
                    mRender: function (data, type, full) {
                        return moment(data).format('DD MMM YYYY');
                    }
                },
            ]
        });
        widget.lookup.show();        
    });

    $("#btnCustomerCode").on("click", function () {
        if ($('#InvoiceNo').val() == '' || $('#InvoiceNo').val() == undefined) {
            return;
        }

        widget.lookup.init({
            name: "CustCodeBillList",
            title: "Pembayar Lookup",
            source: "sv.api/grid/InvoicesMaintainCustCodeBill",
            sortings: [[0, "desc"]],
            columns: [
                { mData: "CustomerCode", sTitle: "Kode Pelanggan", sWidth: "110px" },
                { mData: "CustomerName", sTitle: "Nama Pelanggan", sWidth: "160px" },
                { mData: "Address", sTitle: "Alamat", sWidth: "300px" },
            ]
        });
        widget.lookup.show();
    });

    function SetMandatory(invoiceNo, orderGroup) {
        if (orderGroup == 0) {
            if (invoiceNo != null && invoiceNo.startsWith("INF")) {
                $('#PartJobNo,#btnPartJobNo,#PriceNet,#PartName').attr('readonly', true);
                $('#QtyNK,#Price,#DiscPct').attr('readonly', false);
            }
            else {
                $('#Price,#PriceNet,#PartName').attr('readonly', true);
                $('#PartJobNo,#btnPartJobNo,#QtyNK,#DiscPct').attr('readonly', false);
            }
        }
        else {
            $('#PartJobNo,#btnPartJobNo,#QtyNK,#PriceNet,#PartName').attr('readonly', true);
            $('#Price,#DiscPct').attr('readonly', false);
        }
    };

    $('#Price,#DiscPct').on("blur", function () {
        var price = parseInt($('#Price').val().replace(",", ""));
        var disc = parseInt($('#DiscPct').val().replace(",", ""));
        var total = price - (disc / 100 * price);
        $('#PriceNet').val(total);
    });
});