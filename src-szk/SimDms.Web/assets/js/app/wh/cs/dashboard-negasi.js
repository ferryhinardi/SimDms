﻿$(document).ready(function () {
    var options = {
        title: "Customer Satisfaction - Processed Data",
        xtype: "panels",
        toolbars: [
            { name: "btnRefresh", text: "Refresh", icon: "fa fa-refresh" },
            { name: "btnBack", text: "Back", icon: "fa fa-hand-o-left", cls: "hide" },
            { name: "btnExportXls", text: "Export (xls)", icon: "fa fa-file-excel-o" },
        ],
        panels: [
            {
                items: [
                    { name: "CompanyCode", text: "Dealer Name", type: "select", cls: "span6 full", readonly: false },
                    { name: "BranchCode", text: "Branch Name", opt_text: "-- SELECT ALL --", type: "select", cls: "span6 full", readonly: false },
                    //{ name: "CompanyName", text: "Dealer", cls: "span6", readonly: true },
                    //{ name: "BranchName", text: "Outlet", cls: "span6", readonly: true },
                    { name: "ReminderDate", text: "Date of Reminder", cls: "span3", type: "date", readonly: true },
                ]
            },
            {
                name: "pnlCro",
                title: "C. R. O.",
                items: [
                    { name: "3DaysCall", text: "3 Days Call", cls: "span6 full", type: "popup", icon: "fa fa-hand-o-right", readonly: true },
                    { name: "BDayCall", text: "Birthday Call", cls: "span6 full", type: "popup", icon: "fa fa-hand-o-right", readonly: true },
                    { name: "BpkbRemind", text: "BPKB Reminder", cls: "span6 full", type: "popup", icon: "fa fa-hand-o-right", readonly: true },
                    { name: "StnkExt", text: "STNK Extension", cls: "span6 full", type: "popup", icon: "fa fa-hand-o-right", readonly: true },
                    //{ name: "Holiays", text: "Holidays", cls: "span6 full", type: "popup", icon: "fa fa-hand-o-right", readonly: true },
                ]
            },
            {
                name: "pnl3DayCall",
                title: "3 Days Call List",
                xtype: "k-grid",
            },
            {
                name: "pnlBirthDayCall",
                title: "BirthDay Call List",
                xtype: "k-grid",
            },
            {
                name: "pnlStnkExt",
                title: "STNK Extension List",
                xtype: "k-grid",
            },
            {
                name: "pnlBpkb",
                title: "BPKB Reminder List",
                xtype: "k-grid",
            },
            //{
            //    name: "pnlHoliday",
            //    title: "Holidays List",
            //    xtype: "kgrid",
            //},
        ]
    };
    var widget = new SimDms.Widget(options);
    widget.default = {};
    widget.render(init);

    function init() {
        $("#ReminderDate").val(widget.toDateFormat(new Date()));
        showPanel("pnlCro");
        setElementEvent();
        refresh();
        widget.hideAccordion();
        widget.setSelect([
            {
                name: "CompanyCode",
                url: "wh.api/Combo/DealerList",
                params: {
                    LinkedModule: 'CS'
                }
            },
            {
                name: "BranchCode",
                url: "wh.api/Combo/Branchs",
                optionalText: "-- SELECT ALL --",
                cascade: {
                    name: "CompanyCode",
                    additionalParams: [
                        {
                            name: "comp",
                            source: "CompanyCode"
                        }
                    ]
                }
            }
        ]);
    }

    function setElementEvent() {
        $("[name='CompanyCode'], [name='BranchCode']").on("change", function (evt) {
            $("#btnRefresh").click();
        });
    }

    function showPanel(name) {
        var hide = [];
        var show = [];
        $.each(options.panels, function (idx, val) {
            if (val !== undefined && val.name !== undefined) {
                if (val.name == name) {
                    switch (val.xtype) {
                        case "kgrid":
                            show.push({ name: val.name, type: val.xtype });
                            break;
                        default:
                            show.push(val.name);
                            break;
                    }
                }
                else {
                    switch (val.xtype) {
                        case "kgrid":
                            hide.push({ name: val.name, type: val.xtype });
                            break;
                        default:
                            hide.push(val.name);
                            break;
                    }
                }
            }
        });
        options["ActivePanel"] = name;
        widget.hidePanel(hide);
        widget.showToolbars((show.length > 0 && show[0] == "pnlCro") ? ["btnRefresh"] : ["btnBack", "btnExportXls"]);
        setTimeout(function () {
            widget.showPanel(show);
        }, 500);
    }

    function back() {
        showPanel(["pnlCro"]);
        $("[name='CompanyCode'], [name='BranchCode']").removeAttr("disabled");
    }

    function refresh() {
        var params = {
            CompanyCode: $("[name='CompanyCode']").val(),
            BranchCode: $("[name='BranchCode']").val(),
            Outstanding: 'N'
        };

        $("#3DaysCall, #BDayCall, #StnkExt, #BpkbRemind").val("0");
        widget.post("wh.api/Inquiry/CsDashDefaultNotOutstanding", params, function (result) {
            if (result.success) {
                widget.default = result.data;
                widget.populate(widget.default);

                var record = {};
                $.each(result.list, function (idx, val) {
                    record[val.ControlLink] = val.RemValue;
                })
                widget.populate(record);
            }
        });
    }

    function load3DaysCall() {
        showPanel(["pnl3DayCall"]);
        $("[name='CompanyCode'], [name='BranchCode']").attr("disabled", true);

        options["3DayCall"] = widget.kgrid({
            name: "pnl3DayCall",
            url: "wh.api/Inquiry/tdaycalls",
            params: {
                OutStanding: "N",
                CompanyCode: $("#CompanyCode").val(),
                BranchCode: $("#BranchCode").val()
            },
            serverBinding: true,
            pageSize: 8,
            columns: [
                { field: "CustomerCode", title: "Customer Code", width: 140 },
                { field: "CustomerName", title: "Customer Name", width: 240 },
                { field: "Chassis", title: "Vin No", width: 140 },
                { field: "PoliceRegNo", title: "Police No", width: 140 },
                { field: "DODate", title: "DO Date", width: 140, template: "#= (DODate == undefined) ? '' : moment(DODate).format('DD MMM YYYY') #" },
            ],
        });
    };

    function loadBirthDayCall() {
        showPanel(["pnlBirthDayCall"]);
        $("[name='CompanyCode'], [name='BranchCode']").attr("disabled", true);

        options["BirthDayCall"] = widget.kgrid({
            name: "pnlBirthDayCall",
            url: "wh.api/Inquiry/CustomerBirthDays",
            params: {
                OutStanding: "N",
                CompanyCode: $("#CompanyCode").val(),
                BranchCode: $("#BranchCode").val()
            },
            serverBinding: true,
            pageSize: 8,
            columns: [
                { field: "CustomerCode", title: "Customer Code", width: 140 },
                { field: "CustomerName", title: "Customer Name", width: 240 },
                { field: "CustomerTelephone", title: "Telephone", width: 100 },
                { field: "CustomerBirthDate", title: "Birth Date", width: 140, template: "#= (CustomerBirthDate == undefined) ? '' : moment(CustomerBirthDate).format('DD MMM YYYY') #" },
            ],
        });
    }

    function loadStnkExt() {
        showPanel(["pnlStnkExt"]);
        $("[name='CompanyCode'], [name='BranchCode']").attr("disabled", true);

        options["StnkExt"] = widget.kgrid({
            name: "pnlStnkExt",
            url: "wh.api/Inquiry/stnkext",
            params: {
                OutStanding: "N",
                CompanyCode: $("#CompanyCode").val(),
                BranchCode: $("#BranchCode").val()
            },
            serverBinding: true,
            pageSize: 8,
            columns: [
                { field: "CustomerCode", title: "Customer Code", width: 140 },
                { field: "CustomerName", title: "Customer Name", width: 240 },
                { field: "Chassis", title: "Vin No", width: 140 },
                { field: "PoliceRegNo", title: "Police No", width: 140 },
                { field: "StnkExpiredDate", title: "Stnk Expired Date", width: 160, template: "#= (StnkExpiredDate == undefined) ? '' : moment(StnkExpiredDate).format('DD MMM YYYY') #" },
            ],
        });
    }

    function loadBpkb() {
        showPanel(["pnlBpkb"]);
        $("[name='CompanyCode'], [name='BranchCode']").attr("disabled", true);

        options["Bpkb"] = widget.kgrid({
            name: "pnlBpkb",
            url: "wh.api/Inquiry/bpkbs",
            params: {
                OutStanding: "N",
                CompanyCode: $("#CompanyCode").val(),
                BranchCode: $("#BranchCode").val()
            },
            serverBinding: true,
            pageSize: 8,
            columns: [
                { field: "CustomerCode", title: "Customer Code", width: 140 },
                { field: "CustomerName", title: "Customer Name", width: 240 },
                { field: "Chassis", title: "Vin No", width: 140 },
                { field: "PoliceRegNo", title: "Police No", width: 140 },
                { field: "BPKDate", title: "BPKB Date", width: 160, template: "#= (BPKDate == undefined) ? '' : moment(BPKDate).format('DD MMM YYYY') #" },
            ],
        });
    }

    function exportXls() {
        var names = options["ActivePanel"];

        $.each(names, function (idx, val) {
            switch (val) {
                case "pnl3DayCall":
                    widget.exportXls({
                        source: "wh.api/Report/tdaycalls",
                        params: {
                            OutStanding: "N",
                            CompanyCode: $("[name='CompanyCode']").val(),
                            BranchCode: $("[name='BranchCode']").val(),
                        },
                        type: "kgridlnk",
                        fileName: "3DayCall",
                        items: [
                            { name: "CustomerCode", text: "Customer Code" },
                            { name: "CustomerName", text: "Customer Name" },
                            { name: "Chassis", text: "Vin No" },
                            { name: "PoliceRegNo", text: "Police No" },
                            { name: "DODate", text: "DO Date", type: "date" },
                        ]
                    });
                    break;
                case "pnlBirthDayCall":
                    widget.exportXls({
                        source: "wh.api/Report/CustomerBirthDays",
                        params: {
                            OutStanding: "N",
                            CompanyCode: $("[name='CompanyCode']").val(),
                            BranchCode: $("[name='BranchCode']").val(),
                        },
                        type: "kgridlnk",
                        fileName: "BirthDayCall",
                        items: [
                            { name: "CustomerCode", text: "Customer Code" },
                            { name: "CustomerName", text: "Customer Name" },
                            { name: "CustomerTelephone", text: "Telephone" },
                            { name: "PoliceRegNo", text: "Police No" },
                            { name: "CustomerBirthDate", text: "Birth Date", type: "date" },
                        ]
                    });
                    break;
                case "pnlStnkExt":
                    widget.exportXls({
                        source: "wh.api/Report/stnkext",
                        params: {
                            OutStanding: "N",
                            CompanyCode: $("[name='CompanyCode']").val(),
                            BranchCode: $("[name='BranchCode']").val(),
                        },
                        type: "kgridlnk",
                        fileName: "StnkExt",
                        items: [
                            { name: "CustomerCode", text: "Customer Code" },
                            { name: "CustomerName", text: "Customer Name" },
                            { name: "Chassis", text: "Vin No" },
                            { name: "PoliceRegNo", text: "Police No" },
                            { name: "StnkExpiredDate", text: "Stnk Expired Date", type: "date" },
                        ]
                    });
                    break;
                case "pnlBpkb":
                    widget.exportXls({
                        source: "wh.api/Report/bpkbs",
                        params: {
                            OutStanding: "N",
                            CompanyCode: $("[name='CompanyCode']").val(),
                            BranchCode: $("[name='BranchCode']").val(),
                        },
                        type: "kgridlnk",
                        fileName: "BpkbReminder",
                        items: [
                            { name: "CustomerCode", text: "Customer Code" },
                            { name: "CustomerName", text: "Customer Name" },
                            { name: "Chassis", text: "Vin No" },
                            { name: "PoliceRegNo", text: "Police No" },
                            { name: "BPKDate", text: "BPK Date", type: "date" },
                        ]
                    });
                    break;
                default:
                    break;
            }
        });
    }

    $("#btnRefresh").on("click", refresh);
    $("#btnExportXls").on("click", exportXls);
    $("#btnBack").on("click", back);
    
    $("#btn3DaysCall").on("click", load3DaysCall);
    $("#btnBDayCall").on("click", loadBirthDayCall);
    $("#btnStnkExt").on("click", loadStnkExt);
    $("#btnBpkbRemind").on("click", loadBpkb);
    //$("#btnHoliays").on("click", loadHoliday);
});