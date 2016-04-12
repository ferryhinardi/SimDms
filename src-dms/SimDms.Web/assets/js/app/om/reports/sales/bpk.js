"use strict"; //Reportid OmRpSalesTrn003
function RptSalesBPK($scope, $http, $injector) {

    var me = $scope;

    $injector.invoke(BaseController, this, { $scope: me });

    me.Status = [
        { "value": '-1', "text": 'All' },
        { "value": '0', "text": 'Open' },
        { "value": '1', "text": 'Printed' },
        { "value": '2', "text": 'Approved' },
        { "value": '3', "text": 'Cenceled' },
        { "value": '9', "text": 'Finished' }
    ];

    me.BPKNo = function () {
        if (me.data.Status == '') {
            me.data.Status = '-1';
        }
        var lookup = Wx.blookup({
            name: "BPKLookup4Report",
            title: "BPK",
            manager: spSalesManager,
            query: "BPKLookup4Report?Status=" + me.data.Status,
            defaultSort: "SONo asc",
            columns: [
                { field: "BPKNo", title: "No. BPK" },
                { field: "GroupPriceCode", title: "Tipe" },
                {
                    field: "BPKDate", title: "Tgl. BPK",
                    template: "#= (BPKDate == undefined) ? '' : moment(BPKDate).format('DD MMM YYYY') #"
                },
                { field: "Status", title: "Status" },
            ],
        });
        lookup.dblClick(function (data) {
            if (data != null) {
                me.data.BPKNo = data.BPKNo;
                me.data.BPKDate = data.BPKDate;
                me.Apply();
            }
        });

    }

    me.BPKNoTo = function () {
        if (me.data.Status == '') {
            me.data.Status = '-1';
        }
        var lookup = Wx.blookup({
            name: "BPKLookup4Report",
            title: "BPK",
            manager: spSalesManager,
            query: "BPKLookup4Report?Status=" + me.data.Status,
            defaultSort: "SONo asc",
            columns: [
                { field: "BPKNo", title: "No. BPK" },
                { field: "GroupPriceCode", title: "Tipe" },
                {
                    field: "BPKDate", title: "Tgl. BPK",
                    template: "#= (BPKDate == undefined) ? '' : moment(BPKDate).format('DD MMM YYYY') #"
                },
                { field: "Status", title: "Status" },
            ],
        });
        lookup.dblClick(function (data) {
            if (data != null) {
                me.data.BPKNoTo = data.BPKNo;
                me.data.BPKDateTo = data.BPKDate;
                me.Apply();
            }
        });

    }

    me.printPreview = function () {
        if (me.data.BPKNo == undefined || me.data.BPKNoTo == undefined) {
            MsgBox('Ada Informasi Yang Belum Lengkap', MSG_ERROR);
        }
        else {
            $http.post('om.api/ReportSales/ValidatePrintBPK', me.data)
           .success(function (e) {
               if (e.success) {
                   BootstrapDialog.show({
                       message: $(
                           '<div class="container">' +
                           '<div class="row">' +

                           '<input type="radio" name="PrintType" id="PrintType1" value="0" checked>&nbsp Pre - Printed</div>' +

                           '<div class="row">' +

                           '<input type="radio" name="PrintType" id="PrintType2" value="1">&nbsp Formatting</div>' +

                           '<div class="row">' +

                           '<input type="radio" name="PrintType" id="PrintType4" value="2">&nbsp Surat Jalan Sheet</div>'),
                       closable: false,
                       draggable: true,
                       type: BootstrapDialog.TYPE_INFO,
                       title: 'Print Pesanan Penjualan',
                       buttons: [{
                           label: ' Print',
                           cssClass: 'btn-primary icon-print',
                           action: function (dialogRef) {
                               localStorage.setItem('MyCheck', $('input[name=PrintType]:checked').val());
                               if ($('input[name=PrintType]:checked').val() === '2') {
                                   me.printType();
                                   dialogRef.close();
                               }
                               else {
                                   me.Print();
                                   dialogRef.close();
                               }
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
                   MsgBox(e.message, MSG_ERROR);
                   return;
               }
           })
           .error(function (e) {
               MsgBox('Terjadi Kesalahan, Hubungi SDMS Support', MSG_ERROR);
           });
        }

    }

    me.printType = function () {
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
    }

    me.Print = function () {
        var printType = localStorage.getItem('MyCheck');
        if (me.data.Status == '-1') {
            me.data.Status = '';
        }
        if (printType === '0') {

            var ReportId = 'OmRpSalesTrn003A';
            var par = [
                me.data.BPKNo,
                me.data.BPKNoTo,
                me.data.ProfitCenterCode,
            ]
            var rparam = 'Print Pre Printed'

            Wx.showPdfReport({
                id: ReportId,
                pparam: par.join(','),
                textprint: true,
                rparam: rparam,
                type: "devex"
            });
        }
        else if (printType === '1') {

            var ReportId = 'OmRpSalesTrn003D';
            var par = [
                me.data.BPKNo,
                me.data.BPKNoTo,
                me.data.ProfitCenterCode,
            ]
            var rparam = 'Print Formatting'

            Wx.showPdfReport({
                id: ReportId,
                pparam: par.join(','),
                textprint: true,
                rparam: rparam,
                type: "devex"
            });
        }
        else {
            var sizeType = $('input[name=sizeType]:checked').val() === 'full';
            var ReportId = sizeType ? 'OmRpSalesTrn003B' : 'OmRpSalesTrn003C';
            var par = [
                me.data.BPKNo,
                me.data.BPKNoTo,
                me.data.ProfitCenterCode,
                me.data.Status
            ]
            var rparam = 'Print Surat Jalan Sheet'

            Wx.showPdfReport({
                id: ReportId,
                pparam: par.join(','),
                textprint: true,
                rparam: rparam,
                type: "devex"
            });
        }

    }

    me.initialize = function () {
        me.data = {};
        me.change = false;
        me.data.BPKDate = me.now();
        me.data.BPKDateTo = me.now();

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

        me.isPrintAvailable = true;
    }


    me.start();

}


$(document).ready(function () {
    var options = {
        title: "Report BPK",
        xtype: "panels",
        toolbars: [
            { name: "btnPrintPreview", text: "Print", cls: "btn btn-primary", icon: "icon-print", click: "printPreview()", disable: "!isPrintEnable" },
            { name: "btnCancel", text: "Cancel", cls: "btn btn-warning", icon: "icon-remove", click: "cancelOrClose()" },
        ],
        panels: [
            {
                name: "pnlA",
                items: [
                        { name: "CompanyCode", model: "data.CompanyCode", text: "Kode Perusahaan", cls: "span4 full", disable: "isPrintAvailable", show: false },
                        { name: "BranchCode", model: "data.BranchCode", text: "Kode Cabang", cls: "span4 full", disable: "isPrintAvailable", show: false },
                        { name: "ProfitCenterCode", model: "data.ProfitCenterCode", text: "Kode Cabang", cls: "span4 full", disable: "isPrintAvailable", show: false },
                        { name: "Status", opt_text: "", cls: "span3", type: "select2", text: "Status", datasource: "Status" },
                        {
                            text: "BPK",
                            type: "controls",
                            items: [
                                { name: "BPKNo", cls: "span2", type: "popup", click: "BPKNo()" },
                                { name: "BPKDate", cls: "span3", type: "ng-datepicker", disable: "true", readonly: true },
                            ]
                        },
                        {
                            text: "s/d",
                            type: "controls",
                            items: [
                                { name: "BPKNoTo", cls: "span2", type: "popup", click: "BPKNoTo()" },
                                { name: "BPKDateTo", cls: "span3", type: "ng-datepicker", disable: "true", readonly: true },
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
        SimDms.Angular("RptSalesBPK");

    }
});