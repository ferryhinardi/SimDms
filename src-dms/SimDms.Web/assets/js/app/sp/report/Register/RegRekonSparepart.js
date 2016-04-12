var report = "SpRpRgs022";
var isHolding = true;
"use strict"

function RptRegisterRekonSparepart($scope, $http, $injector) {

    var me = $scope;

    $injector.invoke(BaseController, this, { $scope: me });

    $http.post('sp.api/Combo/Years').
    success(function (data, status, headers, config) {
        me.comboYear = data;
    });

    $http.post('sp.api/Combo/Months').
    success(function (data, status, headers, config) {
        me.comboMonth = data;
    });

    me.default = function () {
        $http.post('sp.api/reportregister/default').

            success(function (e) {
                if (e.IsBranch) {
                    isHolding = !e.IsBranch;
                    me.data.BranchCodeFrom = me.data.BranchCodeTo = e.BranchCode;
                    me.data.CompanyNameFrom = me.data.CompanyNameTo = e.CompanyName;
                    $('#btnBranchCodeFrom').attr('disabled', true);
                    $('#btnBranchCodeTo').attr('disabled', true);
                }
                else {
                    $('#btnBranchCodeFrom').removeAttr('disabled');
                    $('#btnBranchCodeTo').removeAttr('disabled');
                }
                $('#Month').select2('val', e.Month);
                $('#Year').select2('val', e.Year);
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
                me.data.CompanyNameFrom = data.CompanyName;
                if (me.data.BranchCodeTo === undefined) { me.data.BranchCodeTo = data.BranchCode; me.data.CompanyNameTo = data.CompanyName };
                if (me.data.BranchCodeTo < me.data.BranchCodeFrom) { me.data.BranchCodeTo = data.BranchCode ; me.data.CompanyNameTo = data.CompanyName};
            }
            else {
                me.data.BranchCodeTo = data.BranchCode;
                me.data.CompanyNameTo = data.CompanyName;
                if (me.data.BranchCodeFrom === undefined) { me.data.BranchCodeFrom = data.BranchCode ; me.data.CompanyNameFrom = data.CompanyName};
                if (me.data.BranchCodeTo < me.data.BranchCodeFrom) { me.data.BranchCodeFrom = data.BranchCode; me.data.CompanyNameFrom = data.CompanyName };
            }

            me.save = false;
            me.Apply();
        });
    }

    me.printPreview = function () {
        var month = $('#Month').select2('val');
        var year = $('#Year').select2('val');

        var currentTime = new Date(year, month, 1);
        var oldTime = currentTime.getMonth() - 1;

        var branch = isHolding? (me.data.BranchCodeFrom != undefined ? me.data.BranchCodeFrom == me.data.BranchCodeTo ? "CABANG : " + me.data.CompanyNameFrom : "CABANG : " + me.data.CompanyNameFrom + " sd " + me.data.CompanyNameTo
            : "CABANG : SEMUA CABANG")
            : "CABANG : " + me.data.CompanyNameFrom

        var par = 'companycode' + "," + me.data.BranchCodeFrom + "," + me.data.BranchCodeTo + "," + year + "," + month;
        var rparam = moment(oldTime).format('MMM-YYYY') + ',' + moment(currentTime).format('MMM-YYYY') + ',' + branch;

        Wx.showPdfReport({
            id: report,
            pparam: par,
            rparam: rparam,
            type: "devex"
        });
    }

    me.initialize = function () {
        me.isPrintAvailable = true;
        me.default();
    }

    me.start();
}

$(document).ready(function () {
    var options = {
        title: "Register Rekon Sparepart",
        xtype: "panels",
        toolbars: [
            { name: "btnPrintPreview", text: "Print", cls: "btn btn-primary", icon: "icon-print", click: "printPreview()" },
        ],
        panels: [
            {
                name: "pnlRegisterRekonSparepart",
                items: [
                  { name: "Month", required: true, cls: "span4", text: "Bulan", type: "select2", datasource: "comboMonth" },
                  { name: "Year", required: true, cls: "span4", text: "-", type: "select2", datasource: "comboYear" },
                  { name: "BranchCodeFrom", text: "Kode Branch", placeHolder: "", cls: "span4", type: "popup", click: "browseBranch(0)", validasi: "required", disable: isHolding },
                  { name: "CompanyNameFrom", cls: "hide" },
                  { name: "BranchCodeTo", text: "S/D", placeHolder: " ", cls: "span4", type: "popup", click: "browseBranch(1)", validasi: "required", disable: isHolding },
                  { name: "CompanyNameTo", cls: 'hide' },
                ]
            }
        ]
    };

    Wx = new SimDms.Widget(options);
    Wx.default = {};
    Wx.render(init);

    function init(s) {
        SimDms.Angular("RptRegisterRekonSparepart");
    }
});