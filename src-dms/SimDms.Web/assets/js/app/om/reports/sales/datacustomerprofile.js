"use strict";
var baseOnOrder = "0";
var baseOnFormat = "0";

function RptDataCustomerProfile($scope, $http, $injector) {

    var me = $scope;

    $injector.invoke(BaseController, this, { $scope: me });

    me.$watch('optionByOrder', function (newValue, oldValue) {
        me.init();
        if (newValue !== oldValue) {
            me.$broadcast(newValue);
            baseOnOrder = newValue;
            console.log(baseOnOrder);
        }
    });

    me.$watch('optionByFormat', function (newValue, oldValue) {
        me.init();
        if (newValue !== oldValue) {
            me.$broadcast(newValue);
            baseOnFormat = newValue;
            console.log(baseOnFormat);
        }
    });

    me.printPreview = function () {

        if (me.data.PeriodDateStart > me.data.PeriodDateTo) {
            MsgBox('Periode Awal tidak boleh lebih besar dari periode akhir', MSG_ERROR);
            return;
        }

        if (baseOnFormat=="0") {
            var param = [
            moment(me.data.PeriodDateStart).format('DD MMM YYYY'),
            moment(me.data.PeriodDateTo).format('DD MMM YYYY'),
            baseOnOrder

            ];
            Wx.showPdfReport({
                id: "OmRpSalRgs034",
                pparam: param.join(','),
                //textprint: true,
                rparam: "PER : " + moment(me.data.PeriodDateStart).format('DD MMM YYYY') + " S/D " + moment(me.data.PeriodDateTo).format('DD MMM YYYY'),
                type: "devex"
            });
        }
        else {
            var reportID = "OmRpSalRgs034";
            var startDate = moment(me.data.PeriodDateStart).format('YYYYMMDD');
            var toDate = moment(me.data.PeriodDateTo).format('YYYYMMDD');
            var from = moment(me.data.PeriodDateStart).format('DD MMM YYYY');
            var to = moment(me.data.PeriodDateTo).format('DD MMM YYYY');
            var order = baseOnOrder;

            var url = "om.api/Report/DataCustomerProfile?";
            var params = "&CompanyCode=" + me.data.CompanyCode;
            params += "&BranchCode=" + me.data.BranchCode;
            params += "&StartDate=" + startDate;
            params += "&ToDate=" + toDate;
            params += "&Order=" + order;
            params += "&ReportID=" + reportID;
            params += "&From=" + from;
            params += "&To=" + to;
            url = url + params;
            window.location = url;
        }
    }

    me.initialize = function () {
        me.data = {};
        me.change = false;
        var d = new Date(Date.now()).getDate();
        var m = new Date(Date.now()).getMonth();
        var y = new Date(Date.now()).getFullYear();
        me.data.PeriodDateStart = new Date(y, m, 1);
        me.data.PeriodDateTo = new Date(y, m, d);
        $http.get('breeze/sales/CurrentUserInfo').
          success(function (dl, status, headers, config) {
              me.data.CompanyCode = dl.CompanyCode;
              me.data.BranchCode = dl.BranchCode;
          });

        me.isPrintAvailable = true;
    }

    me.optionByOrder = "0";
    me.optionByFormat = "0";

    me.start();

}

$(document).ready(function () {
    var options = {
        title: "Report Data Customer Profile",
        xtype: "panels",
        toolbars: [
            { name: "btnPrintPreview", text: "Print", cls: "btn btn-primary", icon: "icon-print", click: "printPreview()", disable: "!isPrintEnable" },
            { name: "btnCancel", text: "Cancel", cls: "btn btn-warning", icon: "icon-remove", click: "cancelOrClose()" },
        ],
        panels: [
            {
                name: "pnlA",
                items: [
                        { name: "CompanyCode", model: "data.CompanyCode", text: "Kode Perusahaan", cls: "span2 full", disable: "isPrintAvailable", show: false },
                        { name: "BranchCode", model: "data.BranchCode", text: "Kode Cabang", cls: "span3 full", disable: "isPrintAvailable", show: false },

                        {
                            text: "Periode",
                            type: "controls",
                            cls: "span4 full",
                            items: [
                                { name: "PeriodDateStart", model: "data.PeriodDateStart", placeHolder: "Tgl Periode Awal", cls: "span4", type: 'ng-datepicker' },
                            ]
                        },
                        {
                            text: "S/D",
                            type: "controls",
                            cls: "span4 full",
                            items: [
                                { name: "PeriodDateTo", model: "data.PeriodDateTo", placeHolder: "Tgl Periode Akhir", cls: "span4", type: 'ng-datepicker' },
                            ]
                        },

                        {
                            text: "Order by",
                            type: "controls",
                            items: [
                                {
                                    type: "optionbuttons",
                                    name: "tabpageoptions",
                                    model: "optionByOrder",
                                    items: [
                                        { name: "0", text: "Nama Customer" },
                                        { name: "1", text: "Tanggal Invoice" },
                                    ]
                                },
                            ]
                        },

                        {
                            text: "Format",
                            type: "controls",
                            items: [
                                {
                                    type: "optionbuttons",
                                    name: "tabpageoptions",
                                    model: "optionByFormat",
                                    items: [
                                        { name: "0", text: "Viewer" },
                                        { name: "1", text: "Excell" },
                                    ]
                                },
                            ]
                        },

                ]
            }
        ]
    };

    Wx = new SimDms.Widget(options);
    Wx.default = {};
    Wx.render(init);



    function init(s) {
        SimDms.Angular("RptDataCustomerProfile");

    }
});