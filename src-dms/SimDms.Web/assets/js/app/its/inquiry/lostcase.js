var report = "PmRpInqSummaryWeb";
var iTab = "1";
var isCEO = false;
var empID = "";
var pType = "";
var CompanyCode = "";
var BranchCode = "";

"use strict";

function ITSLostCase($scope, $http, $injector) {

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
        me.comboBM = {};
        me.comboSH = {};
        me.comboSC = {};
        me.comboSM = {};
        me.data.dtpFrom = me.now();
        me.data.dtpTo = me.now();
        me.isPrintAvailable = false;
        me.getUserProperties();
        me.isComboShow = pType === '4W' ? true : false;
        $http.post('its.api/InquiryIts/default').
            success(function (dt, status, headers, config) {
                if (dt.success) {
                    me.comboBM = dt.data.EmpBMList;
                    me.comboSH = dt.data.EmpSHList;
                    me.comboSC = dt.data.EmpSCList;
                    me.comboSM = dt.data.EmpSLList;
                    CompanyCode = dt.data.CompanyCode;
                    BranchCode = dt.data.BranchCode;
                    if (dt.data.Position == "S") {
                        if (pType == '4W') {
                            me.data.cmbBM = dt.data.NikBM;
                            me.data.cmbSH = dt.data.NikSH;
                            me.data.cmbSM = dt.data.NikSL;
                            Wx.enable({ value: false, items: ["cmbBM", "cmbSH", "cmbSM"] });
                        } else {
                            me.data.cmbBM = dt.data.NikBM;
                            me.data.cmbSC = dt.data.NikSC;
                            me.data.cmbSM = dt.data.NikSL;
                            Wx.enable({ value: false, items: ["cmbBM", "cmbSC", "cmbSM"] });
                        }
                    }
                    else if (dt.data.Position == "SH" || dt.data.Position == "SHSTD") {
                        me.data.cmbBM = dt.data.NikBM;
                        me.data.cmbSH = dt.data.NikSH;
                        $http.post('its.api/Combo/getSalesmanByPos?emp=' + me.data.cmbSH + '&pos=S').
                        success(function (data, status, headers, config) {
                            me.comboSM = data;
                        });
                        Wx.enable({ value: false, items: ["cmbBM", "cmbSH"] });
                    }
                    else if (dt.data.Position == "SC") {
                        me.data.cmbBM = dt.data.NikBM;
                        me.data.cmbSC = dt.data.NikSC;
                        $http.post('its.api/Combo/getSalesmanByPos?emp=' + me.data.cmbSC + '&pos=S').
                        success(function (data, status, headers, config) {
                            me.comboSM = data;
                        });
                        Wx.enable({ value: false, items: ["cmbBM", "cmbSC"] });
                    }
                    else if (dt.data.Position == "BM") {
                        me.data.cmbBM = dt.data.NikBM;
                        $http.post('its.api/Combo/getSalesmanByPos?emp=' + me.data.cmbBM + '&pos=SH').
                        success(function (data, status, headers, config) {
                            me.comboSH = data;
                            me.comboSC = data;
                        });
                        Wx.enable({ value: false, items: ["cmbBM"] })
                    }
                    else if (dt.data.Position == "GM") {
                        $http.post('its.api/Combo/BranchManager').
                                success(function (dt, status, headers, config) {
                                    me.comboBM = dt;
                                    Wx.enable({ value: false, items: ["cmbSH", "cmbSM"] })
                                });
                        
                    }
                    else if (dt.data.Position == "BMSH") {
                        me.data.cmbBM = dt.data.NikBM;
                        me.comboSH = dt.data.EmpSHList;
                    }
                    else if (dt.data.Position == "GMBM") {
                        $http.post('its.api/Combo/BranchManager').
                                success(function (dt, status, headers, config) {
                                    me.comboBM = dt;
                                    Wx.enable({ value: false, items: ["cmbSH", "cmbSM"] })
                                });
                    }
                } else {
                    MsgBox(dt.message, MSG_ERROR);
                    Wx.enable({ value: false, items: ["cmbBM", "cmbSH", "cmbSC", "cmbSM"] })
                }
            });
    }

    $("[name=cmbBM]").on('click', function () {
        $.ajax({
            async: true,
            type: "POST",
            data: { branch: $("[name='cmbBM']").val() },
            url: 'its.api/Combo/getSH',
            success: function (data) {
                if (me.data.cmbBM != "") {
                    Wx.enable({ value: true, items: ["cmbSH"] })
                    me.comboSH = data;
                    me.Apply();
                } else {
                    Wx.enable({ value: false, items: ["cmbSH"] })
                    me.data.cmbSH = "";
                    me.Apply();
                }
            }
        });
    });

    $("[name=cmbSH]").on('click', function () {
        if (me.data.cmbSH != "") {
            $.ajax({
                async: true,
                type: "POST",
                data: { emp: $("[name='cmbSH']").val(), pos: 'S' },
                url: 'its.api/Combo/getSalesmanByPos',
                success: function (data) {
                    Wx.enable({ value: true, items: ["cmbSM"] })
                    me.comboSM = data;
                    me.Apply();
                }
            });
        } else {
            $http.post('its.api/Combo/SalesHead?EmployeeID=' + empID).
                    success(function (dt, status, headers, config) {
                        me.comboSH = dt.list;
                        me.comboSM = dt.listSM;
                    });
            me.data.cmbSM = "";
        }
    });

    $("[name=cmbSC]").on('click', function () {
        if (me.data.cmbSC != "") {
            $.ajax({
                async: false,
                type: "POST",
                data: { emp: $("[name='cmbSC']").val(), pos: 'S' },
                url: 'its.api/Combo/getSalesmanByPos',
                success: function (data) {
                    Wx.enable({ value: true, items: ["cmbSM"] })
                    me.comboSM = data;
                    me.Apply();
                }
            });
        } else {
            $http.post('its.api/Combo/SalesHead?EmployeeID=' + empID).
                    success(function (dt, status, headers, config) {
                        me.comboSH = dt.list;
                        me.comboSM = dt.listSM;
                    });
            me.data.cmbSM = "";
        }
    });

    $("[name=cmbSM]").on('click', function () {
        if (me.data.cmbSH == "" || me.data.cmbSC == "") {
            if (me.data.cmbSM != "") {
                $.ajax({
                    async: true,
                    type: "POST",
                    data: { emp: $("[name='cmbSM']").val() },
                    url: 'its.api/Combo/getTeamLeader',
                    success: function (data) {
                        if (pType == '4W') {
                            Wx.enable({ value: false, items: ["cmbSH"] })
                            me.comboSH = data.listSH;
                            me.data.cmbSH = data.empSH;
                            me.Apply();
                        } else {
                            Wx.enable({ value: false, items: ["cmbSH"] })
                            me.comboSC = data.listSH;
                            me.data.cmbSC = data.empSH;
                            me.Apply();
                        }
                    }
                });
            } else {
                $http.post('its.api/Combo/SalesHead?EmployeeID=' + empID).
                    success(function (dt, status, headers, config) {
                        me.comboSH = dt.list;
                        me.comboSM = dt.listSM;
                    });
                Wx.enable({ value: true, items: ["cmbSH"] })
                me.data.cmbSH = "";
                me.data.cmbSC = "";
                me.Apply();
            }
        }
    });

    me.printPreview = function () {
        var tanggal = moment(me.data.dtpFrom).format("DD-MM-YYYY") + " S/D " + moment(me.data.dtpTo).format("DD-MM-YYYY");
        var param1 = me.data.cmbBM == "" ? "SEMUA" : $("[name=cmbBM]").select2("data").text;
        var param2 = pType == '4W' ? me.data.cmbSH == "" ? "SEMUA" : $("[name=cmbSH]").select2("data").text : me.data.cmbSC == "" ? "SEMUA" : $("[name=cmbSC]").select2("data").text
        var param3 = "-";
        var param4 = me.data.cmbSM == "" ? "SEMUA" : $("[name=cmbSM]").select2("data").text;
        var param5 = pType == '4W' ? "Nama Sales Head" : "Nama Sales Kordinator";
        param1 = param1.replace(',', '.');
        param2 = param2.replace(',', '.');
        param4 = param4.replace(',', '.');
        var rparam = tanggal + "," + param1 + "," + param2 + "," + param3 + "," + param4 + "," + param5;
        var prm = [
                   moment(me.data.dtpFrom).format('YYYYMMDD'),
                   moment(me.data.dtpTo).format('YYYYMMDD'),
                   me.data.cmbBM,
                   me.data.cmbSH,
                   me.data.cmbSC,
                   me.data.cmbSM
        ];
        console.log(prm);
        Wx.showPdfReport({
            id: "PmRpInqLostCaseWeb",
            pparam: prm.join(','),
            rparam: rparam,
            type: "devex"
        });
    }

    me.start();
}


$(document).ready(function () {
    var options = {
        title: "Inquiry - Analisa Lost Case",
        xtype: "panels",
        toolbars: [
            { name: "btnPrintPreview", text: "Print", cls: "btn btn-primary", icon: "icon-print", click: "printPreview()", disable: "!isPrintEnable" },
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
                            { name: "cmbBM", cls: "span4 full", text: "Branch Manager", type: "select2", opt_text: "[SELECT ALL]", datasource: "comboBM" },
                            { name: "cmbSH", cls: "span4 full", text: "Sales Head", type: "select2", opt_text: "[SELECT ALL]", datasource: "comboSH", show: "isComboShow" },
                            { name: "cmbSC", cls: "span4 full", text: "Sales Coordinator", type: "select2", opt_text: "[SELECT ALL]", datasource: "comboSC", show: "!isComboShow" },
                            { name: "cmbSM", cls: "span4 full", text: "Salesman", type: "select2", opt_text: "[SELECT ALL]", datasource: "comboSM" },

                ]
            },
        ]
    };
    
    Wx = new SimDms.Widget(options);
    Wx.default = {};
    Wx.render(init);

    function init(s) {
        $(".mylabel").attr("style", "padding:9px 9px 0px 5px");
        SimDms.Angular("ITSLostCase");
    }

});


//$(document).ready(function () {
//    var widget = new SimDms.Widget({
//        title: "Inquiry - Analisa Lost Case",
//        xtype: "panels",
//        panels: [
//            {
//                name: "pnlFilter",
//                items: [
//                    {
//                        text: "Date (From - To)",
//                        type: "controls", items: [
//                            { name: "DateFrom", text: "Date From", cls: "span2", type: "kdatepicker" },
//                            { name: "DateTo", text: "Date To", cls: "span2", type: "kdatepicker" },
//                        ]
//                    },
//                    { name: "NikBM", text: "Brach Manager", cls: "span6", type: "select" },
//                    { name: "NikSH", text: "Sales Head", cls: "span6", type: "select" },
//                    { name: "NikSC", text: "Sales Koordinator", cls: "span6", type: "select" },
//                    { name: "NikSL", text: "Salesman", cls: "span6", type: "select" },
//                ],
//            },
//            {
//                name: "TDayCall",
//                xtype: "k-grid",
//            },
//        ],
//        toolbars: [
//            { name: "btnRefresh", text: "Refresh", icon: "icon-refresh" },
//        ]
//    });
//    widget.render(init);

//    function init() {
//        widget.post("its.api/inquiryits/default", function (result) {
//            if (result.success) {
//                widget.default = result.data;
//                widget.select({ name: "NikSL", data: result.data.EmpSLList });
//                widget.select({ name: "NikSC", data: result.data.EmpSCList, optionText: "-- ALL SC --", optionValue: "--" });
//                widget.select({ name: "NikSH", data: result.data.EmpSHList });
//                if (result.data.Position == "S") widget.enable({ value: false, items: ["NikSL", "NikSC", "NikSH"] });
//                if (result.data.Position == "SC") widget.enable({ value: false, items: ["NikSC", "NikSH"] });
//                if (result.data.Position == "SH") {
//                    widget.enable({ value: false, items: ["NikSH"] });
//                    widget.selectparam({ name: "NikSL", url: "its.api/combo/employee", param: "NikSC", optionText: "-- ALL SALESMAN --" });
//                }
//                widget.populate(widget.default);
//            }
//            else {
//                widget.alert(result.message || "User belum terdaftar di Master Position !");
//                widget.showToolbars([]);
//            }
//        });
//    };
//    $('#btnRefresh').click(refresh);
//    function refresh() {
//        var params = {
//            Nik: $("[name=NikSL]").val(),
//            DateFrom: getSqlDate($("[name=DateFrom]").val()),
//            DateTo: getSqlDate($("[name=DateTo]").val())
//        }
//        widget.kgrid({
//            url: "its.api/inquiryits/lostcase",
//            name: "pnlList",
//            params: params,
//            columns: [
//                { field: "InquiryNumber", title: "No. Inquiry", width: 100 },
//                { field: "NamaProspek", title: "NamaProspek", width: 280 },
//                { field: "InquiryDate", title: "Tgl Inquiry", width: 120, template: "#= (InquiryDate == undefined) ? '' : moment(InquiryDate).format('DD MMM YYYY') #" },
//                { field: "TipeKendaraan", title: "Tipe", width: 200 },
//                { field: "Variant", title: "Variant", width: 180 },
//                { field: "PerolehanDataDesc", title: "Perolehan Data", width: 200 },
//                { field: "LostDate", title: "Tgl Lost", width: 120, template: "#= (LostDate == undefined) ? '' : moment(LostDate).format('DD MMM YYYY') #" },
//                { field: "LostCaseCategoryDesc", title: "Kategori", width: 300 },
//                { field: "LostCaseOtherReason", title: "Alasan", width: 300 },
//                { field: "LostCaseVoiceOfCustomer", title: "Voice of Customer", width: 300 },
//            ],
//        });
//        widget.chart({
//            name: "pnlChart1",
//            url: "its.api/inquiryits/lostbytype",
//            params: params,
//        });
//        widget.chart({
//            name: "pnlChart2",
//            url: "its.api/inquiryits/lostbysource",
//            params: params,
//        });
//    }

//    function exportXls() {
//        widget.exportXls({
//            name: "pnlList",
//            type: "kgrid",
//            items: [
//                { name: "InquiryNumber", text: "No. Inquiry", width: 100 },
//                { name: "NamaProspek", text: "NamaProspek", width: 280 },
//                { name: "InquiryDate", text: "Tgl Inquiry", width: 120, type: "date" },
//                { name: "TipeKendaraan", text: "Tipe", width: 200 },
//                { name: "Variant", text: "Variant", width: 100 },
//                { name: "PerolehanDataDesc", text: "Perolehan Data", width: 200 },
//                { name: "LostDate", text: "Tgl Lost", width: 120, type: "date" },
//                { name: "LostCaseCategoryDesc", text: "Kategori", width: 300 },
//                { name: "LostCaseOtherReason", text: "Alasan", width: 300 },
//                { name: "LostCaseVoiceOfCustomer", text: "Voice of Customer", width: 300 },
//            ]
//        });
//    }

//    function getSqlDate(value) {
//        return moment(value, "DD-MMM-YYYY").format("YYYYMMDD");
//    }
//});