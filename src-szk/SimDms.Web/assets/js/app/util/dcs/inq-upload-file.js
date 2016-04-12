$(document).ready(function () {
    var options = {
        xtype: "panels",
        title: "Inquiry Upload File",
        toolbars: [
            { name: "btnRefresh", text: "Refresh", cls: "button small", icon: "fa fa-refresh" }
        ],
        panels: [
            {
                name: "Filter",
                items: [
                    { name: "GroupArea", type: "select", cls: "span4", text: "Area" },
                    { name: "CustomerCode", type: "select", cls: "span4", text: "Dealer", opt_text: "-- SELECT ALL -- " },
                    { name: "DataID", type: "select", cls: "span4", text: "Data ID" },
                    { name: "ProductType", type: "select", cls: "span4", text: "Product Type" },
                    { name: "Status", type: "select", cls: "span4", text: "Status" }
                ]
            },
            {
                name: "gridData",
                xtype: "k-grid"
            }
        ]
    };
    var widget = new SimDms.Widget(options);

    var setSelectOptions = [
        {
            name: "GroupArea",
            url: "wh.api/Combo/GroupAreas",
            optionalText: "-- SELECT ALL --"
        },
        {
            name: "CustomerCode",
            url: "wh.api/Combo/Companies",
            optionalText: "-- SELECT ALL --",
            cascade: {
                name: "GroupArea"
            }
        },
        {
            name: "DataID",
            url: "util.api/Combo/DmsUploadFileDataID",
            optionalText: "-- SELECT ALL --",
        },
        {
            name: "Status",
            url: "util.api/Combo/DmsUploadFileStatus",
            optionalText: "-- SELECT ALL --",
        },
        {
            name: "ProductType",
            url: "util.api/Combo/DmsDownloadFileProductType",
            optionalText: "-- SELECT ALL --",
        }
    ];
    widget.setSelect(setSelectOptions);

    widget.render(renderCallback);

    function renderCallback() {
        refreshGrid();
        initElementEvents();
    }

    function initElementEvents() {
        var btnRefresh = $("#btnRefresh");

        btnRefresh.off();
        btnRefresh.on("click", function () {
            refreshGrid();
        });
    }

    function refreshGrid() {
        var params = {
            CustomerCode: $("[name='CustomerCode']").val(),
            DataID: $("[name='DataID']").val(),
            ProductType: $("[name='ProductType']").val(),
            Status: $("[name='Status']").val(),
        }
        widget.kgrid({
            url: "util.api/Grid/DmsUploadFile",
            name: "gridData",
            params: params,
            serverBinding: true,
            pageSize: 10,
            selectable: true,
            multiselect: false,
            columns: [
                { field: "ID", title: "ID", width: 100 },
                { field: "DataID", title: "Data ID", width: 100 },
                { field: "CustomerCodeBilling", title: "Customer Code", width: 120 },
                { field: "DealerName", title: "Customer Name", width: 250 },
                { field: "CreatedDate", title: "Created Date", width: 120, template: "#= (CreatedDate == undefined) ? '-':moment(CreatedDate).format('DD MMM YYYY') #" },
                { field: "Status", title: "Status", width: 120 },
                { field: "ProductType", title: "Product Type", width: 120 },
            ],
            detailInit: detailInit
        }, function () {
            var btnViewFileContents = $("#btnViewFileContents");
            btnViewFileContents.off();
            btnViewFileContents.on("click", function () {
                widget.selectedRow("gridData", function (data) {
                    widget.showCustomPopup(data.ClobContent, {
                        status: true,
                        height: 120,
                        width: 120
                    }, function () {
                        console.log("rendering popup ...");
                    });
                });
            });
        });
    }

    function detailInit(e) {
        var data = e.data;
        var clobContent = data.ClobContent || "An Empty File.";
        $("<div/>").appendTo(e.detailCell).kendoGrid({
            dataSource: { data: [{ ClobContent: clobContent }] },
            columns: [{ field: "ClobContent", title: "File Content" }]
        });
    }
});