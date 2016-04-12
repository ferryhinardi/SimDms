"use strict"; //Reportid OmRpMst001
function spRptMstSales($scope, $http, $injector) {

    var me = $scope;

    $injector.invoke(BaseController, this, { $scope: me });

    me.Status = [
        { "value": '0', "text": 'Dalam Rupiah' },
        { "value": '1', "text": 'Dalam Unit' },

    ];

    me.BranchCode = function () {
        var lookup = Wx.blookup({
            name: "BranchLookup4Report",
            title: "Branch",
            manager: spSalesManager,
            query: "BranchLookup4Report",
            defaultSort: "BranchCode asc",
            columns: [
                { field: "BranchCode", title: "Kode Cabang" },
                { field: "CompanyName", title: "Nama Cabang" },
            ],
        });
        lookup.dblClick(function (data) {
            if (data != null) {
                me.data.BranchCodeFrom = data.BranchCode;
                me.data.BranchNameFrom = data.CompanyName;
                me.Apply();
            }
        });

    }

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

    me.sales = function (x) {
        if (me.optionsK == '0')
            me.employee(x);
        else
            me.kelompokAR(x);
    }

    me.employee = function (x) {
        var lookup = Wx.blookup({
            name: "SalesmanLookup",
            title: "Salesman",
            manager: spSalesManager,
            query: "SalesmanIDLookup",
            defaultSort: "EmployeeID asc",
            columns: [
                { field: "EmployeeID", title: "Kode Sales" },
                { field: "EmployeeName", title: "Nama Sales" },
            ],
        });
        lookup.dblClick(function (data) {
            if (data != null) {
                if (x) {
                    me.data.SalesCodeFrom = data.EmployeeID;
                    me.data.SalesNameFrom = data.EmployeeName;
                } else {
                    me.data.SalesCodeTo = data.EmployeeID;
                    me.data.SalesNameTo = data.EmployeeName;
                }
                me.Apply();
            }
        });
    }

    me.kelompokAR = function (x) {
        var lookup = Wx.blookup({
            name: "SalesmanLookup",
            title: "Salesman",
            manager: spSalesManager,
            query: "GroupARLookup",//new breeze.EntityQuery.from("PO4Lookup").withParameters({ CodeId: "GPAR" }),
            defaultSort: "LookUpValue asc",
            columns: [
                { field: "LookUpValue", title: "Kode Sales" },
                { field: "LookUpValueName", title: "Nama Sales" },
            ],
        });
        lookup.dblClick(function (data) {
            if (data != null) {
                if (x) {
                    me.data.SalesCodeFrom = data.LookUpValue;
                    me.data.SalesNameFrom = data.LookUpValueName;
                } else {
                    me.data.SalesCodeTo = data.LookUpValue;
                    me.data.SalesNameTo = data.LookUpValueName;
                }
                me.Apply();
            }
        });
    }

    me.printPreview = function () {
        var groupBy = "0"; var type = "0";
        var param1 = "";
        var param2 = "";
        var reportID = ""; var dateFrom = ""; var dateTo = "";
        var branchParam = ""; var branchFrom = ""; var branchName = "";
        var sparam="";

        if (me.data.FiscalMonth == 1) {
            param1 = me.data.FiscalYear + "01"
            param2 = me.data.FiscalYear + "12"
            if ($('#isC3').prop('checked') == true) reportID = "OmRpStatSalB";
            else if ($('#isC2').prop('checked') == true) reportID = "OmRpStatSal";
        }
        else if (me.data.FiscalMonth == 4) {
            if (me.options == "0") {
                param1 = me.data.FiscalYear + "04"
                param2 = (me.data.FiscalYear + 1) + "03"
                if (me.optionss == "0") {
                    if (me.optionsT == "0")
                        reportID = "OmRpStatSalC";
                    else
                        reportID = "OmRpStatSalC1";
                }
                else if (me.optionss == "1") {
                    if (me.optionsT == "0")
                        reportID = "OmRpStatSalA";
                    else
                        reportID = "OmRpStatSalA1";
                }

            }
            else {
                param1 = me.data.FiscalYear + "01";
                param2 = me.data.FiscalYear + "12";
                if (me.optionss == "0") {
                    if (me.optionsT == "0")
                        reportID = "OmRpStatSalB";
                    else
                        reportID = "OmRpStatSalC1";
                }
                else if (me.optionss == "1") {
                    if (me.optionsT == "0")
                        reportID = "OmRpStatSal";
                    else
                        reportID = "OmRpStatSalA1";
                }
            }
        }

        if (me.optionsK == '1') groupBy = "1"; else groupBy = "0";
        if (me.data.Report == 0) type = "1"; else type = "0";

        if ($('#isC4').prop('checked') == true) {
            branchFrom = me.data.BranchCodeFrom;
            branchParam = me.data.BranchCodeFrom;
            branchName = " - " + me.data.BranchNameFrom
        }
        else {
            branchParam = "All";
        }

        if ($('#isC1').prop('checked') == true) {
            dateFrom = moment(me.data.DateFrom).format('YYYYMMDD');
            dateTo = moment(me.data.DateTo).format('YYYYMMDD');
        }

        if (dateFrom == "") {
            sparam = "DARI TANGGAL : " + moment(me.data.DateFrom).format("DD-MMM-YYYY") + " S/D " + moment(me.data.DateTo).format("dd-MMM-yyyy"), "", "", branchParam + branchName;
            if (me.data.Report == 0) sparam =[ "DALAM RUPIAH  -  TAHUN : " + me.data.FiscalYear + ", nilai dalam x Rp.1000,-",  branchParam + branchName];
        }
        else {
            sparam = "DALAM UNIT  -  TAHUN : " + me.data.FiscalYear, "", "Periode : " + moment(me.data.DateFrom).format("DD-MMM-YYYY") + " s/d " + moment(me.data.DateTo).format("DD-MMM-YYYY"), branchParam + branchName;
            if (me.data.Report == 0) sparam = "DALAM RUPIAH  -  TAHUN : " + me.data.FiscalYear + ", nilai dalam x Rp.1000,-", "", "Periode : " + moment(me.data.DateFrom).format("DD-MMM-YYYY") + " s/d " + moment(me.data.DateTo).format("DD-MMM-YYYY"), branchParam + branchName;
        }

        var prm = [
                   // me.data.CompanyCode,
                    me.data.ModelCodeFrom,
                    me.data.ModelCodeTo,
                    me.data.SalesCodeFrom,
                    me.data.SalesCodeTo,
                    param1,
                    param2,
                    groupBy,
                    type,
                    me.data.BranchCodeFrom,
                    dateFrom,
                    dateTo,
                   ($('#isC3').prop('checked') == true) ? "SalesModelCode" : "Sales",
                   //"Sales",
                   (me.optionsT = '0') ? "1" : "0"
                  // "1"
        ];
        Wx.showPdfReport({
            id: reportID,
            pparam: prm.join(','),
            textprint: true,
            rparam: sparam,
            type: "devex"
        });
    }

    me.initialize = function () {
        me.data = {};
        me.change = false;
        me.data.Report = '0';
        me.data.FiscalYear = "2014";
        $http.get('breeze/sales/CurrentUserInfo').
          success(function (dl, status, headers, config) {
              me.data.CompanyCode = dl.CompanyCode;
              me.data.BranchCode = dl.BranchCode;
          });
        $http.get('breeze/sales/fiscalMonth').
         success(function (dl, status, headers, config) {
             me.data.FiscalMonth = dl.FiscalMonth;
         });
        //alert(new Date().getMonth());
        $('#BranchCode').attr('disabled', true);
        $('#btnBranchCode').attr('disabled', true);
        $('#SalesCodeFrom').attr('disabled', true);
        $('#btnSalesCodeFrom').attr('disabled', true);
        $('#SalesCodeTo').attr('disabled', true);
        $('#btnSalesCodeTo').attr('disabled', true);

        $('#ModelCodeFrom').attr('disabled', true);
        $('#ModelCodeTo').attr('disabled', true);
        $('#btnModelCodeFrom').attr('disabled', true);
        $('#btnModelCodeTo').attr('disabled', true);

        me.isPrintAvailable = true;
    }

    $('#isC4').on('change', function (e) {
        if ($('#isC4').prop('checked') == true) {
            $('#BranchCode').removeAttr('disabled');
            $('#btnBranchCode').removeAttr('disabled');
        } else {
            $('#BranchCode').attr('disabled', true);
            $('#btnBranchCode').attr('disabled', true);
            $('#BranchCode').val("");
            $('#BranchName').val("");
        }
        me.Apply();
    })

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
            $('#SalesCodeFrom').removeAttr('disabled');
            $('#btnSalesCodeFrom').removeAttr('disabled');
            $('#SalesCodeTo').removeAttr('disabled');
            $('#btnSalesCodeTo').removeAttr('disabled');
        } else {
            $('#SalesCodeFrom').attr('disabled', true);
            $('#btnSalesCodeFrom').attr('disabled', true);
            $('#SalesCodeTo').attr('disabled', true);
            $('#btnSalesCodeTo').attr('disabled', true);
            $('#SalesCodeFrom').val("");
            $('#SalesNameFrom').val("");
            $('#SalesCodeTo').val("");
            $('#SalesNameTo').val("");
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
            $('#DateFrom').prop('readonly', true);
            me.data.DateTo = undefined;
            $('#DateTo').prop('readonly', true);
        }
        me.Apply();
    })

    me.start();
    me.options = '0';
    me.optionss = '0';
    me.optionsK = '0';
    me.optionsT = '0';
}


$(document).ready(function () {
    var options = {
        title: "Report Statistik Salesman",
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
                             type: "optionbuttons",
                             name: "tabpageoptions",
                             model: "options",
                             cls: "span3",
                             items: [
                                 { name: "0", text: "Tahun Fiscal" },
                                 { name: "1", text: "Tahun Kalender" },
                             ]
                         },
                         { name: 'isC3', type: 'check', text: 'Model', cls: 'span1 full', float: 'left' },
                          {
                              text: "",
                              type: "controls",
                              items: [
                                  
                                  { name: "ModelCodeFrom", cls: "span3", type: "popup", btnName: "btnModelCodeFrom", click: "ModelCodeFrom()", disable: "data.isActive == false" },
                                  { name: "ModelNameFrom", cls: "span4", type: "text", readonly: true },
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
                               text: "Kriteria",
                               type: "optionbuttons",
                               name: "tabpageoptions",
                               model: "optionsK",
                               cls: "span3",
                               items: [
                                   { name: "0", text: "Salesman" },
                                   { name: "1", text: "Kelompok AR" },
                               ]
                           },
                           { name: 'isC2', type: 'check', text: '', cls: 'span1 full', float: 'left' },
                        {
                            text: "",
                            type: "controls",
                            items: [
                                
                                { name: "SalesCodeFrom", cls: "span3", type: "popup", btnName: "btnSalesCodeFrom", click: "sales(1)", disable: "data.isActive == false" },
                                { name: "SalesNameFrom", text: "", cls: "span4", type: "text", readonly: true },
                            ]
                        },
                         {
                             text: "",
                             type: "controls",
                             items: [
                                 { name: "SalesCodeTo", cls: "span3", type: "popup", btnName: "btnSalesCodeTo", click: "sales()", disable: "data.isActive == false" },
                                 { name: "SalesNameTo", text: "", cls: "span4", type: "text", readonly: true },
                             ]
                         },
                          { name: 'isC4', type: 'check', text: 'Branch', cls: 'span1 full', float: 'left' },
                          {
                              text: "",
                              type: "controls",
                              items: [
                                 
                                  { name: "BranchCodeFrom", cls: "span3", type: "popup", btnName: "btnBranchCode", click: "BranchCode()", disable: "data.isActive == false" },
                                  { name: "BranchNameFrom", text: "", cls: "span4", type: "text", readonly: true },
                              ]
                          },
                          { name: 'isC1', type: 'check', text: 'Tanggal :', cls: 'span1', float: 'left' },
                          {
                              text: "",
                              type: "controls",
                              items: [
                                  
                                   { name: "DateFrom", text: "", cls: "span3", type: "ng-datepicker" },
                                  { name: "DateTo", text: "", cls: "span3", type: "ng-datepicker" },
                              ]
                          },
                         { name: "Report", opt_text: "", cls: "span3", type: "select2", text: "Report", datasource: "Status" },
                        {
                            type: "optionbuttons",
                            name: "tabpageoptions",
                            model: "optionss",
                            text: "Urut Berdasarkan",
                            cls: "span8",
                            items: [
                                { name: "0", text: "Model" },
                                { name: "1", text: "Salesman" },
                            ]
                        },
                         {
                             type: "optionbuttons",
                             name: "tabpageoptions",
                             model: "optionsT",
                             text: "Tipe Report",
                             cls: "span8",
                             items: [
                                 { name: "0", text: "Detail" },
                                 { name: "1", text: "Total" },
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