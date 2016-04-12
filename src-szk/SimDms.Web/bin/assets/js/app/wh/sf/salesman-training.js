$(document).ready(function () {
    var options = {
        xtype: "panels",
        title: "Salesman Training",
        toolbars: [
            { name: "btnRefresh", text: "Refresh", icon: "fa fa-refresh" },
            { name: "btnExportXls", text: "Export (xls)", icon: "fa fa-file-excel-o" },
        ],
        panels: [
            {
                name: "pnlFilter",
                //title: "Filter",
                items: [
                    { name: "GroupArea", type: "select", cls: "span6", text: "Area", readonly: false, opt_text: "-- SELECT ALL --" },
                    { name: "CompanyCode", type: "select", cls: "span6", text: "Dealer Name", readonly: false, opt_text: "-- SELECT ALL --" },
                    { name: "BranchCode", type: "select", cls: "span6", text: "Branch Name", readonly: false, opt_text: "-- SELECT ALL --" },
                    { name: "ByDate", type: "datepicker", cls: "span3", text: "By Date", readonly: false, opt_text: "-- SELECT ALL --" },
                ]
            },
            {
                name: "gridSalesmanTraining",
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

        // reloadData();
    }

    function reloadData() {
        var params = {
            GroupArea: $("[name='GroupArea']").val(),
            CompanyCode: $("[name='CompanyCode']").val(),
            BranchCode: $("[name='BranchCode']").val(),
            ByDate: $("[name='ByDate']").val(),
        };
        var url = "wh.api/Inquiry/SfmSalesmanTrainingNew";

        widget.kgrid({
            url: url,
            name: "gridSalesmanTraining",
            serverBinding: true,
            pageSize: 10,
            params: params,
            columns: [
                //{ field: "CompanyName", title: "Dealer Name", width: 400 },
                { field: "BranchName", title: "Outlet Name", width: 400 },
                { field: "Trainee", title: "Trainee", width: 90 },
                { field: "Silver", title: "Silver", width: 90 },
                { field: "Gold", title: "Gold", width: 90 },
                { field: "Platinum", title: "Platinum", width: 90 },
                { field: "TotalSalesman", title: "Total Wiraniaga", width: 130 },
                { field: "GoldTerminated", title: "Gold Terminated", width: 160 },
                { field: "PlatinumTerminated", title: "Platinum Terminated", width: 160 },
                { field: "STDP1", title: "STDP 1", width: 80 },
                { field: "STDP2", title: "STDP 2", width: 80 },
                { field: "STDP3", title: "STDP 3", width: 80 },
                { field: "STDP4", title: "STDP 4", width: 80 },
                { field: "STDP5", title: "STDP 5", width: 80 },
                { field: "STDP6", title: "STDP 6", width: 80 },
                { field: "STDP7", title: "STDP 7", width: 80 },
                { field: "TotalSTDP", title: "Total STDP", width: 100 },
                { field: "SPSSilver", title: "SPS Silver", width: 100 },
                { field: "SPSGold", title: "SPS Gold", width: 100 },
                { field: "SPSPlatinum", title: "SPS Platinum", width: 120 },
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
        var url = "wh.api/Report/SfmSalesmanTrainingNew";

        $.ajax({
            async: true,
            type: "POST",
            data: params,
            url: url,
            success: function (data) {
                if (data.message == "") {
                    location.href = 'wh.api/report/DownloadExcelFile?key=' + data.value + '&filename=Salesman Training Report';
                } else {
                    sdms.info(data.message, "Error");
                }
            }
        });
        /*
        widget.exportXls({
            source: url,
            params: params,
            fileName: "Salesman Training",
            items: [
                { type: "text", name: "CompanyName", text: "Dealer Name", width: 400 },
                { type: "text", name: "BranchName", text: "Outlet Name", width: 400 },
                { type: "text", name: "Trainee", text: "Trainee", width: 90 },
                { type: "text", name: "Silver", text: "Silver", width: 90 },
                { type: "text", name: "Gold", text: "Gold", width: 90 },
                { type: "text", name: "Platinum", text: "Platinum", width: 90 },
                { type: "text", name: "TotalSalesman", text: "Total Wiraniaga", width: 130 },
                { type: "text", name: "GoldTerminated", text: "Gold Terminated", width: 160 },
                { type: "text", name: "PlatinumTerminated", text: "Platinum Terminated", width: 160 },
                { type: "text", name: "STDP1", text: "STDP 1", width: 80 },
                { type: "text", name: "STDP2", text: "STDP 2", width: 80 },
                { type: "text", name: "STDP3", text: "STDP 3", width: 80 },
                { type: "text", name: "STDP4", text: "STDP 4", width: 80 },
                { type: "text", name: "STDP5", text: "STDP 5", width: 80 },
                { type: "text", name: "STDP6", text: "STDP 6", width: 80 },
                { type: "text", name: "STDP7", text: "STDP 7", width: 80 },
                { type: "text", name: "TotalSTDP", text: "Total STDP", width: 100 },
                { type: "text", name: "SPSSilver", text: "SPS Silver", width: 100 },
                { type: "text", name: "SPSGold", text: "SPS Gold", width: 100 },
                { type: "text", name: "SPSPlatinum", text: "SPS Platinum", width: 120 },
            ]
        });
        */
    }
});

