"use strict"

function generatekdp($scope, $http, $injector) {

    var me = $scope;

    $injector.invoke(BaseController, this, { $scope: me });

    me.btnExportExcel = function () {
        window.location.href = 'its.api/Inquiry/GenerateITSWithStatusAndTestDrive?StartDate=' + $('[name=DateFrom]').val() + '&EndDate=' + $('[name=DateTo]').val();
    }

    me.$watch('data.DateFrom', function (newValue, oldValue) {
        if (me.data.DateFrom > me.data.DateTo) {
            me.data.DateTo = me.data.DateFrom;
        }
    }, true);

    me.$watch('data.DateTo', function (newValue, oldValue) {
        if (me.data.DateTo < me.data.DateFrom) {
            me.data.DateFrom = me.data.DateTo;
        }
    }, true);

    me.initialize = function () {
        me.data.DateFrom = new Date();
        me.data.DateTo = new Date();
    }

    me.start();
}

$(document).ready(function () {
    var options = {
        title: "Generate ITS With Status and Test Drive",
        xtype: "panels",
        toolbars: [
            { name: "btnExportExcel", text: "Generate Excel", icon: "fa fa-file-excel-o", cls: "small", click: "btnExportExcel()" },
        ],
        panels: [
            {
                items: [
                    {
                        text: "Periode",
                        type: "controls",
                        items: [
                            { name: "DateFrom", type: "ng-datepicker", cls: "span1" },
                            { type: "label", text: "s/d", cls: "span1", style: "line-height: 33px" },
                            { name: "DateTo", text: "S/D", type: "ng-datepicker", cls: "span1" },
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
        SimDms.Angular("generatekdp");
    }
});

//$(document).ready(function () {
//    var options = {
//        title: "Generate ITS With Status and Test Drive",
//        xtype: "panels",
//        toolbars: [
//            { name: "btnExportExcel", text: "Generate Excel", icon: "fa fa-file-excel-o", cls: "small", click: "Generate()" },
//        ],
//        panels: [
//            {
//                name: "panelFilter",
//                items: [
//                    {
//                        text: "Date (From - To)",
//                        cls: "span6",
//                        type: "controls",
//                        items: [
//                            { name: "StartDate", text: "Date From", cls: "span4", type: "datepicker" },
//                            { name: "EndDate", text: "Date To", cls: "span4", type: "datepicker" },
//                        ]
//                    },
//                ]
//            }
//        ]
//    };

//    var widget = new SimDms.Widget(options);
//    widget.render(renderCallback);

//    function renderCallback() {
//        initElementEvents();
//    }

//    function initElementEvents() {
//        var btnExportExcel = $('#btnExportExcel');

//        btnExportExcel.off();
//        btnExportExcel.on('click', function () {
//            window.location.href = 'its.api/Inquiry/GenerateITSWithStatusAndTestDrive?StartDate=' + $('[name=StartDate]').val() + '&EndDate=' + $('[name=EndDate]').val();
//        });
//    }
//});