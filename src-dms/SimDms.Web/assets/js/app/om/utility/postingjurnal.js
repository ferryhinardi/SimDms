var totalSrvAmt = 0;
var status = 'N';
var svType = '2';

"use strict";

function omPostingJurnalController($scope, $http, $injector) {

    var me = $scope;

    $injector.invoke(BaseController, this, { $scope: me });

    me.Status = [
        { "value": '0', "text": 'POSTED' },
        { "value": '1', "text": 'UNPOSTED' },
    ];

    me.initialize = function () {
        me.data.Trans = "0";
        me.data.Status = "0";
        me.data.Transaksi = "ALL";
        $http.post('om.api/PostingJurnal/Periode').
          success(function (e) {
              me.data.DateFrom = e.DateFrom;
              me.data.DateTo = e.DateTo;
              me.data.DateFromHide = e.DateFrom;
              me.data.DateToHide = e.DateTo;
          });
        
        $http.post('om.api/PostingJurnal/DefaultPeriod').
            success(function (result, status, headers, config) {
                if (result.data.length > 0) {
                    me.data.Year = result.data[0].FiscalYear;
                    me.data.PriodeID = result.data[0].FiscalYear + '' + result.data[0].FiscalPeriod;
                    me.data.PriodeDesc = 'Dari Tanggal ' + moment(result.data[0].PeriodBeg).format('DD MMM YYYY') + ' s/d Tanggal ' + moment(result.data[0].PeriodEnd).format('DD MMM YYYY');
                }
            });
        me.gridTransaksi.adjust();
    }

    $("[name = 'Transaksi']").on('change', function () {
        me.clearTable(me.gridTransaksi);
        $http.post('om.api/PostingJurnal/ComboTransaksi', me.data).
                success(function (data, status, headers, config) {
                    if (data.success) {
                        if (data.data.Data != "") {
                            $(".panel.tabpage1").hide();
                            $(".panel.tabpage1.tA").show();
                            me.detail = data.data.Data
                            me.loadTableData(me.gridTransaksi, data.data.Data);
                        }
                    } else {
                        MsgBox(data.message, MSG_ERROR);
                    }
                });
    });

    $("[name = 'Status']").on('change', function () {
        me.clearTable(me.gridTransaksi);
        console.log(me.data);
        $http.post('om.api/PostingJurnal/ComboTransaksi', me.data).
                success(function (data, status, headers, config) {
                    if (data.success) {
                        if (data.data.Data != "") {
                            $(".panel.tabpage1").hide();
                            $(".panel.tabpage1.tA").show();
                            me.detail = data.data.Data
                            me.loadTableData(me.gridTransaksi, data.data.Data);
                        }
                    } else {
                        MsgBox(data.message, MSG_ERROR);
                    }
                });
    });

    me.$watch('data.Trans', function (newValue, oldValue) {
        if (newValue !== oldValue) {
            me.$broadcast(newValue);
            $http.post('om.api/PostingJurnal/TransaksiDataID?Trans=' + newValue).
            success(function (data, status, headers, config) {
                me.Transaksi = data;
                me.initGrid();
                me.Apply();
            });
        }
    });

    me.Process = function () {
        if (me.detail.length < 1)
            return;
        MsgConfirm("Anda yakin?", function (e) {
            if (e) {
                var datDetail = [];

                $.each(me.detail, function (key, val) {
                    if (val["isSelected"] == 1) {
                        var arr = {
                            "DocNo": val["DocNo"],
                            "TypeJournal": val["TypeJournal"],
                            "isSelected": val["isSelected"]
                        }
                        datDetail.push(arr);
                    }
                });

                var dat = {};
                dat["model"] = datDetail;
                var JSONData = JSON.stringify(dat);

                $http.post('om.api/PostingJurnal/PostingJurnal', JSONData).
                success(function (dl, status, headers, config) {
                    if (dl.success) {
                        MsgBox("Proses Posting Data berhasil");
                        me.initialize();
                    } else {
                        MsgBox(dl.message, MSG_ERROR);
                        console.log(dl.error_log);
                    }
                }).
                error(function (e, status, headers, config) {
                    MsgBox("Connecting server error", MSG_ERROR);
                });
            }
        });
    };

    me.initGrid = function () {
        if (me.data.Trans == "2") {
            var col = [
                { id: "isSelected", header: { content: "masterCheckbox", contentId: "chkSelect" }, width: 50, template: "{common.checkbox()}" },
                { id: "Status", header: "Status", width: 100 },
                { id: "TypeJournal", header: "Jenis Transaksi", width: 150 },
                { id: "DocNo", header: "No.Dokumen", width: 150 },
                { id: "DocDate", header: "Tgl.Dokumen", width: 150 },
                { id: "BranchCodeFrom", header: "Branch Code From", width: 150 },
                { id: "BranchCodeTo", header: "Branch Code To", width: 300 },
                { id: "RefNo", header: "No.Ref.", width: 100 },
            ]
        } else if (me.data.Trans == "1") {
            var col = [
                { id: "isSelected", header: { content: "masterCheckbox", contentId: "chkSelect" }, width: 50, template: "{common.checkbox()}" },
                { id: "Status", header: "Status", width: 100 },
                { id: "TypeJournal", header: "Jenis Transaksi", width: 150 },
                { id: "DocNo", header: "No.Dokumen", width: 150 },
                { id: "DocDate", header: "Tgl.Dokumen", width: 150 },
                { id: "RefCode", header: "Kode Pelanggan", width: 150 },
                { id: "RefName", header: "Nama Pelanggan", width: 300 },
                { id: "RefNo", header: "No.Ref.", width: 100 },
            ]
        } else {
            var col = [
                { id: "isSelected", header: { content: "masterCheckbox", contentId: "chkSelect" }, width: 50, template: "{common.checkbox()}" },
                { id: "Status", header: "Status", width: 100 },
                { id: "TypeJournal", header: "Jenis Transaksi", width: 150 },
                { id: "DocNo", header: "No.Dokumen", width: 150 },
                { id: "DocDate", header: "Tgl.Dokumen", width: 150 },
                { id: "RefCode", header: "Kode Pemasok", width: 150 },
                { id: "RefName", header: "Nama Pemasok", width: 300 },
                { id: "RefNo", header: "No.Ref.", width: 100 },
            ]
        }
        if(me.gridTransaksi != undefined)
        me.gridTransaksi.destructor();
        me.gridTransaksi = new webix.ui({
            container: "GridTransaksi",
            view: "wxtable", css:"alternating",
            scrollX: true,
            scrollY: true,
            height: 500,
            autoheight: false,
            columns: col,
            on: {
                onSelectChange: function () {
                    if (me.gridTransaksi.getSelectedId() !== undefined) {
                        var data = this.getItem(me.gridTransaksi.getSelectedId().id);

                        var datas = {
                            "TypeJournal": data.TypeJournal,
                            "DocNo": data.DocNo
                        }

                        $http.post('om.api/PostingJurnal/GetJurnal', datas)
                        .success(function (e) {
                            if (e.success) {
                                $(".panel.tabpage1").hide();
                                $(".panel.tabpage1.tB").show();
                                me.loadTableData(me.gridJurnal, e.data);

                                me.data.AmountDb = e.dataDtl[0].AmountDb;
                                me.data.AmountCr = e.dataDtl[0].AmountCr;
                                me.Apply();
                                me.ReformatNumber();

                                if (e.dataDtl[0].AmountDb == e.dataDtl[0].AmountCr) {
                                    me.data.LabelStatus = "BALANCE";
                                    $('#LabelStatus').html(me.data.LabelStatus);
                                    $('#LabelStatus').css(
                                    {
                                        "font-size": "18px",
                                        "color": "blue",
                                        "font-weight": "bold",
                                        "text-align": "center"
                                    });
                                } else {
                                    me.data.LabelStatus = "UNBALANCE";
                                    $('#LabelStatus').html(me.data.LabelStatus);
                                    $('#LabelStatus').css(
                                    {
                                        "font-size": "18px",
                                        "color": "red",
                                        "font-weight": "bold",
                                        "text-align": "center"
                                    });
                                }
                            } else {
                                MsgBox(e.message, MSG_ERROR);
                            }
                        })
                        .error(function (e) {
                            MsgBox('Terjadi Kesalahan, Hubungi SDMS Support', MSG_ERROR);
                        });
                    }
                }
            }
        });
    }

    me.gridJurnal = new webix.ui({
        container: "GridJurnal",
        view: "wxtable", css: "alternating", scrollX: true,
        columns: [
            { id: "AccountNo", header: "No.Account", width: 250 },
            { id: "AccDescription", header:  "Keterangan Account" , width: 450},
            { id: "TypeTrans", header:  "Tipe Trans", width: 150 },
            { id: "AmountDb", header: "Debet", width: 150, format: me.decimalFormat, css: "rightcell" },
            { id: "AmountCr", header: "Kredit", width: 150, format: me.decimalFormat, css: "rightcell" },
        ]
    });

    me.initGrid();

    webix.event(window, "resize", function () {
        me.gridTransaksi.adjust();
    })

    me.start();
}

$(document).ready(function () {
    var options = {
        title: "Posting Jurnal",
        xtype: "panels",
        panels: [
            {
                name: "pnlPosting",
                title: "Posting",
                items: [
                    {
                        type: "optionbuttons", name: "Trans", model: "data.Trans",
                        items: [
                            { name: "0", text: "PURCHASE" },
                            { name: "1", text: "SALES" },
                            { name: "2", text: "TRANSFER STOK" },
                        ]
                    }
                ]
            },
            {
                name: "pnlPostingTrans",
                title: "Posting Purchase",
                items: [
                    { name: "Year", text: "Tahun Fiskal", cls: "span2 full", readonly: true },
                    {
                        text: "Periode",
                        type: "controls",
                        cls: "span6",
                        items: [
                            { name: "PriodeID", cls: "span2", readonly: true },
                            { name: "PriodeDesc", cls: "span6", readonly: true },
                        ]
                    },
                    {
                        text: "Tanggal",
                        type: "controls",
                        cls: "span6",
                        items: [
                            { name: "DateFrom", text: "", cls: "span3", type: "ng-datepicker" },
                            { type: "label", text: "s/d", cls: "span1 mylabel" },
                            { name: "DateTo", text: "", cls: "span3", type: "ng-datepicker" },
                        ]
                    },
                    { name: "Transaksi", cls: "span3", type: "select2", text: "Transaksi", datasource: "Transaksi" },
                    { name: "Status", cls: "span3", type: "select2", text: "Status", datasource: "Status" },
                    {
                        type: "buttons",
                        items: [
                             { name: "Process", text: "Process", icon: "icon-Search", cls: "span4", click: "Process()", disable: "data.Status == 0" },
                        ]
                    },
                    { name: "DateFromHide", text: "", cls: "span3", type: "ng-datepicker", show: false },
                    { name: "DateToHide", text: "", cls: "span3", type: "ng-datepicker", show: false },
                ]
            },
            {
                xtype: "tabs",
                name: "tabpage1",
                items: [
                    { name: "tA", text: "Transaksi", cls: "active" },
                    { name: "tB", text: "Jurnal" },
                ]
            },
                    {
                        name: "GridTransaksi",
                        title: "Transaksi",
                        cls: "tabpage1 tA",
                        xtype: "wxtable"
                    },
                    {
                        name: "GridJurnal",
                        title: "Jurnal",
                        cls: "tabpage1 tB",
                        xtype: "wxtable"
                    },
            {
                title: "",
                cls: "tabpage1 tB",
                items: [
                    { name: "LabelStatus",type: "label", text: "", cls: "span4" },
                    {
                        text: "Total",
                        type: "controls",
                        cls: "span4",
                        items: [
                            { name: "AmountDb", cls: "span4 number-int", readonly: true },
                            { name: "AmountCr", cls: "span4 number-int", readonly: true },
                        ]
                    },
                ]
            },
        ]
    };

    Wx = new SimDms.Widget(options);
    Wx.default = {};
    Wx.render(init);

    function init(s) {
        $(".mylabel").attr("style", "padding:9px 9px 5px 5px", "align: center");
        SimDms.Angular("omPostingJurnalController");
    }

});