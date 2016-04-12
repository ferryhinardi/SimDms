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
                            { name: "DateFrom", text: "Date From", cls: "span2", type: "datepicker" },
                            { name: "DateTo", text: "Date To", cls: "span2", type: "datepicker" },
                        ],
                    },
                ],
            },
            {
                name: "reviewgrid",
                xtype: "k-grid",
            },
        ],
        toolbars: [
            { name: "btnRefresh", action: 'refresh', text: "Refresh", icon: "fa fa-refresh" },
            { action: 'expand', text: 'Expand', icon: 'fa fa-expand' },
            { action: 'collapse', text: 'Collapse', icon: 'fa fa-compress', cls: 'hide' },
        ],
        onToolbarClick: function (action) {
            switch (action) {
                case 'refresh':
                    refreshGrid();
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
            //widget.select({ selector: "[name=CompanyCode]", url: "wh.api/combo/DealerList", params: { LinkedModule: "mp", GroupArea: $("[name=GroupArea]").val() }, optionalText: "-- SELECT ALL --" });
            widget.select({ selector: "[name=CompanyCode]", url: "wh.api/combo/ComboDealerList", params: { GroupArea: $("#pnlFilter [name=GroupArea]").val() }, optionalText: "-- SELECT ALL --" });
            $("[name=CompanyCode]").prop("selectedIndex", 0);
            $("[name=CompanyCode]").change();
        });
        $("[name=CompanyCode]").on("change", function () {
            //widget.select({ selector: "[name=BranchCode]", url: "wh.api/combo/branchs", params: { comp: $("#pnlFilter [name=CompanyCode]").val() }, optionalText: "-- SELECT ALL --" });
            widget.select({ selector: "[name=BranchCode]", url: "wh.api/combo/ComboOutletList", params: { companyCode: $("#pnlFilter [name=CompanyCode]").val() }, optionalText: "-- SELECT ALL --" });
            $("[name=BranchCode]").prop("selectedIndex", 0);
            $("[name=BranchCode]").change();
        });
        var date1 = new Date();
        var date2 = new Date(date1.getFullYear(), date1.getMonth(), 1);
        widget.populate({ DateFrom: date2, DateTo: date1 });
        //refreshGrid();

        $('#CompanyCode').attr('disabled', 'disabled');
        $('#BranchCode').attr('disabled', 'disabled');
    });

    $('#GroupArea').on('change', function () {
        if ($('#GroupArea').val() != "") {
            $('#CompanyCode').removeAttr('disabled');
        } else {
            $('#CompanyCode').attr('disabled', 'disabled');
            $('#BranchCode').attr('disabled', 'disabled');
            $('#CompanyCode').select2('val', "");
            $('#BranchCode').select2('val', "");
        }
        $('#CompanyCode').select2('val', "");
        $('#BranchCode').select2('val', "");
    });

    $('#CompanyCode').on('change', function () {
        if ($('#CompanyCode').val() != "") {
            $('#BranchCode').removeAttr('disabled');
        } else {
            $('#BranchCode').attr('disabled', 'disabled');
        }
        $('#BranchCode').select2('val', "");
    });

    function refreshGrid() {
        var params = {
            GroupArea: $("[name='GroupArea']").val(),
            CompanyCode: $("[name='CompanyCode']").val(),
            BranchCode: $("[name='BranchCode']").val(),
            DateFrom: getSqlDate($("[name=DateFrom]").val()),
            DateTo: getSqlDate($("[name=DateTo]").val())
        }
        widget.kgrid({
            url: "wh.api/inquiry/Review",
            name: "reviewgrid",
            params: params,
            columns: [
                { field: "DateFrom", title: "Date From", width: "150px", template: "#= (DateFrom == undefined) ? '' : moment(DateFrom).format('DD MMM YYYY') #" },
                { field: "DateTo", title: "Date To", width: "150px", template: "#= (DateTo == undefined) ? '' : moment(DateTo).format('DD MMM YYYY') #" },
                { field: "Plan", title: "Plan", width: "150px" },
                { field: "Do", title: "Do", width: "200px" },
                { field: "Check", title: "Check", width: "300px" },
                { field: "Action", title: "Action", width: "300px" },
                { field: "PIC", title: "PIC" },
                { field: "CommentbyGM", title: "Comment by GM", width: "300px" },
                { field: "CommentbySIS", title: "Comment by SIS", width: "300px" }
            ],
        });
    }

    function getSqlDate(value) {
        return moment(value, "DD-MMM-YYYY").format("YYYYMMDD");
    }
});