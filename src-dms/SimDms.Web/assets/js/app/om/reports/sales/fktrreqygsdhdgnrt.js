"use strict";

function RptFakturReqGenerate($scope, $http, $injector) {
    var me = $scope;

    $injector.invoke(BaseController, this, { $scope: me });

    me.RegFrom = function () {
        var lookup = Wx.blookup({
            name: "FakturReqSudahTergenerate4Report",
            title: "Registrasi From",
            manager: spSalesManager,
            query: new breeze.EntityQuery.from('FakturReqSudahTergenerate4Report'),
            defaultSort: "ReqNo desc",
            columns: [
                { field: "ReqNo", title: "No. SO" },
                {
                    field: "ReqDate", title: "Tanggal",
                    template: "#= (ReqDate == undefined) ? '' : moment(ReqDate).format('DD MMM YYYY') #"
                },
            ],
        });
        lookup.dblClick(function (data) {
            if (data != null) {
                me.data.RegFrom = data.ReqNo;
                me.data.RegFromDate = data.ReqDate;
                me.Apply();
            }
        });
    }

    me.RegTo = function () {
        var lookup = Wx.blookup({
            name: "FakturReqSudahTergenerate4Report",
            title: "Registrasi To",
            manager: spSalesManager,
            query: new breeze.EntityQuery.from('FakturReqSudahTergenerate4Report'),
            defaultSort: "ReqNo desc",
            columns: [
                { field: "ReqNo", title: "No. SO" },
                {
                    field: "ReqDate", title: "Tanggal",
                    template: "#= (ReqDate == undefined) ? '' : moment(ReqDate).format('DD MMM YYYY') #"
                },
            ],
        });
        lookup.dblClick(function (data) {
            if (data != null) {
                me.data.RegTo = data.ReqNo;
                me.data.RegToDate = data.ReqDate,
                me.Apply();
            }
        });
    }
    
    me.printPreview = function () {
        if (me.data.RegFromDate > me.data.RegToDate) {
            MsgBox('No. Registrasi awal tidak boleh lebih besar dari No. Registrasi akhir', MSG_ERROR);
            return;
        }
        else {
            if (me.data.chkDate == true) {
                var param = [
                    moment(me.data.FromDate).format('YYYYMMDD'),
                    moment(me.data.ToDate).format('YYYYMMDD'),
                    me.data.RegFrom,
                    me.data.RegTo,
                    1
                ];

                Wx.showPdfReport({
                    id: 'OmRpSalRgs013Web',
                    pparam: param.join(','),
                    rparam: 'PER TANGGAL : ' + moment(me.data.RegFromDate).format('DD MMM YYYY') + ' S/D ' + moment(me.data.RegToDate).format('DD MMM YYYY'),
                    type: "devex"
                });
            }
            else if (me.data.chkDate == false) {
                var param = [
                   moment(me.data.FromDate).format('YYYYMMDD'),
                   moment(me.data.ToDate).format('YYYYMMDD'),
                   me.data.RegFrom,
                   me.data.RegTo,
                   0
                ];

                Wx.showPdfReport({
                    id: 'OmRpSalRgs013Web',
                    pparam: param.join(','),
                    textprint: true,
                    rparam: 'PER TANGGAL : ' + moment(me.data.FromDate).format('DD MMM YYYY') + ' S/D ' + moment(me.data.ToDate).format('DD MMM YYYY'),
                    type: "devex"
                });
            }
        }

    }

    $("[name = 'checkReg']").on('change', function () {
        me.data.isActive = $('#checkReg').prop('checked');
        me.data.RegFrom = "";
        me.data.RegFromDate = "";
        me.data.RegTo = "";
        me.data.RegToDesc = "";
        me.Apply();
    });

    me.initialize = function () {
        me.data = {};
        me.data.chkDate = false;
        me.data.checkReg = false;
        $http.get('breeze/sales/CurrentUserInfo').
          success(function (dl, status, headers, config) {
              me.data.CompanyCode = dl.CompanyCode;
              me.data.BranchCode = dl.BranchCode;

          });
        $http.post('om.api/ReportSales/Periode').
          success(function (e) {
              me.data.FromDate = e.DateFrom;
              me.data.ToDate = e.DateTo;
          });
        me.isPrintAvailable = true;
    }

    me.start();

}

$(document).ready(function () {
    var options = {
        title: "Report Faktur - Faktur Request yang Sudah Digenerate",
        xtype: "panels",
        toolbars: [
            { name: "btnPrintPreview", text: "Print", cls: "btn btn-primary", icon: "icon-print", click: "printPreview()", disable: "!isPrintEnable" },
            { name: "btnCancel", text: "Cancel", cls: "btn btn-warning", icon: "icon-remove", click: "cancelOrClose()" },
        ],
        panels: [
            {
                name: "pnlA",
                items: [
                        { name: "CompanyCode", model: "data.CompanyCode", text: "Kode Perusahaan", cls: "span4 full", disable: "isPrintAvailable", show: false },
                        { name: "BranchCode", model: "data.BranchCode", text: "Kode Cabang", cls: "span4 full", disable: "isPrintAvailable", show: false },
                        {
                            text: "Tanggal",
                            type: "controls",
                            cls: "span8",
                            items: [
                                { name: "chkDate", model: "data.chkDate", cls: "span1", type: "ng-check" },
                                { name: "FromDate", model: "data.FromDate", placeHolder: "Tgl. Awal", cls: "span2", type: 'ng-datepicker', disable: "!data.chkDate" },
                                { name: "ToDate", model: "data.ToDate", placeHolder: "Tgl. Akhir", cls: "span2", type: 'ng-datepicker', disable: "!data.chkDate" },
                            ]
                        },

                        {
                            text: "No. Registrasi",
                            type: "controls",
                            items: [
                                { name: "checkReg", model: "data.checkReg", cls: "span1", type: "ng-check" },
                                { name: "RegFrom", model: "data.RegFrom", cls: "span2", placeHolder: " ", readonly: true, type: "popup", click: "RegFrom()", disable: "!data.checkReg" },
                                { name: "RegFromDate", cls: "span1", placeHolder: " ", readonly: true, show: false },
                                { name: "RegTo", model: "data.RegTo", cls: "span2", placeHolder: " ", readonly: true, type: "popup", click: "RegTo()", disable: "!data.checkReg" },
                                { name: "RegToDate", cls: "span1", placeHolder: " ", readonly: true, show: false },
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
        SimDms.Angular("RptFakturReqGenerate");

    }
});