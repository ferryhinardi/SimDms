var widget = new SimDms.Widget({
    title: 'Live Stock Portal : Buy / Sell Mode (2W)',
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
                        { name: 'Area', text: 'Area', type: 'select', cls: 'span3', opt_text: "-- SELECT ALL --" },
                        { text: "MODEL", type: "label", cls: "span1" },
                        { name: "Model", text: "Model", cls: "span3", maxlength: 2000 }
                    ]
                },
                {
                    text: "PART", type: "controls", items: [
                        { name: 'checkArea2', text: '', type: 'check', cls: 'span1' },
                        { name: "PartNo", text: "Part No", type: "popup", cls: "span3", additional: "disabled", maxlength: 20, required: "width:" },
                        { name: "PartName", text: "Part Name", cls: "span4" }
                    ]
                }
                ,
                {
                    text: "SUBSTITUSI",
                    type: "controls",
                    items: [
                        { name: 'hidden', text: '', type: 'check', cls: 'span1', style: 'visibility:hidden;' },
                        { name: 'Substitusi', text: 'Substitusi', type: 'text', cls: 'span3', readonly: 'readonly' },
                        { name: 'SubsName', text: 'Substitusi Name', type: 'text', cls: 'span4', readonly: 'readonly' },
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
        widget.setValue({ name: "Substitusi", value: (list.length <= 0 ? '' : list[0].NewPartNo), isControl: false });
        widget.setValue({ name: "SubsName", value: (list.length <= 0 ? '' : list[0].PartName), isControl: false });
    });
}
widget.setSelect([{ name: "Area", url: "wh.api/combo/GroupAreas2W", optionalText: "-- SELECT ALL --" }]);
widget.render(function () {
    $("[name=Qty]").on("change", function () {
        if (!$.isNumeric($(this).val()))
            $("div.errorQty label").text("This Field Must Be Numeric.").css("font-style", "italic");
        else
            $("div.errorQty label").text("");
    }).attr("placeholder", "Qty");

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

    $("[name=PartNo]").on("change", function () {
        var partNo = $(this).val();

        $.ajax({
            url: 'wh.api/lookupgrid/LoopUpModel2',
            data: { PartNo: partNo },
            type: 'POST',
            success: function (data) {
                var model = "";
                for (var i = 0; i < data.length; i++)
                    model += (i != 0 ? ", " : "") + data[i].Model;
                $("#Model").val(model);
            }
        });

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

    widget.enable({ value: false, items: ["Area", "btnPartNo", "PartName", "PartNo", "Model"] });
    widget.setValue({ name: "PartNo", value: "-- ALL PARTS --" });

    widget.checked("checkArea", function (result) {
        if (result) {
            widget.enable({ value: true, items: ["Area"] });
        }
        else {
            widget.enable({ value: false, items: ["Area"] });
            $("[name=Area]").prop('selectedIndex', 0);
            $("#select2-chosen-1").text($("[name=Area] option:first").text());
        }
    });

    widget.checked("checkArea2", function (result) {
        if (result) {
            widget.enable({ value: true, items: ["btnPartNo", "PartName", "PartNo", "Model"] });
            widget.setValue({ name: "PartNo", value: "" });
        }
        else {
            widget.enable({ value: false, items: ["btnPartNo", "PartName", "PartNo", "Model"] });
            widget.setValue({ name: "PartNo", value: "-- ALL PARTS --" });
            widget.setValue({ name: "PartName", value: "", isControl: false });
        }
    });

    $("#pnlFilter").css("max-width", "900px");
    $("div.gl-widget form").append("<div id='divContactPerson'>" +
                                "<h4>CONTACT PERSON</h4>" +
                                "<div>" +
                                    "<div id='dealerName'><span style='font-weight:bold; text-decoration:underline;'></span></div>" +
                                    "<div style='display: inline-flex;'>" +
                                        "<div id='phone' style='width:170px;'>Phone : <span></span></div>" +
                                        "<div>Contact Person</div>" +
                                    "</div>" +
                                    "<div></div>" +
                                    "<div style='display: inline-flex;'>" +
                                        "<div id='fax' style='width:170px;'>Fax : <span></span></div>" +
                                        "<div id='cp' style='max-width: 200px;word-wrap: break-word;'>" +
                                            "<span style='font-weight:bold; text-decoration:underline;'></span>" +
                                        "</div>" +
                                    "</div>" +
                                    "<div id='email'>Email : <span></span></div>" +
                                "</div>" +
                              "<div>");
    $("#divContactPerson").css({
        "margin-top": "-70px",
        "margin-left": $("#pnlFilter").width()
    });
    $("div#divContactPerson div:first").css({
        "border": "solid",
        "padding": "10px"
    });
    $("#divContactPerson").hide();
});

function refreshGrid() {
    $("#divContactPerson").hide();
    var filter = widget.serializeObject('pnlFilter');
    widget.kgrid({
        url: "wh.api/inquiry/LiveStockPartDetailTrans2",
        name: "pnlResult",
        params: filter,
        selectable: "row",
        serverBinding: true,
        sort: [
            { field: "Area", dir: "asc" },
            { field: "Dealer", dir: "asc" },
            { field: "Outlet", dir: "asc" },
            { field: "Average", dir: "asc" },
            { field: "MovingCode", dir: "asc" },
        ],
        columns: [
            { field: "Area", width: 200, title: "AREA" },
            { field: "Dealer", title: "DEALER" },
            { field: "Outlet", title: "OUTLET" },
            { field: "Average", width: 100, title: "D.AVG/DAY" },
            { field: "MovingCode", width: 100, title: "MC" },
            { field: "DealerName", hidden: true, attributes: { "class": "DealerName" } },
            { field: "ContactPersonName", hidden: true, attributes: { "class": "CPName" } },
            { field: "FaxNo", hidden: true, attributes: { "class": "FaxNo" } },
            { field: "PhoneNo", hidden: true, attributes: { "class": "PhoneNo" } },
            { field: "HandPhoneNo", hidden: true, attributes: { "class": "HPNo" } },
            { field: "EmailAddr", hidden: true, attributes: { "class": "Email" } },
        ],
        onDblClick: function (event) {
            var dealerName = $(".k-state-selected").find(".DealerName").text();
            var CP = $(".k-state-selected").find(".CPName").text();
            var Fax = $(".k-state-selected").find(".FaxNo").text();
            var Phone = $(".k-state-selected").find(".PhoneNo").text();
            var HP = $(".k-state-selected").find(".HandPhoneNo").text();
            var Email = $(".k-state-selected").find(".EmailAddr").text();
            $("#dealerName span").text(dealerName);
            $("#phone span").text(Phone);
            $("#fax span").text(Fax);
            $("#email span").text(Email);
            $("#cp span").text(CP);
            $("#divContactPerson").show();
        },
    });
}

function exportToExcel() {
    var url = "wh.api/InquiryProd/LiveStockPartTransProd2?";
    var filter = widget.serializeObject('pnlFilter');
    var params = ''

    $.each(filter || [], function (key, val) {
        params += key + '=' + val + '&';
    });
    params = params.substring(0, params.length - 1);

    url += params;
    window.location = url;
}