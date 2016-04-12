var totalSrvAmt = 0;
var status = 'N';
var svType = '2';

"use strict";

var isBranch;
var isUserHolding;

function spInquiryDataPartSales($scope, $http, $injector) {

    var me = $scope;

    $injector.invoke(BaseController, this, { $scope: me });

    me.BranchCode = function () {
        var lookup = Wx.blookup({
            name: "ShowBranchLookup",
            title: "No. Invoice Awal",
            manager: spManager,
            query: new breeze.EntityQuery.from("ShowBranchLookup"),
            defaultSort: "BranchCode asc",
            columns: [
                { field: "BranchCode", title: "Cabang" },
                { field: "CompanyName", title: "Nama Cabang" },
            ],
        });
        lookup.dblClick(function (data) {
            if (data != null) {
                me.data.BranchCode = data.BranchCode;
                me.data.BranchName = data.CompanyName;
                me.Apply();
            }
        });
    }

    $http.post('sp.api/Inquiry/GetTypePartPenjualan').
    success(function (data, status, headers, config) {
        me.typePary = data.data;
        me.loadTableData(me.gridTypePart, data.data);
        //console.log(me.typePary);
    });

    me.CustomerCode = function () {
        var branchId;
        if (me.data.BranchCode != null) {
            branchId = me.data.BranchCode;
        }
        else {
            branchId = me.data.BranchCodes;
        }
        var lookup = Wx.blookup({
            name: "GetCustomer",
            title: "Customer",
            manager: spManager,
            query: new breeze.EntityQuery.from("GetCustomer").withParameters({ CompanyCode: me.data.CompanyCode, BranchCode: branchId, ProfitCenter: me.data.ProfitCenter }),
            defaultSort: "CustomerCode asc",
            columns: [
                { field: "CustomerCode", title: "Customer" },
                { field: "CustomerName", title: "Nama Customer" },
                { field: "Address", title: "Alamat" },
                { field: "ProfitCenter", title: "ProfitCenter" },
            ],
        });
        lookup.dblClick(function (data) {
            if (data != null) {
                me.data.CustomerCode = data.CustomerCode;
                me.data.CustomerDesc = data.CustomerName;
                me.Apply();
            }
        });
        console.log(branchId);
    }

    me.PartSlsCode = function () {
        var branchId;
        var nameDesc;
        if (me.data.BranchCode == me.data.BranchCodes) {
            isUserHolding = true;
        }
        else {
            isUserHolding = false;
        }
        if (isUserHolding == true) {
            nameDesc = "GetEmployeeAllBranch";
        }
        else {
            nameDesc = "GetEmployeeBranch";
        }
        var lookup = Wx.blookup({
            name: nameDesc,
            title: "Customer",
            manager: spManager,
            query: new breeze.EntityQuery.from(nameDesc),
            //defaultSort: "EmployeeName asc",
            columns: [
                { field: "EmployeeID", title: "Kode Part Sales" },
                { field: "EmployeeName", title: "Nama Part Sales" },
            ],
        });
        lookup.dblClick(function (data) {
            if (data != null) {
                me.data.PartSlsCode = data.EmployeeID;
                me.data.PartSlsDesc = data.EmployeeName;
                me.Apply();
            }
        });
    }

    me.rawData = function () {
        var type = "";

        var vPart = [];
        var value = "";
        var years = moment(me.data.Periode).format('YYYY');
        $.each(me.typePary, function (key, val) {
            var arr = {
                "chkSelect": val["chkSelect"],
                "LookUpValue": key,
            }
            vPart.push(arr);
            if (arr.chkSelect == 1) {
                value += "''" + key + "'',";
            }
        });

        var values = value.substring(0, value.length - 1);
        var type = "" + values + "";

        if (type==undefined || type=="") {
            MsgBox("Type part belum ada yang dipilih", MSG_ERROR);
            return;
        }

        me.data.StartDate = moment(me.data.StartDate).format('YYYY-MM-DD');
        me.data.EndDate = moment(me.data.EndDate).format('YYYY-MM-DD');
        
        var url = "sp.api/inquiry/inquirypartsales?CompanyCode=" + me.data.CompanyCode
                    + '&BranchCode=' + me.data.BranchCode + '&InvoiceDateFrom=' + me.data.StartDate
                    + '&InvoiceDateTo=' + me.data.EndDate + '&CustomerCode=' + me.data.CustomerCode
                    + '&PartSales=' + me.data.PartSlsCode + '&SalesType=' + type;
        layout.loadAjaxLoader();
        $http.post(url)
            .success(function (data, status, headers, config) {
                if (data.success==true) {
                    me.refreshGrid1(data.grid);
                }
                else {
                    MsgBox(data.message, MSG_ERROR);
                }
                console.log(data.data);
            }).error(function (e, status, headers, config) {
                MsgBox(e, MSG_ERROR);
            });


        console.log(me.data.CompanyCode, me.data.BranchCode, me.data.StartDate, me.data.EndDate, me.data.CustomerCode, me.data.PartSlsCode, type);
    }

    me.genExcell = function () {

        var type = "";

        var vPart = [];
        var value = "";
        var years = moment(me.data.Periode).format('YYYY');
        $.each(me.typePary, function (key, val) {
            var arr = {
                "chkSelect": val["chkSelect"],
                "LookUpValue": key,
            }
            vPart.push(arr);
            if (arr.chkSelect == 1) {
                value += "'" + key + "',";
            }
        });

        var values = value.substring(0, value.length - 1);
        var type = "" + values + "";

        if (type == undefined || type == "") {
            MsgBox("Type part belum ada yang dipilih", MSG_ERROR);
            return;
        }

        var spID = "usprpt_SpRpSum025";
        var startDate = moment(me.data.StartDate).format('YYYY-MM-DD');
        var toDate = moment(me.data.EndDate).format('YYYY-MM-DD');
        var from = moment(me.data.StartDate).format('DD-MM-YYYY');
        var to = moment(me.data.EndDate).format('DD-MM-YYYY');

        var url = "sp.api/inquiry/inquirypartsalesgenexcell?";
        var params = "&CompanyCode=" + me.data.CompanyCode;
        params += "&BranchCode=" + me.data.BranchCode;
        params += "&InvoiceDateFrom=" + startDate;
        params += "&InvoiceDateTo=" + toDate;
        params += "&CustomerCode=" + me.data.CustomerCode;
        params += "&PartSales=" + me.data.PartSlsCode;
        params += "&SalesType=" + type;
        params += "&SpID=" + spID;
        params += "&From=" + from;
        params += "&To=" + to;
        url = url + params;
        window.location = url;
    }

    Wx.kgrid({
        scrollable: true,
        name: "wXgrid1",
        serverBinding: false,
        resizable: true,
        columns: [
                { field: "BranchCode", title: "Kode Cabang", width: 100 },
                { field: "LockingBy", title: "Part Sales", width: 100 },
                { field: "EmployeeName", title: "Nama Part Sales", width: 200 },
                { field: "CustomerCode", title: "Kode Pelanggan", width: 150 },
                { field: "CustomerName", title: "Nama Pelanggan", width: 250 },
                { field: "DocNo", title: "No Sales Order", width: 150 },
                { field: "DocDate", title: "Tgl Sales Order", width: 120, template: "#=  (DocDate===null) ? '' : moment(DocDate).format('DD MMM YYYY') #" },
                { field: "PickingSlipNo", title: "No Picking Slip", width: 150 },
                { field: "PickingSlipDate", title: "Tgl Picking Slip", width: 120, template: "#=  (PickingSlipDate===null) ? '' : moment(PickingSlipDate).format('DD MMM YYYY') #" },
                { field: "InvoiceNo", title: "No Invoice", width: 150 },
                { field: "InvoiceDate", title: "Tgl Invoice", width: 120, template: "#=  (InvoiceDate===null) ? '' : moment(InvoiceDate).format('DD MMM YYYY') #" },
                { field: "FPJNo", title: "No Faktur Pajak", width: 150 },
                { field: "FPJDate", title: "Tgl Faktur Pajak", width: 120, template: "#=  (FPJDate===null) ? '' : moment(FPJDate).format('DD MMM YYYY') #" },
                { field: "PartNo", title: "No Part", width: 150 },
                { field: "PartName", title: "Nama Part", width: 350 },
                { field: "QtyOrder", title: "Qty Order", width: 100 },
                { field: "QtyBill", title: "Qty Bill", width: 100 },
                { field: "RetailPrice", title: "@Unit Price", width: 120 },
                { field: "SalesAmt", title: "Total Price", width: 120 },
                { field: "DiscPct", title: "Disc(%)", width: 100 },
                { field: "DiscAmt", title: "Disc Amount", width: 120 },
                { field: "NetSalesAmt", title: "DPP", width: 75 },
                { field: "PPNAmt", title: "PPN", width: 75 },
                { field: "TotFinalSalesAmt", title: "Total Sales", width: 120 },
                { field: "CostPrice", title: "Harga Pokok", width: 120 }
        ]
    });

    me.refreshGrid1 = function (result) {
        Wx.kgrid({
            data: result,
            scrollable: true,
            name: "wXgrid1",
            serverBinding: false,
            resizable: true,
            change: grid_change,
            columns: [
                { field: "BranchCode", title: "Kode Cabang", width: 100 },
                { field: "LockingBy", title: "Part Sales", width: 100 },
                { field: "EmployeeName", title: "Nama Part Sales", width: 200 },
                { field: "CustomerCode", title: "Kode Pelanggan", width: 150 },
                { field: "CustomerName", title: "Nama Pelanggan", width: 250 },
                { field: "DocNo", title: "No Sales Order", width: 150 },
                { field: "DocDate", title: "Tgl Sales Order", width: 120, template: "#=  (DocDate===null) ? '' : moment(DocDate).format('DD MMM YYYY') #" },
                { field: "PickingSlipNo", title: "No Picking Slip", width: 150 },
                { field: "PickingSlipDate", title: "Tgl Picking Slip", width: 120, template: "#=  (PickingSlipDate===null) ? '' : moment(PickingSlipDate).format('DD MMM YYYY') #" },
                { field: "InvoiceNo", title: "No Invoice", width: 150 },
                { field: "InvoiceDate", title: "Tgl Invoice", width: 120, template: "#=  (InvoiceDate===null) ? '' : moment(InvoiceDate).format('DD MMM YYYY') #" },
                { field: "FPJNo", title: "No Faktur Pajak", width: 150 },
                { field: "FPJDate", title: "Tgl Faktur Pajak", width: 120, template: "#=  (FPJDate===null) ? '' : moment(FPJDate).format('DD MMM YYYY') #" },
                { field: "PartNo", title: "No Part", width: 150 },
                { field: "PartName", title: "Nama Part", width: 350 },
                { field: "QtyOrder", title: "Qty Order", width: 100 },
                { field: "QtyBill", title: "Qty Bill", width: 100 },
                { field: "RetailPrice", title: "@Unit Price", width: 120 },
                { field: "SalesAmt", title: "Total Price", width: 120 },
                { field: "DiscPct", title: "Disc(%)", width: 100 },
                { field: "DiscAmt", title: "Disc Amount", width: 120 },
                { field: "NetSalesAmt", title: "DPP", width: 75 },
                { field: "PPNAmt", title: "PPN", width: 75 },
                { field: "TotFinalSalesAmt", title: "Total Sales", width: 120 },
                { field: "CostPrice", title: "Harga Pokok", width: 120 }
           ]
        });

    }

    function grid_change(e) {
        e.preventDefault();
        var grid = e.sender;
        var d = grid.dataItem(this.select());
        detailInit(d);
    }

    me.gridTypePart = new webix.ui({
        container: "typePart",
        view: "wxtable", css:"alternating",
        width: 250,
        autoheight: true,
        columns: [
            { id: "chkSelect", header: { content: "masterCheckbox", contentId: "chkSelect" }, width: 60, template: "{common.checkbox()}" },

            { id: "LookUpValueName", header: "Type Part", width: 180 },
        ],

    });

    me.initialize = function () {
        me.data = {};
        var d = new Date(Date.now()).getDate();
        var m = new Date(Date.now()).getMonth();
        var y = new Date(Date.now()).getFullYear();
        me.is1 = me.is2 = me.is3 = true;
        me.data.StartDate = new Date(y, m, 1);
        me.data.EndDate = new Date(y, m, d);

        $http.get('breeze/SparePart/CurrentUserInfo').
          success(function (dl, status, headers, config) {
              me.data.CompanyCode = dl.CompanyCode;
              me.data.CompanyName = dl.CompanyGovName;
              me.data.ProfitCenter = dl.ProfitCenter;
              me.data.BranchCodes = dl.BranchCode;
          });
        $('#CompanyCode').attr('disabled', 'disabled');
        $('#CompanyName').attr('disabled', 'disabled');
        $('#BranchCode').attr('disabled', 'disabled');
        $('#BranchName').attr('disabled', 'disabled');
        $('#CustomerCode').attr('disabled', 'disabled');
        $('#CustomerDesc').attr('disabled', 'disabled');
        $('#PartSlsCode').attr('disabled', 'disabled');
        $('#PartSlsDesc').attr('disabled', 'disabled');
        me.data.BranchCode = "";
        me.data.BranchName = "";
        me.data.CustomerCode = "";
        me.data.CustomerDesc = "";
        me.data.PartSlsCode = "";
        me.data.PartSlsDesc = "";
        //me.gridDataPartSls.adjust();
        me.gridTypePart.adjust();
    }

    $('#chkBranch').on('change', function (e) {
        if ($('#chkBranch').prop('checked') == true) {
            isBranch = true;
            $('#BranchCode').removeAttr('disabled');
            $('#Branchname').removeAttr('disabled');
        } else {
            isBranch = false;
            $('#BranchCode').attr('disabled', true);
            $('#Branchname').attr('disabled', true);
            me.data.BranchCode = "";
            me.data.BranchName = "";
        }
        me.Apply();
    })

    $('#chkCust').on('change', function (e) {
        if ($('#chkCust').prop('checked') == true) {
            $('#CustomerCode').removeAttr('disabled');
            $('#CustomerDesc').removeAttr('disabled');
        } else {
            $('#CustomerCode').attr('disabled', true);
            $('#CustomerDesc').attr('disabled', true);
            me.data.CustomerCode = "";
            me.data.CustomerDesc = "";
        }
        me.Apply();
    })

    $('#chkPartSls').on('change', function (e) {
        if ($('#chkPartSls').prop('checked') == true) {
            $('#PartSlsCode').removeAttr('disabled');
            $('#PartSlsDesc').removeAttr('disabled');
        } else {
            $('#PartSlsCode').attr('disabled', true);
            $('#PartSlsDesc').attr('disabled', true);
            me.data.PartSlsCode = "";
            me.data.PartSlsDesc = "";
        }
        me.Apply();
    })

    me.start();
}

$(document).ready(function () {
    var options = {
        title: "Inquiry Data Part Sales",
        xtype: "panels",
        toolbars: [
            { name: "btnRawData", text: "Raw Data", cls: "btn btn-primary", icon: "icon-print", click: "rawData()" },
            { name: "btnExcell", text: "Generate Excell", cls: "btn btn-primary", icon: "icon-print", click: "genExcell()" },
        ],
        panels: [

            {
                name: "DataPartSales",
                title: "Filter",
                items: [
                    { name: "BranchCodes", model: "data.BranchCodes", text: "Kode Cabang", cls: "span4 full", show: false },
                    { name: "ProfitCenter", model: "data.ProfitCenter", text: "Profit Center", cls: "span4 full", show: false },
                    {
                        text: "Dealer",
                        type: "controls",
                        cls: "span8",
                        items: [
                            { name: "CompanyCode", model: "data.CompanyCode", cls: "span2", placeHolder: " ", readonly: true },
                            { name: "CompanyName", model: "data.CompanyName", cls: "span5", placeHolder: " ", readonly: true }
                        ]
                    },
                    { name: "chkBranch", model: "data.chkBranch", text: "Cabang", cls: "span1", type: "ng-check" },
                    {
                        text: "",
                        type: "controls",
                        items: [

                            { name: "BranchCode", model: "data.BranchCode", cls: "span2", placeHolder: " ", readonly: true, type: "popup", click: "BranchCode()", disable: "!data.chkBranch" },
                            { name: "BranchName", model: "data.BranchName", cls: "span5", placeHolder: " ", readonly: true, disable: "!data.chkBranch" }
                        ]
                    },
                    {
                        text: "Periode",
                        type: "controls",
                        cls: "span8 full",
                        items: [

                            { name: "StartDate", cls: "span2", placeHolder: "", type: "ng-datepicker" },
                            { type: "label", text: "s.d", cls: "span1 mylabel" },
                            { name: "EndDate", cls: "span2", placeHolder: "", type: "ng-datepicker" },
                        ]
                    },
                    { name: "chkCust", model: "data.chkCust", text: "Customer", cls: "span1", type: "ng-check" },
                    {
                        text: "",
                        type: "controls",
                        items: [

                            { name: "CustomerCode", model: "data.CustomerCode", cls: "span2", placeHolder: " ", readonly: true, type: "popup", click: "CustomerCode()", disable: "!data.chkCust" },
                            { name: "CustomerDesc", model: "data.CustomerDesc", cls: "span5", placeHolder: " ", readonly: true, disable: "!data.chkCust" }
                        ]
                    },
                    { name: "chkPartSls", model: "data.chkPartSls", text: "Part Sales", cls: "span1", type: "ng-check" },
                    {
                        text: "",
                        type: "controls",
                        items: [

                            { name: "PartSlsCode", model: "data.PartSlsCode", cls: "span2", placeHolder: " ", readonly: true, type: "popup", click: "PartSlsCode()", disable: "!data.chkPartSls" },
                            { name: "PartSlsDesc", model: "data.PartSlsDesc", cls: "span5", placeHolder: " ", readonly: true, disable: "!data.chkPartSls" }
                        ]
                    },

                    { type: "hr" },
                ]
            },
            {
                name: "typePart",
                title: "Type Part",
                xtype: "wxtable"
            },

            {
                xtype: "tabs",
                name: "tabpage1",
                items: [
                    { name: "tabList", text: "Data Part Sales", cls: "active" },
                ],
            },

            {
                name: "wXgrid1",
                xtype: "k-grid"
            },

        ]
    };

    Wx = new SimDms.Widget(options);
    Wx.default = {};
    Wx.render(init);

    function init(s) {
        $(".mylabel").attr("style", "padding:9px 9px 0px 5px");
        SimDms.Angular("spInquiryDataPartSales");
    }

});