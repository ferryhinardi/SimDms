function svSubConRvcPrintController($scope, $http, $injector) {
    var me = $scope;
    $injector.invoke(BaseController, this, { $scope: me });

    me.initialize = function () {
    };
    me.start();

    me.lookupSPKBegin = function () {
        me.lookupSPK().dblClick(function (data) {
            if (me.data.PONoEnd != PONoDefault) {
                if (me.data.PONoEnd < data.PONo) {
                    me.data.PONo = data.PONo;
                    me.data.PONoEnd = data.PONo;
                }
                else {
                    me.data.PONo = data.PONo;
                }
            }
            else {
                me.data.PONo = data.PONo;
            }
            me.Apply();
            me.isLoadData = true;
        });
    };
    
    me.lookupSPKEnd = function () {
        me.lookupSPK().dblClick(function (data) {
            if (me.data.PONo != PONoDefault) {
                if (me.data.PONo > data.PONo) {
                    me.data.PONo = data.PONo;
                    me.data.PONoEnd = data.PONo;
                }
                else {
                    me.data.PONoEnd = data.PONo;
                }
            }
            else {
                me.data.PONo = data.PONo;
                me.data.PONoEnd = data.PONo;
            }
            me.Apply();
        });
    };

    me.lookupSPK = function () {
        //var param = (me.data.ServiceType == null) ? "" : "?ServiceType=" + me.data.ServiceType;
        var lookup = Wx.klookup({
            name: "lookupSPKBegin",
            title: "Data Penerimaan Pekerjaan Luar",
            url: "sv.api/subcon/getpekerjaan" + param,
            serverBinding: true,
            sort: [
                { 'field': 'PONo', 'dir': 'desc' },
                { 'field': 'PODate', 'dir': 'desc' }
            ],
            pageSize: 10,
            columns: [
                { field: "PONo", title: "No SPK", width: 150 },
                {
                    field: "PODate", title: "SPK Date", width: 130,
                    template: "#= (PODate == undefined) ? '' : moment(PODate).format('DD MMM YYYY') #"
                },
                { field: "PoliceRegNo", title: "Police Reg. No.", width: 130 },
                { field: "ServiceBookNo", title: "Service Book No.", width: 130 },
                { field: "ChassisCode", title: "Chassis Code", width: 110 },
                { field: "ChassisNo", title: "Chassis No", width: 150 },
                { field: "BasicModel", title: "Basic Model", width: 150 },
                { field: "TransmissionType", title: "Transmission Type", width: 130 },
                { field: "EngineCode", title: "Engine Code", width: 110 },
                { field: "ColorCode", title: "Color Code", width: 100 },
                { field: "Customer", title: "Customer", width: 250 }
            ],
        });

        return lookup;
    };

    me.print = function () {
        if (me.data.PONo == PONoDefault)
            return;

        var prm = [
            me.UserInfo.ProductType,
            me.data.PONo,
            me.data.PONoEnd,
            '0'
        ];

        Wx.showPdfReport({
            id: 'SvRpTrn001',
            pparam: prm.join(','),
            rparam: 'Print Surat Perintah Kerja',
            type: 'devex'
        });

        var param = {
            PONo: me.data.PONo,
            PONoEnd: me.data.PONoEnd
        };

        $http.post('sv.api/spk/UpdateSPKPrintSeq', param);
        eraseCookie('ServiceType');
        eraseCookie('PONo');
    }
}

$(document).ready(function () {
    var options = {
        title: "Print Input Pesanan Luar",
        xtype: "panels",
        panels: [
            {
                name: "pnlInput",
                cls: "span8",
                items: [
                    {
                        type: "controls",
                        cls: "span6",
                        text: "NO. PO",
                        items: [
                            {
                                name: "PONo",
                                model: "data.PONo",
                                cls: "span3",
                                type: "popup",
                                btnName: "btnPONo",
                                readonly: true,
                                required: "required",
                                click: 'lookupSPKBegin()'
                            },
                            {
                                cls: "span1",
                                type: "label",
                                text: "s/d"
                            },
                            {
                                name: "PONoEnd",
                                model: "data.PONoEnd",
                                cls: "span3",
                                type: "popup",
                                readonly: true,
                                required: "required",
                                click: 'lookupSPKEnd()'
                            }
                        ]
                    },
                    {
                        name: "Sign",
                        model: "data.SignName",
                        cls: "span5",
                        text: "Penanda Tangan",
                        type: "popup",
                        readonly: true,
                        required: "required",
                        click: 'lookupSPKEnd()'
                    },
                    {
                        type: "buttons",
                        items: [
                            { name: "btnPrint", text: "Print", icon: "icon-print", cls: "span4", click: 'print()' },
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
        SimDms.Angular("svSubConRvcPrintController");
    }
});