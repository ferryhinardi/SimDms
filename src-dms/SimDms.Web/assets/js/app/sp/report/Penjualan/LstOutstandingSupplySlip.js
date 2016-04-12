function spLstOutstandingSupplySlip($scope, $http, $injector) {

    var me = $scope;
    $injector.invoke(BaseController, this, { $scope: me });

    me.comboDocOrigin = [
                       { text: "Sales Retur Memo", value: '0' },
                       { text: "Nota Retur", value: '1' }
    ];

    me.$watch('data.salesType', function (newValue, oldValue) {
        me.data.UsageDocNo1 = '';
        me.data.UsageDocNo2 = '';
    }, true);


    me.SPKLkp = function (x) {
        var lookup = Wx.blookup({
            name: "SparepartLookup",
            title: "Pencarian No. Dokumen",
            manager: spManager,
            query: "SPkNoLstOutStandingLkp",
            defaultSort: "UsageDocNo asc",
            columns: [
                { field: "UsageDocNo", title: "No. Retur" },
                { field: "UsageDocDate", title: "Tgl. Retur", template: "#= (UsageDocDate == undefined) ? '' : moment(UsageDocDate).format('DD MMM YYYY HH:mm:ss') #" }
            ],
        });

        lookup.dblClick(function (data) {
            if (data != null) {
                if (x == 1) {
                    if (me.data.UsageDocNo2 < data.UsageDocNo || me.data.UsageDocNo2 == '') {
                        me.data.UsageDocNo1 = data.UsageDocNo;
                        me.data.UsageDocNo2 = data.UsageDocNo;
                    }
                    else {
                        me.data.UsageDocNo1 = data.UsageDocNo;
                    }
                }
                else {
                    if (me.data.UsageDocNo1 > data.UsageDocNo || me.data.UsageDocNo1 == '') {
                        me.data.UsageDocNo1 = data.UsageDocNo;
                        me.data.UsageDocNo2 = data.UsageDocNo;
                    }
                    else {
                        me.data.UsageDocNo2 = data.UsageDocNo;
                    }
                }
                me.Apply();
            }
        });
    }


    me.SignatureLkp = function (x) {
        var lookup = Wx.blookup({
            name: "SparepartLookup",
            title: "SPK Out Standing Search",
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
        var prm = [me.data.UsageDocNo1, me.data.UsageDocNo2,  "typeofgoods"];
        Wx.showPdfReport({
            id: "SpRpTrn014",
            pparam: prm.join(','),
            type: "devex"
        });
    }




    me.initialize = function () {
        me.data = {};
        me.data.UsageDocNo1 = '';
        me.data.UsageDocNo1 = '';
        me.data.salesType = '0';
        me.isPrintAvailable = true;
    }


    me.start();
}

$(document).ready(function () {
    var options = {
        title: "Daftar Outstanding Supply Slip",
        xtype: "panels",
        toolbars: [
            { name: "btnPrintPreview", text: "Print", cls: "btn btn-primary", icon: "icon-print", click: "printPreview()", disable: "!isPrintEnable" },
        ],
        panels: [
            {
                name: "pnlA",
                items: [
                        {
                            text: "No. SPK", type: "controls",
                            items: [
                                    { name: "UsageDocNo1", model: "data.UsageDocNo1", cls: "span1", placeHolder: "SPK No", type: "popup", click: "SPKLkp(1)" },
                                    { type: "label", text: "S/D", cls: "span1 mylabel" },
                                    { name: "UsageDocNo2", model: "data.UsageDocNo2", cls: "span1", placeHolder: "SPK No", type: "popup", click: "SPKLkp(2)" }
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
        $(".mylabel").attr("style", "padding:9px 9px 0px 5px");
        SimDms.Angular("spLstOutstandingSupplySlip");
        $("#salesType option:first").remove();
    }



});