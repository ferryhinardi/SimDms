$(document).ready(function () {
    var options = {
        title: "STNK Extension",
        xtype: "panels",
        panels: [
            {
                title: "Customer Details",
                items: [
                    {
                        text: "Company",
                        type: "controls",
                        items: [
                            { name: "CompanyCode", cls: "span2", placeHolder: "Company Code", readonly: true },
                            { name: "CompanyName", cls: "span6", placeHolder: "Company Name", readonly: true }
                        ]
                    },
                    //{
                    //    text: "Branch",
                    //    type: "controls",
                    //    items: [
                    //        { name: "BranchCode", cls: "span2", placeHolder: "Branch Code", readonly: true },
                    //        { name: "BranchName", cls: "span6", placeHolder: "Branch Name", readonly: true }
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
                    { name: "PhoneNo", text: "PhoneNo", readonly: true },
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
                    { name: "StnkDate", text: "Tgl STNK", cls: "span4", readonly: false, type: "datepicker" },
                ]
            },
            {
                title: "Customer Input for STNK Extension",
                items: [
                    { name: "IsStnkExtend", text: "Extension", type: "switch", cls: "span4", float: "left" },
                    { name: "StnkExpiredDate", text: "STNK Expired Date", type: "datepicker", cls: "span4", required: true },
                ]
            },
            {
                title: "STNK Requirements",
                name:"STNKRequirement",
                items: [
                     {
                         name: "CustomerCategory", text: "Customer Category", cls: "span4 full", type: "select", required:true,
                         items: [
                             { value: "L", text: "Leasing" },
                             { value: "T", text: "Tunai" },
                         ]
                     },
                    { name: "ReqKtp", text: "KTP", type: "switch", float: "left", cls: "ktp span2" },
                    { name: "ReqStnk", text: "STNK", type: "switch", float: "left", cls: "kuasa span2" },
                    { name: "ReqBpkb", text: "BPKB asli", type: "switch", float: "left", cls: "bpkb span2" },
                    //{ name: "ReqSuratKuasa", text: "Surat Kuasa", type: "switch", float: "left", cls: "kuasa span2" },
                    { name: "LeasingCode", text: "Leasing", cls: "leasing span4", type: "select",required:true },
                    {
                        name: "Tenor", text: "Tenor", cls: "tenor span4", type: "select", required: true,
                        items: [
                             { value: "6", text: "6 Bulan" },
                             { value: "12", text: "12 Bulan" },
                             { value: "18", text: "18 Bulan" },
                             { value: "24", text: "24 Bulan" },
                             { value: "32", text: "32 Bulan" },
                             { value: "36", text: "36 Bulan" },
                        ]
                    },
                    { name: "Comment", text: "Customer comments", type: "textarea" },
                    { name: "Additional", text: "Additional inquiries" },
                    { name: "Status", readonly: true, type: "hidden" }
                ]
            },
            {
                title:"",
                name:"Status",
                items: [
                    { name: "StatusInfo", text: "Status", readonly: true, cls: "Status" },
                ]
            }
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
    widget.render();
    widget.lookup.onDblClick(function (e, data, name) {
        console.log(data);
        widget.lookup.hide();
        if (name === "STNKExt") {
            widget.populate($.extend({ PeriodYear: new Date().getFullYear() }, data));
            refreshData(data);
        }
        else {
            $("input[type='text']").val("");
            $('textarea').val("");
            widget.populate($.extend({}, widget.DefaultData, data));
            widget.populate({ IsStnkExtend: false, ReqKtp: false, ReqSuratKuasa: false, ReqBpkb: false, ReqStnk: false });
            $(".toolbar > button").hide();
            widget.showToolbars(["btnBrowse", "btnSave", "btnClear"]);
            $('#STNKRequirement').slideUp();
            $('.Status').show();
        }
    });

    if ($('#CustomerCategory').val() == "")
    {
        $('div.leasing').hide();
        $('div.tenor').hide();
        $('div.bpkb').hide();
    }
    
    widget.render(function () {
        widget.select({ selector: "#LeasingCode", url: "cs.api/Combo/CustLeasing" });
    });

    $('div#Status.panel > div.subtitle').hide();

    $("#btnEdit").on("click", function () {
        widget.showToolbars(["btnSave", "btnCancel"]);
    });

    $("#btnSave").on("click", function () {
        var valid = $(".main form").valid();
        if (valid) {
            var data = $(".main form").serialize();
            data["StnkDate"] = $("[name='StnkDate']").val();
            data["BpkbDate"] = $("[name='BpkbDate']").val();
            widget.post("cs.api/stnkext/save", data, function (result) {
                if (result.success) {
                    //console.log($('[name="CustomerCode"]').val());
                    var data = {CompanyCode: $('#CompanyCode').val(),
                        CustomerCode: $('[name="CustomerCode"]').val(),
                        Chassis: $('#Chassis').val()};
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

    $('#btnDelete').on('click', function () {
        var data = $(".main form").serializeObject();
        if (confirm("Anda yakin akan menghapus data ini?")) {
            widget.post("cs.api/stnkext/delete", data, function (result) {
                if (result.success) {
                    $("#btnClear").click();
                }
            });
        };
    });

    $("#btnClear").on("click", function () {
        $(".toolbar > button").hide();
        $("#btnBrowse").show();
        $("input[type='text']").val("");
        $('textarea').val("");
        $('#CustomerCategory').val('');
        widget.populate({ IsStnkExtend: false, ReqKtp: false, ReqSuratKuasa: false, ReqBpkb: false, ReqStnk: false });
        DisplayCusCategory('');
        $('#STNKRequirement').slideUp();
    });

    $("#btnCustBrowse").on("click", function () {
        widget.lookup.init({
            name: "CustList",
            title: "Customer Buy List",
            source: "cs.api/grid/stnkext?lookupType=browse",
            columns: [
                //{ mData: "CustomerCode", sTitle: "Cust Code", sWidth: "100px" },
                { mData: "CustomerName", sTitle: "Customer Name", sWidth: "200px" },
                { mData: "Address", sTitle: "Address" },
                { mData: "PhoneNo", sTitle: "Phone No" },
                { mData: "PoliceRegNo", sTitle: "No Polisi", sWidth: "100px" },
                { mData: "SalesmanName", sTitle: "Salesman" },
                { mData: "Chassis", sTitle: "Chassis", bVisible: false, bSearchable: false },
                {
                    mData: "StnkExpiredDate", sTitle: "Exp. Date", bVisible: true, bSearchable: false, sWidth: "120px",
                    mRender: function (data, type, full) {
                        if (widget.isNullOrEmpty(data)) {
                            return "-";
                        }

                        return widget.toDateFormat(widget.cleanJsonDate(data));
                    }
                },
            ]
        });
        widget.lookup.show();
    });

    $("#btnBrowse").on("click", function () {
        widget.lookup.init({
            name: "STNKExt",
            title: "STNK Extension List",
            source: "cs.api/grid/StnkExt?lookupType=open",
            columns: [
                { mData: "CustomerCode", sTitle: "Code", sWidth: "80px" },
                { mData: "CustomerName", sTitle: "Name" },
                { mData: "Address", sTitle: "Address" },
                { mData: "PhoneNo", sTitle: "Phone No", sWidth: "90px" },
                { mData: "PoliceRegNo", sTitle: "No.Polisi", sWidth: "90px" },
                { mData: "SalesmanName", sTitle: "Salesman" },
                { mData: "StatusInfo", sTitle: "Status", sWidth: "90px" },
                { mData: "Chassis", sTitle: "Chassis", bVisible: false, bSearchable: false },
                { mData: "CreatedDate", sTitle: "Input", bSearchable: false, sWidth: "90px" },
                { mData: "FinishDate", sTitle: "Finish", bSearchable: false, sWidth: "90px" },
            ]
        });

        widget.lookup.show();
    });

    function DisplayCusCategory(Category)
    {
        //console.log("Display customer category : " + Category);
        if (Category == "L") {
            $('div.bpkb').slideUp(function () {
                $('div.bpkb > label').text('Surat Keterangan');
            });
            //console.log(Category);
            $('div.bpkb').slideDown();
            $('div.leasing').slideDown();
            $('div.tenor').slideDown();
        } else if (Category == "T") {
            $('div.bpkb').slideUp(function () {
                $('div.bpkb > label').text('BPKB Asli');
            });
            //console.log(Category);
            $('div.bpkb').slideDown();
            $('div.leasing').slideUp();
            $('div.tenor').slideUp();
        }
        else {
            //console.log(Category);
            $('div.bpkb').slideUp();
            $('div.leasing').slideUp();
            $('div.tenor').slideUp();
            $('#LeasingCode').val('');
            $('#Tenor').val('');
        }
    }
    $('#CustomerCategory').on("change", function (event) {
        var customerCategory = $(this).val();
        //DisplayCusCategory(event.currentTarget.value);
        DisplayCusCategory(customerCategory);
    });

    $('#STNKRequirement').hide();
    $('#IsStnkExtendY, #IsStnkExtendN').on("change", function (event) {
        setTimeout(function () {
            if ($('#IsStnkExtendY').val() == "false") {
                $('#STNKRequirement').slideUp();
            } else {
                $('#STNKRequirement').slideDown();
            }
        }, (SimDms.switchChangeDelay || 1000));
    });

    function refreshData(data) {
        var data = {
            CompanyCode: data["CompanyCode"],
            CustomerCode: data["CustomerCode"],
            Chassis: data["Chassis"]
        };
        widget.post("cs.api/stnkext/get", data, function (result) {
            if (result.success) {
                widget.populate(result.data);
                setTimeout(function () {
                    DisplayCusCategory($('#CustomerCategory').val());
                }, SimDms.defaultTimeout);
                //DisplayCusCategory($('#CustomerCategory').val());
                $(".toolbar > button").hide();
                widget.showToolbars(["btnBrowse", "btnSave", "btnDelete", "btnClear"]);
                if (result.data["IsStnkExtend"] == false) {
                    $('#STNKRequirement').slideUp();
                } else {
                    $('#STNKRequirement').slideDown();
                }
            }
        });
    }
});