$(document).ready(function () {
    var options = {
        xtype: "panels",
        title: "Review SFM",
        toolbars: [
            { name: "btnRefresh", text: "Refresh", icon: "fa fa-refresh" },
            { name: "btnExportXls", text: "Export (xls)", icon: "fa fa-file-excel-o" },
        ],
        panels: [
            {
                name: "pnlFilter",
                //title: "Filter",
                items: [
                    { name: "GroupArea", type: "select", cls: "span4", text: "Area", readonly: false, opt_text: "-- SELECT ALL --" },
                    { name: "CompanyCode", type: "select", cls: "span6", text: "Dealer Name", readonly: false, opt_text: "-- SELECT ALL --" },
                    { name: "BranchCode", type: "select", cls: "span6", text: "Branch Name", readonly: false, opt_text: "-- SELECT ALL --" },
                    { name: "ByDate", type: "datepicker", cls: "span3", text: "By Date", readonly: false },
                ]
            },
            {
                name: "gridSfmReview",
                xtype: "k-grid"
            }
        ]
    };

    var widget = new SimDms.Widget(options);
    widget.setSelect([{ name: "GroupArea", url: "wh.api/combo/GroupAreas", optionalText: "-- SELECT ALL --" }]);
    widget.default = { CompanyCode: "", ByDate: new Date() };

    widget.render(function () {
        widget.populate(widget.default);
        $("#CompanyCode").prop('disabled', true);
        $("[name=GroupArea]").on("change", function () {
            var groupArea = $("[name=GroupArea]").val();
            if (groupArea == '' || groupArea == undefined) {
                $("#CompanyCode").prop('disabled', true);
            }
            else {
                $("#CompanyCode").prop('disabled', false);
            }

            widget.select({ selector: "[name=CompanyCode]", url: "wh.api/combo/ComboDealerList", params: { LinkedModule: "mp", GroupArea: groupArea }, optionalText: "-- SELECT ALL --" });
            $("[name=CompanyCode]").prop("selectedIndex", 0);
            $("[name=CompanyCode]").change();
        });
        $("[name=CompanyCode]").on("change", function () {
            widget.select({ selector: "[name=BranchCode]", url: "wh.api/combo/ComboOutletList", params: { CompanyCode: $("#pnlFilter [name=CompanyCode]").val() }, optionalText: "-- SELECT ALL --" });
            $("[name=BranchCode]").prop("selectedIndex", 0);
            $("[name=BranchCode]").change();
        });

        var buttonRefresh = $("#btnRefresh");
        var buttonExportXls = $("#btnExportXls");

        buttonRefresh.off();
        buttonExportXls.off();

        buttonRefresh.on("click", function (evt) {
            reloadData();
        });
        buttonExportXls.on("click", function (evt) {
            exportXls();
        });
    });

    function reloadData() {
        var params = {
            GroupArea: $("[name='GroupArea']").val(),
            CompanyCode: $("[name='CompanyCode']").val(),
            BranchCode: $("[name='BranchCode']").val(),
            ByDate: $("[name='ByDate']").val(),
        };
        var url = "wh.api/Inquiry/SfmReviewNew";

        widget.kgrid({
            url: url,
            name: "gridSfmReview",
            serverBinding: true,
            pageSize: 10,
            //pageable: false,
            params: params,
            columns: [
                //{ field: "CompanyName", title: "Dealer Name", width: 400 },
                { field: "BranchName", title: "Outlet Name", width: 400 },
                { field: "TotalSalesman", title: "Total Wiraniaga", width: 150 },
                { field: "Trainee", title: "Trainee", width: 80 },
                { field: "Silver", title: "Silver", width: 80 },
                { field: "Gold", title: "Gold", width: 80 },
                { field: "Platinum", title: "Platinum", width: 80 },
                { field: "SC", title: "SC", width: 80 },
                { field: "SH", title: "SH", width: 80 },
                { field: "BM", title: "BM", width: 80 },
                { field: "TotalSCSHBM", title: "Total SC SH BM", width: 250 },
            ],
        });
    }

    function exportXls() {
        var params = {
            GroupArea: $("[name='GroupArea']").val(),
            CompanyCode: $("[name='CompanyCode']").val(),
            BranchCode: $("[name='BranchCode']").val(),
            ByDate: $("[name='ByDate']").val(),
        };
        var url = "wh.api/Report/SfmReviewNew";

        $('#btnExportXls').attr('disabled', 'disabled');
        sdms.info("Please wait...");
        $.ajax({
            async: true,
            type: "POST",
            data: params,
            url: url,
            success: function (data) {
                if (data.message == "") {
                    location.href = 'wh.api/Report/DownloadExcelFile?key=' + data.value;
                } else {
                    alert(data.message);
                    //sdms.info(data.message, "Error");
                }
            }
        });
        $('#btnExportXls').removeAttr('disabled');
        //widget.exportXls({
        //    source: url,
        //    params: params,
        //    fileName: "Review SFM",
        //    items: [
        //        { type: "text",  name: "CompanyName", text: "Dealer Name", width: 400 },
        //        { type: "text",  name: "BranchName", text: "Outlet Name", width: 400 },
        //        { type: "text",  name: "TotalSalesman", text: "Total Wiraniaga", width: 150 },
        //        { type: "text",  name: "Trainee", text: "Trainee", width: 150 },
        //        { type: "text",  name: "Silver", text: "Silver", width: 150 },
        //        { type: "text",  name: "Gold", text: "Gold", width: 150 },
        //        { type: "text",  name: "Platinum", text: "Platinum", width: 150 },
        //        { type: "text",  name: "SC", text: "SC", width: 150 },
        //        { type: "text",  name: "SH", text: "SH", width: 150 },
        //        { type: "text",  name: "BM", text: "BM", width: 150 },
        //        { type: "text",  name: "TotalSCSHBM", text: "Total SC SH BM", width: 250 },
        //    ]
        //});
    }
});

