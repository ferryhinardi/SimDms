var status = 'N';

"use strict";

function srvworkshopbodypaintController($scope, $http, $injector) {
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

    me.loadData = function () {
        me.refreshGrid();
    }
    me.exportExcel = function () {
        me.exportXls();
    }

    me.refreshGrid = function () {
        console.log('test');
        var prms = {
            Year: $('#Year').select2('data').id,
            Month: $('#Month').select2('data').id
        };
        var lookup = Wx.kgrid({
            url: "sv.api/WorkshopBodyPaint/WorkshopBodyPaintGrid",
            name: "WorkshopBodyPaintSuzuki",
            params: prms,
            scrollable: true,
            filterable: false,
            pageable: false,
            pageSize: 200,
            columns: [
                { field: "SeqNo", title: "No", width: 60 },
                { field: "BdpDescription", title: "Description", width: 450 },
                { field: "Suzuki", title: "Suzuki", width: 150, type: 'decimal' },
                { field: "NonSuzuki", title: "Non Suzuki", width: 150, type: 'decimal' },
                { field: "Total", title: "Total", width: 150, type: 'decimal' }
            ],
        });
    }

    me.exportXls = function () {
        
        var url = "sv.api/WorkshopBodyPaint/ExportExcelBodyRepair?";
        var year = $('#Year').select2('data').id;
        var month = $('#Month').select2('data').id;
        var spID = "uspfn_srvworkshopbodypaintexcel";

        var
        params = "SpID=" + spID;
        params += "&periodYear=" + year;
        params += "&periodMonth=" + month;

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
        title: "Workshop Body & Paint Suzuki",
        xtype: "panels",

        panels: [
            {
                name: "pnlFilter",
                items: [
                    {
                        text: "Tahun",
                        type: "controls",
                        items: [
                            { name: "Year", text: "Year", cls: "span2", type: "select2", datasource: 'comboYear', opt_text: "-- SELECT ONE --" },
                             { name: "Month", cls: "span4", text: "Month", type: "select2", datasource: "comboMonth" }
                        ]
                    },
                ],
            },
            {
                name: "WorkshopBodyPaintSuzuki",
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
        SimDms.Angular("srvworkshopbodypaintController");
    }
});
