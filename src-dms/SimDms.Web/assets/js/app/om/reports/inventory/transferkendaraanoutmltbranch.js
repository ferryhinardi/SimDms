"use strict";

function RptTransferKndrnOutBranch($scope, $http, $injector) {
    var me = $scope;

    $injector.invoke(BaseController, this, { $scope: me });
    me.Status = [
       { "value": '0', "text": 'ALL' },
       { "value": '1', "text": 'OPEN' },
       { "value": '2', "text": 'PRINTED' },
       { "value": '3', "text": 'APPROVED' },
       { "value": '4', "text": 'CANCELED' },
       { "value": '5', "text": 'FINISHED' },
    ];

    me.StatusTrnfIn = [
       { "value": '0', "text": 'ALL' },
       { "value": '1', "text": 'OUTSTANDING' },
       { "value": '2', "text": 'RECEIVED' }
    ];

    me.printPreview = function () {
        var sts = $('#Status').val();
        var stsTrnfIn = $('#StatusTrnfIn').val();
        if (stsTrnfIn == '') {
            MsgBox('Ada data yang belum lengkap', MSG_ERROR);
            return;
        }
        if (sts=='') {
            MsgBox('Ada data yang belum lengkap', MSG_ERROR);
            return;
        }
        else if (sts=="0") {
            sts = "";
        }
        var param = [
                    moment(me.data.FromDate).format('YYYY-MM-DD'),
                    moment(me.data.ToDate).format('YYYY-MM-DD'),
                    sts,
                    stsTrnfIn
        ];

        var rparam = [
           $('#Status').select2('data').text,
           moment(me.data.FromDate).format('DD-MMM-YYYY'),
           moment(me.data.ToDate).format('DD-MMM-YYYY'),
           "2"
        ];

        Wx.showPdfReport({
            id: 'OmRpInvRgs006A',
            pparam: param.join(','),
            rparam: rparam,
            type: "devex"
        });
        console.log(moment(me.data.FromDate).format('YYYY-MM-DD'), moment(me.data.ToDate).format('YYYY-MM-DD'), sts, stsTrnfIn)
    }

    me.initialize = function () {
        me.data = {};
        me.change = false;
        me.data.Status = "0";
        me.data.StatusTrnfIn = "0";
        var d = new Date(Date.now()).getDate();
        var m = new Date(Date.now()).getMonth();
        var y = new Date(Date.now()).getFullYear();
        me.data.FromDate = new Date(y, m, 1);
        me.data.ToDate = new Date(y, m + 1, 0);
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
        title: "Transfer Kendaraan Out Multi Branch",
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
                        { name: "Status", opt_text: "", cls: "span3 full", type: "select2", text: "Status", datasource: "Status" },
                        { name: "StatusTrnfIn", opt_text: "", cls: "span3 full", type: "select2", text: "Status Transfer In", datasource: "StatusTrnfIn" },
                        {
                            text: "Tanggal",
                            type: "controls",
                            cls: "span4",
                            items: [
                                { name: "FromDate", model: "data.FromDate", placeHolder: "Tgl. Awal", cls: "span4", type: 'ng-datepicker' },
                                { name: "ToDate", model: "data.ToDate", placeHolder: "Tgl. Akhir", cls: "span4", type: 'ng-datepicker' },
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
        SimDms.Angular("RptTransferKndrnOutBranch");

    }
});