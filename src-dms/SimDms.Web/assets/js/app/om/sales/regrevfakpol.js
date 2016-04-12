"use strict"

function regrevfakpol($scope, $http, $injector) {

    var me = $scope;

    $injector.invoke(BaseController, this, { $scope: me });

    me.initialize = function () {
        $('#cmbDealer').removeAttr('disabled');
        $('#cmbOutlet').removeAttr('disabled');

        me.getUserProfile();
        me.getDealer();
        me.getOutlets();
    }

    me.getUserProfile = function () {
        $.ajax({
            async: false,
            type: "POST",
            url: 'om.api/RegisterRevisiFakPol/GetUserProfile',
            success: function (data) {
                if (data.message == "") {
                    if (!data.isHolding) {
                        $('#cmbOutlet').attr('disabled', 'disabled');
                    }
                    me.data.dtpFrom = data.today;
                    me.data.dtpTo = data.today;
                }
            }
        });
    }

    me.getDealer = function () {
        $.ajax({
            async: false,
            type: "POST",
            url: 'om.api/RegisterRevisiFakPol/GetDealer',
            success: function (data) {
                if (data.message == "") {
                    me.comboDealer = data.dealers;
                    me.data.cmbDealer = data.dealerCode;
                }
            }
        });
    }

    me.getOutlets = function () {
        $http.post('om.api/RegisterRevisiFakPol/GetOutlets').success(function (data) {
            if (data.message == '') {
                me.comboOutlet = data.outlets;
                me.data.cmbOutlet = data.outletCode;
            } else {
                MsgBox(data.message);
            }
        });
    }

    me.getReport = function () {
        $.ajax({
            async: false,
            type: "POST",
            data: {
                CompanyCode: me.data.cmbDealer,
                BranchCode: me.data.cmbOutlet,
                DateFrom: moment(me.data.dtpFrom).format("YYYYMMDD"),
                DateTo: moment(me.data.dtpTo).format("YYYYMMDD")
            },
            url: 'om.api/RegisterRevisiFakPol/GetReport',
            success: function (data) {
                if (data.message == "") {
                    location.href = 'om.api/RegisterRevisiFakPol/DownloadExcelFile?key=' + data.value;
                } else {
                    MsgBox("Error: " + data.message + "\r\n" + "Inner Exception: " + data.inner);
                    return;
                }
            }
        });


    }

    me.start();
}

$(document).ready(function () {
    var options = {
        title: "Register Revisi Faktur Polisi",
        xtype: "panels",
        //xtype: "report",
        toolbars: [
            { name: "btnProcess", text: "Process", icon: "icon-bolt", click: "getReport()" },
        ],
        panels: [
            {
                items: [
                    {
                        text: "Dealer",
                        cls: "span5",
                        type: "controls", items: [
                            { name: "cmbDealer", type: "select2", opt_text: "[SELECT ALL]", datasource: "comboDealer" }
                        ]
                    },
                    {
                        text: "Branch/Outlet",
                        cls: "span5",
                        type: "controls", items: [
                            { name: "cmbOutlet", type: "select2", opt_text: "[SELECT ALL]", datasource: "comboOutlet" }
                        ]
                    },
                    {
                        text: "Date (From - To)",
                        cls: "span5",
                        type: "controls", items: [
                            { name: "dtpFrom", text: "Date From", cls: "span4", type: "ng-datepicker" },
                            { name: "dtpTo", text: "Date To", cls: "span4", type: "ng-datepicker" },
                        ]
                    }
                ]
            }
        ]
    }

    Wx = new SimDms.Widget(options);
    Wx.default = {};
    Wx.render(init);

    function init(s) {
        SimDms.Angular("regrevfakpol");
    }
});