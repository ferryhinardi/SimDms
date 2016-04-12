var totalSrvAmt = 0;
var status = 'N';
var svType = '2';
"use strict";

function spInquiryDataSparepart($scope, $http, $injector) {

    var me = $scope;

    $injector.invoke(BaseController, this, { $scope: me });
    var codeID = "INQSPR";
    $http.post('gn.api/combo/LoadLookup?CodeID=' + codeID).
    success(function (data, status, headers, config) {
        me.PrintReport = data;
        //console.log(data);
    });

    $http.post('sp.api/Inquiry/GetTypePart').
    success(function (data, status, headers, config) {
        me.typePary = data.data;
        me.loadTableData(me.gridTypePart, data.data);
        //console.log(me.typePary);
    });


    me.Area = function () {
        var lookup = Wx.blookup({
            name: "GetInquirySpBtn",
            title: "Area",
            manager: spManager,
            query: new breeze.EntityQuery.from("GetInquirySpBtn").withParameters({ Area: me.data.AreaDesc, Dealer: "", Outlet: "", Detail: "1" }),
            defaultSort: "GroupNo asc",
            columns: [
                { field: "Area", title: "Area" },
            ],
        });
        lookup.dblClick(function (data) {
            if (data != null) {
                me.data.AreaDesc = data.Area;
                me.Apply();
            }
        });
        //console.log(me.data.AreaDesc, me.data.DealerDesc, me.data.OutletDesc, "2")
    }

    me.Dealer = function () {
        var lookup = Wx.blookup({
            name: "GetInquirySpBtn",
            title: "Dealer",
            manager: spManager,
            query: new breeze.EntityQuery.from("GetInquirySpBtn").withParameters({ Area: me.data.AreaDesc, Dealer: "", Outlet: "", Detail: "2" }),
            defaultSort: "GroupNo asc",
            columns: [
                { field: "DealerCode", title: "Branch ID" },
                { field: "DealerName", title: "Branch Name" },
            ]
        });
        lookup.dblClick(function (data) {
            if (data != null) {
                me.data.DealerDesc = data.DealerName;
                me.data.DealerCode = data.DealerCode;
                me.Apply();
            }
        });
    };

    me.Outlet = function () {
        var lookup = Wx.blookup({
            name: "GetInquirySpBtn",
            title: "Outlet",
            manager: spManager,
            query: new breeze.EntityQuery.from("GetInquirySpBtn").withParameters({ Area: me.data.AreaDesc, Dealer: me.data.DealerCode, Outlet: "", Detail: "3" }),
            defaultSort: "GroupNo asc",
            columns: [
                { field: "OutletCode", title: "Outlet ID" },
                { field: "OutletName", title: "Outlet Name" },
            ]
        });
        lookup.dblClick(function (data) {
            if (data != null) {
                me.data.OutletDesc = data.OutletName;
                me.data.OutletCode = data.OutletCode;
                me.Apply();
            }
        });
    };

    me.rawData = function () {
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
                //"''0'',''1'',''2'',''3'',''4'',''5''";
                value += "''" + key + "'',";
            }
        });

        var values = value.substring(0, value.length - 1);
        var type = "" + values + "";

        if (type == undefined || type == "") {
            MsgBox("Type part belum ada yang dipilih", MSG_ERROR);
            return;
        }

        var s1 = type;

        var url = "sp.api/inquiry/InquerySparepart?CompanyCode=" + me.data.DealerCode
                    + '&BranchCode=' + me.data.OutletCode + '&Area=' + me.data.AreaDesc
                    + '&Year=' + years + '&S1=' + s1;
        layout.loadAjaxLoader();
        $http.post(url)
            .success(function (data, status, headers, config) {
                if (data.success == true) {
                    me.refreshGrid1(data.grid);
                }
                else {
                    MsgBox(data.message, MSG_ERROR);
                }
                console.log(data.data);
            }).error(function (e, status, headers, config) {
                MsgBox(e, MSG_ERROR);
            });

        console.log(s1);
    }

    me.genExcell = function () {

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
                //"''0'',''1'',''2'',''3'',''4'',''5''";
                value += "''" + key + "'',";
            }
        });

        var values = value.substring(0, value.length - 1);
        var type = "" + values + "";

        if (type == undefined || type=="") {
            MsgBox("Type part belum ada yang dipilih", MSG_ERROR);
            return;
        }
        
        var s1 = type;
        var spID = "usprpt_SpRpSum024";
        var startDate = moment(me.data.StartDate).format('YYYY-MM-DD');
        var toDate = moment(me.data.EndDate).format('YYYY-MM-DD');
        var from = moment(me.data.StartDate).format('DD-MM-YYYY');
        var to = moment(me.data.EndDate).format('DD-MM-YYYY');

        var url = "sp.api/inquiry/inquirysparepartgenexcell?";
        var params = "&Area=" + me.data.AreaDesc;
        params += "&CompanyCode=" + me.data.DealerCode;
        params += "&BranchCode=" + me.data.OutletCode;
        params += "&Year=" + years;
        params += "&S1=" + s1;
        params += "&SpID=" + spID;
        url = url + params;
        window.location = url;

        console.log(me.data.DealerCode, me.data.OutletCode, me.data.AreaDesc, years, s1, type);
        
    }


    Wx.kgrid({
        scrollable: true,
        name: "wXgrid1",
        serverBinding: false,
        resizable: true,
        columns: [
                { field: "CompanyCode", title: "Dealer", width: 100 },
                { field: "BranchCode", title: "Outlet", width: 100 },
                { field: "PeriodYear", title: "Year", width: 200 },
                { field: "PeriodMonth", title: "Month", width: 150 },
                { field: "PenjualanKotor", title: "Penjualan Kotor", width: 250 },
                { field: "PenjualanBersih", title: "Penjualan Bersih", width: 150 },
                { field: "Penjualan3S2S", title: "Penjualan 3S + 2S", width: 120 },
                { field: "PenjualanPartShop", title: "Penjualan ke Part Shop", width: 150 },
                { field: "JumlahJaringan", title: "Penjualan Ke Lain-lain", width: 120, },
                { field: "HargaPokok", title: "Harga Pokok", width: 150 },
                { field: "PenerimaanPembelian", title: "Penerimaan Pembelian", width: 120, },
                { field: "NilaiStock", title: "Nilai Stock", width: 150 },
                { field: "ITO", title: "ITO", width: 120, },
                { field: "AVGITO", title: "ITO (AVG)", width: 150 },
                { field: "Ratio", title: "Ratio", width: 350 },
                { field: "RatioSuzuki", title: "Ration Suzuki", width: 100 },
                { field: "DemandLine", title: "Demand Line", width: 100 },
                { field: "DemandQuantity", title: "Demand Qty", width: 120 },
                { field: "SupplyLine", title: "Supply Line", width: 120 },
                { field: "SupplyQuantity", title: "Supply Qty", width: 100 },
                { field: "SupplyNilai", title: "Suppliy Nilai", width: 120 },
                { field: "ServiceRatioLine", title: "Service Ratio Line", width: 75 },
                { field: "ServiceRatioQuantity", title: "Service Ratio Qty", width: 75 },
                { field: "ServiceRatioNilai", title: "Service Ratio Nilai", width: 120 },
                { field: "DataStockMC4", title: "Stock MC4", width: 120 },
                { field: "DataStockMC5", title: "Stock MC5", width: 120 },
                { field: "SlowMoving", title: "Slow Moving", width: 120 }
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
                { field: "CompanyCode", title: "Dealer", width: 100 },
                { field: "BranchCode", title: "Outlet", width: 100 },
                { field: "PeriodYear", title: "Year", width: 200 },
                { field: "PeriodMonth", title: "Month", width: 150 },
                { field: "PenjualanKotor", title: "Penjualan Kotor", width: 250 },
                { field: "PenjualanBersih", title: "Penjualan Bersih", width: 150 },
                { field: "Penjualan3S2S", title: "Penjualan 3S + 2S", width: 120 },
                { field: "PenjualanPartShop", title: "Penjualan ke Part Shop", width: 150 },
                { field: "JumlahJaringan", title: "Penjualan Ke Lain-lain", width: 120, },
                { field: "HargaPokok", title: "Harga Pokok", width: 150 },
                { field: "PenerimaanPembelian", title: "Penerimaan Pembelian", width: 120, },
                { field: "NilaiStock", title: "Nilai Stock", width: 150 },
                { field: "ITO", title: "ITO", width: 120, },
                { field: "AVGITO", title: "ITO (AVG)", width: 150 },
                { field: "Ratio", title: "Ratio", width: 350 },
                { field: "RatioSuzuki", title: "Ration Suzuki", width: 100 },
                { field: "DemandLine", title: "Demand Line", width: 100 },
                { field: "DemandQuantity", title: "Demand Qty", width: 120 },
                { field: "SupplyLine", title: "Supply Line", width: 120 },
                { field: "SupplyQuantity", title: "Supply Qty", width: 100 },
                { field: "SupplyNilai", title: "Suppliy Nilai", width: 120 },
                { field: "ServiceRatioLine", title: "Service Ratio Line", width: 75 },
                { field: "ServiceRatioQuantity", title: "Service Ratio Qty", width: 75 },
                { field: "ServiceRatioNilai", title: "Service Ratio Nilai", width: 120 },
                { field: "DataStockMC4", title: "Stock MC4", width: 120 },
                { field: "DataStockMC5", title: "Stock MC5", width: 120 },
                { field: "SlowMoving", title: "Slow Moving", width: 120 }
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
        me.typePary = {};
        var d = new Date(Date.now()).getDate();
        var m = new Date(Date.now()).getMonth();
        var y = new Date(Date.now()).getFullYear();
        me.is1 = me.is2 = me.is3 = true;
        me.data.Periode = me.now();
        me.data.PrintReport = "0";

        $http.get('breeze/SparePart/CurrentUserInfo').
          success(function (dl, status, headers, config) {
              me.data.CompanyCode = dl.CompanyCode;
              me.data.CompanyName = dl.CompanyGovName;
              me.data.ProfitCenter = dl.ProfitCenter;
              //me.data.BranchCode = dl.BranchCode;
          });

        me.gridTypePart.adjust();
        
        me.data.BranchCode = "";
        me.data.AreaDesc = "";
        me.data.DealerDesc = "";
        me.data.OutletDesc = "";
        me.data.AreaCode = "";
        me.data.DealerCode = "";
        me.data.OutletCode = "";
    }

    $('#chkArea').on('change', function (e) {
        if ($('#chkArea').prop('checked') == true) {
            isBranch = true;
            $('#AreaDesc').removeAttr('disabled');
        } else {
            isBranch = false;
            $('#AreaDesc').attr('disabled', true);
            me.data.AreaDesc = "";
        }
        me.Apply();
    })
    $('#chkDealer').on('change', function (e) {
        if ($('#chkDealer').prop('checked') == true) {
            isBranch = true;
            $('#DealerDesc').removeAttr('disabled');
            $('#DealerCode').removeAttr('disabled');
        } else {
            isBranch = false;
            $('#DealerDesc').attr('disabled', true);
            $('#DealerCode').attr('disabled', true);
            me.data.DealerDesc = "";
            me.data.DealerCode = "";
        }
        me.Apply();
    })
    $('#chkOutlet').on('change', function (e) {
        if ($('#chkOutlet').prop('checked') == true) {
            isBranch = true;
            $('#OutletDesc').removeAttr('disabled');
            $('#OutletCode').removeAttr('disabled');
        } else {
            isBranch = false;
            $('#OutletDesc').attr('disabled', true);
            $('#OutletCode').attr('disabled', true);
            me.data.OutletDesc = "";
            me.data.OutletCode = "";
        }
        me.Apply();
    })
    
    me.start();

}

$(document).ready(function () {
    //var options = {
    //    title: "test",
    //    xtype: "iframe",
    //    url: "/assets/js/app/sp/inquiry/inqsparepart.js"
    //};  

    var options = {
        title: "Inquiry Data Sparepart",
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
                    { name: "BranchCode", model: "data.BranchCodes", text: "Kode Cabang", cls: "span4 full", show: false },
                    { name: "ProfitCenter", model: "data.ProfitCenter", text: "Profit Center", cls: "span4 full", show: false },

                    { name: "Periode", cls: "span3 full", placeHolder: "", text: "Periode", type: "ng-datepicker" },
                    {
                        text: "Area",
                        type: "controls",
                        cls: "span8 full",
                        items: [
                            { name: "chkArea", model: "data.chkArea", text: "Area", cls: "span1", type: "ng-check" },
                            { name: "AreaDesc", model: "data.AreaDesc", cls: "span5", placeHolder: " ", readonly: true, type: "popup", click: "Area()", disable: "!data.chkArea" },
                        ]
                    },

                    {
                        text: "Dealer",
                        type: "controls",
                        cls: "span8 full",
                        items: [
                            { name: "chkDealer", model: "data.chkDealer", text: "Customer", cls: "span1", type: "ng-check" },
                            { name: "DealerDesc", model: "data.DealerDesc", cls: "span5", placeHolder: " ", readonly: true, type: "popup", click: "Dealer()", disable: "!data.chkDealer" },
                            { name: "DealerCode", model: "data.DealerCode", cls: "span3", placeHolder: " ", readonly: true, show: false, disable: "!data.chkDealer" },
                        ]
                    },

                    {
                        text: "Outlet",
                        type: "controls",
                        cls: "span8 full",
                        items: [
                            { name: "chkOutlet", model: "data.chkOutlet", text: "Part Sales", cls: "span1", type: "ng-check" },
                            { name: "OutletDesc", model: "data.OutletDesc", cls: "span5", placeHolder: " ", readonly: true, type: "popup", click: "Outlet()", disable: "!data.chkOutlet" },
                            { name: "OutletCode", model: "data.OutletCode", cls: "span3", placeHolder: " ", readonly: true, show: false, disable: "!data.chkOutlet" },
                        ]
                    },
                ]
            },

            {
                name: "PrintArea",
                title: "Print",
                cls: "span6 full",
                items: [
                    { name: "PrintReport", opt_text: "", cls: "span4", type: "select2", text: "Print", datasource: "PrintReport" },
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
                    { name: "tabList", text: "Data Inq Sparepart", cls: "active" },
                ],

            },

            {
                name: "wXgrid1",
                xtype: "k-grid"
            },

        ],


    };


    Wx = new SimDms.Widget(options);
    Wx.default = {};
    Wx.render(init);

    function init(s) {
        $(".mylabel").attr("style", "padding:9px 9px 0px 5px");
        SimDms.Angular("spInquiryDataSparepart");
    }

});