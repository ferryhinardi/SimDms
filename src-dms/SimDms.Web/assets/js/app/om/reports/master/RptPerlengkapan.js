"use strict"; //Reportid OmRpMst005
function spRptMstSales($scope, $http, $injector) {

    var me = $scope;

    $injector.invoke(BaseController, this, { $scope: me });
    me.Status = [
           { "value": '2', "text": 'All' },
           { "value": '0', "text": 'Tidak Aktif' },
           { "value": '1', "text": 'Aktif' },
    ];

    me.KodePerlengkapan = function () {
        var lookup = Wx.blookup({
            name: "PerlengkapanCodeLookup",
            title: "Kode perlengkapan",
            manager: spSalesManager,
            query: "PerlengkapanCodeLookup",
            defaultSort: "PerlengkapanCode asc",
            columns: [
                { field: "PerlengkapanCode", title: "Kode Perlengkapan" },
                { field: "PerlengkapanName", title: "Nama Perlengkapan" }
            ]
        });
        lookup.dblClick(function (data) {
            if (data != null) {
                me.data.PerlengkapanCode = data.PerlengkapanCode;
                me.data.PerlengkapanName = data.PerlengkapanName;
                me.Apply();
            }
        });

    }

    me.KodePerlengkapanTo = function () {
        var lookup = Wx.blookup({
            name: "PerlengkapanCodeLookup",
            title: "Kode perlengkapan",
            manager: spSalesManager,
            query: "PerlengkapanCodeLookup",
            defaultSort: "PerlengkapanCode asc",
            columns: [
                { field: "PerlengkapanCode", title: "Kode Perlengkapan" },
                { field: "PerlengkapanName", title: "Nama Perlengkapan" }
            ]
        });
        lookup.dblClick(function (data) {
            if (data != null) {
                me.data.PerlengkapanCodeTo = data.PerlengkapanCode;
                me.data.PerlengkapanNameTo = data.PerlengkapanName;
                me.Apply();
            }
        });

    }

    $("[name='PerlengkapanCode']").on('blur', function () {
        if (me.data.PerlengkapanCode != "") {
            $http.post('om.api/MstModelPerlengkapan/PerlengkapanCode', me.data).
               success(function (data, status, headers, config) {
                   if (data.success) {
                       me.data.PerlengkapanCode = data.data.PerlengkapanCode;
                       me.data.PerlengkapanName = data.data.PerlengkapanName;
                       me.Apply();
                   }
                   else {
                       me.data.PerlengkapanCode = "";
                       me.data.PerlengkapanName = "";
                       me.KodePerlengkapan();
                   }
               }).
               error(function (data, status, headers, config) {
                   alert('error');
               });
        }
    });

    $("[name='PerlengkapanCodeTo']").on('blur', function () {
        if (me.data.PerlengkapanCodeTo != "") {
            $http.post('om.api/MstModelPerlengkapan/PerlengkapanCode', me.data).
               success(function (data, status, headers, config) {
                   if (data.success) {
                       me.data.PerlengkapanCodeTo = data.data.PerlengkapanCode;
                       me.data.PerlengkapanNameTo = data.data.PerlengkapanName;
                       me.Apply();
                   }
                   else {
                       me.data.PerlengkapanCodeTo = "";
                       me.data.PerlengkapanNameTo = "";
                       me.KodePerlengkapanTo();
                   }
               }).
               error(function (data, status, headers, config) {
                   alert('error');
               });
        }
    });


    me.printPreview = function () {


        if (me.data.Status == '2') {
            me.data.Status = '';
        }


        var prm = [

                    me.data.KodePerlengkapan,
                    me.data.KodePerlengkapanTo,
                    me.data.Status
        ];
        Wx.showPdfReport({
            id: "OmRpMst005",
            pparam: prm.join(','),
            rparam: "semua",
            type: "devex"
        });
    }

    $("[name = 'isActive']").on('change', function () {
        me.data.isActive = $('#isActive').prop('checked');
        me.data.PerlengkapanCode = "";
        me.data.PerlengkapanName = "";
        me.data.PerlengkapanCodeTo = "";
        me.data.PerlengkapanNameTo = "";
        me.Apply();
    });

    me.initialize = function () {
        me.data = {};
        $('#isActive').prop('checked', false);
        me.data.isActive = false;
        me.data.Status = '2';
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
        title: "Report Perlengkapan",
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
                        { name: 'isActive', type: 'check', cls: 'span2', text: "Kode Perlengkapan", float: 'left' },
                        {
                            text: "",
                            type: "controls",
                            items: [
                                { name: "PerlengkapanCode", cls: "span2", type: "popup", btnName: "btnKodePerlengkapan", click: "KodePerlengkapan()", disable: "data.isActive == false" },
                                { name: "PerlengkapanName", cls: "span4", readonly: true },
                            ]
                        },
                        {
                            text: "",
                            type: "controls",
                            items: [
                                { name: "PerlengkapanCodeTo", cls: "span2", type: "popup", btnName: "btnKodePerlengkapanTo", click: "KodePerlengkapanTo()", disable: "data.isActive == false" },
                                { name: "PerlengkapanNameTo", cls: "span4", readonly: true },
                            ]
                        },
                        { name: "Status", opt_text: "", cls: "span4", type: "select2", text: "Status", datasource: "Status" },
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