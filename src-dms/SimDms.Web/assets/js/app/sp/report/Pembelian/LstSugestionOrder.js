"use strict";
function spRptSuggestionOrder($scope, $http, $injector) {

    var me = $scope;
    $injector.invoke(BaseController, this, { $scope: me });

    me.SuggorLkp = function (x) {
        var lookup = Wx.blookup({
            name: "SparepartLookup",
            title: "Pencarian No. Suggor",
            manager: spManager,
            query: "SuggorLookup",            
            columns: [
                { field: "SuggorNo", title: "No. Suggor" },
                { field: "SuggorDate", title: "Tanggal Suggor" },
                { field: "SupplierCode", title: "Kode Suplier" }                
            ],
        });
        
        lookup.dblClick(function (data) {
            if (data != null) {
                if (x == 1) {
                    me.data.SuggorNo1 = data.SuggorNo;
                    me.data.SuggorNo2 = data.SuggorNo;
                }
                else {
                    me.data.SuggorNo2 = data.SuggorNo;
                }
                me.Apply();
            }
        });
    }


    me.printPreview = function () {


      
        var prm = [me.data.SuggorNo1,me.data.SuggorNo2,'typeofgoods'];
        Wx.showPdfReport({
            id: "SpRpTrn001",
            pparam: prm.join(','),            
            type: "devex"
        });
    }

    me.initialize = function () {
        me.data = {};
        me.data.SuggorNo1 = '';
        me.data.SuggorNo2 = '';

        $http.get('breeze/sparepart/CurrentUserInfo').
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
        title: "Sugestion Order",
        xtype: "panels" ,
        toolbars: [
            { name: "btnPrintPreview", text: "Print", cls: "btn btn-primary", icon: "icon-print", click: "printPreview()", disable: "!isPrintEnable" },
        ],
        panels: [
            {
                name: "pnlA",
                items: [
                        {
                            text: "No. Sugor", type: "controls",
                            items: [                                    
                                    { name: "SuggorNo1", model: "data.SuggorNo1", cls: "span1", placeHolder: "Suggor No", type: "popup", click: "SuggorLkp(1)" },
                                    { type: "label", text: "S/D", cls: "span1 mylabel" },
                                    { name: "SuggorNo2", model: "data.SuggorNo2", cls: "span1", placeHolder: "Suggor No", type: "popup", click: "SuggorLkp(2)" }
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
        SimDms.Angular("spRptSuggestionOrder");
    }



});