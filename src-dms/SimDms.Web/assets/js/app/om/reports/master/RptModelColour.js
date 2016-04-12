"use strict"; //Reportid OmRpMst003
function spRptMstSales($scope, $http, $injector) {

    var me = $scope;

    $injector.invoke(BaseController, this, { $scope: me });

    me.Status = [
        { "value": '2', "text": 'All' },
        { "value": '0', "text": 'Tidak Aktif' },
        { "value": '1', "text": 'Aktif' }
    ];

    me.SalesModelCode = function () {
        var lookup = Wx.blookup({
            name: "SalesModelCodeLookup",
            title: "Sales Model Code",
            manager: spSalesManager,
            query: "SalesModelCodeLookup",
            defaultSort: "SalesModelCode asc",
            columns: [
                { field: "SalesModelCode", title: "Sales Model Code" },
                { field: "SalesModelDesc", title: "Sales Model Desc" }
            ]
        });
        lookup.dblClick(function (data) {
            if (data != null) {
                me.data.SalesModelCode = data.SalesModelCode;
                me.data.SalesModelDesc = data.SalesModelDesc;
                me.Apply();
            }
        });
    }

    me.SalesModelCodeTo = function () {
        var lookup = Wx.blookup({
            name: "SalesModelCodeLookup",
            title: "Sales Model Code",
            manager: spSalesManager,
            query: "SalesModelCodeLookup",
            defaultSort: "SalesModelCode asc",
            columns: [
                { field: "SalesModelCode", title: "Sales Model Code" },
                { field: "SalesModelDesc", title: "Sales Model Desc" }
            ]
        });
        lookup.dblClick(function (data) {
            if (data != null) {
                me.data.SalesModelCodeTo = data.SalesModelCode;
                me.data.SalesModelDescTo = data.SalesModelDesc;
                me.Apply();
            }
        });
    }

    me.ColourCode = function () {
        var lookup = Wx.blookup({
            name: "ColourCodeLookup",
            title: "Colour",
            manager: spSalesManager,
            query: "ColourCodeLookup",
            defaultSort: "RefferenceCode asc",
            columns: [
                { field: "RefferenceCode", title: "Colour Code" },
                { field: "RefferenceDesc1", title: "Description" }
            ]
        });
        lookup.dblClick(function (data) {
            if (data != null) {
                me.data.ColourCode = data.RefferenceCode;
                me.data.ColourDesc = data.RefferenceDesc1;
                me.Apply();
            }
        });
    }

    me.ColourCodeTo = function () {
        var lookup = Wx.blookup({
            name: "ColourCodeLookup",
            title: "Colour",
            manager: spSalesManager,
            query: "ColourCodeLookup",
            defaultSort: "RefferenceCode asc",
            columns: [
                { field: "RefferenceCode", title: "Colour Code" },
                { field: "RefferenceDesc1", title: "Description" }
            ]
        });
        lookup.dblClick(function (data) {
            if (data != null) {
                me.data.ColourCodeTo = data.RefferenceCode;
                me.data.ColourDescTo = data.RefferenceDesc1;
                me.Apply();
            }
        });
    }

    $("[name='SalesModelCode']").on('blur', function () {
        if (me.data.SalesModelCode != "") {
            $http.post('om.api/MstModelColor/ModelCode', me.data).
               success(function (data, status, headers, config) {
                   if (data.success) {
                       me.data.SalesModelCode = data.data.SalesModelCode;
                       me.data.SalesModelDesc = data.data.SalesModelDesc;
                       me.Apply();
                   }
                   else {
                       me.data.SalesModelCode = "";
                       me.data.SalesModelDesc = "";
                       me.SalesModelCode();
                   }
               }).
               error(function (data, status, headers, config) {
                   alert('error');
               });
        }
    });

    $("[name='SalesModelCodeTo']").on('blur', function () {
        if (me.data.SalesModelCodeTo != "") {
            $http.post('om.api/MstModelColor/ModelCode', me.data).
               success(function (data, status, headers, config) {
                   if (data.success) {
                       me.data.SalesModelCodeTo = data.data.SalesModelCode;
                       me.data.SalesModelDescTo = data.data.SalesModelDesc;
                       me.Apply();
                   }
                   else {
                       me.data.SalesModelCodeTo = "";
                       me.data.SalesModelDescTo = "";
                       me.SalesModelCodeTo();
                   }
               }).
               error(function (data, status, headers, config) {
                   alert('error');
               });
        }
    });

    $("[name='ColourCode']").on('blur', function () {
        if (me.data.ColourCode != "") {
            $http.post('om.api/MstModelColor/ColourCode', me.data).
               success(function (data, status, headers, config) {
                   if (data.success) {
                       me.data.ColourCode = data.data.RefferenceCode;
                       me.data.ColourDesc = data.data.RefferenceDesc1;
                       me.Apply();
                   }
                   else {
                       me.data.ColourCode = "";
                       me.data.ColourDesc = "";
                       me.ColourCode();
                   }
               }).
               error(function (data, status, headers, config) {
                   alert('error');
               });
        }
    });

    $("[name='ColourCodeTo']").on('blur', function () {
        if (me.data.ColourCodeTo != "") {
            $http.post('om.api/MstModelColor/ColourCode', me.data).
               success(function (data, status, headers, config) {
                   if (data.success) {
                       me.data.ColourCodeTo = data.data.RefferenceCode;
                       me.data.ColourDescTo = data.data.RefferenceDesc1;
                       me.Apply();
                   }
                   else {
                       me.data.ColourCodeTo = "";
                       me.data.ColourDescTo = "";
                       me.ColourCodeTo();
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

        var refftype = "COLO";
        var prm = [

                    me.data.SalesModelCode,
                    me.data.SalesModelCodeTo,
                    me.data.ColourCode,
                    me.data.ColourCodeTo,
                    refftype,
                    me.data.Status
        ];
        Wx.showPdfReport({
            id: "OmRpMst003",
            pparam: prm.join(','),
            rparam: "semua",
            type: "devex"
        });
    }

    $("[name = 'isActive']").on('change', function () {
        me.data.isActive = $('#isActive').prop('checked');
        me.data.SalesModelCode = "";
        me.data.SalesModelDesc = "";
        me.data.SalesModelCodeTo = "";
        me.data.SalesModelDescTo = "";
        me.Apply();
    });

    $("[name = 'isActiveColour']").on('change', function () {
        me.data.isActiveColour = $('#isActiveColour').prop('checked');
        me.data.ColourCode = "";
        me.data.ColourDesc = "";
        me.data.ColourCodeTo = "";
        me.data.ColourDescTo = "";
        me.Apply();
    });

    me.initialize = function () {
        me.data = {};
        $('#isActive').prop('checked', false);
        me.data.isActive = false;
        $('#isActiveColour').prop('checked', false);
        me.data.isActiveColour = false;
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
        title: "Report Model Colour",
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
                        { name: 'isActive', type: 'check', cls: 'span2', text: "Sales Model Code", float: 'left' },
                        {
                            text: "",
                            type: "controls",
                            items: [
                                { name: "SalesModelCode", cls: "span2", type: "popup", btnName: "btnSalesModelCode", click: "SalesModelCode()", disable: "data.isActive == false" },
                                { name: "SalesModelDesc", cls: "span4", readonly: true },
                            ]
                        },
                        {
                            text: "",
                            type: "controls",
                            items: [
                                { name: "SalesModelCodeTo", cls: "span2", type: "popup", btnName: "btnSalesModelCodeTo", click: "SalesModelCodeTo()", disable: "data.isActive == false" },
                                { name: "SalesModelDescTo", cls: "span4", readonly: true },
                            ]
                        },
                        { name: 'isActiveColour', type: 'check', cls: 'span2', text: "Warna", float: 'left' },
                        {
                            text: "",
                            type: "controls",
                            items: [
                                { name: "ColourCode", cls: "span2", type: "popup", btnName: "btnColourCode", click: "ColourCode()", disable: "data.isActiveColour == false" },
                                { name: "ColourDesc", cls: "span4", readonly: true },
                            ]
                        },
                        {
                            text: "",
                            type: "controls",
                            items: [
                                { name: "ColourCodeTo", cls: "span2", type: "popup", btnName: "btnColourCodeTo", click: "ColourCodeTo()", disable: "data.isActiveColour == false" },
                                { name: "ColourDescTo", cls: "span4", readonly: true },
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