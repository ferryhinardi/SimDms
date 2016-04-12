"use strict"; //Reportid OmRpMst001
function spRptMstSales($scope, $http, $injector) {

    var me = $scope;

    $injector.invoke(BaseController, this, { $scope: me });

    me.BranchFrom = function () {
        var lookup = Wx.blookup({
            name: "BranchBrowse",
            title: "Cabang",
            manager: spSalesManager,
            query: new breeze.EntityQuery().from("RefferenceCodeLookup").withParameters({ RefferenceType: "WARE" }),
            defaultSort: "RefferenceCode asc",
            columns: [
                { field: "RefferenceCode", title: "Kode Cabang" },
                { field: "RefferenceDesc1", title: "Nama Cabang" },
            ]
        });
        lookup.dblClick(function (data) {
            if (data != null) {
                me.data.BranchCodeFrom = data.RefferenceCode;
                me.data.BranchNameFrom = data.RefferenceDesc1;
                me.data.BranchCodeTo = data.RefferenceCode;
                me.data.BranchNameTo = data.RefferenceDesc1;
                me.Apply();
            }
        });
    };

    me.BranchTo = function () {
        var lookup = Wx.blookup({
            name: "BranchBrowse",
            title: "Cabang",
            manager: spSalesManager,
            query: new breeze.EntityQuery().from("RefferenceCodeLookup").withParameters({ RefferenceType: "WARE" }),
            defaultSort: "RefferenceCode asc",
            columns: [
                { field: "RefferenceCode", title: "Kode Cabang" },
                { field: "RefferenceDesc1", title: "Nama Cabang" },
            ]
        });
        lookup.dblClick(function (data) {
            if (data != null) {
                me.data.BranchCodeTo = data.RefferenceCode;
                me.data.BranchNameTo = data.RefferenceDesc1;
                me.Apply();
            }
        });
    };

    me.printPreview = function () {
        var prm = ""; var sparam = "";
        var dateFrom = ""; var dateTo = "";
        if ($('#isC1').prop('checked') == true) {
            dateFrom = moment(me.data.DateFrom).format('YYYYMMDD');
            dateTo = moment(me.data.DateTo).format('YYYYMMDD');
            sparam="PERIODE : " + moment(me.data.DateFrom).format("DD-MMM-YYYY") + " s/d " + moment(me.data.DateTo).format("DD-MMM-YYYY");
        }
        else {
            sparam="PERIODE : ALL";
        }
        prm = [
                    me.data.BranchCodeFrom,
                    me.data.BranchCodeTo,
                    dateFrom,
                    dateTo
        ];
        Wx.showPdfReport({
            id: "OmRpSalRgs029",
            pparam: prm.join(','),
            textprint: true,
            rparam: sparam,
            type: "devex"
        });
    }

    me.initialize = function () {
        me.data = {};
        me.change = false;
        $http.get('breeze/sales/CurrentUserInfo').
          success(function (dl, status, headers, config) {
              me.data.CompanyCode = dl.CompanyCode;
              me.data.BranchCode = dl.BranchCode;

          });
        me.data.DateFrom = undefined;
        $('#DateFrom').attr('disabled', true);
        me.data.DateTo = undefined;
        $('#DateTo').attr('disabled', true);
        me.isPrintAvailable = true;
    }
    
    $('#isC1').on('change', function (e) {
        if ($('#isC1').prop('checked') == true) {
            me.data.DateFrom = me.now();
            me.data.DateTo = me.now();
            $('#DateFrom').attr('disabled', false);
            $('#DateTo').attr('disabled', false);
        } else {
            me.data.DateFrom = undefined;
            $('#DateFrom').attr('disabled', true);
            me.data.DateTo = undefined;
            $('#DateTo').attr('disabled', true);
        }
        me.Apply();
    })

    me.start();
}

$(document).ready(function () {
    var options = {
        title: "Report Daftar BPKB",
        xtype: "panels",
        toolbars: [
            { name: "btnPrintPreview", text: "Print", cls: "btn btn-primary", icon: "icon-print", click: "printPreview()", disable: "!isPrintEnable" },
            { name: "btnCancel", text: "Cancel", cls: "btn btn-warning", icon: "icon-remove", click: "cancelOrClose()" },
        ],
        panels: [
            {
                name: "pnlA",
                items: [
                        { name: "CompanyCode", model: "data.CompanyCode", text: "Kode Perusahaan", cls: "span4 full", disable: "isPrintAvailable" },
                        { name: "BranchCode", model: "data.BranchCode", text: "Kode Cabang", cls: "span4 full", disable: "isPrintAvailable" },
                         {
                             text: "Cabang : ",
                             type: "controls",
                             items: [

                                 { name: "BranchCodeFrom", cls: "span3", type: "popup", btnName: "btnBranchCodeFrom", click: "BranchFrom()", disable: "data.isActive == false" },
                                 { name: "BranchNameFrom", text: "", cls: "span4", type: "text", readonly: true },
                             ]
                         },
                         {
                             text: "",
                             type: "controls",
                             items: [
                                 { name: "BranchCodeTo", cls: "span3", type: "popup", btnName: "btnBranchCodeTo", click: "BranchTo()", disable: "data.isActive == false" },
                                 { name: "BranchNameTo", text: "", cls: "span4", type: "text", readonly: true },
                             ]
                         },
                        {
                            text: "Periode :",
                            type: "controls",
                            items: [
                                    { name: 'isC1', type: 'check', text: '', cls: 'span1', float: 'left' },
                                    { name: "DateFrom", text: "", cls: "span3", type: "ng-datepicker" },
                                    { name: "DateTo", text: "", cls: "span3", type: "ng-datepicker" },
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
        $(".switchlabel").attr("style", "padding:9px 0px 0px 5px")
        SimDms.Angular("spRptMstSales");

    }
});