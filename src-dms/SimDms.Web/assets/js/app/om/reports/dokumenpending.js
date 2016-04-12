"use strict"; //Reportid OmRpMst001
function spRptMstSales($scope, $http, $injector) {

    var me = $scope;

    $injector.invoke(BaseController, this, { $scope: me });

    me.Status = [
        { "value": '0', "text": 'Dalam Rupiah' },
        { "value": '1', "text": 'Dalam Unit' },

    ];

    me.Periode = function () {
        var lookup = Wx.blookup({
            name: "SupplierBrowse",
            title: "Pemasok",
            manager: spSalesManager,
            query: new breeze.EntityQuery().from("Periode4Lookup").withParameters({ FiscalYear: me.data.FiscalYear }),
            defaultSort: "PeriodeNum asc",
            columns: [
                { field: "Periode", title: "Periode" },
                { field: "FromDate", title: "Dari Tanggal", template: "#= (FromDate == undefined) ? '' : moment(FromDate).format('DD MMM YYYY') #" },
                { field: "EndDate", title: "Sampai Tanggal", template: "#= (EndDate == undefined) ? '' : moment(EndDate).format('DD MMM YYYY') #" }
            ]
        });
        lookup.dblClick(function (data) {
            if (data != null) {
                me.data.Periode = data.Periode;
                me.data.PeriodeDesc = "Dari Tanggal " + moment(data.FromDate).format('DD MMM YYYY') + " s/d Tanggal " + moment(data.EndDate).format('DD MMM YYYY');
                me.data.FromDate = data.FromDate;
                me.Apply();
            }
        });
    };

    me.printPreview = function () {
        var periode = new Date(me.data.FromDate).getMonth() + 1 + '/' + new Date(me.data.FromDate).getDate() + '/' + new Date(me.data.FromDate).getFullYear();
        var prm = [
                   // me.data.CompanyCode,
                    periode
        ];
        Wx.showPdfReport({
            id: "OmRpDocPending",
            pparam: prm.join(','),
            textprint: true,
            rparam: "semua",
            type: "devex"
        });
    }

    me.initialize = function () {
        me.data = {};
        me.change = false;
        me.data.FiscalYear = new Date().getFullYear();
        $http.get('breeze/sales/CurrentUserInfo').
          success(function (dl, status, headers, config) {
              me.data.CompanyCode = dl.CompanyCode;
              me.data.BranchCode = dl.BranchCode;
          });

        me.isPrintAvailable = true;
    }

    $('#FiscalYear').on('keydown', function (e) {
        me.data.Periode = undefined;
        me.data.PeriodeDesc = undefined;
        me.Apply();
    })
    me.start();

}


$(document).ready(function () {
    var options = {
        title: "Report Dokumen Pending",
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
                        { name: "FiscalYear", opt_text: "", cls: "span3", type: "text", text: "Tahun Fiscal", datasource: "Status" },
                          {
                              text: "Periode",
                              type: "controls",
                              items: [
                                  { name: "Periode", cls: "span3", type: "popup", btnName: "btnModelCodeFrom", click: "Periode()", disable: "data.isActive == false", required: true, validasi: "required" },
                                  { name: "PeriodeDesc", cls: "span5", type: "text", readonly: true },
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