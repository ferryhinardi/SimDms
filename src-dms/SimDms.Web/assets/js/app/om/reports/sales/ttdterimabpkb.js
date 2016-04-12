"use strict";
function RptTandaTerimaBpkb($scope, $http, $injector) {

    var me = $scope;

    $injector.invoke(BaseController, this, { $scope: me });

    me.Storage = [
       { "value": '0', "text": 'Leasing' },
       { "value": '1', "text": 'Cabang' },
       { "value": '2', "text": 'Pelanggan' }
    ];

    me.BPKBCode = function () {
        var docNoStart = $('#DocNoStart').val();
        var docNoTo = $('#DocNoStart').val();
        var lookup = Wx.blookup({
            name: "TtdTerimaBpkbStrg4Report",
            title: "Tempat Penyimpanan",
            manager: spSalesManager,
            query: new breeze.EntityQuery.from('TtdTerimaBpkbStrg4Report').withParameters({ Storage: me.data.Storage, DocNoStart: docNoStart, DocNoTo: docNoTo }),
            defaultSort: "BPKBOutBy asc",
            columns: [
                { field: "BPKBOutBy", title: "BPKB Code" },
                { field: "BPKBOutByName", title: "BPKB Name" },
            ],
        });
        lookup.dblClick(function (data) {
            if (data != null) {
                me.data.BPKBCode = data.BPKBOutBy;
                me.data.BPKBName = data.BPKBOutByName;
                me.Apply();
            }
        });
        console.log(docNoStart, docNoTo);
    }

    me.DocNoStart = function () {
        var bPKBCode = $('#BPKBCode').val();
        var lookup = Wx.blookup({
            name: "TtdTerimaBpkbDocNo4Report",
            title: "No. Document",
            manager: spSalesManager,
            query: new breeze.EntityQuery.from('TtdTerimaBpkbDocNo4Report').withParameters({ Storage: me.data.Storage, BPKBCode: bPKBCode }),
            defaultSort: "DocNo asc",
            columns: [
                { field: "DocNo", title: "No. Document" },
                 {
                     field: "DocDate", title: "Tanggal Document",
                     template: "#= (DocDate == undefined) ? '' : moment(DocDate).format('DD MMM YYYY') #"
                 },
                { field: "Remark", title: "Keterangan" },
            ],
        });
        lookup.dblClick(function (data) {
            if (data != null) {
                me.data.DocNoStart = data.DocNo;
                me.data.DocNoStartDate = data.DocDate;
                me.Apply();
            }
        });
        console.log(docNoStart, docNoTo);
    }

    me.DocNoTo = function () {
        var bPKBCode = $('#BPKBCode').val();
        var lookup = Wx.blookup({
            name: "TtdTerimaBpkbDocNo4Report",
            title: "No. Document",
            manager: spSalesManager,
            query: new breeze.EntityQuery.from('TtdTerimaBpkbDocNo4Report').withParameters({ Storage: me.data.Storage, BPKBCode: bPKBCode }),
            defaultSort: "DocNo asc",
            columns: [
                { field: "DocNo", title: "No. Document" },
                 {
                     field: "DocDate", title: "Tanggal Document",
                     template: "#= (DocDate == undefined) ? '' : moment(DocDate).format('DD MMM YYYY') #"
                 },
                { field: "Remark", title: "Keterangan" },
            ],
        });
        lookup.dblClick(function (data) {
            if (data != null) {
                me.data.DocNoTo = data.DocNo;
                me.data.DocNoToDate = data.DocDate;
                me.Apply();
            }
        });
        console.log(docNoStart, docNoTo);
    }



    me.printPreview = function () {
        if (me.data.BPKBCode==null) {
            MsgBox('Ada data yang belum lengkap', MSG_ERROR);
            return;
        }
        if (me.data.DocNoStartDate == null || me.data.DocNoToDate == null) {
            MsgBox('Ada data yang belum lengkap', MSG_ERROR);
            return;
        }
        if (me.data.DocNoStartDate > me.data.DocNoToDate) {
            MsgBox('No. Document awal tidak boleh lebih besar dari No. Document akhir', MSG_ERROR);
            return;
        }

        var param = [
                    me.data.DocNoStart,
                    me.data.DocNoTo,
                    me.data.BPKBCode

        ];

        Wx.showPdfReport({
            id: 'OmRpSalRgs028',
            pparam: param.join(','),
            textprint: true,
            rparam: 'Print Tanda Terima BPKB',
            type: "devex"
        });
    }

    me.initialize = function () {
        me.data = {};
        me.change = false;
        me.data.Storage = '0';
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
        title: "Report Tanda terima BPKB",
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
                        { name: "Storage", opt_text: "", cls: "span3", type: "select2", text: "Kode Penyimpanan", datasource: "Storage" },
                        {
                            text: "Tempat Penyimpanan",
                            type: "controls",
                            items: [


                                { name: "BPKBCode", model: "data.BPKBCode", cls: "span2", placeHolder: " ", readonly: true, type: "popup", click: "BPKBCode()" },
                                { name: "BPKBName", model: "data.BPKBName", cls: "span6", placeHolder: " ", readonly: true }
                            ]
                        },
                        {
                            text: "No Document",
                            type: "controls",
                            items: [


                                { name: "DocNoStart", model: "data.DocNoStart", cls: "span2", placeHolder: " ", readonly: true, type: "popup", click: "DocNoStart()" },
                                { name: "DocNoStartDate", model: "data.DocNoStartDate", cls: "span6", placeHolder: " ", readonly: true, show: false }
                            ]
                        },
                        {
                            text: "S/D",
                            type: "controls",
                            items: [


                                { name: "DocNoTo", model: "data.DocNoTo", cls: "span2", placeHolder: " ", readonly: true, type: "popup", click: "DocNoTo()" },
                                { name: "DocNoToDate", model: "data.DocNoToDate", cls: "span6", placeHolder: " ", readonly: true, show: false }
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
        $(".switchlabel").attr("style", "padding:9px 0px 0px 5px")
        SimDms.Angular("RptTandaTerimaBpkb");

    }
});