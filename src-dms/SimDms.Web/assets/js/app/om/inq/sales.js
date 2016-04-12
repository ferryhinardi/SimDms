var totalSrvAmt = 0;
var status = 'N';
var svType = '2';
var pType = "4W";
"use strict";

function omInquirySalesController($scope, $http, $injector) {

    var me = $scope;

    
    $injector.invoke(BaseController, this, { $scope: me });

    me.initialize = function () {

        me.data.ProductType = '0';
        me.data.DealerOption = '3';
        me.is1 =  me.is2 = me.is3 = true;
        me.is4 = me.is5 = me.is6 = me.is7 = me.is8 = me.is9 = me.is10 = false;

        me.isPivot = false

        $('#isArea').attr('disabled', 'disabled');
        $('#isDealer').attr('disabled', 'disabled');
        $('#isArea').prop('checked', true);
        me.data.isArea = true;
        $('#isDealer').prop('checked', true);
        me.data.isDealer = true;

        $http.post('om.api/InquirySales/GetProdType').
          success(function (e) {
              pType = e.pType
              me.SHSow = pType == '4W' ? true : false
              console.log(me.SHSow);
          });

        $http.post('om.api/ReportSales/Periode').
          success(function (e) {
              me.data.StartDate = e.DateFrom;
              me.data.EndDate = e.DateTo;
          });
        $http.post('om.api/ReportSales/CheckArea').
          success(function (e) {
              me.data.Area = e.Area;
              me.data.Dealer = e.Dealer;
          });

        me.data.Outlet = "<----Select All---->";
        me.data.BranchHead = "<----Select All---->";
        me.data.SalesHead = "<----Select All---->";
        me.data.SalesCoordinator = "<----Select All---->";
        me.data.Wiraniaga = "<----Select All---->";

        me.data.TestDate = me.now();

        me.gridHasil.clearAll();
        me.gridHasil.adjust();
        
    }

    me.ProductType = [
       { "value": '0', "text": 'Sales Report' },
       { "value": '1', "text": 'Sales Trand Report' },
       { "value": '2', "text": 'Productivity Trand Report' },
       { "value": '3', "text": 'Comparasion : Wolesale vs Retail' },
       { "value": '4', "text": 'Comparasion : Actual vs Target' },
       { "value": '5', "text": 'Comparasion : Month to Month vs Year to Year' },
    ];

    //$("[name = 'StartDate']").on('change', function () {
    //    console.log(moment(me.data.StartDate).format('YYYY-MM-DD HH:mm:ss'));
    //});


    
    $("[name = 'ProductType']").on('change', function () {
        var Type = "";
        Type = $('[name="ProductType"]').val();
        
        if (Type == 0) {
            me.is1 = me.is2 = me.is3 = true;
            me.is4 = me.is5 = me.is6 = me.is7 = me.is8 = me.is9 = me.is10 = false;
            $('#isOutlet').removeAttr('disabled');
            $('#isBrachHead').removeAttr('disabled');
            $('#isSalesHead').removeAttr('disabled');
            $('#isOutlet').prop('checked', false);
            me.data.isOutlet = false;
            $('#isBrachHead').prop('checked', false);
            me.data.isBrachHead = false;
            $('#isSalesHead').prop('checked', false);
            me.data.isSalesHead = false;
        }
        if (Type == 1) {
            me.is1 = me.is2 = me.is3 = me.is4 = me.is5 = true;
            me.is6 = me.is7 = me.is8 = me.is9 = false;
            $('#isOutlet').removeAttr('disabled');
            $('#isBrachHead').removeAttr('disabled');
            $('#isSalesHead').removeAttr('disabled');
            $('#isOutlet').prop('checked', false);
            me.data.isOutlet = false;
            $('#isBrachHead').prop('checked', false);
            me.data.isBrachHead = false;
            $('#isSalesHead').prop('checked', false);
            me.data.isSalesHead = false;
        }
        if (Type == 2) {
            me.is3 = me.is4 = me.is5 = me.is6 = me.is7 = me.is8 = me.is9 = true;
            me.is1 = me.is2 = me.is10 = false;
            $('#isOutlet').attr('disabled', 'disabled');
            $('#isBrachHead').attr('disabled', 'disabled');
            $('#isSalesHead').attr('disabled', 'disabled');
            $('#isOutlet').prop('checked', true);
            me.data.isOutlet = true;
            $('#isBrachHead').prop('checked', true);
            me.data.isBrachHead = true;
            $('#isSalesHead').prop('checked', true);
            me.data.isSalesHead = true;
        }
        if (Type == 3 || Type == 4 || Type == 5) {
            me.is1 = me.is2 = me.is3 = me.is10 = true;
            me.is4 = me.is5 = me.is6 = me.is7 = me.is8 = me.is9 = false;
            $('#isOutlet').removeAttr('disabled');
            $('#isBrachHead').removeAttr('disabled');
            $('#isSalesHead').removeAttr('disabled');
            $('#isOutlet').prop('checked', false);
            me.data.isOutlet = false;
            $('#isBrachHead').prop('checked', false);
            me.data.isBrachHead = false;
            $('#isSalesHead').prop('checked', false);
            me.data.isSalesHead = false;
        }
    });

    me.gridHasil = new webix.ui({
        container: "wxDetail",
        view: "wxtable", css: "alternating",
        scrollX: true,
        scrollY: true,
        columns: [
            { id: "CompanyName", header: "Dealer", width: 100 },
            { id: "BranchName", header: "Outlet", width: 100 },
            { id: "BranchHeadName", header: "Branch Head", width: 100 },
            { id: "SalesHeadName", header: "Sales Head", width: 100 },
            //{ id: "SalesCoordinator", header: "Sales Coordinator", width: 100 },
            { id: "SalesmanName", header: "Wiraniaga", width: 100 },
            { id: "Grade", header: "Grade", width: 100 },
            { id: "ModelCatagory", header: "Category", width: 100 },
            { id: "SalesModelCode", header: "Sales Model Code", width: 200 },
            { id: "SalesModelYear", header: "Warna", Year: 100 },
            { id: "SalesModelDesc", header: "Sales Model Desc", width: 200 },
            { id: "FakturPolisiDesc", header: "Faktur Polisi Desc", width: 200 },
            { id: "MarketModelDesc", header: "Market Model Desc", width: 200 },
            { id: "GroupMarketModel", header: "Group Market Model", width: 100 },
            { id: "ColumnMarketModel", header: "ColumnMarketModel", width: 100 },
            { id: "ColourCode", header: "Colour Code", width: 100 },
            { id: "ColourName", header: "Colour Name", width: 100 },
            { id: "SoNo", header: "SoNo", width: 100 },
            { id: "InvoiceNo", header: "Invoice No", width: 100 },
            { id: "InvoiceDate", header: "Invoice Date", width: 100 },
            { id: "FakturPolisNo", header: "Faktur Polis No", width: 100 },
            { id: "FakturPolisDate", header: "Faktur Polis Date", width: 100 },

        ]
    });

    me.ExcelData = function () {
        if (me.data.DealerOption == '0') { me.data.DealerOption = "SALES" }
        if (me.data.DealerOption == '1') { me.data.DealerOption = "WHOLESALE" }
        if (me.data.DealerOption == '2') { me.data.DealerOption = "RETAIL" }
        if (me.data.DealerOption == '3') { me.data.DealerOption = "FPOL" }
        if (me.data.DealerOption == '4') { me.data.DealerOption = "REGPOL" }

        if (me.data.Outlet == "<----Select All---->") { me.data.Outlet = "" } else { me.data.Outlet }
        if (me.data.BranchHead == "<----Select All---->") { me.data.BranchHead = "" } else { me.data.BranchHead }
        if (me.data.SalesHead == "<----Select All---->") { me.data.SalesHead = "" } else { me.data.SalesHead }
        if (me.data.SalesCoordinator == "<----Select All---->") { me.data.SalesCoordinator = "" } else { me.data.SalesCoordinator }
        if (me.data.Wiraniaga == "<----Select All---->") { me.data.Wiraniaga = "" } else { me.data.Wiraniaga }

        var url = "om.api/Inquiry/PrintInqSales?";
        var params = "&StartDate=" + $('[name="StartDate"]').val();
        params += "&EndDate=" + $('[name="EndDate"]').val();
        params += "&Area=" + $('[name="Area"]').val();
        params += "&Dealer=" + $('[name="DealerID"]').val();
        params += "&Outlet=" + $('[name="OutletID"]').val();
        params += "&BranchHead=" + $('[name="BranchHead"]').val();
        params += "&SalesHead=" + $('[name="SalesHead"]').val();
        params += "&SalesCoordinator=" + $('[name="SalesCoordinator"]').val();
        params += "&salesman=" + $('[name="Wiraniaga"]').val();
        params += "&SalesType=" + $('[name="DealerOption"]').val();
        params += "&isOutlet=" + $('[name="isOutlet"]').val();
        url = url + params;
        window.location = url;
    }

    me.RowData = function () {
        var SalesType = ""
        
        if (me.data.DealerOption == '0') { SalesType = "SALES" }
        if (me.data.DealerOption == '1') { SalesType = "WHOLESALE" }
        if (me.data.DealerOption == '2') { SalesType = "RETAIL" }
        if (me.data.DealerOption == '3') { SalesType = "FPOL" }
        if (me.data.DealerOption == '4') { SalesType = "REGPOL" }
        
        console.log(me.data.Area);
        if (me.data.Area == '<----Select All---->') { me.data.Area = ''; }
        if (me.data.Dealer == '<----Select All---->') { me.data.Dealer = '' }
        if (me.data.Outlet == "<----Select All---->") { me.data.Outlet = "" }else{ me.data.Outlet }
        if (me.data.BranchHead == "<----Select All---->") { me.data.BranchHead = "" } else { me.data.BranchHead }
        if (me.data.SalesHead == "<----Select All---->") { me.data.SalesHead = "" } else { me.data.SalesHead }
        if (me.data.SalesCoordinator == "<----Select All---->") { me.data.SalesCoordinator = "" } else { me.data.SalesCoordinator }
        if (me.data.Wiraniaga == "<----Select All---->") { me.data.Wiraniaga = "" } else { me.data.Wiraniaga }
        
        var date = new Date(me.data.StartDate).getDate();
        var month = new Date(me.data.StartDate).getMonth() + 1;
        var year = new Date(me.data.StartDate).getFullYear();

        if (me.data.ProductType == '5') {
            if (month - 1 == 0) { month = 12; year = year - 1 } else { month = month - 1}
                me.data.StartDate = year + '/' + month + '/' + date;                
        }
        else if (me.data.ProductType == '4') {
            year = year - 1;
            me.data.StartDate = year + '/' + month + '/' + date;
        } else {
            me.data.StartDate = year + '/' + month + '/' + date;
        }
                
        $http.post('om.api/InquirySales/GetReportOmRpSalRgs039Web?startDate=' + me.data.StartDate + '&endDate=' + moment(me.data.EndDate).format('YYYY/MM/DD')
                    + '&area=' + me.data.Area + '&dealer=' + me.data.Dealer + '&outlet=' + me.data.Outlet
                    + '&branchHead=' + me.data.BranchHead + '&salesHead=' + me.data.SalesHead + '&salesCoordinator=' + me.data.SalesCoordinator
                    + '&salesman=' + me.data.Wiraniaga + '&salesType=' + SalesType)
                    .success(function (e) {
                        if (e.success) {
                            if (e.grid != "") {
                                me.isPivot = false;
                                me.HidePivot()
                                me.loadTableData(me.gridHasil, e.grid);
                            } else {
                                MsgBox("Tidak Ada Data", MSG_ERROR);
                            }
                        } else {
                            MsgBox(e.message, MSG_ERROR);
                        }
                    })
                    .error(function (e) {
                        MsgBox('Terjadi Kesalahan, Hubungi SDMS Support', MSG_ERROR);
                    });
    }

    me.HidePivot = function () {
        var tbl = document.getElementById("wxpivotgrid");
        var chld = tbl.firstElementChild;

        if (chld != null) {
            console.log(tbl, chld);
            tbl.innerHTML = "";
        }
    }

    me.PivotData = function () {
        var SalesType = ""
        if (me.data.DealerOption == '0') { SalesType = "SALES" }
        if (me.data.DealerOption == '1') { SalesType = "WHOLESALE" }
        if (me.data.DealerOption == '2') { SalesType = "RETAIL" }
        if (me.data.DealerOption == '3') { SalesType = "FPOL" }
        if (me.data.DealerOption == '4') { SalesType = "REGPOL" }

        if (me.data.Outlet == "<----Select All---->") { me.data.Outlet = "" } else { me.data.Outlet }
        if (me.data.BranchHead == "<----Select All---->") { me.data.BranchHead = "" } else { me.data.BranchHead }
        if (me.data.SalesHead == "<----Select All---->") { me.data.SalesHead = "" } else { me.data.SalesHead }
        if (me.data.SalesCoordinator == "<----Select All---->") { me.data.SalesCoordinator = "" } else { me.data.SalesCoordinator }
        if (me.data.Wiraniaga == "<----Select All---->") { me.data.Wiraniaga = "" } else { me.data.Wiraniaga }

        var date = new Date(me.data.StartDate).getDate();
        var month = new Date(me.data.StartDate).getMonth() + 1;
        var year = new Date(me.data.StartDate).getFullYear();

        if (me.data.ProductType == '5') {
            if (month - 1 == 0) { month = 12; year = year - 1 } else { month = month - 1 }
            me.data.StartDate = year + '/' + month + '/' + date;
        }
        else if (me.data.ProductType == '4') {
            year = year - 1;
            me.data.StartDate = year + '/' + month + '/' + date;
        } else {
            me.data.StartDate = year + '/' + month + '/' + date;
        }

        $http.post('om.api/InquirySales/GetOmRpSalRgs039PivotWeb?startDate=' + me.data.StartDate + '&endDate=' + moment(me.data.EndDate).format('YYYY/MM/DD')
                    + '&area=' + me.data.Area + '&dealer=' + me.data.Dealer + '&outlet=' + me.data.Outlet
                    + '&branchHead=' + me.data.BranchHead + '&salesHead=' + me.data.SalesHead + '&salesCoordinator=' + me.data.SalesCoordinator
                    + '&salesman=' + me.data.Wiraniaga + '&salesType=' + SalesType)
                    .success(function (e) {
                        if (e.success) {
                            if (e.grid != "") {
                                //me.loadTableData(me.grid, e.grid);
                                me.isPivot = true
                                console.log(me.isPivot)
                                window.pivotdata = e.grid;
                                $("#wxpivotgrid").pivotUI(window.pivotdata, {
                                    //derivedAttributes: {
                                    //    "Company Name": function(mp){
                                    //        return mp.CompanyName;
                                    //    },
                                    //},
                                    rows: ["BranchHeadName", "SalesHeadName", "SalesmanName", "Grade", "ModelCatagory", "MarketModel", "GroupMarketModel", "ColumnMarketModel", "ColourName" ],
                                    cols: ["Year", "Month"],
                                    //aggregatorName: "Integer Sum",
                                    //vals: ["Service Amount"],
                                    hiddenAttributes: ["CompanyCode", "BranchCode", "BranchHeadID", "SalesHeadID","SalesCoordinatorID", "SalesCoordinatorName", "SalesmanID", "SalesType", "SalesModelCode",
                                                        "SalesModelYear", "SalesModelDesc", "FakturPolisiNo", "FakturPolisiDesc", "FakturPolisiDate", "ColourCode", "InvoiceDate", "SONo", "InvoiceNo", "COGS", "DPP", "DPPAccs","COGSAccs","Margin"],
                                });
                            } else {
                                MsgBox("Tidak Ada Data", MSG_ERROR);
                            }
                        } else {
                            MsgBox(e.message, MSG_ERROR);
                        }
                    })
                    .error(function (e) {
                        MsgBox('Terjadi Kesalahan, Hubungi SDMS Support', MSG_ERROR);
                    });
    }

    me.Area = function () {
        var lookup = Wx.blookup({
            name: "GetInquiryBtn",
            title: "Area",
            manager: spSalesManager,
            query: new breeze.EntityQuery().from("GetInquiryBtn").withParameters({ Area: "", Dealer: me.data.Dealer, Outlet: "", Detail: "1" }),
            defaultSort: "GroupNo asc",
            columns: [
                { field: "Area", title: "Area" },
            ]
        });
        lookup.dblClick(function (data) {
            if (data != null) {
                me.data.Area = data.Area;
                me.Apply();
            }
        });
    };

    me.Dealer = function () {
        var lookup = Wx.blookup({
            name: "GetInquiryBtn",
            title: "Dealer",
            manager: spSalesManager,
            query: new breeze.EntityQuery().from("GetInquiryBtn").withParameters({ Area: me.data.Area, Dealer: me.data.Dealer, Outlet: "", Detail: "2" }),
            defaultSort: "GroupNo asc",
            columns: [
                { field: "DealerCode", title: "Branch ID" },
                { field: "DealerName", title: "Branch Name" },
            ]
        });
        lookup.dblClick(function (data) {
            if (data != null) {
                me.data.Dealer = data.DealerCode;
                me.data.DealerName = data.DealerName;
                me.Apply();
            }
        });
    };

    me.Outlet = function () {
        var lookup = Wx.blookup({
            name: "GetInquiryBtn",
            title: "Outlet",
            manager: spSalesManager,
            query: new breeze.EntityQuery().from("GetInquiryBtn").withParameters({ Area: me.data.Area, Dealer: me.data.Dealer, Outlet: "", Detail: "3" }),
            defaultSort: "GroupNo asc",
            columns: [
                { field: "OutletCode", title: "Outlet ID" },
                { field: "OutletName", title: "Outlet Name" },
            ]
        });
        lookup.dblClick(function (data) {
            if (data != null) {
                me.data.Outlet = data.OutletCode;
                me.data.OutletName = data.OutletName;
                me.Apply();
            }
        });
    };

    me.BranchHead = function () {
        var salesType = ""
        if(me.data.DealerOption == '0'){salesType = "SALES"}
        if(me.data.DealerOption == '1'){salesType = "WHOLESALE"}
        if(me.data.DealerOption == '2'){salesType = "RETAIL"}
        if(me.data.DealerOption == '3'){salesType = "FPOL"}
        if(me.data.DealerOption == '4'){salesType = "REGPOL"}

        var lookup = Wx.blookup({
            name: "GetInquirySalesLookUpBtn",
            title: "Branch Head",
            manager: spSalesManager,
            query: new breeze.EntityQuery().from("GetInquirySalesLookUpBtn")
                .withParameters({ startDate: moment(me.data.StartDate).format('YYYY/MM/DD'), endDate: moment(me.data.EndDate).format('YYYY/MM/DD'), area: me.data.Area, dealer: me.data.Dealer, outlet: me.data.Outlet, branchHead: "", salesHead: "", salesCoordinator: "", salesman: "", detail: "4", salesType: salesType }),
            defaultSort: "GroupNo asc",
            columns: [
                { field: "BranchHeadID", title: "BrachHead ID" },
                { field: "BranchHeadName", title: "BrachHead Name" },
            ]
        });
        lookup.dblClick(function (data) {
            if (data != null) {
                me.data.BranchHead = data.BranchHeadID;
                me.data.BranchHeadName = data.BranchHeadName;
                me.Apply();
            }
        });
    };

    me.SalesHead = function () {
        var salesType = ""
        if (me.data.DealerOption == '0') { salesType = "SALES" }
        if (me.data.DealerOption == '1') { salesType = "WHOLESALE" }
        if (me.data.DealerOption == '2') { salesType = "RETAIL" }
        if (me.data.DealerOption == '3') { salesType = "FPOL" }
        if (me.data.DealerOption == '4') { salesType = "REGPOL" }
        var lookup = Wx.blookup({
            name: "GetInquirySalesLookUpBtn",
            title: "Sales Head",
            manager: spSalesManager,
            query: new breeze.EntityQuery().from("GetInquirySalesLookUpBtn")
                .withParameters({ startDate: moment(me.data.StartDate).format('YYYY/MM/DD'), endDate: moment(me.data.EndDate).format('YYYY/MM/DD'), area: me.data.Area, dealer: me.data.Dealer, outlet: me.data.Outlet, branchHead: me.data.BranchHead, salesHead: "", salesCoordinator: "", salesman: "", detail: "5", salesType: salesType }),
            defaultSort: "GroupNo asc",
            columns: [
                { field: "SalesHeadID", title: "SalesHead ID" },
                { field: "SalesHeadName", title: "SalesHead Name" },
            ]
        });
        lookup.dblClick(function (data) {
            if (data != null) {
                me.data.SalesHead = data.SalesHeadID;
                me.data.SalesHeadName = data.SalesHeadName;
                me.Apply();
            }
        });
    };

    me.SalesCoordinator = function () {
        var salesType = ""
        if (me.data.DealerOption == '0') { salesType = "SALES" }
        if (me.data.DealerOption == '1') { salesType = "WHOLESALE" }
        if (me.data.DealerOption == '2') { salesType = "RETAIL" }
        if (me.data.DealerOption == '3') { salesType = "FPOL" }
        if (me.data.DealerOption == '4') { salesType = "REGPOL" }

        var lookup = Wx.blookup({
            name: "GetInquirySalesLookUpBtn",
            title: "SalesCoorditator",
            manager: spSalesManager,
            query: new breeze.EntityQuery().from("GetInquirySalesLookUpBtn")
                .withParameters({ startDate: moment(me.data.StartDate).format('YYYY/MM/DD'), endDate: moment(me.data.EndDate).format('YYYY/MM/DD'), area: me.data.Area, dealer: me.data.Dealer, outlet: me.data.Outlet, branchHead: me.data.BranchHead, salesHead: me.data.SalesHead, salesCoordinator: "", salesman: "", detail: "6", salesType: salesType }),
            defaultSort: "GroupNo asc",
            columns: [
                { field: "SalesCoordinatorID", title: "SalesCoordinator ID" },
                { field: "SalesCoordinatorName", title: "SalesCoordinator Name" },
            ]
        });
        lookup.dblClick(function (data) {
            if (data != null) {
                me.data.SalesCoordinator = data.SalesCoordinatorID;
                me.data.SalesCoordinatorName = data.SalesCoordinatorName;
                me.Apply();
            }
        });
    };

    me.Wiraniaga = function () {
        var salesType = ""
        if (me.data.DealerOption == '0') { salesType = "SALES" }
        if (me.data.DealerOption == '1') { salesType = "WHOLESALE" }
        if (me.data.DealerOption == '2') { salesType = "RETAIL" }
        if (me.data.DealerOption == '3') { salesType = "FPOL" }
        if (me.data.DealerOption == '4') { salesType = "REGPOL" }
        


        if (pType == "4W") {
            var lookup = Wx.blookup({
                name: "GetInquirySalesLookUpBtn",
                title: "Salesman",
                manager: spSalesManager,
                query: new breeze.EntityQuery().from("GetInquirySalesLookUpBtn")
                    .withParameters({ startDate: moment(me.data.StartDate).format('YYYY/MM/DD'), endDate: moment(me.data.EndDate).format('YYYY/MM/DD'), area: me.data.Area, dealer: me.data.Dealer, outlet: me.data.Outlet, branchHead: me.data.BranchHead, salesHead: me.data.SalesHead, salesCoordinator: "", salesman: "", detail: "7", salesType: salesType }),
                defaultSort: "GroupNo asc",
                columns: [
                    { field: "SalesmanID", title: "Salesman ID" },
                    { field: "SalesmanName", title: "Salesman Name" },
                ]
            });
            lookup.dblClick(function (data) {
                if (data != null) {
                    me.data.Wiraniaga = data.SalesmanID;
                    me.data.WiraniagaName = data.SalesmanName;
                    me.Apply();
                }
            });
        } else {
            var lookup = Wx.blookup({
                name: "GetInquirySalesLookUpBtn",
                title: "Salesman",
                manager: spSalesManager,
                query: new breeze.EntityQuery().from("GetInquirySalesLookUpBtn")
                    .withParameters({ startDate: moment(me.data.StartDate).format('YYYY/MM/DD'), endDate: moment(me.data.EndDate).format('YYYY/MM/DD'), area: me.data.Area, dealer: me.data.Dealer, outlet: me.data.Outlet, branchHead: me.data.BranchHead, salesHead: "", salesCoorditator: me.data.SalesCoordinator, salesman: "", detail: "7", salesType: salesType }),
                defaultSort: "GroupNo asc",
                columns: [
                    { field: "SalesmanID", title: "Salesman ID" },
                    { field: "SalesmanName", title: "Salesman Name" },
                ]
            });
            lookup.dblClick(function (data) {
                if (data != null) {
                    me.data.Wiraniaga = data.SalesmanID;
                    me.Apply();
                }
            });
        }
    };

    me.Type = function () {
        var salesType = ""
        if (me.data.DealerOption == '0') { salesType = "SALES" }
        if (me.data.DealerOption == '1') { salesType = "WHOLESALE" }
        if (me.data.DealerOption == '2') { salesType = "RETAIL" }
        if (me.data.DealerOption == '3') { salesType = "FPOL" }
        if (me.data.DealerOption == '4') { salesType = "REGPOL" }

        var lookup = Wx.blookup({
            name: "GetInquirySalesLookUpBtn",
            title: "MarketModel",
            manager: spSalesManager,
            query: new breeze.EntityQuery().from("GetInquirySalesLookUpBtn")
                .withParameters({ startDate: moment(me.data.StartDate).format('YYYY/MM/DD'), endDate: moment(me.data.EndDate).format('YYYY/MM/DD'), area: "", dealer: "", outlet: "", branchHead: "", salesHead: "", salesCoordinator: "", salesman: "", detail: "8", salesType: salesType }),
            defaultSort: "GroupNo asc",
            columns: [
                { field: "MarketModel", title: "Market Model" },
            ]
        });
        lookup.dblClick(function (data) {
            if (data != null) {
                me.data.Type = data.MarketModel;
                me.data.PreType = data.MarketModel;
                me.Apply();
            }
        });
    };

    me.start();
}



$(document).ready(function () {
    var options = {
        title: "Inquiry Sales",
        xtype: "panels",
        panels: [
            {
                name: "pnlA",
                items: [
                    {
                        text: "Periode",
                        type: "controls",
                        cls: "span5 full",
                        items: [
                            { name: "StartDate", cls: "span3", type: "ng-datepicker", disable: "AllowPeriod()" },
                            { type: "label", text: "s.d", cls: "span1 mylabel" },
                            { name: "EndDate", cls: "span3", type: "ng-datepicker", disable: "AllowPeriod()" },
                        ]
                    },
                    {
                        text: "Area",
                        type: "controls",
                        cls: "span8",
                        items: [
                            { name: "Area", cls: "span2 ", text: "Area", type: "popup", click: "Area()", readonly: true },
                        ]
                    },
                    {
                        text: "Dealer",
                        type: "controls",
                        cls: "span8",
                        items: [
                             { name: "Dealer", cls: "span2 ", text: "Dealer", type: "popup", click: "Dealer()", readonly: true },
                             { name: "DealerName", cls: "span6 ", readonly: true },
                        ]
                    },
                    {
                        text: "Outlet",
                        type: "controls",
                        cls: "span8",
                        items: [
                             { name: "Outlet", cls: "span2 ", text: "Outlet", type: "popup", click: "Outlet()", readonly: true },
                             { name: "OutletName", cls: "span6 ", readonly: true },
                        ]
                    },
                    {
                        text: "Branch Head",
                        type: "controls",
                        cls: "span8",
                        items: [
                            { name: "BranchHead", cls: "span2 ", text: "Branch Head", type: "popup", click: "BranchHead()", readonly: true },
                            { name: "BranchHeadName", cls: "span6 ", readonly: true },
                        ]
                    },
                    {
                        text: "Sales Head",
                        type: "controls",
                        cls: "span8",
                        show: "SHSow",
                        items: [
                            { name: "SalesHead", cls: "span2 ", text: "Sales Head", type: "popup", click: "SalesHead()", readonly: true, show: "SHSow" },
                            { name: "SalesHeadName", cls: "span6 ", readonly: true },
                        ]
                    },
                    {
                        text: "Sales Coordinator",
                        type: "controls",
                        cls: "span8",
                        show: "!SHSow",
                        items: [
                            { name: "SalesCoordinator", cls: "span2 ", text: "Sales Coordinator", type: "popup", click: "SalesCoordinator()", readonly: true, show: "!SHSow" },
                            { name: "SalesCoordinatorName", cls: "span6 ", readonly: true },
                        ]
                    },
                    {
                        text: "Wiraniaga/Sales",
                        type: "controls",
                        cls: "span8",
                        items: [
                            { name: "Wiraniaga", cls: "span2 ", text: "Wiraniaga/Sales", type: "popup", click: "Wiraniaga()", readonly: true },
                            { name: "WiraniagaName", cls: "span6 ", readonly: true },
                        ]
                    },
                    { type: "hr" },
                    { name: "ProductType", cls: "span4 full", type: "select2", text: "", datasource: "ProductType" },
                    { name: "isArea", type: "check", text: "Area", cls: "span4"},
                    { name: "isDealer", type: "check", text: "Dealer", cls: "span4 full" },
                    { name: "isOutlet", type: "check", text: "Outlet", cls: "span4 full" },
                    //{
                    //    type: "controls",
                    //    cls: "span3 full",
                    //    name: "ctrl1",
                    //    show: "is1",
                    //    items: [
                    //        { name: "isArea", type: "check", text: "Area", cls: "span1", float: "left" },
                    //        { type: "label", text: "Area", cls: "span7 mylabel" },
                    //    ]
                    //},
                    //{
                    //    type: "controls",
                    //    cls: "span3 full",
                    //    name: "ctrl2",
                    //    show: "is2",
                    //    items: [
                    //        { name: "isDealer", type: "check", text: "Dealer", cls: "span1", float: "left" },
                    //        { type: "label", text: "Dealer", cls: "span7 mylabel" },
                    //    ]
                    //},
                    //{
                    //    type: "controls",
                    //    cls: "span3 full",
                    //    name: "ctrl3",
                    //    items: [
                    //        { name: "isOutlet", type: "check", text: "Outlet", cls: "span1", float: "left" },
                    //        { type: "label", text: "Outlet", cls: "span7 mylabel" },
                    //    ]
                    //},
                    {
                        type: "controls",
                        cls: "span3 full",
                        name: "ctrl4",
                        show: "is4",
                        items: [
                            { name: "isBrachHead", type: "check", text: "Brach Head", cls: "span1", float: "left" },
                            { type: "label", text: "Brach Head", cls: "span7 mylabel" },
                        ]
                    },
                    {
                        type: "controls",
                        cls: "span3 full",
                        name: "ctrl5",
                        show: "is5",
                        items: [
                            { name: "isSalesHead", type: "check", text: "Sales Head", cls: "span1", float: "left" },
                            { type: "label", text: "Sales Head", cls: "span7 mylabel" },
                        ]
                    },
                    {
                        type: "controls",
                        cls: "span3 full",
                        name: "ctrl6",
                        show: "is6",
                        items: [
                            { name: "isWiraniaga", type: "check", text: "Wiraniaga", cls: "span1", float: "left" },
                            { type: "label", text: "Wiraniaga", cls: "span7 mylabel" },
                        ]
                    },

                    {
                        type: "controls",
                        cls: "span4",
                        name: "ctrl7",
                        show: "is7",
                        items: [
                            { name: "isType", type: "check", text: "", cls: "span1", float: "left" },
                            { type: "label", text: "Type", cls: "span1 mylabel" },
                            { name: "Type", cls: "span5 ", text: "Type", type: "popup", click: "Type()" },
                        ]
                    },
                    {
                        type: "controls",
                        cls: "span3 full",
                        name: "ctrl8",
                        show: "is8",
                        items: [
                            { name: "isColour", type: "check", text: "", cls: "span1", float: "left" },
                            { type: "label", text: "Warna", cls: "span7 mylabel" },
                        ]
                    },
                    {
                        type: "controls",
                        cls: "span3 full",
                        name: "ctrl9",
                        show: "is9",
                        items: [
                            { name: "isGread", type: "check", text: "", cls: "span1", float: "left" },
                            { type: "label", text: "Gread", cls: "span7 mylabel" },
                        ]
                    },
                    {
                        type: "controls",
                        cls: "span4",
                        name: "ctrl10",
                        show: "is10",
                        items: [
                            { name: "isType", type: "check", text: "", cls: "span1", float: "left" },
                            { type: "label", text: "Pre-Type", cls: "span1 mylabel" },
                            { name: "PreType", cls: "span5 ", text: "Pre-Type", type: "popup", click: "Type()" },
                        ]
                    },
                    {
                        type: "optionbuttons", name: "DealerOption", model: "data.DealerOption", text: "Dealer",
                        items: [
                            { name: "0", text: "Sales" },
                            { name: "1", text: "WholeSale" },
                            { name: "2", text: "Retail Sales" },
                            { name: "3", text: "Faktur Polisi" },
                            { name: "4", text: "Req. Pol", show: false },
                        ]
                    },
                    { type: "hr" },
                    {
                        type: "controls",
                        cls: "span6 full",
                        items: [
                            {
                                type: "buttons", cls: "span2", items: [
                                    { name: "btnExcel", text: "Excel", icon: "icon-ok", click: "ExcelData()", cls: "button small btn btn-success" },
                                ]
                            },
                            {
                                type: "buttons", cls: "span2", items: [
                                    { name: "btnRowData", text: "Row Data", icon: "icon-ok", click: "RowData()", cls: "button small btn btn-success" },
                                ]
                            },
                            {
                                type: "buttons", cls: "span2", items: [
                                    { name: "btnPivot", text: "Pivot", icon: "icon-ok", click: "PivotData()", cls: "button small btn btn-success" },
                                ]
                            },
                        ]
                    },
                    {
                        name: "wxDetail",
                        type: "wxdiv",
                        show: "!isPivot"
                    },

                ]
            },
            {
                name: 'wxpivotgrid',
                xtype: 'wxtable',
                style: 'margin-top: 35px;',
                show: "isPivot"
            }
        ]

    };

    Wx = new SimDms.Widget(options);
    Wx.default = {};
    Wx.render(init);



    function init(s) {
        $(".mylabel").attr("style", "padding:9px 9px 0px 5px");
        SimDms.Angular("omInquirySalesController");
    }



});