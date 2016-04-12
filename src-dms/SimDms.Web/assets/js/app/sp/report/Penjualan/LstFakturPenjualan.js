function spLstFakturPenjualan($scope, $http, $injector) {

    var me = $scope;
    $injector.invoke(BaseController, this, { $scope: me });
    
    me.SignatureLkp = function (x) {
        var lookup = Wx.blookup({
            name: "SparepartLookup",
            title: "Pencarian No. Dokumen",
            manager: spManager,
            query: new breeze.EntityQuery().from("SignatureLkp").withParameters({ doctype: 'SDH'}),
            defaultSort: "SignName asc",
            columns: [
                { field: "SignName", title: "Nama" },
                { field: "TitleSign", title: "Jabatan" }                
            ],
        });

        lookup.dblClick(function (data) {
            if (data != null) {                
                me.data.SignName=data.SignName;
                me.data.SeqNo=data.SeqNo;
                me.Apply();
            }
        });
    }

    me.FakturLkp = function (x) {
        var lookup = Wx.blookup({
            name: "SparepartLookup",
            title: "Pencarian No. Dokumen",
            manager: spManager,
            query: "FakturLkp",
            defaultSort: "FPJNo asc",
            columns: [
                { field: "FPJNo", title: "No. Faktur" },
                { field: "FPJDate", title: "Tgl Faktur", template: "#= (FPJDate == undefined) ? '' : moment(FPJDate).format('DD-MM-YYYY') #" },
                { field: "PickingSlipNo", title: "No. Pick. Slip" },
                { field: "PickingSlipDate", title: "Tgl Pick. Slip", template: "#= (PickingSlipDate == undefined) ? '' : moment(PickingSlipDate).format('DD-MM-YYYY') #" },
                { field: "InvoiceNo", title: "No. Invoice" },
                { field: "InvoiceDate", title: "Tgl Invoice", template: "#= (InvoiceDate == undefined) ? '' : moment(InvoiceDate).format('DD-MM-YYYY') #" },
            ],
        });

        lookup.dblClick(function (data) {
            if (data != null) {
                if (x == 1) {
                    me.data.FPJNo1 = data.FPJNo;
                    me.data.FPJNo2 = data.FPJNo;
                }
                else {
                    me.data.FPJNo2 = data.FPJNo;
                }
                me.Apply();
            }
        });
    }


    me.printPreview = function () {
        var prm=[me.data.FPJNo1,me.data.FPJNo2,"profitcenter",me.data.SeqNo,"typeofgoods"];
        Wx.showPdfReport({
            id: "SpRpTrn011LongV2",
            pparam: prm.join(','),            
            type: "devex"
        });
    }




    me.initialize = function () {
        me.data = {};
        me.data.DocNo1 = '';
        me.data.DocNo2 = '';
        me.data.salesType = '0';
        me.isPrintAvailable = true;
    }


    me.start();
}


$(document).ready(function () {
    var options = {
        title: "Daftar Faktur Penjualan",
        xtype: "panels",
        toolbars: [
            { name: "btnPrintPreview", text: "Print", cls: "btn btn-primary", icon: "icon-print", click: "printPreview()", disable: "!isPrintEnable" },
        ],
        panels: [
            {
                name: "pnlA",
                items: [                      
                      {
                          text: "No. Faktur", type: "controls",
                          items: [
                                  { name: "FPJNo1", model: "data.FPJNo1", cls: "span1", placeHolder: "PickingList No", type: "popup", click: "FakturLkp(1)" },
                                  { type: "label", text: "S/D", cls: "span1 mylabel" },
                                  { name: "FPJNo2", model: "data.FPJNo2", cls: "span1", placeHolder: "PickingList No", type: "popup", click: "FakturLkp(2)" }
                          ]
                      },
                      { name: "SeqNo", text: "Penanda Tangan", model: "data.SignName", cls: "span3", placeHolder: "PickingList No", type: "popup", click: "SignatureLkp()" }
                ]
            }
        ]
    };

    Wx = new SimDms.Widget(options);
    Wx.default = {};
    Wx.render(init);



    function init(s) {
        $(".mylabel").attr("style", "padding:9px 9px 0px 5px");
        SimDms.Angular("spLstFakturPenjualan");
    }



});