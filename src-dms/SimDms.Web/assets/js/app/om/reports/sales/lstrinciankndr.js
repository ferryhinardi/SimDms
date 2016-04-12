"use strict"; //Reportid OmRpSalesRgs001
function RptRegisterRincianKendaraan($scope, $http, $injector) {

    var me = $scope;

    $injector.invoke(BaseController, this, { $scope: me });

    me.PriodeCode = function () {
        var lookup = Wx.blookup({
            name: "PriodeLookup4Report",
            title: "Priode",
            manager: spSalesManager,
            query: new breeze.EntityQuery.from('PriodeLookup4Report').withParameters({FiscalYear: me.data.FiscalYear}),
            defaultSort: "Periode asc",
            columns: [
                { field: "Periode", title: "Priode"},
                {
                    field: "FromDate", title: "Dari Tanggal",
                    template: "#= moment(FromDate).format('DD MMM YYYY') #"
                },
                {
                    field: "EndDate", title: "Sampai Tanggal",
                    template: "#= moment(EndDate).format('DD MMM YYYY') #"
                },
            ],
        });
        lookup.dblClick(function (data) {
            if (data != null) {
                $('#PriodeCode').css("text-align", "right");
                me.data.PriodeCode = data.Periode;
                me.data.PriodeDesc = "Dari Tanggal " + moment(data.FromDate).format('DD-MMM-YYYY') + " s.d tanggal " + moment(data.EndDate).format('DD-MMM-YYYY');
                localStorage.setItem('FromDate', data.FromDate);
                localStorage.setItem('EndDate', data.EndDate);
                localStorage.setItem('PeriodeName', data.PeriodeName);
                me.Apply();
            }
        });

    }

    me.printPreview = function () {
        if (me.data.PriodeCode == undefined) {
            MsgBox('Ada informasi yang belum lengkap', MSG_ERROR);
        }
        else {
            var FromDate = localStorage.getItem('FromDate');
            var EndDate = localStorage.getItem('EndDate');
            var PeriodeName = localStorage.getItem('PeriodeName');

            var ReportId = 'OmRpSalRgs009';
            var par = [
                           moment(FromDate).format('YYYYMMDD'),
                           moment(EndDate).format('YYYYMMDD')
            ]
            var rparam = 'PERIODE : ' + PeriodeName;

            Wx.showPdfReport({
                id: ReportId,
                pparam: par.join(','),
                textprint: true,
                rparam: rparam,
                type: "devex"
            });
        }
    }

    me.initialize = function () {
        me.data.FiscalYear = new Date(Date.now()).getFullYear();
        $('#FiscalYear').css("text-align", "right");

        $http.get('breeze/sales/CurrentUserInfo').
              success(function (dl, status, headers, config) {
                  me.data.CompanyCode = dl.CompanyCode;
                  //me.data.BranchCode = dl.BranchCode;
              });
        $http.get('breeze/sales/ProfitCenter').
        success(function (dl, status, headers, config) {
            me.data.ProfitCenterCode = dl.ProfitCenter;
        });
        me.Apply();
    }

me.start();
}

$(document).ready(function () {
    var options = {
        title: "Daftar Rincian Kendaraan Bermotor",
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
                    { name: "FiscalYear", text: "Tahun Fiskal", cls: "span2 full" },
                    {
                        type: "controls",
                        text: "Periode",
                        required: true, validasi: "required",
                        items: [
                    { name: "PriodeCode", cls: "span2", type: "popup", click: "PriodeCode()" },
                    { name: "PriodeDesc", cls: "span4", readonly: true },
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
        $(".mylabel").attr("style", "padding:9px 9px 0px 5px");
        SimDms.Angular("RptRegisterRincianKendaraan");
    }



});