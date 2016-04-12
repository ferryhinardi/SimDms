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

    me.ReturnNoFrom = function () {
        var lookup = Wx.blookup({
            name: "ReturnNoLookup",
            title: "No. Return",
            manager: spSalesManager,
            query: new breeze.EntityQuery.from("Return4Lookup").withParameters({ Supplier: "" }),
            defaultSort: "ReturnNo asc",
            columns: [
                { field: "ReturnNo", title: "No. Return" },
                { field: "ReturnDate", title: "Tgl. Return", template: "#= (ReturnDate == undefined) ? '' : moment(ReturnDate).format('DD MMM YYYY') #" },
            ],
        });
        lookup.dblClick(function (data) {
            if (data != null) {
                me.data.ReturnNoFrom = data.ReturnNo;
                me.Apply();
            }
        });

    }

    me.ReturnNoTo = function () {
        var lookup = Wx.blookup({
            name: "ReturnNoLookup",
            title: "No. Return",
            manager: spSalesManager,
            query: new breeze.EntityQuery.from("Return4Lookup").withParameters({ Supplier: "" }),
            defaultSort: "ReturnNo asc",
            columns: [
                { field: "ReturnNo", title: "No. Return" },
                { field: "ReturnDate", title: "Tgl. Return", template: "#= (ReturnDate == undefined) ? '' : moment(ReturnDate).format('DD MMM YYYY') #" },
            ],
        });
        lookup.dblClick(function (data) {
            if (data != null) {
                me.data.ReturnNoTo = data.ReturnNo;
                me.Apply();
            }
        });

    }

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
        //alert(me.options);
        var param = "";
        if ($('#isC1').prop('checked') == true)
            param = "1";
        else
            param = "0";
       
        var sizeType = $('input[name=sizeType]:checked').val() === 'full';
        var ReportId = sizeType ? 'OmRpPurRgs003' : 'OmRpPurRgs003A';
        var prm = [
                   // me.data.CompanyCode,
                    me.data.DateFrom,
                    me.data.DateTo,
                    me.data.SupplierCode,
                    me.data.BPUNoFrom,
                    me.data.BPUNoTo,
                    me.data.ReturnNoFrom,
                    me.data.ReturnNoTo,
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
        me.data.Status = '-1';
        $('#isC1').prop('checked', true);
        $http.get('breeze/sales/CurrentUserInfo').
          success(function (dl, status, headers, config) {
              me.data.CompanyCode = dl.CompanyCode;
              me.data.BranchCode = dl.BranchCode;

          });

        //alert(new Date().getMonth());
        //me.data.DateFrom = new Date().getMonth() + 1 + '/' + new Date().getDay() + '/' + new Date().getFullYear();//me.now();
        // me.data.DateTo = new Date().getMonth() + 1 + '/' + new Date().getDay() + '/' + new Date().getFullYear();  //me.now();
        //me.data.DateFrom = 7 + '/' + 1 + '/' + (new Date().getFullYear() - 1);//me.now();
        //me.data.DateTo = 12 + '/' + 31 + '/' + (new Date().getFullYear() - 1);  //me.now();
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
        $('#ReturnNoFrom').attr('disabled', true);
        $('#ReturnNoTo').attr('disabled', true);
        $('#btnReturnNoFrom').attr('disabled', true);
        $('#btnReturnNoTo').attr('disabled', true);

        $('#BPUNoFrom').attr('disabled', true);
        $('#BPUNoTo').attr('disabled', true);
        $('#btnBPUNoFrom').attr('disabled', true);
        $('#btnBPUNoTo').attr('disabled', true);
        me.isPrintAvailable = true;
    }

    $('#isC4').on('change', function (e) {
        if ($('#isC4').prop('checked') == true) {
            $('#BPUNoFrom').removeAttr('disabled');
            $('#BPUNoTo').removeAttr('disabled');
            $('#btnBPUNoFrom').removeAttr('disabled');
            $('#btnBPUNoTo').removeAttr('disabled');
        } else {
            $('#BPUNoFrom').attr('disabled', true);
            $('#BPUNoTo').attr('disabled', true);
            $('#btnBPUNoFrom').attr('disabled', true);
            $('#btnBPUNoTo').attr('disabled', true);
            $('#BPUNoFrom').val("");
            $('#BPUNoTo').val("");
        }
        me.Apply();
    })

    $('#isC3').on('change', function (e) {
        if ($('#isC3').prop('checked') == true) {
            $('#ReturnNoFrom').removeAttr('disabled');
            $('#ReturnNoTo').removeAttr('disabled');
            $('#btnReturnNoFrom').removeAttr('disabled');
            $('#btnReturnNoTo').removeAttr('disabled');
        } else {
            $('#ReturnNoFrom').attr('disabled', true);
            $('#ReturnNoTo').attr('disabled', true);
            $('#btnReturnNoFrom').attr('disabled', true);
            $('#btnReturnNoTo').attr('disabled', true);
            $('#ReturnNoFrom').val("");
            $('#ReturnNoTo').val("");
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
            me.data.DateTo = undefined;
            $('#DateFrom').prop('readonly', true);
            $('#DateTo').prop('readonly', true);
        }
        me.Apply();
    })
    me.start();
    me.options = '0';
}


$(document).ready(function () {
    var options = {
        title: "Report Register Pengembalian Unit",
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
                                { name: "SupplierName", text: "", cls: "span4", type: "text", readonly: true },
                            ]
                        },
                          {
                              text: "No.BPU",
                              type: "controls",
                              items: [
                                  { name: 'isC4', type: 'check', text: '', cls: 'span1', float: 'left' },
                                  { name: "BPUNoFrom", cls: "span3", type: "popup", btnName: "btnBPUNoFrom", click: "BPUNoFrom()", disable: "data.isActive == false" },
                                  { name: "BPUNoTo", cls: "span3", type: "popup", btnName: "btnBPUNoTo", click: "BPUNoTo()", disable: "data.isActive == false" },
                              ]
                          },
                            {
                                text: "No.Return",
                                type: "controls",
                                items: [
                                    { name: 'isC3', type: 'check', text: '', cls: 'span1', float: 'left' },
                                    { name: "ReturnNoFrom", cls: "span3", type: "popup", btnName: "btnReturnNoFrom", click: "ReturnNoFrom()", disable: "data.isActive == false" },
                                    { name: "ReturnNoTo", cls: "span3", type: "popup", btnName: "btnReturnNoTo", click: "ReturnNoTo()", disable: "data.isActive == false" },
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