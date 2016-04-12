"use strict"; //Reportid OmRpSalesTrn004
function RptSalesInvoice($scope, $http, $injector) {

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

    me.InvoiceNo = function () {
        if (me.data.Status == '') {
            me.data.Status = '-1';
        }
        var lookup = Wx.blookup({
            name: "SalesInvoiceLookup4Report",
            title: "Invoice",
            manager: spSalesManager,
            query: "SalesInvoiceLookup4Report?Status=" + me.data.Status,
            defaultSort: "InvoiceNo asc",
            columns: [
                { field: "InvoiceNo", title: "No. Invoice" },
                { field: "GroupPriceCode", title: "Tipe" },
                {
                    field: "InvoiceDate", title: "Tgl. Invoice",
                    template: "#= (InvoiceDate == undefined) ? '' : moment(InvoiceDate).format('DD MMM YYYY') #"
                },
                { field: "Status", title: "Status" },
            ],
        });
        lookup.dblClick(function (data) {
            if (data != null) {
                me.data.InvoiceNo = data.InvoiceNo;
                me.data.InvoiceDate = data.InvoiceDate;
                me.Apply();
            }
        });

    }

    me.InvoiceNoTo = function () {
        if (me.data.Status == '') {
            me.data.Status = '-1';
        }
        var lookup = Wx.blookup({
            name: "SalesInvoiceLookup4Report",
            title: "Invoice",
            manager: spSalesManager,
            query: "SalesInvoiceLookup4Report?Status=" + me.data.Status,
            defaultSort: "InvoiceNo asc",
            columns: [
                { field: "InvoiceNo", title: "No. Invoice" },
                { field: "GroupPriceCode", title: "Tipe" },
                {
                    field: "InvoiceDate", title: "Tgl. Invoice",
                    template: "#= (InvoiceDate == undefined) ? '' : moment(InvoiceDate).format('DD MMM YYYY') #"
                },
                { field: "Status", title: "Status" },
            ],
        });
        lookup.dblClick(function (data) {
            if (data != null) {
                me.data.InvoiceNoTo = data.InvoiceNo;
                me.data.InvoiceDateTo = data.InvoiceDate;
                me.Apply();
            }
        });

    }

    me.printPreview = function () {
        if (me.data.InvoiceNo == undefined || me.data.InvoiceNoTo == undefined) {
            MsgBox('Ada Informasi Yang Belum Lengkap', MSG_ERROR);
        }
        else {
            $http.post('om.api/ReportSales/ValidatePrintInvoice', me.data)
           .success(function (e) {
               if (e.success) {
                   BootstrapDialog.show({
                       message: $(
                           '<div class="container">' +
                           '<div class="row">' +

                           '<input type="checkbox" name="Param" id="Param">&nbsp Tampilkan Sperpart/Accesorris</div>' +

                           '<div class="row">' +

                           '<input type="radio" name="PrintType" id="PrintType1" value="0">&nbsp Print Invoice Pre-Printed</div>' +

                           '<div class="row">' +

                           '<input type="radio" name="PrintType" id="PrintType2" value="1" checked>&nbsp Print Invoice Non Pre-Printed</div>' +

                           '<div class="row">' +

                           '<input type="radio" name="PrintType" id="PrintType2" value="2">&nbsp Print Nota Debet 1/2 Halaman</div>' +

                           '<div class="row">' +

                           '<input type="radio" name="PrintType" id="PrintType4" value="3">&nbsp Print Nota Debet 1 Halaman</div>'),
                       closable: false,
                       draggable: true,
                       type: BootstrapDialog.TYPE_INFO,
                       title: 'Print',
                       buttons: [{
                           label: ' Print',
                           cssClass: 'btn-primary icon-print',
                           action: function (dialogRef) {
                               localStorage.setItem('MyParam', $('input[name=Param]:checked').val());
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
                   MsgBox(e.message, MSG_ERROR);
                   return;
               }
           })
           .error(function (e) {
               MsgBox('Terjadi Kesalahan, Hubungi SDMS Support', MSG_ERROR);
           });
        }
    }

    me.Print = function () {
        if (me.data.Status == '-1') {
            me.data.Status = '';
        }
        var param = localStorage.getItem('MyParam');
        var Param = param == 'on' ? '1' : '0';

        if ($('input[name=PrintType]:checked').val() === '0') {
            var ReportId = 'OmRpSalesTrn004';
            var par = [
                me.data.InvoiceNo,
                me.data.InvoiceNoTo,
                '200',
                me.data.Status,
                Param
            ]
            var rparam = 'Print Invoice'

            Wx.showPdfReport({
                id: ReportId,
                pparam: par.join(','),
                textprint: true,
                rparam: rparam,
                type: "devex"
            });
        }
        else if ($('input[name=PrintType]:checked').val() === '1') {
            var ReportId = 'OmRpSalesTrn004A';
            var par = [
                me.data.InvoiceNo,
                me.data.InvoiceNoTo,
                me.data.ProfitCenterCode,
                me.data.Status,
                Param
            ]
            var rparam = 'Print Invoice'

            Wx.showPdfReport({
                id: ReportId,
                pparam: par.join(','),
                textprint: true,
                rparam: rparam,
                type: "devex"
            });
        }
        else if ($('input[name=PrintType]:checked').val() === '2') {
            var ReportId = 'OmRpSalesTrn009A';
            var par = [
                me.data.InvoiceNo,
                me.data.InvoiceNoTo,
            ]
            var rparam = 'Print Nota Debet'

            Wx.showPdfReport({
                id: ReportId,
                pparam: par.join(','),
                textprint: true,
                rparam: rparam,
                type: "devex"
            });
        }
        else {
            var ReportId = 'OmRpSalesTrn009';
            var par = [
                me.data.InvoiceNo,
                me.data.InvoiceNoTo,
            ]
            var rparam = 'Print Nota Debet'

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
        me.data.InvoiceDate = me.now();
        me.data.InvoiceDateTo = me.now();
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
        title: "Report Sales Invoice",
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
                            text: "No. Invoice",
                            type: "controls",
                            items: [
                                { name: "InvoiceNo", cls: "span2", type: "popup", click: "InvoiceNo()" },
                                { name: "InvoiceDate", cls: "span3", type: "ng-datepicker", disable: "true", readonly: true },
                            ]
                        },
                        {
                            text: "s/d",
                            type: "controls",
                            items: [
                                { name: "InvoiceNoTo", cls: "span2", type: "popup", click: "InvoiceNoTo()" },
                                { name: "InvoiceDateTo", cls: "span3", type: "ng-datepicker", disable: "true", readonly: true },
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
        SimDms.Angular("RptSalesInvoice");

    }
});