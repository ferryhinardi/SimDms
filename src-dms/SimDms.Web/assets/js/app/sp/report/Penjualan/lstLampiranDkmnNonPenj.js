"use strict";
function spLstLmprnDokNonPenjualan($scope, $http, $injector) {

    var me = $scope;
    $injector.invoke(BaseController, this, { $scope: me });

    me.comboDocOrigin = [                       
                       { text: "Non Penjualan", value: '1'},
                       { text: "Service", value: '2' },
                       { text: "Unit Order", value: '3' }
    ];

    me.$watch('data.salesType', function (newValue, oldValue) {
        me.data.Lamp1 = '';
        me.data.Lamp2 = '';
    }, true);


    me.DocumentLkp = function (x) {

        var myqry = (me.data.salesType == '2' ? 
                    new breeze.EntityQuery().from("SpSrvLampLkp").withParameters({ salestype: me.data.salesType }) : 
                    new breeze.EntityQuery().from("SpRptLampLkp").withParameters({ salestype: me.data.salesType }) );
        
        var lookup = Wx.blookup({
            name: "SparepartLookup",
            title: "Pencarian No. Dokumen",
            manager: spManager,
            query: myqry,
            defaultSort: "LmpNo asc",
            columns: [
                { field: "LmpNo", title: "No. Dokumen" },
                { field: "LmpDate", title: "Tanggal Dokumen", template: "#= (LmpDate == undefined) ? '' : moment(LmpDate).format('DD MMM YYYY HH:mm:ss') #" },
                { field: "SSNo", title: "No. Dokumen" },
                { field: "SSDate", title: "Tanggal Dokumen", template: "#= (SSDate == undefined) ? '' : moment(SSDate).format('DD MMM YYYY HH:mm:ss') #" },
                { field: "PickingSlipNo", title: "No. Dokumen" },
                { field: "PickingSlipDate", title: "Tanggal Dokumen", template: "#= (PickingSlipDate == undefined) ? '' : moment(PickingSlipDate).format('DD MMM YYYY HH:mm:ss') #" },
                { field: "BPSFNo", title: "No. Dokumen" },
                { field: "BPSFDate", title: "Tanggal Dokumen", template: "#= (BPSFDate == undefined) ? '' : moment(BPSFDate).format('DD MMM YYYY HH:mm:ss') #" }
            ],
        });

        lookup.dblClick(function (data) {
            if (data != null) {
                if (x == 1) {
                    me.data.Lamp1 = data.LmpNo;
                    me.data.Lamp2 = data.LmpNo;
                }
                else {
                    me.data.Lamp2 = data.LmpNo;
                }
                me.Apply();
            }
        });
    }


    me.SignatureLkp = function (x) {
        var lookup = Wx.blookup({
            name: "SparepartLookup",
            title: "Pencarian No. Dokumen",
            manager: spManager,
            query: new breeze.EntityQuery().from("SignatureLkp").withParameters({ doctype: 'LMP' }),
            defaultSort: "SignName asc",
            columns: [
                { field: "SignName", title: "Nama" },
                { field: "TitleSign", title: "Jabatan" }
            ],
        });

        lookup.dblClick(function (data) {
            if (data != null) {
                me.data.SignName = data.SignName;
                me.data.SeqNo = data.SeqNo;
                me.Apply();
            }
        });
    }

    me.printPreview = function () {
        var params = {
            firstLmp: me.data.Lamp1,
            lastLmp: me.data.Lamp2,
            salesType:  me.data.salesType
        };

        $http.post('sp.api/SpInquiry/UpdateRangeLampiran', params)
        .success(function (e) {
            if (e.success) {
                var no = me.data.Lamp1 == undefined ? '' : me.data.Lamp1;
                var no2 = me.data.Lamp2 == undefined ? '' : me.data.Lamp2;
                var sign = me.data.SignName == undefined ? '' : me.data.SignName;

                if ((no == '' && no2 == '') || sign == '') {
                    MsgBox('Ada informasi yang belum lengkap', MSG_WARNING);
                    return;
                }

                var prm = [me.data.salesType, me.data.Lamp1, me.data.Lamp2, "profitcenter", "%", me.data.SeqNo];
                var slstyp = '';

                Wx.showPdfReport({
                    id: "SpRpTrn028",
                    pparam: prm.join(','),
                    type: "devex"
                });
            }
            else {
                MsgBox(e.message, MSG_ERROR);
            }
        })
        .error(function (e) {
            MsgBox('Terjadi Kesalahan, Hubungi SDMS Support', MSG_ERROR);
        });
    }

    me.initialize = function () {
        me.data = {};
        me.data.Lamp1 = '';
        me.data.Lamp2 = '';
        me.data.SignName = '';
        me.data.salesType = '1';
        me.isPrintAvailable = true;
    }


    me.start();
}


$(document).ready(function () {
    var options = {
        title: "Daftar Lampiran Dokumen Nonpenjualan",
        xtype: "panels",
        toolbars: [
            { name: "btnPrintPreview", text: "Print", cls: "btn btn-primary", icon: "icon-print", click: "printPreview()", disable: "!isPrintEnable" },
        ],
        panels: [
            {
                name: "pnlA",
                items: [
                      { name: "salesType", model: "data.salesType", text: "Tipe Penjualan", type: "select2", cls: "span4", datasource: 'comboDocOrigin' },
                      {
                          text: "No. Lampiran", type: "controls",
                          items: [
                                  { name: "Lamp1", model: "data.Lamp1", cls: "span1", placeHolder: "PickingList No", type: "popup", click: "DocumentLkp(1)", required: true },
                                  { type: "label", text: "S/D", cls: "span1 mylabel" },
                                  { name: "Lamp2", model: "data.Lamp2", cls: "span1", placeHolder: "PickingList No", type: "popup", click: "DocumentLkp(2)", required: true }
                          ]
                      },
                      { name: "SeqNo", text: "Penanda Tangan", model: "data.SignName", cls: "span3", placeHolder: "PickingList No", type: "popup", click: "SignatureLkp()", required: true }
                ]
            }
        ]
    };

    Wx = new SimDms.Widget(options);
    Wx.default = {};
    Wx.render(init);



    function init(s) {
        $(".mylabel").attr("style", "padding:9px 9px 0px 5px");
        SimDms.Angular("spLstLmprnDokNonPenjualan");
        $("#salesType option:first").remove();
    }



});