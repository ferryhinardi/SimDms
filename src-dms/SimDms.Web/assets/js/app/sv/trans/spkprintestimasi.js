function svSpkPrintEstimasiController($scope, $http, $injector) {
    var me = $scope;
    $injector.invoke(BaseController, this, { $scope: me });
    var JobOrderNoDefault = 'EST/XX/YYYYYY';

    me.initialize = function () {
        me.data.ServiceType = readCookie('ServiceType');
        me.data.JobOrderNo = localStorage.getItem('params'); //(readCookie('JobOrderNo') == "" || readCookie('JobOrderNo') == undefined) ? JobOrderNoDefault : readCookie('JobOrderNo');
        me.data.JobOrderNoEnd = me.data.JobOrderNo;
    };
    me.start();

    me.lookupSpkBegin = function () {
        me.lookupSpk().dblClick(function (data) {
            if (me.data.JobOrderNoEnd != JobOrderNoDefault) {
                if (me.data.JobOrderNoEnd < data.JobOrderNo) {
                    me.data.JobOrderNo = data.JobOrderNo;
                    me.data.JobOrderNoEnd = data.JobOrderNo;
                }
                else {
                    me.data.JobOrderNo = data.JobOrderNo;
                }
            }
            else {
                me.data.JobOrderNo = data.JobOrderNo;
            }
            me.Apply();
            me.isLoadData = true;
        });
    };
    
    me.lookupSpkEnd = function () {
        me.lookupSpk().dblClick(function (data) {
            if (me.data.JobOrderNo != JobOrderNoDefault) {
                if (me.data.JobOrderNo > data.JobOrderNo) {
                    me.data.JobOrderNo = data.JobOrderNo;
                    me.data.JobOrderNoEnd = data.JobOrderNo;
                }
                else {
                    me.data.JobOrderNoEnd = data.JobOrderNo;
                }
            }
            else {
                me.data.JobOrderNo = data.JobOrderNo;
                me.data.JobOrderNoEnd = data.JobOrderNo;
            }
            me.Apply();
        });
    };

    me.lookupSpk = function () {
        var param = (me.data.ServiceType == null) ? "" : "?ServiceType=" + me.data.ServiceType;
        var lookup = Wx.klookup({
            name: "lookupSpkBegin",
            title: "Lookup SPK",
            url: "sv.api/grid/JobOrderLookup" + param,
            serverBinding: true,
            sort: [
                { 'field': 'JobOrderNo', 'dir': 'desc' },
                { 'field': 'JobOrderDate', 'dir': 'desc' }
            ],
            pageSize: 10,
            columns: [
                { field: "JobOrderNo", title: "No SPK", width: 150 },
                {
                    field: "JobOrderDate", title: "SPK Date", width: 130,
                    template: "#= (JobOrderDate == undefined) ? '' : moment(JobOrderDate).format('DD MMM YYYY') #"
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

    me.lookupSign = function () {
        var lookup = Wx.klookup({
            name: "lookupSign",
            title: "Penandatangan",
            url: "sv.api/grid/SignForSubCon",
            serverBinding: true,
            sort: [
                { 'field': 'SignName', 'dir': 'asc' },
                { 'field': 'TitleSign', 'dir': 'asc' }
            ],
            pageSize: 10,
            columns: [
                { field: "SignName", title: "Nama", width: 120 },
                { field: "TitleSign", title: "Jabatan", width: 130 }
            ],
        });

        lookup.dblClick(function (data) {
            if (data != undefined) {
                me.data.SignName = data.SignName;
                me.data.TitleSign = data.TitleSign;
            }
            else {
                me.data.SignName = "";
            }
            me.Apply();
        });
    };

    me.print = function () {
        if (me.data.JobOrderNo == JobOrderNoDefault) {
            MsgBox("Ada informasi yang belum lengkap", MSG_WARNING);
            return;
        }

        var prm = [
            me.UserInfo.ProductType,
            me.data.JobOrderNo,
            me.data.JobOrderNoEnd
        ];

        Wx.showPdfReport({
            id: 'SvRpTrn00101',
            pparam: prm.join(','),
            textprint: true,
            rparam: me.data.SignName + ',' + me.data.TitleSign.replace('&', '%26'),
            type: 'devex'
        });

        var param = {
            JobOrderNo: me.data.JobOrderNo,
            JobOrderNoEnd: me.data.JobOrderNoEnd
        };

        //$http.post('sv.api/spk/UpdateSPKPrintSeq', param);
        eraseCookie('ServiceType');
        eraseCookie('JobOrderNo');
    }
}

$(document).ready(function () {
    var options = {
        title: "Perkiraan Biaya",
        xtype: "panels",
        panels: [
            {
                name: "pnlRefService",
                items: [
                    {
                        name: "JobOrderNo",
                        model: "data.JobOrderNo",
                        cls: "span4",
                        text: "No. Estimasi dari",
                        type: "popup",
                        btnName: "btnJobOrderNo",
                        disable: true,
                        readonly: true,
                        required: "required",
                        click: 'lookupSpkBegin()'
                    },{
                        name: "JobOrderNoEnd",
                        model: "data.JobOrderNoEnd",
                        text: "No. SPK End",
                        cls: "span4",
                        type: "popup",
                        btnName: "btnJobOrderNoEnd",
                        readonly: true,
                        disable: true,
                        required: "required",
                        click: 'lookupSpkEnd()'
                    },{
                        name: "Sign",
                        model: "data.SignName",
                        cls: "span5",
                        text: "Penanda Tangan",
                        type: "popup",
                        readonly: true,
                        validasi: "required",
                        required: true,
                        click: 'lookupSign()'
                    }, {
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
        SimDms.Angular("svSpkPrintEstimasiController");
    }
});