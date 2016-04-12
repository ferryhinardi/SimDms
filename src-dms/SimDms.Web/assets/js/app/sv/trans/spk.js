var totalSrvAmt = 0;
var status = 'N';
var svType = '2';
var admin = false;

$(document).ready(function () {
    var options = {
        title: "Input SPK",
        xtype: "panels",
        id: "titleSPK",
        toolbars: [
            { name: "btnCreate", text: "New", icon: "icon-file" },
            { name: "btnBrowse", text: "Browse", icon: "icon-search" },
            { name: "btnSave", text: "Save", icon: "icon-save" },
            { name: "btnProcess", text: "Create SPK", icon: "icon-bolt", cls: "hide" },
            { name: "btnCancel", text: "Cancel SPK", icon: "icon-remove", cls: "hide" },
            { name: "btnClose", text: "Close SPK", icon: "icon-signout", cls: "hide" },
            { name: "btnOpen", text: "Open SPK", icon: "icon-signin", cls: "hide" },
        ],
        panels: [
            {
                name: "pnlButton",
                title: "",
                items: [
                    { name: "StatusSO", text: "", cls: "span5", readonly: true, type: "label" },
                    {
                        type: "buttons", cls: "span3",
                        items: [
                                    { name: "btnLkpKendaraan", text: "Kendaraan", icon: "icon-user", cls: "btn btn-info", click: "showKendaraan()" },
                                    { name: "btnLkpPelanggan", text: "Pelanggan", icon: "icon-user", cls: "btn btn-info", click: "showPelanggan()" },
                        ]
                    },
                ]
            },
            {
                name: "pnlServiceInfo",
                title: "Service Information",
                items: [
                    { name: "ServiceNo", type: "hidden" },
                    {
                        name: "ServiceType",
                        text: "Service Type",
                        type: "select",
                        cls: "span4",
                        items: [
                            { value: '0', text: 'ESTIMASI' },
                            { value: '1', text: 'BOOKING' },
                            { value: '2', text: 'SPK' }
                        ]
                    },
                    {
                        text: "Company",
                        type: "controls",
                        items: [
                            { name: "CompanyCode", cls: "span2", placeHolder: "Company Code", readonly: true },
                            { name: "CompanyName", cls: "span6", placeHolder: "Company Name", readonly: true }
                        ]
                    },
                    {
                        text: "Branch",
                        type: "controls",
                        items: [
                            { name: "BranchCode", cls: "span2", placeHolder: "Branch Code", readonly: true },
                            { name: "BranchName", cls: "span6", placeHolder: "Branch Name", readonly: true }
                        ]
                    },
                    { name: "JobOrderNo", text: "SPK No", placeHolder: "SPK/YY/99999", cls: "span4" },
                    { name: "JobOrderDate", text: "SPK Date", cls: "span4", type: "datepicker", format: 'DD MMM YYYY hh:mm:ss' },
                    { name: "ServiceStatusDesc", text: "Service Status", readonly: true },
                    { name: "SrvTotalSrvAmt", text: "Total Amount", readonly: true, cls: "number-int" },
                ]
            },
            {
                name: "tabpage1",
                xtype: "tabs",                
                items: [
                    { name: "CustVehicle", text: "Customer & Vehicle" },
                    { name: "TaskPart", text: "Task & Part" },
                    { name: "InvClaim", text: "List Invoice Claim" }
                ]
            },
            {
                name: "CustVehicle",
                title: "Customer & Vehicle",
                cls: "tabpage1 CustVehicle",
                items: [
                    { name: "PoliceRegNo", text: "Police Reg No", cls: "span4", type: "popup", btnName: "btnPoliceRegNo", readonly: true },
                    { name: "ServiceBookNo", text: "Service Book No", cls: "span4", readonly: true },
                    { name: "BasicModel", text: "Basic Model", cls: "span4", readonly: true },
                    { name: "TransmissionType", text: "Trans Type", cls: "span4", readonly: true },
                    { name: "ChassisCode", text: "Chassis Code", cls: "span4", readonly: true },
                    { name: "ChassisNo", text: "Chassis No", cls: "span4", readonly: true },
                    { name: "EngineCode", text: "Engine Code", cls: "span4", readonly: true },
                    { name: "EngineNo", text: "Engine No", cls: "span4", readonly: true },
                    { name: "ColourCode", text: "Color", cls: "span4", readonly: true },
                    { name: "Odometer", text: "Odometer", cls: "span4 number-int" },
                    {
                        text: "Customer",
                        type: "controls",
                        items: [
                            { name: "CustomerCode", cls: "span2", placeHolder: "Cust Code", readonly: true },
                            { name: "CustomerName", cls: "span6", placeHolder: "Cust Name", readonly: true },
                        ]
                    },
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
                ]
            },
            {
                cls: "tabpage1 CustVehicle",
                title: "Contract & Club",
                items: [
                    { name: "ContractNo", text: "Contract No", placeHolder: "No", cls: "span2", readonly: true },
                    { name: "ContractExpired", text: "Expired", cls: "span3", readonly: true },
                    { name: "ContractStatus", text: "Status", cls: "span3", readonly: true },
                    { name: "ClubNo", text: "Club No", placeHolder: "No", cls: "span2", readonly: true },
                    { name: "ClubExpired", text: "Expired", cls: "span3", readonly: true },
                    { name: "ClubStatus", text: "Status", cls: "span3", readonly: true },
                ]
            },
            {
                cls: "tabpage1 CustVehicle",
                title: "Customer Bill",
                items: [
                    { name: "InsurancePayFlag", text: "Insurance", cls: "span4", type: "switch", float: "left" },
                    { name: "InsuranceOwnRisk", text: "Own Risk", cls: "span4 number" },
                    { name: "InsuranceNo", text: "Polis No", cls: "span4" },
                    { name: "InsuranceJobOrderNo", text: "Insurance No", cls: "span4" },
                    {
                        text: "Customer",
                        type: "controls",
                        items: [
                            { name: "CustomerCodeBill", cls: "span2", placeHolder: "Cust Code", readonly: true, type: "popup", btnName: "btnCustomerCodeBill" },
                            { name: "CustomerNameBill", cls: "span6", placeHolder: "Cust Name", readonly: true },
                        ]
                    },
                    { name: "CustAddr1Bill", text: "Address", maxlength: 100, readonly: true },
                    { name: "CustAddr2Bill", text: "", readonly: true },
                    { name: "CustAddr3Bill", text: "", readonly: true },
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
                        type: "controls",
                        name: "ctlDiscP",
                        text: "Discount (%)",
                        items: [
                            { name: "pLaborDiscPct", cls: "span2 number", placeHolder: "Labor", type: "popup", btnName: "btnLaborDiscPct" },
                            { name: "pPartDiscPct", placeHolder: "Part", cls: "span2 number", type: "popup", btnName: "btnPartDiscPct" },
                            { name: "pMaterialDiscPct", placeHolder: "Material", cls: "span2 number", type: "popup", btnName: "btnMaterialDiscPct" },
                       ]
                    },
                    {
                        type: "controls",
                        name: "ctlDisc",
                        items: [
                            { name: "LaborDiscPct", cls: "span2 number", placeHolder: "Labor" },
                            { name: "PartDiscPct", placeHolder: "Part", cls: "span2 number" },
                            { name: "MaterialDiscPct", placeHolder: "Material", cls: "span2 number" },
                        ]
                    },
                    { name: "IsPPN", text: "PPN", type: "switch", float: "left" },
                ]
            },
            {
                cls: "tabpage1 TaskPart",
                title: "Job Request",
                items: [
                    { name: "ServiceRequestDesc", text: "Job Request", type: "textarea" },
                    {
                        text: "Job Type",
                        type: "controls",
                        items: [
                            { name: "JobType", cls: "span2", placeHolder: "Code", type: "popup", readonly: true },
                            { name: "JobTypeDesc", cls: "span6", placeHolder: "Description", readonly: true },
                        ]
                    },
                    { name: "ConfirmChangingPart", text: "Allow Change Part", type: "switch", float: "left" },
                    {
                        text: "Service Advisor (SA)",
                        type: "controls",
                        items: [
                            { name: "ForemanID", cls: "span2", placeHolder: "Code", type: "popup", readonly: true },
                            { name: "ForemanName", cls: "span6", placeHolder: "Name", readonly: true },
                        ]
                    },
                    {
                        text: "Foreman (FM)",
                        type: "controls",
                        items: [
                            { name: "MechanicID", cls: "span2", placeHolder: "Code", type: "popup", readonly: true },
                            { name: "MechanicName", cls: "span6", placeHolder: "Name", readonly: true },
                        ]
                    },
                    {
                        text: "Estimate Finish",
                        type: "controls",
                        items: [
                            { name: "EstimateFinishDate", type: "datepicker", cls: "span2" },
                        ]
                    },
                    { name: "IsSparepartClaim", text: "Sparepart Claim?", type: "switch", cls: "span4", float: "left" }
                ]
            },
            {
                cls: "tabpage1 TaskPart",
                title: "Detail Task / Part",
                xtype: "table",
                pnlname: "pnlTaskPart",
                tblname: "tblTaskPart",
                buttons: [{ name: "btnAddDtl", text: "Add New Task Part", icon: "icon-plus" }],
                items: [
                     { name: "TaskPartSeq", type: "hidden" },
                    {
                        name: "BillType", text: "Ditanggung Oleh", cls: "span3", type: "select",                      
                    },
                    {
                        name: "ItemType", text: "Item Type", cls: "span3", type: "select",
                        items: [
                            { value: "L", text: "Labor (Jasa)" },
                            { value: "0", text: "Sparepart & Material" },
                        ]
                    },
                    {
                        text: "Part / Job",
                        type: "controls",
                        cls: "span6",
                        items: [
                            { name: "TaskPartNo", cls: "span3", placeHolder: "Code", type: "popup", readonly: false },
                            { name: "TaskPartDesc", cls: "span5", placeHolder: "Description", readonly: true }
                        ]
                    },
                    { name: "QtyAvail", text: "Available", cls: "span3 number", readonly: true },
                    { name: "Price", text: "Price", cls: "span3 number-int" },
                    { name: "OprHourDemandQty", text: "Qty / NK", cls: "span3 number" },
                    { name: "DiscPct", text: "Discount", cls: "span3 number" },
                    { name: "PriceNet", text: "Net Price", cls: "span3 number-int", readonly: true },                   
                    {
                        type: "buttons", items: [
                            { name: "btnSaveDtl", text: "Save", icon: "icon-save" },
                            { name: "btnCancelDtl", text: "Cancel", icon: "icon-undo" },
                            { name: "btnUpdNPrice", text: "Update New Price" }
                        ]
                    },
                ],
                columns: [
                    { name: "BillType", cls: "hide" },
                    { name: "ItemType", cls: "hide" },
                    { text: "Action", type: "action", width: 80 },
                    { name: "BillTypeDesc", text: "Ditanggung Oleh" },
                    { name: "TypeOfGoodsDesc", text: "Jenis Item" },
                    { name: "TaskPartNo", text: "Task/Part" },
                    { name: "TaskPartDesc", cls: "hide" },
                    { name: "OprHourDemandQty", text: "Qty/NK", cls: "right", width: 80, type: "numeric" },
                    { name: "OprRetailPrice", text: "Price", cls: "right", width: 110, type: "price" },
                    { name: "QtyAvail", text: "Available", cls: "right", width: 90, type: "numeric" },
                    { name: "DiscPct", text: "Discount", cls: "right", width: 80, type: "price" },
                    { name: "PriceNet", text: "Net Price", cls: "right", width: 120, type: "price" },
                    { name: "TaskPartSeq", cls: "hide" },
                ]
            },
            {
                cls: "tabpage1 TaskPart summary",
                items: [
                    { name: "LaborDppAmt", text: "DPP - Jasa", cls: "span6 number-int", readonly: true },
                    { name: "PartsDppAmt", text: "DPP - Part", cls: "span6 number-int", readonly: true },
                    { name: "MaterialDppAmt", text: "DPP - Material", cls: "span6 number-int", readonly: true },
                    { name: "TotalDppAmt", text: "Total DPP", cls: "span6 indent number-int", readonly: true },
                    { name: "TotalPpnAmt", text: "Total PPN", cls: "span6 indent number-int", readonly: true },
                    { name: "SrvTotalSrvAmt", text: "Total Amount", cls: "span6 indent number-int", readonly: true },
                ]
            },
            {
                cls: "tabpage1 InvClaim",
                title: "Invoice Claim",            
                xtype: "table",
                pnlname: "pnlInvClaim",
                tblName: "tblInvClaim",
                name: "tblInvClaim",
                buttons: [{ name: "btnInvoiceNo", text: "Invoice No", icon: "icon-search" }],               
                columns: [
                    { text: "Action", type: "action", width: 80 },
                    { name: "InvoiceNo", text: "Invoice No" },
                    { name: "InvoiceDate", text: "Invoice Date", type: "dateTime" },
                    { name: "JobOrderNo", text: "Job Order No" },
                    { name: "JobOrderDate", text: "Job Order Date", type: "dateTime" },
                    { name: "LaborDppAmt", text: "Labor Dpp Amt", cls: "right" },
                    { name: "PartsDppAmt", text: "Parts Dpp Amt", cls: "right" },
                    { name: "MaterialDppAmt", text: "Material Dpp Amt", cls: "right" },
                    { name: "TotalDppAmt", text: "Total Dpp Amt", cls: "right" },
                    ]
            }
        ],
    }
    var widget = new SimDms.Widget(options);
    widget.setSelect([
        { name: "BillType", url: "sv.api/combo/billtype" },
    ]);
    widget.default = { };
    widget.JobOrderNo = "";
    widget.render(function () {
        clearData();
        console.log(widget.title);
    });
    widget.onTableClick(function (icon, row) {
        console.log(icon);
        switch (icon) {
            case "edit":
                editDetail(row);
                break;
            case "trash":
                deleteDetail(row);
                break;
            default:
                break;
        }
    });
    widget.onTabsChanged(function (obj, parent, name) {
        //console.log(parent, name);
    });

    // Double click
    widget.lookup.onDblClick(function (e, data, name) {
        widget.lookup.hide();        
        switch (name) {
            case "JobOrderList":
                data.showAjax = false;
                widget.showAjaxLoad();
                widget.post("sv.api/spk/get", data, function (result) {
                    if (result.success) {
                        populateData(result);
                        $('#ServiceType').attr('disabled', 'disabled');
                    }
                    else {
                        widget.hideAjaxLoad();
                    }
                });
                break;
            case "CustomerVehicle":
                populateCustVehicle(data);
                break;
            case "CustomerBill":
                populateCustBill(data);
                break;
            case "JobType":
                widget.populate({ JobType: data.JobType, JobTypeDesc: data.Description });
                break;
            case "Foreman":
                widget.populate({ ForemanID: data.EmployeeID, ForemanName: data.EmployeeName });
                break;
            case "Mechanic":
                widget.populate({ MechanicID: data.EmployeeID, MechanicName: data.EmployeeName });
                break;
            case "TaskPart":
                if ($('#ItemType').val() == "L") {
                    widget.populate({
                        TaskPartNo: data.OperationNo,
                        TaskPartDesc: data.DescriptionTask,
                        OprHourDemandQty: number_format(data.Qty, 2),
                        Price: number_format(data.Price),
                        DiscPct: number_format($('#LaborDiscPct').val(), 2),
                        PriceNet: number_format(data.Qty * data.Price * (100 - $('#LaborDiscPct').val()) * 0.01)
                    });
                    validatePackage(data.OperationNo, "L");
                }
                else {
                    widget.populate({
                        TaskPartNo: data.PartNo,
                        TaskPartDesc: data.PartName,
                        QtyAvail: data.Available,
                        Price: number_format(data.Price, 0),
                        PriceNet: number_format(data.Qty * data.Price * (100 - $('#LaborDiscPct').val()) * 0.01, 0),
                        OprHourDemandQty: 0,
                        TaskPartSeq: data.TaskPartSeq
                    });

                    if (data.GroupTypeOfGoods == "SPAREPART") {
                        $("#DiscPct").val($("#PartDiscPct").val());
                    }
                    else {
                        $("#DiscPct").val($("#MaterialDiscPct").val());
                    }
                }                
                break;
            case "Discount":
                widget.populate({
                    pLaborDiscPct: number_format(data.LaborDiscPct, 2),
                    pPartDiscPct: number_format(data.PartDiscPct, 2),
                    pMaterialDiscPct: number_format(data.MaterialDiscPct, 2),
                    LaborDiscPct: number_format(data.LaborDiscPct, 2),
                    PartDiscPct: number_format(data.PartDiscPct, 2),
                    MaterialDiscPct: number_format(data.MaterialDiscPct, 2)
                });
                break;
            case "InvoiceList":
                var params = {
                    JobOrderNo: $('#JobOrderNo').val(),
                    InvoiceNo: data.InvoiceNo
                }
                widget.post("sv.api/spk/SaveInvoice", params, function (result) {
                    if (result.Message == '') {
                        widget.post("sv.api/spk/ClaimList", params, function (result) {
                            $('#tblInvClaim').show();
                            widget.populateTable({ selector: "#tblInvClaim", data: result.claimList });
                        });
                    } else {
                        widget.alert(result.Message);
                    }
                });                
            default:
                break;
           
        }
    });
    $('#OprHourDemandQty, #Price, #DiscPct').on('blur', function (e) {
        calculateTotal();
    });

    $('#PriceNet, #LaborDppAmt, #PartsDppAmt, #MaterialDppAmt, #TotalDppAmt, #TotalPpnAmt, #SrvTotalSrvAmt, #Odometer').on('blur', function (e) {
        $('#PriceNet').val(number_format($('#PriceNet').val()));
        $('#LaborDppAmt').val(number_format($('#LaborDppAmt').val()));
        $('#PartsDppAmt').val(number_format($('#PartsDppAmt').val()));
        $('#MaterialDppAmt').val(number_format($('#MaterialDppAmt').val()));
        $('#TotalDppAmt').val(number_format($('#TotalDppAmt').val()));
        $('#TotalPpnAmt').val(number_format($('#TotalPpnAmt').val()));
        $('#SrvTotalSrvAmt').val(number_format($('#SrvTotalSrvAmt').val()));
        $('#Odometer').val(number_format($('#Odometer').val()));
    });

    function calculateTotal() {
        var params = {
            OprHourDemandQty: $("[Name=OprHourDemandQty]").val().replace(",", ""),
            Price: $("[Name=Price]").val().replace(",", ""),
            DiscPct: $("[Name=DiscPct]").val().replace(",", "")
        }

        widget.post("sv.api/spk/CalculateTotal", params, function (result) { 
            if (result.success == true) {
                $('#PriceNet').val(number_format(result.PriceNet));
            }
        });
        
    }

    $("#InsurancePayFlagN").on('change', function (e) {
        $("#InsuranceOwnRisk, #InsuranceJobOrderNo, #InsuranceNo").attr('disabled', 'disabled');
    });
    $("#InsurancePayFlagY").on('change', function (e) {
        $("#InsuranceOwnRisk, #InsuranceJobOrderNo, #InsuranceNo").removeAttr('disabled');
    });
    $("#btnpLaborDiscPct, #btnpPartDiscPct, #btnpMaterialDiscPct").on("click", function () {
        if ($('#JobType').val() == "") {
            widget.alert("Silahkan isi Job Type terlebih dahulu");
            return;
        }
        widget.lookup.init({
            name: "Discount",
            title: "Discount List",
            source: "sv.api/grid/ListDiscountServiceLookup",
            columns: [
               { mData: "SeqNo", sTitle: "No" },
               { mData: "DiscountType", sTitle: "Discount Type" },
               { mData: "LaborDiscPct", sTitle: "Labor Disct" },
               { mData: "PartDiscPct", sTitle: "Part Disct" },
               { mData: "MaterialDiscPct", sTitle: "Material Disct" },
            ],
            additionalParams: [
               { name: "CustomerCode", element: "CustomerCodeBill", type: "text" },
               { name: "ChassisCode", element: "ChassisCode", type: "text" },
               { name: "ChassisNo", element: "ChassisNo", type: "text" },
               { name: "JobType", element: "JobType", type: "text" },
            ],
        });
        widget.lookup.show();
    });
    $("#btnCreate").on("click", clearData);
    $("#btnBrowse").on("click", browseData);
    $("#btnSave").on("click", saveData);
    $("#btnUpdNPrice").on("click", function () {
        if (!confirm("Harga akan diupdate dari data Master, proses dilanjutkan ?")) return;
        var params = {
            serviceNo: $("[name=ServiceNo]").val(),
            operationNo: $("[name=OperationNo]").val()
        }
        widget.post("sv.api/spk/UpdateNewPrice", params, function (result) {
            if (result.Message == '') {
                refreshData();
            } else {
                alert(result.Message);
            }
        });
    });
    $('#ServiceType').on('change', function (e) {
        svType = $(this).val();
        if (svType == '0') {
            $('#JobOrderNo').attr('name', 'EstimationNo');
            $('#JobOrderNo').attr('placeHolder', 'EST/XX/YYYYY');
            $('[name=JobOrderDate]').attr('name', 'EstimationDate');
            $('[name=BookingDate]').attr('name', 'EstimationDate');
        } else if (svType == '1') {
            $('#JobOrderNo').attr('name', 'BookingNo');
            $('#JobOrderNo').attr('placeHolder', 'BOK/XX/YYYYY');
            $('[name=EstimationDate]').attr('name', 'BookingDate');
            $('[name=JobOrderDate]').attr('name', 'BookingDate');
        } else if (svType == '2') {
            $('#JobOrderNo').attr('name', 'JobOrderNo');
            $('#JobOrderNo').attr('placeHolder', 'SPK/XX/YYYYY');
            $('[name=EstimationDate]').attr('name', 'JobOrderDate');
            $('[name=BookingDate]').attr('name', 'JobOrderDate');
        }
    });
    $("#JobOrderNo").on("blur", function () {
        $("#ServiceStatusDesc").focus();
        refreshData();
    });
    $("#btnPoliceRegNo").on("click", function () {
        widget.lookup.init({
            name: "CustomerVehicle",
            title: "Job Order List",
            source: "sv.api/grid/customervehicles",
            sortings: [[0, "asc"]],
            columns: [
                { mData: "VinNo", sTitle: "Vin No", sWidth: "110px" },
                { mData: "PoliceRegNo", sTitle: "Police RegNo", sWidth: "110px" },
                { mData: "CustomerName", sTitle: "Customer Name" },
                { mData: "BasicModel", sTitle: "Model", sWidth: "80px" },
                { mData: "TransmissionType", sTitle: "MT/AT", sWidth: "80px" },
            ]
        });
        widget.lookup.show();
    });
    $("#btnCustomerCodeBill").on("click", function () {
        widget.lookup.init({
            name: "CustomerBill",
            title: "Customer List",
            source: "sv.api/grid/customers",
            sortings: [[0, "asc"]],
            columns: [
                { mData: "CustomerCode", sTitle: "Customer Code", sWidth: "180px" },
                { mData: "CustomerName", sTitle: "Customer Name" },
                { mData: "HPNo", sTitle: "HP No", sWidth: "120px" },
                { mData: "PhoneNo", sTitle: "Phone No", sWidth: "120px" },
            ]
        });
        widget.lookup.show();
    });
    $("#btnJobType").on("click", function () {
        widget.lookup.init({
            name: "JobType",
            title: "Job Type List",
            source: "sv.api/grid/jobtypes",
            columns: [
                { mData: "JobType", sTitle: "Job Type", sWidth: "180px" },
                { mData: "Description", sTitle: "Job Type Descr" },
                { mData: "BasicModel", sTitle: "Model", sWidth: "110px" },
            ],
            additionalParams: [
                { name: "BasicModel", element: "BasicModel", type: "text" },
            ],
        });
        widget.lookup.show();
    });
    $("#btnForemanID").on("click", function () {
        widget.lookup.init({
            name: "Foreman",
            title: "Service Advisor List",
            source: "sv.api/grid/serviceadvisors",
            sortings: [[1, "asc"]],
            columns: [
                { mData: "EmployeeID", sTitle: "NIK", sWidth: "110px" },
                { mData: "EmployeeName", sTitle: "Name" },
            ],
        });
        widget.lookup.show();
    });
    $("#btnMechanicID").on("click", function () {        
        widget.lookup.init({
            name: "Mechanic",
            title: "Foreman List",
            source: "sv.api/grid/foremans",
            sortings: [[1, "asc"]],
            columns: [
                { mData: "EmployeeID", sTitle: "NIK", sWidth: "110px" },
                { mData: "EmployeeName", sTitle: "Name" },
            ],
        });
        widget.lookup.show();
    });
    $("#btnTaskPartNo").on("click", lkuTaskPart);
    $("#btnSaveDtl").on("click", saveDetail);
    $("#btnCancelDtl").on("click", listDetail);
    $("#btnAddDtl").on("click", addDetail);
    $("#tblTaskPart").on("dblclick", editDetail);
    $('#btnProcess').on('click', function (e) {
        var params = {
            serviceNo: $("[name=ServiceNo]").val()
        }
        widget.post("sv.api/spk/createSpk", params, function (result) {
            if (result.Message == '') {
                $('#ServiceType').val('2');
                $('#ServiceType').change();
                refreshData();
            } else {
                alert(result.Message);
            }
        });
    });
    $('#btnCancel').on('click', function (e) {
        if (!confirm("SPK akan dibatalkan, apakah anda yakin?")) return;
        var params = {
            serviceNo: $("[name=ServiceNo]").val()
        }
        widget.post("sv.api/spk/cancelSpk", params, function (result) {
            if (result.Message == '') {
                refreshData();
            } else {
                alert(result.Message);
            }
        });
    });
    $('#btnInvoiceNo').on('click', function (e) {    
        widget.lookup.init({
            name: "InvoiceList",
            title: "Invoice List",
            source: "sv.api/grid/InvoiceList",
            columns: [
                { mData: "InvoiceNo", sTitle: "Invoice No" },
                {
                    mData: "InvoiceDate", sTitle: "Invoice Date",
                    mRender: function (data, type, full) {
                        return moment(data).format('DD MMM YYYY');
                    }
                },
                { mData: "JobOrderNo", sTitle: "Job Order No" },
                {
                    mData: "JobOrderDate", sTitle: "Job Order Date",
                    mRender: function (data, type, full) {
                        return moment(data).format('DD MMM YYYY');
                    }
                },
                { mData: "LaborDppAmt", sTitle: "Labor Dpp Amt" },
                { mData: "PartsDppAmt", sTitle: "Parts Dpp Amt" },
                { mData: "MaterialDppAmt", sTitle: "Material Dpp Amt" },
                { mData: "TotalDppAmt", sTitle: "Total Dpp Amt" },
            ],
            additionalParams: [             
              { name: "ChassisCode", element: "ChassisCode", type: "text" },
              { name: "ChassisNo", element: "ChassisNo", type: "text" },
              { name: "JobOrderNo", element: "JobOrderNo", type: "text" },
            ],
        });
        widget.lookup.show();
    });
    $('#btnClose').on('click', function (e) {
        if (totalSrvAmt == 0) {
            if (!confirm("Total Biaya adalah 0, Apakah akan dilanjutkan?")) return;
        }
        var params = {
            serviceNo: $("[name=ServiceNo]").val()
        }
        widget.post("sv.api/spk/closeSpk", params, function (result) {
            if (result.Message == '') {
                refreshData();
            } else {
                alert(result.Message);
            }
        });
    });
    $('#btnOpen').on('click', function (e) {
        var params = {
            serviceNo: $("[name=ServiceNo]").val()
        }
        widget.post("sv.api/spk/openSpk", params, function (result) {
            if (result.Message == '') {
                refreshData();
            } else {
                alert(result.Message);
            }
        });
    });
  
    function validatePackage(operationNo, itemType) {
        var param = $(".main .gl-widget").serializeObject();
        widget.post("sv.api/spk/validatepackage?taskPartNo=" + operationNo + "&itemType=" + itemType, param, function (result) {
            if (result.data != null) {
                $('#BillType').val("P");
                $('#BillType').attr('disabled', 'disabled');
                $('#DiscPct').val(result.data.DiscPct);
            }
        });
    }

    function clearData() {
        widget.clearForm();
        widget.post("sv.api/spk/default", function (result) {                       
            widget.default = $.extend({
                ServiceNo: "0",
                ServiceType: "2",
                TotalSrvAmt: "0",
                Odometer: "0",
                InsuranceOwnRisk: "0",
                //LaborDiscPct: "0.00",
                //PartDiscPct: "0.00",
                //MaterialDiscPct: "0.00",
                ConfirmChangingPart: true,
                LaborDppAmt: "0",
                PartsDppAmt: "0",
                MaterialDppAmt: "0",
                TotalDppAmt: "0",
                TotalPpnAmt: "0",
                SrvTotalSrvAmt: "0",
                ForemanID: result.ForemanID,
                ForemanName: result.ForemanName
            }, result);            
            widget.populate(widget.default);
            widget.JobOrderNo = "";
            svType = '2';
            $('#ServiceType').removeAttr('disabled');
            //$('#JobOrderNo').attr('placeHolder', 'SPK/XX/YYYYY');
            $('[name=IsPPN]').attr('disabled', 'disabled');
            $('input[name="JobOrderDate"]').removeAttr('disabled');
            totalSrvAmt = 0;
            status = 'N';
            clearDtl();
            alterUI(status);
            
            $('#tblTaskPart, #ctlDiscP, #tblInvClaim').hide();
            $("#pnlTaskPart, #pnlInvClaim").slideUp();            
            $("#tblTaskPart td .icon").addClass("link");
            $("#tblInvClaim td .icon").addClass("link");
            $("#InsuranceOwnRisk, #InsuranceJobOrderNo, #InsuranceNo").attr('disabled', 'disabled');
            $('#ctlDisc').show();
        });
    }

    function clearDtl() {
        $('#SeqNo').val("");       
        $('#TaskPartNo').val("");
        $('#TaskPartDesc').val("");
        $('#QtyAvail').val("0");
        $('#Price').val("0");
        $('#OprHourDemandQty').val("0");
        $('#DiscPct').val("0");
        $('#PriceNet').val("0");        
    }

    function browseData() {
        if (svType == '0') {
            widget.lookup.init({
                name: "JobOrderList",
                title: "Estimation List",
                source: "sv.api/grid/estimations",
                sortings: [[0, "desc"]],
                columns: [
                    { mData: "EstimationNo", sTitle: "Estimation No", sWidth: "110px" },
                    {
                        mData: "EstimationDate", sTitle: "Estimation Date", sWidth: "130px",
                        mRender: function (data, type, full) {
                            return moment(data).format('DD MMM YYYY - HH:mm');
                        }
                    },
                    { mData: "CustomerName", sTitle: "Customer" },
                    { mData: "PoliceRegNo", sTitle: "Police Reg" },
                    { mData: "ServiceBookNo", sTitle: "Service Book" },
                    { mData: "ForemanName", sTitle: "Foreman" },
                    { mData: "BasicModel", sTitle: "Model" },
                    { mData: "ServiceStatusDesc", sTitle: "Status", sWidth: "140px" },
                ]
            });
        }
        else if (svType == '1') {
            widget.lookup.init({
                name: "JobOrderList",
                title: "Booking List",
                source: "sv.api/grid/bookings",
                sortings: [[0, "desc"]],
                columns: [
                    { mData: "BookingNo", sTitle: "Booking No", sWidth: "110px" },
                    {
                        mData: "BookingDate", sTitle: "Booking Date", sWidth: "130px",
                        mRender: function (data, type, full) {
                            return moment(data).format('DD MMM YYYY - HH:mm');
                        }
                    },
                    { mData: "CustomerName", sTitle: "Customer" },
                    { mData: "PoliceRegNo", sTitle: "Police Reg" },
                    { mData: "ServiceBookNo", sTitle: "Service Book" },
                    { mData: "ForemanName", sTitle: "Foreman" },
                    { mData: "BasicModel", sTitle: "Model" },
                    { mData: "ServiceStatusDesc", sTitle: "Status", sWidth: "140px" },
                ]
            });
        }
        else if (svType == '2') {
            widget.lookup.init({
                name: "JobOrderList",
                title: "Job Order List",    
                source: "sv.api/grid/joborders",
                sortings: [[0, "desc"]],
                columns: [
                    { mData: "JobOrderNo", sTitle: "JobOrder No", sWidth: "110px" },
                    {
                        mData: "JobOrderDate", sTitle: "JobOrder Date", sWidth: "130px",
                        mRender: function (data, type, full) {
                            return moment(data).format('DD MMM YYYY - HH:mm');
                        }
                    },
                    { mData: "CustomerName", sTitle: "Customer" },
                    { mData: "PoliceRegNo", sTitle: "Police Reg" },
                    { mData: "ServiceBookNo", sTitle: "Service Book" },
                    { mData: "ForemanName", sTitle: "Foreman" },
                    { mData: "BasicModel", sTitle: "Model" },
                    { mData: "ServiceStatusDesc", sTitle: "Status", sWidth: "140px" },
                ]
            });
        }
        widget.lookup.show();
    }

    function saveData() {
        if ($('#PoliceRegNo').val() == "") {
            widget.alert('No Polisi Harus Diisi');
            return;
        }
        if ($('#CustomerCodeBill').val() == "") {
            widget.alert('Kode Customer Harus Diisi');
            return;
        }
        if ($('#JobType').val() == "") {
            widget.alert('Job Type Harus Diisi');
            return;
        }
        if ($('#ForemanID').val() == "") {
            widget.alert('SA Harus Diisi');
            return;
        }
        if ($('#MechanicID').val() == "") {
            widget.alert('Foreman Harus Diisi');
            return;
        }
        if ($('#InsurancePayFlagY').val() == "true") {
            if ($('#InsuranceNo').val() == "") {
                widget.alert('No Insurance Harus Diisi');
                return;
            }
            if ($('#InsuranceJobOrderNo').val() == "") {
                widget.alert('No Polis Harus Diisi');
                return;
            }
            if ($('#InsuranceOwnRisk').val() == "") {
                widget.alert('Jumlah Own Risk Harus Diisi');
                return;
            }
        }
        if ($('#JobType').val() == "REWORK") {
            if ($('#ServiceRequestDesc').val() == "") {
                widget.alert('Job Request Harus Diisi');
                return;
            }
        }
        if ($('#JobType').val() != "PDI" && $('#JobType').val().indexOf("FSC") == -1 && parseInt($('#Odometer').val(), 10) <= 0) {
            widget.alert('Odometer harus lebih dari 0');
            return;
        }

        var overrideDisc = false;
        var param = $(".main .gl-widget").serializeObject();
        widget.post("sv.api/spk/ValidateOdometer?Odometer=" + param.Odometer, param, function (result) {
            if (result.success) {
                widget.post("sv.api/spk/ValidateInsertSPK", param, function (result0) {
                    if (result0.success) {
                        widget.post("sv.api/spk/EditDiscPart", param, function (result1) {
                            if (result1.data != '') {
                                if (confirm(result1.data)) {
                                    overrideDisc = true;
                                }
                            }
                            widget.post("sv.api/spk/save?bOverrideDisc=" + overrideDisc + "&odometer=" + param.Odometer, param, function (result2) {
                                if (result2.success) {
                                    widget.populate(result2.data);
                                    widget.showNotification("data saved...");
                                    $('#ServiceType').attr('disabled', 'disabled');
                                    refreshData();
                                }
                            });
                        });
                    }
                    else {
                        if (result0.confirm) {
                            if (confirm(result0.data)) {
                                widget.post("sv.api/spk/EditDiscPart", param, function (result1) {
                                    if (result1.data != '') {
                                        if (confirm(result1.data)) {
                                            overrideDisc = true;
                                        }
                                    }
                                    widget.post("sv.api/spk/save?bOverrideDisc=" + overrideDisc, param, function (result2) {
                                        if (result2.success) {
                                            widget.populate(result2.data);
                                            widget.showNotification("data saved...");
                                            $('#ServiceType').attr('disabled', 'disabled');
                                            refreshData();
                                        }
                                    });
                                });
                            }
                        }
                        else {
                            widget.alert(result0.data);
                        }
                    }
                });                
            }
            else {                              
                if (confirm(result.data)) {
                    widget.post("sv.api/spk/ValidateInsertSPK", param, function (result0) {
                        if (result0.success) {
                            widget.post("sv.api/spk/EditDiscPart", param, function (result1) {                                
                                if (result1.data != '') {
                                    if (confirm(result1.data)) {
                                        overrideDisc = true;
                                    }
                                }
                                widget.post("sv.api/spk/save?bOverrideDisc=" + overrideDisc, param, function (result2) {
                                    if (result2.success) {
                                        widget.populate(result2.data);
                                        widget.showNotification("data saved...");
                                        $('#ServiceType').attr('disabled', 'disabled');
                                        refreshData();
                                    }
                                });
                            });                            
                        }
                        else {
                            if (result0.confirm) {
                                if (confirm(result0.data)) {
                                    widget.post("sv.api/spk/EditDiscPart", param, function (result1) {
                                        if (result1.data != '') {
                                            if (confirm(result1.data)) {
                                                overrideDisc = true;
                                            }
                                        }
                                        widget.post("sv.api/spk/save?bOverrideDisc=" + overrideDisc, param, function (result2) {
                                            if (result2.success) {
                                                widget.populate(result2.data);
                                                widget.showNotification("data saved...");
                                                $('#ServiceType').attr('disabled', 'disabled');
                                                refreshData();
                                            }
                                        });
                                    });
                                }
                            }
                            else {
                                widget.alert(result0.data);
                            }
                        }
                    });
                }                           
            }
        });        
    }

    function addDetail(e) {                       
        e.preventDefault();
        clearDtl();
        $("#pnlTaskPart").slideDown();
        $("#btnAddDtl").parent().hide();
        $("#tblTaskPart td .icon").removeClass("link");
        $('#BillType, #ItemType, #btnTaskPartNo, #TaskPartNo').removeAttr('disabled', 'disabled');
        var params = {
            BasicModel: $('#BasicModel').val(),
            JobType: $('#JobType').val(),
        }
        widget.post("sv.api/spk/editdetail", params, function (result) {
            var header = {
                IsSparepartClaim: $('#IsSparepartClaim').val(),
                InsurancePayFlag: $('#InsurancePayFlag').val()
            }
            validateJobTypeDetailJob(header, result.lockBill, result.lockInsu, result.lockQtyNK);
        });
        
    }
   
    function validateJobTypeDetailJob(header, lockBill, lockInsu, lockQtyNK) {
        var data = {
            BasicModel: $('#BasicModel').val(),
            JobType: $('#JobType').val(),           
        }
        widget.post("sv.api/pekerjaan/get", data, function (result) {
            if (result.success) {                
                if (result.data.GroupJobType == "CLM" || (result.data.GroupJobType == "FSC" && (parseInt(result.data.PdiFscSeq, 1) <= 1) || (parseInt(result.data.PdiFscSeq, 1) >= 5 && parseInt(result.data.PdiFscSeq, 1) <= 9))) {
                    $('[name=btnCustomerCodeBill]').attr('disabled', 'disabled');
                }
                else {
                    $('[name=btnCustomerCodeBill]').removeAttr('disabled');
                }

                if (result.data.GroupJobType == "CLM") {
                    if (header.IsSparepartClaim == true) {
                        $("#BillType").val("S");
                    }
                    else {
                        $("#BillType").val("W");
                    }
                    $('[name=BillType]').attr('disabled', 'disabled');
                    $('[name=ItemType]').removeAttr('disabled');
                }
                else if (result.data.GroupJobType == "FSC") {
                    if (parseInt(result.data.PdiFscSeq, 1) <= 1 || (parseInt(result.data.PdiFscSeq, 1) >= 5 && parseInt(result.data.PdiFscSeq, 1) <= 9)) {
                        $("#BillType").val("F");
                    }
                    else {
                        $("#BillType").val("C");
                    }
                    $('[name=BillType],[name=ItemType]').removeAttr('disabled');
                    $('[name=btnCustomerCodeBill]').attr('disabled', 'disabled');
                    //$('[name=BillType],[name=ItemType],[name=btnCustomerCodeBill]').attr('disabled', 'disabled');
                }
                else if (result.data.GroupJobType == "OTH" || result.data.GroupJobType == "RTN") {
                    if (result.detailList == 0) {
                        $("#BillType").val("C");
                        $('[name=ItemType],[name=BillType]').removeAttr('disabled');
                    }
                    else {
                        $("#BillType").val("C");
                        $('[name=BillType]').attr('disabled', 'disabled');
                        $('[name=ItemType]').removeAttr('disabled');
                    }
                    if (lockQtyNK != null) {
                        if (lockQtyNK.ParaValue == "1") {
                            if (result.data.JobType.indexOf("PB") == 0 && result.data.GroupJobType == "RTN") {
                                $("#OprHourDemandQty").attr('disabled', 'disabled');
                            }
                            else {
                                $("#OprHourDemandQty").removeAttr('disabled');
                            }
                        }
                        else {
                            $("#OprHourDemandQty").removeAttr('disabled');
                        }
                    }
                    else {
                        $("#OprHourDemandQty").removeAttr('disabled');
                    }
                }
                else {
                    if (result.data.JobType == "REWORK") {
                        $("#BillType").val("I");
                        $('[name=BillType]').attr('disabled', 'disabled');
                        $('[name=ItemType]').removeAttr('disabled');
                    }
                    else {
                        $("#BillType").val("C");
                        $('[name=ItemType],[name=BillType]').removeAttr('disabled');
                    }
                }

                if (document.getElementById("tblTaskPart").getElementsByTagName("tbody")[0].getElementsByTagName("tr").length > 0) {
                    $("#ItemType").val("0");
                    $('#TaskPartNo, #OprHourDemandQty').removeAttr('disabled');
                    $('#Price, #DiscPct').attr('disabled', 'disabled');
                }
                else {
                    $("#ItemType").val("L");
                    $('#TaskPartNo, #OprHourDemandQty, #DiscPct').removeAttr('disabled');
                    $('#ItemType, #Price').attr('disabled', 'disabled');
                }
                
                if (lockBill != null) {
                    if (lockBill.ParaValue == "0") {
                        var groups = "FSC,CLM,REWORK";
                        if (((result.data.GroupJobType.indexOf("FSC") != -1 || result.data.GroupJobType.indexOf("CLM") != -1 || result.data.GroupJobType.indexOf("REWORK") != -1) || result.data.JobType == "REWORK") == false) {
                            $('[name=BillType]').removeAttr('disabled');                           
                        }
                    }
                }

                if (lockInsu != null) {
                    if (lockInsu.ParaValue == "0") {                       
                        if (header.InsurancePayFlag == true) {
                            $('[name=BillType]').removeAttr('disabled');
                        }
                    }
                }
            }
        });        
    }

    function editDetail(row) {
        if (parseInt(status) >= 5) return;
        var params = {
            BasicModel: $('#BasicModel').val(),
            JobType: $('#JobType').val(),       
        }

        widget.post("sv.api/spk/editdetail", params, function (result) {
            if (result.success == true) {
                if (row[0] == "F" && $('#JobType').val() == "FSC01") {
                    if (row[1] != "L" && result.spkAdmin.ParaValue == "0") {
                        return;
                    }
                }

                $("#pnlTaskPart").slideDown();
                $("#btnAddDtl").parent().hide();
                $("#tblTaskPart td .icon").removeClass("link");

                var data = {
                    BillType: row[0],
                    ItemType: row[1],
                    TaskPartNo: row[5],
                    TaskPartDesc: row[6],
                    QtyAvail: row[9],
                    Price: row[8],
                    OprHourDemandQty: row[7],
                    DiscPct: row[10],
                    PriceNet: row[11],
                    TaskPartSeq: row[12]
                }
                widget.populate(data, "#pnlTaskPart")

                $('#BillType, #ItemType, #btnTaskPartNo, #TaskPartNo').attr('disabled', 'disabled');

                if (data.ItemType == "L") {
                    $('#btnUpdNPrice, #Price').removeAttr('disabled');
                    $('#OprHourDemandQty').attr('disabled', 'disabled');
                }
                else {
                    $('#btnUpdNPrice, #Price').attr('disabled', 'disabled');
                    $('#OprHourDemandQty').removeAttr('disabled');
                }

                if (result.lockBill != null) {
                    if (result.lockBill.ParaValue == "0") {
                        var groups = "FSC,CLM,REWORK";
                        if (((result.job.GroupJobType.indexOf("FSC") != -1 || result.job.GroupJobType.indexOf("CLM") != -1 || result.job.GroupJobType.indexOf("REWORK") != -1) || result.job.JobType == "REWORK") == false) {
                            $('[name=BillType]').removeAttr('disabled');
                        }
                    }
                }

                if (result.lockInsu != null) {
                    if (result.lockInsu.ParaValue == "0") {
                        if (result.job.InsurancePayFlag == true) {
                            $('[name=BillType]').removeAttr('disabled');
                        }
                    }
                }

                if (result.lockPrice != null) {
                    if (result.lockPrice.ParaValue == "1") {
                        $('[name=Price]').attr('disabled');
                    }
                }

                if (result.job.GroupJobType == "CLM" && result.job.IsSparepartClaim == true) {
                    $('[name=BillType]').removeAttr('disabled');
                }

                if (result.spkAdmin != null) {
                    if (result.spkAdmin.ParaValue == "1") {
                        $('[name=BillType]', '[name=OprHourDemandQty]').removeAttr('disabled');
                    }
                }

                validatePackage(data.TaskPartNo, data.ItemType);

                if (result.lockQtyNK != null) {
                    if (result.lockQtyNK.ParaValue == "1") {
                        if (result.job.GroupJobType.indexOf("PB") == 0 && result.job.GroupJobType == "RTN") {
                            $("#OprHourDemandQty").attr('disabled', 'disabled');
                        }
                        else {
                            $("#OprHourDemandQty").removeAttr('disabled');
                        }
                    }
                    else {
                        $("#OprHourDemandQty").removeAttr('disabled');
                    }
                }
                else {
                    $("#OprHourDemandQty").removeAttr('disabled');
                }

               
            }
        });
    }
   
    function deleteDetail(row) {
        if (status == '5') return;

        var partSeq = 0;
        if (row[1] != "L") partSeq = row[12];

        if (confirm("Anda yakin akan menghapus data ini?")) {
            var params = {
                ServiceNo: $("[name=ServiceNo]").val(),
                TaskPartType: row[1],
                TaskPartNo: row[5],
                PartSeq: partSeq,
                InvoiceNo: row[1],
                JobOrderNo: $('#JobOrderNo').val()
            }

            console.log(params.InvoiceNo);
            if (params.InvoiceNo != null && params.InvoiceNo != 0) {
                widget.post("sv.api/spk/deleteinvoice", params, function (result) {
                    if (result.success) {
                        refreshData();
                    }
                    else {
                        widget.alert(result.message);
                    }
                });
            }
            else {
                widget.post("sv.api/spk/deletedetail", params, function (result) {
                    if (result.success) {
                        refreshData();
                    }
                    else {
                        widget.alert(result.message);
                    }
                });
            }
            
        };
    }

    function lkuTaskPart() {
        var data = {
            CompanyCode: $("#CompanyCode").val(),
            BranchCode: $("#BranchCode").val(),
            BasicModel: $("#BasicModel").val(),
            JobType: $("#JobType").val(),
            ChassisCode: $("#ChassisCode").val(),
            ChassisNo: $("#ChassisNo").val(),
            TransType: $("#TransmissionType").val(),
            ItemType: $("#ItemType").val(),
            BillType: $("#BillType").val()
        }
        
        if (data.BasicModel.length == 0) return;
        switch (data.ItemType) {
            case "L":
                widget.lookup.init({
                    name: "TaskPart",
                    title: "Task List",
                    source: "sv.api/grid/partno?basicmodel=" + data.BasicModel + '&jobtype=' + data.JobType,
                    data: data,
                    sortings: [[0, "asc"]],
                    columns: [
                        { mData: "OperationNo", sTitle: "Pekerjaan", sWidth: 140 },
                        { mData: "DescriptionTask", sTitle: "Keterangan", sWidth: 200 },
                        { mData: "Qty", sTitle: "NK", sWidth: 80 },
                        { mData: "Price", sTitle: "Nilai Jasa", sWidth: 180 },
                        //{ mData: "IsActive", sTitle: "Status", sWidth: 120 },
                    ],
                });
                widget.lookup.show();
                break;
            case "0":
                widget.lookup.init({
                    name: "TaskPart",
                    title: "Task List",
                    source: "sv.api/grid/NoPartOpen",
                    data: data,
                    sortings: [[1, "asc"]],
                    columns: [
                        { mData: "PartNo", sTitle: "No Part", sWidth: 140 },
                        { mData: "PartName", sTitle: "Keterangan", sWidth: 200 },
                        { mData: "GroupTypeOfGoods", sTitle: "Group", sWidth: 80 },
                        { mData: "Available", sTitle: "Available", sWidth: 100 },
                        { mData: "Price", sTitle: "Nilai Part", sWidth: 100 },
                        { mData: "Status", sTitle: "Status", sWidth: 100 },
                    ],
                });
                widget.lookup.show();
                break;
            default:
                break;
        }
    }

    function assignTask(data) {
        var params = {
            TaskPartNo: data.OperationNo,
            TaskPartDesc: data.Description,
            QtyAvail: "0",
            OprHourDemandQty: number_format(data.OperationHour, 2),
            Price: number_format(data.LaborPrice, 0),
            DiscPct: "0.00",
            PriceNet: number_format(data.OperationHour * data.LaborPrice, 0)
        }
        widget.populate(params, "#pnlTaskPart")
    }

    function assignPart(data) {
        var params = {
            TaskPartNo: data.PartNo,
            TaskPartDesc: data.PartName,
            QtyAvail: number_format(data.Available, 2),
            OprHourDemandQty: "1",
            Price: number_format(data.NilaiPart, 0),
            DiscPct: "0.00",
            PriceNet: number_format(data.NilaiPart, 0)
        }
        widget.populate(params, "#pnlTaskPart")
    }

    function saveDetail() {
        if ($("[Name=OprHourDemandQty]").val() == 0) {
            alert("Qty / NK wajib diisi");
            return;
        }
        var params = {
            ServiceNo: $("[name=ServiceNo]").val(),
            BillType: $("[Name=BillType]").val(),
            ItemType: $("[Name=ItemType]").val(),
            TaskPart: $("[Name=TaskPartNo]").val().replace(",", ""),
            HourQty: $("[Name=OprHourDemandQty]").val().replace(",", ""),
            TaskPrice: $("[Name=Price]").val().replace(",", ""),
            DiscPct: $("[Name=DiscPct]").val().replace(",", ""),
            PartSeq: $("[Name=TaskPartSeq]").val().replace(",", ""),
            ChassisCode: $("[Name=ChassisCode]").val(),
            ChassisNo: $("[Name=ChassisNo]").val(),
            BasicModel: $("[Name=BasicModel]").val(),
            JobType: $("[Name=JobType]").val(),
        }
        if (params.PartSeq == "") params.PartSeq = -1;

        if (params.ItemType == "L") {
            widget.post("sv.api/spk/ServiceValidation", params, function (result) {
                if (result.success == false) {
                    widget.alert(result.data);
                    refreshData();
                }
                else {
                    widget.post("sv.api/spk/savedetail", params, function (result) {
                        if (result.success == true) {
                            refreshData();
                        }
                        else {
                            widget.alert(result.message);
                            refreshData();
                        }
                    });
                }
            });
        }
        else {
            widget.post("sv.api/spk/savedetail", params, function (result) {
                if (result.success == true) {
                    refreshData();
                }
                else {
                    widget.alert(result.message);
                    refreshData();
                }
            });
        }
       
    }

    function listDetail(e) {
        e.preventDefault();
        $("#pnlTaskPart").slideUp();
        $("#tblTaskPart td .icon").addClass("link");
        $("#btnAddDtl").parent().show();
    }

    function populateCustVehicle(data) {
        var model = {
            PoliceRegNo: data.PoliceRegNo,
            ServiceBookNo: data.ServiceBookNo,
            BasicModel: data.BasicModel,
            TransmissionType: data.TransmissionType,
            ChassisCode: data.ChassisCode,
            ChassisNo: data.ChassisNo,
            EngineCode: data.EngineCode,
            EngineNo: data.EngineNo,
            ColourCode: data.ColourCode,
            CustomerCode: data.CustomerCode,
            CustomerName: data.CustomerName,
            CustAddr1: data.Address1,
            CustAddr2: data.Address2,
            CustAddr3: data.Address3,
            CityCode: data.CityCode,
            CityName: data.CityName,
           
            IsPPN: true
        }
        populateCustBill(data);

        widget.populate(model);
    }

    function populateCustBill(data) {
        var model = {
            CustomerCodeBill: data.CustomerCode,
            CustomerNameBill: data.CustomerName,
            CustAddr1Bill: data.Address1,
            CustAddr2Bill: data.Address2,
            CustAddr3Bill: data.Address3,
            CityCodeBill: data.CityCode,
            CityNameBill: data.CityName,
            CityCodeBill: data.CityCode,
            CityNameBill: data.CityName,
            PhoneNo: data.PhoneNo,
            HPNo: data.HPNo,
            FaxNo: data.FaxNo,
        }
       
        widget.post("sv.api/spk/ListDiscountService", data, function (result) {
            if (result.success) {
                if (result.count > 1) {
                    $('#ctlDisc').hide();
                    $('#ctlDiscP').show();
                }

                $('#LaborDiscPct, #pLaborDiscPct').val(number_format(result.data.LaborDiscPct, 2));
                $('#PartDiscPct, #pPartDiscPct').val(number_format(result.data.PartDiscPct, 2));
                $('#MaterialDiscPct, #pMaterialDiscPct').val(number_format(result.data.MaterialDiscPct, 2));
            }
        });

        widget.populate(model);
    }
     
    function populateData(result) {
        var data = result.data || {};
        var header = {
            ServiceNo: data.ServiceNo,
            JobOrderNo: data.JobOrderNo,
            JobOrderDate: data.JobOrderDate == null ? '1 Jan 1990' : moment(data.JobOrderDate).format(SimDms.date),
            EstimationNo: data.EstimationNo,
            EstimationDate: data.EstimationDate == null ? '1 Jan 1990' : moment(data.EstimationDate).format(SimDms.dateFormat),
            BookingNo: data.BookingNo,
            BookingDate: data.BookingDate,
            ServiceStatusDesc: svType == '0' ? 'ESTIMATION' : svType == '1' ? 'BOOKING' : data.ServiceStatusDesc,
            CustomerCode: data.CustomerCode,
            CustomerName: data.CustomerName,
            CustAddr1: data.CustAddr1,
            CustAddr2: data.CustAddr2,
            CustAddr3: data.CustAddr3,
            PoliceRegNo: data.PoliceRegNo,
            CityCode: data.CityCode,
            CityName: data.CityName,
            PhoneNo: data.PhoneNo,
            FaxNo: data.FaxNo,
            HPNo: data.HPNo,
            ServiceBookNo: data.ServiceBookNo,
            BasicModel: data.BasicModel,
            TransmissionType: data.TransmissionType,
            ChassisCode: data.ChassisCode,
            ChassisNo: data.ChassisNo,
            EngineCode: data.EngineCode,
            EngineNo: data.EngineNo,
            ColourCode: data.ColorCodeDesc,
            Odometer: data.Odometer,
            ContractNo: data.ContractNo,
            //ContractExpired: ((data.ContractEndPeriod || "").length > 0) ? moment(data.ContractEndPeriod).format(SimDms.dateFormat) : "",
            ContractExpired: data.ContractEndPeriod == null ? '1 Jan 1990' : moment(data.ContractEndPeriod).format(SimDms.dateFormat),
            ContractStatus: data.ContractStatusDesc,
            ClubNo: data.ClubCode,
            //ClubExpired: ((data.ClubEndPeriod || "").length > 0) ? moment(data.ClubEndPeriod).format(SimDms.dateFormat) : "",
            ClubExpired: data.ClubEndPeriod == null ? '1 Jan 1990': moment(data.ClubEndPeriod).format(SimDms.dateFormat),
            ClubStatus: data.ClubStatusDesc,
            InsurancePayFlag: data.InsurancePayFlag,
            InsuranceOwnRisk: data.InsuranceOwnRisk,
            InsuranceNo: data.InsuranceNo,
            InsuranceJobOrderNo: data.InsuranceJobOrderNo,
            CustomerCodeBill: data.CustomerCodeBill,
            CustomerNameBill: data.CustomerNameBill,
            CustAddr1Bill: data.CustAddr1Bill,
            CustAddr2Bill: data.CustAddr2Bill,
            CustAddr3Bill: data.CustAddr3Bill,
            CityCodeBill: data.CityCodeBill,
            CityNameBill: data.CityNameBill,
            LaborDiscPct: data.LaborDiscPct,
            PartDiscPct: data.PartDiscPct,
            MaterialDiscPct: data.MaterialDiscPct,
            IsPPN: ((data.TaxCode || "PPN") === "PPN" ? true : false),
            ServiceRequestDesc: data.ServiceRequestDesc,
            JobType: data.JobType,
            JobTypeDesc: data.JobTypeDesc,
            ConfirmChangingPart: data.ConfirmChangingPart,
            ForemanID: data.ForemanID,
            ForemanName: data.ForemanName,
            MechanicID: data.MechanicID,
            MechanicName: data.MechanicName,
            EstimateFinishDate: data.EstimateFinishDate == null ? '1 Jan 1990' : moment(data.EstimateFinishDate).format(SimDms.dateFormat),
            ClaimType: (data.IsSparepartClaim || true) ? "P" : "S",
            LaborDppAmt: number_format(data.SrvLaborDppAmt, 0),
            PartsDppAmt: number_format(data.SrvPartsDppAmt, 0),
            MaterialDppAmt: number_format(data.SrvMaterialDppAmt, 0),
            TotalDppAmt: number_format(data.SrvTotalDppAmt, 0),
            TotalPpnAmt: number_format(data.SrvTotalPpnAmt, 0),
            SrvTotalSrvAmt: number_format(data.TotalSrvAmt, 0),
            IsSparepartClaim: data.IsSparepartClaim
        }              

        status = data.ServiceStatus;
        alterUI(status);

        if (data.IsSparepartClaim == true) {
            $('#btnInvoiceNo').removeAttr('disabled');
            widget.post("sv.api/spk/ClaimList", data, function (result1) {
                $('#tblInvClaim').show();
                widget.populateTable({ selector: "#tblInvClaim", data: result1.claimList });
            });            
        }
        else {
            $('#btnInvoiceNo').attr('disabled', 'disabled');
        }
        widget.populate(header);
        $('#tblTaskPart').show();        
        widget.populateTable({ selector: "#tblTaskPart", data: result.list });
      
        widget.hideAjaxLoad();
        widget.JobOrderNo = data.JobOrderNo;
        $('input[name="JobOrderDate"]').attr('disabled', 'disabled');
        $('input[name="EstimationDate"]').attr('disabled', 'disabled');
        $('input[name="BookingDate"]').attr('disabled', 'disabled');
       
        $("#pnlTaskPart, #pnlInvClaim").slideUp();
        $("#tblTaskPart td .icon").addClass("link");
        $("#tblInvClaim td .icon").addClass("link");
        $("#btnAddDtl").parent().show();        

        if ($("#InsurancePayFlagY").val() == "true") {
            $("#InsuranceOwnRisk, #InsuranceJobOrderNo, #InsuranceNo").removeAttr('disabled');
        }
        else{
            $("#InsuranceOwnRisk, #InsuranceJobOrderNo, #InsuranceNo").attr('disabled', 'disabled');
        }

        validateJobTypeDetailJob(header, result.spkFlagLockBill, result.spkFlagLockInsu, result.spkFlagLockQtyNK);

        widget.post("sv.api/spk/IsEnableCloseSPK", data, function (result) {
            if (result.enabled == true) {                              
                $('#btnClose').removeClass('hide');
                $('#btnClose').show();
            }
            else {
                $('#btnClose').addClass('hide');
            }
        });
    }
     
    function refreshData() {
        var data = $("#pnlServiceInfo").serializeObject();
        //if (widget.JobOrderNo !== data.JobOrderNo) {
            data.showAjax = false;
            widget.showAjaxLoad();
            widget.post("sv.api/spk/get", data, function (result) {
                if (result.success) {
                    totalSrvAmt = result.data.TotalSrvAmt;
                    populateData(result);
                }
                else {
                    $('#JobOrderNo').val('');
                    widget.hideAjaxLoad();
                }
            });
        //}
    }

    function alterUI(status) {        
        if (status == 'N') {
            $('#btnProcess').addClass('hide');
            $('#btnCancel').addClass('hide');
            $('#btnClose').addClass('hide');
            $('#btnOpen').addClass('hide');
            $('#btnCustomerCodeBill, #btnPoliceRegNo, #btnJobType, #CustomerCodeBill').removeAttr('disabled');
            $("#btnAddDtl").parent().hide();
            //$('#btnMaterialDiscPct, #btnLaborDiscPct, #btnPartDiscPct, #pLaborDiscPct, #pMaterialDiscPct, #pPartDiscPct').hide();
            $('#ctlDiscP').hide();
            $('#btnInvoiceNo').attr('disabled', 'disabled');
            clearDtl();
        } else {
            if (svType == '0' || svType == '1') {
                $('#btnProcess').removeClass('hide');
                $('#btnCancel').addClass('hide');
                $('#btnClose').addClass('hide');
                $('#btnOpen').addClass('hide');
                $('#btnPoliceRegNo').attr('disabled', 'disabled');
                $('#btnJobType').attr('disabled', 'disabled');
                $('#CustomerCodeBill').removeAttr('disabled');
                $("#btnAddDtl").parent().show();                
            } else if (svType == '2') {                
                //0,2,3,4 sama
                //6,7,8,9,A,B sama
                if (status == '0') {
                    $('#btnProcess').addClass('hide');
                    $('#btnCancel').removeClass('hide');
                    $('#btnClose').addClass('hide');
                    $('#btnOpen').addClass('hide');
                    $("#btnAddDtl").parent().show();
                }
                else if (status == '1') {
                    $('#btnProcess').addClass('hide');
                    $('#btnCancel').removeClass('hide');
                    $('#btnClose').removeClass('hide');
                    $('#btnOpen').addClass('hide');
                    $('#btnPoliceRegNo').attr('disabled', 'disabled');
                    $('#btnJobType').attr('disabled', 'disabled');
                    $('#CustomerCodeBill').attr('disabled', 'disabled');                    
                    $("#btnAddDtl").parent().show();
                } else if (status == '5') {
                    $('#btnProcess').addClass('hide');
                    $('#btnCancel').addClass('hide');
                    $('#btnClose').addClass('hide');
                    $('#btnOpen').removeClass('hide');
                    $('#btnPoliceRegNo').attr('disabled', 'disabled');
                    $('#btnJobType').attr('disabled', 'disabled');
                    $('#CustomerCodeBill').removeAttr('disabled');
                    $("#btnAddDtl").parent().hide();
                } else if (status == '2' || status == '3' || status == '4') {
                    $('#btnProcess').addClass('hide');
                    $('#btnCancel').removeClass('hide');
                    $('#btnClose').addClass('hide');
                    $('#btnOpen').addClass('hide');
                    $('#btnPoliceRegNo').attr('disabled', 'disabled');
                    $('#btnJobType').attr('disabled', 'disabled');
                    $('#CustomerCodeBill').removeAttr('disabled');
                    $("#btnAddDtl").parent().show();
                } 
                else {
                    $('#btnProcess').addClass('hide');
                    $('#btnCancel').addClass('hide');
                    $('#btnClose').addClass('hide');
                    $('#btnOpen').addClass('hide');
                    $('#btnSave').addClass('hide');
                    $('#btnPoliceRegNo').attr('disabled', 'disabled');
                    $('#btnJobType').attr('disabled', 'disabled');
                    $('#CustomerCodeBill').removeAttr('disabled');
                    $("#btnAddDtl").parent().hide();
                }
            }
        }
    }
    
});