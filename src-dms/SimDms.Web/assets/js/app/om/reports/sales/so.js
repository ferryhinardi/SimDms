"use strict"; //Reportid OmRpSalesTrn001
function spRptSalesOrder($scope, $http, $injector) {

    var me = $scope;
    var currentDate = moment().format();

    $injector.invoke(BaseController, this, { $scope: me });

    me.Status = [
        { "value": '-1', "text": 'All' },
        { "value": '0', "text": 'Open' },
        { "value": '1', "text": 'Printed' },
        { "value": '2', "text": 'Approved' },
        { "value": '3', "text": 'Cenceled' },
        { "value": '9', "text": 'Finished' }
    ];

    me.SONo = function () {
        if (me.data.Status == '') {
            me.data.Status = '-1';
        }
        var lookup = Wx.blookup({
            name: "SOLookup4Report",
            title: "SO",
            manager: spSalesManager,
            query: "SOLookup4Report?Status=" + me.data.Status,
            defaultSort: "SONo asc",
            columns: [
                { field: "SONo", title: "No. SO" },
                { field: "GroupPriceCode", title: "Tipe" },
                {
                    field: "SODate", title: "Tgl. SO",
                    template: "#= (SODate == undefined) ? '' : moment(SODate).format('DD MMM YYYY') #"
                },
                { field: "Status", title: "Status" },
            ],
        });
        lookup.dblClick(function (data) {
            if (data != null) {
                me.data.SONo = data.SONo;
                me.data.SODate = data.SODate;
                me.Apply();
            }
        });

    }

    me.SONoTo = function () {
        if (me.data.Status == '') {
            me.data.Status = '-1';
        }
        var lookup = Wx.blookup({
            name: "SOLookup4Report",
            title: "SO",
            manager: spSalesManager,
            query: "SOLookup4Report?Status=" + me.data.Status,
            defaultSort: "SONo asc",
            columns: [
                { field: "SONo", title: "No. SO" },
                { field: "GroupPriceCode", title: "Tipe" },
                {
                    field: "SODate", title: "Tgl. SO",
                    template: "#= (SODate == undefined) ? '' : moment(SODate).format('DD MMM YYYY') #"
                },
                { field: "Status", title: "Status" },
            ],
        });
        lookup.dblClick(function (data) {
            if (data != null) {
                me.data.SONoTo = data.SONo;
                me.data.SODateTo = data.SODate;
                me.Apply();
            }
        });

    }

    me.printPreview = function () {
        if (me.data.SONo == undefined || me.data.SONoTo == undefined) {
            MsgBox('Ada Informasi Yang Belum Lengkap', MSG_ERROR);
        }
        else {
            $http.post('om.api/ReportSales/ValidatePrintSO', me.data)
           .success(function (e) {
               if (e.success) {
                   BootstrapDialog.show({
                       message: $(
                           '<div class="container">' +
                           '<div class="row">' +
                           '<input type="checkbox" name="Param" id="Param">&nbsp Print Print Aksesoris/Part</div>' +

                           '<div class="row">' +

                           '<input type="radio" name="PrintType" id="PrintType1" value="0" checked>&nbsp Print Pesanan Penjualan</div>' +

                           '<div class="row">' +

                           '<input type="radio" name="PrintType" id="PrintType2" value="1">&nbsp Print Pesanan Penjualan dengan Rangka/Mesin</div>' +

                           '<div class="row">' +

                           '<input type="radio" name="PrintType" id="PrintType3" value="2">&nbsp Print Catatan Penjualan</div>' +

                           '<div class="row">' +

                           '<input type="radio" name="PrintType" id="PrintType4" value="3">&nbsp Print Lain-Lain</div>'),
                       closable: false,
                       draggable: true,
                       type: BootstrapDialog.TYPE_INFO,
                       title: 'Print Pesanan Penjualan',
                       buttons: [{
                           label: ' Print',
                           cssClass: 'btn-primary icon-print',
                           action: function (dialogRef) {
                               localStorage.setItem('MyParam', $('input[name=Param]:checked').val());
                               localStorage.setItem('MyCheck', $('input[name=PrintType]:checked').val());
                               me.printType();
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
        var param = localStorage.getItem('MyParam');
        if (me.data.Status == '-1') {
            me.data.Status = '';
        }
        if (printType === '0') {
            
            var Param = param == 'on' ? '1' : '0';
            var sizeType = $('input[name=sizeType]:checked').val() === 'full';
            var ReportId = sizeType ? 'OmRpSalesTrn001' : 'OmRpSalesTrn001D';
            var par = [
                me.data.SONo,
                me.data.SONoTo,
                me.data.ProfitCenterCode,
                me.data.Status,
                Param
            ]
            var rparam = 'Print Pesanan Penjualan'

            Wx.showPdfReport({
                id: ReportId,
                pparam: par.join(','),
                textprint: true,
                rparam: rparam,
                type: "devex"
            });
        }
        else if (printType === '1') {

            var sizeType = $('input[name=sizeType]:checked').val() === 'full';
            var ReportId = sizeType ? 'OmRpSalesTrn001A' : 'OmRpSalesTrn001E';
            var par = [
                me.data.SONo,
                me.data.SONoTo
            ]
            var rparam = 'Print Pesanan Penjualan dengan Rangka/Mesin'

            Wx.showPdfReport({
                id: ReportId,
                pparam: par.join(','),
                textprint: true,
                rparam: rparam,
                type: "devex"
            });
        }
        else if (printType === '2') {

            var Param = param == 'on' ? '1' : '0';
            var sizeType = $('input[name=sizeType]:checked').val() === 'full';
            var ReportId = sizeType ? 'OmRpSalesTrn001B' : 'OmRpSalesTrn001F';
            var par = [
                me.data.SONo,
                me.data.SONoTo,
                Param
            ]
            var rparam = 'Print Catatan Penjualan'

            Wx.showPdfReport({
                id: ReportId,
                pparam: par.join(','),
                textprint: true,
                rparam: rparam,
                type: "devex"
            });
        }
        else {

            var Param = param == 'on' ? '1' : '0';
            var sizeType = $('input[name=sizeType]:checked').val() === 'full';
            var ReportId = sizeType ? 'OmRpSalesTrn001C' : 'OmRpSalesTrn001D';
            var par = [
                me.data.SONo,
                me.data.SONoTo
            ]
            var rparam = 'Print Lain - Lain'

            $http.post('om.api/ReportSales/ValidatePrintSO', me.data)
           .success(function (e) {
               if (e.success) {
                   Wx.showPdfReport({
                       id: ReportId,
                       pparam: par.join(','),
                       rparam: rparam,
                       type: "devex"
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

    me.initialize = function () {
        me.data = {};
        me.change = false;
        me.data.SODate = currentDate;
        me.data.SODateTo = currentDate;
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
        title: "Report Sales Order",
        xtype: "panels",
        toolbars: [
            { name: "btnPrintPreview", text: "Print", cls: "btn btn-primary", icon: "icon-print", click: "printPreview()", disable: "!isPrintEnable" },
            { name: "btnCancel", text: "Cancel", cls: "btn btn-warning", icon: "icon-remove", click: "cancelOrClose()" },
        ],
        panels: [
            {
                name: "pnlA",
                items: [
                        { name: "CompanyCode", model: "data.CompanyCode", text: "Kode Perusahaan", cls: "span4 full", disable: "isPrintAvailable", show : false },
                        { name: "BranchCode", model: "data.BranchCode", text: "Kode Cabang", cls: "span4 full", disable: "isPrintAvailable", show: false },
                        { name: "ProfitCenterCode", model: "data.ProfitCenterCode", text: "Kode Cabang", cls: "span4 full", disable: "isPrintAvailable", show: false },
                        { name: "Status", opt_text: "", cls: "span3", type: "select2", text: "Status", datasource: "Status" },
                        {
                            text: "SO",
                            type: "controls",
                            required: true, validasi: "required",
                            items: [
                                { name: "SONo", cls: "span2", type: "popup", click: "SONo()" },
                                { name: "SODate", cls: "span3", type: "ng-datepicker", disable: "true", readonly: true },
                            ]
                        },
                        {
                            text: "s/d",
                            type: "controls",
                            required: true, validasi: "required",
                            items: [
                                { name: "SONoTo", cls: "span2", type: "popup", click: "SONoTo()" },
                                { name: "SODateTo", cls: "span3", type: "ng-datepicker", disable: "true", readonly: true },
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
        SimDms.Angular("spRptSalesOrder");

    }
});