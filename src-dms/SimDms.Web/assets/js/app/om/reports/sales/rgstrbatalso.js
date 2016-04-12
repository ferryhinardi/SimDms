"use strict"; //Reportid OmRpSalesRgs001
function RptRegisterBatalSalesOrder($scope, $http, $injector) {

    var me = $scope;

    $injector.invoke(BaseController, this, { $scope: me });

    me.SONo = function () {
        var lookup = Wx.blookup({
            name: "SoLookup",
            title: "SO",
            manager: spSalesManager,
            query: "SOLookup",
            defaultSort: "SONo asc",
            columns: [
                { field: "SONo", title: "SO No" },
                { field: "SODate", title: "SO Date" },
            ],
        });
        lookup.dblClick(function (data) {
            if (data != null) {
                me.data.SONo = data.SONo;
                me.Apply();
            }
        });

    }

    me.SONoTo = function () {
        var lookup = Wx.blookup({
            name: "SoLookup",
            title: "SO",
            manager: spSalesManager,
            query: "SOLookup",
            defaultSort: "SONo asc",
            columns: [
                { field: "SONo", title: "SO No" },
                { field: "SODate", title: "SO Date" },
            ],
        });
        lookup.dblClick(function (data) {
            if (data != null) {
                me.data.SONoTo = data.SONo;
                me.Apply();
            }
        });

    }

    me.printPreview = function () {
        $http.post('om.api/ReportSales/ValidatePrintSO', me.data)
           .success(function (e) {
               if (e.success) {
                   var param = "";
                   if (me.data.isActiveDate == true && me.data.isNoSO == true)
                   { param = "1"; }
                   else if (me.data.isActiveDate == true && me.data.isNoSO == false)
                   { param = "2"; }
                   else if (me.data.isActiveDate == false && me.data.isNoSO == true)
                   { param = "3"; }
                   else if (me.data.isActiveDate == false && me.data.isNoSO == false)
                   { param = "4"; }

                   var ReportId = "OmRpSalesRgs006";
                   var par = [
                       moment(me.data.DateFrom).format('YYYYMMDD'),
                       moment(me.data.DateTo).format('YYYYMMDD'),
                       me.data.SONo,
                       me.data.SONoTo,
                       param
                   ]
                   var rparam = 'DARI TANGGAL : ' + moment(me.data.DateFrom).format('DD-MMM-YYYY') + ' S/D ' + moment(me.data.DateTo).format('DD-MMM-YYYY')

                   Wx.showPdfReport({
                       id: ReportId,
                       pparam: par.join(','),
                       textprint: true,
                       rparam: rparam,
                       type: "devex"
                   });
               } else {
                   MsgBox(e.message, MSG_ERROR);
                   return;
               }
           })
           .error(function (e) {
               MsgBox('Terjadi Kesalahan, Hubungi SDMS Support', MSG_ERROR);
           });

    }

    $("[name = 'isActiveType']").on('change', function () {
        me.data.isActiveType = $('#isActiveType').prop('checked');
        me.data.Type = "";
        me.Apply();
    });

    $("[name = 'isNoSO']").on('change', function () {
        me.data.isNoSO = $('#isNoSO').prop('checked');
        me.data.SONo = "";
        me.data.SONoTo = "";
        me.Apply();
    });

    me.initialize = function () {

        $('#isActiveDate').prop('checked', true);
        me.data.isActiveDate = true;
        $('#isNoSO').prop('checked', false);
        me.data.isNoSO = false;
        $http.get('breeze/sales/CurrentUserInfo').
          success(function (dl, status, headers, config) {
              me.data.CompanyCode = dl.CompanyCode;
              me.data.BranchCode = dl.BranchCode;
          });
        $http.get('breeze/sales/ProfitCenter').
        success(function (dl, status, headers, config) {
            me.data.ProfitCenterCode = dl.ProfitCenter;
        });
        $http.post('om.api/ReportSales/Periode').
          success(function (e) {
              me.data.DateFrom = e.DateFrom;
              me.data.DateTo = e.DateTo;
          });
        me.Apply();

    }

    me.start();
}

$(document).ready(function () {
    var options = {
        title: "Report Register Pembatalan Sales Order",
        xtype: "panels",
        toolbars: [
            { name: "btnPrintPreview", text: "Print", cls: "btn btn-primary", icon: "icon-print", click: "printPreview()", disable: "!isPrintEnable" },
            { name: "btnCancel", text: "Cancel", cls: "btn btn-warning", icon: "icon-remove", click: "cancelOrClose()" },
        ],
        panels: [
            {
                name: "pnlA",
                items: [
                    { name: "CompanyCode", model: "data.CompanyCode", text: "Kode Perusahaan", cls: "span4 full", disable: "isPrintAvailable", show: false },
                    { name: "BranchCode", model: "data.BranchCode", text: "Kode Cabang", cls: "span4 full", disable: "isPrintAvailable", show: false },
                    { name: "ProfitCenterCode", model: "data.ProfitCenterCode", text: "Kode Cabang", cls: "span4 full", disable: "isPrintAvailable", show: false },
                    {
                        text: "Tanggal",
                        type: "controls",
                        items: [
                                { name: 'isActiveDate', type: 'check', cls: "span1", text: "", float: 'left' },
                                { name: "DateFrom", text: "", cls: "span2", type: "ng-datepicker", disable: "data.isActiveDate == false" },
                                { name: "DateTo", text: "", cls: "span2", type: "ng-datepicker", disable: "data.isActiveDate == false" },
                        ]
                    },
                    {
                        text: "No SO",
                        type: "controls",
                        items: [
                                { name: 'isNoSO', type: 'check', cls: "span1", text: "", float: 'left' },
                                { name: "SONo", cls: "span2", type: "popup", btnName: "btnKaroseriSPKNo", click: "SONo()", disable: "data.isNoSO == false" },
                                { name: "SONoTo", cls: "span2", type: "popup", btnName: "btnKaroseriSPKNoTo", click: "SONoTo()", disable: "data.isNoSO == false" },
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
        $(".mylabel").attr("style", "padding:9px 9px 0px 5px");
        SimDms.Angular("RptRegisterBatalSalesOrder");
    }



});