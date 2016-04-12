"use strict";

function spLstPengeluaranStock($scope, $http, $injector) {

    var me = $scope;
    $injector.invoke(BaseController, this, { $scope: me });

    me.comboDocOrigin = [
                       { text: "Penjualan", value: 0 },
                       { text: "Non Penjualan (BPS)", value: 1 },
                       { text: "Service", value: 2 },
                       { text: "Sales", value: 3 },
    ];

    me.$watch('data.salesType', function (newValue, oldValue) {
        me.data.DocNo1 = '';
        me.data.DocNo2 = '';
    }, true);


    me.DocumentLkp = function (x) {
        var lookup = Wx.blookup({
            name: "SparepartLookup",
            title: "Pencarian No. Dokumen",
            manager: spManager,
            query: new breeze.EntityQuery().from("SORDHdrLkp").withParameters({ salestype: me.data.salesType }),
            defaultSort: "DocNo asc",
            columns: [
                { field: "DocNo", title: "No. Dokumen" },
                { field: "DocDate", title: "Tanggal Dokumen", template: "#= (DocDate == undefined) ? '' : moment(DocDate).format('DD MMM YYYY HH:mm:ss') #" }
            ],
        });

        lookup.dblClick(function (data) {
            if (data != null) {
                if (x == 1) {
                    me.data.DocNo1 = data.DocNo;
                    me.data.DocNo2 = data.DocNo;
                }
                else {
                    me.data.DocNo2 = data.DocNo;
                }
                me.Apply();
            }
        });
    }


    me.printPreview = function () {
        var rprtid = "";
        var prm = [];
        var rprm = "";                
        var slstyp = '';
        slstyp = me.data.salesType;        
        switch (slstyp)
        {
            case '0':                
                rprtid = "SpRpTrn031";
                prm = [me.data.DocNo1, me.data.DocNo1, "typeofgoods"];                
                break;
            case '1':                
                rprtid = "SpRpTrn009";
                prm = [me.data.DocNo1, me.data.DocNo2, "profitcenter", me.data.salesType, "typeofgoods"];
                rprm = "SPAREPART";                
                break;
            case '2':
                rprtid = "SpRpTrn009";
                prm = [me.data.DocNo1, me.data.DocNo2, "profitcenter", me.data.salesType, "typeofgoods"];
                rprm = "SERVICE";                
                break;
            case '3':
                rprtid = "SpRpTrn009";
                prm = [me.data.DocNo1, me.data.DocNo2, "profitcenter", me.data.salesType, "typeofgoods"];
                rprm = "UNIT ORDER";                
                break;
            default:
                MsgBox('Unknownsxs Sales Type', MSG_ERROR);
                break;
        }
        
        Wx.showPdfReport({
            id: rprtid,
            pparam: prm.join(','),
            rparam:rprm,
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
        title: "Daftar pengeluaran Stock",
        xtype: "panels",
        toolbars: [
            { name: "btnPrintPreview", text: "Print", cls: "btn btn-primary", icon: "icon-print", click: "printPreview()", disable: "!isPrintEnable" },
        ],
        panels: [
            {
                name: "pnlA",
                items: [
                      { name: "salesType", model: "data.salesType", text: "Asal Dokumen", type: "select2", cls: "span4", datasource: 'comboDocOrigin' },
                      {
                          text: "No. Picking List", type: "controls",
                          items: [
                                  { name: "DocNo1", model: "data.DocNo1", cls: "span1", placeHolder: "PickingList No", type: "popup", click: "DocumentLkp(1)" },
                                  { type: "label", text: "S/D", cls: "span1 mylabel" },
                                  { name: "DocNo2", model: "data.DocNo2", cls: "span1", placeHolder: "PickingList No", type: "popup", click: "DocumentLkp(2)" }
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
        SimDms.Angular("spLstPengeluaranStock");
        $("#salesType option:first").remove();
    }




});