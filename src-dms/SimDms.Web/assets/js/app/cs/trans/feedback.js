$(document).ready(function () {
    var options = {
        title: "Customer Feedback",
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
                    { name: "Address", text: "Address", type: "textarea", readonly: true },
                    { name: "PhoneNo", text: "Telephone", readonly: true },
                ]
            },
            {
                items: [
                    { name: "CarType", text: "Car Type", cls: "span4", readonly: true },
                    { name: "PoliceRegNo", text: "No Polisi", cls: "span4", readonly: true },
                    { name: "Engine", text: "Engine", cls: "span4", readonly: true },
                    { name: "Chassis", text: "Chassis", cls: "span4", readonly: true },
                ]
            },
            {
                title: "Customer Feedback",
                items: [
                    { name: "FeedbackA", text: "1. Bagaimana pendapat Bapak/Ibu tentang fasilitas kenyamanan showroom kami?", placeHolder: "...", cls: "full-length", type: "textarea" },
                    { name: "FeedbackB", text: "2. Apakah Bapak/Ibu mendapatkan semua yang telah disepakati  sesuai yang tertera di SPK", placeHolder: "...", cls: "full-length", type: "textarea" },
                    { name: "FeedbackC", text: "3. Bagaimana pelayanan kami secara keseluruhan?", placeHolder: "...", cls: "full-length", type: "textarea" },
                    { name: "FeedbackD", text: "4. Apakah ada saran dari Bapak/Ibu agar kami dapat memberikan pelayanan yang lebih baik", placeHolder: "...", cls: "full-length", type: "textarea" },
                ]
            },
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
        widget.post("cs.api/feedback/default", function (result) {
            widget.default = result;
            widget.populate(widget.default);
        });
    });
    widget.lookup.onDblClick(function (e, data, name) {
        widget.lookup.hide();
        widget.populate(data);

        if (name === "FeedbackList") {
            widget.showToolbars(["btnNew", "btnBrowse", "btnSave", "btnDelete"]);
        }
        if (name === "CustList") {
            widget.showToolbars(["btnNew", "btnBrowse", "btnSave"]);
        }
    });

    $("#btnNew").on("click", function () {
        $("input[type='text'],textarea,select").not("[name=CompanyCode],[name=CompanyName]").val("");

        widget.showToolbars(["btnBrowse"]);
        widget.populate(widget.default);
    });

    $("#btnBrowse").on("click", function () {
        widget.lookup.init({
            name: "FeedbackList",
            title: "Customer Feedback List",
            source: "cs.api/grid/custfeedback",
            additionalParams: [{ name: "IsFeedback", value: "Y" }],
            columns: [
                { mData: "CustomerCode", sTitle: "Outlet", sWidth: "100px" },
                { mData: "CustomerName", sTitle: "Customer Name", sWidth: "200px" },
                { mData: "PhoneNo", sTitle: "Phone No", sWidth: "100px" },
                { mData: "PoliceRegNo", sTitle: "Polisi No", sWidth: "120px" },
                { mData: "ServiceBookNo", sTitle: "Srv. Book No", sWidth: "120px" },
            ]
        });
        widget.lookup.show();
    });

    $("#btnCustBrowse").on("click", function () {
        widget.lookup.init({
            name: "CustList",
            title: "Customer List",
            source: "cs.api/grid/custfeedback",
            additionalParams: [{ name: "IsFeedback", value: "N" }],
            columns: [
                { mData: "CustomerCode", sTitle: "Outlet", sWidth: "100px" },
                { mData: "CustomerName", sTitle: "Customer Name", sWidth: "200px" },
                { mData: "PhoneNo", sTitle: "Phone No", sWidth: "100px" },
                { mData: "PoliceRegNo", sTitle: "Polisi No", sWidth: "120px" },
                { mData: "ServiceBookNo", sTitle: "Srv. Book No", sWidth: "120px" },
            ]
        });
        widget.lookup.show();
    });

    $("#btnSave").on("click", function () {
        var data = $(".main form").serialize();
        widget.post("cs.api/feedback/save", data, function (result) {
            if (result.success) {
                widget.populate(result.data);
                widget.showToolbars(["btnNew", "btnBrowse", "btnSave", "btnDelete"]);
            }
        });
    });

    $("#btnDelete").on("click", function () {
        var params = {
            CustomerCode: $("[name=CustomerCode]").val(),
            Chassis: $("[name=Chassis]").val(),
        };
        if (confirm("Anda yakin akan menghapus data ini?")) {
            widget.post("cs.api/feedback/delete", params, function (result) {
                if (result.success) {
                    $("#btnNew").click();
                }
            });
        };
    });
});