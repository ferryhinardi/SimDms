
function svClosingMonthController($scope, $http, $injector) {

    var me = $scope;

    $injector.invoke(BaseController, this, { $scope: me });

    me.default = function () {
        $http.post("sv.api/closingmonth/default")
        .success(function (e) {
            me.data = e;
            if (e.status == 1) $('#btnClosing').removeAttr('disabled');
            else {
                if (e.status == 0) MsgBox(e.message, MSG_ERROR);
                else MsgBox(e.message);
                $('#btnClosing').attr('disabled', true);
            }
        })
    }

    me.closing = function () {
        $http.post("sv.api/closingmonth/validateclosing")
        .success(function (e) {
            if (e.success == false) {
                $('#wxClosingMonth').show();
                MsgBox(e.message, MSG_ERROR);
                me.loadTableData(me.gridClosingMonth, e.grid);
            }
            else {
                Wx.confirm("Apakah data sudah dilakukan Backup ?", function (e) {
                    if (e == 'Yes') {
                        Wx.confirm("Apakah anda sudah yakin ?", function (e) {
                            if (e == 'Yes') {
                                $http.post('sv.api/closingmonth/closing')
                                .success(function (e) {
                                    if (e.success) {
                                        Wx.Success("Proses tutup bulan berhasil");
                                        $http.post("sv.api/closingmonth/default")
                                        .success(function (e) {
                                            me.data = e;
                                            $('#btnClosing').attr('disabled', true);
                                        });
                                    } else {
                                        MsgBox(e.message, MSG_ERROR);
                                    }
                                })
                                .error(function (e) {
                                    MsgBox('Terjadi Kesalahan, Hubungi SDMS Support');
                                });
                            }
                        })
                    }
                })
            }
        })
    }

    me.gridClosingMonth = new webix.ui({
        container: "wxClosingMonth",
        view: "wxtable", css:"alternating",
        columns: [
                { id: "BranchCode", header: "BranchCode", width: 90 },
                { id: "InvoiceNo", header: "No. Invoice", width: 120 },
                { id: "InvoiceDate", header: "Tgl. Invoice", width: 130, format: me.dateFormat },
                { id: "JobOrderNo", header: "No. SPK", width: 120 },
                { id: "JobOrderDate", header: "Tgl. SPK", width: 130, format: me.dateFormat },
                { id: "FPJNo", header: "No. FPJ", width: 120 },
                { id: "FPJDate", header: "Tgl. FPJ", width: 130, format: me.dateFormat },
                { id: "Info", header: "Keterangan", fillspace: true }
        ],
        on: {
            onSelectChange: function () {
                //if (me.grid1.getSelectedId() !== undefined) {
                //    me.pcmodel = this.getItem(me.grid1.getSelectedId().id);
                //    me.pcmodel.oid = me.grid1.getSelectedId();
                //    me.showProfitCenterInfo(me.data.CustomerCode, me.pcmodel.ProfitCenterCode);
                //    me.Apply();
                //}
            }
        }
    });

    me.initialize = function () {
        me.default();
        $('#wxClosingMonth').hide();
        me.clearTable(me.gridClosingMonth);

    }

    me.gridClosingMonth.adjust();

    me.start();
}

$(document).ready(function () {
    var options = {
        title: "Proses Tutup Bulan",
        xtype: "panels",
        toolbars: [
            { name: "btnNew", text: "Clear", cls: "btn btn-primary", icon: "", click: "cancelOrClose()" },
        ],
        panels: [
            {
                name: "pnlClosingMonth",
                items: [
                   { name: "FiscalYear", text: "Tahun Fiskal", cls: "span2", readonly: true },
                   { name: "FiscalMonth", text: "Bulan Fiskal", cls: "span2", readonly: true },
                   { type: "div" ,style:"padding-bottom:1px"},
                   { name: "Periode", text: "Periode", cls: "span2", readonly: true },
                   { name: "PeriodeName", text: "Nama Periode", cls: "span2", readonly: true },
                   {
                       type: "buttons", cls: "span4", items: [
                              { name: "btnClosing", text: "TUTUP BULAN", cls: "btn btn-info", click: "closing()", disable: true }
                       ]
                   },
                   {
                       name: "wxClosingMonth",
                       type: "wxdiv",
                   },
                ]
            }
        ]
    };

    Wx = new SimDms.Widget(options);
    Wx.default = {};
    Wx.render(init);

    function init(s) {
        SimDms.Angular("svClosingMonthController");
    }
});