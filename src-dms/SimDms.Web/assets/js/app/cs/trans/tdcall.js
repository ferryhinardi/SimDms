﻿$(document).ready(function () {
    var options = {
        title: "3 Days Call",
        xtype: "panels",
        panels: [
            {
                title: "Customer Details - Vehicle",
                items: [
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
                    {
                        text: "Customer",
                        type: "controls",
                        items: [
                            { name: "CustomerCode", cls: "span2", placeHolder: "Branch Code", readonly: true, type: "popup" },
                            { name: "CustomerName", cls: "span6", placeHolder: "Branch Name", readonly: true }
                        ]
                    },
                    { name: "Address", text: "Address", type: "textarea", readonly: true },
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
                title: "Additional Data",
                items: [
                    { name: "AddPhone1", text: "Additional Phone 1", cls: "span4" },
                    { name: "AddPhone2", text: "Additional Phone 2", cls: "span4" },
                    { name: "BirthDate", text: "Birth Date", cls: "span4", type: "kdatepicker" },
                    { name: "ReligionCode", text: "Religion", cls: "span4", type: "select", required: true },
                ]
            },
            {
                title: "Check Delivery SOP Penyerahan Kendaraan",
                items: [
                    { name: "IsDeliveredA", text: "a) Penjelasan isi Buku manual", type: "switch-full" },
                    { name: "IsDeliveredB", text: "b) Penjelasan Fitur keamanan", type: "switch-full" },
                    { name: "IsDeliveredC", text: "c) Penjelasan Jadwal servis berkala", type: "switch-full" },
                    { name: "IsDeliveredD", text: "d) Penjelasan Garansi", type: "switch-full" },
                    { name: "IsDeliveredE", text: "e) Kartu nama PIC Servis / bengkel", type: "switch-full" },
                    { name: "IsDeliveredF", text: "f) Customer Feedback Card", type: "switch-full" },
                    { name: "IsDeliveredG", text: "g) Thank you letter", type: "switch-full" },
                    { name: "Comment", text: "Customer Voice", cls: "full-length", type: "textarea" },
                    { name: "Additional", text: "Additional inquiries", cls: "full-length hide" },
                    { name: "Finish", text: "Finish", cls: "span2", type: "switch", float: "left" },
                    { name: "Reason", text: "Alasan", cls: "span6", type: "select" },
                    { name: "field_kosong", text: "  ", cls: "span6", type: "divider" }
                ]
            },
        ],
        toolbars: [
            { name: "btnNew", text: "New", icon: "icon-file", cls: "hide" },
            { name: "btnBrowse", text: "Browse", icon: "icon-search" },
            { name: "btnEdit", text: "Edit", icon: "icon-edit", cls: "hide" },
            { name: "btnSave", text: "Save", icon: "icon-save", cls: "hide" },
            { name: "btnCancel", text: "Cancel", icon: "icon-undo", cls: "hide" },
            { name: "btnDelete", text: "Delete", icon: "icon-trash", cls: "hide" },
        ],
    }
    var widget = new SimDms.Widget(options);
    widget.default = {};
    widget.render(initial);

    function initial() {
        var temp = [];
        widget.klookup({
            name: "btnBrowse",
            title: "3 Day Call History",
            url: "cs.api/lookup/CsTDaysCall",
            params: { Outstanding: "N" },
            sort: ({ field: "DeliveryDate", dir: "desc" }),
            serverBinding: true,
            pageSize: 14,
            filters: [
                {
                    text: "Filter",
                    type: "controls",
                    left: 80,
                    items: [
                        { name: "fltCustName", text: "Customer Name", cls: "span4" },
                        { name: "fltVinNo", text: "Vin No", cls: "span2" },
                        { name: "fltPolReg", text: "Police No", cls: "span2" },
                    ]
                }
            ],
            columns: [
                { field: "BranchCode", title: "Outlet", width: 100, template: "#= (Status == 0) ? '<span class=font-red>' + BranchCode + '</span>' : BranchCode #" },
                { field: "CustomerCode", title: "ID Cust", width: 100, template: "#= (Status == 0) ? '<span class=font-red>' + CustomerCode + '</span>' : CustomerCode #" },
                { field: "CustomerName", title: "Customer Name", width: 350, template: "#= (Status == 0) ? '<span class=font-red>' + CustomerName + '</span>' : CustomerName #" },
                { field: "Chassis", title: "Vin No", width: 160, template: "#= (Status == 0) ? '<span class=font-red>' + Chassis + '</span>' : Chassis #" },
                { field: "SalesModelCode", title: "Sales Model", width: 150, template: "#= (Status == 0) ? '<span class=font-red>' + SalesModelCode + '</span>' : SalesModelCode #" },
                { field: "SalesModelYear", title: "Year", width: 80, filterable: false, template: "#= (Status == 0) ? '<span class=font-red>' + SalesModelYear + '</span>' : SalesModelYear #" },
                { field: "PoliceRegNo", title: "Police No", width: 100, template: "#= (Status == 0) ? '<span class=font-red>' + PoliceRegNo + '</span>' : PoliceRegNo #" },
                { field: "DeliveryDate", title: "Delivery Date", width: 120, filterable: false, template: "#= (DeliveryDate == undefined) ? '' : ((Status == 0) ? '<span class=font-red>' + moment(DeliveryDate).format('DD MMM YYYY') + '</span>' : moment(DeliveryDate).format('DD MMM YYYY')) #" },
                { field: "BPKDate", title: "BPK Date", width: 120, filterable: false, template: "#= (BPKDate == undefined) ? '' : ((Status == 0) ? '<span class=font-red>' + moment(BPKDate).format('DD MMM YYYY') + '</span>' : moment(BPKDate).format('DD MMM YYYY')) #" },
                { field: "Status", title: "Status", width: 100, template: "#= (Status == 1) ? 'FINISH' : '<span class=font-red>NOT FINISH</span>' #" },
            ],
            onSelected: function (data) {
                var params = { CompanyCode: data.CompanyCode, CustomerCode: data.CustomerCode, Chassis: data.Chassis };
                refreshData(params);
            },
        });
        widget.klookup({
            name: "btnCustomerCode",
            title: "3 Day Call History",
            url: "cs.api/lookup/CsTDaysCall",
            params: { Outstanding: "Y" },
            sort: ({ field: "DeliveryDate", dir: "desc" }),
            serverBinding: true,
            pageSize: 14,
            filters: [
                {
                    text: "Filter",
                    type: "controls",
                    left: 80,
                    items: [
                        { name: "fltCustName", text: "Customer Name", cls: "span4" },
                        { name: "fltVinNo", text: "Vin No", cls: "span2" },
                        { name: "fltPolReg", text: "Police No", cls: "span2" },
                    ]
                }
            ],
            columns: [
                { field: "BranchCode", title: "Outlet", width: 100 },
                { field: "CustomerCode", title: "ID Cust", width: 100 },
                { field: "CustomerName", title: "Customer Name", width: 350 },
                { field: "Chassis", title: "Vin No", width: 160 },
                { field: "SalesModelCode", title: "Sales Model", width: 150 },
                { field: "SalesModelYear", title: "Year", width: 80, filterable: false },
                { field: "PoliceRegNo", title: "Police No", width: 100 },
                { field: "DeliveryDate", title: "Delivery Date", width: 120, filterable: false, template: "#= (DeliveryDate == undefined) ? '' : moment(DeliveryDate).format('DD MMM YYYY') #" },
                { field: "BPKDate", title: "BPK Date", width: 120, filterable: false, template: "#= (BPKDate == undefined) ? '' : moment(BPKDate).format('DD MMM YYYY') #" },
            ],
            onSelected: function (data) {
                var params = { CompanyCode: data.CompanyCode, CustomerCode: data.CustomerCode, Chassis: data.Chassis };
                refreshData(params);
            }
        });
        widget.post("cs.api/tdaycall/default", function (result) {
            widget.default = result;
            widget.populate(widget.default);
        });

        widget.select({ name: "ReligionCode", url: "cs.api/combo/lookups/rlgn" });

        widget.setSelect([
            { name: "Reason", url: "cs.api/Combo/Reasons/" },
        ]);

    }

    function refreshData(params) {
        if (params === undefined) {
            params = {
                CompanyCode: $("[name=CompanyCode]").val(),
                CustomerCode: $("[name=CustomerCode]").val(),
                Chassis: $("[name=Chassis]").val()
            }
        }
        
        if (!widget.isNullOrEmpty(params.Chassis)) {
            widget.post("cs.api/tdaycall/get", params, function (result) {
                if (result.success) {
                    var data = result.data;

                    data = purifyData(data);

                    widget.populate(data);
                    if (result.isNew === undefined || result.isNew) {
                        widget.showToolbars(["btnNew", "btnBrowse", "btnSave"]);
                    }
                    else {
                        widget.showToolbars(["btnNew", "btnBrowse", "btnSave", "btnDelete"]);
                    }
                }
            });
        }
        else {
            widget.showNotification('Maaf, data terpilih tidak valid.\nNomor Chassis masih kosong, tolong cek data penjualan customer terpilih pada SDMS.'
                                    + '\nJika ada data yang kurang sesuai, tolong disesuaikan terlebih dahulu.'
                                    );
        }
    }

    $("#Finish").on("change", function () {
        setTimeout(function () {
            console.log("test")
        }, 500)
    })

    $("#btnNew").on("click", function () {
        widget.showToolbars(["btnBrowse"]);
        $("input[type='text'],textarea,select").val("");
        widget.populate(widget.default);
        widget.populate({ IsDeliveredA: false, IsDeliveredB: false, IsDeliveredC: false, IsDeliveredD: false, IsDeliveredE: false, IsDeliveredF: false, IsDeliveredG: false });
    });

    $("#btnSave").on("click", function () {

        var data = $(".main form").serializeObject();
        data.Status = ((data.Finish == "true") ? 1 : 0);
        console.log(data)

        if (data.Finish == "false" && data.Reason == "") {
            $('#Reason').focus();
            widget.Error("Silahkan input field Alasan");

            var puterror = setInterval(function () {
                $('#Reason').addClass('error');
                setTimeout(function () { if ($('#Reason').hasClass("error")) { $('#Reason').removeClass('error'); } }, 250);
            }, 300);
            setTimeout(function () { clearInterval(puterror); }, 2400);

            return;
        }

        widget.post("cs.api/tdaycall/save", data, function (result) {
            if (result.success) {
                widget.showNotification("Data berhasil disimpan.");
                refreshData(data);
            }

        });
    });

    $("#btnDelete").on("click", function () {
        var data = $(".main form").serializeObject();

        widget.confirm("Anda yakin akan menghapus data ini?", function (result) {
            if (result == "Yes") {
                widget.post("cs.api/tdaycall/delete", data, function (result) {
                    if (result.success) {
                        $("#btnNew").click();
                    }
                });
            }
        });
    });

    $("#FinishN").on("click", function () {
        $("[name=Reason]").parent().parent().slideDown();
    });

    $("#FinishY").on("click", function () {
        $("[name=Reason]").parent().parent().slideUp();
    });

    function purifyData(data) {
        var purifiedData = data;
       console.log(data)

        if (purifiedData.IsDeliveredA == "Ya") {
            purifiedData.IsDeliveredA = true;
        }
        else {
            purifiedData.IsDeliveredA = false;
        }

        if (purifiedData.IsDeliveredB == "Ya") {
            purifiedData.IsDeliveredB = true;
        }                           
        else {                      
            purifiedData.IsDeliveredB = false;
        }

        if (purifiedData.IsDeliveredC == "Ya") {
            purifiedData.IsDeliveredC = true;
        }
        else {
            purifiedData.IsDeliveredC = false;
        }

        if (purifiedData.IsDeliveredD == "Ya") {
            purifiedData.IsDeliveredD = true;
        }
        else {
            purifiedData.IsDeliveredD = false;
        }

        if (purifiedData.IsDeliveredE == "Ya") {
            purifiedData.IsDeliveredE = true;
        }
        else {
            purifiedData.IsDeliveredE = false;
        }

        if (purifiedData.IsDeliveredF == "Ya") {
            purifiedData.IsDeliveredF = true;
        }
        else {
            purifiedData.IsDeliveredF = false;
        }

        if (purifiedData.IsDeliveredG == "Ya") {
            purifiedData.IsDeliveredG = true;
        }
        else {
            purifiedData.IsDeliveredG = false;
        }

        if (purifiedData.Status == 1) {
            purifiedData.Finish = true;
            $("[name=Reason]").parent().parent().slideUp();
        }
        else {
            purifiedData.Finish = false;
            $("[name=Reason]").parent().parent().slideDown();
        }

        return purifiedData;
    }
});