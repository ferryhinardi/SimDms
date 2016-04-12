function svSubConPrintController($scope, $http, $injector) {
    var me = $scope;
    $injector.invoke(BaseController, this, { $scope: me });
    var PONoDefault = 'SPK/XX/YYYYYY';

    me.initialize = function () {

        var value = true;
        $("#IsHalfPageY").prop('checked', value).val(value);
        $("#IsHalfPageN").prop('checked', !value).val(value);

        me.data.PONo =me.data.PONoEnd= localStorage.getItem('params');
        
    };
    me.start();

    me.lookupPOBegin = function () {
        me.lookupPO().dblClick(function (data) {
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
    
    me.lookupPOEnd = function () {
        me.lookupPO().dblClick(function (data) {
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

    me.lookupPO = function () {
        var lookup = Wx.klookup({
            name: "lookupPOBegin",
            title: "Data Penerimaan Pekerjaan Luar",
            url: "sv.api/subcon/LookupJobOrder",
            serverBinding: true,
            sort: [
                { 'field': 'PONo', 'dir': 'desc' },
                { 'field': 'PODate', 'dir': 'desc' }
            ],
            pageSize: 10,
            columns: [
                { field: "PONo", title: "PO No.", width: 150 },
                {
                    field: "PODate", title: "PO Date", width: 130,
                    template: "#= (PODate == undefined) ? '' : moment(PODate).format('DD MMM YYYY') #"
                },
                { field: "JobOrderNo", title: "SPK No.", width: 130 },
                {
                    field: "JobOrderDate", title: "SPK Date", width: 130,
                    template: "#= (JobOrderDate == undefined) ? '' : moment(JobOrderDate).format('DD MMM YYYY') #"
                },
                { field: "SupplierName", title: "Pemasok", width: 280 },
                { field: "Description", title: "Status", width: 150 }
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
            }
            else {
                me.data.SignName = "";
            }
            me.Apply();
        });
    }

    me.print = function () {
        if (me.data.PONo === "" || me.data.PONo == undefined ||
            me.data.PONoEnd === "" || me.data.PONoEnd == undefined ||
            me.data.SignName === "" || me.data.SignName == undefined) {
            MsgBox("Ada informasi yang belum lengkap", MSG_WARNING);
            return;
        }
        else {
            var prm = [
                me.UserInfo.ProductType,
                me.data.PONo,
                me.data.PONoEnd
            ];

            var rparam = [
               "Input Pesanan",
                me.data.SignName,
                me.UserInfo.UserId
            ];

            var rbHalfPage = $("#IsHalfPageY").prop('checked');
          
            var rprtid = (rbHalfPage) ? "SvRpTrn004001H" : "SvRpTrn004001";;
            
            Wx.showPdfReport({
                id: rprtid,
                pparam: prm.join(','),
                rparam: rparam.join(','),
                textprint: true,
                type: 'devex'
            });
        }
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
                        required: true,
                        items: [
                            {
                                name: "PONo",
                                model: "data.PONo",
                                cls: "span3",
                                type: "popup",
                                btnName: "btnPONo",
                                readonly: true,
                                validasi: "required",
                                required: true,
                                click: 'lookupPOBegin()'
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
                                validasi: "required",
                                required: true,
                                click: 'lookupPOEnd()'
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
                        validasi: "required",
                        required: true,
                        click: 'lookupSign()'
                    },
                    { name: "IsHalfPage", model: "data.IsHalfPage", text: "Setengah Halaman", type: "switch", cls: "span3 full", float: "left" },                     
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
        SimDms.Angular("svSubConPrintController");
    }
});