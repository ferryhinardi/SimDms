var widget;
$(document).ready(function () {
    var options = {
        title: "Review",
        xtype: "panels",
        panels: [
            {
                name: "pnlFilter",
                items: [
                    { name: "GroupArea", text: "Area", cls: "span6", type: "select", opt_text: "-- SELECT ALL --" },
                    { name: "CompanyCode", text: "Dealer", type: "select", cls: "span6", opt_text: "-- SELECT ALL --" },
                    { name: "BranchCode", text: "Outlet", type: "select", cls: "span6", opt_text: "-- SELECT ALL --" },
                    {
                        text: "Input Date",
                        type: "controls", items: [
                            { name: "InputFrom", text: "Date From", cls: "span2", type: "datepicker" },
                            { name: "InputTo", text: "Date To", cls: "span2", type: "datepicker" },
                        ],
                    },
                ],
            },
            {
                name: "reviewgrid",
                xtype: "k-grid",
            },
            {
                name: "pnlFilter2",
                cls: "hide",
                items: [
                    { name: 'Dealer', text: 'Dealer', type: 'text', cls: 'span6', readonly: true },
                    { name: 'Outlet', text: 'Outlet', type: 'text', cls: 'span6', readonly: true },
                    { name: 'DateFrom', text: 'Date From', type: 'text', cls: 'span3', readonly: true },
                    { name: 'DateTo', text: 'Date To', type: 'text', cls: 'span3', readonly: true },
                    { name: 'Check', text: 'Check', type: 'textarea', cls: 'span6', readonly: true },
                    { name: 'Action', text: 'Action', type: 'textarea', cls: 'span6', readonly: true },
                    { name: 'PIC', text: 'PIC', type: 'text', cls: 'span6', readonly: true },
                    { name: 'CommentbyGM', text: 'Comment by GM', type: 'textarea', cls: 'span6', readonly: true },
                    { name: 'CommentbySIS', text: 'Comment by SIS', type: 'textarea', cls: 'span6' },
                ],
            },
        ],
        toolbars: [
            { name: "btnRefresh", action: 'refresh', text: "Refresh", icon: "fa fa-refresh" },
            { name: "btnSave", action: 'save', text: "Save", icon: "fa fa-save", cls: 'hide' },
            { name: "btnCancel", action: 'cancel', text: "Cancel", icon: "fa fa-cancel", cls: 'hide' },
            { action: 'expand', text: 'Expand', icon: 'fa fa-expand' },
            { action: 'collapse', text: 'Collapse', icon: 'fa fa-compress', cls: 'hide' },
        ],
        onToolbarClick: function (action) {
            switch (action) {
                case 'refresh':
                    refreshGrid();
                    break;
                case 'save':
                    save();
                    $('#pnlFilter2, #btnSave, #btnCancel').hide();
                    $('#pnlFilter, #btnRefresh').fadeIn();
                    break;
                case 'cancel':
                    $('#pnlFilter2, #btnSave, #btnCancel').hide();
                    $('#pnlFilter, #btnRefresh').fadeIn();
                    break;
                case 'collapse':
                    widget.exitFullWindow();
                    widget.showToolbars(['refresh', 'export', 'expand']);
                    break;
                case 'expand':
                    widget.requestFullWindow();
                    widget.showToolbars(['refresh', 'export', 'collapse']);
                    break;
            }
        }
    };

    widget = new SimDms.Widget(options);
    widget.setSelect([
        { name: "GroupArea", url: "wh.api/combo/GroupAreas", optionalText: "-- SELECT ALL --" },
        { name: "PeriodYear", url: "wh.api/combo/years" }
    ]);
    widget.render(function () {
        $("[name=GroupArea]").on("change", function () {
            widget.select({ selector: "[name=CompanyCode]", url: "wh.api/combo/DealerList", params: { LinkedModule: "mp", GroupArea: $("[name=GroupArea]").val() }, optionalText: "-- SELECT ALL --" });
            $("[name=CompanyCode]").prop("selectedIndex", 0);
            $("[name=CompanyCode]").change();
        });
        $("[name=CompanyCode]").on("change", function () {
            widget.select({ selector: "[name=BranchCode]", url: "wh.api/combo/branchs", params: { comp: $("#pnlFilter [name=CompanyCode]").val() }, optionalText: "-- SELECT ALL --" });
            $("[name=BranchCode]").prop("selectedIndex", 0);
            $("[name=BranchCode]").change();
        });
        var date1 = new Date();
        var date2 = new Date(date1.getFullYear(), date1.getMonth(), 1);
        widget.populate({ InputFrom: date2, InputTo: date1 });
        refreshGrid();
    });

    function refreshGrid() {
        var params = {
            GroupArea: $("[name='GroupArea']").val(),
            CompanyCode: $("[name='CompanyCode']").val(),
            BranchCode: $("[name='BranchCode']").val(),
            DateFrom: getSqlDate($("[name=InputFrom]").val()),
            DateTo: getSqlDate($("[name=InputTo]").val())
        }
        widget.kgrid({
            url: "wh.api/inquiry/Review",
            name: "reviewgrid",
            params: params,
            columns: [
                { field: "DealerCode", title: "DealerCode", width: "150px", hidden: true },
                { field: "EmployeeID", title: "EmployeeID", width: "150px", hidden: true },
                { field: "DealerAbbreviation", title: "Dealer", width: "100px" },
                { field: "OutletAbbreviation", title: "Outlet", width: "100px" },
                { field: "DateFrom", title: "Date From", width: "150px", template: "#= (DateFrom == undefined) ? '' : moment(DateFrom).format('DD MMM YYYY') #" },
                { field: "DateTo", title: "Date To", width: "150px", template: "#= (DateTo == undefined) ? '' : moment(DateTo).format('DD MMM YYYY') #" },
                { field: "Plan", title: "Plan", width: "150px" },
                { field: "Do", title: "Do", width: "200px" },
                { field: "Check", title: "Check", width: "300px" },
                { field: "Action", title: "Action", width: "300px" },
                { field: "PIC", title: "PIC" },
                { field: "CommentbyGM", title: "Comment by GM", width: "300px" },
                { field: "CommentbySIS", title: "Comment by SIS", width: "300px" },
                { field: "BranchCode", title: "BranchCode", width: "150px", hidden: true }
            ],
            onDblClick: function () {
                var a = $(".kgrid #pnlResult .k-grid-content tr.k-state-selected td:eq(0)").text();
                //if (a.hasClass('k-minus')) { a.click(); }
                $('#pnlFilter2, #btnSave, #btnCancel').fadeIn();
                $('#pnlFilter, #btnRefresh').hide();

                $('#Dealer').val(getSelectedCell(2));
                $('#Outlet').val(getSelectedCell(3));
                $('#DateFrom').val(getSelectedCell(4));
                $('#DateTo').val(getSelectedCell(5));
                $('#Plan').val(getSelectedCell(6));
                $('#Do').val(getSelectedCell(7));
                $('#Check').val(getSelectedCell(8));
                $('#Action').val(getSelectedCell(9));
                $('#PIC').val(getSelectedCell(10));
                $('#CommentbyGM').val(getSelectedCell(11));
                $('#CommentbySIS').val(getSelectedCell(12));
            },
        });
    }

    function save() {
        var dataMain = {
            CompanyCode: getSelectedCell(0),
            BranchCode: getSelectedCell(13),
            EmployeeID: getSelectedCell(1),
            DateFrom: getSelectedCell(4),
            DateTo: getSelectedCell(5),
            Plan: getSelectedCell(6),
            CommentbySIS: $('#CommentbySIS').val()
        };
        console.log(dataMain)
        widget.post("wh.api/CsReviews/Save", dataMain, function (result) {
            if (result.status) {
                refreshGrid();
                widget.showNotification(result.message || SimDms.defaultInformationMessage);
            }
            else {
                widget.showNotification(result.message || SimDms.defaultErrorMessage);
            }
        });
    }

    function getSqlDate(value) {
        return moment(value, "DD-MMM-YYYY").format("YYYYMMDD");
    }

    function getSelectedCell(idx) {
        return $(".kgrid .k-grid-content tr.k-state-selected td:eq(" + idx + ")").text();
    }
});