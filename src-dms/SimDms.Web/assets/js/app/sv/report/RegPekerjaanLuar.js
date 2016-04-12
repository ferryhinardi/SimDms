"use strict"

function svRegPekerjaanLuarController($scope, $http, $injector)
{
    var me = $scope;

    $injector.invoke(BaseController, this, { $scope: me });

    me.dsJobType = [
        { value: "0", text: "Pesanan" },
        { value: "1", text: "Penerimaan" }
    ];
    
    $http.post('sv.api/combo/ServiceRefference?RefType=PORRSTAT').
    success(function (data, status, headers, config) {
        me.dsReffType = data;
        me.init();
    });

    $('#TaskY, #TaskN').on('change', function () {
        if ($('#btnSupplierCode').attr('disabled') == 'disabled') {
            $('#btnSupplierCode').removeAttr('disabled');
        } else {
            me.data.SupplierCode = "";
            me.data.SupplierName = "";
            me.Apply();

            $('#btnSupplierCode').attr('disabled', 'disabled');
        }
    });

    me.printPreview = function () {
        var valid = $(".main form").valid();
        if (!valid) return;

        var reportID = (me.data.JobType == 0) ? "SvRpReport022" : "SvRpReport023";
        var JobType = me.data.JobType;
        var status = (me.data.Status == undefined) ? '%' : me.data.Status
        var dateFrom = moment(me.data.DateFrom).format("YYYY-MM-DD");
        var dateTo = moment(me.data.DateTo).format("YYYY-MM-DD");
        var supplierCode = (me.data.SupplierCode == undefined) ? '' : me.data.SupplierCode;

        var param = "producttype," + status + "," + dateFrom + "," + dateTo + "," + supplierCode;
        var rparam = [
            'PERIODE : ' + dateFrom + ' s/d ' + dateTo,
            me.UserInfo.UserId
        ];

        Wx.showPdfReport({
            id: reportID,
            pparam: param,
            rparam: rparam,
            type: "devex"
        });
    }

    me.initialize = function () {
        $('#btnSupplierCode').attr('disabled', 'disabled');
        var ym = me.now("YYYY-MM") + "-01";
        me.data.DateFrom = moment(ym);
        me.data.DateTo = moment(ym).add("months", 1).add("days", -1);

        //var date1 = me.now();
        //var date2 = me.now();//new Date(date1.getFullYear(), date1.getMonth(), 1);
        //me.data.DateFrom = date2;
        //me.data.DateTo = date1;
    }

    me.LookupSupplier = function () {
        var lookup = Wx.klookup({
            name: "SupplierList",
            title: "Supplier List",
            url: "sv.api/lookup/SupplierRegPekerjaanLuar",
            params: {
                dateFrom: moment(me.data.DateFrom).format("YYYY-MM-DD"),
                dateTo: moment(me.data.DateTo).format("YYYY-MM-DD")
            },
            serverBinding: true,
            pageSize: 10,
            columns: [
                { field: "SupplierCode", title: "SupplierCode", width: 140 },
                { field: "SupplierName", title: "SupplierName", width: 200 }
            ],
        });

        lookup.dblClick(assignPart);
    }

    function assignPart(data) {
        me.data.SupplierCode = data.SupplierCode;
        me.data.SupplierName = data.SupplierName;
        me.Apply();
    }

    me.start();
}

$(document).ready(function () {
    var options = {
        title: "Register Pekerjaan Luar",
        xtype: "panels",
        toolbars: [
            { name: "btnPrintPreview", text: "Print", cls: "btn btn-primary", icon: "icon-print", click: "printPreview()" },
        ],
        panels: [
            {
                title: "Register Pekerjaan Luar",
                name: "summaryspk",
                items: [
                    {
                        name: "JobType", model: "data.JobType", required: true, cls: "span4 full", text: "Option Pekerjaan Luar", type: "select2", datasource: 'dsJobType'
                    },
                    { name: "DateFrom", text: "Periode", cls: "span4", type: "ng-datepicker", required: true, model: "data.DateFrom" },
                    { name: "DateTo", text: "s/d", cls: "span4", type: "ng-datepicker", required: true, model: "data.DateTo" },
                    {
                        text: "Supplier",
                        type: "controls",
                        items: [
                            { name: "Task", cls: "span2", type: "switch" }
                        ]
                    },
                    {
                        text: "Supplier",
                        type: "controls",
                        items: [
                            { text: "", name: "SupplierCode", model: "data.SupplierCode", type: "popup", display: "SupplierName", click: "LookupSupplier()", cls: "span2" },
                            { text: "", name: "SupplierName", model: "data.SupplierName", display: "SupplierName", cls: "span4" },
                        ]
                    },
                    { name: "Status", model: "data.Status", required: true, cls: "span4 full", text: "Status", type: "select2", datasource: 'dsReffType' },
                ],
            },
        ],
    };

    Wx = new SimDms.Widget(options);
    Wx.default = {};
    Wx.render(init);

    function init(s) {
        SimDms.Angular("svRegPekerjaanLuarController");
    }
});