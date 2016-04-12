"use strict"; //Reportid OmRpMst001
function spRptMstSales($scope, $http, $injector) {

    var me = $scope;

    $injector.invoke(BaseController, this, { $scope: me });
    me.Status = [
        { "value": '0', "text": 'Aktif' },
        { "value": '1', "text": 'Tidak Aktif' },
    ];
    me.ModelCode = function () {
        var lookup = Wx.blookup({
            name: "ModelCodeLookup",
            title: "Kode Model Akhir",
            manager: spSalesManager,
            query: "ModulBrowse",
            defaultSort: "SalesModelCode asc",
            columns: [
                { field: "SalesModelCode", title: "Model Code" },
            ],
        });
        lookup.dblClick(function (data) {
            if (data != null) {
                me.data.ModelCode = data.SalesModelCode;
                me.data.ModelName = data.SalesModelDesc;
                me.Apply();
            }
        });

    }

    me.Wiraniaga = function () {
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
                me.data.Wiraniaga = data.EmployeeName;
                me.Apply();
            }
        });
    }

    me.printPreview = function () {
        var reportID = ""; var dateFrom = ""; var dateTo = ""; var SDate = ""; var EDate = "";
        var PaidDateFrom = ""; var PaidDateTo = ""; var SODateFrom = ""; var SODateTo = '';
        var BDateFrom = ''; var BDateTo = ''; var MCode = ''; var emp = ''; var sts = '';
        var PaidFrom = ""; var PaidTo = ""; var SOFrom = ""; var SOTo = '';
        var BFrom = ''; var BTo = '';
        if ($('#isC1').prop('checked') == true) {
            MCode = me.data.ModelCode
        }
        if ($('#isC2').prop('checked') == true) {
            SODateFrom = moment(me.data.SODateFrom).format('DD-MMM-YYYY');
            SODateTo = moment(me.data.SODateTo).format('DD-MMM-YYYY');
            SOFrom = moment(me.data.SODateFrom).format('YYYYMMDD');
            SOTo = moment(me.data.SODateTo).format('YYYYMMDD');
        }
        if ($('#isC3').prop('checked') == true) {
            PaidDateFrom = moment(me.data.PaymentDateFrom).format('DD-MMM-YYYY');
            PaidDateTo = moment(me.data.PaymentDateTo).format('DD-MMM-YYYY');
            PaidFrom = moment(me.data.PaymentDateFrom).format('YYYYMMDD');
            PaidTo = moment(me.data.PaymentDateTo).format('YYYYMMDD');
        }
        if ($('#isC4').prop('checked') == true) {
            var BDateFrom = moment(me.data.BirthDateFrom).format('DD-MMM-YYYY');
            var BDateTo = moment(me.data.BirthDateTo).format('DD-MMM-YYYY');
            var BFrom = moment(me.data.BirthDateFrom).format('YYYYMMDD');
            var BTo = moment(me.data.BirthDateTo).format('YYYYMMDD');
        }
        if ($('#isC5').prop('checked') == true) {
            emp = me.data.Wiraniaga
        }
        if ($('#isC6').prop('checked') == true) {
            if (me.data.Status == '0')
                sts = "1";
            else sts = "0";
        }
        var sortby = "";
        if (sortby == "") {
            if ($('#isC7').prop('checked') == true && sortby == "") {
                sortby = "SalesModelCode"
            }

            if ($('#isC8').prop('checked') == true && sortby == "") {
                sortby = "EmployeeName "
            } else if ($('#isC8').prop('checked') == true && sortby != "") sortby += ", EmployeeName "

            if ($('#isC9').prop('checked') == true && sortby == "") {
                sortby ="CustomerCode";
            } else if ($('#isC9').prop('checked') == true && sortby != "")  sortby += ", CustomerCode";

            if ($('#isC10').prop('checked') == true && sortby == "") {
                sortby = "FinalPaymentDate";
            } else if ($('#isC10').prop('checked') == true && sortby != "") sortby += ", FinalPaymentDate";
        }
        if (sortby != "") sortby += " asc"
 
        var url = "om.api/Report/CustomerListSalesUnit?";
        var params = "&CompanyCode=" + $('[name="CompanyCode"]').val();
        params += "&BranchCode=" + $('[name="BranchCode"]').val();
        params += "&SalesModelCode=" + MCode;
        params += "&PaidDateFrom=" + PaidDateFrom;
        params += "&PaidDateTo=" + PaidDateTo;
        params += "&SODateFrom=" + SODateFrom;
        params += "&SODateTo=" + SODateTo;
        params += "&BirthDateFrom=" + BDateFrom;
        params += "&BirthDateTo=" + BDateTo;
        params += "&Employee=" + emp;
        params += "&Status=" + sts;
        params += "&SortBy=" + sortby;
        params += "&SOFrom=" + SOFrom;
        params += "&SOTo=" + SOTo;
        params += "&PaidFrom=" + PaidFrom;
        params += "&PaidTo=" + PaidTo;
        params += "&BirthFrom=" + BFrom;
        params += "&BirthTo=" + BTo;
        url = url + params;
        window.location = url;
    }

    me.initialize = function () {
        me.data = {};
        me.change = false;
        $http.get('breeze/sales/CurrentUserInfo').
          success(function (dl, status, headers, config) {
              me.data.CompanyCode = dl.CompanyCode;
              me.data.BranchCode = dl.BranchCode;
          });

        $('#ModelCode').attr('disabled', true);
        $('#btnModelCode').attr('disabled', true);
        $('#SODateFrom').attr('disabled', true);
        $('#SODateTo').attr('disabled', true);
        $('#PaymentDateFrom').attr('disabled', true);
        $('#PaymentDateTo').attr('disabled', true);
        $('#BirthDateFrom').attr('disabled', true);
        $('#BirthDateTo').attr('disabled', true);
        $('#Wiraniaga').attr('disabled', true);
        $('#btnWiraniaga').attr('disabled', true);
        $('#Status').attr('disabled', true);
        me.isPrintAvailable = true;
    }
    $('#isC1').on('change', function (e) {
        if ($('#isC1').prop('checked') == true) {
            $('#ModelCode').removeAttr('disabled');
            $('#btnModelCode').removeAttr('disabled');
        } else {
            $('#ModelCode').val("");
            $('#ModelCode').attr('disabled', true);
            $('#btnModelCode').attr('disabled', true);
        }
        me.Apply();
    })
    $('#isC2').on('change', function (e) {
        if ($('#isC2').prop('checked') == true) {
            me.data.SODateFrom = me.now();
            me.data.SODateTo = me.now();
            $('#SODateFrom').removeAttr('disabled');
            $('#SODateTo').removeAttr('disabled');
        } else {
            me.data.SODateFrom = undefined;
            me.data.SODateTo = undefined;
            $('#SODateFrom').attr('disabled', true);
            $('#SODateTo').attr('disabled', true);
        }
        me.Apply();
    })
    $('#isC3').on('change', function (e) {
        if ($('#isC3').prop('checked') == true) {
            me.data.PaymentDateFrom = me.now();
            me.data.PaymentDateTo = me.now();
            $('#PaymentDateFrom').removeAttr('disabled');
            $('#PaymentDateTo').removeAttr('disabled');
        } else {
            me.data.PaymentDateFrom = undefined;
            me.data.PaymentDateTo = undefined;
            $('#PaymentDateFrom').attr('disabled', true);
            $('#PaymentDateTo').attr('disabled', true);
        }
        me.Apply();
    })
    $('#isC4').on('change', function (e) {
        if ($('#isC4').prop('checked') == true) {
            me.data.BirthDateFrom = me.now();
            me.data.BirthDateTo = me.now();
            $('#BirthDateFrom').removeAttr('disabled');
            $('#BirthDateTo').removeAttr('disabled');
        } else {
            me.data.BirthDateFrom = undefined;
            me.data.BirthDateTo = undefined;
            $('#BirthDateFrom').attr('disabled', true);
            $('#BirthDateTo').attr('disabled', true);
        }
        me.Apply();
    })
    $('#isC5').on('change', function (e) {
        if ($('#isC5').prop('checked') == true) {
            $('#Wiraniaga').removeAttr('disabled');
            $('#btnWiraniaga').removeAttr('disabled');
        } else {
            $('#Wiraniaga').val("");
            $('#Wiraniaga').attr('disabled', true);
            $('#btnWiraniaga').attr('disabled', true);
        }
        me.Apply();
    })
    $('#isC6').on('change', function (e) {
        if ($('#isC6').prop('checked') == true) {
            $('#Status').removeAttr('disabled');
            me.data.Status = '0';
        } else {
            me.data.Status = undefined;
            $('#Status').attr('disabled', true);
        }
        me.Apply();
    })
    me.start();

}


$(document).ready(function () {
    var options = {
        title: "Report Customer List - Sales Unit",
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
                             text: "Tipe(Sales Model)",
                             type: "controls",
                             items: [
                                       { name: 'isC1', type: 'check', text: '', cls: 'span1', float: 'left' },
                                       { name: "ModelCode", cls: "span4", type: "popup", btnName: "btnModelCode", click: "ModelCode()", disable: "data.isActive == false" },
                             ]
                         },
                        {
                            text: "Tgl SO(SO Date)",
                            type: "controls",
                            items: [
                                      { name: 'isC2', type: 'check', text: '', cls: 'span1 full', float: 'left' },
                                      { name: "SODateFrom", text: "", cls: "span3", type: "ng-datepicker" },
                                      { name: "SODateTo", text: "", cls: "span3", type: "ng-datepicker" },
                            ]
                        },
                        ////////////
                        
                        {
                            text: "Tgl Lunas",
                            type: "controls",
                            items: [
                                      { name: 'isC3', type: 'check', text: '', cls: 'span1 ', float: 'left' },
                                      { name: "PaymentDateFrom", text: "", cls: "span3", type: "ng-datepicker" },
                                      { name: "PaymentDateTo", text: "", cls: "span3", type: "ng-datepicker" },
                            ]
                        },
                        ///////////////
                        {
                            text: "Tgl Lahir",
                            type: "controls",
                            items: [
                                      { name: 'isC4', type: 'check', text: '', cls: 'span1 full', float: 'left' },
                                      { name: "BirthDateFrom", text: "", cls: "span3", type: "ng-datepicker" },
                                      { name: "BirthDateTo", text: "", cls: "span3", type: "ng-datepicker" },
                            ]
                        },
                        //////////
                         {
                             text: "Wiraniaga",
                             type: "controls",
                             items: [
                        { name: 'isC5', type: 'check', text: '', cls: 'span1 full', float: 'left' },
                        { name: "Wiraniaga", cls: "span4", type: "popup", btnName: "btnWiraniaga", click: "Wiraniaga()", disable: "data.isActive == false" },
                        ]
                         },
                          {
                              text: "Status",
                              type: "controls",
                              items: [
                                        { name: 'isC6', type: 'check', text: '', cls: 'span1 full', float: 'left' },
                                        { name: "Status", opt_text: "", cls: "span4 full", type: "select2", text: "", datasource: "Status" },

                              ]
                          },

                        {
                            type: "controls",
                            cls: "span3",
                            items: [
                                 { name: 'isC7', type: 'check', cls: 'span1' },
                                 { type: "label", text: "Tipe (Sales Model)", cls: "span7 mylabel" },
                            ]
                        },
                         {
                             type: "controls",
                             cls: "span3",
                             items: [
                                  { name: 'isC8', type: 'check', cls: 'span1' },
                                 { type: "label", text: "Wiraniaga (Salesman)", cls: "span7 mylabel" },
                             ]
                         },
                          {
                              type: "controls",
                              cls: "span3",
                              items: [
                                    { name: 'isC9', type: 'check',  cls: 'span1' },
                                   { type: "label", text: 'Pelanggan (Customer)', cls: "span7 mylabel" },
                              ]
                          },
                           {
                               type: "controls",
                               cls: "span3",
                               items: [
                                    { name: 'isC10', type: 'check',  cls: 'span1' },
                                    { type: "label", text: 'Tgl Lunas (Final Payment Date)', cls: "span7 mylabel" },
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