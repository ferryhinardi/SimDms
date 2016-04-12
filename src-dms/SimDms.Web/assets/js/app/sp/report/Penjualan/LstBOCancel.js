"use strict";
function LstBOCancel($scope, $http, $injector) {

    var me = $scope;

    $injector.invoke(BaseController, this, { $scope: me });


    me.BOCancelBrowse = function (x) {
        var lookup = Wx.blookup({
            name: "DocNo",
            title: "Document No Lookup",
            manager: spReportPenjualanManager,
            query: "BOCancelBrowse",
            defaultSort: "DocNo asc",
            columns: [
                { field: "DocNo", title: "No. Dokumen" },
                { field: "DocDate", title: "Tgl. Dokumen" }
            ],
        });

        // fungsi untuk menghandle double click event pada k-grid / select button
        lookup.dblClick(function (data) {
            if (data != null) {
                if (x == 1) {
                    me.data.DocNo = data.DocNo;
                    me.data.DocNo1 = data.DocNo;
                }
                else {
                    me.data.DocNo1 = data.DocNo;
                }
                me.Apply();
            }
        });
    }


    me.printPreview = function () {
        var data = me.data.DocNo + "," + me.data.DocNo1;

        Wx.showPdfReport({
            id: "SpRpTrn030",
            pparam: data,
            rparam: "",
            type: "devex"
        });

    }

    me.initialize = function () {
        me.data = {};

        me.isPrintAvailable = true;

    }


    me.start();

}       
$(document).ready(function () {
    var options = {
        title: "Back Order Cancel List",
        xtype: "panels",
        toolbars: [
            { name: "btnPrintPreview", text: "Print", cls: "btn btn-primary", icon: "icon-print", click: "printPreview()", disable: "data.DocNo == undefined" },
        ],
        panels: [
            {
                name: "pnlA",
                items: [
                    {
                        name: "DocNo",
                        text: "No. Reserved",
                        cls: "span4",
                        placeHolder: "No. Reserved",
                        readonly: true,
                        type: "popup",
                        click: "BOCancelBrowse(1)"
                    },
                    {
                        name: "DocNo1",
                        text: "S/D",
                        cls: "span4",
                        placeHolder: "No. Reserved",
                        readonly: true,
                        disable: "data.DocNo == undefined",
                        type: "popup",
                        click: "BOCancelBrowse(2)"
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
        SimDms.Angular("LstBOCancel");

    }



});