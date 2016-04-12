"use strict";
function spRptLstPickingList($scope, $http, $injector) {

    var me = $scope;
    $injector.invoke(BaseController, this, { $scope: me });

    me.SuggorLkp = function (x) {
        var lookup = Wx.blookup({
            name: "SparepartLookup",
            title: "Pencarian No. Picking List",
            manager: spManager,
            query: "PickingListHdrBrowse",
            defaultSort: "PickingSlipNo asc",
            columns: [
                { field: "PickingSlipNo", title: "No. Picking List" },
                { field: "PickingSlipDate", title: "Tanggal Picking Slip", template: "#= (PickingSlipDate == undefined) ? '' : moment(PickingSlipDate).format('DD MMM YYYY HH:mm:ss') #" },
                { field: "Remark", title: "Keterangan" },
                
            ],
        });
        
        lookup.dblClick(function (data) {
            if (data != null) {
                if (x == 1) {
                    me.data.PickingList1 = data.PickingSlipNo;
                    me.data.PickingList2 = data.PickingSlipNo;
                }
                else {
                    me.data.PickingList2 = data.PickingSlipNo;
                }
                me.Apply();
            }
        });
    }


    me.printPreview = function () {
        var prm = [me.data.PickingList1, me.data.PickingList2, '300',' typeofgoods'];
        Wx.showPdfReport({
            id: "SpRpTrn008",
            pparam: prm.join(','),
            type: "devex"
        });
    }

    me.initialize = function () {
        me.data = {};
        me.data.PickingList1 = '';
        me.data.PickingList2 = '';

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
        title: "Daftar Picking List",
        xtype: "panels",
        toolbars: [
            { name: "btnPrintPreview", text: "Print", cls: "btn btn-primary", icon: "icon-print", click: "printPreview()", disable: "!isPrintEnable" },
        ],
        panels: [
            {
                name: "pnlA",
                items: [
                    {
                        text: "No. Picking List", type: "controls",
                        items: [
                                { name: "PickingList1", model: "data.PickingList1", cls: "span1", placeHolder: "PickingList No", type: "popup", click: "SuggorLkp(1)" },
                                { type: "label", text: "S/D", cls: "span1 mylabel" },
                                { name: "PickingList2", model: "data.PickingList2", cls: "span1", placeHolder: "PickingList No", type: "popup", click: "SuggorLkp(2)" }
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
        SimDms.Angular("spRptLstPickingList");
    }



});