var isHolding = true;
var report = "SpRpRgs009";
"use strict"

function RptRegisterBonPengeluaranSparePart($scope, $http, $injector) {

    var me = $scope;

    $injector.invoke(BaseController, this, { $scope: me });

    $http.post('sp.api/Combo/LoadComboData?CodeId=TTSR').
    success(function (data, status, headers, config) {
        me.comboTransType = data;
    });

    $http.post('sp.api/Combo/LoadComboData?CodeId=TPGO').
    success(function (data, status, headers, config) {
        me.comboPartType = data;
        var part = document.getElementById('PartType')
        part.options[0].text = 'SELECT ALL';
    });

    me.$watch('ReportType', function (newValue, oldValue) {
        if (newValue !== oldValue) {
            me.$broadcast(newValue);
        }
    });

    $scope.$on('Summary', function () {
        report = "SpRpRgs009";
    });

    $scope.$on('Detail', function () {
        report = "SpRpRgs027";
    });

    me.printPreview = function () {
        
        var trans = $('#TransType').select2('val');
        var part = $('#PartType').select2('val');
        var sPart = "TIPE PART : "
        sPart += part == "" ? "SEMUA" : $('#PartType').select2('data').text;

        var firstPeriod = moment(me.data.FirstPeriod).format('YYYYMMDD');
        var endPeriod = moment(me.data.EndPeriod).format('YYYYMMDD');
        if (trans == "") {
            Wx.alert("Tipe Transaksi harus dipilh salah satu!!!");
        }
        else {
            var par = 'companycode' + "," + me.data.BranchCodeFrom + "," + me.data.BranchCodeTo + "," +
                me.data.CustomerCodeFrom + ',' + me.data.CustomerCodeTo + ',' + firstPeriod + "," + endPeriod + "," + trans + "," + part + "," + isHolding;

            var periode = "PERIODE : " + moment(me.data.FirstPeriod).format('DD-MMMM-YYYY') + " S/D " + moment(me.data.EndPeriod).format('DD-MMMM-YYYY');
            var kodebranch = isHolding == true ? "KODE BRANCH : " : "";
            var rparam = periode + "," + sPart + "," + kodebranch;

            Wx.showPdfReport({
                id: report,
                pparam: par,
                rparam: rparam,
                type: "devex"
            });
        }
    }


    me.default = function () {
        $http.post('sp.api/reportregister/default').
          success(function (e) {
              if (e.IsBranch) {
                  isHolding = !e.IsBranch;
                  $('#BranchCodeFrom,#BranchCodeTo').val(e.BranchCode);
                  $('#btnBranchCodeFrom').attr('disabled', true);
                  $('#btnBranchCodeTo').attr('disabled', true);
              }
              else {
                  $('#btnBranchCodeFrom').removeAttr('disabled');
                  $('#btnBranchCodeTo').removeAttr('disabled');
              }
          });
    }

    me.browseBranch = function (e) {
        var lookup = Wx.blookup({
            name: "BranchCodeBrowse",
            title: "Cabang",
            manager: spRptRegisterManager,
            query: "BrowseBranch",
            defaultSort: "BranchCode asc",
            columns: [
               { field: 'BranchCode', title: 'Kode Cabang' },
               { field: 'CompanyName', title: 'Nama Cabang' }
            ]
        });
        lookup.dblClick(function (data) {
            if (e == 0) {
                me.data.BranchCodeFrom = data.BranchCode;
                if (me.data.BranchCodeTo === undefined) { me.data.BranchCodeTo = data.BranchCode };
                if (me.data.BranchCodeTo < me.data.BranchCodeFrom) { me.data.BranchCodeTo = data.BranchCode };
            }
            else {
                me.data.BranchCodeTo = data.BranchCode;
                if (me.data.BranchCodeFrom === undefined) { me.data.BranchCodeFrom = data.BranchCode };
                if (me.data.BranchCodeTo < me.data.BranchCodeFrom) { me.data.BranchCodeFrom = data.BranchCode };
            }

            me.save = false;
            me.Apply();
        });
    }

    me.browseCustomer = function (e) {
        var lookup = Wx.blookup({
            name: "CustomerCodeBrowse",
            title: "Pelanggan",
            manager: spRptRegisterManager,
            query: "BrowseCustomer",
            defaultSort: "CustomerCode asc",
            columns: [
               { field: 'CustomerCode', title: 'Customer' },
               { field: 'CustomerName', title: 'Customer Name' },
            ]
        });
        lookup.dblClick(function (data) {
            if (e == 0) {
                me.data.CustomerCodeFrom = data.CustomerCode;
                me.data.CustomerNameFrom = data.CustomerName;
                if (me.data.CustomerCodeTo === undefined) { me.data.CustomerCodeTo = data.CustomerCode; me.data.CustomerNameTo = data.CustomerName };
                if (me.data.CustomerCodeTo < me.data.CustomerCodeFrom) { me.data.CustomerCodeTo = data.CustomerCode };
            }
            else {
                me.data.CustomerCodeTo = data.CustomerCode;
                me.data.CustomerNameTo = data.CustomerName;
                if (me.data.CustomerCodeFrom === undefined) { me.data.CustomerCodeFrom = data.CustomerCode; me.data.CustomerNameFrom = data.CustomerName };
                if (me.data.CustomerCodeTo < me.data.CustomerCodeFrom) { me.data.CustomerCodeFrom = data.CustomerCode; me.data.CustomerNameFrom = data.CustomerName};
            }
            me.save = false;
            me.Apply();
        });
    }

    $('#FirstPeriod').on('change', function (e) {
        var firstPeriod = $('#FirstPeriod').val();
        var endPeriod = $('#EndPeriod').val();

        if (firstPeriod > endPeriod) { $('#EndPeriod').val(firstPeriod) }
    });

    $('#EndPeriod').on('change', function (e) {
        var firstPeriod = $('#FirstPeriod').val();
        var endPeriod = $('#EndPeriod').val();

        if (firstPeriod < endPeriod) { $('#FirstPeriod').val(endPeriod) }
    });

    me.initialize = function () {
        me.data.FirstPeriod = me.data.EndPeriod = me.now();
        me.isPrintAvailable = true;
        me.default();
    }

    me.start();
    me.ReportType = "Summary";
}

$(document).ready(function () {
    var options = {
        title: "Register Bon Pengeluaran Sparepart",
        xtype: "panels",
        toolbars: [
            { name: "btnPrintPreview", text: "Print", cls: "btn btn-primary", icon: "icon-print", click: "printPreview()" },
        ],
        panels: [
            {
                name: "pnlBonPengeluaranSparepart",
                items: [
                   { name: "BranchCodeFrom", text: "Kode Branch", placeHolder: "", cls: "span4", type: "popup", click: "browseBranch(0)", validasi: "required", disable: isHolding },
                   { name: "BranchCodeTo", text: "S/D", placeHolder: " ", cls: "span4", type: "popup", click: "browseBranch(1)", validasi: "required", disable: isHolding },
                   { name: "FirstPeriod", text: "Periode", cls: "span4", type: "ng-datepicker", validasi: "required" },
                   { name: "EndPeriod", text: "S/D", cls: "span4", type: "ng-datepicker", validasi: "required" },
                   { name: "TransType", text: "Tipe Transaksi", cls: "span4 full ", type: "select2", datasource: "comboTransType" },
                   { name: "PartType", text: "Tipe Part", cls: "span4 full ", type: "select2", datasource: "comboPartType" },
                   {
                       type: "optionbuttons",
                       name: "tabpage1",
                       model: "ReportType",
                       text: "Report Type",
                       items: [
                           { name: "Summary", text: "Summary" },
                           { name: "Detail", text: "Detail" }, 
                       ]
                   },
                   {
                       text: "Pelanggan",
                       type: "controls",
                       items: [
                           { name: "CustomerCodeFrom", cls: "span2", placeHolder: "Kode Customer", readonly: true, type: "popup", click: "browseCustomer(0)" },
                           { name: "CustomerNameFrom", cls: "span6", placeHolder: "Nama Customer", readonly: true }
                       ]
                   },
                   {
                       text: "S/D",
                       type: "controls",
                       items: [
                           { name: "CustomerCodeTo", cls: "span2", placeHolder: "Kode Customer", readonly: true, type: "popup", click: "browseCustomer(1)" },
                           { name: "CustomerNameTo", cls: "span6", placeHolder: "Nama Customer", readonly: true }
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
        SimDms.Angular("RptRegisterBonPengeluaranSparePart");
    }
});