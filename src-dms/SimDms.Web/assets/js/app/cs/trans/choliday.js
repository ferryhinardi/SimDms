$(document).ready(function () {
    var options = {
        title: "Holiday",
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
                    { name: "GiftSeq", text: "GiftSeq", type: "hidden", cls: "hide", readonly: true },
                    {
                        text: "Customer",
                        type: "lookup",
                        name: "CustomerCode",
                        display: "CustomerName",
                        btnName: "btnCustBrowse"
                    },
                    { name: "Address", text: "Address", readonly: true },
                    { name: "PhoneNo", text: "Telephone", readonly: true },
                    //{ name: "BpkDate", text: "Tgl BPK", cls: "span4", readonly: true },
                    //{ name: "StnkDate", text: "Tgl STNK", cls: "span4", readonly: true },
                    {
                        name: "ReligionCode", text: "Religion", type: "select", cls: "span4", required: true,
                        items: [
                            { value: "ISL", text: "Islam" },
                            { value: "KRS", text: "Kristen" },
                            { value: "KTH", text: "Katholik" },
                            { value: "HND", text: "Hindu" },
                            { value: "BDH", text: "Budha" },
                            { value: "OTH", text: "Other" },
                        ]
                    },
                    { name: "PeriodYear", text: "Year", readonly: true, cls: "span4" },
                ]
            },
            {
                title: "Type of Gift",
                items: [
                    { name: "HolidayCode", text: "Holiday", type: "select", cls: "span8 full", required: true },
                    { name: "IsGiftCard", text: "Gift Card", type: "switch", cls: "span4", float: "left" },
                    { name: "IsGiftLetter", text: "Gift Letter", type: "switch", cls: "span4", float: "left" },
                    { name: "IsGiftSms", text: "Gift Sms", type: "switch", cls: "span4", float: "left" },
                    { name: "IsGiftSouvenir", text: "Gift Souvenir", type: "switch", cls: "span4", float: "left" },
                    { name: "SouvenirSent", text: "Souvenir Sent", type: "datepicker", cls: "span4", float: "left" },
                    { name: "SouvenirReceived", text: "Souvenir Received", type: "datepicker", cls: "span4", float: "left" },
                ]
            },
            {
                items: [
                    { name: "Comment", text: "Customer comments", type: "textarea" },
                    { name: "Additional", text: "Additional inquiries", type: "text" },
                    { name: "StatusInfo", text: "Status", readonly: true },
                ]
            },
        ],
        toolbars: [
            { name: "btnClear", text: "New", icon: "icon-undo", cls: "hide" },
            { name: "btnBrowse", text: "Browse", icon: "icon-search" },
            { name: "btnEdit", text: "Edit", icon: "icon-edit", cls: "hide" },
            { name: "btnSave", text: "Save", icon: "icon-save", cls: "hide" },
            { name: "btnCancel", text: "Cancel", icon: "icon-undo", cls: "hide" },
            { name: "btnDelete", text: "Delete", icon: "icon-trash", cls: "hide" },
        ],
    }
    var widget = new SimDms.Widget(options);
    widget.DefaultData = {
        ReligionCode: "",
        PeriodYear: new Date().getFullYear(),
        HolidayCode: "",
        IsGiftCard: false,
        IsGiftLetter: false,
        IsGiftSms: false,
        IsGiftSouvenir: false,
        SouvenirSent: null,
        SouvenirReceived: null,
        Comment: "",
        Additional: "",
        StatusInfo: ""
    }

    widget.render(function () {
        widget.select({ selector: "#HolidayCode", url: "cs.api/combo/holidays" });
    });
    widget.lookup.onDblClick(function (e, data, name) {
        widget.lookup.hide();
        //console.log(name);

        if (name === "CHoliday") {
            widget.populate($.extend({ PeriodYear: new Date().getFullYear() }, data));
            refreshData();
        }
        else {
            widget.populate($.extend({}, widget.DefaultData, data));
            $("[name='HolidayCode']").attr('disabled', false);

            $(".toolbar > button").hide();
            widget.showToolbars(["btnBrowse", "btnSave", "btnClear"]);
        }
    });

    $("#btnBrowse").on("click", function () {
        widget.lookup.init({
            name: "CHoliday",
            title: "Customer Holiday List",
            source: "cs.api/grid/custholidays",
            columns: [
                { mData: "CustomerCode", sTitle: "Cust Code", sWidth: "100px" },
                {
                    mData: "CustomerName", sTitle: "Customer Name", sWidth: "240px",
                    mRender: function (data, type, full) {
                        return data.substring(0, 28);
                    }
                },
                { mData: "HolidayDesc", sTitle: "Holiday", sWidth: "180px" },
                {
                    mData: "Address", sTitle: "Address", sWidth: "300px",
                    mRender: function (data, type, full) {
                        return data.substring(0, 33);
                    }
                },
                { mData: "PhoneNo", sTitle: "Phone No" },
                {
                    mData: "BirthDate", sTitle: "Birth Date", sWidth: "100px",
                    mRender: function (data, type, full) {
                        return moment(data).format('DD MMM YYYY');
                    }
                },
                { mData: "StatusInfo", sTitle: "Status", sWidth: "100px" },
            ]
        });

        widget.lookup.show();
    });

    $("#btnEdit").on("click", function () {
        widget.showToolbars(["btnSave", "btnCancel"]);
    });

    $("#btnSave").on("click", function () {
        var valid = $(".main form").valid();
        if (valid) {
            var data = $(".main form").serializeObject();
            widget.post("cs.api/custholiday/save", data, function (result) {
                if (result.success) {
                    widget.populate(result.data);
                    refreshData();
                }
            });
        }
    });

    $("#btnCancel").on("click", function () {
        refreshData();
    });

    $("#btnDelete").on("click", function () {
        var data = $(".main form").serializeObject();
        if (confirm("Anda yakin akan menghapus data ini?")) {
            widget.post("cs.api/custholiday/delete", data, function (result) {
                if (result.success) {
                    $("#btnClear").click();
                }
            });
        };
    });

    $("#btnClear").on("click", function () {
        widget.showToolbars(["btnBrowse"]);
        widget.populate({ IsGiftCard: false, IsGiftLetter: false, IsGiftSms: false, IsGiftSouvenir: false });
        $("input[type='text'],textarea,select").val("");
    });

    $("#btnCustBrowse").on("click", function () {
        widget.lookup.init({
            name: "CustList",
            title: "Customer List",
            source: "cs.api/grid/customers",
            columns: [
                { mData: "CustomerCode", sTitle: "Cust Code", sWidth: "100px" },
                {
                    mData: "CustomerName", sTitle: "Customer Name", sWidth: "300px",
                    mRender: function (data, type, full) {
                        if (widget.isNullOrEmpty(data)) {
                            return "-";
                        }

                        return data.substring(0, 40);
                    }
                },
                {
                    mData: "Address", sTitle: "Address",
                    mRender: function (data, type, full) {
                        if (widget.isNullOrEmpty(data)) {
                            return "-";
                        }

                        return data.substring(0, 40);
                    }
                },
                { mData: "PhoneNo", sTitle: "Phone No" },
                {
                    mData: "BirthDate", sTitle: "Birth Date", sWidth: "100px",
                    mRender: function (data, type, full) {
                        if (widget.isNullOrEmpty(data)) {
                            return "-";
                        }

                        return moment(data).format('DD MMM YYYY');;
                    }
                },
            ]
        });
        widget.lookup.show();
    });

    $("#HolidayCode").on("change", function () {
        var data = $(".main form").serializeObject();
        widget.post("cs.api/custholiday/getdata", data, function (result) {
            if (result.success) {
                populateData(result.data);
            }
        });
    });

    function refreshData() {
        var data = $(".main form").serializeObject();
        widget.post("cs.api/custholiday/get", data, function (result) {
            if (result.success) {
                populateData(result.data);
            }
        });
    }

    function populateData(data) {
        var xdata = $.extend({}, widget.DefaultData, data);
        widget.populate(xdata);
        $("[name='HolidayCode']").attr('disabled', (xdata.HolidayCode != null && xdata.HolidayCode.length > 0));

        if (data.GiftSeq !== undefined && data.GiftSeq > 0) {
            widget.showToolbars(["btnBrowse", "btnEdit", "btnDelete", "btnClear"]);
        }
        else {
            widget.showToolbars(["btnBrowse", "btnEdit", "btnClear"]);
        }
    }
});

