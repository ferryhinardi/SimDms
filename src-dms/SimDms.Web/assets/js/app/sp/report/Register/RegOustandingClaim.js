var report = "SpRpRgs020";
"use strict"

function RptRegisterOustandingClaim($scope, $http, $injector) {

    var me = $scope;

    $injector.invoke(BaseController, this, { $scope: me });

    $http.post('sp.api/Combo/LoadComboData?CodeId=TPGO').
    success(function (data, status, headers, config) {
        me.comboPartType = data;
        var part = document.getElementById('PartType')
        part.options[0].text = 'SELECT ALL';
    });

    me.browseClaimNo = function (e) {
        var part = $('#PartType').select2('val');
        part = part == "" ? "%" : part;

        var lookup = Wx.blookup({
            name: "ClaimNoBrowse",
            title: "Pencarian No Claim",
            manager: spRptRegisterManager,
            query: new breeze.EntityQuery.from("BrowseClaimNo").withParameters({ part: part }),
            defaultSort: "ClaimNo asc",
            columns: [
               { field: 'ClaimNo', title: 'No. Claim' },
               { field: 'ClaimDate', title: 'Tgl. Claim' },
            ]
        });
        lookup.dblClick(function (data) {
            if (e == 0) {
                me.data.ClaimNoFrom = data.ClaimNo;
    
                if (me.data.ClaimNoTo === undefined) { me.data.ClaimNoTo = data.ClaimNo };
                if (me.data.ClaimNoTo < me.data.ClaimNoFrom) { me.data.ClaimNoTo = data.ClaimNo };
            }
            else {
                me.data.ClaimNoTo = data.ClaimNo;
                if (me.data.ClaimNoFrom === undefined) { me.data.ClaimNoFrom = data.ClaimNo };
                if (me.data.ClaimNoTo < me.data.ClaimNoFrom) { me.data.ClaimNoFrom = data.ClaimNo };
            }
            me.save = false;
            me.Apply();
        });
    }

    me.printPreview = function () {
        var part = $('#PartType').select2('val');
        part = part == "" ? "%" : part;
        var sPart = "TIPE PART : "
        sPart += part == "" ? "SEMUA" : $('#PartType').select2('data').text;

        var par = me.data.ClaimNoFrom + ',' + me.data.ClaimNoTo + ',' + part;
        var rparam = sPart;
        Wx.showPdfReport({
            id: report,
            pparam: par,
            rparam: rparam,
            type: "devex"
        });
    }

    $('#PartType').on('change', function (e) {
        me.data.ClaimNoFrom = me.data.ClaimNoTo = "";
        me.Apply();
    });

    me.initialize = function () {
        me.isPrintAvailable = true;
    }

    me.start();
}

$(document).ready(function () {
    var options = {
        title: "Register Oustanding Claim",
        xtype: "panels",
        toolbars: [
            { name: "btnPrintPreview", text: "Print", cls: "btn btn-primary", icon: "icon-print", click: "printPreview()" },
        ],
        panels: [
            {
                name: "pnlRegisterOutstandingClaim",
                items: [
                    { name: "ClaimNoFrom", text: "No. Claim",cls: "span4", placeHolder: "No. Claim", readonly: true, required:true,type: "popup", click: "browseClaimNo(0)" },
                    { name: "ClaimNoTo", text: "S/D", cls: "span4", placeHolder: "No. Claim", readonly: true, required: true, type: "popup", click: "browseClaimNo(1)" },
                    { name: "PartType", text: "Tipe Part", cls: "span4 full ", type: "select2", datasource: "comboPartType" },
                ]
            }
        ]
    };

    Wx = new SimDms.Widget(options);
    Wx.default = {};
    Wx.render(init);

    function init(s) {
        SimDms.Angular("RptRegisterOustandingClaim");
    }
});