var totalSrvAmt = 0;
var status = 'N';
var svType = '2';

"use strict";

function omRePostingJurnalController($scope, $http, $injector) {

    var me = $scope;

    $injector.invoke(BaseController, this, { $scope: me });

    me.Status = [
        { "value": '0', "text": 'POSTED' },
        { "value": '1', "text": 'UNPOSTED' },
    ];

    me.initialize = function () {
        me.data.Trans = "2";
        $http.post('om.api/RePostingJurnal/PopulateData', me.data).
                success(function (data, status, headers, config) {
                    if (data.success) {
                        me.detail = data.data;
                        me.loadTableData(me.gridTransaksi, data.data);
                    } else {
                        MsgBox(data.message, MSG_ERROR);
                    }
                });

        me.gridTransaksi.adjust();
    }

    me.Process = function () {
        if (me.detail.length < 1)
            return;
        MsgConfirm("Anda yakin?", function (e) {
            if (e) {
                var datDetail = [];

                $.each(me.detail, function (key, val) {
                    var arr = {
                        "DocNo": val["DocNo"],
                        "TypeJournal": val["TypeJournal"]
                    }
                    datDetail.push(arr);
                });

                var dat = {};
                dat["model"] = datDetail;
                var JSONData = JSON.stringify(dat);

                $http.post('om.api/RePostingJurnal/RePostingJurnal', JSONData).
                success(function (dl, status, headers, config) {
                    if (dl.success) {
                        MsgBox(dl.message);
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

        me.gridTransaksi = new webix.ui({
            container: "GridTransaksi",
            view: "wxtable", css:"alternating",
            scrollX: true,
            scrollY: true,
            height: 500,
            autoheight: false,
            columns: [
                { id: "isSelected", header: { content: "masterCheckbox", contentId: "chkSelect" }, width: 50, template: "{common.checkbox()}" },
                { id: "Status", header: "Status", width: 100 },
                { id: "TypeJournal", header: "Jenis Transaksi", width: 150 },
                { id: "DocNo", header: "No.Dokumen", width: 150 },
                { id: "DocDate", header: "Tgl.Dokumen", width: 150 },
                { id: "BranchCodeFrom", header: "Branch Code From", width: 150 },
                { id: "BranchCodeTo", header: "Branch Code To", width: 300 },
                { id: "RefNo", header: "No.Ref.", width: 100 },
            ],
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

    me.gridJurnal = new webix.ui({
        container: "GridJurnal",
        view: "wxtable", css:"alternating",
        scrollX: true,
        scrollY: true,
        height: 500,
        autoheight: false,
        columns: [
            { id: "AccountNo", header: "No.Account", width: 250 },
            { id: "AccDescription", header: "Keterangan Account", width: 350 },
            { id: "TypeTrans", header: "Tipe Trans", width: 150 },
            { id: "AmountDb", header: "Debet", width: 150, format: webix.i18n.numberFormat, css: { "text-align": "right" } },
            { id: "AmountCr", header: "Kredit", width: 150, format: webix.i18n.numberFormat, css: { "text-align": "right" } },
        ]

    });

    webix.event(window, "resize", function () {
        me.gridTransaksi.adjust();
    })

    me.start();
}

$(document).ready(function () {
    var options = {
        title: "Re-Posting Jurnal",
        xtype: "panels",
        panels: [
            {
                name: "pnlPosting",
                title: "Re-Posting",
                items: [
                    {
                        type: "optionbuttons", name: "Trans", model: "data.Trans", disable: true,
                        items: [
                            { name: "0", text: "PURCHASE", disable: true },
                            { name: "1", text: "SALES", disable: true },
                            { name: "2", text: "TRANSFER STOK" },
                        ]
                    }
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
                name: "pnlProsess",
                items: [
                    {
                        type: "buttons",
                        items: [
                             { name: "Process", text: "Process", icon: "icon-Search", cls: "span4", click: "Process()", disable: "data.Status == 0" },
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
        SimDms.Angular("omRePostingJurnalController");
    }

});