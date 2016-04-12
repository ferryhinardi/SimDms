var report = "SpRpRgs006";
var isHolding = true;
"use strict"

function RptRegisterReturPenjualan($scope, $http, $injector) {

    var me = $scope;

    $injector.invoke(BaseController, this, { $scope: me });

    $http.post('sp.api/Combo/LoadComboData?CodeId=TPGO').
    success(function (data, status, headers, config) {
        me.comboPartType = data;
        var part = document.getElementById('PartType')
        part.options[0].text = 'SELECT ALL';
    });

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

    me.printPreview = function () {
             var part = $('#PartType').select2('val');
        var sPart = "TIPE PART : "
        sPart += part == "" ? "SEMUA" : $('#PartType').select2('data').text;

        var firstPeriod = moment(me.data.FirstPeriod).format('YYYYMMDD');
        var endPeriod = moment(me.data.EndPeriod).format('YYYYMMDD');
        var par = 'companycode' + "," + me.data.BranchCodeFrom + "," + me.data.BranchCodeTo + "," + firstPeriod + "," + endPeriod + "," + part + "," + isHolding;

        var data = $(".main .gl-widget").serializeObject();
        var periode = "PERIODE : " + data.FirstPeriod + " S/D " + data.EndPeriod;
        var kodebranch = isHolding == true ? "KODE BRANCH : " : "";
        var rparam = periode + "," + sPart + "," + kodebranch;

        Wx.showPdfReport({
            id: report,
            pparam: par,
            rparam: rparam,
            type: "devex"
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
}

$(document).ready(function () {
    var options = {
        title: "Register Retur Penjualan",
        xtype: "panels",
        toolbars: [
            { name: "btnPrintPreview", text: "Print", cls: "btn btn-primary", icon: "icon-print", click: "printPreview()" },
        ],
        panels: [
            {
                name: "pnlRegisterReturPenjualan",
                items: [
                   { name: "BranchCodeFrom", text: "Kode Branch", placeHolder: "", cls: "span4", type: "popup", click: "browseBranch(0)", validasi: "required", disable: isHolding },
                   { name: "BranchCodeTo", text: "S/D", placeHolder: " ", cls: "span4", type: "popup", click: "browseBranch(1)", validasi: "required", disable: isHolding },
                   { name: "FirstPeriod", text: "Periode", cls: "span4", type: "ng-datepicker", validasi: "required" },
                   { name: "EndPeriod", text: "S/D", cls: "span4", type: "ng-datepicker", validasi: "required" },
                   { name: "PartType", text: "Tipe Part", cls: "span4 full ", type: "select2", datasource: "comboPartType" },
                ]
            }
        ]
    };

    Wx = new SimDms.Widget(options);
    Wx.default = {};
    Wx.render(init);

    function init(s) {
        SimDms.Angular("RptRegisterReturPenjualan");
    }
});