var widget = new SimDms.Widget({
    title: 'Live Stock Portal (2W)',
    xtype: 'panels',
    toolbars: [
        { text: 'Refresh', action: 'refresh', icon: 'fa fa-refresh' },
        { text: 'Expand', action: 'expand', icon: 'fa fa-expand' },
        { text: 'Collapse', action: 'collapse', icon: 'fa fa-compress', cls: 'hide' },
        { text: 'Export to Excel', action: 'exportToExcel', icon: 'fa fa-file-excel-o', cls: '' },
    ],
    panels: [
        {
            name: "pnlFilter",
            items: [
                {
                    text: "AREA",
                    type: "controls",
                    items: [
                        { name: 'checkArea', text: '', type: 'check', cls: 'span1' },
                        { name: 'Area', text: 'Area', type: 'select', cls: 'span2' },
                    ]
                },
                {
                    text: "PART", type: "controls", items: [
                        { name: 'checkArea2', text: '', type: 'check', cls: 'span1' },
                        { name: "PartNo", text: "Part No", type: "popup", cls: "span2", additional:"disabled", maxlength: 20, required:"width:"},
                        { name: "PartName", text: "Part Name", cls: "span3" }
                    ]
                }
                ,
                {
                    text: "SUBSTITUSI",
                    type: "controls",
                    items: [
                        { name: 'hidden', text: '', type: 'check', cls: 'span1', style: 'visibility:hidden;' },
                        { name: 'Substitusi', text: 'Substitusi', type: 'text', cls: 'span2', readonly: 'readonly' },
                        { name: 'SubsName', text: 'Substitusi Name', type: 'text', cls: 'span3', readonly: 'readonly' },
                    ]
                }
            ]
        },
        {
            name: "pnlResult",
            xtype: "k-grid",
        },
    ],
    onToolbarClick: function (action) {
        switch (action) {
            case 'refresh':
                refreshGrid();
                break;
            case 'collapse':
                widget.exitFullWindow();
                widget.showToolbars(['refresh', 'expand', 'exportToExcel']);
                break;
            case 'expand':
                widget.requestFullWindow();
                widget.showToolbars(['refresh', 'collapse', 'exportToExcel']);
                break;
            case 'exportToExcel':
                exportToExcel();
                break;
            default:
                break;
        }
    },
});

function selectedParts(row) {
    widget.populate({ PartNo: row.PartNo, PartName: row.PartName });

    widget.post("wh.api/combo/LiveStockSubs2", { "PartNo": row.PartNo }, function (result) {
        var list = [];
        list = Enumerable.From(result[0]).ToArray();
        widget.setValue({ name: "Substitusi", value: (list.length <=0 ? '' : list[0].NewPartNo), isControl: false });
        widget.setValue({ name: "SubsName", value: (list.length <= 0 ? '' : list[0].PartName), isControl: false });
    });

    //setTimeout(refreshGrid, 500);
}

widget.render(function () {
    widget.post("wh.api/combo/LiveStockArea2", function (result) {
        widget.bind({
            name: 'Area',
            text: '-- ALL AREAS --',
            data: result[0],
            //onChange: function () { setTimeout(refreshGrid, 500) }
        });

        //setTimeout(refreshGrid, 500);
    });

    $("[name=PartNo]").on("change", function () {
        var partNo = $(this).val();
        $.ajax({
            url: 'wh.api/lookupgrid/LookUpPartNo2',
            data: { PartNo: partNo },
            type: 'POST',
            success: function (data) {
                data = data['data'][0];
                $("#PartNo").val(data.PartNo);
                $("#PartName").val(data.PartName);
                selectedParts(data);
            }
        });
    });

    $('#btnPartNo').on('click', function () {
        sdms.lookup({
            title: 'Daftar Part',
            url: 'wh.api/lookupgrid/LookUpPartNo2',
            sort: [{ field: 'PartNo', dir: 'asc' }, { field: 'PartName', dir: 'asc' }],
            fields: [
                { name: 'PartNo', text: 'Part No', width: 200 },
                { name: 'PartName', text: 'Part Name', width: 200 }
            ],
            dblclick: function (row) {
                selectedParts(row);
            },
            onclick: function (row) {
                selectedParts(row);
            }
        });
    });

    widget.enable({ value: false, items: ["Area", "btnPartNo", "PartName", "PartNo"] });
    widget.setValue({ name: "PartNo", value: "-- ALL PARTS --" });

    widget.checked("checkArea", function (result) {
        if (result) {
            widget.enable({ value: true, items: ["Area"] });
        }
        else {
            widget.enable({ value: false, items: ["Area"] });
            widget.setValue({ name: "Area", value: "0" });

            //setTimeout(refreshGrid, 500);
        }
    });

    widget.checked("checkArea2", function (result) {
        if (result) {
            widget.enable({ value: true, items: ["btnPartNo", "PartName", "PartNo"] });
            widget.setValue({ name: "PartNo", value: "" });
        }
        else {
            widget.enable({ value: false, items: ["btnPartNo", "PartName", "PartNo"] });
            widget.setValue({ name: "PartNo", value: "-- ALL PARTS --" });
            widget.setValue({ name: "PartName", value: "", isControl: false });

            //setTimeout(refreshGrid, 500);
        }
    });
});

function refreshGrid() {
    var filter = widget.serializeObject('pnlFilter');
    widget.kgrid({
        url: "wh.api/inquiry/LiveStockPart2",
        name: "pnlResult",
        params: filter,
        selectable: "row",
        serverBinding: true,
        sort: [
            { field: "No", dir: "asc" },
        ],
        columns: [
            { field: "No", width: 150, title: "No" },
            { field: "PartNo", width: 400, title: "Part No" },
            { field: "PartName", width: 400, title: "Part Name" },
        ],
        onDblClick: function () {
            var param = $(".kgrid #pnlResult .k-grid-content tr.k-state-selected td:first-child + td").text();
            var area = $("#Area option:selected").val();
            widget.kgrid({
                url: "wh.api/inquiry/LiveStockPartDetail2",
                name: "pnlResult",
                params: { "PartNo": param, "Area": area },
                selectable: "row",
                serverBinding: true,
                sort: [
                    { field: "Area", dir: "asc" },
                    { field: "Dealer", dir: "asc" },
                    { field: "Outlet", dir: "asc" },
                    { field: "QtyAvail", dir: "asc" },
                ],
                columns: [
                    { field: "Area", width: 300, title: "AREA" },
                    { field: "Dealer", width: 400, title: "DEALER" },
                    { field: "Outlet", width: 300, title: "OUTLET" },
                    { field: "QtyAvail", width: 200, title: "QTY-AVAIL" },
                ],
            });
        },
    });
}

function exportToExcel() {
    var url = "wh.api/InquiryProd/LiveStockPartProd2?";
    var filter = widget.serializeObject('pnlFilter');
    var params = ''

    $.each(filter || [], function (key, val) {
        params += key + '=' + val + '&';
    });
    params = params.substring(0, params.length - 1);

    url += params;
    window.location = url;
}