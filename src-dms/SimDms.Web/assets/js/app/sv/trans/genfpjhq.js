"use strict"

function svGenFPJHQController($scope, $http, $injector) {

    var me = $scope;

    $injector.invoke(BaseController, this, { $scope: me });

    me.default = function () {
        $http.post('sv.api/genfpjhq/default').
        success(function (e) {
            if (e.success) {
                me.data.BranchFrom = e.BranchFrom;
                me.data.BranchTo = e.BranchTo;
            }
            else {
                MsgBox(e.message, MSG_WARNING);
                $('#btnBrowse').attr('disabled', true);
                $('#btnQuery').attr('disabled', true);
                $('#ViewBy').attr('disabled', true);
            }
        });
    }

    $('#ViewBy').on('change', function () {
        var vb = $('#ViewBy').val();
        me.populateForm(vb);
    });

    me.populateForm = function (vb) {
        switch (vb) {
            case '0':
                $('.pdifsc').show();
                $('#PDIFSC').val(0);
                $('#Claim').val('');
                $('#CustomerCode, #CustomerName').val('');
                $('.cust, .claim').hide();
                break;
            case '1':
                $('.claim').show();
                $('#Claim').val(0);
                $('.pdifsc, .cust').hide();
                $('#PDIFSC').val('');
                $('#CustomerCode, #CustomerName').val('');
                break;
            case '2':
                $('.cust').show();
                $('#PDIFSC,#Claim').val('');
                $('.claim,.pdifsc').hide();
                break;
            case '':
                $('#PDIFSC, #Claim').val('');
                $('#CustomerCode, #CustomerName').val('');
                $('.pdifsc, .claim, .cust').hide();
                break;
        }
    }

    me.custFPJGLookup = function () {
        var lookup = Wx.blookup({
            name: "FPJHQCustLookup",
            title: "Select Customer",
            manager: svServiceManager,
            query: "FPJHQCustLookup",
            defaultSort: "CustomerCode asc",
            columns: [
                { field: 'CustomerCode', title: 'Customer Code', width: 130 },
                { field: 'CustomerName', title: 'Customer Name' ,width: 220},
                { field: 'Address', title: 'Address' },
            ]
        });
        lookup.dblClick(function (data) {
            $('#CustomerCode').val(data.CustomerCode);
            $('#CustomerName').val(data.CustomerName);
        });
    }

    me.browse = function () {
        var lookup = Wx.blookup({
            name: "TaxInvoice",
            title: "Faktur Pajak",
            manager: svServiceManager,
            query: "TaxInvoiceHQ",
            columns: [
                { field: "FPJNo", title: 'No. Faktur', width: 110 },
                {
                    field: "FPJDate", title: "Tgl. Faktur", width: 130,
                    template: "#= (FPJDate == undefined) ? '' : moment(FPJDate).format('DD MMM YYYY') #"
                },
                { field: "Invoice", title: "No Invoice" },
                { field: "Customer", title: "Pelanggan" },
                { field: "CustomerBill", title: "Pembayar" }
            ]
        });
        lookup.dblClick(function (data) {
            me.data = data;
            me.data.BranchFrom = data.BranchStart;
            me.data.BranchTo = data.BranchEnd;
            me.loadDetail(me.data);
        });
    }

    me.loadDetail = function (data) {
        $http.post('sv.api/genfpjhq/loaddata', {fpjNo : data.FPJNo, branchFrom : data.BranchFrom, branchTo : data.BranchTo}).
        success(function (e) {
            if (e.docPrefix != "") {
                $('#ViewBy').val(e.docPrefix);
            }
            me.loadTableData(me.grid1, e.data);
            $('#btnGenerate').attr('disabled', 'disabled');
        }).
        error(function (e) {
            MsgBox('Terjadi Kesalahan, Hubungi SDMS Support');
        })
        me.data.FPJNo = data.FPJNo;
        me.Apply();
    }

    me.query = function () {

        var vb = $('#ViewBy').val();
        var dataMain = $(".main form").serializeObject();
        if (vb == '') {
            MsgBox('Silakan pilih dahulu kategori yang ingin ditampilkan !!!')
        }
        else {
            var data = {
                docPrefix: vb == '' ? '' : vb == '0' ? 'INF' : vb == '1' ? 'INW' : 'INC',
                isPdi: vb == '0' && dataMain.PDIFSC == 0 ? 1 : 0,
                isFsc: vb == '0' && dataMain.PDIFSC == 1 ? 1 : 0,
                isFscCampaign: vb == '0' && dataMain.PDIFSC == 2 ? 1 : 0,
                isSprClaim: dataMain.Claim == '' ? 0 : dataMain.Claim,
                CustBill: vb == '0' ? '' : vb == '1' ? '' : dataMain.CustomerCode,
            };

            var url = vb == 0 ? "sv.api/GenfpjHQ/getDataFPJHQ" : vb == 1 ? "sv.api/GenfpjHQ/getDataClaimHQ" : "sv.api/GenfpjHQ/getDataCustHQ";

            $http.post(url, data)
            .success(function (e) {
               
                    me.loadTableData(me.grid1, e);
                    if (e != null) { $('#btnGenerate').removeAttr('disabled'); }
                    me.detail = e;
               
            })
            .error(function (e) {
                MsgBox('Terjadi Kesalahan, Hubungi SDMS Support');
            });

        }
    }

    me.grid1 = new webix.ui({
        container: "wxtaxinvoice",
        view: "wxtable", css: "alternating",
        scrollX: true,
        scrollY: true,
        autoHeight: false,
        height: 330,
        checkboxRefresh: true,
        columns: [
            { id: "IsSelected", header: { content: "masterCheckbox", contentId: "chkSelect" }, template: "{common.checkbox()}", width: 40 },
            { id: "RowNum", header: "No.", width: 40 },
            { id: "InvoiceNo", header: "No. Faktur", width: 120 },
            { id: "InvoiceDate", header: "Tgl. Faktur", format: me.dateFormat, width: 130 },
            { id: "JobOrderNo", header: "No. SPK", width: 120 },
            { id: "JobOrderDate", header: "Tgl. SPK", format: me.dateFormat, width: 130 },
            { id: "DueDate", header: "Jatuh Tempo", format: me.dateFormat, width: 130 },
            { id: "TotalDPPAmt", header: "DPP", width: 100, format: me.intFormat },
            { id: "TotalPpnAmt", header: "PPN", width: 100, format: me.intFormat },
            { id: "TotalSrvAmt", header: "Total", width: 100, format: me.intFormat },
            { id: "JobType", header: "Jenis Pekerjaan", width: 120 },
            { id: "PoliceRegNo", header: "No. Polisi", width: 120 },
            { id: "BasicModel", header: "Basic Model", width: 120 },
            { id: "ChassisCode", header: "Kode Rangka", width: 120 },
            { id: "ChassisNo", header: "No Rangka", width: 120 },
            { id: "EngineCode", header: "Kode Mesin", width: 120 },
            { id: "EngineNo", header: "No. Mesin", width: 120 },
            { id: "Pelanggan", header: "Pelanggan", width: 300 },
            { id: "CustomerCode" },
            { id: "CustomerCodeBill" },
            { id: "BranchCode"},
        ]
    });

    me.generate = function () {
        var datDetail = [];
        $.each(me.detail, function (key, val) {
            if (val["IsSelected"] == 1) {
                datDetail.push({BranchCode: val["BranchCode"], InvoiceNo: val["InvoiceNo"] });
            }
        })

        var dat = {};
        dat["model"] = datDetail;

        $http.post('sv.api/genfpjhq/save', JSON.stringify(dat)).
            success(function (e) {
                if(e.success)
                {
                    Wx.Success(e.message);
                    me.loadDetail(e.data);
                }
                else {
                    MsgBox(e.message);
                }
            }).error(function (e) {
                MsgBox('Terjadi Kesalahan, Hubungi SDMS Support');
            });
    }

    webix.event(window, "resize", function () {
        me.grid1.adjust();
    });

    me.initialize = function () {
        me.default();
        me.detail = {};
        me.clearTable(me.grid1);
        $('.pdifsc, .claim, .cust').hide();
        $('#btnBrowse').removeAttr('disabled');
        $('#btnQuery').removeAttr('disabled');
        $('#ViewBy').removeAttr('disabled');
        me.grid1.hideColumn('CustomerCode');
        me.grid1.hideColumn('CustomerCodeBill');
        me.grid1.hideColumn('BranchCode');
    }

    me.start();
}

$(document).ready(function () {
    var options = {
        title: "Generate Faktur Pajak Gabungan (HQ)",
        xtype: "panels",
        toolbars: [
            { name: "btnNew", text: "New", cls: "btn btn-success", icon: "icon-refresh", click: "cancelOrClose()" },
            { name: "btnBrowse", text: "Browse", cls: "btn btn-info", icon: "icon-search", click: "browse()" },
        ],
        panels: [
            {
                name: "pnlbutton",
                items: [{
                    type: "buttons",
                    items: [
                        { name: "btnQuery", text: "Query", cls: "btn btn-info", icon: "icon-search", click: "query()" },
                        { name: "btnGenerate", text: "Generate", cls: "btn btn-info", icon: "icon-bolt", click: "generate()" , disable : true}
                    ]
                },
                ]
            },
            {
                name: "pnlFPJ",
                text: "Faktur Pajak Standard",
                items: [
                    { name: "FPJNo", text: "No. Faktur Pajak", cls: "span4", placeHolder: "FPS/XX/YYYYYY", readonly: true, style: "padding-right:1px" },
                    {
                        name: 'ViewBy', text: 'View By', cls: 'span4 full', type: 'select',
                        items: [
                            { value: '0', text: 'PDI / FSC' },
                            { value: '1', text: 'Claim' },
                            { value: '2', text: 'Customer' }
                        ]
                    },
                    {
                        name: 'PDIFSC', text: 'PDI / FSC', cls: 'span4 full pdifsc', type: 'select',
                        items: [
                            { value: '0', text: 'PDI' },
                            { value: '1', text: 'FSC' },
                            { value: '2', text: 'FSC Campaign' }
                        ]
                    },
                    {
                        name: 'Claim', text: 'Claim', cls: 'span4 full claim', type: 'select',
                        items: [
                            { value: '0', text: 'Service Claim' },
                            { value: '1', text: 'Sparepart Claim' }
                        ]
                    },
                    {
                        text: "Customer",
                        type: "controls",
                        cls: "cust",
                        items: [
                            { name: "CustomerCode", cls: "span2", placeHolder: "Customer Code", readonly: true, type: "popup", click:"custFPJGLookup()" },
                            { name: "CustomerName", cls: "span6", placeHolder: "Customer Name", readonly: true }
                        ]
                    },
                    { name: 'BranchFrom', text: 'Branch Code', cls: 'span4', readonly: true },
                    { name: 'BranchTo', text: 's/d', cls: 'span4', readonly: true },
                    {
                        name: "wxtaxinvoice",
                        title: "List",
                        type: "wxdiv"
                    },
                ]
            }
        ]
    };

    Wx = new SimDms.Widget(options);
    Wx.default = {};
    Wx.render(init);

    function init(s) {
        SimDms.Angular("svGenFPJHQController");
    }
});