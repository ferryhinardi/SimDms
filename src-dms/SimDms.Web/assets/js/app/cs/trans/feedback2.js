$(document).ready(function () {
    var options = {
        title: "Customer Feedback",
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
                    { name: "SalesModelCode", text: "Car Type", readonly: true },
                    { name: "ColourCode", text: "Warna", readonly: true },
                    { name: "PoliceRegNo", text: "No Polisi", readonly: true },
                    { name: "Engine", text: "Engine", readonly: true },
                    { name: "Chassis", text: "Chassis", readonly: true },
                    { name: "SalesmanName", text: "Sales Name", readonly: true },
                ]
            },
            {
                title: "Customer Feedback",
                items: [
                    { name: "IsManual", text: "Manual Feedback ?", type: "switch-full" },
                ]
            },
            {
                name: "pnlFeedback",
                items: [
                    { name: "FeedbackA", text: "1. Bagaimana pendapat Bapak/Ibu tentang fasilitas kenyamanan showroom kami?", placeHolder: "...", cls: "full-length", type: "textarea" },
                    { name: "FeedbackB", text: "2. Apakah Bapak/Ibu mendapatkan semua yang telah disepakati  sesuai yang tertera di SPK", placeHolder: "...", cls: "full-length", type: "textarea" },
                    { name: "FeedbackC", text: "3. Bagaimana pelayanan kami secara keseluruhan?", placeHolder: "...", cls: "full-length", type: "textarea" },
                    { name: "FeedbackD", text: "4. Apakah ada saran dari Bapak/Ibu agar kami dapat memberikan pelayanan yang lebih baik", placeHolder: "...", cls: "full-length", type: "textarea" },
                ]
            },
            {
                items: [
                    { name: "Status", text: "Finish", cls: "span2 finish", type: "switch", float: "left" },
                    { name: "Reason", text: "Alasan", cls: "span6", type: "select" },
                ]
            }
        ],
        toolbars: [
            { name: "btnNew", text: "New", icon: "icon-file", cls: "hide" },
            { name: "btnBrowse", text: "Browse", icon: "icon-search" },
            { name: "btnSave", text: "Save", icon: "icon-save", cls: "hide" },
            { name: "btnDelete", text: "Delete", icon: "icon-trash", cls: "hide" },
        ],
    }
    var widget = new SimDms.Widget(options);
    widget.default = {};
    widget.render(function () {
        widget.setSelect([
            { name: "Reason", url: "cs.api/Combo/Reasons/" },
        ]);
        widget.post("cs.api/feedback/default", function (result) {
            widget.default = $.extend({}, result, { IsManual: false });
            widget.populate(widget.default);

            $("[name=IsManual]").on("change", function () {
                setTimeout(function () {
                    var val = $("[name=IsManual]").val();
                    if (val.toString() == "true") {
                        $("#pnlFeedback").slideUp();
                    } else {
                        $("#pnlFeedback").slideDown();
                    }

                }, SimDms.switchChangeDelay || 500);
            });
        });
    });

    $(".finish input").change(function (e) {
        if ($(this).val() == "true")
            $("[name=Reason]").parent().parent().slideUp();
        else
            $("[name=Reason]").parent().parent().slideDown();
    });

    $("#btnNew").on("click", function () {
        $("input[type='text'],textarea,select").val("");
        widget.populate(widget.default);
        widget.showToolbars(["btnBrowse"]);
        $("#pnlFeedback").slideDown();
    });

    $("#btnBrowse,#btnCustomerCode").on("click", function () {
        var id = this.id;
        var lookup = widget.klookup({
            name: "StnkExt",
            title: "Customer List",
            url: "cs.api/lookup/Feedbacks",
            params: { OutStanding: (id == "btnBrowse") ? "N" : "Y" },
            sort: ({ field: "StnkDate", dir: "desc" }),
            serverBinding: true,

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
                { field: "CustomerName", title: "Customer Name", width: 250 },
                { field: "SalesModelYear", title: "Model Year", width: 100 },
                { field: "PoliceRegNo", title: "No Polisi", width: 180 },
                { field: "Chassis", title: "Vin No", width: 180 },
                { field: "SalesModelCode", title: "Sales Model", width: 160 },
                { field: "Feedback", title: "Feedback", width: 120 },
            ],
        });
        lookup.dblClick(refreshData);
    });

    $("#btnSave").on("click", function () {
        var data = $(".main form").serialize();
        widget.post("cs.api/feedback/save", data, function (result) {
            if (result.success) {
                refreshData();
            }
        });
    });

    $("#btnDelete").on("click", function () {
        var data = $(".main form").serializeObject();
        widget.confirm("Anda yakin akan menghapus data ini?", function (result) {
            if (result == "Yes") {
                widget.post("cs.api/feedback/delete", data, function (result) {
                    if (result.success) {
                        $("#btnNew").click();
                    }
                });
            }
        });
    });

    function refreshData(data) {
        var params = undefined;
        if (data === undefined) {
            params = { CompanyCode: $("[name=CompanyCode]").val(), CustomerCode: $("[name=CustomerCode]").val(), Chassis: $("[name=Chassis]").val() };
        } else {
            params = { CompanyCode: data.CompanyCode, CustomerCode: data.CustomerCode, Chassis: data.Chassis };
        }

        widget.post("cs.api/feedback/get", params, function (result) {
            if (result.success) {
                widget.populate(result.data);
                if (result.data.IsNew === 1) {
                    widget.showToolbars(["btnNew", "btnBrowse", "btnSave"]);
                }
                else {
                    widget.showToolbars(["btnNew", "btnBrowse", "btnSave", "btnDelete"]);
                }

                if (result.data.IsManual) {
                    $("#pnlFeedback").slideUp();
                }
                else {
                    $("#pnlFeedback").slideDown();
                }
            }
        });
    }
});