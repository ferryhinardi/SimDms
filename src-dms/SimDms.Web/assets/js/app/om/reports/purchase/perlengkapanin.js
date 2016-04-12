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

    me.PerlengkapanNoFrom = function () {
        var lookup = Wx.blookup({
            name: "PerlengkapanNoLookup",
            title: "No. Perlengkapan",
            manager: spSalesManager,
            query: new breeze.EntityQuery.from("Perlengkapan4Lookup").withParameters({ Status: me.data.Status }),
            defaultSort: "PerlengkapanNo asc",
            columns: [
                { field: "PerlengkapanNo", title: "No. Perlengkapan" },
                { field: "PerlengkapanDate", title: "Tgl. Perlengkapan", template: "#= (PerlengkapanDate == undefined) ? '' : moment(PerlengkapanDate).format('DD MMM YYYY') #" },
                 { field: "Status", title: "Status" },
            ],
        });
        lookup.dblClick(function (data) {
            if (data != null) {
                me.data.PerlengkapanNoFrom = data.PerlengkapanNo;
                me.data.PerlengkapanDateFrom = data.PerlengkapanDate;
                me.Apply();
            }
        });

    }

    me.PerlengkapanNoTo = function () {
        var lookup = Wx.blookup({
            name: "NoPerlengkapanLookup",
            title: "No. Perlengkapan",
            manager: spSalesManager,
            query: new breeze.EntityQuery.from("Perlengkapan4Lookup").withParameters({ Status: me.data.Status }),
            defaultSort: "PerlengkapanNo asc",
            columns: [
                 { field: "PerlengkapanNo", title: "No. Perlengkapan" },
                { field: "PerlengkapanDate", title: "Tgl. Perlengkapan", template: "#= (PerlengkapanDate == undefined) ? '' : moment(PerlengkapanDate).format('DD MMM YYYY') #" },
                 { field: "Status", title: "Status" },
            ],
        });
        lookup.dblClick(function (data) {
            if (data != null) {
                me.data.PerlengkapanNoTo = data.PerlengkapanNo;
                me.data.PerlengkapanDateTo = data.PerlengkapanDate;
                me.Apply();
            }
        });

    }

    me.printPreview = function () {
        if (me.data.PerlengkapanNoFrom || me.data.PerlengkapanNoTo) {
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
            MsgBox("No. Perlengkapan Awal/ Akhir Harus diisi", MSG_ERROR);
        }
    }

    me.Print = function () {
        //alert(me.data.ProfitCenterCode);
        if (me.data.Status == '-1') {
            me.data.Status = '';
        }
        var sizeType = $('input[name=sizeType]:checked').val() === 'full';
        var ReportId = sizeType ? 'OmRpPurTRN005' : 'OmRpPurTRN005A';
        var prm = [
                   // me.data.CompanyCode,
                    me.data.PerlengkapanNoFrom,
                    me.data.PerlengkapanNoTo,
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

        me.data.PerlengkapanDateFrom = me.now();
        me.data.PerlengkapanDateTo = me.now();
        $('#PerlengkapanDateFrom').attr('disabled', true);
        $('#PerlengkapanDateTo').attr('disabled', true);
        me.isPrintAvailable = true;
    }
    
    me.start();

}


$(document).ready(function () {
    var options = {
        title: "Report Perlengkapan In",
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
                            text: "No.Perlengkapan",
                            type: "controls",
                            required: true,
                            items: [
                                { name: "PerlengkapanNoFrom", cls: "span3", type: "popup", btnName: "btnPerlengkapanNoFrom", click: "PerlengkapanNoFrom()", disable: "data.isActive == false", required: true, validasi: "required" },
                                { name: "PerlengkapanDateFrom", text: "", cls: "span3", type: "ng-datepicker" },
                            ]
                        },
                        {
                            text: "s/d",
                            type: "controls",
                            required: true,
                            items: [
                                { name: "PerlengkapanNoTo", cls: "span3", type: "popup", btnName: "btnPerlengkapanNoTo", click: "PerlengkapanNoTo()", disable: "data.isActive == false", required: true, validasi: "required" },
                                { name: "PerlengkapanDateTo", text: "", cls: "span3", type: "ng-datepicker" },

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