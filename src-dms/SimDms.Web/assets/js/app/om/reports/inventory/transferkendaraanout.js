"use strict"

var isHolding = true;

function omReportTransferKendaraanOutController($scope, $http, $injector) {

    var me = $scope;

    $injector.invoke(BaseController, this, { $scope: me });

    $http.post('om.api/Combo/Status').
         success(function (data) {
             me.comboStatus = data;
             var tran = document.getElementById('Status')
             tran.options[0].text = 'ALL';
         });

    $http.post('om.api/Combo/StatusTransfer').
         success(function (data) {
             me.comboStatusTransfer = data;
             var tran = document.getElementById('StatusTransfer')
             tran.options[0].text = 'ALL';
         });

    me.branchBrowse = function (e) {
        var lookup = Wx.blookup({
            name: "BranchBrowse",
            title: "Cabang",
            manager: spSalesManager,
            query: "BranchBrowse",
            defaultSort: "Code asc",
            columns: [
                   { field: 'Code', title: 'Kode Cabang' },
                   { field: 'Desc', title: 'Nama Cabang' },
            ]
        });
        lookup.dblClick(function (data) {
            if (e == 0) {
                me.data.BranchCodeFrom = data.Code;
                if (me.data.BranchCodeTo === undefined) { me.data.BranchCodeTo = data.Code };
                if (me.data.BranchCodeTo < me.data.BranchCodeFrom) { me.data.BranchCodeTo = data.Code };
            }
            else {
                me.data.BranchCodeTo = data.Code;
                if (me.data.BranchCodeFrom === undefined) { me.data.BranchCodeFrom = data.Code };
                if (me.data.BranchCodeTo < me.data.BranchCodeFrom) { me.data.BranchCodeFrom = data.Code };
            }

            me.Apply();
        });
    }

    me.printPreview = function () {
        var status = $('#Status').select2('val');
        var statusTransfer = $('#StatusTransfer').select2('val');
        statusTransfer = statusTransfer == "" ? 0 : statusTransfer;
        var ReportId = "OmRpInvRgs006";

        var par = [
            'companycode',
            me.data.BranchCodeFrom,
            me.data.BranchCodeTo,
            me.data.DateFrom,
            me.data.DateTo,
            status,
            statusTransfer,
            isHolding
        ]

        var rparam = [
            $('#Status').select2('data').text,
            moment(me.data.DateFrom).format('DD-MMM-YYYY'),
            moment(me.data.DateTo).format('DD-MMM-YYYY'),
            '1',
            isHolding == true ? "KODE CABANG : " : ''
        ]

        Wx.showPdfReport({
            id: ReportId,
            pparam: par.join(','),
            rparam: rparam,
            type: "devex"
        });
    }

    me.default = function () {
        $http.post('om.api/reportinventory/Transfer').
          success(function (e) {
              me.data.DateFrom = e.DateFrom;
              me.data.DateTo = e.DateTo;
              if (e.IsBranch) {
                  isHolding = !e.IsBranch;
              }
              else {
              }
          });
        me.Apply();
    }

    $("#BranchSwitchN").on('change', function (e) {
        $('#btnBranchCodeFrom, #btnBranchCodeTo').attr('disabled', 'disabled');
        $('#BranchCodeFrom, #BranchCodeTo').val('');
    });
    $("#BranchSwitchY").on('change', function (e) {
        $('#btnBranchCodeFrom, #btnBranchCodeTo').removeAttr('disabled');
    });

    me.initialize = function () {
        me.default();

        $('#btnBranchCodeFrom, #btnBranchCodeTo').attr('disabled', 'disabled');
    }

    me.start();
}

$(document).ready(function () {
    var options = {
        title: "Transfer Kendaraan Out",
        xtype: "panels",
        toolbars: [
            { name: "btnPrintPreview", text: "Print", cls: "btn btn-primary", icon: "icon-print", click: "printPreview()" },
        ],
        panels: [
           {
               name: "pnlTransferOut",
               items: [
                 { name: "BranchSwitch", text: "Gudang", cls: "span2 full", type: "switch" },
                 { name: "BranchCodeFrom", text: "Kode Branch", cls: "span4", type: "popup", readonly: true, click: "branchBrowse(0)" },
                 { name: "BranchCodeTo", text: "S/D", cls: "span4", type: "popup", readonly: true, click: "branchBrowse(1)" },
                 { name: "Status", text: "Status", cls: "span4 full ", type: "select2", datasource: "comboStatus" },
                 { name: "StatusTransfer", text: "Status Transfer In", cls: "span4 full ", type: "select2", datasource: "comboStatusTransfer" },
                 { name: "DateFrom", text: "Date", cls: "span4", type: "ng-datepicker" },
                 { name: "DateTo", text: "S/D", cls: "span4", type: "ng-datepicker" },
               ]
           }
        ]
    };

    Wx = new SimDms.Widget(options);
    Wx.default = {};
    Wx.render(init);

    function init(s) {
        SimDms.Angular("omReportTransferKendaraanOutController");
    }
});