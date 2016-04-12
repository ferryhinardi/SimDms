var report = "PmRpInqSummaryWeb";
var iTab = "1";
var isCEO = false;
var empID = "";
var pType = "";
var Branch = "";
"use strict"

function itsInquirySummary($scope, $http, $injector) {

    var me = $scope;

    $injector.invoke(BaseController, this, { $scope: me });

    me.getUserProperties = function () {
        $.ajax({
            async: false,
            type: "POST",
            url: 'its.api/InquiryIts/itsUserProperties',
            success: function (dt) {
                isCEO = dt.data.isCOO;
                empID = dt.data.EmployeeID;
                pType = dt.data.ProductType;
            }
        });
    }

    me.initialize = function () {
        me.data = {};
        me.data.dtpFrom = me.now();
        me.data.dtpTo = me.now();
        me.isPrintAvailable = false;
        me.clearTable(me.grid1);
        me.clearTable(me.grid2);
        me.clearTable(me.grid3);
        me.getUserProperties();
        me.isComboShow = pType === '4W' ? true : false;
        $http.post('its.api/InquiryIts/default').
            success(function (dt, status, headers, config) {
                if (dt.success) {
                    me.data.cmbBM = dt.data.NikBM;
                    me.data.cmbBMName = dt.data.NikBMName;
                    me.data.cmbSH = dt.data.NikSH;
                    me.data.cmbSHName = dt.data.NikSHName;
                    me.data.cmbSC = dt.data.NikSC;
                    me.data.cmbSCName = dt.data.NikSCName;
                    me.data.cmbSM = dt.data.NikSL;
                    me.data.cmbSMName = dt.data.NikSLName;
                    if (dt.data.Position == "S") {
                        if (pType == '4W') {
                            $('#cmbBM, #cmbSH, #cmbSM, #btncmbBM, #btncmbSH, #btncmbSM').attr('disabled', 'disabled');
                        } else {
                            $('#cmbBM, #cmbSC, #cmbSM, #btncmbBM, #btncmbSC, #btncmbSM').attr('disabled', 'disabled');
                        }
                    }
                    else if (dt.data.Position == "SH") {
                        if (pType == '4W') {
                            $('#cmbBM, #cmbSH, #btncmbBM, #btncmbSH').attr('disabled', 'disabled');
                        } else {
                            $('#cmbBM, #cmbSC, #btncmbBM, #btncmbSC').attr('disabled', 'disabled');
                        }
                    }
                    else if (dt.data.Position == "BM") {
                        $('#cmbBM, #btncmbBM').attr('disabled', 'disabled');
                    } else if (dt.data.Position == "GM") {
                        $('#cmbSH, #cmbSC, #cmbSM, #btncmbSH, #btncmbSC, #btncmbSM').attr('disabled', 'disabled');
                    }
                } else {
                    MsgBox(dt.message, MSG_INFO);
                    $('#cmbBM, #cmbSH, #cmbSC, #cmbSM, #btncmbBM, #btncmbSH, #btncmbSC, #btncmbSM').attr('disabled', 'disabled');
                    $('#btnCari').attr('disabled', 'disabled');
                }
            });
    }

    me.BM = function () {
        var lookup = Wx.klookup({
            name: "btnBrowse",
            title: "Branch Manager",
            url: "its.api/grid/BranchManager",
            //params: { name: "controls", items: [{ name: "NikSH", param: "NikSH" }, { name: "NikSC", param: "NikSC" }] },
            pageSize: 10,
            serverBinding: true,
            columns: [
                { field: "EmployeeID", title: "ID" },
                { field: "EmployeeName", title: "Branch Manager" },
                { field: "TitleName", title: "Jabatan" },
            ]
        });
        lookup.dblClick(function (data) {
            if (data != null) {
                me.data.cmbBM = data.EmployeeID;
                me.data.cmbBMName = data.EmployeeName;
                me.data.cmbSH = undefined;
                me.data.cmbSHName = undefined;
                me.data.cmbSM = undefined;
                me.data.cmbSMName = undefined;
                me.Apply();
                if ($("[name='cmbBM']").val() != "") {
                    if (pType == '4W') {
                        $('#cmbSH, #btncmbSH').removeAttr('disabled');
                        $('#cmbSM, #btncmbSM').attr('disabled', 'disabled');
                    } else {
                        $('#cmbSC, #btncmbSC').removeAttr('disabled');
                        $('#cmbSM, #btncmbSM').attr('disabled', 'disabled');
                    }
                }
                $.ajax({
                    async: false,
                    type: "POST",
                    url: 'its.api/InquiryIts/getBranchEmployee?Employeeid=' + $("[name='cmbBM']").val(),
                    success: function (dt) {
                        Branch = dt.data.Branch;
                    }
                });
            }
        });
    }

    me.SalesHead = function () {
        var lookup = Wx.klookup({
            name: "btnBrowse",
            title: "Sales Head",
            url: "its.api/grid/SalesHead",
            params: { name: "controls", items: [{ name: "cmbBM", param: "cmbBM" }] },
            pageSize: 10,
            serverBinding: true,
            columns: [
                { field: "EmployeeID", title: "ID" },
                { field: "EmployeeName", title: "Sales Head" },
                { field: "TitleName", title: "Jabatan" },
            ]
        });
        lookup.dblClick(function (data) {
            if (data != null) {
                me.data.cmbSH = data.EmployeeID;
                me.data.cmbSHName = data.EmployeeName;
                me.data.cmbSM = undefined;
                me.data.cmbSMName = undefined;
                me.Apply();
                if ($("[name='cmbSH']").val() != "") {
                    $('#cmbSM, #btncmbSM').removeAttr('disabled');
                }
            }
        });
    }

    me.Salesman = function () {
        var lookup = Wx.klookup({
            name: "btnBrowse",
            title: "Salesman",
            url: "its.api/grid/Salesman",
            params: { name: "controls", items: [{ name: "cmbSH", param: "cmbSH" }, { name: "cmbSC", param: "cmbSC" }] },
            pageSize: 10,
            serverBinding: true,
            columns: [
                { field: "EmployeeID", title: "ID" },
                { field: "EmployeeName", title: "Salesman" },
                { field: "TitleName", title: "Jabatan" },
            ]
        });
        lookup.dblClick(function (data) {
            if (data != null) {
                me.data.cmbSM = data.EmployeeID;
                me.data.cmbSMName = data.EmployeeName;
                me.Apply();
            }
        });
    }

    me.tbl1 = function () {
        var data1 = {
            IdTab: "1",
            DateFrom: moment(me.data.dtpFrom).format('YYYY-MM-DD'),
            DateTo: moment(me.data.dtpTo).format('YYYY-MM-DD'),
            EmpID1: $("[name=cmbBM]").val(),
            EmpID2: pType === '4W' ? $("[name=cmbSH]").val() : $("[name=cmbSC]").val(),
            EmpID3: $("[name=cmbSM]").val(),
            IsGM: isCEO
        }
        console.log('Branch Manager : ' + $("[name=cmbBM]").val() + ' SH : ' + $("[name=cmbSH]").val() + ' Sales : ' + $("[name=cmbSM]").val());
        $('.page > .ajax-loader').show();
        $http.post('its.api/InquiryIts/itsLoadInqSummary', data1)
        .success(function (dt, status, headers, config) {
            $('.page > .ajax-loader').hide();
            me.loadTableData(me.grid1, dt);
            me.isPrintAvailable = true;
        })
        .error(function (e, status, header, config) {
            $('.page > .ajax-loader').hide();
            MsgBox(e, MSG_ERROR);
        });
    }

    me.tbl2 = function () {
        var data2 = {
            IdTab: "2",
            DateFrom: moment(me.data.dtpFrom).format('YYYY-MM-DD'),
            DateTo: moment(me.data.dtpTo).format('YYYY-MM-DD'),
            EmpID1: $("[name=cmbBM]").val(),
            EmpID2: pType === '4W' ? $("[name=cmbSH]").val() : $("[name=cmbSC]").val(),
            EmpID3: $("[name=cmbSM]").val(),
            IsGM: isCEO
        }
        $http.post('its.api/InquiryIts/itsLoadInqSummary', data2)
        .success(function (dt, status, headers, config) {
            $('.page > .ajax-loader').hide();
            me.loadTableData(me.grid2, dt);
            me.isPrintAvailable = true;
        })
        .error(function (e, status, header, config) {
            $('.page > .ajax-loader').hide();
            MsgBox(e, MSG_ERROR);
        });
    }

    me.tbl3 = function () {
        var data3 = {
            IdTab: "3",
            DateFrom: moment(me.data.dtpFrom).format('YYYY-MM-DD'),
            DateTo: moment(me.data.dtpTo).format('YYYY-MM-DD'),
            EmpID1: $("[name=cmbBM]").val(),
            EmpID2: pType === '4W' ? $("[name=cmbSH]").val() : $("[name=cmbSC]").val(),
            EmpID3: $("[name=cmbSM]").val(),
            IsGM: isCEO
        }
        $http.post('its.api/InquiryIts/itsLoadInqSummary', data3)
        .success(function (dt, status, headers, config) {
            $('.page > .ajax-loader').hide();
            me.loadTableData(me.grid3, dt);
            me.isPrintAvailable = true;
        })
        .error(function (e, status, header, config) {
            $('.page > .ajax-loader').hide();
            MsgBox(e, MSG_ERROR);
        });
    }

    me.clkLoadData = function () {
        //Load data by Salesman
        me.tbl1();

        //Load data by Tipe Kendaraan
        me.tbl2();

        //Load data by Sumber Data
        me.tbl3();
    }

    webix.event(window, "resize", function () {
        me.grid1.adjust();
        me.grid2.adjust();
        me.grid3.adjust();
    });

    me.OnTabChange = function (e, id) {
        if (id === "tabSalesman") {
            me.grid1.adjust();
            iTab = "1";
            me.tbl1();
        }
        if (id === "tabTipeKendaraan") {
            me.grid2.adjust();
            iTab = "2";
            me.tbl2();
        }
        if (id === "tabSumberData") {
            me.grid3.adjust();
            iTab = "3";
            me.tbl3();
        }
    }

    me.printPreview = function () {
        var firstPeriod = moment(me.data.dtpFrom).format("YYYY-MM-DD");
        var endPeriod = moment(me.data.dtpTo).format("YYYY-MM-DD");
        var period = moment(me.data.dtpFrom).format("DD-MM-YYYY") + " S/D " + moment(me.data.dtpTo).format("DD-MM-YYYY");
        var bm = $("[name=cmbBM]").val();
        var sh = pType == '4W' ? $("[name=cmbSH]").val() : $("[name=cmbSC]").val();
        var sm = $("[name=cmbSM]").val();
        
        console.log('Branch Manager : ' + $("[name=cmbBM]").val() + ' SH : ' + $("[name=cmbSH]").val() + ' Sales : ' + $("[name=cmbSM]").val());
        //var par = [firstPeriod + "," + endPeriod + "," + bm + "," + sh + "," + sm + "," + iTab + "," + 1];
        var par = [
            'companycode',
            Branch,
            firstPeriod, endPeriod, bm, sh, sm, '', 1
        ];

        var param1, param2, param3, param4;
        var param3 = "";
        if ($("[name=cmbBM]").val() == "" || $("[name=cmbBM]").val() == "undefined")
            param1 = "SEMUA";
        else
            param1 = $("[name=cmbBMName]").val();

        if (pType == '4W') {
            if ($("[name=cmbSH]").val() == "" || $("[name=cmbSM]").val() == "undefined")
                param2 = "SEMUA";
            else
                param2 = $("[name=cmbSHName]").val();
        } else {
            if ($("[name=cmbSC]").val() == "" && $("[name=cmbSC]").val() == "undefined")
                param2 = "SEMUA";
            else
                param2 = $("[name=cmbSCName]").val();
        }
        
        if ($("[name=cmbSM]").val() == "" || $("[name=cmbSM]").val() == "undefined")
            param4 = "SEMUA";
        else
            param4 = $("[name=cmbSMName]").val();

        var param5 = pType == '4W' ? "NAMA SALES HEAD" : "NAMA SALES KORDINATOR";
        param1 = param1.replace(',', '.');
        param2 = param2.replace(',', '.');
        param4 = param4.replace(',', '.');
        rparam = [period, param1, param2, param3, param4, param5];
        console.log(period, param1, param2, param3, param4, param5);
        Wx.showPdfReport({
            id: report,
            pparam: par,
            rparam: rparam,
            type: "devex"
        });
    }

    $("[name='cmbBM']").on('blur', function () {
        if ($("[name='cmbBM']").val() == "") {
            me.data.cmbBMName = undefined;
            $("[name='cmbBMName']").val("");
        } 
    });

    $("[name='cmbSH']").on('blur', function () {
        if ($("[name='cmbSH']").val() == "") {
            me.data.cmbSHName = undefined;
            $("[name='cmbSHName']").val("");
        }
    });

    $("[name='cmbSC']").on('blur', function () {
        if ($("[name='cmbSC']").val() == "") {
            me.data.cmbSCName = undefined;
            $("[name='cmbSCName']").val("");
        }
    });

    $("[name='cmbSM']").on('blur', function () {
        if ($("[name='cmbSM']").val() == "") {
            me.data.cmbSMName = undefined;
            $("[name='cmbSMName']").val("");
        }
    });

    me.grid1 = new webix.ui({
        container: "tabSalesman",
        view: "wxtable", css: "alternating",
        autowidth: true,
        scrollx: true,
        scrollY: true,
        height: 450,
        autoHeight: false,
        columns: [
            { id: "Position", header: "Posisi", width: 120 },
            { id: "EmployeeName", header: "Nama", width: 250 },
            { id: "NEW", header: "New", width: 90, css: { "text-align": "right" } },
            { id: "REORDER", header: "Re Order", width: 90, css: { "text-align": "right" } },
            { id: "PROSPECT", header: "Prospect", width: 90, css: { "text-align": "right" } },
            { id: "HOTPROSPECT", header: "Hot Pros.", width: 90, css: { "text-align": "right" } },
            { id: "SPK", header: "SPK", width: 90, css: { "text-align": "right" } },
            { id: "DO", header: "DO", width: 90, css: { "text-align": "right" } },
            { id: "DELIVERY", header: "Delivery", width: 90, css: { "text-align": "right" } },
            { id: "LOST", header: "Lost", width: 90, css: { "text-align": "right" } }
        ]
    });

    me.grid2 = new webix.ui({
        container: "tabTipeKendaraan",
        view: "wxtable", css: "alternating",
        autowidth: false,
        scrollx: true,
        scrollY: true,
        height: 450,
        autoHeight: false,
        columns: [
            { id: "TipeKendaraan", header: "Tipe Kendaraan", width: 300, format: me.replaceNull },
            { id: "NEW", header: "New", width: 90, css: { "text-align": "right" } },
            { id: "REORDER", header: "Re Order", width: 90, css: { "text-align": "right" } },
            { id: "PROSPECT", header: "Prospect", width: 90, css: { "text-align": "right" } },
            { id: "HOTPROSPECT", header: "Hot Pros.", width: 90, css: { "text-align": "right" } },
            { id: "SPK", header: "SPK", width: 90, css: { "text-align": "right" } },
            { id: "DO", header: "DO", width: 90, css: { "text-align": "right" } },
            { id: "DELIVERY", header: "Delivery", width: 90, css: { "text-align": "right" } },
            { id: "LOST", header: "Lost", width: 90, css: { "text-align": "right" } }
        ]
    });

    me.grid3 = new webix.ui({
        container: "tabSumberData",
        view: "wxtable", css: "alternating",
        autowidth: false,
        scrollx: true,
        scrollY: true,
        height: 450,
        autoHeight: false,
        columns: [
            { id: "SumberData", header: "Sumber Data", width: 250 },
            { id: "NEW", header: "New", width: 100, css: { "text-align": "right" } },
            { id: "REORDER", header: "Re Order", width: 100, css: { "text-align": "right" } },
            { id: "PROSPECT", header: "Prospect", width: 100, css: { "text-align": "right" } },
            { id: "HOTPROSPECT", header: "Hot Pros.", width: 100, css: { "text-align": "right" } },
            { id: "SPK", header: "SPK", width: 100, css: { "text-align": "right" } },
            { id: "DO", header: "DO", width: 100, css: { "text-align": "right" } },
            { id: "DELIVERY", header: "Delivery", width: 100, css: { "text-align": "right" } },
            { id: "LOST", header: "Lost", width: 100, css: { "text-align": "right" } }
        ]
    });

    me.start();
}

$(document).ready(function () {
    var options = {
        title: "Inquiry Summary",
        xtype: "panels",
        toolbars: [
                    { name: "btnPrint", cls: "btn btn-primary", text: "Print", icon: "icon-print", click: "printPreview()", show: "isPrintAvailable" }
        ],
        panels: [
            {
                items: [
                            {
                                text: "Date (From - To)",
                                type: "controls",
                                items: [
                                            { name: "dtpFrom", cls: "span2", type: "ng-datepicker" },
                                            { name: "dtpTo", cls: "span2", type: "ng-datepicker" }
                                ]
                            },
                            //{ name: "cmbBM", cls: "span4 full", text: "Branch Manager", type: "select2", opt_text: "[SELECT ALL]", datasource: "comboBM" },
                            {
                                type: "controls", text: "Branch Manager", cls: "span8", items: [
                                    { name: "cmbBM", model: "data.cmbBM", type: "popup", text: "Branch Manager", cls: "span3", click: "BM()" },
                                    { name: "cmbBMName", model: "data.cmbBMName", type: "", text: "Branch Manager Name", cls: "span5", readonly: true },
                                ],
                            },
                            //{ name: "cmbSH", cls: "span4 full", text: "Sales Header", type: "select2", opt_text: "[SELECT ALL]", datasource: "comboSH", show: "isComboShow"},
                             {
                                 type: "controls", text: "Sales Head", cls: "span8", show: "isComboShow", items: [
                                     { name: "cmbSH", model: "data.cmbSH", type: "popup", text: "Sales Head", cls: "span3", click: "SalesHead()" },
                                     { name: "cmbSHName", model: "data.cmbSHName", type: "", text: "Sales Head Name", cls: "span5", readonly: true },
                                 ],
                             },
                            //{ name: "cmbSC", cls: "span4 full", text: "Sales Coordinator", type: "select2", opt_text: "[SELECT ALL]", datasource: "comboSC", show: "!isComboShow" },
                            {
                                type: "controls", text: "Sales Kordinator", cls: "span8", show: "!isComboShow", items: [
                                    { name: "cmbSC", model: "data.cmbSC", type: "popup", text: "Sales Coor", cls: "span3", click: "SalesHead()" },
                                    { name: "cmbSCName", model: "data.cmbSCName", type: "", text: "Sales Coor Name", cls: "span5", readonly: true },
                                ],
                            },
                            //{ name: "cmbSM", cls: "span4 full", text: "Salesman", type: "select2", opt_text: "[SELECT ALL]", datasource: "comboSM" },
                            {
                                type: "controls", text: "Salesman", cls: "span8", items: [
                                    { name: "cmbSM", model: "data.cmbSM", type: "popup", text: "Salesman", cls: "span3", click: "Salesman()" },
                                    { name: "cmbSMName", model: "data.cmbSMName", type: "", text: "Salesman Name", cls: "span5", readonly: true },
                                ],
                            },
                            {
                                type: "buttons", cls: "span2 left", items: [
                                { name: "btnCari", cls: "btn-small", text: "Cari", icon: "icon-search", click: "clkLoadData()" }
                                ]
                            }

                ]
            },
            {
                xtype: "tabs",
                name: "tabpage1",
                items: [
                           { name: "tabSalesman", text: "Berdasarkan Salesman" },
                           { name: "tabTipeKendaraan", text: "Berdasarkan Tipe Kendaraan" },
                           { name: "tabSumberData", text: "Berdasarkan Sumber Data" }
                       ]
             },
             {
                title: "",
                cls: "tabpage1 tabSalesman",
                xtype: "wxtable",
                name: "tabSalesman"
             },
             {
                title: "",
                cls: "tabpage1 tabTipeKendaraan",
                xtype: "wxtable",
                name: "tabTipeKendaraan"
             },
             {
                title: "",
                cls: "tabpage1 tabSumberData",
                xtype: "wxtable",
                name: "tabSumberData"
             }]
    };

    Wx = new SimDms.Widget(options);
    Wx.default = {};
    Wx.render(init);

    $("p[data-name='tabSalesman']").addClass('active');

    function init(s) {
        SimDms.Angular("itsInquirySummary");
    }

});