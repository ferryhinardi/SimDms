"use strict";
var isCEO = false;
var empID = "";
var pType = "";
var CompanyCode = "";
var BranchCode = "";

function ITSSalesAchievement($scope, $http, $injector) {

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
        me.NikBM = {};
        me.NikSH = {};
        me.NikSC = {};
        me.NikSL = {};
        me.data.Year = moment(me.now()).format('YYYY');
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
                    if (pType == '4W') {
                        me.data.NikBM = data.data.NikBM;
                        me.data.NikSH = data.data.NikSH;
                        $http.post('its.api/Combo/getSalesmanByPos?emp=' + me.data.NikSH + '&pos=S').
                        success(function (data, status, headers, config) {
                            me.NikSL = data;
                        });
                        Wx.enable({ value: false, items: ["NikBM", "NikSH"] });
                    } else {
                        me.data.NikBM = data.data.NikBM;
                        me.data.NikSH = data.data.NikSC;
                        $http.post('its.api/Combo/getSalesmanByPos?emp=' + me.data.NikSC + '&pos=S').
                        success(function (data, status, headers, config) {
                            me.NikSL = data;
                        });
                        Wx.enable({ value: false, items: ["NikBM", "NikSC"] });
                    }
                }
                else if (data.data.Position == "BM") {
                    me.data.NikBM = data.data.NikBM;
                    me.comboSH = data.data.EmpSHList;
                    //$http.post('its.api/Combo/getSalesmanByPos?emp=' + me.data.NikBM + '&pos=SH').
                    //success(function (data, status, headers, config) {
                    //    me.NikSH = data;
                    //    me.NikSC = data;
                    //});
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
                    me.comboSH = data.data.EmpSHList;
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
                    me.data.NikSL = "";
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
            $http.post('its.api/Combo/SalesHead?EmployeeID=' + me.data.NikBM).
                    success(function (dt, status, headers, config) {
                        me.NikSH = dt.list;
                        me.NikSL = dt.listSM;
                    });
            me.data.NikSH = "";
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
            $http.post('its.api/Combo/SalesHead?EmployeeID=' + me.data.NikBM).
                    success(function (dt, status, headers, config) {
                        me.NikSC = dt.list;
                        me.NikSL = dt.listSM;
                    });
            me.data.cmbSM = "";
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
                $http.post('its.api/Combo/SalesHead?EmployeeID=' + me.data.NikBM).
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

    $http.post('its.api/Combo/Years').
    success(function (data, status, headers, config) {
        me.Year = data;
    });

    me.printPreview = function () {
        var bm, sh, spv, emp;
        var param1, param2, param3, param4;

        if (me.data.Year == undefined) {
            MsgBox('Ada informasi yang belum lengkap!', MSG_ERROR);
        } else {

            if (me.data.NikBM == '') {
                bm = "";
                param1 = "SEMUA";
            } else {
                bm = me.data.NikBM
                param1 = $("[name=NikBM]").select2("data").text;
            }

            if (me.data.NikSH == '') {
                sh = "";
                spv = me.data.NikSC;
                //param2 = pType == '4W' ? "SEMUA" : $("[name=NikSC]").select2("data").text;
            }
            else {
                sh = me.data.NikSH;
                spv = "";
                //param2 = pType == '4W' ? $("[name=NikSH]").select2("data").text : "SEMUA";
            }

            var param2 = pType == '4W' ? me.data.NikSH == "" ? "SEMUA" : $("[name=NikSH]").select2("data").text : me.data.NikSC == "" ? "SEMUA" : $("[name=NikSC]").select2("data").text
            var param3 = "-";
            //if (me.data.NikSC == '') {
            //    spv = "";
            //    if (pType == '2W') { param3 = "SEMUA"; }else{ param3 = "-"}
            //}
            //else {
            //    spv = me.data.NikSC;
            //    param3 = $("[name=NikSC]").select2("data").text;
            //}

            if (me.data.NikSL == '') {
                emp = "";
                param4 = "SEMUA";
            }
            else {
                emp = me.data.NikSL;
                param4 = $("[name=NikSL]").select2("data").text;
            }

            var param5 = pType == '4W' ? "Nama Sales Head" : "Nama Sales Kordinator";

            param1 = param1.replace(',', '.');
            param2 = param2.replace(',', '.');
            param4 = param4.replace(',', '.');

            var prm = [
                bm,
                sh,
                spv,
                emp,
                me.data.Year,
            ];
            console.log(prm);
            var rparam = me.data.Year + "," + param1 + "," + param2 + "," + param3 + "," + param4 + "," + param5;
            Wx.showPdfReport({
                id: "PmRpInqSalesAchievementWeb",
                pparam: prm.join(','),
                rparam: rparam,
                type: "devex"
            });
        }
    }

    me.start();
}


$(document).ready(function () {
    var options = {
        title: "Inquiry - Sales Achievement",
        xtype: "panels",
        toolbars: [
            { name: "btnPrintPreview", text: "Print", cls: "btn btn-primary", icon: "icon-print", click: "printPreview()", disable: "!isPrintEnable" },
        ],
        panels: [
            {
                name: "pnlLostCase",
                title: "Data Sales Achievement",
                items: [
                   { name: "Year", cls: "span4", type: "select2", datasource: "Year", text: "Year", required: true, validasi: "required" },
                   { name: "NikBM", text: "Brach Manager", cls: "span6", type: "select2", datasource: "NikBM" },
                   { name: "NikSH", text: "Sales Head", cls: "span6", type: "select2", datasource: "NikSH", show: "isComboShow" },
                   { name: "NikSC", text: "Sales Koordinator", cls: "span6", type: "select2", datasource: "NikSC", show: "!isComboShow" },
                   { name: "NikSL", text: "Salesman", cls: "span6", type: "select2", datasource: "NikSL", disable: "me.data.NikSC == ''" },
                ]
            },
        ]
    };

    Wx = new SimDms.Widget(options);
    Wx.default = {};
    Wx.render(init);

    function init(s) {
        $(".mylabel").attr("style", "padding:9px 9px 0px 5px");
        SimDms.Angular("ITSSalesAchievement");
    }

});
