$(document).ready(function () {
    var options = {
        title: "Inquiry - Customer Birthday",
        xtype: "panels",
        panels: [
            {
                name: "pnlFilter",
                items: [
                    { name: "MonthFrom", type: "select", text: "Month From", cls: "span3" },
                    { name: "MonthTo", type: "select", text: "Month To", cls: "span3" },
                    { name: "Year", type: "select", cls: "span3", text: "Outlet", text: "Year" },
                    {
                        name: "Status", type: "select", cls: "span3", text: "Status", items: [
                            //{ text: "All Status", value: "-" },
                            { text: "Outstanding", value: "Y" },
                            { text: "Inputted", value: "N" }
                        ]
                    },
                ],
            },
            {
                name: "CustBDay",
                xtype: "k-grid",
            },
        ],
        toolbars: [
            { name: "btnRefresh", text: "Refresh", icon: "icon-refresh" },
            { name: "btnExportXls", text: "Export (xls)", icon: "icon-xls" },
        ],
    }
    var widget = new SimDms.Widget(options);
    widget.render(function () {
        var currentMonth = (new Date().getMonth() + 1);
        widget.populate({ MonthFrom: currentMonth, MonthTo: currentMonth });
        setTimeout(refreshGrid, 1000);

        renderCallback();
    });

    function renderCallback() {
        widget.select({ name: "BranchCode", url: "cs.api/Combo/ListBranchCode" });
        widget.select({ name: "Year", url: "cs.api/Combo/ListOfYear" });
        widget.select({ name: "MonthFrom", url: "cs.api/Combo/MonthList" });
        widget.select({ name: "MonthTo", url: "cs.api/Combo/MonthList" });
        $("#btnExportXls").on("click", exportXls);
        $(document).on("click", "#btnRefresh", refreshGrid);        
         $("[name='BranchCode'], [name='Year'], [name='Month'], [name='Status']").on("change", function () {
            refreshGrid();
        });
    }

    function refreshGrid() {
        var year = $("[name='Year']").val();
        var monthFrom = $("[name='MonthFrom']").val();
        var monthTo = $("[name='MonthTo']").val();
        var branchCode = $("[name='BranchCode']").val();
        var status = $("[name='Status']").val();

        var params = {
            Year: year,
            MonthFrom: monthFrom,
            MonthTo: monthTo,
            BranchCode: branchCode,
            Status: status
        };

        widget.kgrid({
            url: "cs.api/inquiry/CsCustomerBirthday",
            name: "CustBDay",
            params: params,
            columns: [
                { field: "InputDate", title: "Input Date", width: 150, template: "#= (InputDate == undefined) ? '' : moment(InputDate).format('DD MMM YYYY') #" },
                { field: "CustomerCode", title: "ID Customer", width: 100 },
                { field: "CustomerName", title: "Customer Name", width: 280 },
                { field: "CustomerBirthDate", filterable: false, title: "Birth Date", width: 120, template: "#= (CustomerBirthDate == undefined) ? '' : moment(CustomerBirthDate).format('DD MMM YYYY') #" },
                { field: "CustomerAddress", title: "Address", width: 700 },
                { field: "CustomerHandphone", title: "Handphone", width: 120 },
                { field: "CustomerTelephone", title: "Telephone", width: 120 },
                { field: "CustomerAddPhone1", title: "Additional Phone 1", width: 150 },
                { field: "CustomerAddPhone2", title: "Additional Phone 2", width: 150 },
                { field: "GreetingBy", title: "Greeting By", width: 150 },
                //{ field: "CustomerGiftSentDate", filterable: false, title: "Souvenir Sent Date", width: 170, template: "#= (CustomerGiftSentDate == undefined) ? '' : moment(CustomerGiftSentDate).format('DD MMM YYYY') #" },
                //{ field: "CustomerGiftReceivedDate", filterable: false, title: "Souvenir Received Date", width: 170, template: "#= (CustomerGiftReceivedDate == undefined) ? '' : moment(CustomerGiftReceivedDate).format('DD MMM YYYY') #" },
                //{ field: "NumberOfSpouse", filterable: false, title: "Spouse", width: 150 },
                //{ field: "NumberOfChildren", filterable: false, title: "Children", width: 150 },
            ],
        });
    }

    function exportXls() {
        widget.exportXls({
            name: "CustBDay",
            type: "kgrid",
            items: [
                { name: "InputDate", type: "date", filterable: false, text: "Input Date", width: 150, template: "#= (InputDate == undefined) ? '' : moment(CustomerGiftSentDate).format('DD MMM YYYY') #" },
                { name: "CustomerCode", type: "text", text: "Customer Code", width: 150 },
                { name: "CustomerName", type: "text", filterable: false, text: "Customer Name", width: 200 },
                { name: "CustomerBirthDate", type: 'date', text: "Birth Date", width: 150, template: "#= (CustomerBirthDate == undefined) ? '' : moment(CustomerBirthDate).format('DD MMM YYYY') #" },
                { name: "CustomerAddress", text: "Address", width: 600 },
                { name: "CustomerHandphone", filterable: false, text: "Handphone", width: 170 },
                { name: "CustomerTelephone", filterable: false, text: "Telephone", width: 170 },
                { name: "CustomerAddPhone1", filterable: false, text: "Additional Phone 1", width: 170 },
                { name: "CustomerAddPhone2", filterable: false, text: "Additional Phone 2", width: 150 },
            ]
        });
    }

    function getSqlDate(value) {
        return moment(value, "DD-MMM-YYYY").format("YYYYMMDD");
    }
});
