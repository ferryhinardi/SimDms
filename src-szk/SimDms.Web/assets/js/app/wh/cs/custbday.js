$(document).ready(function () {
    var options = {
        title: "Inquiry - Customer Birthday",
        xtype: "panels",
        panels: [
            {
                name: "pnlFilter",
                items: [
                    { name: "GroupArea", text: "Area", cls: "span6", type: "select", opt_text: "-- SELECT ALL --" },
                    { name: "CompanyCode", text: "Dealer", type: "select", cls: "span6", opt_text: "-- SELECT ALL --" },
                    { name: "BranchCode", text: "Outlet", type: "select", cls: "span6", opt_text: "-- SELECT ALL --" },
                    {
                        name: "ParStatus", text: "Status", type: "select", cls: "span3", items: [
                            { value: "0", text: "All Status" },
                            { value: "1", text: "Not Inputted" },
                            { value: "2", text: "Inputted" },
                        ],
                        fullItem: true
                    },
                    { name: "PeriodYear", text: "Year", type: "select", cls: "span3" },
                    { name: "ParMonth1", text: "Month From", type: "select", cls: "span3" },
                    { name: "ParMonth2", text: "Month To", type: "select", cls: "span3" },
                ],
            },
            {
                name: "CustBDay",
                xtype: "k-grid",
            },
        ],
        toolbars: [
            { name: "btnRefresh", action: 'refresh', text: "Refresh", icon: "fa fa-refresh" },
            { action: 'expand', text: 'Expand', icon: 'fa fa-expand' },
            { action: 'collapse', text: 'Collapse', icon: 'fa fa-compress', cls: 'hide' },
            { name: "btnExportXls", action: 'export', text: "Export (Xls)", icon: "fa fa-file-excel-o" },
        ],
        onToolbarClick: function (action) {
            switch (action) {
                case 'collapse':
                    widget.exitFullWindow();
                    widget.showToolbars(['refresh', 'export', 'expand']);
                    break;
                case 'expand':
                    widget.requestFullWindow();
                    widget.showToolbars(['refresh', 'export', 'collapse']);
                    break;
                default:
                    break;
            }
        },
    }
    var widget = new SimDms.Widget(options);
    widget.setSelect([{ name: "GroupArea", url: "wh.api/combo/GroupAreas", optionalText: "-- SELECT ALL --" }]);
    widget.render(function () {
        var initial = {
            ParMonth1: new Date().getMonth(),
            ParMonth2: new Date().getMonth()
        }
        widget.populate({ PeriodYear: new Date().getFullYear(), ParMonth: new Date().getMonth() + 1, ParStatus: 2 });
        $("[name=GroupArea]").on("change", function () {
            widget.select({
                selector: "[name=CompanyCode]",
                url: "wh.api/combo/DealerList",
                params: { LinkedModule: "mp", GroupArea: $("[name=GroupArea]").val() },
                optionalText: "-- SELECT ALL --"
            });
            $("[name=CompanyCode]").prop("selectedIndex", 0);
            $("[name=CompanyCode]").change();
        });
        widget.select({ selector: "[name=PeriodYear]", url: "wh.api/combo/years" });
        widget.select({ selector: "[name=ParMonth1]", url: "wh.api/combo/listofmonth" });
        widget.select({ selector: "[name=ParMonth2]", url: "wh.api/combo/listofmonth" });
        widget.selectparam({
            name: "BranchCode", url: "wh.api/combo/branchs",
            params: [{ name: "CompanyCode", param: "comp" }],
            optionalText: "-- SELECT ALL --"
        });
        widget.populate(initial);
    });

    $("[name=BranchCode],[name=PeriodYear],[name=ParMonth1],[name=ParMonth2],[name=ParStatus]").on("change", refreshGrid);
    $("#btnRefresh").on("click", refreshGrid);
    $("#btnExportXls").on("click", exportXls);

    function refreshGrid() {
        var params = $("#pnlFilter").serializeObject();

        if (widget.isNullOrEmpty(params.ParMonth1) == false &&
            widget.isNullOrEmpty(params.ParMonth2) == false &&
            widget.isNullOrEmpty(params.PeriodYear) == false &&
            //widget.isNullOrEmpty(params.BranchCode) == false &&
            widget.isNullOrEmpty(params.CompanyCode) == false) {

            widget.kgrid({
                url: "wh.api/inquiry/CsBirthday",
                name: "CustBDay",
                pageSize: 10,
                params: params,
                columns: [
                    { field: "BranchCode", title: "Branch Code", width: 100 },
                    { field: "CustomerCode", title: "Cust Code", width: 100 },
                    { field: "CustomerName", title: "Customer Name", width: 280 },
                    { field: "BirthDate", filterable: false, title: "Birth Date", width: 120, template: "#= (BirthDate == undefined) ? '' : moment(BirthDate).format('YYYY-MM-DD') #" },
                    { field: "Address", title: "Address", width: 700 },
                    { field: "PhoneNo", title: "Phone No", width: 120 },
                    { field: "HPNo", title: "HP No", width: 120 },
                    { field: "AddPhone1", title: "Add Phone 1", width: 120 },
                    { field: "AddPhone2", title: "Add Phone 2", width: 120 },
                    { field: "IsReminder", title: "Inputted", width: 120 },
                    { field: "InputDate", filterable: false, title: "Inputted Date", width: 180, template: "#= (InputDate == undefined) ? '' : moment(InputDate).format('YYYY-MM-DD  HH:mm:ss') #" },
                ],
            });
        }
    }

    function getSqlDate(value) {
        return moment(value, "DD-MMM-YYYY").format("YYYYMMDD");
    }

    function exportXls() {
        widget.exportXls({
            name: "CustBDay",
            type: "kgrid",
            fileName: "CustBDay",
            items: [
                { name: "BranchCode", text: "Branch Code", width: 100 },
                { name: "CustomerCode", text: "Cust Code", width: 100 },
                { name: "CustomerName", text: "Customer Name", width: 280 },
                { name: "BirthDate", text: "Birth Date", type: "date" },
                { name: "Address", text: "Address", width: 700 },
                { name: "PhoneNo", text: "Phone No", width: 120, type: "text" },
                { name: "HPNo", text: "HP No", width: 120, type: "text" },
                { name: "AddPhone1", text: "Add Phone 1", width: 120, type: "text" },
                { name: "AddPhone1", text: "Add Phone 2", width: 120, type: "text" },
                { name: "IsReminder", text: "Inputted", width: 110 },
                { name: "InputDate", text: "Inputted Date", width: 110, type: "date" },
            ]
        });
    }
    /*
    function exportMonitoring() {
        var params = $("#pnlFilter").serializeObject();

        $.ajax({
            async: true,
            type: "POST",
            data: params,
            url: "wh.api/report/CsBirthdayMonitorReport",
            success: function (data) {
                if (data.message == "") {
                    location.href = 'wh.api/report/DownloadExcelFile?key=' + data.value + '&filename=Customer Birthday Monitoring';
                } else {
                    sdms.info(data.message, "Error");
                }
            }
        });
    }
    */
});
