function omTransferOutPrintController($scope, $http, $injector) {
    var me = $scope;
    $injector.invoke(BaseController, this, { $scope: me });

    me.initialize = function () {
        me.data.TransferOutNo = readCookie('TransferOutNo');
        me.data.OptionType = "0";
    };
    me.start();

    me.print = function () {
        var prm = [
                    me.data.TransferOutNo
        ];

        if (me.data.OptionType = "0") {
            Wx.showPdfReport({
                id: "OmRpInventTrn001",
                pparam: prm.join(','),
                rparam: "Print Transfer Out",
                type: "devex"
            });
        }
        if (me.data.OptionType = "1") {
            Wx.showPdfReport({
                id: "OmRpInventTrn001A",
                pparam: prm.join(','),
                rparam: "Print Transfer Out",
                type: "devex"
            });
        }
       
        eraseCookie('TransferOutNo');
    }
}

$(document).ready(function () {
    var options = {
        title: "Print Transfer Out",
        xtype: "panels",
        panels: [
            {
                name: "pnl1",
                items: [
                    {   name: "OptionType",
                        text: "Option Print",
                        type: "select",
                        cls: "span4",
                        items: [
                            { value: '0', text: 'Print satu halaman' },
                            { value: '1', text: 'Print setengah halaman' },
                        ]
                    },
                    { 
                        type: "buttons",
                        items: [
                            { name: "btnPrint", text: "Print", icon: "icon-print", click: "print()", cls: "span4" }
                        ]
                    }
                ]
            }
        ]
    }

    Wx = new SimDms.Widget(options);
    Wx.default = {};
    Wx.render(init);

    function init(s) {
        SimDms.Angular("omTransferOutPrintController");
    }
});