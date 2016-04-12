"use strict";

var baseOnYear = "0";
var isBranch = false;
var year = (new Date).getFullYear();


function RptPenjualanTerbaik($scope, $http, $injector) {

    var me = $scope;


    $injector.invoke(BaseController, this, { $scope: me });

    me.Report = [
       { "value": '-1', "text": 'Kelompok AR' },
       { "value": '0', "text": 'Pelanggan' },
       { "value": '1', "text": 'Model' }
    ];

    me.$watch('optionByYear', function (newValue, oldValue) {
        me.init();
        if (newValue !== oldValue) {
            me.$broadcast(newValue);
            baseOnYear = newValue;
        }
    });

    me.PeriodFrom = function () {
        var yrs = $('#Year').val();
        if (yrs === '') {
            MsgBox('Tahun harus diisi', MSG_ERROR);
            return;
        }
        var lookup = Wx.blookup({
            name: "PenjualanTerbaikLookup4Report",
            title: "Periode From",
            manager: spSalesManager,
            query: new breeze.EntityQuery.from('PenjualanTerbaikLookup4Report').withParameters({ optionByYear: baseOnYear, Year: yrs }),
            columns: [
                { field: "Periode", title: "Periode" },
                { field: "PeriodeName", title: "Nama Periode" },
                {
                    field: "FromDate", title: "Dari Tanggal",
                    template: "#= (FromDate == undefined) ? '' : moment(FromDate).format('DD MMM YYYY') #"
                },
                {
                    field: "EndDate", title: "Sampai Tanggal",
                    template: "#= (EndDate == undefined) ? '' : moment(EndDate).format('DD MMM YYYY') #"
                },
            ],
        });
        lookup.dblClick(function (data) {
            if (data != null) {
                me.data.PeriodFrom = data.Periode;
                me.data.PeriodDesc = "Dari tanggal " + moment(data.FromDate).format('DD MMM YYYY') + " sampai tanggal " + moment(data.EndDate).format('DD MMM YYYY');
                me.data.PeriodDescTemp = data.FromDate;
                me.Apply();
            }
        });

        console.log(yrs);
    }

    me.PeriodTo = function () {
        var yrs = $('#Year').val();
        if (yrs === '') {
            MsgBox('Tahun harus diisi', MSG_ERROR);
            return;
        }
        var lookup = Wx.blookup({
            name: "PenjualanTerbaikLookup4Report",
            title: "Periode To",
            manager: spSalesManager,
            query: new breeze.EntityQuery.from('PenjualanTerbaikLookup4Report').withParameters({ optionByYear: baseOnYear, Year: yrs }),
            columns: [
                { field: "Periode", title: "Periode" },
                { field: "PeriodeName", title: "Nama Periode" },
                {
                    field: "FromDate", title: "Dari Tanggal",
                    template: "#= (FromDate == undefined) ? '' : moment(FromDate).format('DD MMM YYYY') #"
                },
                {
                    field: "EndDate", title: "Sampai Tanggal",
                    template: "#= (EndDate == undefined) ? '' : moment(EndDate).format('DD MMM YYYY') #"
                },
            ],
        });
        lookup.dblClick(function (data) {
            if (data != null) {
                me.data.PeriodTo = data.Periode;
                me.data.PeriodToDesc = "Dari tanggal " + moment(data.FromDate).format('DD MMM YYYY') + " sampai tanggal " + moment(data.EndDate).format('DD MMM YYYY');
                me.data.PeriodToTemp = data.EndDate;
                me.Apply();
            }
        });

    }

    me.Branch = function () {
        var lookup = Wx.blookup({
            name: "BranchPenjualanTerbaikLookup4Report",
            title: "Branch",
            manager: spSalesManager,
            query: "BranchPenjualanTerbaikLookup4Report",
            defaultSort: "BranchCode ASC",
            columns: [
                { field: "CompanyCode", title: "Kode Perusahaan" },
                { field: "BranchCode", title: "Kode Cabang" },
                { field: "CompanyName", title: "Nama Cabang" }
            ],
        });
        lookup.dblClick(function (data) {
            if (data != null) {
                me.data.Branch = data.BranchCode;
                me.data.BranchDesc = data.CompanyName;
                me.Apply();
            }
        });

    }

    $("#BranchSwitchN").on('change', function (e) {
        isBranch = false;
        $('#btnBranch').attr('disabled', 'disabled');
        $('#Branch, #BranchDesc').val('');
    });
    $("#BranchSwitchY").on('change', function (e) {
        isBranch = true;
        $('#btnBranch').removeAttr('disabled');
    });

    me.printPreview = function () {

        if ($('input[name=Year]').val() === '') {
            MsgBox('Tahun harus diisi', MSG_ERROR);
            return;
        }
        if ($('input[name=PeriodFrom]').val() === '') {
            return;
        }
        if ($('input[name=PeriodTo]').val() === '') {
            return;
        }
        if (me.data.PeriodDescTemp > me.data.PeriodToTemp) {
            MsgBox('Tanggal awal tidak boleh lebih besar dari tanggal akhir', MSG_ERROR);
            return;
        }

        if (me.data.Report === '-1') {
            var param = [
                me.data.PeriodDescTemp,
                me.data.PeriodToTemp
            ];

            Wx.showPdfReport({
                id: 'OmRpSalRgs010',
                pparam: param.join(','),
                textprint: true,
                rparam: 'Print Kelompok AR',
                type: "devex"
            });

        }
        if (me.data.Report === '0') {
            var ReportId = 'OmRpSalRgs011';
            var param = [
                me.data.PeriodDescTemp,
                me.data.PeriodToTemp
            ]
            var rparam = 'Print Pelanggan'

            Wx.showPdfReport({
                id: ReportId,
                pparam: param.join(','),
                textprint: true,
                rparam: rparam,
                type: "devex"
            });
        }
        if (me.data.Report === '1') {
            var ReportId = 'OmRpSalRgs012';
            var param = [
                me.data.PeriodDescTemp,
                me.data.PeriodToTemp
            ]
            var rparam = 'Print Model'

            Wx.showPdfReport({
                id: ReportId,
                pparam: param.join(','),
                textprint: true,
                rparam: rparam,
                type: "devex"
            });
        }
    }

    me.initialize = function () {
        me.data = {};
        me.change = false;
        me.data.Report = '-1';
        $('#btnBranch').attr('disabled', 'disabled');
        $('#Year').val(year);

        $http.get('breeze/sales/CurrentUserInfo').
          success(function (dl, status, headers, config) {
              me.data.CompanyCode = dl.CompanyCode;
              me.data.BranchCode = dl.BranchCode;
          });

        me.isPrintAvailable = true;

    }

    me.optionByYear = "0";
    me.start();
}

$(document).ready(function () {
    var options = {
        title: "Report Penjualan Terbaik",
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
                        text: "Berdasarkan Tahun",
                        type: "controls",
                        items: [
                            { name: "Year", cls: "span1", readonly: false },
                            {
                                type: "optionbuttons",
                                name: "tabpageoptions",
                                model: "optionByYear",
                                items: [
                                    { name: "0", text: "Tahun Fiskal" },
                                    { name: "1", text: "Tahun Kalender" },
                                ]
                            },
                        ]
                    },

                    {
                        text: "Periode Dari",
                        type: "controls",
                        items: [
                            { name: "PeriodFrom", cls: "span2", readonly: true, type: "popup", click: "PeriodFrom()" },
                            { name: "PeriodDesc", cls: "span5", placeHolder: " ", readonly: true },
                            { name: "PeriodDescTemp", cls: "span1", placeHolder: " ", readonly: true, show: false }
                        ]
                    },

                    {
                        text: "Periode Sampai",
                        type: "controls",
                        items: [
                            { name: "PeriodTo", cls: "span2", placeHolder: " ", readonly: true, type: "popup", click: "PeriodTo()" },
                            { name: "PeriodToDesc", cls: "span5", placeHolder: " ", readonly: true },
                            { name: "PeriodToTemp", cls: "span1", placeHolder: " ", readonly: true, show: false }
                        ]
                    },

                    { name: "Report", opt_text: "", cls: "span3", type: "select2", text: "Report", datasource: "Report" },

                    //{ name: "BranchSwitch", text: "Cabang", cls: "span2 full", type: "switch" },
                    //{
                    //    text: "",
                    //    type: "controls",
                    //    items: [
                    //        { name: "Branch", cls: "span2", placeHolder: " ", readonly: true, type: "popup", click: "Branch()" },
                    //        { name: "BranchDesc", cls: "span6", placeHolder: " ", readonly: true }
                    //    ]
                    //},

                ],
            },
        ]
    };

    Wx = new SimDms.Widget(options);
    Wx.default = {};
    Wx.render(init);

    function init(s) {
        SimDms.Angular("RptPenjualanTerbaik");
    }

});