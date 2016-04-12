"use strict"; //Reportid OmRpSalesRgs001
function RptRegisterPelangganPerleasing($scope, $http, $injector) {

    var me = $scope;

    $injector.invoke(BaseController, this, { $scope: me });

    me.FakturPolisiNo = function () {
        var lookup = Wx.blookup({
            name: "FakturLookup4Report",
            title: "Faktur Polis",
            manager: spSalesManager,
            query: "FakturLookup4Report",
            defaultSort: "FakturPolisiNo asc",
            columns: [
                { field: "FakturPolisiNo", title: "No Faktur" },
                {
                    field: "FakturPolisiDate", title: "Sampai Tanggal",
                    template: "#= moment(FakturPolisiDate).format('DD MMM YYYY') #"
                },
                { field: "IsBlanko", title: "Blanko" },
                { field: "Status", title: "Status" },
            ],
        });
        lookup.dblClick(function (data) {
            if (data != null) {
                me.data.FakturPolisiNo = data.FakturPolisiNo;
                me.data.FakturPolisiDate = data.FakturPolisiDate;
                me.Apply();
            }
        });

    }

    me.FakturPolisiNoTo = function () {
        var lookup = Wx.blookup({
            name: "FakturLookup4Report",
            title: "Faktur Polis",
            manager: spSalesManager,
            query: "FakturLookup4Report",
            defaultSort: "FakturPolisiNo asc",
            columns: [
                { field: "FakturPolisiNo", title: "No Faktur" },
                {
                    field: "FakturPolisiDate", title: "Sampai Tanggal",
                    template: "#= moment(FakturPolisiDate).format('DD MMM YYYY') #"
                },
                { field: "IsBlanko", title: "Blanko" },
                { field: "Status", title: "Status" },
            ],
        });
        lookup.dblClick(function (data) {
            if (data != null) {
                me.data.FakturPolisiNoTo = data.FakturPolisiNo;
                me.data.FakturPolisiDateTo = data.FakturPolisiDate;
                me.Apply();
            }
        });

    }

    $("[name = 'isActiveDate']").on('change', function () {
        me.data.isActiveDate = $('#isActiveDate').prop('checked');
        me.Apply();
    });

    $("[name = 'isActiveEntryDate']").on('change', function () {
        me.data.isActiveEntryDate = $('#isActiveEntryDate').prop('checked');
        me.Apply();
    });

    $("[name = 'isFakturPolisi']").on('change', function () {
        me.data.isFakturPolisi = $('#isFakturPolisi').prop('checked');
        me.data.FakturPolisiNo = "";
        me.data.FakturPolisiNoTo = "";
        me.data.FakturPolisiDate = Date.now();
        me.data.FakturPolisiDateTo = Date.now();
        me.Apply();
    });

    me.printPreview = function () {
        $http.post('om.api/ReportSales/ValidatePrintFakturPolis', me.data)
           .success(function (e) {
               if (e.success) {
                   var ReportId = 'OmRpSalRgs015';
                   var par = [
                        moment(me.data.DateFrom).format('YYYYMMDD'),
                        moment(me.data.DateTo).format('YYYYMMDD'),
                        moment(me.data.EntryDateFrom).format('YYYYMMDD'),
                        moment(me.data.EntryDateTo).format('YYYYMMDD'),
                        me.data.FakturPolisiNo,
                        me.data.FakturPolisiNoTo
                   ]
                   var rparam = 'PERIODE : ' + moment(me.data.DateFrom).format('DD-MMM-YYYY') + ' S/D ' + moment(me.data.DateFrom).format('DD-MMM-YYYY');

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

    me.initialize = function () {

        $('#isActiveDate').prop('checked', true);
        me.data.isActiveDate = true;
        $('#isActiveEntryDate').prop('checked', true);
        me.data.isActiveEntryDate = true;
        $('#isFakturPolisi').prop('checked', false);
        me.data.isFakturPolisi = false;

        me.data.FakturPolisiNo = '';
        me.data.FakturPolisiNoTo = '';
        me.data.FakturPolisiDate = Date.now();
        me.data.FakturPolisiDateTo = Date.now();

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
              me.data.EntryDateFrom = e.DateFrom;
              me.data.EntryDateTo = e.DateTo;
          });
        me.Apply();

    }

    me.start();
}

$(document).ready(function () {
    var options = {
        title: "Report Register Faktur Polis",
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
                        text: "Tanggal Entry",
                        type: "controls",
                        items: [
                                { name: 'isActiveEntryDate', type: 'check', cls: "span1", text: "", float: 'left' },
                                { name: "EntryDateFrom", text: "", cls: "span2", type: "ng-datepicker", disable: "data.isActiveEntryDate == false" },
                                { name: "EntryDateTo", text: "", cls: "span2", type: "ng-datepicker", disable: "data.isActiveEntryDate == false" },
                        ]
                    },
                    { name: 'isFakturPolisi', type: 'check', cls: "", text: "No Faktur Polis", float: 'left' },
                    {
                        type: "controls",
                        items: [
                            { name: "FakturPolisiNo", cls: "span2", type: "popup", click: "FakturPolisiNo()", disable: "data.isFakturPolisi == false" },
                            { name: "FakturPolisiDate", cls: "span3", type: "ng-datepicker", readonly: true },
                        ]
                    },
                    {
                        type: "controls",
                        items: [
                            { name: "FakturPolisiNoTo", cls: "span2", type: "popup", click: "FakturPolisiNoTo()", disable: "data.isFakturPolisi == false" },
                            { name: "FakturPolisiDateTo", cls: "span3", type: "ng-datepicker", readonly: true },
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
        SimDms.Angular("RptRegisterPelangganPerleasing");
    }



});