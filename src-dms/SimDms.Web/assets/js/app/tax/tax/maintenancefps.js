"use strict"
var FPJNo = 0;

function TaxMaintenancefps($scope, $http, $injector) {
    var me = $scope;

    $injector.invoke(BaseController, this, { $scope: me });

    me.BranchCode = function () {
        var lookup = Wx.blookup({
            name: "BranchLookup4Report",
            title: "Branch",
            manager: spSalesManager,
            query: "BranchLookup4Report",
            defaultSort: "BranchCode asc",
            columns: [
                { field: "BranchCode", title: "Kode Cabang" },
                { field: "CompanyName", title: "Nama Cabang" },
            ],
        });
        lookup.dblClick(function (data) {
            if (data != null) {
                me.data.BranchCode = data.BranchCode;
                me.data.BranchName = data.CompanyName;
                me.Apply();
            }
        });

    }

    me.FPJNo = function () {
        if (me.optionsTrans == "Service") {
            browseService();
        } else if (me.optionsTrans == "Sparepart") {
            browseSparepart();
        }
    }

    me.CustomerCodeBill = function () {
        if (me.data.BranchCode == undefined) { me.data.BranchCode = ""; } else { me.data.BranchCode; }
        if (me.data.CustomerCodeBill == undefined) { me.data.CustomerCodeBill = ""; } else { me.data.CustomerCodeBill; }
        if (me.data.FPJNo == undefined) { me.data.FPJNo = ""; } else { me.data.FPJNo; }

        var lookup = Wx.blookup({
            name: "CustomerCodeBill",
            title: "Customer",
            manager: TaxManager,
            query: new breeze.EntityQuery().from("CustomerCodeBill").withParameters({ Branch: me.data.BranchCode, OptionsTrans: me.optionsTrans, CustomerCode: me.data.CustomerCodeBill, FPJNo: me.data.FPJNo }),
            defaultSort: "CustomerCode asc",
            columns: [
                { field: "CustomerCode", title: "Kode Customer" },
                { field: "CustomerName", title: "Nama Customer" },
            ],
        });
        lookup.dblClick(function (data) {
            if (data != null) {
                me.data.CustomerCodeBill = data.CustomerCode;
                me.data.CustomerNameBill = data.CustomerName;
                me.Apply();
            }
        });

    }

    me.browse = function () {
        if (me.optionsTrans == "Service") {
            browseService();
        } else if (me.optionsTrans == "Sparepart") {
            browseSparepart();
        }
    }

    function browseService() {
        if (me.data.BranchCode == undefined) {
            me.data.BranchCode = "";
        } else {
            me.data.BranchCode;
        }
        var lookup = Wx.blookup({
            name: "svFakturPajak4Tax",
            title: "No Faktur Pajak",
            manager: TaxManager,
            //query: "svFakturPajak4Tax",
            query: new breeze.EntityQuery().from("svFakturPajak4Tax").withParameters({ Branch: me.data.BranchCode }),
            defaultSort: "FPJNo desc",
            columns: [
                { field: "FPJNo", title: "No Faktur Pajak" },
                {
                    field: "FPJDate", title: "Tgl Faktur Pajak",
                    template: "#= (FPJDate == undefined) ? '' : moment(FPJDate).format('DD MMM YYYY') #"
                },
                { field: "CustomerNameBill", title: "Pelanggan" },
            ]
        });
        lookup.dblClick(function (data) {
            if (data != null) {
                me.lookupAfterSelect(data);
                me.Apply();
            }
        });
    }

    function browseSparepart() {
        if (me.data.BranchCode == undefined) {
            me.data.BranchCode = "";
        } else {
            me.data.BranchCode;
        }
        var lookup = Wx.blookup({
            name: "spFakturPajak4Tax",
            title: "No Faktur Pajak",
            manager: TaxManager,
            //query: "spFakturPajak4Tax",
            query: new breeze.EntityQuery().from("spFakturPajak4Tax").withParameters({ Branch: me.data.BranchCode }),
            defaultSort: "FPJNo desc",
            columns: [
                { field: "FPJNo", title: "No Faktur Pajak" },
                {
                    field: "FPJDate", title: "Tgl Faktur Pajak",
                    template: "#= (FPJDate == undefined) ? '' : moment(FPJDate).format('DD MMM YYYY') #"
                },
                { field: "PickingSlipNo", title: "No Picking Slip" },
                {
                    field: "PickingSlipDate", title: "Tgl Picking Slip",
                    template: "#= (PickingSlipDate == undefined) ? '' : moment(PickingSlipDate).format('DD MMM YYYY') #"
                },
                { field: "InvoiceNo", title: "No Invoice" },
                {
                    field: "InvoiceDate", title: "Tgl Invoice",
                    template: "#= (InvoiceDate == undefined) ? '' : moment(InvoiceDate).format('DD MMM YYYY') #"
                },
                { field: "CustomerNameBill", title: "Pelanggan" },
            ]
        });
        lookup.dblClick(function (data) {
            if (data != null) {
                me.lookupAfterSelect(data);
                me.Apply();
            }
        });
    }

    function SetBranchInterface() {
        $http.post('tax.api/report/SetBranchInterface', me.data)
                        .success(function (e) {
                            if (e.success) {
                                localStorage.setItem('isBranch', e.isBranch);
                                localStorage.setItem('isMultiBranch', e.isMultiBranch);
                                localStorage.setItem('isFpjCentral', e.isFpjCentral);

                                if (e.isMultiBranch == true) {
                                    if (e.isBranch == true) {
                                        $('#BranchCode').attr('disabled', 'disabled');
                                        $('#btnBranchCode').attr('disabled', 'disabled');
                                        me.data.BranchCode = e.Branch.BranchCode;
                                        me.data.BranchName = e.Branch.BranchName;
                                    }
                                } else {
                                    $('#BranchCode').attr('disabled', 'disabled');
                                    $('#btnBranchCode').attr('disabled', 'disabled');
                                    me.data.BranchCode = e.Branch[0].BranchCode;
                                    me.data.BranchName = e.Branch[0].BranchName;
                                }
                            } else {
                                MsgBox(e.message, MSG_ERROR);
                            }
                        })
                        .error(function (e) {
                            MsgBox('Terjadi Kesalahan, Hubungi SDMS Support', MSG_ERROR);
                        });
    }

    $scope.$on('Service', function () {
        //alert('Service');
        $('#Address4').show();
        $('#PhoneNo').show();
        $('#HPNo').show();
        me.data = {};
        SetBranchInterface();
    });

    $scope.$on('Sparepart', function () {
        //alert('Sparepart');
        $('#Address4').hide();
        $('#PhoneNo').hide();
        $('#HPNo').hide();
        me.data = {};
        SetBranchInterface();
    });

    me.$watch('optionsTrans', function (newValue, oldValue) {
        me.init();
        if (newValue !== oldValue) {
            me.$broadcast(newValue);
        }
    });

    me.initialize = function () {
        SetBranchInterface();
    }

    me.start();
    me.optionsTrans = 'Service';
}

$(document).ready(function () {
    var options = {
        title: "Pemeliharaan Faktur Pajak Standar",
        xtype: "panels",
        toolbars: [
                    { name: "btnBrowse", text: "Browse", cls: "btn btn-info", icon: "icon-search", show: "!hasChanged || isInitialize", click: "browse()" },
                    { name: "btnSave", text: "Save", cls: "btn btn-success", icon: "icon-save", show: "(hasChanged || isLoadData) && !isInitialize", click: "save()", disable: "!isSave" },
                    { name: "btnCancel", text: "Cancel", cls: "btn btn-warning", icon: "icon-remove", show: "(hasChanged || isLoadData) && !isInitialize", click: "cancelOrClose()" },
        ],
        panels: [
            {
                name: "pnlA",
                items: [
                    {
                        type: "optionbuttons",
                        name: "optionsTrans",
                        model: "optionsTrans",
                        items: [
                            { name: "Service", text: "Service" },
                            { name: "Sparepart", text: "Sparepart" },
                        ]
                    },
                    {
                        type: "controls",
                        text: "Cabang",
                        cls: "span8",
                        items: [
                            { name: "BranchCode", cls: "span2", type: "popup", click: "BranchCode()" },
                            { name: "BranchName", cls: "span4", readonly: true },
                        ]
                    },
                    { name: "FPJNo", cls: "span3", text: "No. Faktur Pajak", type: "popup", click: "FPJNo()" },
                    { name: "FPJDate", cls: "span3", text: "Tgl.", readonly: true, type: "ng-datepicker" },
                    { name: "FPJGovNo", cls: "span3", text: "No Seri Pajak", readonly: true },
                    {
                        type: "controls",
                        text: "Pembayar",
                        cls: "span8",
                        items: [
                            { name: "CustomerCodeBill", cls: "span2", type: "popup", click: "CustomerCodeBill()" },
                            { name: "CustomerNameBill", cls: "span4", readonly: true },
                        ]
                    },
                    { name: "Address1", cls: "span6", text: "Alamat", readonly: true },
                    { name: "Address2", cls: "span6", text: "", readonly: true },
                    { name: "Address3", cls: "span6", text: "", readonly: true },
                    { name: "Address4", cls: "span6", text: "", readonly: true },
                    { name: "PhoneNo", cls: "span3", text: "Telepon", readonly: true },
                    { name: "HPNo", cls: "span3", text: "HP", readonly: true },
                    { name: "NPWPNo", cls: "span3", text: "NPWP", readonly: true },
                    { name: "NPWPDate", cls: "span3", text: "Tgl", type: "ng-datepicker", readonly: true },
                    { name: "SKPNo", cls: "span3", text: "NPPKP", readonly: true },
                    { name: "SKPDate", cls: "span3", text: "Tgl", type: "ng-datepicker", readonly: true },
                ]
            }
        ]
    };

    Wx = new SimDms.Widget(options);
    Wx.default = {};
    Wx.render(init);



    function init(s) {
        SimDms.Angular("TaxMaintenancefps");
    }

});