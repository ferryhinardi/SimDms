"use strict"; //Reportid OmRpMst001
function spRptMstSales($scope, $http, $injector) {

    var me = $scope;

    $injector.invoke(BaseController, this, { $scope: me });

    $http.post('om.api/Combo/RefferenceType?').
    success(function (data, status, headers, config) {
        me.RefferenceType = data;
    });


    me.Status = [
        { "value": '2', "text": 'All' },
        { "value": '0', "text": 'Tidak Aktif' },
        { "value": '1', "text": 'Aktif' }
    ];

    me.RefferenceCode = function () {
        var lookup = Wx.blookup({
            name: "RefferenceCodeLookup",
            title: "Refference Code",
            manager: spSalesManager,
            query: "RefferenceCodeLookup?RefferenceType=" + me.data.RefferenceType,
            defaultSort: "SalesModelYear asc",
            columns: [
                { field: "RefferenceCode", title: "Reff. Code" },
                { field: "RefferenceDesc1", title: "Reff. Desc" },
            ],
        });
        lookup.dblClick(function (data) {
            if (data != null) {
                me.data.RefferenceCode = data.RefferenceCode;
                me.data.RefferenceDesc = data.RefferenceDesc1;
                me.Apply();
            }
        });

    }

    me.RefferenceCodeTo = function () {
        var lookup = Wx.blookup({
            name: "RefferenceCodeLookup",
            title: "Refference Code",
            manager: spSalesManager,
            query: "RefferenceCodeLookup?RefferenceType=" + me.data.RefferenceType,
            defaultSort: "SalesModelYear asc",
            columns: [
                { field: "RefferenceCode", title: "Reff. Code" },
                { field: "RefferenceDesc1", title: "Reff. Desc" },
            ],
        });
        lookup.dblClick(function (data) {
            if (data != null) {
                me.data.RefferenceCodeTo = data.RefferenceCode;
                me.data.RefferenceDescTo = data.RefferenceDesc1;
                me.Apply();
            }
        });

    }

    $("[name='RefferenceCode']").on('blur', function () {
        if (me.data.RefferenceCode != "") {
            $http.post('om.api/MstRefference/koderef', me.data).
               success(function (data, status, headers, config) {
                   if (data.success) {
                       me.data.RefferenceCode = data.data.RefferenceCode;
                       me.data.RefferenceDesc = data.data.RefferenceDesc1;
                   }
                   else {
                       me.data.RefferenceCode = "";
                       me.data.RefferenceDesc = "";
                       me.RefferenceCode();
                   }
               }).
               error(function (data, status, headers, config) {
                   alert('error');
               });
        }
    });

    $("[name='RefferenceCodeTo']").on('blur', function () {
        if (me.data.RefferenceCodeTo != "") {
            $http.post('om.api/MstRefference/koderef', me.data).
               success(function (data, status, headers, config) {
                   if (data.success) {
                       me.data.RefferenceCodeTo = data.data.RefferenceCode;
                       me.data.RefferenceDescTo = data.data.RefferenceDesc1;
                   }
                   else {
                       me.data.RefferenceCodeTo = "";
                       me.data.RefferenceDescTo = "";
                       me.RefferenceCodeTo();
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
                   // me.data.CompanyCode,
                    me.data.RefferenceType,
                    me.data.RefferenceCode,
                    me.data.RefferenceCodeTo,
                    me.data.Status
        ];
        Wx.showPdfReport({
            id: "OmRpMst001",
            pparam: prm.join(','),
            rparam: "semua",
            type: "devex"
        });
    }

    $("[name = 'isActive']").on('change', function () {
        me.data.isActive = $('#isActive').prop('checked');
        me.data.RefferenceCode = "";
        me.data.RefferenceDesc = "";
        me.data.RefferenceCodeTo = "";
        me.data.RefferenceDescTo = "";
        me.Apply();
    });

    me.initialize = function () {
        me.data = {};
        me.change = false;
        me.data.isActive = "0";
        me.data.RefferenceType = 'COLO';
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
        title: "Report Refference",
        xtype: "panels",
        toolbars: [
            { name: "btnPrintPreview", text: "Print", cls: "btn btn-primary", icon: "icon-print", click: "printPreview()", disable: "!isPrintEnable" },
            { name: "btnCancel", text: "Cancel", cls: "btn btn-warning", icon: "icon-remove", click: "cancelOrClose()"  },
        ],
        panels: [
            {
                name: "pnlA",
                items: [
                        { name: "CompanyCode", model: "data.CompanyCode", text: "Kode Perusahaan", cls: "span4 full", disable: "isPrintAvailable" },
                        { name: "BranchCode", model: "data.BranchCode", text: "Kode Cabang", cls: "span4 full", disable: "isPrintAvailable" },
                        { name: "RefferenceType", model: "data.RefferenceType", text: "Tipe Ref.", type: "select2", cls: "span4 full", datasource: "RefferenceType" },
                        { name: 'isActive', type: 'check', cls: 'span2', text: "Kode Ref.", float: 'left' },
                        {
                            text: "",
                            type: "controls",
                            items: [
                                { name: "RefferenceCode", cls: "span2", type: "popup", btnName: "btnRefferenceCode", click: "RefferenceCode()", disable: "data.isActive == false" },
                                { name: "RefferenceDesc", cls: "span4", readonly: true },
                            ]
                        },
                        {
                            text: "",
                            type: "controls",
                            items: [
                                { name: "RefferenceCodeTo", cls: "span2", type: "popup", btnName: "btnRefferenceCodeTo", click: "RefferenceCodeTo()", disable: "data.isActive == false" },
                                { name: "RefferenceDescTo", cls: "span4", readonly: true },
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