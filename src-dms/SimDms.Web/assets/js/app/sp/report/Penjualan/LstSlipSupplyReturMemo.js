"use strict";
function spLstSupplyReturMemo($scope, $http, $injector) {

    var me = $scope;
    $injector.invoke(BaseController, this, { $scope: me });

  

    me.$watch('data.salesType', function (newValue, oldValue) {
        me.data.ReturnNo1 = '';
        me.data.ReturnNo2 = '';
    }, true);


    me.ReturLkp = function (x) {        

        var lookup = Wx.blookup({
            name: "SparepartLookup",
            title: "Pencarian No. Retur",
            manager: spManager,
            query: "SpTrnSRturSSHdrLkp",
            defaultSort: "ReturnNo asc",
            columns: [
                   { field: "ReturnNo", title: "No. Retur" },
                   { field: "ReturnDate", title: "Tgl. Retur", template: "#= (LmpDate == undefined) ? '' : moment(LmpDate).format('DD MMM YYYY HH:mm:ss') #" }
            ],
        });

        lookup.dblClick(function (data) {
            if (data != null) {
                if (x == 1) {
                    me.data.ReturnNo1 = data.ReturnNo;
                    me.data.ReturnNo2 = data.ReturnNo;
                }
                else {
                    me.data.ReturnNo1 = data.ReturnNo;
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
        var prm = [me.data.ReturnNo1, me.data.ReturnNo2, "profitcenter", "typeofgoods"];
        var slstyp = '';

        Wx.showPdfReport({
            id: "SpRpTrn013",
            pparam: prm.join(','),
            type: "devex"
        });
    }


    me.initialize = function () {
        me.data = {};
        me.data.ReturnNo1 = '';
        me.data.ReturnNo2 = '';
        me.data.salesType = '1';
        me.isPrintAvailable = true;
    }

    me.start();
}


$(document).ready(function () {
    var options = {
        title: "Daftar Slip Supply Retur Memo",
        xtype: "panels",
        toolbars: [
            { name: "btnPrintPreview", text: "Print", cls: "btn btn-primary", icon: "icon-print", click: "printPreview()", disable: "!isPrintEnable" },
        ],
        panels: [
            {
                name: "pnlA",
                items: [
                    {
                        text: "No. Retur", type: "controls",
                        items: [
                                  { name: "ReturnNo1", model: "data.ReturnNo1", cls: "span1", placeHolder: "Retur No", type: "popup", click: "ReturLkp(1)" },
                                  { type: "label", text: "S/D", cls: "span1 mylabel" },
                                  { name: "ReturnNo2", model: "data.ReturnNo2", cls: "span1", placeHolder: "Retur No", type: "popup", click: "ReturLkp(2)" }
                        ]
                    }
                ]
            }
        ]
    };

    Wx = new SimDms.Widget(options);
    Wx.default = {};
    Wx.render(init);



    function init(s) {
        $(".mylabel").attr("style", "padding:9px 9px 0px 5px");
        SimDms.Angular("spLstSupplyReturMemo");
        $("#salesType option:first").remove();
    }



});