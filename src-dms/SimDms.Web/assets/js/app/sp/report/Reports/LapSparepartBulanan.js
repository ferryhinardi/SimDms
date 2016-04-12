var status = 'N';

"use strict";

function spLapSparepartBulanan($scope, $http, $injector) {
    var me = $scope;
    $injector.invoke(BaseController, this, { $scope: me });

    $http.post('sp.api/ReportSparepartMonthly/AreaMapping').
    success(function (data, status, headers, config) {
        me.comboArea = data;
    });
    $http.post('sp.api/ReportSparepartMonthly/DealerMapping').
    success(function (data, status, headers, config) {
        me.comboDealer = data;
    });

    $http.post('sp.api/ReportSparepartMonthly/BranchMapping').
    success(function (data, status, headers, config) {
        me.comboOutlet = data;
    });

    $http.post('sp.api/combo/Years').
    success(function (data, status, headers, config) {
        me.comboYear = data;
    });

    $http.post('sp.api/combo/Months').
  success(function (data, status, headers, config) {
      me.comboMonth = data;
  });

    $http.post('sp.api/combo/TypePart').
  success(function (data, status, headers, config) {
      me.comboTypeOfGoods = data;
  });

    me.loadData = function () {
        me.refreshGrid();
    }
    me.exportExcel = function () {
        me.exportXls();
    }

    me.refreshGrid = function () {
        console.log(me.DataSource);
        var prms = {
            Area: $('#Area').select2('data').id,
            CompanyCode: $('#CompanyCode').select2('data').id,
            BranchCode: $('#BranchCode').select2('data').id,
            Year: $('#Year').select2('data').id,
            Month: $('#Month').select2('data').id,
            TypeOfGoods: $('#typeofgoods').select2('data').id,
        };
        console.log(prms);
        var lookup = Wx.kgrid({
            url: "sp.api/ReportSparepartMonthly/ReportSparepartMonthlyGrid",
            name: "LapSpAnalisisBulanan",
            params: prms,
            scrollable: true,
            filterable: false,
            pageable: false,
            pageSize: 15,
            columns: [
                {
                    field: "Bulan",
                    title: "Bulan",
                    width: 100
                }, {
                    field: "JumlahJaringan",
                    title: "Jml. Jaringan",
                    template: '<div style="text-align:right;">#= kendo.toString(JumlahJaringan, "n0") #</div>',
                    width: 150
                },
                {
                    title: "Penjualan Kotor",
                    columns: [
                         {
                             field: "Workshop_PK",
                             title: "Workshop",
                             template: '<div style="text-align:right;">#= kendo.toString(Workshop_PK, "n0") #</div>',
                             width: 150,
                         }, {
                             field: "Counter_PK",
                             title: "Counter",
                             template: '<div style="text-align:right;">#= kendo.toString(Counter_PK, "n0") #</div>',
                             width: 150
                         }, {
                             field: "Partshop_PK",
                             title: "Partshop",
                             template: '<div style="text-align:right;">#= kendo.toString(Partshop_PK, "n0") #</div>',
                             width: 150
                         }, {
                             field: "SubDealer_PK",
                             title: "Sub Dealer",
                             template: '<div style="text-align:right;">#= kendo.toString(SubDealer_PK, "n0") #</div>',
                             width: 150
                         },
                    ]
                },
                 {
                     title: "Penjualan Bersih",
                     columns: [
                          {
                              field: "Workshop_PB",
                              title: "Workshop",
                              template: '<div style="text-align:right;">#= kendo.toString(Workshop_PB, "n0") #</div>',
                              width: 150
                          }, {
                              field: "Counter_PB",
                              title: "Counter",
                              template: '<div style="text-align:right;">#= kendo.toString(Counter_PB, "n0") #</div>',
                              width: 150
                          }, {
                              field: "Partshop_PB",
                              title: "Partshop",
                              template: '<div style="text-align:right;">#= kendo.toString(Partshop_PB, "n0") #</div>',
                              width: 150
                          }, {
                              field: "SubDealer_PB",
                              title: "Sub Dealer",
                              template: '<div style="text-align:right;">#= kendo.toString(SubDealer_PB, "n0") #</div>',
                              width: 150
                          },
                     ]
                 },
                 {
                     title: "HPP",
                     columns: [
                          {
                              field: "Workshop_HPP",
                              title: "Workshop",
                              template: '<div style="text-align:right;">#= kendo.toString(Workshop_HPP, "n0") #</div>',
                              width: 150
                          }, {
                              field: "Counter_HPP",
                              title: "Counter",
                              template: '<div style="text-align:right;">#= kendo.toString(Counter_HPP, "n0") #</div>',
                              width: 150
                          }, {
                              field: "Partshop_HPP",
                              title: "Partshop",
                              template: '<div style="text-align:right;">#= kendo.toString(Partshop_HPP, "n0") #</div>',
                              width: 100
                          }, {
                              field: "SubDealer_HPP",
                              title: "Sub Dealer",
                              template: '<div style="text-align:right;">#= kendo.toString(SubDealer_HPP, "n0") #</div>',
                              width: 150
                          },
                          {
                              field: "Total_HPP",
                              title: "Total HPP",
                              template: '<div style="text-align:right;">#= kendo.toString(Total_HPP, "n0") #</div>',
                              width: 150
                          },
                     ]
                 },
                 {
                     title: "Margin",
                     columns: [
                          {
                              field: "Workshop_Margin",
                              title: "Workshop",
                              template: '<div style="text-align:right;">#= kendo.toString(Workshop_Margin, "n0") #</div>',
                              width: 150
                          }, {
                              field: "Counter_Margin",
                              title: "Counter",
                              template: '<div style="text-align:right;">#= kendo.toString(Counter_Margin, "n0") #</div>',
                              width: 150
                          }, {
                              field: "Partshop_Margin",
                              title: "Partshop",
                              template: '<div style="text-align:right;">#= kendo.toString(Partshop_Margin, "n0") #</div>',
                              width: 100
                          }, {
                              field: "SubDealer_Margin",
                              title: "Sub Dealer",
                              template: '<div style="text-align:right;">#= kendo.toString(SubDealer_Margin, "n2") #</div>',
                              width: 150
                          },
                     ]
                 },
                 {
                     field: "Penerimaan_Pembelian",
                     title: "Penerimaan Pembelian",
                     template: '<div style="text-align:right;">#= kendo.toString(Penerimaan_Pembelian, "n0") #</div>',
                     width: 200
                 },
                 {
                     field: "Nilai_Stock",
                     title: "Nilai Stock",
                     template: '<div style="text-align:right;">#= kendo.toString(Nilai_Stock, "n0") #</div>',
                     width: 175
                 },
                 {
                     field: "ITO",
                     title: "ITO",
                     template: '<div style="text-align:right;">#= kendo.toString(ITO, "n2") #</div>',
                     width: 150
                 },
                 {
                     title: "Demand",
                     columns: [
                          {
                              field: "Line_Demand",
                              title: "Line",
                              template: '<div style="text-align:right;">#= kendo.toString(Line_Demand, "n0") #</div>',
                              width: 150
                          }, {
                              field: "Quantity_Demand",
                              title: "Quantity",
                              template: '<div style="text-align:right;">#= kendo.toString(Quantity_Demand, "n1") #</div>',
                              width: 150
                          }, {
                              field: "Nilai_Demand",
                              title: "Nilai",
                              template: '<div style="text-align:right;">#= kendo.toString(Nilai_Demand, "n0") #</div>',
                              width: 100
                          },
                     ]
                 },
                  {
                      title: "Supply",
                      columns: [
                           {
                               field: "Line_Supply",
                               title: "Line",
                               template: '<div style="text-align:right;">#= kendo.toString(Line_Supply, "n0") #</div>',
                               width: 150
                           }, {
                               field: "Quantity_Supply",
                               title: "Quantity",
                               template: '<div style="text-align:right;">#= kendo.toString(Quantity_Supply, "n1") #</div>',
                               width: 150
                           }, {
                               field: "Nilai_Supply",
                               title: "Nilai",
                               template: '<div style="text-align:right;">#= kendo.toString(Nilai_Supply, "n0") #</div>',
                               width: 100
                           },
                      ]
                  },
                   {
                       title: "Service Ratio",
                       columns: [
                            {
                                field: "Line_Service_Ratio",
                                title: "Line",
                                template: '<div style="text-align:right;">#= kendo.toString(Line_Service_Ratio, "n1") #</div>',
                                width: 150
                            }, {
                                field: "Quantity_Service_Ratio",
                                title: "Quantity",
                                template: '<div style="text-align:right;">#= kendo.toString(Quantity_Service_Ratio, "n1") #</div>',
                                width: 150
                            }, {
                                field: "Nilai_Service_Ratio",
                                title: "Nilai",
                                template: '<div style="text-align:right;">#= kendo.toString(Nilai_Service_Ratio, "n1") #</div>',
                                width: 100
                            },
                       ]
                   },
                   {
                       title: "Data Stock",
                       columns: [
                            {                               
                                title: "Moving Code 0",
                                columns: [
                                {
                                    field: "Ammount_MC0",
                                    title: "Amount",
                                    template: '<div style="text-align:right;">#= kendo.toString(Ammount_MC0, "n0") #</div>',
                                    width: 150
                                }, {
                                    field: "Qty_MC0",
                                    title: "Quantity",
                                    template: '<div style="text-align:right;">#= kendo.toString(Qty_MC0, "n1") #</div>',
                                    width: 150
                                },
                                ]
                            },
                            {
                                title: "Moving Code 1",
                                columns: [
                                {
                                    field: "Ammount_MC1",
                                    title: "Amount",
                                    template: '<div style="text-align:right;">#= kendo.toString(Ammount_MC1, "n0") #</div>',
                                    width: 150
                                }, {
                                    field: "Qty_MC1",
                                    title: "Quantity",
                                    template: '<div style="text-align:right;">#= kendo.toString(Qty_MC1, "n1") #</div>',
                                    width: 150
                                },
                                ]
                            },
                            {
                                title: "Moving Code 2",
                                columns: [
                                {
                                    field: "Ammount_MC2",
                                    title: "Amount",
                                    template: '<div style="text-align:right;">#= kendo.toString(Ammount_MC2, "n0") #</div>',
                                    width: 150
                                }, {
                                    field: "Qty_MC2",
                                    title: "Quantity",
                                    template: '<div style="text-align:right;">#= kendo.toString(Qty_MC2, "n1") #</div>',
                                    width: 150
                                },
                                ]
                            },
                            {
                                title: "Moving Code 3",
                                columns: [
                                {
                                    field: "Ammount_MC3",
                                    title: "Amount",
                                    template: '<div style="text-align:right;">#= kendo.toString(Ammount_MC3, "n0") #</div>',
                                    width: 150
                                }, {
                                    field: "Qty_MC3",
                                    title: "Quantity",
                                    template: '<div style="text-align:right;">#= kendo.toString(Qty_MC3, "n1") #</div>',
                                    width: 150
                                },
                                ]
                            },
                            {
                                title: "Moving Code 4",
                                columns: [
                                {
                                    field: "Ammount_MC4",
                                    title: "Amount",
                                    template: '<div style="text-align:right;">#= kendo.toString(Ammount_MC4, "n0") #</div>',
                                    width: 150
                                }, {
                                    field: "Qty_MC4",
                                    title: "Quantity",
                                    template: '<div style="text-align:right;">#= kendo.toString(Qty_MC4, "n1") #</div>',
                                    width: 150
                                },
                                ]
                            }, {
                                title: "Moving Code 5",
                                columns: [
                                {
                                    field: "Ammount_MC5",
                                    title: "Amount",
                                    template: '<div style="text-align:right;">#= kendo.toString(Ammount_MC5, "n0") #</div>',
                                    width: 150
                                }, {
                                    field: "Qty_MC5",
                                    title: "Quantity",
                                    template: '<div style="text-align:right;">#= kendo.toString(Qty_MC5, "n1") #</div>',
                                    width: 150
                                },
                                ]
                            },
                       ]
                   },
                   {
                       field: "Slow_Moving",
                       title: "Slow Moving",
                       template: '<div style="text-align:right;">#= kendo.toString(Slow_Moving, "n2") #</div>',
                       width: 175
                   },
                   {
                       title: "Lead Time Order",
                       columns: [
                            {
                                field: "LT_Reguler",
                                title: "Reguler",
                                template: '<div style="text-align:right;">#= kendo.toString(LT_Reguler, "n2") #</div>',
                                width: 150
                            }, {
                                field: "LT_Emergency",
                                title: "Emergency",
                                template: '<div style="text-align:right;">#= kendo.toString(LT_Emergency, "n2") #</div>',
                                width: 150
                            }, 
                       ]
                   },

                  
            ],
        });

        $("#LapSpAnalisisBulanan").attr("style", "height:80px");
        $(".k-grid-header").attr("style", "height:80px");
        $(".k-grid-header-wrap").attr("style", "height:80px");
        $(".k-header").attr("style", "border : solid #94c0d2 1px;");

    }
    
    me.exportXls = function () {
        var spID = "uspfn_spAnalisisBulananViewgrid";

        var url = "sp.api/ReportSparepartMonthly/exportExcel?";
        var area = $('#Area').select2('data').id;
        var dealer = $('#CompanyCode').select2('data').id;
        var outlet = $('#BranchCode').select2('data').id;
        var year = $('#Year').select2('data').id;
        var areaText = $('#Area').select2('data').text;
        var dealerText = $('#CompanyCode').select2('data').text;
        var outletText = $('#BranchCode').select2('data').text;
        var month = $('#Month').select2('data').id;
        var monthText = $('#Month').select2('data').text;
        var typeOfGoods = $('#typeofgoods').select2('data').id;
        var typeOfGoodsText = $('#typeofgoods').select2('data').text;

        var
        params = "&Area=" + area;
        params += "&Dealer=" + dealer;
        params += "&Outlet=" + outlet;
        params += "&SpID=" + spID;
        params += "&Year=" + year;
        params += "&AreaText=" + areaText;
        params += "&DealerText=" + dealerText;
        params += "&OutletText=" + outletText;
        params += "&Month=" + month;
        params += "&MonthText=" + monthText;
        params += "&TypeOfGoods=" + typeOfGoods;
        params += "&TypeOfGoodsText=" + typeOfGoodsText;

        url = url + params;
        window.location = url;

        console.log(url);
    }

    me.default = function () {
        $http.post('sp.api/ReportSparepartMonthly/default').
        success(function (data, status, headers, config) {
            me.data.Area = data.Area;
            me.data.CompanyCode = data.Dealer;
            me.data.BranchCode = data.BranchCode;
            me.data.Year = data.Year;
            me.data.Month = data.Month;

            console.log(data)
        });
    }

    me.clearGrid = function () {
        $("#LapSpAnalisisBulanan").empty();
    }

    me.initialize = function () {
        me.default();
    }

    me.start();

    me.HideForm = function () {
        $(".body > .panel").fadeOut();
    }
}

$(document).ready(function () {
    var options = {
        title: "Laporan Sparepart Analisis Bulanan",
        xtype: "panels",

        panels: [
            {
                name: "pnlFilter",
                items: [
                    {
                        text: "Area",
                        type: "controls",
                        items: [
                            { name: "Area", cls: "span6", type: "select2", opt_text: "-- SELECT ALL --", disable: true, datasource: "comboArea" },
                        ]
                    },
                    {
                        text: "Dealer Name",
                        type: "controls",
                        items: [
                            { name: "CompanyCode", cls: "span6", type: "select2", opt_text: "-- SELECT ALL --", disable: true, datasource: "comboDealer" },
                        ]
                    },
                    {
                        text: "Branch Name",
                        type: "controls",
                        items: [
                            { name: "BranchCode", cls: "span6", type: "select2", opt_text: "-- SELECT ALL --", disable: true, datasource: "comboOutlet" },
                        ]
                    },
                    {
                        text: "Tahun",
                        type: "controls",
                        items: [
                            { name: "Year", text: "Year", cls: "span3", type: "select2", datasource: 'comboYear', opt_text: "-- SELECT ONE --" },
                             { name: "Month", cls: "span4", cls: "span3", text: "Month", type: "select2", datasource: "comboMonth" }
                        ]
                    },
                    {
                        text: "Type Part",
                        type: "controls",
                        items: [
                            { name: "typeofgoods", text: "Year", cls: "span3", type: "select2", datasource: 'comboTypeOfGoods', opt_text: "-- SELECT ALL --" },
                        ]
                    },
                ],
            },
            {
                name: "LapSpAnalisisBulanan",
                xtype: "k-grid",
                //xtype: "wxtable",
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
        SimDms.Angular("spLapSparepartBulanan");
    }
});
