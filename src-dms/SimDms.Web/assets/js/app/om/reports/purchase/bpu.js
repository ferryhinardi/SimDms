"use strict"; //Reportid OmRpMst001
function spRptMstSales($scope, $http, $injector) {

    var me = $scope;

    $injector.invoke(BaseController, this, { $scope: me });

    me.Status = [
        { "value": '-1', "text": 'ALL' },
        { "value": '0', "text": 'OPEN' },
        { "value": '1', "text": 'PRINTED' },
        { "value": '2', "text": 'APPROVED' },
        { "value": '3', "text": 'CANCELED' },
        { "value": '9', "text": 'FINISHED' },
    ];

    me.BPUNoFrom = function () {
        var lookup = Wx.blookup({
            name: "BPUNoLookup",
            title: "No. BPU",
            manager: spSalesManager,
            query: new breeze.EntityQuery.from("BPU4Lookup").withParameters({ Status: me.data.Status }),
            defaultSort: "BPUNo asc",
            columns: [
                { field: "BPUNo", title: "No. BPU" },
                { field: "BPUDate", title: "Tgl. BPU", template: "#= (BPUDate == undefined) ? '' : moment(BPUDate).format('DD MMM YYYY') #" },
                 { field: "Status", title: "Status" },
            ],
        });
        lookup.dblClick(function (data) {
            if (data != null) {
                me.data.BPUNoFrom = data.BPUNo;
                me.data.BPUDateFrom = data.BPUDate;
                me.Apply();
            }
        });

    }

    me.BPUNoTo = function () {
        var lookup = Wx.blookup({
            name: "NoBPULookup",
            title: "No. BPU",
            manager: spSalesManager,
            query: new breeze.EntityQuery.from("BPU4Lookup").withParameters({ Status: me.data.Status }),
            defaultSort: "BPUNo asc",
            columns: [
                 { field: "BPUNo", title: "No. BPU" },
                { field: "BPUDate", title: "Tgl. BPU", template: "#= (BPUDate == undefined) ? '' : moment(BPUDate).format('DD MMM YYYY') #" },
                 { field: "Status", title: "Status" },
            ],
        });
        lookup.dblClick(function (data) {
            if (data != null) {
                me.data.BPUNoTo = data.BPUNo;
                me.data.BPUDateTo = data.BPUDate;
                me.Apply();
            }
        });

    }

    me.printPreview = function () {
        if (me.data.BPUNoFrom || me.data.BPUNoTo) {
            BootstrapDialog.show({
                message: $(
                    '<div class="container">' +
                    '<div class="row">' +

                    '<input type="radio" name="sizeType" id="sizeType1" value="full" checked>&nbsp Print Satu Halaman</div>' +

                    '<div class="row">' +

                    '<input type="radio" name="sizeType" id="sizeType2" value="half">&nbsp Print Setengah Halaman</div>'),
                closable: false,
                draggable: true,
                type: BootstrapDialog.TYPE_INFO,
                title: 'Print',
                buttons: [{
                    label: ' Print',
                    cssClass: 'btn-primary icon-print',
                    action: function (dialogRef) {
                        me.Print();
                        dialogRef.close();
                    }
                }, {
                    label: ' Cancel',
                    cssClass: 'btn-warning icon-remove',
                    action: function (dialogRef) {
                        dialogRef.close();
                    }
                }]
            });
        } else {
            MsgBox("No. BPU Awal/ Akhir Harus diisi", MSG_ERROR);
        }
    }

    me.Print = function () {
        //alert(me.data.ProfitCenterCode);
        if (me.data.Status == '-1') {
            me.data.Status = '';
        }
        var sizeType = $('input[name=sizeType]:checked').val() === 'full';
        var ReportId = sizeType ? 'OmRpPurTRN002' : 'OmRpPurTRN002A';
        var prm = [
                   // me.data.CompanyCode,
                    me.data.BPUNoFrom,
                    me.data.BPUNoTo,
                    me.data.ProfitCenterCode,
                    me.data.Status
        ];
        Wx.showPdfReport({
            id: ReportId,//"OmRpPurTRN001",
            pparam: prm.join(','),
            rparam: "semua",
            type: "devex"
        });
    }

    me.initialize = function () {
        me.data = {};
        me.change = false;
        me.data.Status = '-1';
        $http.get('breeze/sales/CurrentUserInfo').
          success(function (dl, status, headers, config) {
              me.data.CompanyCode = dl.CompanyCode;
              me.data.BranchCode = dl.BranchCode;

          });

        $http.get('breeze/sales/ProfitCenter').
        success(function (dl, status, headers, config) {
            me.data.ProfitCenterCode = dl.ProfitCenter;
        });
        me.data.BPUDateFrom = me.now();
        me.data.BPUDateTo = me.now();
        $('#BPUDateFrom').attr('disabled', true);
        $('#BPUDateTo').attr('disabled', true);
        me.isPrintAvailable = true;
    }
    
    me.start();

}


$(document).ready(function () {
    var options = {
        title: "Report BPU",
        xtype: "panels",
        toolbars: [
            { name: "btnPrintPreview", text: "Print", cls: "btn btn-primary", icon: "icon-print", click: "printPreview()", disable: "!isPrintEnable" },
            { name: "btnCancel", text: "Cancel", cls: "btn btn-warning", icon: "icon-remove", click: "cancelOrClose()" },
        ],
        panels: [
            {
                name: "pnlA",
                items: [
                        { name: "CompanyCode", model: "data.CompanyCode", text: "Kode Perusahaan", cls: "span4 full", disable: "isPrintAvailable" },
                        { name: "BranchCode", model: "data.BranchCode", text: "Kode Cabang", cls: "span4 full", disable: "isPrintAvailable" },
                        { name: "Status", opt_text: "", cls: "span3", type: "select2", text: "Status", datasource: "Status" },
                        {
                            text: "No.BPU",
                            type: "controls",
                            required: true,
                            items: [
                                { name: "BPUNoFrom", cls: "span3", type: "popup", btnName: "btnBPUNoFrom", click: "BPUNoFrom()", disable: "data.isActive == false", required: true, validasi: "required" },
                                { name: "BPUDateFrom", text: "", cls: "span3", type: "ng-datepicker" },
                            ]
                        },
                        {
                            text: "s/d",
                            type: "controls",
                            required: true,
                            items: [
                                { name: "BPUNoTo", cls: "span3", type: "popup", btnName: "btnBPUNoTo", click: "BPUNoTo()", disable: "data.isActive == false", required: true, validasi: "required" },
                                { name: "BPUDateTo", text: "", cls: "span3", type: "ng-datepicker" },

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
        $(".switchlabel").attr("style", "padding:9px 0px 0px 5px")
        SimDms.Angular("spRptMstSales");

    }
});