$(document).ready(function () {
    var options = {
        title: "Process Invoice",
        xtype: "panels",
        toolbars: [
            { name: "btnCreate", text: "New", icon: "icon-file", cls: "btn" },
            { name: "btnBrowse", text: "Browse", icon: "icon-search", cls: "btn" },
            { name: "btnPrint", text: "Print", icon: "icon-print", cls: "btn", disable: true },
            { name: "btnProcess", text: "Process Invoice", icon: "icon-bolt", cls: "btn" },
            { name: "btnProcessPrint", text: "Print Invoice", icon: "icon-bolt", cls: "btn" },
        ],
        panels: [
            {
                name: "pnlInfoSPK",
                items: [
                    {
                        type: "label", name: "TotalSrvAmt", cls: "span3 right", style: "font-size:36px;color:blue;text-align:center"
                    },
                    {
                        type: "label", name: "ServiceStatusDesc", cls: "span5 right", style: "font-size:36px;color:blue;text-align:center"
                    }
                ]
            },
            {
                name: "pnlServiceInfo",
                title: "Service Information",
                items: [
                    {
                        text: "Branch",
                        type: "controls",
                        items: [
                            { name: "BranchCode", cls: "span2", placeHolder: "Branch Code", readonly: true },
                            { name: "BranchName", cls: "span6", placeHolder: "Branch Name", readonly: true }
                        ]
                    },
                    { name: "InvoiceNo", text: "Invoice No", cls: "span4 full", type: "popup", readonly: false },
                    { name: "JobOrderNo", text: "SPK No", placeHolder: "XXX/YY/99999", cls: "span4", type: "popup", readonly: false },
                    { name: "JobOrderDate", text: "SPK Date", cls: "span4", readonly: true, type: "date" },
                    //{ name: "ServiceStatusDesc", text: "Service Status", readonly: true },
                    //{ name: "TotalSrvAmt", text: "Total Amount", readonly: true, cls: "number span4 full" },
                    { name: "Signer", text: "Signer", type: "popup", cls: "span4", readonly: true },
                    { name: "SignerTitle", text: "Title", cls: "span4 hide" },
                    {
                        name: "Recommendation", text: "Recommendation", type: "textarea", readonly: true
                    },
                ]
            },
            {
                xtype: "tabs",
                name: "tabpage1",
                items: [
                    { name: "CustVehicle", text: "Customer & Vehicle" },
                    { name: "TaskPart", text: "Task & Part" },
                ]
            },
            {
                name: "CustVehicle",
                cls: "tabpage1 CustVehicle",
                title: "Customer & Vehicle",
                items: [
                    {
                        text: "Customer Bill",
                        type: "controls",
                        items: [
                            { name: "CustomerCode", cls: "span2", placeHolder: "Cust Code", readonly: true },
                            { name: "CustomerName", cls: "span6", placeHolder: "Cust Name", readonly: true },
                        ]
                    },
                    { name: "PoliceRegNo", text: "Police Reg No", readonly: true },
                    { name: "CustAddr1", text: "Address", maxlength: 100, readonly: true },
                    { name: "CustAddr2", text: "", maxlength: 100, readonly: true },
                    { name: "CustAddr3", text: "", maxlength: 100, readonly: true },
                    {
                        text: "City",
                        type: "controls",
                        items: [
                            { name: "CityCode", cls: "span2", placeHolder: "City Code", readonly: true },
                            { name: "CityName", cls: "span6", placeHolder: "City Name", readonly: true },
                        ]
                    },
                    {
                        text: "Phone",
                        type: "controls",
                        items: [
                            { cls: "span2", placeHolder: "Telephone", readonly: true },
                            { cls: "span3", placeHolder: "Fax", readonly: true },
                            { cls: "span3", placeHolder: "HP", readonly: true },
                        ]
                    },
                    { name: "ServiceBookNo", text: "Service Book No", readonly: true },
                    { name: "BasicModel", text: "Basic Model", cls: "span4", readonly: true },
                    { name: "TransType", text: "Trans Type", cls: "span4", readonly: true },
                    { name: "ChassisCode", text: "Chassis Code", cls: "span4", readonly: true },
                    { name: "ChassisNo", text: "Chassis No", cls: "span4", readonly: true },
                    { name: "EngineCode", text: "Engine Code", cls: "span4", readonly: true },
                    { name: "EngineNo", text: "Engine No", cls: "span4", readonly: true },
                    { name: "ColorCode", text: "Color", cls: "span4", readonly: true },
                    { name: "Odometer", text: "Odometer", cls: "span4" },
                ]
            },
            {
                cls: "tabpage1 CustVehicle",
                title: "Customer Bill",
                items: [
                    { name: "InsurancePayFlag", text: "Insurance", type: "switch", float: "left", cls: "span4" },
                    { name: "InsuranceOwnRisk", text: "Own Risk", cls: "span4", readonly: true },
                    { name: "InsuranceNo", text: "Insurance No", cls: "span4", readonly: true },
                    { name: "InsuranceJobOrderNo", text: "Insurance JobOrder No", cls: "span4", readonly: true },
                    {
                        text: "Customer",
                        type: "controls",
                        items: [
                            { name: "CustomerCodeBill", cls: "span2", placeHolder: "Cust Code", readonly: true, type: "popup" },
                            { name: "CustomerNameBill", cls: "span6", placeHolder: "Cust Name", readonly: true },
                        ]
                    },
                    { name: "CustAddr1", text: "Address", maxlength: 100, readonly: true },
                    { name: "CustAddr2", text: "", maxlength: 100, readonly: true },
                    { name: "CustAddr3", text: "", maxlength: 100, readonly: true },
                    {
                        text: "City",
                        type: "controls",
                        items: [
                            { name: "CityCodeBill", cls: "span2", placeHolder: "City Code", readonly: true },
                            { name: "CityNameBill", cls: "span6", placeHolder: "City Name", readonly: true },
                        ]
                    },
                    {
                        text: "Phone",
                        type: "controls",
                        items: [
                            { name: "PhoneNo", cls: "span2", placeHolder: "Telephone", readonly: true },
                            { name: "FaxNo", cls: "span3", placeHolder: "Fax", readonly: true },
                            { name: "HPNo", cls: "span3", placeHolder: "HP", readonly: true },
                        ]
                    },
                    {
                        text: "Discount (%)",
                        type: "controls",
                        items: [
                           { name: "LaborDiscPct", cls: "span2 number", placeHolder: "Task", readonly: true },
                           { name: "PartsDiscPct", cls: "span3 number", placeHolder: "Part", readonly: true },
                           { name: "MaterialDiscPct", cls: "span3 number", placeHolder: "Material", readonly: true },
                        ]
                    },
                    { name: "IsPPN", text: "PPN", type: "switch", float: "left" },
                ]
            },
            {

                cls: "tabpage1 CustVehicle",
                title: "Contract & Club",
                items: [
                    { name: "ContractNo", text: "Contract No", cls: "span4 full", readonly: true },
                    { name: "ContractEndPeriod", text: "Contract End Period", cls: "span4 full", readonly: true, type: "date" },
                    { name: "ContractStatus", text: "Contract Status", cls: "span4 full", readonly: true },
                    { name: "ClubCode", text: "Club No", cls: "span4 full", readonly: true },
                    { name: "ClubEndPeriod", text: "Club End Period", cls: "span4 full", readonly: true, type: "date" },
                    { name: "ClubStatus", text: "Club Status", cls: "span4 full", readonly: true },
                ]
            },
            {
                cls: "tabpage1 TaskPart",
                title: "Job Request",
                items: [
                    { name: "ServiceRequestDesc", text: "Job Request", type: "textarea", readonly: true },
                    {
                        text: "Job Type",
                        type: "controls",
                        items: [
                           { name: "JobType", cls: "span2", placeHolder: "Code", readonly: true },
                           { name: "JobTypeDesc", cls: "span6", placeHolder: "Description", readonly: true },
                        ]
                    },
                    { name: "ConfirmChangingPart", text: "Allow Change Part", type: "switch", float: "left" },
                    {
                        text: "Service Advisor (SA)",
                        type: "controls",
                        items: [
                           { name: "ForemanID", cls: "span2", placeHolder: "Code", readonly: true },
                           { name: "ForemanName", cls: "span6", placeHolder: "Name", readonly: true },
                        ]
                    },
                    {
                        text: "Foreman (FM)",
                        type: "controls",
                        items: [
                             { name: "MechanicID", cls: "span2", placeHolder: "Code", readonly: true },
                             { name: "MechanicName", cls: "span6", placeHolder: "Name", readonly: true },
                        ]
                    },
                ]
            },
            {
                title: "Detail Job",
                xtype: "table",
                name: "tblTaskPart",
                cls: "tabpage1 TaskPart",
                columns: [
                    { name: "BillTypeDesc", text: "Ditanggung Oleh" },
                    { name: "TypeOfGoodsDesc", text: "Jenis Item" },
                    { name: "TaskPartNo", text: "Task/Part" },
                    { name: "SupplyQty", text: "Supply Qty" },
                    { name: "Price", text: "Price" },
                    { name: "DiscPct", text: "Discount", cls: "right", width: 80 },
                    { name: "PriceNet", text: "Net Price", cls: "right", width: 120 },
                    { name: "SupplySlipNo", cls: "Supply Slip No" },
                    { name: "TaskPartDesc", cls: "Description" },
                ]
            },
            {
                cls: "tabpage1 TaskPart summary",
                items: [
                   { name: "LaborDppAmt", text: "DPP - Jasa", cls: "span6 number", readonly: true },
                   { name: "PartsDppAmt", text: "DPP - Part", cls: "span6 number", readonly: true },
                   { name: "MaterialDppAmt", text: "DPP - Material", cls: "span6 number", readonly: true },
                   { name: "TotalDppAmt", text: "Total DPP", cls: "span6 indent number", readonly: true },
                   { name: "TotalPpnAmt", text: "Total PPN", cls: "span6 indent number", readonly: true },
                   { name: "TotalSrvAmt", text: "Total Amount", cls: "span6 indent number", readonly: true },
                ]
            },
        ],
    }
    var widget = new SimDms.Widget(options);
    widget.default = {};
    widget.render(function () {
        $.post("sv.api/invoice/default", function (result) {
            widget.default = result;
            widget.populate(result);
        });
        $('#ServiceStatusDesc').html("NEW");
        $('#TotalSrvAmt').html("0");

        var date = new Date();
        var today = moment(date).format("YYYY-MM-DD");
        console.log(today);
    });

    $("#btnBrowse, #btnInvoiceNo").on("click", function () {
        var lookup = widget.klookup({
            name: "Invoice",
            title: "Invoice",
            url: "sv.api/invoice/LookUpInvoice", //"sv.api/grid/invoiceview",
            serverBinding: true,
            pageSize: 10,
            filterable: true,
            sort: [
		        { 'field': 'InvoiceDate', 'dir': 'desc' },
                { 'field': 'InvoiceNo', 'dir': 'desc' }
            ],
            filters: [
                { name: "InvoiceNo", text: "Invoice No.", cls: "span4" },
                { name: "FPJNo", text: "FPJ No.", cls: "span4" },
                { name: "JobOrderNo", text: "Job Order No.", cls: "span4" }
            ],
            columns: [
		        { field: "InvoiceNo", title: "Invoice No", width: 130 },
		        {
		            field: "InvoiceDate", title: "Tanggal Faktur",
		            template: "#= (InvoiceDate == undefined) ? '' : moment(InvoiceDate).format('DD MMM YYYY') #", width: 140
		        },
		        { field: "FPJNo", title: "FPJ No", width: 150 },
		         {
		             field: "FPJDate", title: "FPJ Date",
		             template: "#= (FPJDate == undefined) ? '' : moment(FPJDate).format('DD MMM YYYY') #", width: 130
		         },
		         { field: "JobOrderNo", title: "JobOrder No", width: 160 },
		         {
		             field: "JobOrderDate", title: "JobOrder Date",
		             template: "#= (JobOrderDate == undefined) ? '' : moment(JobOrderDate).format('DD MMM YYYY') #", width: 130
		         },
		         { field: "PoliceRegNo", title: "Police No.", width: 160 },
		         { field: "ServiceBookNo", title: "Service Book No", width: 160 },
		         { field: "JobType", title: "JobType", width: 160 },
		         { field: "ChassisCode", title: "Chassis Code", width: 160 },
		         { field: "ChassisNo", title: "Chassis No", width: 160 },
		            { field: "EngineCode", title: "Engine Code", width: 160 },
		         { field: "EngineNo", title: "Engine No", width: 160 },
		         { field: "BasicModel", title: "Basic Model", width: 160 },
		         { field: "Customer", title: "Customer", width: 480 },
		         { field: "CustomerBill", title: "Customer Bill", width: 480 }
            ]
        });

        lookup.dblClick(function (data) {
            if (data != null) {
                $.post("sv.api/invoice/get", { joborderno: data.JobOrderNo, invoiceNomor: data.InvoiceNo }, function (result) {
                    if (result.success) {
                        result.list["IsPPN"] = result.list.TaxCode == "PPN" ? true : false;
                        $('#btnProcess').attr('disabled', 'disabled');
                        $('#btnProcessPrint').attr('disabled', 'disabled');
                        $('#Recommendation').attr('readonly', true)

                        widget.populate(result.list);
                        $('#InvoiceNo').val(data.InvoiceNo);
                        $('#btnPrint').removeAttr('disabled');
                        $('#ServiceStatusDesc').html(result.list.ServiceStatusDesc);
                        $('#TotalSrvAmt').html(number_format(result.list.TotalSrvAmt));
                        
                        //console.log(result.list.Remarks);

                        $('#Recommendation').val(result.list.Remarks);

                        $.post("sv.api/invoice/getTable", { invoiceno: data.InvoiceNo, joborderno: result.list.JobOrderNo, jobtype: result.list.JobType, serviceno: result.list.ServiceNo }, function (result) {
                            widget.populateTable({ selector: "#tblTaskPart", data: result.list });
                        });
                    }
                    else {
                        widget.alert(result.message);
                        $('#btnProcess').removeAttr('disabled');
                        $('#btnProcessPrint').removeAttr('disabled');
                        $('#Recommendation').removeAttr('readonly');
                    }
                });
            }
        });
    });

    $('#btnSigner').on('click', function (e) {
        var lookup = widget.klookup({
            name: "Signer",
            title: "Signer",
            url: "sv.api/grid/SignaturesForInvoice",
            serverBinding: true,
            sort: [
                { 'field': 'SignName', 'dir': 'asc' },
		        { 'field': 'TitleSign', 'dir': 'asc' }
            ],
            pageSize: 10,
            columns: [
		         { field: "SignName", title: "Nama", width: 230 },
		         { field: "TitleSign", title: "Jabatan", width: 170 }
            ]
        });

        lookup.dblClick(function (data) {
            if (data != null) {
                $('#Signer').val(data.SignName);
                $('#SignerTitle').val(data.TitleSign);
            }
        });

        //$('#btnProcess').removeAttr('disabled');
        //$('#btnProcessPrint').removeAttr('disabled');
        //$('#Recommendation').removeAttr('readonly');
    });

    $('#btnJobOrderNo').on('click', function (e) {
        $('#Signer').val("");
        $('#Recommendation').val("");

        var lookup = widget.klookup({
            name: "JobOrder",
            title: "Job Order",
            url: "sv.api/grid/JobOrdersByStatusForInvoice",
            params: { serviceStatus: 5 },
            serverBinding: true,
            pageSize: 10,
            sort: [
                { 'field': 'JobOrderNo', 'dir': 'desc' },
                { 'field': 'JobOrderDate', 'dir': 'desc' }
            ],
            filters: [
                {
                    text: "Semua Status",
                    type: "controls",
                    cls: "span8",
                    items: [
                        {
                            name: "ShowAll", type: "select", text: "", cls: "span2", items: [
                                { value: "0", text: "Ya" },
                                { value: "1", text: "Tidak", selected: 'selected' }
                            ]
                        }
                    ]
                }
            ],
            columns: [
                { field: "InvoiceNo", title: "No. Faktur", width: 160 },
                { field: "JobOrderNo", title: "Job Order No", width: 160 },
                {
                    field: "JobOrderDate", title: "Job Order Date",
                    template: "#= (JobOrderDate == undefined) ? '' : moment(JobOrderDate).format('DD MMM YYYY') #", width: 130
                },
                { field: "PoliceRegNo", title: "Police Reg", width: 160 },
                { field: "ServiceBookNo", title: "Service Book", width: 160 },
                { field: "BasicModel", title: "Model", width: 160 },
                { field: "TransmissionType", title: "Transmission Type", width: 150 },
                { field: "ChassisCode", title: "Chassis Code", width: 160 },
                { field: "EngineCode", title: "EngineCode", width: 160 },
                { field: "ColorCode", title: "Color", width: 160 },
                { field: "Customer", title: "Customer", width: 260 },
                { field: "CustomerBill", title: "Customer Bill", width: 260 },
                { field: "JobType", title: "Job Type", width: 160 },
                { field: "ForemanID", title: "Foreman", width: 200 },
                { field: "MechanicID", title: "Mechanic", width: 160 },
                { field: "ServiceStatusDesc", title: "Status", width: 160 }
            ]
        });

        lookup.dblClick(function (data) {
            if (data != null) {
                $('#JobOrderNo').val(data.JobOrderNo);

                //get(data.JobOrderNo)
                //$.post("sv.api/invoice/get", { joborderno: data.JobOrderNo }, function (result) {
                //    if (result.success) {
                //        result.list["IsPPN"] = result.list.TaxCode == "PPN" ? true : false;
                //        widget.populate(result.list);
                //        $('#InvoiceNo').val(data.InvoiceNo);
                //        $('#btnPrint').removeAttr('disabled');
                //        $('#ServiceStatusDesc').html(result.list.ServiceStatusDesc);
                //        $('#TotalSrvAmt').html(number_format(result.list.TotalSrvAmt));

                //        if (result.list.Remarks == null) {
                //            $('#Recommendation').val("TERIMA KASIH ATAS KEPERCAYAAN ANDA TELAH MELAKUKAN PERAWATAN DAN PERBAIKAN DI BERES KAMI.\nSERVIS BERIKUT: \nSARAN SERVIS: \n \nTERIMA KASIH")
                //        } else {
                //            $('#Recommendation').val(result.list.Remarks)
                //        }

                //        $.post("sv.api/invoice/getTable", { invoiceno: data.InvoiceNo, joborderno: result.list.JobOrderNo, jobtype: result.list.JobType, serviceno: result.list.ServiceNo }, function (result) {
                //            widget.populateTable({ selector: "#tblTaskPart", data: result.list });
                //        });
                //    }
                //    else {
                //        widget.alert(result.message)
                //    }
                //});
            }

            $('#btnProcess').removeAttr('disabled');
            $('#btnProcessPrint').removeAttr('disabled');
            $('#Recommendation').removeAttr('readonly');
            get($('#JobOrderNo').val(), data.InvoiceNo);
        });
    });

    $('#btnProcess').on('click', function (e) {

        var date = new Date();
        var today = moment(date).format("YYYY-MM-DD");

        if ($('#Signer').val() == "") {
            widget.alert('Signer Harus Diisi');
            return;
        }
        else 
            $('#btnProcess').attr('disabled', 'disabled');
            $('.page > .ajax-loader').show()
            $.post('sv.api/invoice/ProcessInvoice', { jobOrderNo: $('#JobOrderNo').val(), remark: $('#Recommendation').val(),Today:today }, function (e) {
                if (e.Success) {
                    $('#InvoiceNo').val(e.invNo);
                    get($('#JobOrderNo').val(), $('#InvoiceNo').val());
                    widget.Success(e.Message);
                    $('#btnProcessPrint').attr('disabled', 'disabled');
                    $('.page > .ajax-loader').hide()

                }
                else {
                    widget.Error(e.Message);
                    $('.page > .ajax-loader').hide()
                    return;
                }
            })
    });

    $('#btnProcessPrint').on('click', function (e) {
        var date = new Date();
        var today = moment(date).format("YYYY-MM-DD");

        if ($('#Signer').val() == "") {
            widget.alert('Signer Harus Diisi');
            return;
        }
        else {
            $.post('sv.api/invoice/ProcessInvoice', { jobOrderNo: $('#JobOrderNo').val(), remark: $('#Recommendation').val(),Today:today }, function (e) {
                if (e.Success) {
                    $('#InvoiceNo').val(e.invNo);

                    var ReportId = "SvRpTrn004";
                    var par = [
                        'producttype',
                        $('#InvoiceNo').val(),
                        $('#InvoiceNo').val()
                    ];
                    var rparam = $("#Signer").val() + "," + $('#SignerTitle').val();

                    widget.showPdfReport({
                        textprint: true,
                        id: ReportId,
                        pparam: par,
                        rparam: rparam,
                        type: "devex"
                    });

                    widget.populateTable({ selector: "#tblTaskPart", data: e.grid });
                    get($('#JobOrderNo').val());

                    widget.Success(e.Message);

                }
                else {
                    widget.Error(e.Message);
                }

                $(".ajax-loader").hide();
            });
        }
    });

    function get(jobOrderNo, invoiceNo) {
        $.post("sv.api/invoice/get", { joborderno: jobOrderNo, invoiceNomor: invoiceNo }, function (result) {
            if (result.success) {
                result.list["IsPPN"] = result.list.TaxCode == "PPN" ? true : false;
                //if (name == 'Invoice') {
                //    $('#btnProcess').attr('disabled', 'disabled');
                //    $('#btnProcessPrint').attr('disabled', 'disabled');

                //    $('#Recommendation').attr('readonly', true);
                //}
                //else {
                //    $('#btnProcess').removeAttr('disabled');
                //    $('#btnProcessPrint').removeAttr('disabled');
                //    $('#Recommendation').removeAttr('readonly');
                //}

                if ($("#InvoiceNo").val() === "") {
                    $('#btnProcess').removeAttr('disabled');
                    $('#btnProcessPrint').removeAttr('disabled');
                    $('#Recommendation').removeAttr('readonly');
                }
                else {
                    $('#btnProcess').attr('disabled', 'disabled');
                    $('#btnProcessPrint').attr('disabled', 'disabled');
                    $('#Recommendation').attr('readonly', true);
                }

                widget.populate(result.list);
                $('#btnPrint').removeAttr('disabled');
                $('#ServiceStatusDesc').html(result.list.ServiceStatusDesc);
                $('#TotalSrvAmt').html(number_format(result.list.TotalSrvAmt));

                if (result.list.Remarks == null) {
                    $('#Recommendation').val("TERIMA KASIH ATAS KEPERCAYAAN ANDA TELAH MELAKUKAN PERAWATAN DAN PERBAIKAN DI BERES KAMI.\nSERVIS BERIKUT: \nSARAN SERVIS: \n \nTERIMA KASIH")
                } else {
                    $('#Recommendation').val(result.list.Remarks)
                }

                $.post("sv.api/invoice/getTable", { invoiceno: result.list.InvoiceNo, joborderno: result.list.JobOrderNo, jobtype: result.list.JobType, serviceno: result.list.ServiceNo }, function (result) {
                    widget.populateTable({ selector: "#tblTaskPart", data: result.list });
                });
            }
            else {
                widget.alert(result.message)
            }
        });
    }

    $('#btnPrint').on('click', function (e) {
        if ($('#Signer').val() == "") {
            widget.alert('Signer Harus Diisi');
        }
        else {
            var ReportId = "SvRpTrn004";
            var par = [
                'producttype',
                $('#InvoiceNo').val(),
                $('#InvoiceNo').val()
            ];
            var rparam = $("#Signer").val() + "," + $('#SignerTitle').val();

            widget.showPdfReport({
                textprint: true,
                id: ReportId,
                pparam: par,
                rparam: rparam,
                type: "devex"
            });
        }
    });

    $('#btnProcess,#btnPrint,#btnProcessPrint').attr('disabled', 'disabled');
    $('input[type="radio"]').attr('disabled', 'disabled');

    $('#btnCreate').on('click', function (e) {
        widget.clearForm();
        widget.populateTable({ selector: "#tblTaskPart", data: [] });
        $.post("sv.api/invoice/default", function (result) {
            widget.default = result;
            widget.populate(result);
        });
        $('#ServiceStatusDesc').html("NEW");
        $('#TotalSrvAmt').html("0");

        $('#btnProcess,#btnPrint,#btnProcessPrint').attr('disabled', 'disabled');
        $('input[type="radio"]').attr('disabled', 'disabled');
    });

    $("#JobOrderNo").on("keypress", function (e) {
        if (e.keyCode == 13) {
            $("#Signer").focus();
        }
    });

    $('#JobOrderNo').on('blur', function () {
        var spkNo = $('#JobOrderNo').val();
        if (spkNo == undefined || spkNo == null || spkNo=="") {
            return;
        }
        if (spkNo != null || spkNo != undefined)
        {
			//$.post('sv.api/invoice/GetDataByJobOrder', { SpkNo: spkNo }).success(function (result) {
                //if (result.success) {
                    $.post('sv.api/invoice/get', { joborderno: spkNo }).success(function (result) {
                        if (result.success) {
                            result.list["IsPPN"] = result.TaxCode == "PPN" ? true : false;
                            widget.populate(result.list);
                            $('#InvoiceNo').val(result.list.InvoiceNo);
                            $('#btnPrint').removeAttr('disabled');
                            $('#ServiceStatusDesc').html(result.list.ServiceStatusDesc);
                            $('#TotalSrvAmt').html(number_format(result.list.TotalSrvAmt));
                            $('#Recommendation').val(result.list.Remarks);

                            $.post("sv.api/invoice/getTable", { invoiceno: result.list.InvoiceNo, joborderno: result.list.JobOrderNo, jobtype: result.list.JobType, serviceno: result.list.ServiceNo }, function (result) {
                                widget.populateTable({ selector: "#tblTaskPart", data: result.list });
                            });
                        }
                        else {
                            widget.alert(result.message)
                        }

                    });
                //}
                //else {
                //    widget.alert("No SPK (Job Order No.) salah");
               //}
            //});
        }
    });

    $("#InvoiceNo").on("keypress", function (e) {
        if (e.keyCode == 13) {
            $("#JobOrderNo").focus();
        }
    });

    $("#InvoiceNo").on("blur", function (e) {
        var invNo = $(this).val();
        if (invNo == undefined || invNo == null || invNo == "") {           
            return;
        }
        $.post('sv.api/invoice/GetDataByInvNo', { InvNo: invNo }).success(function (result) {
            if (result.success) {
                var spkNo = result.data[0].JobOrderNo;
                if (spkNo!=null) {
                    $.post('sv.api/invoice/get', { joborderno: spkNo }).success(function (result) {
                        if (result.success) {
                            result.list["IsPPN"] = result.TaxCode == "PPN" ? true : false;
                            widget.populate(result.list);
                            //$('#JobOrderNo').val(spkNo);
                            $('#btnPrint').removeAttr('disabled');
                            $('#ServiceStatusDesc').html(result.list.ServiceStatusDesc);
                            $('#TotalSrvAmt').html(number_format(result.list.TotalSrvAmt));

                            $('#Recommendation').val(result.list.Remarks);

                            $.post("sv.api/invoice/getTable", { invoiceno: result.list.InvoiceNo, joborderno: result.list.JobOrderNo, jobtype: result.list.JobType, serviceno: result.list.ServiceNo }, function (result) {
                                widget.populateTable({ selector: "#tblTaskPart", data: result.list });
                            });
                        }
                        else {
                            widget.alert(result.message)
                        }

                    });
                }                    
            }
            else {
                widget.alert("No invoice \"" + invNo + "\" tidak ditemukan");
            }
        });
    });
});