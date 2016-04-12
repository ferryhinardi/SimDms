var status = 'N';

"use strict";

function svInqMsIController($scope, $http, $injector) {
    var me = $scope;
    $injector.invoke(BaseController, this, { $scope: me });

    $http.post('sv.api/inqmsi/ListOfYear').
    success(function (data, status, headers, config) {
        me.comboYear = data;
    });

    $http.post('sv.api/inqmsi/ListOfMonth').
  success(function (data, status, headers, config) {
      me.comboMonth = data;
  });

    //$('#Area').on('change', function () {
    //    var area = $('#Area').select2('val');
    //    var areaname = $('#Area').select2('data').text;
    //    if (areaname == "-- SELECT ALL --") {
    //        area = "";
    //        $('#CompanyCode').select2('data', { id: "", text: "-- SELECT ALL --" });
    //        $('#BranchCode').select2('data', { id: "", text: "-- SELECT ALL --" });
    //    }

    //    console.log(area, areaname);
    //    $http.post('sv.api/inqmsi/OrganizationsV2?area=' + area).
    //        success(function (data, status, headers, config) {
    //            me.comboDealer = data;
    //        });
    //});

    //$('#CompanyCode').on('change', function () {
    //    var area = $('#Area').select2('val');
    //    var compcode = $('#CompanyCode').select2('val');
    //    var compname = $('#CompanyCode').select2('data').text;
    //    if (compname == "-- SELECT ALL --") {
    //        compcode = "";
    //        $('#BranchCode').select2('data', { id: "", text: "-- SELECT ALL --" });
    //    }
    //    else {
    //        console.log(compcode, compname);
    //        $http.post('sv.api/inqmsi/BranchsV2?area=' + area + '&comp=' + compcode).
    //            success(function (data, status, headers, config) {
    //                me.comboOutlet = data;
    //            });
    //    }
    //});

    me.loadData = function () {
        me.refreshGrid();
    }
    me.exportExcel = function () {
        me.exportXls();
    }

    me.refreshGrid = function () {
        console.log(me.DataSource);
        var prms = {
            //Area: $('#Area').select2('data').id,
            //CompanyCode: $('#CompanyCode').select2('data').id,
            //BranchCode: $('#BranchCode').select2('data').id,
            Year: $('#Year').select2('data').id,
            Month: $('#Month').select2('data').id,
            DataSource: me.DataSource
        };
        var lookup = Wx.kgrid({
            url: "sv.api/inqmsi/SvMsiV2",
            name: "InqPers",
            params: prms,
            scrollable: true,
            filterable: false,
            pageable: false,
            pageSize: 200,
            group: [
                   { field: "MsiGroup" },
            ],
            columns: [
                { field: "SeqNo", title: "No", width: 60 },
                { field: "MsiDesc", title: "Description", width: 650 },
                { field: "Unit", title: "Unit", width: 100 },
                { field: "Average", title: "Average", width: 150, type: 'decimal' },
                { field: "Total", title: "Total", width: 150, type: 'decimal' },
                { field: "Month01", title: "Jan", width: 150, type: 'decimal' },
                { field: "Month02", title: "Feb", width: 150, type: 'decimal' },
                { field: "Month03", title: "Mar", width: 150, type: 'decimal' },
                { field: "Month04", title: "Apr", width: 150, type: 'decimal' },
                { field: "Month05", title: "May", width: 150, type: 'decimal' },
                { field: "Month06", title: "Jun", width: 150, type: 'decimal' },
                { field: "Month07", title: "Jul", width: 150, type: 'decimal' },
                { field: "Month08", title: "Aug", width: 150, type: 'decimal' },
                { field: "Month09", title: "Sep", width: 150, type: 'decimal' },
                { field: "Month10", title: "Oct", width: 150, type: 'decimal' },
                { field: "Month11", title: "Nov", width: 150, type: 'decimal' },
                { field: "Month12", title: "Dec", width: 150, type: 'decimal' },

                // { field: "SeqNo", title: "No.", width: 60 },
                // { field: "SeqNo", title: "No", width: 60 },
                //{
                //    field: "MsiGroup", title: "Units In Stock",
                //    groupHeaderTemplate: "Group : #= value #",
                //},
                //{ field: "MsiDesc", title: 'Keterangan', width: 650 },
                //{ field: "Unit", title: "Unit", width: 100 },
                //{ field: "Average", title: "Average", width: 100 },
                //{ field: "Total", title: "Total", width: 100 },
                //{ field: "Month01", title: 'Jan', width: 100 },
                //{ field: "Month02", title: 'Feb', width: 100 },
                //{ field: "Month03", title: 'Mar', width: 100 },
                //{ field: "Month04", title: 'Apr', width: 100 },
                //{ field: "Month05", title: 'May', width: 100 },
                //{ field: "Month06", title: 'Jun', width: 100 },
                //{ field: "Month07", title: 'Jul', width: 100 },
                //{ field: "Month08", title: 'Aug', width: 100 },
                //{ field: "Month09", title: 'Sep', width: 100 },
                //{ field: "Month10", title: 'Oct', width: 100 },
                //{ field: "Month11", title: 'Nov', width: 100 },
                //{ field: "Month12", title: 'Dec', width: 100 }
            ],
        });
    }

    me.exportXls = function () {
        var spID = "";
        var dataSource = me.DataSource;
        if (dataSource == "Invoice") {
            spID = "uspfn_DlrInqMsiV2";
        }
        else {
            spID = "uspfn_DlrInqMsiV2_SPK";
        }

        var url = "sv.api/inqmsi/exportExcel?";
        var area = $('#Area').select2('data').id;
        var dealer = $('#CompanyCode').select2('data').id;
        var outlet = $('#BranchCode').select2('data').id;
        var year = $('#Year').select2('data').id;
        var textArea = $('#Area').select2('data').text;
        var textDealer = $('#CompanyCode').select2('data').text;
        var textOutlet = $('#BranchCode').select2('data').text;
        var month = $('#Month').select2('data').id;

        var
        params = "&Area=" ;
        params += "&Dealer=";
        params += "&Outlet=";
        params += "&SpID=" + spID;
        params += "&Year=" + year;
        params += "&TextArea=";
        params += "&TextDealer=";
        params += "&TextOutlet=";
        params += "&DataSource=" + dataSource;
        params += "&Month=" + month;

        url = url + params;
        window.location = url;

        console.log(url);
    }

    me.default = function () {
        $http.post('sv.api/inqmsi/default').
        success(function (data, status, headers, config) {
            me.data.Year = data.Year;
            me.data.Month = data.Month;
        });

        $http.post("sv.api/inqmsi/isNational").
            success(function (result) {
                isNational = result.isNational;
                area = result.area;
                //isNational = 1;
                if (isNational == 0) {

                    $("#Area").attr("disabled", "disabled");
                    $("#CompanyCode").attr("disabled", "disabled");
                    $("#BranchCode").attr("disabled", "disabled");

                    $('#Area').select2('data', { id: result.area, text: result.area });
                    $('#CompanyCode').select2('data', { id: result.dealerCd, text: result.dealerNm });
                    $('#BranchCode').select2('data', { id: result.outletCd, text: result.outletNm });

                    console.log(result.area, $('#Area').select2('data').text, $('#Area').select2('data').id);
                }
                else {
                    $("#Area").removeAttr("disabled");
                    $("#CompanyCode").removeAttr("disabled");
                    $("#BranchCode").removeAttr("disabled");

                    $http.post('sv.api/inqmsi/Areas').
                        success(function (data, status, headers, config) {
                            me.comboArea = data;
                        });

                }

                $("#pnlFilter select").on("change", me.clearGrid);

            });
    }

    me.clearGrid = function () {
        $("#InqPers").empty();
    }

    me.initialize = function () {
        me.default();
        me.DataSource = "Invoice";
    }

    me.start();

    me.HideForm = function () {
        $(".body > .panel").fadeOut();
    }
}

$(document).ready(function () {
    var options = {
        title: "Inquiry Suzuki MSI",
        xtype: "panels",

        panels: [
            {
                name: "pnlFilter",
                items: [
                    //{
                    //    text: "Area",
                    //    type: "controls",
                    //    items: [
                    //        { name: "Area", cls: "span6", type: "select2", opt_text: "-- SELECT ALL --", datasource: "comboArea" },
                    //    ]
                    //},
                    //{
                    //    text: "Dealer Name",
                    //    type: "controls",
                    //    items: [
                    //        { name: "CompanyCode", cls: "span6", type: "select2", opt_text: "-- SELECT ALL --", datasource: "comboDealer" },
                    //    ]
                    //},
                    //{
                    //    text: "Outlet Name",
                    //    type: "controls",
                    //    items: [
                    //        { name: "BranchCode", cls: "span6", type: "select2", opt_text: "-- SELECT ALL --", datasource: "comboOutlet" },
                    //    ]
                    //},
                    {
                        text: "Tahun",
                        type: "controls",
                        items: [
                            { name: "Year", text: "Year", cls: "span2", type: "select2", datasource: 'comboYear', opt_text: "-- SELECT ONE --" },
                             { name: "Month", cls: "span4", text: "Month", type: "select2", datasource: "comboMonth" }
                        ]
                    },
                    {
                        type: "optionbuttons",
                        text: "Data Source",
                        model: "DataSource",
                        cls: "span4",
                        items: [
                            { name: "Invoice", text: "Invoice" },
                            { name: "SPK", text: "SPK" }
                        ]
                    },
                ],
            },
            {
                name: "InqPers",
                xtype: "k-grid",
            },
        ],
        toolbars: [
           { name: "btnRefresh", text: "Load Data", icon: "fa fa-search", click: "loadData()" },
           { name: "btnExportXls", text: "Export (xls)", icon: "fa fa-file-excel-o", click: "exportExcel()" },
        ],
    }



    Wx = new SimDms.Widget(options);
    Wx.default = {};
    Wx.render(init);

    function init() {
        SimDms.Angular("svInqMsIController");
    }
});
