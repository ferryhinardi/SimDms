$(document).ready(function () {
    var options = {
        xtype: "panels",
        title: "SH BM Training",
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
                name: "gridScShBmTraining",
                xtype: "k-grid"
            }
        ]
    };

    var widget = new SimDms.Widget(options);
    widget.render(renderCallback);

    function renderCallback() {
        $("[name=GroupArea]").on("change", function () {
            var groupArea = $("[name=GroupArea]").val();
            if (groupArea == '' || groupArea == undefined) {
                $("#CompanyCode").prop('disabled', true);
                $("#CompanyCode").val('');
                $("#CompanyCode").select2('val', '');
            }
            else {
                $("#CompanyCode").prop('disabled', false);
            }

            widget.select({ selector: "[name=CompanyCode]", url: "wh.api/combo/ComboDealerList", params: { LinkedModule: "mp", GroupArea: groupArea }, optionalText: "-- SELECT ALL --" });
            $("[name=CompanyCode]").prop("selectedIndex", 0);
            $("[name=CompanyCode]").change();
        });
        $("[name=CompanyCode]").on("change", function () {
            var companyCode = $("[name=CompanyCode]").val();
            if (companyCode == '' || companyCode == undefined) {
                $("#BranchCode").prop('disabled', true);
                $("#BranchCode").val('');
                $("#BranchCode").select2('val', '');
            }
            else {
                $("#BranchCode").prop('disabled', false);
            }

            widget.select({ selector: "[name=BranchCode]", url: "wh.api/combo/ComboOutletList", params: { CompanyCode: $("#pnlFilter [name=CompanyCode]").val() }, optionalText: "-- SELECT ALL --" });
            $("[name=BranchCode]").prop("selectedIndex", 0);
            $("[name=BranchCode]").change();
        });

        initializeValue();
        initializeEvent();
    }

    function initializeValue() {
        widget.setSelect([{ name: "GroupArea", url: "wh.api/combo/GroupAreas", optionalText: "-- SELECT ALL --" }]);
        $("[name='ByDate']").val(widget.toDateFormat(new Date()));

        $("#CompanyCode").prop('disabled', true);
        $("#BranchCode").prop('disabled', true);
    }

    function initializeEvent() {
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

        //$("[name=GroupCode], [name='CompanyCode'], [name='BranchCode'], [name='ByDate']").on("change", function (evt) {
        //    reloadData();
        //});

        //reloadData();
    }

    function reloadData() {
        var params = {
            GroupArea: $("[name='GroupArea']").val(),
            CompanyCode: $("[name='CompanyCode']").val(),
            BranchCode: $("[name='BranchCode']").val(),
            ByDate: $("[name='ByDate']").val(),
        };
        var url = "wh.api/Inquiry/SfmScShBmTrainingNew";

        widget.kgrid({
            url: url,
            name: "gridScShBmTraining",
            serverBinding: true,
            pageSize: 10,
            params: params,
            columns: [
                //{ field: "CompanyName", title: "Dealer Name", width: 400 },
                { field: "BranchName", title: "Outlet Name", width: 400 },
                { field: "SC", title: "SC", width: 70 },
                { field: "SH", title: "SH", width: 70 },
                { field: "BM", title: "BM", width: 70 },
                { field: "SCBasic", title: "SC Basic", width: 100 },
                { field: "SCAdvance", title: "SC Advance", width: 100 },
                { field: "SHBasic", title: "SH Basic", width: 100 },
                { field: "SHIntermediate", title: "SH Intermediate", width: 150 },
                { field: "SHAdvance", title: "SH Advance", width: 120 },
                { field: "BMBasic", title: "BM Basic", width: 120 },
                { field: "BMIntermediate", title: "BM Intermediate", width: 150 },
                { field: "BMAdvance", title: "BM Advance", width: 120 },
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
        var url = "wh.api/Report/SfmScShBmTrainingNew";

        $('#btnExportXls').attr('disabled', 'disabled');
        sdms.info("Please wait...");
        $.ajax({
            async: true,
            type: "POST",
            data: params,
            url: url,
            success: function (data) {
                if (data.message == "") {
                    console.log(data);
                    location.href = 'wh.api/Report/DownloadExcelFile?key=' + data.value;
                } else {
                    sdms.info(data.message, "Error");
                }
            }
        });
        $('#btnExportXls').removeAttr('disabled');
        //widget.exportXls({
        //    source: url,
        //    params: params,
        //    fileName: "SC SH BM Training",
        //    items: [
        //        { name: "CompanyName", text: "Dealer Name", width: 400 },
        //        { name: "BranchName", text: "Outlet Name", width: 400 },
        //        { name: "SC", text: "SC", width: 150 },
        //        { name: "SH", text: "SH", width: 150 },
        //        { name: "BM", text: "BM", width: 150 },
        //        { name: "SCBasic", text: "SC Basic", width: 150 },
        //        { name: "SCAdvance", text: "SC Advance", width: 150 },
        //        { name: "SHBasic", text: "SH Basic", width: 150 },
        //        { name: "SHIntermediate", text: "SH Intermediate", width: 150 },
        //        { name: "SHAdvance", text: "SH Advance", width: 150 },
        //        { name: "BMBasic", text: "BM Basic", width: 150 },
        //        { name: "BMIntermediate", text: "BM Intermediate", width: 150 },
        //        { name: "BMAdvance", text: "BM Advance", width: 150 },
        //    ]
        //});
    }
});

