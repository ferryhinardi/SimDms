"use strict"; //Reportid OmRpMst001
function spRptMstSales($scope, $http, $injector) {

    var me = $scope;

    $injector.invoke(BaseController, this, { $scope: me });

    me.Status = [
        { "value": '0', "text": 'ALL' },
        { "value": '1', "text": 'WHOLESALES' },
        { "value": '2', "text": 'DIRECTSALES' },

    ];
    me.Report = [
        { "value": '0', "text": 'PER UNIT (DETAIL)' },
        { "value": '1', "text": 'PER MODEL (SUMMARY)' },
        { "value": '2', "text": 'PER MODEL (SUMMARY + HOLDING)' },

    ];
    $http.post('gn.api/combo/Organizations').
   success(function (data, status, headers, config) {
       me.Company = data;
   });
    $http.post('gn.api/combo/Branchs').
    success(function (data, status, headers, config) {
        me.Branch = data;
    }); 

    me.SalesFrom = function () {
        var lookup = Wx.blookup({
            name: "CityLookup",
            title: "Lookup Sales",
            manager: gnManager,
            query: new breeze.EntityQuery().from("LookUpDtlAll").withParameters({ param: "GPAR" }),
            defaultSort: "LookUpValue asc",
            columns: [
                { field: "LookUpValue", title: "Kode Sales" },
                { field: "LookUpValueName", title: "Deskripsi" }
            ]
        });
        lookup.dblClick(function (data) {
            if (data != null) {
                me.isSave = false;
                me.data.SalesCodeFrom = data.LookUpValue;
                me.data.SalesNameFrom = data.LookUpValueName;
                me.Apply();
            }
        });

    }

    me.SalesTo = function () {
        var lookup = Wx.blookup({
            name: "CityLookup",
            title: "Lookup Sales",
            manager: gnManager,
            query: new breeze.EntityQuery().from("LookUpDtlAll").withParameters({ param: "GPAR" }),
            defaultSort: "LookUpValue asc",
            columns: [
                { field: "LookUpValue", title: "Kode Sales" },
                { field: "LookUpValueName", title: "Deskripsi" }
            ]
        });
        lookup.dblClick(function (data) {
            if (data != null) {
                me.isSave = false;
                me.data.SalesCodeTo = data.LookUpValue;
                me.data.SalesNameTo = data.LookUpValueName;
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
        var param = "";
        var param2 = ""; var codeLineWidth = ""; var isBranch = false;
        var reportID = ""; var dateFrom = ""; var dateTo = ""; var pdok = "";
        var branchParam = ""; var branchFrom = ""; var branchName = "";
        var sparam = "";
 
        if (parseInt(moment(me.data.DateFrom).format('YYYYMMDD')) > parseInt(moment(me.data.DateTo).format('YYYYMMDD'))) {
            MsgBox("Tanggal Mulai harus lebih kecil atau sama dengan tanggal akhir !", MSG_ERROR);
            return;
        }

        if ($('#isC1').prop('checked') == true)
        { param = "1"; }
        else if ($('#isC1').prop('checked') == false)
        { param = "0"; }

        if (me.data.SalesType =="0") {
            param2 = "";
        }
        else if (me.data.SalesType == "1") {
            param2 = "0";
        }
        else if (me.data.SalesType == "2") {
            param2 = "1";
        }

        if (me.options == "0") {
            pdok = "0";
        }
        else if (me.options == "1") {
            pdok = "1";
        }
        else if (me.options == "2") {
            pdok = "2";
        }

        $http.get('breeze/sales/isBranch').
         success(function (dl, status, headers, config) {
             isBranch = dl.IsBranch;
         });

        if (me.data.Report == "0") {
            reportID = "OmRpLabaRugi001";
            codeLineWidth = "W233";

            if (isBranch) {
                me.data.BranchCodeFrom = me.data.BranchCodeTo = me.data.BranchCode;
            }
            else {
                if ($('#isC2').prop('checked') == true) {
                    me.data.BranchCodeFrom = me.data.BranchCodeFrom;
                    me.data.BranchCodeTo = me.data.BranchCodeFrom
                }
                else me.data.BranchCodeFrom = me.data.BranchCodeTo = undefined;
            }
        }
        else if (me.data.Report == "1") {
            reportID = "OmRpLabaRugi002";
            codeLineWidth = "W96";
        }
        else {
            reportID = "OmRpLabaRugi003";
            codeLineWidth = "W136";
        }
        dateFrom = moment(me.data.DateFrom).format('YYYYMMDD');
        dateTo = moment(me.data.DateTo).format('YYYYMMDD');
        var prm = "";
        if (me.data.Report == "0")
            prm = prm = [
                   'companycode',
                    me.data.BranchCodeFrom,
                    me.data.BranchCodeTo,
                    dateFrom,
                    dateTo,
                    param2,
                    me.data.ModelCodeFrom,
                    me.data.ModelCodeTo,
                    param,
                    pdok
            ];
        else if (me.data.Report == "1")
            prm = [
                    dateFrom,
                    dateTo,
                    param2,
                    me.data.ModelCodeFrom,
                    me.data.ModelCodeTo,
                    param
            ];
        else
            prm = [
                     'companycode',
                     "%",
                   dateFrom,
                   dateTo,
                   param2,
                   me.data.ModelCodeFrom,
                   me.data.ModelCodeTo,
                   param
            ];

        //Sent parameter for report
        if ($('#isC1').prop('checked') == true)
            sparam = "DARI TANGGAL : " + moment(me.data.DateFrom).format("DD-MMM-YYYY") + " S/D " + moment(me.data.DateTo).format("DD-MMM-YYYY");

        //var prm = [
        //           'companycode',
        //            me.data.BranchCodeFrom,
        //            me.data.BranchCodeTo,
        //            dateFrom,
        //            dateTo,
        //            param2,
        //            me.data.ModelCodeFrom,
        //            me.data.ModelCodeTo,
        //            param,
        //            pdok
        //];
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
        me.data.SalesType = '0';
        me.data.Report = '0';
        $('#isC1').prop('checked', true);
        //$http.get('breeze/sales/CurrentUserInfo').
        //  success(function (dl, status, headers, config) {
        //      me.data.CompanyCode = dl.CompanyCode;
        //      me.data.BranchCode = dl.BranchCode;

        //  });
        //if (new Date(Date.now()).getMonth() >= 0 || new Date(Date.now()).getMonth() <= 5) {
        //    me.data.DateFrom = 7 + '/' + 1 + '/' + (new Date().getFullYear() - 1);
        //    me.data.DateTo = 12 + '/' + 31 + '/' + (new Date().getFullYear() - 1);
        //}
        //else {
        //    me.data.DateFrom = 1 + '/' + 1 + '/' + (new Date().getFullYear());
        //    me.data.DateTo = 6 + '/' + 30 + '/' + (new Date().getFullYear());
        //}

        var ym = me.now("YYYY-MM") + "-01";
        me.data.DateFrom = moment(ym);
        me.data.DateTo = moment(ym).add("months", 1).add("days", -1);

        //alert(new Date().getMonth());
        $('#BranchCode').attr('disabled', true);
        $('#CompanyCode').attr('disabled', true);

        $('#SalesCodeFrom').attr('disabled', true);
        $('#SalesCodeTo').attr('disabled', true);
        $('#btnSalesCodeFrom').attr('disabled', true);
        $('#btnSalesCodeTo').attr('disabled', true);

        me.isPrintAvailable = true;
    }

    $('#isC3').on('change', function (e) {
        if ($('#isC3').prop('checked') == true) {
            $('#SalesCodeFrom').removeAttr('disabled');
            $('#SalesCodeTo').removeAttr('disabled');
            $('#btnSalesCodeFrom').removeAttr('disabled');
            $('#btnSalesCodeTo').removeAttr('disabled');
        } else {
            $('#SalesCodeFrom').attr('disabled', true);
            $('#SalesCodeTo').attr('disabled', true);
            $('#btnSalesCodeFrom').attr('disabled', true);
            $('#btnSalesCodeTo').attr('disabled', true);
            $('#SalesCodeFrom').val("");
            $('#SalesCodeTo').val("");
            $('#SalesNameFrom').val("");
            $('#SalesNameTo').val("");
        }
        me.Apply();
    })

    $('#isC2').on('change', function (e) {
        if ($('#isC2').prop('checked') == true) {
            $('#BranchCode').removeAttr('disabled');
            $('#CompanyCode').removeAttr('disabled');
            $http.get('breeze/sales/CurrentUserInfo').
              success(function (dl, status, headers, config) {
                  me.data.CompanyCode = dl.CompanyCode;
                  me.data.BranchCode = dl.BranchCode;
              });
        } else {
            $('#BranchCode').attr('disabled', true);
            $('#CompanyCode').attr('disabled', true);
            me.data.CompanyCode = undefined;
            me.data.BranchCode = undefined;
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

}

$(document).ready(function () {
    var options = {
        title: "Report Laba Rugi V.2",
        xtype: "panels",
        toolbars: [
            { name: "btnPrintPreview", text: "Print", cls: "btn btn-primary", icon: "icon-print", click: "printPreview()", disable: "!isPrintEnable" },
            { name: "btnCancel", text: "Cancel", cls: "btn btn-warning", icon: "icon-remove", click: "cancelOrClose()" },
        ],
        panels: [
            {
                name: "pnlA",
                items: [
                        //{ name: "CompanyCode", model: "data.CompanyCode", text: "Kode Perusahaan", cls: "span4 full", disable: "isPrintAvailable" },
                        //{ name: "BranchCode", model: "data.BranchCode", text: "Kode Cabang", cls: "span4 full", disable: "isPrintAvailable" },
                        { name: 'isC2', type: 'check', text: 'Cabang', cls: 'span1 full', float: 'left' },
                        {
                            text: "",
                            type: "controls",
                            items: [
                                { name: "CompanyCode", opt_text: "", cls: "span7", type: "select2", text: "Company", datasource: "Company" },
                                { name: "BranchCode", opt_text: "", cls: "span7", type: "select2", text: "Branch", datasource: "Branch" },
                            ]
                        },
                         //{
                         //    text: "s/d",
                         //    type: "controls",
                         //    items: [
                         //        { name: "BranchCodeTo", cls: "span3", type: "popup", btnName: "btnBranchCodeTo", click: "BranchTo()", disable: "data.isActive == false" },
                         //        { name: "BranchNameTo", text: "", cls: "span4", type: "text", readonly: true },
                         //    ]
                         //},
                         { name: 'isC1', type: 'check', text: 'Tanggal :', cls: 'span1', float: 'left' },
                          {
                              text: "",
                              type: "controls",
                              items: [

                                   { name: "DateFrom", text: "", cls: "span3", type: "ng-datepicker" },
                                  { name: "DateTo", text: "", cls: "span3", type: "ng-datepicker" },
                              ]
                          },
                           { name: "SalesType", opt_text: "", cls: "span3", type: "select2", text: "Tipe Sales", datasource: "Status" },
                        
                         { name: 'isC3', type: 'check', text: 'Sales Dari/Ke', cls: 'span1 full', float: 'left' },
                          {
                              text: "",
                              type: "controls",
                              items: [

                                  { name: "SalesCodeFrom", cls: "span3", type: "popup", click: "SalesFrom()", disable: "data.isActive == false" },
                                  { name: "SalesNameFrom", cls: "span4", type: "text", readonly: true },
                              ]
                          },
                          {
                              text: "",
                              type: "controls",
                              items: [
                                  { name: "SalesCodeTo", cls: "span3", type: "popup", click: "SalesTo()", disable: "data.isActive == false" },
                                  { name: "SalesCodeTo", cls: "span4", type: "text", readonly: true },
                              ]
                          },
                          { name: "Report", opt_text: "", cls: "span4 full", type: "select2", text: "Report", datasource: "Report" },
                            {
                                type: "optionbuttons",
                                name: "tabpageoptions",
                                text: "Tgl Berdasarkan",
                                model: "options",
                                cls: "span8",
                                items: [
                                    { name: "0", text: "BPU" },
                                    { name: "1", text: "DO Supplier" },
                                    { name: "2", text: "SJ Supplier)" },
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