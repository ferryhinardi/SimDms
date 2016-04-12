"use strict"

function sfmturnoverratio($scope, $http, $injector) {

    var me = $scope;

    $injector.invoke(BaseController, this, { $scope: me });

    me.InitComboYear = function () {
        $.ajax({
            async: false,
            type: "POST",
            url: 'ab.api/TurnOverRatio/Years',
            success: function (data) {
                me.Years = data;
                me.data.StartYear = new Date().getFullYear();
                me.data.EndYear = new Date().getFullYear();
            }
        });
    }

    me.InitComboMonth = function () {
        $.ajax({
            async: false,
            type: "POST",
            url: 'ab.api/TurnOverRatio/Months',
            success: function (data) {
                me.Months = data;
                me.data.StartMonth = new Date().getMonth() + 1;
                me.data.EndMonth = new Date().getMonth() + 1;
            }
        });
    }
    
    me.InitComboArea = function () {
        $.ajax({
            async: false,
            type: "POST",
            url: 'ab.api/TurnOverRatio/Areas',
            success: function (data) {
                me.Areas = data;
                if (data.length == 1) me.data.Area = data[0].value;
            }
        });
    }

    me.InitComboDealer = function () {
        $.ajax({
            async: false,
            type: "POST",
            data: {
                area: me.data.Area
            },
            url: 'ab.api/TurnOverRatio/Dealers',
            success: function (data) {
                me.Dealers = data;
                if (data.length == 1) me.data.Dealer = data[0].value;
            }
        });
    }

    me.InitComboOutlet = function () {
        $.ajax({
            async: false,
            type: "POST",
            data: {
                area: me.data.Area,
                dealer: me.data.Dealer
            },
            url: 'ab.api/TurnOverRatio/Outlets',
            success: function (data) {
                me.Outlets = data;
            }
        });
    }

    me.InitComboSalesForce = function () {
        $.ajax({
            async: false,
            type: "POST",
            url: "ab.api/TurnOverRatio/SalesForces",
            success: function (data) {
                me.SalesForces = data;
            }
        });
    }

    $('#Area').on('change', function () {
        if ($('#Area').val() != '') $('#Dealer').removeAttr('disabled');
        else {
            $('#Dealer').attr('disabled', 'disabled');
            me.data.Dealer = '';
            $('#Outlet').attr('disabled', 'disabled');
            me.data.Outlet = '';
        }
    });
    
    $('#Dealer').on('change', function () {
        if ($('#Dealer').val() != '') $('#Outlet').removeAttr('disabled');
        else {
            $('#Outlet').attr('disabled', 'disabled');
            me.data.Outlet = '';
        }
    });

    me.printPreview = function () {
        if (me.data.StartYear == '' || me.data.StartMonth == '' || me.data.EndYear == '' || me.data.EndMonth == '') {
            MsgBox("Periode harus diisi");
            return;
        }

        var startPeriod = new Date(me.data.StartYear, me.data.StartMonth);
        var endPeriod = new Date(me.data.EndYear, me.data.EndMonth);
        if (startPeriod > endPeriod) {
            MsgBox("Periode akhir harus lebih besar dari periode awal");
            return;
        }

        $.ajax({
            async: false,
            type: "POST",
            data: {
                area: me.data.Area,
                dealer: me.data.Dealer,
                outlet: me.data.Outlet,
                startYear: me.data.StartYear,
                startMonth: me.data.StartMonth,
                endYear: me.data.EndYear,
                endMonth: me.data.EndMonth,
                position: me.data.SalesForce
            },
            url: 'ab.api/TurnOverRatio/Query',
            success: function (data) {
                if (data.message == "") {
                    location.href = 'ab.api/TurnOverRatio/DownloadExcelFile?key=' + data.value;
                }
            }
        });
    }

    me.initialize = function () {
        me.data = [];
        $('#Dealer').attr('disabled', 'disabled');
        $('#Outlet').attr('disabled', 'disabled');
        me.InitComboYear();
        me.InitComboMonth();
        me.InitComboArea();
        me.InitComboDealer();
        me.InitComboOutlet();
        me.InitComboSalesForce();
    }

    me.start();
}

$(document).ready(function () {
    var options = {
        title: "Turn Over Ratio",
        xtype: "panels",
        toolbars: [
            { name: "btnPrintPreview", text: "Print", cls: "btn btn-primary", icon: "icon-print", click: "printPreview()" },
        ],
        panels: [
            {
                items: [
                    {
                        text: "Area",
                        type: "controls",
                        items: [
                            { name: "Area", type: "select2", opt_text: "-- SELECT ALL --", cls: "span4", datasource: "Areas" }
                        ]
                    },
                    {
                        text: "Dealer",
                        type: "controls",
                        items: [
                            { name: "Dealer", type: "select2", opt_text: "-- SELECT ALL --", cls: "span4", datasource: "Dealers" }
                        ]
                    },
                    {
                        text: "Outlet",
                        type: "controls",
                        items: [
                            { name: "Outlet", type: "select2", opt_text: "-- SELECT ALL --", cls: "span4", datasource: "Outlets" }
                        ]
                    },
                    {
                        text: "Sales Force",
                        type: "controls",
                        items: [
                            { name: "SalesForce", type: "select2", opt_text: "-- SELECT ALL --", cls: "span4", datasource: "SalesForces" }
                        ]
                    },
                    {
                        text: "Periode Awal",
                        type: "controls",
                        items: [
                            { name: "StartYear", type: "select2", cls: "span2", datasource: "Years" },
                            { name: "StartMonth", type: "select2", cls: "span2", datasource: "Months" }
                        ]
                    },
                    {
                        text: "Periode Akhir",
                        type: "controls",
                        items: [
                            { name: "EndYear", type: "select2", cls: "span2", datasource: "Years" },
                            { name: "EndMonth", type: "select2", cls: "span2", datasource: "Months" },
                        ]
                    },
                ]
            },
        ]
    };

    Wx = new SimDms.Widget(options);
    Wx.default = {};
    Wx.render(init);

    function init(s) {
        SimDms.Angular("sfmturnoverratio");
    }
});