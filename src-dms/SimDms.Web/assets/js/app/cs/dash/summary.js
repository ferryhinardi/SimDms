$(document).ready(function () {
    var options = {
        title: "Customer Satisfaction",
        xtype: "panels",
        toolbars: [
            { name: "btnRefresh2", text: "Refresh", icon: "icon-refresh" },
            { name: "btnBack", text: "Back", icon: "icon-hand-left", cls: "hide" },
            { name: "btnExportXls", text: "Export (xls)", icon: "icon-xls" },
        ],
        panels: [
            {
                items: [
                    { name: "CompanyName", text: "Dealer", cls: "span6", readonly: true },
                    { name: "BranchName", text: "Outlet", cls: "span6", readonly: true },
                    { name: "RemiderDate", text: "Date of Reminder", cls: "span3", type: "date", readonly: true },
                ]
            },
            {
                name: "pnlCro",
                title: "Today's Reminder",
                items: [
                    { name: "DeliveryOutstanding", text: "Belum Serah Terima Kendaraan", cls: "span6 full", type: "popup", icon: "icon-hand-right", readonly: true },
                    { name: "3DaysCall", text: "3 Days Call", cls: "span6 full", type: "popup", icon: "icon-hand-right", readonly: true },
                    { name: "BDayCall", text: "Birthday Call", cls: "span6 full", type: "popup", icon: "icon-hand-right", readonly: true },
                    { name: "BpkbRemind", text: "BPKB Reminder", cls: "span6 full", type: "popup", icon: "icon-hand-right", readonly: true },
                    { name: "StnkExt", text: "Reminder Perpanjangan STNK", cls: "span6 full", type: "popup", icon: "icon-hand-right", readonly: true },
                    //{ name: "Holiays", text: "Holidays", cls: "span6 full", type: "popup", icon: "icon-hand-right", readonly: true },
                ]
            },
            {
                name: "pnl3DayCall",
                title: "3 Days Call List",
                xtype: "kgrid",
            },
            {
                name: "pnlBirthDayCall",
                title: "BirthDay Call List",
                xtype: "kgrid",
            },
            {
                name: "pnlStnkExt",
                title: "STNK Extension List",
                xtype: "kgrid",
            },
            {
                name: "pnlBpkb",
                title: "BPKB Reminder List",
                xtype: "kgrid",
            },
            {
                name: "pnlDlvry",
                title: "Delivery Outstanding List",
                xtype: "kgrid",
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
        showPanel("pnlCro");
        refresh();
        widget.hideAccordion();
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
        widget.showToolbars((show.length > 0 && show[0] == "pnlCro") ? ["btnRefresh2"] : ["btnBack", "btnExportXls"]);
        setTimeout(function () { widget.showPanel(show); }, 500);
    }

    function back() {
        showPanel(["pnlCro"]);
    }

    function refresh() {
        widget.post("cs.api/summary/default", function (result) {
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

    function refresh2() {
        widget.post("cs.api/summary/default2", function (result) {
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
        if (options["3DayCall"] == undefined) {
            options["3DayCall"] = widget.kgrid({
                name: "pnl3DayCall",
                url: "cs.api/Lookup/CsTDaysCall",
                params: { OutStanding: "Y" },
                serverBinding: true,
                pageSize: 8,
                columns: [
                    { field: "CustomerCode", title: "Customer Code", width: 140 },
                    { field: "CustomerName", title: "Customer Name", width: 240 },
                    { field: "Chassis", title: "Vin No", width: 140 },
                    { field: "PoliceRegNo", title: "Police No", width: 140 },
                    { field: "BPKDate", title: "BPK Date", width: 140, template: "#= (BPKDate == undefined) ? '' : moment(BPKDate).format('DD MMM YYYY') #" },
                    { field: "DeliveryDate", title: "Delivery Date", width: 140, template: "#= (DeliveryDate == undefined) ? '' : moment(DeliveryDate).format('DD MMM YYYY') #" },
                ],
            });
        };
    };

    function loadBirthDayCall() {
        showPanel(["pnlBirthDayCall"]);
        if (options["BirthDayCall"] == undefined) {
            options["BirthDayCall"] = widget.kgrid({
                name: "pnlBirthDayCall",
                url: "cs.api/Lookup/CsCustBirthdays",
                params: { OutStanding: "Y" },
                serverBinding: true,
                pageSize: 8,
                columns: [
                    { field: "CustomerCode", title: "Customer Code", width: 140 },
                    { field: "CustomerName", title: "Customer Name", width: 240 },
                    { field: "CustomerTelephone", title: "Telephone", width: 100 },
                    { field: "CustomerBirthDay", title: "Birth Day", width: 70 },
                    { field: "CustomerBirthDate", title: "Birth Date", width: 80, template: "#= (CustomerBirthDate == undefined) ? '' : moment(CustomerBirthDate).format('MMM YYYY') #", filterable: false/*{ extra: true }*/ },
                ],
                //model: { fields: { CustomerBirthDate: { type: "date" } } },
            });
        };
    }

    function loadStnkExt() {
        showPanel(["pnlStnkExt"]);
        if (options["StnkExt"] == undefined) {
            options["StnkExt"] = widget.kgrid({
                name: "pnlStnkExt",
                url: "cs.api/Lookup/CsStnkExtensions",
                params: { OutStanding: "Y" },
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
        };
    }

    function loadBpkb() {
        showPanel(["pnlBpkb"]);
        if (options["Bpkb"] == undefined) {
            options["Bpkb"] = widget.kgrid({
                name: "pnlBpkb",
                url: "cs.api/Lookup/CsBpkbReminders",
                params: { OutStanding: "Y" },
                serverBinding: true,
                pageSize: 8,
                columns: [
                    { field: "CustomerCode", title: "Customer Code", width: 140 },
                    { field: "CustomerName", title: "Customer Name", width: 240 },
                    { field: "Chassis", title: "Vin No", width: 140 },
                    { field: "PoliceRegNo", title: "Police No", width: 140 },
                    { field: "BpkbDate", title: "BPKB Date", width: 160, template: "#= (BpkbDate == undefined) ? '' : moment(BpkbDate).format('DD MMM YYYY') #" },
                ],
            });
        };
    }

    function loadDlvryOutstanding() {
        showPanel(["pnlDlvry"]);
        if (options["Dlvry"] == undefined) {
            options["Dlvry"] = widget.kgrid({
                name: "pnlDlvry",
                url: "cs.api/Lookup/CsDlvryOutstanding",
                sort: ({ field: "BPKDate", dir: "desc" }),
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
        };
    }
    function exportXls() {
        var names = options["ActivePanel"];
        $.each(names, function (idx, val) {
            switch (val) {
                case "pnlDlvry":
                    widget.exportXls({
                        source: "cs.api/grid/CsDlvryOutstanding",
                        type: "kgridlnk",
                        fileName: "DeliveryOutstanding",
                        items: [
                            { name: "CustomerCode", text: "Customer Code" },
                            { name: "CustomerName", text: "Customer Name" },
                            { name: "Chassis", text: "Vin No" },
                            { name: "PoliceRegNo", text: "Police No" },
                        ]
                    });
                    break;
                case "pnl3DayCall":
                    widget.exportXls({
                        source: "cs.api/grid/CsTDayCalls",
                        params: { OutStanding: "Y" },
                        type: "kgridlnk",
                        fileName: "3DayCall",
                        items: [
                            { name: "CustomerCode", text: "Customer Code" },
                            { name: "CustomerName", text: "Customer Name" },
                            { name: "Chassis", text: "Vin No" },
                            { name: "PoliceRegNo", text: "Police No" },
                            { name: "DeliveryDate", text: "Delivery Date", type: "date" },
                        ]
                    });
                    break;
                case "pnlBirthDayCall":
                    widget.exportXls({
                        source: "cs.api/grid/CustomerBirthDays",
                        params: { OutStanding: "Y" },
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
                        source: "cs.api/grid/stnkext",
                        params: { OutStanding: "Y" },
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
                        source: "cs.api/grid/CsBpkbs",
                        params: { OutStanding: "Y" },
                        type: "kgridlnk",
                        fileName: "BpkbReminder",
                        items: [
                            { name: "CustomerCode", text: "Customer Code" },
                            { name: "CustomerName", text: "Customer Name" },
                            { name: "Chassis", text: "Vin No" },
                            { name: "PoliceRegNo", text: "Police No" },
                            { name: "BpkbDate", text: "BPKB Date", type: "date" },
                        ]
                    });
                    break;
                default:
                    break;
            }
        });
    }

    $("#btnRefresh2").on("click", refresh2);
    $("#btnExportXls").on("click", exportXls);
    $("#btnBack").on("click", back);

    $("#btn3DaysCall").on("click", load3DaysCall);
    $("#btnBDayCall").on("click", loadBirthDayCall);
    $("#btnStnkExt").on("click", loadStnkExt);
    $("#btnBpkbRemind").on("click", loadBpkb);
    $("#btnDeliveryOutstanding").on("click", loadDlvryOutstanding);
    //$("#btnHoliays").on("click", loadHoliday);
});