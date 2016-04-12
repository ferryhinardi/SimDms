"use strict"; //Reportid OmRpMst001
function spRptMstSales($scope, $http, $injector) {

    var me = $scope;

    $injector.invoke(BaseController, this, { $scope: me });

    me.supplier = function () {
        var lookup = Wx.blookup({
            name: "SupplierBrowse",
            title: "Pemasok",
            manager: spSalesManager,
            query: "SupplierCodeLookup",
            defaultSort: "SupplierCode asc",
            columns: [
                { field: "SupplierCode", title: "Kode Pemasok" },
                { field: "SupplierName", title: "Nama Pemasok" },
            ]
        });
        lookup.dblClick(function (data) {
            if (data != null) {
                me.data.SupplierCode = data.SupplierCode;
                me.data.SupplierName = data.SupplierName;
                me.Apply();
            }
        });
    };

    me.PONoFrom = function () {
        var lookup = Wx.blookup({
            name: "PONoLookup",
            title: "No. PO",
            manager: spSalesManager,
            query: new breeze.EntityQuery.from("PO4Lookup").withParameters({ Status: me.data.Status }),
            defaultSort: "PONo asc",
            columns: [
                { field: "PONo", title: "No. PO" },
                { field: "PODate", title: "Tgl. PO", template: "#= (PODate == undefined) ? '' : moment(PODate).format('DD MMM YYYY') #" },
                 { field: "Status", title: "Status" },
            ],
        });
        lookup.dblClick(function (data) {
            if (data != null) {
                me.data.PONoFrom = data.PONo;
                me.data.PODateFrom = data.PODate;
                me.Apply();
            }
        });

    }

    me.PONoTo = function () {
        var lookup = Wx.blookup({
            name: "NoPOLookup",
            title: "No. PO",
            manager: spSalesManager,
            query: new breeze.EntityQuery.from("PO4Lookup").withParameters({ Status: me.data.Status }),
            defaultSort: "PONo asc",
            columns: [
                 { field: "PONo", title: "No. PO" },
                { field: "PODate", title: "Tgl. PO", template: "#= (PODate == undefined) ? '' : moment(PODate).format('DD MMM YYYY') #" },
                 { field: "Status", title: "Status" },
            ],
        });
        lookup.dblClick(function (data) {
            if (data != null) {
                me.data.PONoTo = data.PONo;
                me.data.PODateTo = data.PODate;
                me.Apply();
            }
        });

    }

    me.printPreview = function () {
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
        var param = "";
        if ($('#isC1').prop('checked') == false && $('#isC3').prop('checked') == true)
        { param = "0"; }
        else if ($('#isC1').prop('checked') == false && $('#isC3').prop('checked') == false)
        { param = "1"; }
        else if ($('#isC1').prop('checked') == true && $('#isC3').prop('checked') == false)
        { param = "2"; }
        else if ($('#isC1').prop('checked') == true && $('#isC3').prop('checked') == true)
        { param = "3"; }

        var DateFrom = new Date(me.data.DateFrom).getMonth() + 1 + '/' + new Date(me.data.DateFrom).getDate() + '/' + new Date(me.data.DateFrom).getFullYear();
        var DateTo = new Date(me.data.DateTo).getMonth() + 1 + '/' + new Date(me.data.DateTo).getDate() + '/' + new Date(me.data.DateTo).getFullYear();
        var sizeType = $('input[name=sizeType]:checked').val() === 'full';
        var ReportId = sizeType ? 'OmRpPurRgs001' : 'OmRpPurRgs001A';
        var prm = [
                   // me.data.CompanyCode,
                    DateFrom,
                    DateTo,
                    me.data.SupplierCode,
                    me.data.PONoFrom,
                    me.data.PONoTo,
                    param
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
        me.data.Status = "-1";
        $('#isC1').prop('checked', true);
        $http.get('breeze/sales/CurrentUserInfo').
          success(function (dl, status, headers, config) {
              me.data.CompanyCode = dl.CompanyCode;
              me.data.BranchCode = dl.BranchCode;
          });
        //alert(new Date().getMonth());
        //me.data.DateFrom = new Date().getMonth() + 1 + '/' + new Date().getDay() + '/' + new Date().getFullYear();//me.now();
        //me.data.DateTo = new Date().getMonth() + 1 + '/' + new Date().getDay() + '/' + new Date().getFullYear();  //me.now();
        //me.data.DateFrom = 7 + '/' + 1 + '/' + (new Date().getFullYear() - 1);
        //me.data.DateTo = 12 + '/' + 31 + '/' + (new Date().getFullYear() - 1);
        if (new Date(Date.now()).getMonth() >= 0 || new Date(Date.now()).getMonth() <= 5) {
            me.data.DateFrom = 7 + '/' + 1 + '/' + (new Date().getFullYear() - 1);
            me.data.DateTo = 12 + '/' + 31 + '/' + (new Date().getFullYear() - 1);
        }
        else {
            me.data.DateFrom = 1 + '/' + 1 + '/' + (new Date().getFullYear());
            me.data.DateTo = 6 + '/' + 30 + '/' + (new Date().getFullYear());
        }

        var date = new Date();
        me.data.DateFrom = new Date(date.getFullYear(), date.getMonth(), 1);
        me.data.DateTo = new Date(date.getFullYear(), date.getMonth() + 1, 0);



        $('#SupplierCode').attr('disabled', true);
        $('#btnSupplierCode').attr('disabled', true);
        $('#PONoFrom').attr('disabled', true);
        $('#PONoTo').attr('disabled', true);
        $('#btnPONoFrom').attr('disabled', true);
        $('#btnPONoTo').attr('disabled', true);
        me.isPrintAvailable = true;
    }

    $('#isC3').on('change', function (e) {
        if ($('#isC3').prop('checked') == true) {
            $('#PONoFrom').removeAttr('disabled');
            $('#PONoTo').removeAttr('disabled');
            $('#btnPONoFrom').removeAttr('disabled');
            $('#btnPONoTo').removeAttr('disabled');
        } else {
            $('#PONoFrom').attr('disabled', true);
            $('#PONoTo').attr('disabled', true);
            $('#btnPONoFrom').attr('disabled', true);
            $('#btnPONoTo').attr('disabled', true);
            $('#PONoFrom').val("");
            $('#PONoTo').val("");
        }
        me.Apply();
    })

    $('#isC2').on('change', function (e) {
        if ($('#isC2').prop('checked') == true) {
            $('#SupplierCode').removeAttr('disabled');
            $('#btnSupplierCode').removeAttr('disabled');
        } else {
            $('#SupplierCode').attr('disabled', true);
            $('#btnSupplierCode').attr('disabled', true);
            $('#SupplierCode').attr('disabled', true);
            $('#btnSupplierCode').attr('disabled', true);
            $('#SupplierCode').val("");
            $('#SupplierName').val("");
        }
        me.Apply();
    })

    $('#isC1').on('change', function (e) {
        if ($('#isC1').prop('checked') == true) {
            me.data.DateFrom = me.now();
            me.data.DateTo = me.now();
            $('#DateFrom').prop('readonly', false);
            $('#DateTo').prop('readonly', false);
        } else {
            me.data.DateFrom = undefined;
            $('#DateFrom').prop('readonly', true);
            me.data.DateTo = undefined;
            $('#DateTo').prop('readonly', true);
        }
        me.Apply();
    })
    me.start();

}


$(document).ready(function () {
    var options = {
        title: "Report Register Pesanan Kendaraan",
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
                        //{ name: "Status", opt_text: "", cls: "span3", type: "select2", text: "Status", datasource: "Status" },
                        {
                            text: "Tanggal",
                            type: "controls",
                            items: [
                                { name: 'isC1', type: 'check', text: '', cls: 'span1', float: 'left' },
                                 { name: "DateFrom", text: "", cls: "span3", type: "ng-datepicker" },
                                { name: "DateTo", text: "", cls: "span3", type: "ng-datepicker" },
                            ]
                        },
                        {
                            text: "Pemasok",
                            type: "controls",
                            items: [
                                { name: 'isC2', type: 'check', text: '', cls: 'span1', float: 'left' },
                                { name: "SupplierCode", cls: "span3", type: "popup", btnName: "btnBPUNoFrom", click: "supplier()", disable: "data.isActive == false" },
                                { name: "SupplierName", text: "", cls: "span4", type: "text" , readonly : true},
                            ]
                        },
                         {
                             text: "No.PO",
                             type: "controls",
                             items: [
                                 { name: 'isC3', type: 'check', text: '', cls: 'span1', float: 'left' },
                                 { name: "PONoFrom", cls: "span3", type: "popup", btnName: "btnPONoFrom", click: "PONoFrom()", disable: "data.isActive == false" },
                                 { name: "PONoTo", cls: "span3", type: "popup", btnName: "btnPONoTo", click: "PONoTo()", disable: "data.isActive == false" },
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