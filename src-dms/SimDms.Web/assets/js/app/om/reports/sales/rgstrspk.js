"use strict"; //Reportid OmRpMst001
function spRptMstSales($scope, $http, $injector) {

    var me = $scope;

    $injector.invoke(BaseController, this, { $scope: me });

    me.supplier = function () {
        var lookup = Wx.blookup({
            name: "SupplierBrowse",
            title: "Pemasok",
            manager: spSalesManager,
            query: "SupplierCodeLookup",
            defaultSort: "SupplierCode asc",
            columns: [
                { field: "SupplierCode", title: "Kode Pemasok" },
                { field: "SupplierName", title: "Nama Pemasok" },
            ]
        });
        lookup.dblClick(function (data) {
            if (data != null) {
                me.data.SupplierCode = data.SupplierCode;
                me.data.SupplierName = data.SupplierName;
                me.Apply();
            }
        });
    };

    me.SPKNoFrom = function () {
        var lookup = Wx.blookup({
            name: "SPKLookup",
            title: "No SPK",
            manager: spSalesManager,
            query: "SPKLookup",
            defaultSort: "SPKNo asc",
            columns: [
                { field: "SPKNo", title: "No SPK" },
                { field: "SPKDate", title: "tgl SPK", template: "#= (SPKDate == undefined) ? '' : moment(SPKDate).format('DD MMM YYYY') #" },
            ],
        });
        lookup.dblClick(function (data) {
            if (data != null) {
                me.data.SPKNoFrom = data.SPKNo;
                me.data.SPKDateFrom = data.SPKDate;
                me.Apply();
            }
        });

    }

    me.SPKNoTo = function () {
        var lookup = Wx.blookup({
            name: "SPKLookup",
            title: "No SPK",
            manager: spSalesManager,
            query: "SPKLookup",
            defaultSort: "SPKNo asc",
            columns: [
                { field: "SPKNo", title: "No SPK" },
                { field: "SPKDate", title: "tgl SPK", template: "#= (SPKDate == undefined) ? '' : moment(SPKDate).format('DD MMM YYYY') #" },
            ],
        });
        lookup.dblClick(function (data) {
            if (data != null) {
                me.data.SPKNoTo = data.SPKNo;
                me.data.SPKDateTo = data.SPKDate;
                me.Apply();
            }
        });

    }

    me.printPreview = function () {
        var DateFrom = new Date(me.data.DateFrom).getMonth() + 1 + '/' + new Date(me.data.DateFrom).getDate() + '/' + new Date(me.data.DateFrom).getFullYear();
        var DateTo = new Date(me.data.DateTo).getMonth() + 1 + '/' + new Date(me.data.DateTo).getDate() + '/' + new Date(me.data.DateTo).getFullYear();
        var ReportId = 'OmRpSalRgs016';
        var prm = [
                    DateFrom,
                    DateTo,
                    me.data.SupplierCode,
                    me.data.SPKNoFrom,
                    me.data.SPKNoTo
        ];
        Wx.showPdfReport({
            id: ReportId,
            pparam: prm.join(','),
            textprint: true,
            rparam: "semua",
            type: "devex"
        });
    }

    me.initialize = function () {
        me.data = {};
        me.change = false;
        me.data.Status = "-1";
        $('#isC1').prop('checked', true);
        $http.get('breeze/sales/CurrentUserInfo').
          success(function (dl, status, headers, config) {
              me.data.CompanyCode = dl.CompanyCode;
              me.data.BranchCode = dl.BranchCode;
          });
        //if (new Date(Date.now()).getMonth() >= 0 || new Date(Date.now()).getMonth() <= 5) {
        //    me.data.DateFrom = 7 + '/' + 1 + '/' + (new Date().getFullYear() - 1);
        //    me.data.DateTo = 12 + '/' + 31 + '/' + (new Date().getFullYear() - 1);
        //}
        //else {
        //    me.data.DateFrom = 1 + '/' + 1 + '/' + (new Date().getFullYear());
        //    me.data.DateTo = 6 + '/' + 30 + '/' + (new Date().getFullYear());
        //}

        $http.post('om.api/ReportSales/Periode').
          success(function (e) {
              me.data.DateFrom = e.DateFrom;
              me.data.DateTo = e.DateTo;
          });

        me.data.SPKDateFrom = me.now();
        me.data.SPKDateTo = me.now();
        $('#SupplierCode').attr('disabled', true);
        $('#btnSupplierCode').attr('disabled', true);
        $('#SPKNoFrom').attr('disabled', true);
        $('#SPKNoTo').attr('disabled', true);
        $('#btnSPKNoFrom').attr('disabled', true);
        $('#btnSPKNoTo').attr('disabled', true);
        $('#SPKNoFrom').val("");
        $('#SPKNoTo').val("");
        me.isPrintAvailable = true;
    }

    $('#isC3').on('change', function (e) {
        if ($('#isC3').prop('checked') == true) {
            $('#SPKNoFrom').removeAttr('disabled');
            $('#SPKNoTo').removeAttr('disabled');
            $('#btnSPKNoFrom').removeAttr('disabled');
            $('#btnSPKNoTo').removeAttr('disabled');
        } else {
            $('#SPKNoFrom').attr('disabled', true);
            $('#SPKNoTo').attr('disabled', true);
            $('#btnSPKNoFrom').attr('disabled', true);
            $('#btnSPKNoTo').attr('disabled', true);
            $('#SPKNoFrom').val("");
            $('#SPKNoTo').val("");
        }
        me.Apply();
    })

    $('#isC2').on('change', function (e) {
        if ($('#isC2').prop('checked') == true) {
            $('#SupplierCode').removeAttr('disabled');
            $('#btnSupplierCode').removeAttr('disabled');
        } else {
            $('#SupplierCode').attr('disabled', true);
            $('#btnSupplierCode').attr('disabled', true);
            $('#SupplierCode').attr('disabled', true);
            $('#btnSupplierCode').attr('disabled', true);
            $('#SupplierCode').val("");
            $('#SupplierName').val("");
        }
        me.Apply();
    })

    $('#isC1').on('change', function (e) {
        if ($('#isC1').prop('checked') == true) {
            me.data.DateFrom = me.now();
            me.data.DateTo = me.now();
            $('#DateFrom').prop('readonly', false);
            $('#DateTo').prop('readonly', false);
        } else {
            me.data.DateFrom = undefined;
            me.data.DateTo = undefined;
            $('#DateFrom').prop('readonly', true);
            $('#DateTo').prop('readonly', true);
        }
        me.Apply();
    })
    me.start();

}


$(document).ready(function () {
    var options = {
        title: "Report Register SPK",
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
                            text: "Tanggal",
                            type: "controls",
                            items: [
                                { name: 'isC1', type: 'check', text: '', cls: 'span1', float: 'left' },
                                 { name: "DateFrom", text: "", cls: "span3", type: "ng-datepicker" },
                                { name: "DateTo", text: "", cls: "span3", type: "ng-datepicker" },
                            ]
                        },
                        {
                            text: "Pemasok",
                            type: "controls",
                            items: [
                                { name: 'isC2', type: 'check', text: '', cls: 'span1', float: 'left' },
                                { name: "SupplierCode", cls: "span3", type: "popup", click: "supplier()", disable: "data.isActive == false" },
                                { name: "SupplierName", text: "", cls: "span4", type: "text" , readonly : true},
                            ]
                        },
                        { name: 'isC3', type: 'check', text: 'No.SPK', cls: 'span1', float: 'left' },
                         {
                             text: "",
                             type: "controls",
                             items: [
                                 { name: "SPKNoFrom", cls: "span3", type: "popup", click: "SPKNoFrom()", disable: "data.isActive == false" },
                                 { name: "SPKDateFrom", text: "", cls: "span3", type: "ng-datepicker", disable: true },
                             ]
                         },
                        {
                            text: "",
                            type: "controls",
                            items: [
                                { name: "SPKNoTo", cls: "span3", type: "popup", click: "SPKNoTo()", disable: "data.isActive == false" },
                                { name: "SPKDateTo", text: "", cls: "span3", type: "ng-datepicker" , disable : true},
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