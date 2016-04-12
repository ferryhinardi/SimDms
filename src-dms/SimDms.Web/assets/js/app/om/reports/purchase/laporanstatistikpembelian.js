"use strict"; //Reportid OmRpMst001
function spRptMstSales($scope, $http, $injector) {

    var me = $scope;

    $injector.invoke(BaseController, this, { $scope: me });

    me.Status = [
        { "value": '0', "text": 'Dalam Rupiah' },
        { "value": '1', "text": 'Dalam Unit' },

    ];

    me.supplierFrom = function () {
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
                me.data.SupplierCodeFrom = data.SupplierCode;
                me.data.SupplierNameFrom = data.SupplierName;
                me.Apply();
            }
        });
    };

    me.supplierTo = function () {
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
                me.data.SupplierCodeTo = data.SupplierCode;
                me.data.SupplierNameTo = data.SupplierName;
                me.Apply();
            }
        });
    };

    me.ModelCodeFrom = function () {
        var lookup = Wx.blookup({
            name: "ModelCodeLookup",
            title: "Kode Model Awal",
            manager: spSalesManager,
            query: "ModulBrowse",
            defaultSort: "SalesModelCode asc",
            columns: [
                { field: "SalesModelCode", title: "Model Code" },
                { field: "SalesModelDesc", title: "Desc." },
            ],
        });
        lookup.dblClick(function (data) {
            if (data != null) {
                me.data.ModelCodeFrom = data.SalesModelCode;
                me.data.ModelNameFrom = data.SalesModelDesc;
                me.Apply();
            }
        });

    }

    me.ModelCodeTo = function () {
        var lookup = Wx.blookup({
            name: "ModelCodeLookup",
            title: "Kode Model Akhir",
            manager: spSalesManager,
            query: "ModulBrowse",
            defaultSort: "SalesModelCode asc",
            columns: [
                { field: "SalesModelCode", title: "Model Code" },
                { field: "SalesModelDesc", title: "Desc." },
            ],
        });
        lookup.dblClick(function (data) {
            if (data != null) {
                me.data.ModelCodeTo = data.SalesModelCode;
                me.data.ModelNameTo = data.SalesModelDesc;
                me.Apply();
            }
        });

    }
    
    me.printPreview = function () {
        //if (!compareString(txtModelFrom, txtModelTo)) {
        //    MsgBox("Kode Model Awal tidak boleh melebihi Kode Model Akhir", MSG_ERROR);
        //    return;
        //}

        //if (!compareString(txtSupCodeFrom, txtSupCodeTo)) {
        //    MsgBox("Kode Supplier Awal tidak boleh melebihi Kode Supplier Akhir", MSG_ERROR);
        //    return;
        //}
        var param1 ="";
        var param2 = "";
        var ReportId = "";
        if (me.options == "0") {
            param1 = me.data.FiscalYear + "04"
            param2 = (me.data.FiscalYear + 1) + "03"
            if ($('#isC3').prop('checked') == true)
                ReportId = "OmRpPurRgs005";
            else
                ReportId = "OmRpPurRgs005A";
        } else {
            param1 = me.data.FiscalYear + "01"
            param2 = (me.data.FiscalYear + 1) + "12"
            if ($('#isC3').prop('checked') == true)
                ReportId = "OmRpPurRgs004";
            else
                ReportId = "OmRpPurRgs004A";
            
        }
            var prm = [
                       // me.data.CompanyCode,
                        me.data.ModelCodeFrom,
                        me.data.ModelCodeTo,
                        me.data.SupplierCodeFrom,
                        me.data.SupplierCodeTo,
                        param1,
                        param2
            ];
        Wx.showPdfReport({
            id: ReportId,
            pparam: prm.join(','),
            rparam: "semua",
            type: "devex"
        });
    }

    me.initialize = function () {
        me.data = {};
        me.change = false;
        me.data.Status = '0';
        me.data.FiscalYear = "2014";
        $http.get('breeze/sales/CurrentUserInfo').
          success(function (dl, status, headers, config) {
              me.data.CompanyCode = dl.CompanyCode;
              me.data.BranchCode = dl.BranchCode;

          });

        //alert(new Date().getMonth());
        $('#SupplierCodeFrom').attr('disabled', true);
        $('#btnSupplierCodeFrom').attr('disabled', true);
        $('#SupplierCodeTo').attr('disabled', true);
        $('#btnSupplierCodeTo').attr('disabled', true);

        $('#ModelCodeFrom').attr('disabled', true);
        $('#ModelCodeTo').attr('disabled', true);
        $('#btnModelCodeFrom').attr('disabled', true);
        $('#btnModelCodeTo').attr('disabled', true);

        me.isPrintAvailable = true;
    }

    $('#isC3').on('change', function (e) {
        if ($('#isC3').prop('checked') == true) {
            $('#ModelCodeFrom').removeAttr('disabled');
            $('#ModelCodeTo').removeAttr('disabled');
            $('#btnModelCodeFrom').removeAttr('disabled');
            $('#btnModelCodeTo').removeAttr('disabled');
        } else {
            $('#ModelCodeFrom').attr('disabled', true);
            $('#ModelCodeTo').attr('disabled', true);
            $('#btnModelCodeFrom').attr('disabled', true);
            $('#btnModelCodeTo').attr('disabled', true);
            $('#ModelCodeFrom').val("");
            $('#ModelCodeTo').val("");
            $('#ModelNameFrom').val("");
            $('#ModelNameTo').val("");
        }
        me.Apply();
    })

    $('#isC2').on('change', function (e) {
        if ($('#isC2').prop('checked') == true) {
            $('#SupplierCodeFrom').removeAttr('disabled');
            $('#btnSupplierCodeFrom').removeAttr('disabled');
            $('#SupplierCodeTo').removeAttr('disabled');
            $('#btnSupplierCodeTo').removeAttr('disabled');
        } else {
            $('#SupplierCodeFrom').attr('disabled', true);
            $('#btnSupplierCodeFrom').attr('disabled', true);
            $('#SupplierCodeTo').attr('disabled', true);
            $('#btnSupplierCodeTo').attr('disabled', true);
            $('#SupplierCodeFrom').val("");
            $('#SupplierNameFrom').val("");
            $('#SupplierCodeTo').val("");
            $('#SupplierNameTo').val("");
        }
        me.Apply();
    })

    
    me.start();
    me.options = '0';
    me.optionss = '0';
}


$(document).ready(function () {
    var options = {
        title: "Report Laporan Statistik Pembelian",
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
                        { name: "FiscalYear", opt_text: "", cls: "span3", type: "text", text: "TAhun Fiscal", datasource: "Status" },
                         {
                             type: "optionbuttons",
                             name: "tabpageoptions",
                             model: "options",
                             cls: "span3",
                             items: [
                                 { name: "0", text: "Tahun Fiscal" },
                                 { name: "1", text: "Tahun Kalender" },
                             ]
                         },
                          {
                              text: "Model",
                              type: "controls",
                              items: [
                                  { name: 'isC3', type: 'check', text: '', cls: 'span1', float: 'left' },
                                  { name: "ModelCodeFrom", cls: "span3", type: "popup", btnName: "btnModelCodeFrom", click: "ModelCodeFrom()", disable: "data.isActive == false" },
                                  { name: "ModelNameFrom", cls: "span4", type: "text", readonly : true },
                              ]
                          },
                          {
                              text: "",
                              type: "controls",
                              items: [
                                  { name: "ModelCodeTo", cls: "span3", type: "popup", btnName: "ModelCodeTo", click: "ModelCodeTo()", disable: "data.isActive == false" },
                                  { name: "ModelCodeTo", cls: "span4", type: "text", readonly: true },
                              ]
                          },
                        {
                            text: "Pemasok",
                            type: "controls",
                            items: [
                                { name: 'isC2', type: 'check', text: '', cls: 'span1', float: 'left' },
                                { name: "SupplierCodeFrom", cls: "span3", type: "popup", btnName: "SupplierCodeFrom", click: "supplierFrom()", disable: "data.isActive == false" },
                                { name: "SupplierNameFrom", text: "", cls: "span4", type: "text", readonly: true },
                            ]
                        },
                         {
                             text: "",
                             type: "controls",
                             items: [
                                 { name: "SupplierCodeTo", cls: "span3", type: "popup", btnName: "SupplierCodeTo", click: "supplierTo()", disable: "data.isActive == false" },
                                 { name: "SupplierNameTo", text: "", cls: "span4", type: "text", readonly: true },
                             ]
                         },
                         { name: "Status", opt_text: "", cls: "span3", type: "select2", text: "Report", datasource: "Status" },
                        {
                            type: "optionbuttons",
                            name: "tabpageoptions",
                            model: "optionss",
                            text : "Urut Berdasarkan",
                            cls: "span8",
                            items: [
                                { name: "0", text: "Model" },
                                { name: "1", text: "Pemasok" },
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