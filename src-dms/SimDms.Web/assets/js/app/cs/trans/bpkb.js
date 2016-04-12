$(document).ready(function () {
    var options = {
        title: "BPKB",
        xtype: "panels",
        panels: [
            {
                title: "Customer Details",
                items: [
                    {
                        text: "Company",
                        type: "controls",
                        items: [
                            { name: "CompanyCode", cls: "span2 ticket", placeHolder: "Company Code", readonly: true },
                            { name: "CompanyName", cls: "span6 ticket", placeHolder: "Dealer Name", readonly: true }
                        ]
                    },
                    //{
                    //    text: "Branch",
                    //    type: "controls",
                    //    items: [
                    //        { name: "BranchCode", cls: "span2 ticket", placeHolder: "Branch Code", readonly: true },
                    //        { name: "BranchName", cls: "span6 ticket", placeHolder: "Branch Name", readonly: true }
                    //    ]
                    //},
                    {
                        text: "Customer",
                        type: "lookup",
                        name: "CustomerCode",
                        display: "CustomerName",
                        btnName: "btnCustBrowse",
                        namePlaceholder: "Customer Code",
                        displayPlaceholder: "Customer Name",
                    },
                    { name: "Address", text: "Address", readonly: true },
                    { name: "PhoneNo", text: "Telephone", readonly: true },
                ]
            },
            {
                items: [
                    { name: "CarType", text: "Car Type", readonly: true },
                    { name: "Color", text: "Warna", readonly: true },
                    { name: "PoliceRegNo", text: "No Polisi", readonly: true },
                    { name: "Engine", text: "Engine", readonly: true },
                    { name: "Chassis", text: "Chassis", readonly: true },
                    { name: "SalesmanName", text: "Sales Name", readonly: true },
                ]
            },
            {
                items: [
                    { name: "BpkbDate", text: "Tgl BPKB", cls: "span4", readonly: false, type: "datepicker" },
                    { name: "StnkDate", text: "Tgl STNK", cls: "span4", readonly: true, type: "datepicker" },
                ]
            },
            //{
            //    title: "Customer Input for BPKB",
            //    items: [
                   
            //    ]
            //},
            {
                title: "BPKB Requirements",
                items: [
                    {
                        name: "CustomerCategory", text: "Customer Category", cls: "span4 full", type: "select", required: true,
                        items: [
                            { value: "L", text: "Leasing" },
                            { value: "T", text: "Tunai" },
                        ]
                    },
                     { name: "BpkbReadyDate", text: "Ready date", type: "datepicker", cls: "span4" },
                    { name: "BpkbPickUp", text: "Pick up / delivery", type: "datepicker", cls: "TanggalInput span4" },
                    { name: "ReqKtp", text: "KTP", type: "switch", cls: "span2", float: "left" },
                    { name: "ReqStnk", text: "STNK", type: "switch", cls: "span2", float: "left" },
                    { name: "ReqSuratKuasa", text: "Surat Kuasa", type: "switch", cls: "span2", float: "left" },
                    { name: "LeasingCode", text: "Leasing", cls: "leasing span4", type: "select", required: true },
                    {
                        name: "Tenor", text: "Tenor", cls: "leasing span4", type: "select", required: true,
                        items: [
                             { value: "6", text: "6 Bulan" },
                             { value: "12", text: "12 Bulan" },
                             { value: "18", text: "18 Bulan" },
                             { value: "24", text: "24 Bulan" },
                             { value: "32", text: "32 Bulan" },
                             { value: "36", text: "36 Bulan" },
                        ]
                    },
                    { name: "ReqInfoLeasing", text: "Info ke Leasing", type: "switch", cls: "leasing span4", float: "left" },
                    { name: "ReqInfoCust", text: "Info ke Pelanggan", type: "switch", cls: "leasing span4", float: "left" },
                    { name: "Comment", text: "Customer comments", type: "textarea" },
                    { name: "Additional", text: "Additional inquiries", type: "text" },
                    { name: "StatusInfo", text: "Status", readonly: true },
                    { name: "Status", text: "", readonly: true, type: 'hidden' },
                ]
            },
        ],
        toolbars: [
            { name: "btnClear", text: "New", icon: "icon-file", cls: "hide" },
            { name: "btnBrowse", text: "Browse", icon: "icon-search" },
            //{ name: "btnEdit", text: "Edit", icon: "icon-edit", cls: "hide" },
            { name: "btnSave", text: "Save", icon: "icon-save", cls: "hide" },
            //{ name: "btnCancel", text: "Cancel", icon: "icon-undo", cls: "hide" },
            { name: "btnDelete", text: "Delete", icon: "icon-trash", cls: "hide" },
        ],
    }
    var widget = new SimDms.Widget(options);
    widget.render(function () {
    });

    widget.lookup.onDblClick(function (e, data, name) {
        widget.lookup.hide();
        //console.log(name);
        //console.log(data);

        if (name === "CustBPKB") {
            widget.populate($.extend({ PeriodYear: new Date().getFullYear() }, data));
            setTimeout(function () {
                refreshData(data);

            }, SimDms.defaultTimeout);
        }
        else {
            widget.populate($.extend({}, widget.DefaultData, data));
            widget.showToolbars(["btnBrowse", "btnSave", "btnClear"]);
            HideLeasingColumn("");
            $('#CustomerCategory').val('');
        }
    });

    widget.render(function () {
        widget.select({ selector: "#LeasingCode", url: "cs.api/Combo/CustLeasing" });
    });

    if ($('#CustomerCategory').val() == "") {
        $('div.leasing').hide(); 
        $('div.TanggalInput').hide();
    }

    $('#CustomerCategory').on("change", function (event) {
        HideLeasingColumn(event.currentTarget.value);
    });

    function HideLeasingColumn(CustCategory) {
        if (CustCategory == "L") {
            $('div.leasing').slideDown();
            $('div.TanggalInput').slideDown();
            //widget.populate({ ReqInfoLeasing: false, ReqInfoCust: false });
            $('#LeasingCode').val('');
            $('#Tenor').val('');
            $('div.TanggalInput > label').text("Leasing Notification");
        } else {//(event.currentTarget.value == "T") {
            $('div.leasing').slideUp();
            $('div.TanggalInput').slideDown();
            $('div.TanggalInput > label').text("Pick up / delivery");
        }
        //console.log(CustCategory);
        if (CustCategory==="") {
            $('div.TanggalInput').slideUp();
        }
    }

    $("#btnCustBrowse").on("click", function () {
        widget.lookup.init({
            name: "CustList",
            title: "Customer Buyer List",
            source: "cs.api/grid/CustBPKB?lookupType=browse",
            columns: [
                { mData: "CustomerCode", sTitle: "Cust Code", sWidth: "100px" },
                { mData: "CustomerName", sTitle: "Customer Name", sWidth: "200px" },
                { mData: "Address", sTitle: "Address" },
                { mData: "PhoneNo", sTitle: "Phone No" },
                { mData: "PoliceRegNo", sTitle: "No Polisi", sWidth: "100px" },
                { mData: "SalesmanName", sTitle: "Salesman" },
                { mData: "Chassis", sTitle: "Chassis", bVisible: false, bSearchable: false },
            ]
        });
        widget.lookup.show();
    });

    $('#btnBrowse').on('click', function () {
        widget.lookup.init({
            name: "CustBPKB",
            title: "Customer BPKB List",
            source: "cs.api/grid/CustBPKB?lookupType=open",
            columns: [
                { mData: "CustomerCode", sTitle: "Cust Code", sWidth: "100px", bVisible: false },
                { mData: "CustomerName", sTitle: "Name", sWidth: "150px" },
                { mData: "Address", sTitle: "Address", sWidth: "200px" },
                { mData: "PhoneNo", sTitle: "Phone No", bVisible: false },
                { mData: "PoliceRegNo", sTitle: "No Polisi", sWidth: "90px" },
                { mData: "SalesmanName", sTitle: "Salesman" },
                { mData: "CreatedDate", sTitle: "Input", sWidth: "90px" },
                { mData: "FinishDate", sTitle: "Finish", sWidth: "90px" },
                { mData: "Chassis", sTitle: "Vin No.", bVisible: true, bSearchable: false },
            ]
        });
        widget.lookup.show();
    });

    $('#btnDelete').on('click', function () {
        var data = $(".main form").serializeObject();
        if (confirm("Anda yakin akan menghapus data ini?")) {
            widget.post("cs.api/bpkb/delete", data, function (result) {
                if (result.success) {
                    $("#btnClear").click();
                }
            });
        };
    });

    $("#btnEdit").on("click", function () {
        widget.showToolbars(["btnSave", "btnCancel"]);
    });

    $("#btnSave").on("click", function () {
        var valid = $(".main form").valid();
        if (valid) {
            var data = $(".main form").serialize();
            widget.post("cs.api/bpkb/save", data, function (result) {
                if (result.success) {
                    var data = {
                        CompanyCode: $('#CompanyCode').val(),
                        CustomerCode: $('[name="CustomerCode"]').val(),
                        Chassis: $('#Chassis').val()
                    };
                    refreshData(data);
                }
            });
        }
    });

    $("#btnCancel").on("click", function () {
        var data = {
            CompanyCode: $('#CompanyCode').val(),
            CustomerCode: $('[name="CustomerCode"]').val(),
            Chassis: $('#Chassis').val()
        };
        refreshData(data);
        $("#btnBrowse,#btnEdit,#btnClear").show();
        $("#btnSave,#btnCancel").hide();
    });

    $("#btnClear").on("click", function () {
        $(".toolbar > button").hide();
        $("#btnBrowse").show();
        $("input[type='text']").val("");
        $('textarea').val("");
        $('#CustomerCategory').val("");
        widget.populate({ ReqKtp: false, ReqSuratKuasa: false, ReqInfoLeasing: false, ReqInfoCust: false, ReqStnk:false });
        HideLeasingColumn("");
    });

    function refreshData(data) {
        var data = {
            CompanyCode: data["CompanyCode"],
            CustomerCode: data["CustomerCode"],
            Chassis: data["Chassis"]
        };

        widget.post("cs.api/bpkb/get", data, function (result) {
            if (result.success) {
                widget.populate(result.data);
                //HideLeasingColumn($('#CustomerCategory').val());

                setTimeout(function () {
                    HideLeasingColumn($('#CustomerCategory').val());

                    $("[name='LeasingCode']").val(result.data.LeasingCode || "");
                    $("[name='Tenor']").val(result.data.Tenor || "");

                    if (widget.isNullOrEmpty(result.data.BpkbDate) == false) {
                        $("[name='BpkbDate']").val(widget.toDateFormat(widget.cleanJsonDate(result.data.BpkbDate)));
                    }
                    if (widget.isNullOrEmpty(result.data.StnkDate) == false) {
                        $("[name='StnkDate']").val(widget.toDateFormat(widget.cleanJsonDate(result.data.StnkDate)));
                    }
                    widget.changeSwitchValue({ name: "ReqInfoLeasing", value: (result.data.ReqInfoLeasing) });
                    widget.changeSwitchValue({ name: "ReqInfoCust", value: (result.data.ReqInfoCust) });
                }, SimDms.defaultTimeout);
                widget.showToolbars(["btnBrowse", "btnSave", 'btnDelete', "btnClear"]);
            }
        });
    }
});