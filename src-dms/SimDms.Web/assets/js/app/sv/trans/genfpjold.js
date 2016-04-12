$(document).ready(function () {
    var isBranch;
    var options = {
        title: 'Generate Tax Invoice',
        xtype: 'panels',
        toolbars: [
            { name: "btnNew", text: "New", icon: "icon-file" },
            { name: 'btnBrowse', text: 'Browse', icon: 'icon-search' },
            { name: 'btnQuery', text: 'Query', icon: 'icon-search' },
            { name: 'btnGenerate', text: 'Generate', icon: 'icon-bolt' , cls: "hide"},
            { name: 'btnChange', text: 'Change', icon: 'icon-bug' },
        ],
        panels: [
            {
                title: 'Standard Tax Invoice',
                items: [
                    { name: 'FPJNo', text: 'FPJ No', placeHolder: 'FPS/XX/YYYYYY', cls: 'span4', readonly: true },
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
                        text: 'Customer', cls: 'span6 cust',
                        type: 'lookup',
                        name: 'CustomerCode',
                        namePlaceholder: 'Customer Code',
                        display: 'CustomerName',
                        displayPlaceholder: 'Customer Name',
                        btnName: 'btnCustBrowse'
                    },
                     { name: 'BranchFrom', text: 'Branch Code', cls: 'span4 branchrange hide', readonly: true },
                     { name: 'BranchTo', text: 's/d', cls: 'span4 branchrange hide', readonly: true },
                ]
            },
            {
                 title: "List",
                 xtype: "table",
                 tblname: "tblFPJ",
                 showcheckbox: true,
                 columns: [
                     { name: "RowNum", text: "No" },
                     { name: "BranchCode", text:"Branch Code" ,cls:"hide"},
                     { name: "InvoiceNo", text: "Invoice No" },
                     { name: "InvoiceDate", text: "Invoice Date", type: "date" },
                     { name: "JobOrderNo", text: "Job Order No" },
                     { name: "JobOrderDate", text: "Job Order Date", type: "date" },
                     { name: "DueDate", text: "Due Date", type: "date" },
                     { name: "TotalDPPAmt", text: "DPP" },
                     { name: "TotalPpnAmt", text: "PPN" },
                     { name: "TotalSrvAmt", text: "Total" },
                     { name: "JobType", text: "Job Type" },
                     { name: "PoliceRegNo", text: "Police Reg No" },
                     { name: "BasicModel", text: "Basic Model" },
                     { name: "ChassisCode", text: "Chassis Code" },
                     { name: "ChassisNo", text: "Chassis No" },
                     { name: "EngineCode", text: "Engine Code" },
                     { name: "EngineNo", text: "Engine No" },
                     { name: "TOPCode", cls: "hide" },
                     { name: "TOPDays", cls: "hide" },
                     { name: "FPJNo", cls: "hide" },
                     { name: "FPJDate", cls: "hide" },
                     { name: "IsPkp", cls: "hide" },
                     { name: "Pelanggan", text: "Customer" },
                     { name: "CustomerCode", cls: "hide" },
                     { name: "CustomerCodeBill", cls: "hide" }
                 ]
             },
        ],
    };
    var widget = new SimDms.Widget(options);

    widget.default = {};

    widget.render(function () {
        $.post('sv.api/genfpj/default', function (result) {
            widget.default = result;
            widget.populate(result);
            $('#FPJNo').val('FPS/XX/YYYYYY');
            isBranch = result.IsBranch;
            if (!result.IsBranch)
            {
                $('.branchrange').show();
                $('th[data-field="BranchCode"]').removeAttr('class data-class');
                $('#FPJNo').val('FPH/XX/YYYYYY');
            }
        });
    });

    widget.lookup.onDblClick(function (e, data, name) {
        widget.populate($.extend({}, widget.default, data));
        widget.lookup.hide();
        $('#btnGenerate').attr('disabled', 'disabled');
    });
    
    $('#btnCustBrowse').on('click', function () {
        widget.lookup.init({
            name: 'Select Customer',
            title: 'Select Customer',
            source: 'sv.api/grid/customers',
            columns: [
                { mData: 'CustomerCode', sTitle: 'Cust Code', sWidth: '110px' },
                { mData: 'CustomerName', sTitle: 'Cust Name' },
                { mData: 'Address', sTitle: 'Address' }
            ]
        });
        widget.lookup.show();
    });

    $("#btnBrowse").on("click", function () {
        widget.showToolbars(["btnNew", "btnBrowse", "btnQuery"]);
        var vb = $('#ViewBy').val();

        if (vb == '') {
            confirm('Silakan pilih dahulu kategori yang ingin ditampilkan !!!')
        }
        else {
            var docPrefix = vb == '0' ? 'INF' : vb == '1' ? 'INW' : 'INC';

            widget.lookup.init({
                name: "TaxInvoice",
                title: "Faktur Pajak List",
                source: "sv.api/grid/taxinvoice?docPrefix=" + docPrefix,
                columns: [
                    { mData: "FPJNo", sTitle: "No Faktur", sWidth: "110px" },
                    {
                        mData: "FPJDate", sTitle: "Tanggal Faktur", sWidth: "130px",
                        mRender: function (data, type, full) {
                            return moment(data).format('DD-MMM-YYYY');
                        }
                    },
                    { mData: "Invoice", sTitle: "No Invoice" },
                    { mData: "Customer", sTitle: "Pelanggan" },
                    { mData: "CustomerBill", sTitle: "Pembayar" }
                ],
            });
            widget.lookup.show();
        }
        //var dataMain = $(".main form").serializeObject();
        //console.log(dataMain);
    });

    widget.lookup.onDblClick(function (e,obj) {
        widget.lookup.hide();
        if (obj.FPJNo != null) {
            var data = { FPJNo: obj.FPJNo };
            loadData(data);
        }
        else
        {
            //widget.populateTable({ selector: "#tblFPJ", data: null });
        }
    });
    
    $('#ViewBy').on('change', function () {
        var vb = $('#ViewBy').val();
        isBranch ? $('#FPJNo').val('FPS/XX/YYYYYY') : $('#FPJNo').val('FPH/XX/YYYYYY');

        //$('#btnGenerate').removeClass('disabled');
        populateForm(vb);
    });

    $('.subtitle').on('dblclick', function () {        
        $('.pdifsc').hide();
        $('.claim').hide();
        $('.cust').hide();
    });

    function populateData(data, vb) {
        widget.post(isBranch ? "sv.api/Genfpj/getData" : vb == 0 ? "sv.api/Genfpj/getDataFPJHQ" : vb == 1 ? "sv.api/Genfpj/getDataClaimHQ" : "sv.api/Genfpj/getDataCustHQ", data, function (result) {
            if (result.success) {
                widget.populateTable({ selector: "#tblFPJ", data: result.list , selectable: true, multiselect: true});
                $('#btnGenerate').removeAttr('disabled');
            }
            else {
                confirm(result.message);

                //$("input[type=text]").not("#BranchCode,#BranchName,#JobOrderNo").val("");
            }
        });
    }

    function loadData(data) {
        widget.post(isBranch ? "sv.api/Genfpj/loadData" : "sv.api/Genfpj/loadHQData", data, function (result) {
            if (result.success) {
                widget.populateTable({ selector: "#tblFPJ", data: result.list, showcheckbox: false });
                $('#btnGenerate').attr('disabled', 'disabled');
                $('#FPJNo').val(data.FPJNo);
            }
            else {
                //$("input[type=text]").not("#BranchCode,#BranchName,#JobOrderNo").val("");
            }

        });
    }

    $('#btnNew').on('click', function (e) {
        widget.clearForm();
        widget.showToolbars(["btnNew", "btnBrowse", "btnQuery"]);
        widget.populateTable({ selector: "#tblFPJ", data: {} });
        widget.populate(widget.default);
    });

    $('#btnQuery').on('click', function () {
        widget.showToolbars(["btnNew", "btnBrowse", "btnQuery", "btnGenerate"]);
        var vb = $('#ViewBy').val();
        var dataMain = $(".main form").serializeObject();
        if (vb == '') {
            confirm('Silakan pilih dahulu kategori yang ingin ditampilkan !!!')
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

            isBranch ? $('#FPJNo').val('FPS/XX/YYYYYY') : $('#FPHNo').val('FPS/XX/YYYYYY');

            populateData(data, vb);
        }
    });

    $("#btnGenerate").on("click", function () {
        widget.showToolbars(["btnNew"]);
        var branch = $('.row_selected').map(function (idx, el) {
            var td = $(el).find('td');

            return td.eq(2).text();

        }).get();

        var invoice = $('.row_selected').map(function (idx, el) {
            var td = $(el).find('td');

            return td.eq(3).text();
        }).get();

        var model = {
            "BranchCode": branch,
            "InvoiceNo": invoice
        };

        $.ajax({
            type: 'POST',
            url: '/sv.api/Genfpj/Save',
            data: JSON.stringify(model),
            contentType: 'application/json; charset=utf8',
            dataType: 'json',
            error: function (err) {
                widget.alert("error - " + err.message);
            },
            success: function (e)
            {
                var data = { FPJNo: e.FPJNo };
                loadData(data);
            }
        });
    });

    $("#btnChange").on("click", function () {
        widget.post("sv.api/Genfpj/Change", function (result) {
            if (result.success) {
                window.location.reload();
            }
            else {
                //$("input[type=text]").not("#BranchCode,#BranchName,#JobOrderNo").val("");
            }

        });
    });

    $('#isSelect').on('Click', function (e) {
        console.log(e);
    });

    function populateForm(vb)
    {
        switch (vb) {
            case '0':
                $('.pdifsc').show();
                $('#PDIFSC').val(0);
                $('.claim').hide();
                $('#Claim').val('');
                $('#CustomerCode').text('');
                $('#CustomerName').text('');
                $('.cust').hide();
                break;
            case '1':
                $('.claim').show();
                $('#Claim').val(0);
                $('.pdifsc').hide();
                $('#PDIFSC').val('');
                $('#CustomerCode').text('');
                $('#CustomerName').text('');
                $('.cust').hide();
                break;
            case '2':
                $('.cust').show();
                $('#PDIFSC').val('');
                $('.pdifsc').hide();
                $('#Claim').val('');
                $('.claim').hide();
                break;
            case '':
                $('.pdifsc').hide();
                $('#PDIFSC').val('');
                $('.claim').hide();
                $('#Claim').val('');
                $('#CustomerCode').text('');
                $('#CustomerName').text('');
                $('.cust').hide();
                break;
        }
    }
    isBranch ? $('#FPJNo').val('FPS/XX/YYYYYY') : $('#FPJNo').val('FPH/XX/YYYYYY');
    $('.pdifsc').hide();
    $('.claim').hide();
    $('.cust').hide();

});