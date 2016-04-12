"use strict";

function RptRekapHarian($scope, $http, $injector) {
    var me = $scope;

    $injector.invoke(BaseController, this, { $scope: me });

    me.DealerCode = function () {
        var lookup = Wx.blookup({
            name: "RekapHarian4Report",
            title: "Dealer",
            manager: spSalesManager,
            query: new breeze.EntityQuery.from('RekapHarian4Report'),
            defaultSort: "CustomerCode asc",
            columns: [
                { field: "CustomerCode", title: "Kode Dealer" },
                { field: "CustomerName", title: "Nama Dealer" },
            ],
        });
        lookup.dblClick(function (data) {
            if (data != null) {
                me.data.DealerCode = data.CustomerCode;
                me.data.DealerDesc = data.CustomerName;
                me.Apply();
            }
        });
        console.log(data);
    }    

    me.printPreview = function () {
        if (me.data.chkDealer == true) {
            var param = [
                    moment(me.data.Date).format('DD'),
                    moment(me.data.Date).format('MM'),
                    moment(me.data.Date).format('YYYY'),
                    me.data.DealerCode,
            ];

            Wx.showPdfReport({
                id: 'OmRpSalRgs026',
                pparam: param.join(','),
                textprint: true,
                rparam: 'Print Data Dealer ' + me.data.DealerCode,
                type: "devex"
            });
        }
        else {
            var param = [
                    moment(me.data.Date).format('DD'),
                    moment(me.data.Date).format('MM'),
                    moment(me.data.Date).format('YYYY'),
                    me.data.DealerCode,
            ];

            Wx.showPdfReport({
                id: 'OmRpSalRgs026',
                pparam: param.join(','),
                rparam: 'Print All Dealer',
                type: "devex"
            });
        }       
        
    }

    $("[name = 'chkDealer']").on('change', function () {
        me.data.isActive = $('#chkDealer').prop('checked');
        me.data.DealerCode = "";
        me.data.DealerDesc = "";
        me.Apply();
    });    

    me.initialize = function () {
        me.data = {};
        me.currentDate = me.now();
        me.data.Date = me.currentDate;
        me.data.chkDate = false;
        me.data.checkReg = false;
        $http.get('breeze/sales/CurrentUserInfo').
          success(function (dl, status, headers, config) {
              me.data.CompanyCode = dl.CompanyCode;
              me.data.BranchCode = dl.BranchCode;

          });

        me.isPrintAvailable = true;
    }

    me.start();

}

$(document).ready(function () {
    var options = {
        title: "Report Rekapitulasi Harian",
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

                        { name: "chkDealer", model: "data.chkDealer", text: "Dealer", cls: "span4 full", type: "ng-check" },
                        {
                            text: "",
                            type: "controls",
                            items: [


                                { name: "DealerCode", model: "data.DealerCode", cls: "span2", placeHolder: " ", readonly: true, type: "popup", click: "DealerCode()", disable: "!data.chkDealer" },
                                { name: "DealerDesc", model: "data.DealerDesc", cls: "span6", placeHolder: " ", readonly: true, disable: "!data.chkDealer" }
                            ]
                        },

                        {
                            text: "Tanggal",
                            type: "controls",
                            cls: "span4",
                            items: [
                                { name: "Date", model: "data.Date", placeHolder: "Tanggal", cls: "span4", type: 'ng-datepicker' },
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
        SimDms.Angular("RptRekapHarian");

    }
});