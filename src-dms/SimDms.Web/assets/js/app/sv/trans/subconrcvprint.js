function svSubConRcvPrintController($scope, $http, $injector) {
    var me = $scope;
    $injector.invoke(BaseController, this, { $scope: me });
    var RecNoDefault = 'RRO/XX/YYYYYY';

    me.initialize = function () {
        var value = true;
        $("#IsHalfPageY").prop('checked', value).val(value);
        $("#IsHalfPageN").prop('checked', !value).val(value);

        me.data.RecNo = me.data.RecNoEnd = localStorage.getItem('params');
    };
    me.start();

    me.lookupSPKBegin = function () {
        me.lookupSPK().dblClick(function (data) {
            if (me.data.RecNoEnd != RecNoDefault) {
                if (me.data.RecNoEnd < data.RecNo) {
                    me.data.RecNo = data.RecNo;
                    me.data.RecNoEnd = data.RecNo;
                }
                else {
                    me.data.RecNo = data.RecNo;
                }
            }
            else {
                me.data.RecNo = data.RecNo;
            }
            me.Apply();
            me.isLoadData = true;
        });
    };
    
    me.lookupSPKEnd = function () {
        me.lookupSPK().dblClick(function (data) {
            if (me.data.RecNo != RecNoDefault) {
                if (me.data.RecNo > data.RecNo) {
                    me.data.RecNo = data.RecNo;
                    me.data.RecNoEnd = data.RecNo;
                }
                else {
                    me.data.RecNoEnd = data.RecNo;
                }
            }
            else {
                me.data.RecNo = data.RecNo;
                me.data.RecNoEnd = data.RecNo;
            }
            me.Apply();
        });
    };

    me.lookupSPK = function () {
        var lookup = Wx.klookup({
            name: "lookupSPKBegin",
            title: "Data Penerimaan Pekerjaan Luar",
            url: "sv.api/grid/subconrcvs",
            serverBinding: true,
            sort: [
                { 'field': 'RecNo', 'dir': 'desc' },
                { 'field': 'RecDate', 'dir': 'desc' }
            ],
            pageSize: 10,
            columns: [
                { field: "RecNo", title: "No. Receiving", width: 150 },
                {
                    field: "RecDate", title: "Nomor Faktur/Nota", width: 130,
                    template: "#= (RecDate == undefined) ? '' : moment(RecDate).format('DD MMM YYYY') #"
                },
                { field: "Description", title: "Status", width: 130 }
            ],
        });

        return lookup;
    };

    me.lookupSign = function () {
        var lookup = Wx.klookup({
            name: "lookupSign",
            title: "Penandatangan",
            url: "sv.api/grid/SignForSubConRcv",
            serverBinding: true,
            sort: [
                { 'field': 'SignName', 'dir': 'asc' },
                { 'field': 'TitleSign', 'dir': 'asc' }
            ],
            pageSize: 10,
            columns: [
                { field: "SignName", title: "Nama", width: 150 },
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
       if (me.data.RecNo === "" || me.data.RecNo == undefined ||
           me.data.RecNoEnd === "" || me.data.RecNoEnd == undefined || 
           me.data.SignName === "" || me.data.SignName == undefined) {
           MsgBox("Ada informasi yang belum lengkap", MSG_WARNING);
           return;
       }
       else{
            var prm = [
                me.UserInfo.ProductType,
                me.data.RecNo,
                me.data.RecNoEnd
            ];

            var rparam = [
               "Input Penerimaan",
                me.data.SignName,
                me.UserInfo.UserId
            ];

            var rprtid = ($("#IsHalfPageY").prop('checked') ? "SvRpTrn004002H" : "SvRpTrn004002");

            Wx.showPdfReport({
                id:rprtid,
                pparam: prm.join(','),
                textprint: true,
                rparam: rparam.join(','),
                type: 'devex'
            });
        }
    }
}

$(document).ready(function () {
    var options = {
        title: "Print Input Penerimaan Luar",
        xtype: "panels",
        panels: [
            {
                name: "pnlInput",
                cls: "span8",
                items: [
                    {
                        type: "controls",
                        cls: "span6",
                        text: "NO. Receiving",
                        validasi: "required",
                        required: true,
                        items: [
                            {
                                name: "RecNo",
                                model: "data.RecNo",
                                cls: "span3",
                                type: "popup",
                                btnName: "btnRecNo",
                                readonly: true,
                                validasi: "required",
                                required: true,
                                click: 'lookupSPKBegin()'
                            },
                            {
                                cls: "span1",
                                type: "label",
                                text: "s/d"
                            },
                            {
                                name: "RecNoEnd",
                                model: "data.RecNoEnd",
                                cls: "span3",
                                type: "popup",
                                readonly: true,
                                validasi: "required",
                                required: true,
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
                        validasi: "required",
                        required: true,
                        click: 'lookupSign()'
                    },
                    { name: "IsHalfPage", text: "Setengah Halaman", type: "switch", cls: "span3 full", float: "left" },
                    {
                        type: "buttons",
                        items: [
                            { name: "btnPrint", text: "Print", icon: "icon-print", cls: "btn span4", click: 'print()' },
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
        SimDms.Angular("svSubConRcvPrintController");
    }
});