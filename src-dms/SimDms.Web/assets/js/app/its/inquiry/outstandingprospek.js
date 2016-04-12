"use strict";
var isCEO = false;
var empID = "";
var pType = "";
var CompanyCode = "";
var BranchCode = "";
var Position = "";

function ITSoutStandingProspek($scope, $http, $injector) {

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
                Position = dt.data.Position;
            }
        });
    }

    me.initialize = function () {
        me.data = {};
        me.NikBM = {};
        me.NikSH = {};
        me.NikSC = {};
        me.NikSL = {};
        me.data.Priode = me.now();
        me.isPrintAvailable = false;
        me.getUserProperties();
        me.isComboShow = pType === '4W' ? true : false;
        $http.post('its.api/InquiryIts/default').
        success(function (data, status, headers, config) {
            if (data.success == true) {
                me.NikBM = data.data.EmpBMList;
                me.NikSH = data.data.EmpSHList;
                me.NikSC = data.data.EmpSCList;
                me.NikSL = data.data.EmpSLList;
                CompanyCode = data.data.CompanyCode;
                BranchCode = data.data.BranchCode;
                if (data.data.Position == "S") {
                    if (pType == '4W') {
                        me.data.NikBM = data.data.NikBM;
                        me.data.NikSH = data.data.NikSH;
                        me.data.NikSL = data.data.NikSL;
                        Wx.enable({ value: false, items: ["NikBM", "NikSH", "NikSL"] });
                    } else {
                        me.data.NikBM = data.data.NikBM;
                        me.data.NikSC = data.data.NikSC;
                        me.data.NikSL = data.data.NikSL;
                        Wx.enable({ value: false, items: ["NikBM", "NikSC", "NikSL"] });
                    }
                }
                else if (data.data.Position == "SH") {
                    me.data.NikBM = data.data.NikBM;
                    me.data.NikSH = data.data.NikSH;
                    $http.post('its.api/Combo/getSalesmanByPos?emp=' + me.data.NikSH + '&pos=S').
                    success(function (data, status, headers, config) {
                        me.NikSL = data;
                    });
                    Wx.enable({ value: false, items: ["NikBM", "NikSH"] });
                }
                else if (data.data.Position == "SH") {
                    me.data.NikBM = data.data.NikBM;
                    me.data.NikSH = data.data.NikSC;
                    $http.post('its.api/Combo/getSalesmanByPos?emp=' + me.data.NikSC + '&pos=S').
                    success(function (data, status, headers, config) {
                        me.NikSL = data;
                    });
                    Wx.enable({ value: false, items: ["NikBM", "NikSC"] });
                }
                else if (data.data.Position == "BM") {
                    me.data.NikBM = data.data.NikBM;
                    $http.post('its.api/Combo/getSalesmanByPos?emp=' + me.data.NikBM + '&pos=SH').
                    success(function (data, status, headers, config) {
                        me.NikSH = data;
                        me.NikSC = data;
                    });
                    Wx.enable({ value: false, items: ["NikBM"] })
                }
                else if (data.data.Position == "GM") {
                    $http.post('its.api/Combo/BranchManager').
                            success(function (dt, status, headers, config) {
                                me.NikBM = dt;
                                Wx.enable({ value: false, items: ["NikSH", "NikSL"] })
                            });

                }
                else if (data.data.Position == "BMSH") {
                    me.data.NikBM = data.data.NikBM;
                    me.NikSH = data.data.EmpSHList;
                }
                else if (data.data.Position == "GMBM") {
                    $http.post('its.api/Combo/BranchManager').
                            success(function (dt, status, headers, config) {
                                me.NikBM = dt;
                                Wx.enable({ value: false, items: ["NikSH", "NikSL"] })
                            });
                }
            } else {
                MsgBox(data.message, MSG_ERROR);
                Wx.enable({ value: false, items: ["NikBM", "NikSH", "NikSC", "NikSL"] })
            }
        });
        //me.ClickSalesman();
    }

    $("[name=NikBM]").on('click', function () {
        $.ajax({
            async: true,
            type: "POST",
            data: { branch: $("[name='NikBM']").val() },
            url: 'its.api/Combo/getSH',
            success: function (data) {
                if (me.data.NikBM != "") {
                    Wx.enable({ value: true, items: ["NikSH"] })
                    me.NikSH = data;
                    me.Apply();
                } else {
                    Wx.enable({ value: false, items: ["NikSH", "NikSL"] })
                    me.data.NikSH = "";
                    me.data.NikSL = "";
                    me.Apply();
                }
            }
        });
    });

    $("[name=NikSH]").on('click', function () {
        if (me.data.NikSH != "") {
            $.ajax({
                async: true,
                type: "POST",
                data: { emp: $("[name='NikSH']").val(), pos: 'S' },
                url: 'its.api/Combo/getSalesmanByPos',
                success: function (data) {
                    if (me.data.NikBM != "") {
                        Wx.enable({ value: true, items: ["NikSL"] })
                        me.NikSL = data;
                        me.Apply();
                    } else {
                        Wx.enable({ value: false, items: ["NikSL"] })
                        me.data.NikSL = "";
                        me.Apply();
                    }
                }
            });
        } else {
            me.data.NikSH = "";
            me.data.NikSL = "";
            me.Apply();
        }
    });

    $("[name=NikSC]").on('click', function () {
        if (me.data.NikSH != "") {
            $.ajax({
                async: false,
                type: "POST",
                data: { emp: $("[name='NikSC']").val(), pos: 'S' },
                url: 'its.api/Combo/getSalesmanByPos',
                success: function (data) {
                    me.NikSL = data;
                    me.Apply();
                }
            });
        } else {
            $http.post('its.api/Combo/SalesHead?EmployeeID=' + empID).
                    success(function (dt, status, headers, config) {
                        me.NikSC = dt.list;
                        me.NikSL = dt.listSM;
                    });
            me.data.NikSH = "";
            me.Apply();
        }
    });

    $("[name=NikSL]").on('click', function () {
        if (me.data.NikSH == "" || me.data.NikSC == "") {
            if (me.data.NikSL != "") {
                $.ajax({
                    async: true,
                    type: "POST",
                    data: { emp: $("[name='NikSL']").val() },
                    url: 'its.api/Combo/getTeamLeader',
                    success: function (data) {
                        if (pType == '4W') {
                            Wx.enable({ value: false, items: ["cmbSH"] })
                            me.NikSH = data.listSH;
                            me.data.NikSH = data.empSH;
                            me.Apply();
                        } else {
                            Wx.enable({ value: false, items: ["cmbSH"] })
                            me.NikSC = data.listSH;
                            me.data.NikSC = data.empSH;
                            me.Apply();
                        }
                    }
                });
            } else {
                $http.post('its.api/Combo/SalesHead?EmployeeID=' + empID).
                    success(function (dt, status, headers, config) {
                        me.NikSH = dt.list;
                        me.NikSL = dt.listSM;
                    });
                Wx.enable({ value: true, items: ["cmbSH"] })
                me.data.NikSH = "";
                me.data.NikSC = "";
                me.Apply();
            }
        }
    });

    me.ClickSalesman = function (x) {
            $(".panel.tabpage1").hide();
            $(".panel.tabpage1.tA").show();
            $("p[data-name='tA']").addClass('active');
            $("p[data-name='tB']").removeClass('active');
            $("p[data-name='tC']").removeClass('active');
            me.clearTable(me.grid1);
            me.CariData(x);
    };

    me.ClickTipe = function (x) {
        $(".panel.tabpage1").hide();
        $(".panel.tabpage1.tB").show();
        me.clearTable(me.grid2);
        me.CariData(x);
    };

    me.ClickData = function (x) {
        $(".panel.tabpage1").hide();
        $(".panel.tabpage1.tC").show();
        me.clearTable(me.grid3);
        me.CariData(x);
    };

    me.CariData = function (x) {
        $http.post('its.api/inquiryoutstandingprospek/GetData4InqOutStanding', {
            Priode: moment(me.data.Priode).format('YYYYMMDD'),
            NikBM: me.data.NikBM,
            NikSH: me.data.NikSH,
            NikSC: me.data.NikSC,
            NikSL: me.data.NikSL,
            x : x
        }).
              success(function (data, status, headers, config) {
                  if (x == 1) {
                      me.loadTableData(me.grid1, data.queryable);
                  }
                  if (x == 2) {
                      me.loadTableData(me.grid2, data.queryable);
                  }
                  if (x == 3) {
                      me.loadTableData(me.grid3, data.queryable);
                  }
              }).
              error(function (e, status, headers, config) {
                  console.log(e);
              });
    }

    me.printPreview = function () {
        var COO = Position == 'GM' ? '1' : '';
        var periode = moment(me.data.Priode).format('YYYYMMDD');
        var tanggal = moment(me.data.Priode).format("MM-YYYY");
        var param1 = me.data.NikBM == "" ? "SEMUA" : $("[name=NikBM]").select2("data").text;
        var param2 = pType == '4W' ? me.data.NikSH == "" ? "SEMUA" : $("[name=NikSH]").select2("data").text : me.data.NikSC == "" ? "SEMUA" : $("[name=NikSC]").select2("data").text
        var param3 = "-";
        var param4 = me.data.NikSL == "" ? "SEMUA" : $("[name=NikSL]").select2("data").text;
        var param5 = pType == '4W' ? "NAMA SALES HEAD" : "NAMA SALES KORDINATOR";
        param1 = param1.replace(',', '.');
        param2 = param2.replace(',', '.');
        param4 = param4.replace(',', '.');
        var rparam = tanggal + "," + param1 + "," + param2 + "," + param3 + "," + param4 + "," + param5;
        var prm = [
            periode,
            COO,
            me.data.NikBM,
            me.data.NikSH,
            me.data.NikSC,
            me.data.NikSL
        ];
        Wx.showPdfReport({
            id: "PmRpInqOutStandingNew",
            pparam: prm.join(','),
            rparam: rparam,
            type: "devex"
        });
    }

    me.grid1 = new webix.ui({
        container: "SalesMan",
        view: "wxtable", css:"alternating",
        scrollX: true,
        scrollY: true,
        autoHeight: false,
        height: 350,
        columns: [
            { id: "PositionName", header: "POSISI", width: 150 },
            { id: "EmployeeName", header: "NAMA", width: 250},
            { id: "PROSPECT", header: "P", width: 200},
            { id: "HOTPROSPECT", header: "HP", width: 200},
            { id: "SPK", header: "SPK", width: 200 },
        ]
    });

    me.grid2 = new webix.ui({
        container: "TipeKendaraan",
        view: "wxtable", css:"alternating",
        scrollX: true,
        scrollY: true,
        autoHeight: false,
        height: 350,
        columns: [
            { id: "TipeKendaraan", header: "TIPE", width: 250 },
            { id: "PROSPECT", header: "P", width: 200 },
            { id: "HOTPROSPECT", header: "HP", width: 200 },
            { id: "SPK", header: "SPK", width: 200 },
        ]
    });

    me.grid3 = new webix.ui({
        container: "SumberData",
        view: "wxtable", css:"alternating",
        scrollX: true,
        scrollY: true,
        autoHeight: false,
        height: 350,
        columns: [
            { id: "Source", header: "PEROLEHAN DATA", width: 250 },
            { id: "PROSPECT", header: "P", width: 200 },
            { id: "HOTPROSPECT", header: "HP", width: 200 },
            { id: "SPK", header: "SPK", width: 200 },
        ]
    });
    me.start();
}


$(document).ready(function () {
    var options = {
        title: "Inquiry - OutStanding Prospek",
        xtype: "panels",
        toolbars: [
            { name: "btnPrintPreview", text: "Print", cls: "btn btn-primary", icon: "icon-print", click: "printPreview()", disable: "!isPrintEnable" },
        ],
        panels: [
            {
                name: "pnlLostCase",
                title: "Data OutStanding Prospek",
                items: [
                   { name: "Priode", text: "Priode", cls: "span3", type: "ng-datepicker" },
                   { name: "NikBM", text: "Brach Manager", cls: "span6", type: "select2", datasource: "NikBM" },
                   { name: "NikSH", text: "Sales Head", cls: "span6", type: "select2", datasource: "NikSH", show: "isComboShow" },
                   { name: "NikSC", text: "Sales Koordinator", cls: "span6", type: "select2", datasource: "NikSC", show: "!isComboShow" },
                   { name: "NikSL", text: "Salesman", cls: "span6", type: "select2", datasource: "NikSL", disable: "me.data.NikSC == ''" },
                   {
                       type: "buttons", cls: "span4", items: [
                           { name: "btnCari", text: "Cari", icon: "icon-search", click: "ClickSalesman(1)", cls: "button small btn btn-success" },
                       ]
                   },
                ]
            },
            {
                xtype: "tabs",
                name: "tabpage1",
                items: [
                    { name: "tA", text: "Berdasarkan Salesman", cls: "active", click: "ClickSalesman(1)" },
                    { name: "tB", text: "Berdasarkan Tipe Kendaraan", click: "ClickTipe(2)" },
                    { name: "tC", text: "Berdasarkan Sumber Data", click: "ClickData(3)" },
                ]
            },
            {
                name: "SalesMan",
                title: "Berdasarkan Salesman",
                cls: "tabpage1 tA",
                xtype: "wxtable"
            },
            {
                name: "TipeKendaraan",
                title: "Berdasarkan Tipe Kendaraan",
                cls: "tabpage1 tB",
                xtype: "wxtable"
            },
            {
                name: "SumberData",
                title: "Berdasarkan Sumber Data",
                cls: "tabpage1 tC",
                xtype: "wxtable"
            },
        ]
    };

    Wx = new SimDms.Widget(options);
    Wx.default = {};
    Wx.render(init);

    function init(s) {
        $(".mylabel").attr("style", "padding:9px 9px 0px 5px");
        SimDms.Angular("ITSoutStandingProspek");
    }

});
