function svKsgSpkPrintController($scope, $http, $injector) {
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
                if (me.data.JobOrderNoEnd < data.GenerateNo) {
                    me.data.JobOrderNo = data.GenerateNo;
                    me.data.JobOrderNoEnd = data.GenerateNo;
                }
                else {
                    me.data.JobOrderNo = data.GenerateNo;
                }
            }
            else {
                me.data.JobOrderNo = data.GenerateNo;
            }
            me.Apply();
            me.isLoadData = true;
        });
    };
    
    me.lookupSpkEnd = function () {
        me.lookupSpk().dblClick(function (data) {
            if (me.data.JobOrderNo != JobOrderNoDefault) {
                if (me.data.JobOrderNo > data.GenerateNo) {
                    me.data.JobOrderNo = data.GenerateNo;
                    me.data.JobOrderNoEnd = data.GenerateNo;
                }
                else {
                    me.data.JobOrderNoEnd = data.GenerateNo;
                }
            }
            else {
                me.data.JobOrderNo = data.GenerateNo;
                me.data.JobOrderNoEnd = data.GenerateNo;
            }
            me.Apply();
        });
    };

    me.lookupSpk = function () {
        var param = (me.data.ServiceType == null) ? "" : "?ServiceType=" + me.data.ServiceType;
        var lookup = Wx.klookup({
            name: "lookupSpkBegin",
            title: "Lookup PDI/FSC",
            url: "sv.api/grid/AllBranchFromSPKKG",
            serverBinding: true,           
            pageSize: 10,
            columns: [
               { field: "BranchCode", title: "Branch Code", width: 110 },
                { field: "GenerateNo", title: "PDI FSC No.", width: 110 },
                {
                    field: "GenerateDate", title: "PDI FSC Date", width: 130,
                    template: "#= (GenerateDate == undefined) ? '' : moment(GenerateDate).format('DD MMM YYYY') #"
                },
                { field: "Invoice", title: "No Invoice",width:220 },
                { field: "FPJNo", title: "Tax Invoice No.", width: 110 },
                {
                    field: "FPJDate", title: "Tax Invoice Date", width: 130,
                    template: "#= (FPJDate == undefined) ? '' : moment(FPJDate).format('DD MMM YYYY') #"
                },
                { field: "FPJGovNo", title: "Tax Invoice GOV No."  ,width: 110 },
                { field: "TotalNoOfItem", title: "Total Record", width: 110 },
                { field: "TotalAmt", title: "Total PDI FSC", width: 110 },
                { field: "SenderDealerName", title: "Sender", width: 110 },
                { field: "RefferenceNo", title: "Refference No", width: 110 },
                {
                    field: "RefferenceDate", title: "Refference Date", width: 130,
                    template: "#= (RefferenceDate == undefined) ? '' : moment(RefferenceDate).format('DD MMM YYYY') #"
                },
                { field: "PostingFlagDesc", title: "Status" }
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
        var hst = window.location.origin;        
        hst += "/" + window.location.pathname.replace('/Form', '').replace('/layout', '').replace('/', '');
        var url = hst + "/sv.api/KsgSpk/GetKsgFromSPKXls?GenerateNoFrom=" + me.data.JobOrderNo + "&GenerateNoTo=" + me.data.JobOrderNoEnd + "&SourceData=0";        
        window.open(url);
    }
}

$(document).ready(function () {
    var options = {
        title: "Generate XLS",
        xtype: "panels",
        panels: [           
            {
                name: "pnlRefService",
                items: [
                    {
                        name: "JobOrderNo",
                        model: "data.JobOrderNo",
                        cls: "span4",
                        text: "No. PDI/FSC",
                        type: "popup",
                        btnName: "btnJobOrderNo",
                        readonly: true,
                        required: "required",
                        click: 'lookupSpkBegin()'
                    },{
                        name: "JobOrderNoEnd",
                        model: "data.JobOrderNoEnd",
                        text: "No. PDI/FSC",
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
        SimDms.Angular("svKsgSpkPrintController");
    }
});