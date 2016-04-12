"use strict"

function sfminvalidemployee($scope, $http, $injector) {

    var me = $scope;

    $injector.invoke(BaseController, this, { $scope: me });

    me.InitComboArea = function () {
        $.ajax({
            async: false,
            type: "POST",
            url: 'ab.api/InvalidEmployee/Areas',
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
            url: 'ab.api/InvalidEmployee/Dealers',
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
            url: 'ab.api/InvalidEmployee/Outlets',
            success: function (data) {
                me.Outlets = data;
            }
        });
    }

    me.InitComboStatus = function () {
        $.ajax({
            async: false,
            type: "POST",
            url: 'ab.api/InvalidEmployee/PersonnelStatus',
            success: function (data) {
                me.Statuses = data;
            }
        });
    }

    me.InitComboCase = function () {
        $.ajax({
            async: false,
            type: "POST",
            url: 'ab.api/InvalidEmployee/Cases',
            success: function (data) {
                me.Cases = data;
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
        if ($('#Area').val() == '') {
            sdms.info("Area harus dipilih", "Warning");
            return;
        }

        $('#btnPrintPreview').attr('disabled', 'disabled');
        sdms.info("Please wait...");
        $.ajax({
            async: true,
            type: "POST",
            data: {
                dealer: $('#Dealer').val(),
                outlet: $('#Outlet').val(),
                status: $('#PersonnelStatus').val(),
                caseNo: $('#Case').val()
            },
            url: 'ab.api/InvalidEmployee/Query',
            success: function (data) {
                if (data.message == "") {
                    location.href = 'ab.api/InvalidEmployee/DownloadExcelFile?key=' + data.value;
                } else {
                    sdms.info(data.message, "Error");
                }
                $('#btnPrintPreview').removeAttr('disabled');
            }
        });
    }

    me.initialize = function () {
        me.data = [];
        $('#Dealer').attr('disabled', 'disabled');
        $('#Outlet').attr('disabled', 'disabled');
        me.InitComboArea();
        me.InitComboDealer();
        me.InitComboOutlet();
        me.InitComboStatus();
        me.InitComboCase();
    }

    me.start();
}

$(document).ready(function () {
    var options = {
        title: "Invalid Employee",
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
                        text: "Status",
                        type: "controls",
                        items: [
                            { name: "PersonnelStatus", type: "select2", opt_text: "-- SELECT ALL --", cls: "span4", datasource: "Statuses" },
                        ]
                    },
                    {
                        text: "Case",
                        type: "controls",
                        items: [
                            { name: "Case", type: "select2", opt_text: "-- SELECT ALL --", cls: "span6", datasource: "Cases" }
                        ]
                    }
                ]
            },
        ]
    };

    Wx = new SimDms.Widget(options);
    Wx.default = {};
    Wx.render(init);

    function init(s) {
        SimDms.Angular("sfminvalidemployee");
    }
});