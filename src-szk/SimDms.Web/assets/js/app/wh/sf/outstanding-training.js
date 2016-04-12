$(document).ready(function () {
    var options = {
        xtype: "panels",
        title: "Outstanding Training",
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
                name: "gridOutstandingTraining",
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
        var url = "wh.api/Inquiry/SfmOutstandingTrainingNew";

        widget.kgrid({
            url: url,
            name: "gridOutstandingTraining",
            serverBinding: true,
            pageSize: 10,
            params: params,
            columns: [
                { field: "Department", title: "Department", width: 150 },
                { field: "Position", title: "Position", width: 150 },
                { field: "GradeName", title: "Grade", width: 100 },
                { field: "TrainingName", title: "Training Name", width: 200 },
                { field: "TrainingDescription", title: "Training Description", width: 200 },
                { field: "ManPower", title: "Man Power", width: 100 },
                { field: "Trained", title: "Trained", width: 100 },
                { field: "NotTrained", title: "Not Trained", width: 100 },
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
        var url = "wh.api/Report/SfmOutstandingTrainingNew";

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
                    sdms.info(data.message, "Error");
                }
            }
        });
        $('#btnExportXls').removeAttr('disabled');
        //widget.exportXls({
        //    source: url,
        //    params: params,
        //    fileName: "Outstanding Training",
        //    items: [
        //        { name: "Department", text: "Department", width: 150 },
        //        { name: "Position", text: "Position", width: 150 },
        //        { name: "GradeName", text: "Grade", width: 100 },
        //        { name: "TrainingName", text: "Training Name", width: 200 },
        //        { name: "TrainingDescription", text: "Training Description", width: 200 },
        //        { name: "ManPower", text: "Man Power", width: 100 },
        //        { name: "Trained", text: "Trained", width: 100 },
        //        { name: "NotTrained", text: "Not Trained", width: 100 },
        //    ]
        //});
    }
});

