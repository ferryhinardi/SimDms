var totalSrvAmt = 0;
var status = 'N';
var svType = '2';


"use strict";

function ProsesStockTaking($scope, $http, $injector) {

    var me = $scope;
    $injector.invoke(BaseController, this, { $scope: me });


    me.WHBrowse = function (param) {
        var lookup = Wx.blookup({
            name: "btnRefferenceCode",
            title: "Warehouse Lookup",
            manager: spSalesManager,
            query: new breeze.EntityQuery.from("RefferenceStockTaking").withParameters({ "RefferenceType": "WARE" }),
            defaultSort: "RefferenceCode asc",
            columns: [
                { field: "RefferenceCode", title: "Kode Gudang" },
                { field: "RefferenceDesc1", title: "Nama Gudang" },
            ]
        });
        lookup.dblClick(function (data) {
            if (data != null) {
                if (param == "from") {
                    me.data.WHCode = data.RefferenceCode;
                    me.data.WHDesc = data.RefferenceDesc1;
                } else {
                    me.to.WHCodeTo = data.RefferenceCode;
                    me.to.WHDescTo = data.RefferenceDesc1;
                }
                me.Apply();

            }
        });
    }

    me.ModelBrowse = function (param) {

        var lookup = Wx.blookup({
            name: "btnSalesModelCode",
            title: "Model Lookup",
            manager: spSalesManager,
            query: "SalesModelCodeLookup",
            defaultSort: "SalesModelCode asc",
            columns: [
            { field: "SalesModelCode", title: "Model" },
            { field: "SalesModelDesc", title: "Keterangan" },
            ]
        });
        lookup.dblClick(function (data) {
            if (data != null) {

                if (param == "from") {
                    me.data.SalesModelCode = data.SalesModelCode;
                    me.data.SalesModelDesc = data.SalesModelDesc;
                } else {
                    me.to.SalesModelCodeTo = data.SalesModelCode;
                    me.to.SalesModelDescTo = data.SalesModelDesc;
                }
                me.Apply();

            }
        });
    }

    me.YearBrowse = function (param) {

        var lookup = Wx.blookup({
            name: "btnSalesModelYear",
            title: "Year Lookup",
            manager: spSalesManager,
            query: "YearStockTaking",
            defaultSort: "SalesModelYear desc",
            columns: [
                { field: "SalesModelYear", title: "Tahun" },
            ]
        });
        lookup.dblClick(function (data) {
            if (data != null) {

                if (param == "from") {
                    me.data.SalesModelYear = data.SalesModelYear;
                } else {
                    me.to.SalesModelYearTo = data.SalesModelYear;
                }
                me.Apply();

            }
        });
    }

    me.ColorBrowse = function (param) {

        var lookup = Wx.blookup({
            name: "btnRefferenceCode",
            title: "Warehouse Lookup",
            manager: spSalesManager,
            query: new breeze.EntityQuery.from("RefferenceStockTaking").withParameters({ "RefferenceType": "COLO" }),
            defaultSort: "RefferenceCode asc",
            columns: [
                { field: "RefferenceCode", title: "Kode Warna" },
                { field: "RefferenceDesc1", title: "Nama Warna" },
            ]
        });
        lookup.dblClick(function (data) {
            if (data != null) {
                if (param == "from") {
                    me.data.ColorCode = data.RefferenceCode;
                    me.data.ColorDesc = data.RefferenceDesc1;
                } else {
                    me.to.ColorCodeTo = data.RefferenceCode;
                    me.to.ColorDescTo = data.RefferenceDesc1;
                }
                me.Apply();

            }
        });
    }

    me.ProsesStock = function () {
        me.savemodel = angular.copy(me.data);
        angular.extend(me.savemodel, me.to);
        MsgConfirm("Are you sure?", function (result) {
            if (result) {
                $http.post('om.api/SalesStockOpname/ProsesStockTaking', me.savemodel).
                success(function (dl, status, headers, config) {
                    if (dl.success) {
                        MsgBox(dl.message, MSG_SUCCESS);
                    } else {
                        MsgBox(dl.message, MSG_ERROR)
                        console.log(dl.error_log);
                    }
                }).
                error(function (e, status, headers, config) {
                    MsgBox("Connecting server error", MSG_ERROR);
                });
            }
        });
    }

    me.$watch("data.isWH", function (a, b) {
        if (a != b) {
            if (a == false) {
                me.data.WHCode = "";
                me.data.WHDesc = "";
                me.to.WHCodeTo = "";
                me.to.WHDescTo = "";
            }
        }
    });

    me.$watch("data.isModel", function (a, b) {
        if (a != b) {
            if (a == false) {
                me.data.SalesModelCode = "";
                me.data.SalesModelDesc = "";
                me.to.SalesModelCodeTo = "";
                me.to.SalesModelDescTo = "";
            }
        }
    });

    me.$watch("data.isColor", function (a, b) {
        if (a != b) {
            if (a == false) {
                me.data.ColorCode = "";
                me.data.ColorDesc = "";
                me.to.ColorCodeTo = "";
                me.to.ColorDescTo = "";
            }
        }
    });

    me.$watch("data.isYear", function (a, b) {
        if (a != b) {
            if (a == false) {
                me.data.SalesModelYear = "";
                me.to.SalesModelYearTo = "";
            }
        }
    });



    me.initialize = function () {
        me.to = {};
        me.data = {};
        me.data.STDate = me.now();
        me.data.isWH = false;
        me.data.isModel = false;
        me.data.isColor = false;
        me.data.isYear = false;
    }



    me.start();

}



$(document).ready(function () {
    var options = {
        title: "Proses Stock Taking",
        xtype: "panels",
        // toolbars: WxButtons,
        panels: [
            {
                name: "pnlA",
                title: "",
                items: [
                    {
                        type: "controls",
                        text: "Tgl. Stock Taking",
                        cls: "span8",
                        items: [
                            {
                                type: "ng-check",
                                cls: "span1",
                                style: "visibility: hidden;"
                            },
                            {
                                name: "STDate",
                                type: "ng-datepicker",
                                cls: "span2",
                            }
                        ]
                    },
                    {
                        text: "Gudang",
                        type: "controls",
                        cls: "span8",
                        items: [
                            {
                                name: "isWH",
                                type: "ng-check",
                                cls: "span1",
                            },
                            {
                                name: "WHCode",
                                click: "WHBrowse('from')",
                                cls: "span2",
                                type: "popup",
                                text: "Warehouse From",
                                btnName: "btnRefferenceCode",
                                readonly: true,
                                disable: "data.isWH == false"
                            },
                            {
                                name: "WHDesc",
                                cls: "span4",
                                text: "Desc Warehouse From",
                                readonly: true,
                                disable: "data.isWH == false"
                            }
                        ]
                    },
                    {
                        name: "ctlWHName",
                        type: "controls",
                        cls: "span8",
                        items: [
                            {
                                type: "ng-check",
                                cls: "span1",
                                style: "visibility: hidden;"
                            },
                            {
                                name: "WHCodeTo",
                                click: "WHBrowse('to')",
                                cls: "span2",
                                model: "to.WHCodeTo",
                                text: "Warehouse To",
                                type: "popup",
                                btnName: "btnRefferenceCode",
                                readonly: true,
                                disable: "data.isWH == false"
                            },
                            {
                                name: "WHDescTo",
                                cls: "span4",
                                model: "to.WHDescTo",
                                text: "Desc Warehouse To",
                                readonly: true,
                                disable: "data.isWH == false"
                            }
                        ]
                    },
                    {
                        text: "Model",
                        type: "controls",
                        cls: "span8",
                        items: [
                            {
                                name: "isModel",
                                type: "ng-check",
                                cls: "span1"
                            },
                            {
                                name: "SalesModelCode",
                                click: "ModelBrowse('from')",
                                cls: "span2",
                                type: "popup",
                                text: "Model From",
                                btnName: "btnSalesModelCode",
                                readonly: true,
                                disable: "data.isModel == false"
                            },
                            {
                                name: "SalesModelDesc",
                                cls: "span4",
                                text: "Desc Model From",
                                readonly: true,
                                disable: "data.isModel == false"
                            }
                        ]
                    },
                    {
                        type: "controls",
                        cls: "span8",
                        items: [
                            {
                                type: "ng-check",
                                cls: "span1",
                                style: "visibility: hidden;"
                            },
                            {
                                name: "SalesModelCodeTo",
                                click: "ModelBrowse('to')",
                                cls: "span2",
                                model: "to.SalesModelCodeTo",
                                text: "Model To",
                                type: "popup",
                                btnName: "btnSalesModelCode",
                                readonly: true,
                                disable: "data.isModel == false"
                            },
                            {
                                name: "SalesModelDescTo",
                                cls: "span4",
                                model: "to.SalesModelDescTo",
                                text: "Desc Model To",
                                readonly: true,
                                disable: "data.isModel == false"
                            }
                        ]
                    },
                    {
                        text: "Tahun",
                        type: "controls",
                        cls: "span8",
                        items: [
                            {
                                name: "isYear",
                                type: "ng-check",
                                cls: "span1",
                            },
                            {
                                name: "SalesModelYear",
                                click: "YearBrowse('from')",
                                cls: "span3",
                                type: "popup",
                                text: "Year From",
                                btnName: "btnSalesModelYear",
                                readonly: true,
                                disable: "data.isYear == false"
                            },
                            {
                                name: "SalesModelYearTo",
                                click: "YearBrowse('to')",
                                cls: "span3",
                                model: "to.SalesModelYearTo",
                                type: "popup",
                                text: "Year To",
                                btnName: "btnSalesModelYear",
                                readonly: true,
                                disable: "data.isYear == false"
                            }
                        ]
                    },
                    {
                        text: "Warna",
                        type: "controls",
                        cls: "span8",
                        items: [
                            {
                                name: "isColor",
                                type: "ng-check",
                                cls: "span1",
                            },
                            {
                                name: "ColorCode",
                                click: "ColorBrowse('from')",
                                cls: "span2",
                                type: "popup",
                                text: "Color From",
                                btnName: "btnRefferenceCode1",
                                readonly: true,
                                disable: "data.isColor == false"
                            },
                            {
                                name: "ColorDesc",
                                cls: "span4",
                                text: "Desc Color From",
                                readonly: true,
                                disable: "data.isColor == false"
                            }
                        ]
                    },
                    {
                        type: "controls",
                        cls: "span8",
                        items: [
                            {
                                type: "ng-check",
                                cls: "span1",
                                style: "visibility: hidden;"
                            },
                            {
                                name: "ColorCodeTo",
                                click: "ColorBrowse('to')",
                                cls: "span2",
                                text: "Color To",
                                model: "to.ColorCodeTo",
                                type: "popup",
                                btnName: "btnRefferenceCode1",
                                readonly: true,
                                disable: "data.isColor == false"
                            },
                            {
                                name: "ColorDescTo",
                                model: "to.ColorDescTo",
                                cls: "span4",
                                text: "Desc Color To",
                                readonly: true,
                                disable: "data.isColor == false"
                            }
                        ]
                    }, { type: "div" },
                    {
                        type: "buttons",
                        items: [
                                { name: "btnProsesStockTaking", text: "Proses Stock Taking", cls: "btn btn-info", click: "ProsesStock()" },
                        ]
                    },
                ]
            },
        ]
    };


    Wx = new SimDms.Widget(options);
    Wx.default = {};
    Wx.render(init);

    function init() {
        SimDms.Angular("ProsesStockTaking");
    }





});