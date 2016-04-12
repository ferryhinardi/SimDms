var totalSrvAmt = 0;
var status = 'N';
var svType = '2';

"use strict";

function spItemPriceController($scope, $http, $injector) {

    var me = $scope;
    $injector.invoke(BaseController, this, { $scope: me });
   

    me.Fiscalstate = [
        { "value": "0", "text": "Non Active" },
        { "value": "1", "text": "Active" }
    ];


    me.browse = function () {
        var lookup = Wx.blookup({
            name: "PeriodeLookUp",
            title: "Lookup Periode",
            manager: gnManager,
            query: "periodes",
            defaultSort: "FiscalYear asc",
            columns: [
                { field: "FiscalYear", title: "Fiscal Year" },
                { field: "FiscalMonth", title: "Fiscal Month" },
                { field: "PeriodeNum", title: "Periode No." },
                { field: "PeriodeName", title: "Periode" },
                { field: "FromDate", title: "From Date", template: "#= (FromDate == undefined) ? '' : moment(FromDate).format('DD MMM YYYY') #" },
                { field: "EndDate", title: "End Date", template: "#= (EndDate == undefined) ? '' : moment(EndDate).format('DD MMM YYYY') #" },
                { field: "StatusSparepart", title: "Sparepart" },
                { field: "StatusSales", title: "Sales" },
                { field: "StatusService", title: "Service" },
                { field: "StatusFinanceAP", title: "AP" },
                { field: "StatusFinanceAR", title: "AR" },
                { field: "StatusFinanceGL", title: "GL" },
                { field: "FiscalStatus", title: "Status" }
            ]
        });
        lookup.dblClick(function (data) {
            if (data != null) {
                me.lookupAfterSelect(data, false);
                if (data.FiscalStatus == "Active") { me.data.FiscalStatus = 1 }
                else { me.data.FiscalStatus = 0 }
                me.isSave = false;
                me.Apply();
            }
        });
    };
    me.cancelOrClose = function () {
        me.init();
    };
    me.initialize = function () {
        $http.post('gn.api/Periode/lastperiode').
            success(function (data, status, headers, config) {
                if (data.success) {
                    me.data = data.data;
                } else {
                    MsgBox(data.message, MSG_ERROR);
                }
            }).
            error(function (data, status, headers, config) {
                MsgBox('Terjadi Kesalahan, Hubungi SDMS Support');
            });
        $("#FiscalYear").css("text-align", "right");
        $("#PeriodeNum").css("text-align", "right");
        $("#FiscalMonth").css("text-align", "right");
        me.Apply();
    }

    me.delete = function () {
        MsgConfirm("Are you sure to delete current record?", function (result) {
            $http.post('gn.api/Periode/Delete', me.data).
            success(function (data, status, headers, config) {
                if (data.success) {
                    me.init();
                    Wx.Success("Data Period deleted...");
                } else {
                    MsgBox(data.message, MSG_ERROR);
                }
            }).
            error(function (data, status, headers, config) {
                MsgBox('Terjadi Kesalahan, Hubungi SDMS Support');
            });
        });
    }

    me.save = function (e, param) {
        console.log(me.data.FiscalStatus);
        if (me.data.FiscalStatus == "0") { me.data.FiscalStatus = false } else { me.data.FiscalStatus = true }
        console.log(me.data.FiscalStatus);
        $http.post('gn.api/Periode/Save', me.data).
            success(function (data, status, headers, config) {
                if (data.status) {
                    Wx.Success("Data saved...");
                    if (me.data.FiscalStatus == false) { me.data.FiscalStatus = "0" } else { me.data.FiscalStatus = "1" }
                    me.startEditing();
                } else {
                    MsgBox(data.message, MSG_ERROR);
                }
            }).
            error(function (data, status, headers, config) {
                MsgBox('Terjadi Kesalahan, Hubungi SDMS Support');
            });
    };

    me.start();
   
}
$(document).ready(function () {
    var options = {
        title: "Periode",
        xtype: "panels",
        toolbars :WxButtons,
        panels: [
            {
                name: "pnlInfo",
                title: "",
                items: [
                        { name: "FiscalYear", model: "data.FiscalYear", text: "Fiscal Year", cls: "span4 full", required: true, validasi: "required" },
                        { name: "PeriodeNum", model: "data.PeriodeNum", text: "Periode No.", cls: "span4 full", readonly: true },
                        { name: "FromDate", type: "ng-datepicker", text: "From Date", cls: "span4 full" },
                        { name: "EndDate", type: "ng-datepicker", text: "End Date", cls: "span4 full" },
                        { name: 'PeriodeName', model: 'data.PeriodeName', type: 'text', text: 'Periode', cls: 'span4 full', readonly: true },
                        { name: 'FiscalMonth', model: 'data.FiscalMonth', type: 'text', text: 'Fiscal Month', cls: 'span4 full', readonly: true },
                        { name: "FiscalStatus", model: "data.FiscalStatus", type: "select2", text: "Status", cls: "span4 full", datasource: "Fiscalstate" }
                ]
            }

        ]
    };
    Wx = new SimDms.Widget(options);
    Wx.default = {};
    Wx.render(init);

    function init() {
        SimDms.Angular("spItemPriceController");
    }
});