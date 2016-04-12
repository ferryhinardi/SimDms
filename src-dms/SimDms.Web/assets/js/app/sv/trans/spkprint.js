function svSpkPrintController($scope, $http, $injector) {
    var me = $scope;
    $injector.invoke(BaseController, this, { $scope: me });
    var JobOrderNoDefault = 'SPK/XX/YYYYYY';

    me.initialize = function () {
        me.data.ServiceType = readCookie('ServiceType');
        me.data.JobOrderNo = localStorage.getItem('params'); //(readCookie('JobOrderNo') == "" || readCookie('JobOrderNo') == undefined) ? JobOrderNoDefault : readCookie('JobOrderNo');
        me.data.JobOrderNoEnd = me.data.JobOrderNo;
        me.optionsPrint = "Printed";
        me.data.Printed = true;
        me.data.PrePrinted = false;
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

    me.SetPrinted = function (val) {
        if (val == 1) {
            me.data.Printed = true;
            me.data.PrePrinted = false;
        }
        else {
            me.data.Printed = false;
            me.data.PrePrinted = true;
        }
    };

    me.print = function () {
        if (me.data.JobOrderNo == JobOrderNoDefault) {
            MsgBox("Ada informasi yang belum lengkap", MSG_WARNING);
            return;
        }

        var prm = [
            me.UserInfo.ProductType,
            me.data.JobOrderNo,
            me.data.JobOrderNoEnd,
            '0'
        ];

        Wx.showPdfReport({
            id: me.data.Printed == true ? 'SvRpTrn001' : 'SvRpTrn001PrePrinted',
            textprint:true,
            pparam: prm.join(','),
            rparam: 'Print Surat Perintah Kerja',
            type: 'devex'
        });

        var param = {
            JobOrderNo: me.data.JobOrderNo,
            JobOrderNoEnd: me.data.JobOrderNoEnd
        };

        $http.post('sv.api/spk/UpdateSPKPrintSeq', param);
        eraseCookie('ServiceType');
        eraseCookie('JobOrderNo');
    }
}

$(document).ready(function () {
    var options = {
        title: "Print Surat Perintah Kerja",
        xtype: "panels",
        panels: [
            {
                name: "pnloptions",
                items: [
                    {
                        type: "optionbuttons",
                        name: "optionsPrint",
                        model: "optionsPrint",
                        items: [
                            { name: "Printed", model: "data.Printed", text: "Printed", click: "SetPrinted(1)" },
                            { name: "PrePrinted", model: "data.PrePrinted", text: "Pre Printed", click: "SetPrinted(2)" },
                        ]
                    }
                ]
            },
            {
                name: "pnlRefService",
                items: [
                    {
                        name: "JobOrderNo",
                        model: "data.JobOrderNo",
                        cls: "span4",
                        text: "No. SPK Begin",
                        type: "popup",
                        btnName: "btnJobOrderNo",
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
                        required: "required",
                        click: 'lookupSpkEnd()'
                    },{
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
        SimDms.Angular("svSpkPrintController");
    }
});