"use strict";

function spEntryReturnServiceController($scope, $http, $injector) {

    var me = $scope;

    $injector.invoke(BaseController, this, { $scope: me });

    me.browse = function () {
        //$('#btnInvoiceNo').removeAttr('disabled');
        var lookup = Wx.blookup({
            name: "InvoiceCancelLookUp",
            title: "Invoice No. Browse",
            manager: pengeluaranManager,
            query: "InvoiceCancelOpenLookUp",
            defaultSort: "InvoiceNo asc",
            columns: [
                { field: "InvoiceNo", title: "Invoice No." },
                {
                    field: "InvoiceDate", title: "Invoice Date", template: "#= (InvoiceDate == undefined) ? '' : moment(InvoiceDate).format('DD MMM YYYY') #"
                },
                { field: 'ReturnNo', title: 'Return No.' },
                {
                    field: "ReturnDate", title: "Return Date", template: "#= (ReturnDate == undefined) ? '' : moment(ReturnDate).format('DD MMM YYYY') #"
                },
                { field: 'CustomerCode', title: 'Customer Code' },
                { field: 'CustomerName', title: 'Customer Name' },
            ]
        });

        lookup.dblClick(function (result) {
            if (result != null) {
                me.isInProcess = true;
                me.loadDetail(result);
                $('#btnPosting').attr('disabled', 'disabled');
                me.Apply();
            }
        });
    }

    me.invoiceCancel = function () {
        me.init();
        var lookup = Wx.blookup({
            name: "InvoiceCancelLookUp",
            title: "Invoice No. Browse",
            manager: pengeluaranManager,
            query: "InvoiceCancelLookUp",
            defaultSort: "InvoiceNo asc",
            columns: [
                { field: "InvoiceNo", title: "Invoice No." },
                {
                    field: "InvoiceDate", title: "Invoice Date", template: "#= (InvoiceDate == undefined) ? '' : moment(InvoiceDate).format('DD MMM YYYY') #"
                },
                { field: 'ReturnNo', title: 'Return No.' },
                {
                    field: "ReturnDate", title: "Return Date", template: "#= (ReturnDate == undefined) ? '' : moment(ReturnDate).format('DD MMM YYYY') #"
                },
                { field: 'CustomerCode', title: 'Customer Code' },
                { field: 'CustomerName', title: 'Customer Name' },
            ]
        });

        lookup.dblClick(function (result) {
            if (result != null) {
                me.isInProcess = true;
                me.loadDetail(result);
                $('#btnPosting').removeAttr('disabled');
                me.Apply();
            }
        });
    }

    me.Posting = function () {
        MsgConfirm("Apakah anda yakin melakukan proses return service ?", function (result) {
            if (result) {
                var datDetail = [];

                $.each(me.detail1, function (key, val) {
                    var arr = {
                        "PartNo": val["PartNo"],
                        "Check": val["Check"]
                    }
                    datDetail.push(arr);
                });

                var dat = {};
                dat["InvoiceNo"] = me.data.InvoiceNo;
                dat["model"] = datDetail;
                var JSONData = JSON.stringify(dat);
                $http.post("sp.api/entryreturnservice/openreturn", JSONData)
               .success(function (e) {
                   if (e.success) {
                       Wx.Success(e.message);
                       console.log(e.InvoiceNo);
                       me.loadDetail({ InvoiceNo: e.InvoiceNo });
                   }
                   else {
                       MsgBox(e.message, MSG_ERROR);
                   }
               })
              .error(function (e) {
                  MsgBox(e, MSG_ERROR);
              });
            }
        });
    }

    me.printPreview = function () {
        var data = me.data.InvoiceNo + "," + me.data.InvoiceNo + "," + "profitcenter";
        var rparam = "ga";

        Wx.showPdfReport({
            id: "SpRpTrn012A",
            pparam: data,
            rparam: rparam,
            textprint: true,
            type: "devex"
        });
    }

    me.loadDetail = function (data) {
        console.log(data.InvoiceNo);
        $http.post("sp.api/entryreturnservice/getreturnservicedetail", { InvoiceNo: data.InvoiceNo })
         .success(function (e) {

             me.grid.table = e.Table;
             me.detail1 = e.Table;
             console.log(me.detail);
             var datas = $.extend(e.Customer, e.Vehicle, e.Invoice);

             me.data = datas;
             $('#TotalPartCost').val(e.TotalPartCost);
             $('#TotalPartRetail').val(e.TotalPartRetail);
             $('#ReturnStatus').html(e.ReturnStatus);
             if (parseInt(e.Status) >= 2) {
                 me.isPrintAvailable = true;
                 me.isLoadData = true;
                 $('#btnDelete').hide();
                 console.log(parseInt(e.Status) >= 2);
             } else {
                 me.isPrintAvailable = false;
             }

             me.loadTableData(me.gridPartReturn, me.grid.table);

             //me.isInProcess = false;             
         })
        .error(function (e) {
            MsgBox(e, MSG_ERROR);
        });
    }

    me.initialize = function () {
        me.etcInfo = {};
        me.clearTable(me.gridPartReturn);
        me.isPrintAvailable = false;
        $('#btnDelete').hide();
        $('#ReturnStatus').html("OPEN RETURN");
        $('#ReturnStatus').css(
            {
                "font-size": "28px",
                "color": "red",
                "font-weight": "bold",
                "text-align": "right"
            });
    }

    me.gridPartReturn = new webix.ui({
        container: "wxPartReturn",
        view: "wxtable", css: "alternating",
        columns: [
            { id: "Check", header: { content: "masterCheckbox", contentId: "chkSelect" }, template: "{common.checkbox()}", width: 40 },
            { id: "No", header: "No", fillspace: true },
            { id: "PartNo", header: "Part No", fillspace: true },
            { id: "PartName", header: "Part Name", fillspace: true },
            { id: "CostPrice", header: "Cost Price", fillspace: true },
            { id: "RetailPrice", header: "Retail Price.", fillspace: true },
            { id: "QtyBill", header: "Bill Qty.", fillspace: true },
            { id: "QtyReturn", header: "Return Qty.", fillspace: true },
        ],
    });

    me.OnTabChange = function (e, id) {
        me.gridPartReturn.adjust();
    };

    me.start();
}


$(document).ready(function () {
    var options = {
        title: "Entry Return Service",
        xtype: "panels",
        toolbars: WxButtons,
        panels: [
            {
                name: "pnlStatus",
                items: [
                    { name: "ReturnStatus", text: "", cls: "span4", readonly: true, type: "label" },
                    {
                        type: "buttons", cls: "span4", items: [
                          { name: "btnPosting", text: "Posting", disable: true, click: "Posting()" },
                        ]
                    }
                ]
            },
            {
                name: "pnlInvoice",
                title: "",
                items: [
                        { name: "InvoiceNo", text: "Invoice No.", cls: "span4", readonly: true, type: "popup", click: "invoiceCancel()" },
                        { name: "InvoiceDate", text: "Invoice Date", cls: "span4", type: "ng-datepicker" },
                ]
            },
            {
                name: "tabpage1",
                xtype: "tabs",
                items: [
                    { name: "0", text: "Information" },
                    { name: "1", text: "Detail Part Return" },
                ]
            },
            {
                name: "CustomerInformation",
                title: "Customer Information",
                cls: "tabpage1 0",
                items: [
                   {
                       text: "Customer",
                       type: "controls",
                       items: [
                           { name: "CustomerCode", cls: "span2", placeHolder: "Customer Code", readonly: true },
                           { name: "CustomerName", cls: "span6", placeHolder: "Customer Name", readonly: true },
                       ]
                   },
                   {
                       text: "Address",
                       type: "controls",
                       items: [
                           { name: "Address1", readonly: true, cls: "span8" },
                           { name: "Address2", readonly: true, cls: "span8" },
                           { name: "Address3", readonly: true, cls: "span8" },
                           { name: "Address4", readonly: true, cls: "span8" },

                       ]
                   },
                   { name: "PhoneNo", text: "Phone No", cls: "span4", readonly: true },
                   { name: "FaxNo", text: "Fax No", cls: "span4", readonly: true },
                ]
            },
            {
                name: "Vehicle",
                title: "Vehicle Information",
                cls: "tabpage1 0",
                items: [
                   { name: "PoliceRegNo", text: "Police No", cls: "span4 full", readonly: true },
                   { name: "VIN", text: "VIN", readonly: true },
                   { name: "EngineNo", text: "Engine No.", readonly: true },
                   { name: "ServiceBookNo", text: "Service Book No.", readonly: true },
                   { name: "BasicModel", text: "Model Code", cls: "span4", readonly: true },
                   { name: "JobOrderNo", text: "SPK / Refference No.", cls: "span4", readonly: true },
                   { name: "ColorCode", text: "Color Code", cls: "span4", readonly: true },
                   { name: "JobOrderDate", text: "SPK / Refference Date", cls: "span4", type: "ng-datepicker", readonly: true },
                ]
            },
            {
                name: "EtcInfo",
                title: "",
                cls: "tabpage1 0",
                items: [
                     { name: "FPJNo", text: "FPJ No.", cls: "span4", readonly: true },
                     { name: "FPJDate", text: "FPJ Date", cls: "span4", type: "ng-datepicker", readonly: true },
                     //{ name: "JobOrderNo",  text: "Refference No.", cls: "span4", readonly: true },
                     //{ name: "JobOrderDate", text: "Refference Date", cls: "span4", type: "ng-datepicker", readonly: true },
                ]
            },
            {
                name: "PartReturn",
                title: "Part Return",
                cls: "tabpage1 1",
                items: [
                            {
                                name: "wxPartReturn",
                                type: "wxdiv"
                            },
                            { name: "TotalPartCost", text: "Total Cost", cls: "span4 number", readonly: true },
                            { name: "TotalPartRetail", text: "Total Retail", cls: "span4 number", readonly: true }
                ]
            },
        ]
    };

    Wx = new SimDms.Widget(options);
    Wx.default = {};
    Wx.render(init);

    function init(s) {
        SimDms.Angular("spEntryReturnServiceController");
    }

});