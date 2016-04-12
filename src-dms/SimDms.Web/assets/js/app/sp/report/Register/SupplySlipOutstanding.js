"use strict"
var isHolding = true;
var allPeriod = true;

function RptRegisterSupplySlipOutstanding($scope, $http, $injector) {

    var me = $scope;

    $injector.invoke(BaseController, this, { $scope: me });

    $http.post('sp.api/Combo/LoadComboData?CodeId=TTSR').
    success(function (data, status, headers, config) {
        me.comboTransType = data;
        var tran = document.getElementById('TransType')
        tran.options[0].text = 'SELECT ALL';
    });

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
                  me.data.BranchCodeFrom = e.BranchCode;
                  me.data.BranchCodeTo = e.BranchCode;
                  //$('#BranchCodeFrom,#BranchCodeTo').val(e.BranchCode);
                  $('#btnBranchCodeFrom').attr('disabled', true);
                  $('#btnBranchCodeTo').attr('disabled', true);
              }
              else {
                  $('#btnBranchCodeFrom').removeAttr('disabled');
                  $('#btnBranchCodeTo').removeAttr('disabled');
              }
          });
    }

    me.printPreview = function () {
        var trans = $('#TransType').select2('val');
        var sTrans = "JENIS TRANSAKSI : ";
        sTrans += trans == "" ? "SELURUH JENIS TRANSAKI" : $('#TransType').select2('data').text

        trans = trans == "" ? "ALL" : trans;

        var part = $('#PartType').select2('val');
        var sPart = "TIPE PART : "
        sPart += part == "" ? "SEMUA" : $('#PartType').select2('data').text;

        var firstPeriod = allPeriod ? "" : moment(me.data.FirstPeriod).format("YYYYMMDD 00:00:00");
        var endPeriod = allPeriod ? "" : moment(me.data.EndPeriod).format("YYYYMMDD 23:59:59");
        var par = 'companycode' + "," + me.data.BranchCodeFrom + "," + me.data.BranchCodeTo + "," + firstPeriod + "," + endPeriod + "," + trans + "," + part + "," + isHolding;

        var data = $(".main .gl-widget").serializeObject();
        var periode = "PERIODE : " + data.FirstPeriod + " S/D " + data.EndPeriod;
        var kodebranch = isHolding == true ? "KODE BRANCH : " : "";
        var rparam = periode + "," + sPart + "," + sTrans + "," + kodebranch;        
        console.log(me.data.FirstPeriod, allPeriod, firstPeriod, endPeriod);
        Wx.showPdfReport({
            //id: "SpRpRgs002",
            id:"SpRpRgs002_Web1",
            pparam: par,
            rparam: rparam,
            type: "devex"
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

    me.initialize = function () {
        me.data.FirstPeriod = me.data.EndPeriod = me.now();
        me.isPrintAvailable = true;
        me.default();        
        allPeriod = false;
    }
    
    $('#AllPeriodN').on('change', function (e) {
        $('#FirstPeriod').removeAttr('disabled');
        $('#EndPeriod').removeAttr('disabled');
        allPeriod = false;
    });

    $('#AllPeriodY').on('change', function (e) {
        $('#FirstPeriod').attr('disabled', true);
        $('#EndPeriod').attr('disabled', true);
        allPeriod = true;
    });

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

    me.start();
}

$(document).ready(function () {
    var options = {
        title: "Supply Slip Outstanding",
        xtype: "panels",
        toolbars: [
            { name: "btnPrintPreview", text: "Print", cls: "btn btn-primary", icon: "icon-print", click: "printPreview()" },
        ],
        panels: [
            {
                name: "pnlSSOutstanding",
                items: [
                   { name: "BranchCodeFrom", text: "Kode Branch", placeHolder:"", cls: "span4", type: "popup", click: "browseBranch(0)", validasi: "required" , disable: isHolding},
                   { name: "BranchCodeTo", text: "S/D", placeHolder: " ", cls: "span4", type: "popup", click: "browseBranch(1)", validasi: "required", disable: isHolding },
                   { name: "FirstPeriod", text: "Periode", cls: "span4", type: "ng-datepicker", validasi: "required" },
                   { name: "EndPeriod", text: "S/D", cls: "span4", type: "ng-datepicker", validasi: "required" },
                   { name: "AllPeriod", text: "Semua Periode", cls: "span2 full", type: "switch" },
                   { name: "TransType", text: "Tipe Transaksi", cls: "span4 full ", type: "select2", datasource: "comboTransType" },
                   { name: "PartType", text: "Tipe Part", cls: "span4 full ", type: "select2", datasource: "comboPartType" },
                ]
            }
        ]
    };

    Wx = new SimDms.Widget(options);
    Wx.default = {};
    Wx.render(init);

    function init(s) {
        SimDms.Angular("RptRegisterSupplySlipOutstanding");
    }
});