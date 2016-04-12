$(document).ready(function () {
    var options = {
        xtype: "panels",
        title: "Turn Over",
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
                name: "gridTurnOver",
                xtype: "k-grid"
            }
        ]
    };

    var widget = new SimDms.Widget(options);
    widget.render(renderCallback);

    function renderCallback() {
        widget.setSelect([
            {
                name: "GroupArea",
                url: "wh.api/combo/GroupAreas",
                optionalText: "-- SELECT ALL -- "
            },
            //{
            //    name: "CompanyCode",
            //    url: "wh.api/combo/Companies",
            //    optionalText: "-- SELECT ALL -- ",
            //    cascade: {
            //        name: "GroupArea"
            //    }
            //},
            //{
            //    name: "BranchCode",
            //    url: "wh.api/combo/Branches",
            //    optionalText: "-- SELECT ALL -- ",
            //    cascade: {
            //        name: "CompanyCode"
            //    }
            //}
        ]);

        $("[name=GroupArea]").on("change", function () {
            widget.select({ selector: "[name=CompanyCode]", url: "wh.api/combo/ComboDealerList", params: { GroupArea: $("#pnlFilter [name=GroupArea]").val() }, optionalText: "-- SELECT ALL --" });
            $("[name=CompanyCode]").prop("selectedIndex", 0);
            $("[name=CompanyCode]").change();
        });
        $("[name=CompanyCode]").on("change", function () {
            widget.select({ selector: "[name=BranchCode]", url: "wh.api/combo/ComboOutletList", params: { companyCode: $("#pnlFilter [name=CompanyCode]").val() }, optionalText: "-- SELECT ALL --" });
            $("[name=BranchCode]").prop("selectedIndex", 0);
            $("[name=BranchCode]").change();
        });

        initializeValue();
        initializeEvent();

        $('#CompanyCode').attr('disabled', 'disabled');
        $('#BranchCode').attr('disabled', 'disabled');
    }

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

    function initializeValue() {
        $("[name='ByDate']").val(widget.toDateFormat(new Date()));
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
        var url = "wh.api/Inquiry/SfmTurnOver";

        widget.kgrid({
            url: url,
            name: "gridTurnOver",
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
                { field: "TraineeTerminated", title: "Trainee Terminated", width: 150 },
                { field: "SilverTerminated", title: "Silver Terminated", width: 150 },
                { field: "GoldTerminated", title: "Gold Terminated", width: 150 },
                { field: "PlatinumTerminated", title: "Platinum Terminated", width:150 },
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
        var url = "wh.api/Report/SfmTurnOver";
        $('#btnExportXls').attr('disabled', 'disabled');
        sdms.info("Please wait...");
        $.ajax({
            async: true,
            type: "POST",
            data: params,
            url: url,
            success: function (data) {
                if (data.message == "") {
                //    console.log(data);
                    location.href = 'wh.api/Report/DownloadExcelFile?key=' + data.value + '&filename=TurnOverData';
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
        //    fileName: "Turn Over",
        //    items: [
        //        { name: "CompanyName", text: "CompanyName", type: "text", width: 400, type: "text" },
        //        { name: "BranchName", text: "BranchName", type: "text", width: 400, type: "text" },
        //        { name: "Trainee", text: "Trainee", width: 90, type: "text" },
        //        { name: "Silver", text: "Silver", width: 90, type: "text" },
        //        { name: "Gold", text: "Gold", type: "text", width: 90, type: "text" },
        //        { name: "Platinum", text: "Platinum", width: 90, type: "text", type: "text" },
        //        { name: "TraineeTerminated", text: "Gold Terminated", width: 250, type: "text" },
        //        { name: "SilverTerminated", text: "Gold Terminated", width: 250, type: "text" },
        //        { name: "GoldTerminated", text: "Gold Terminated", width: 250, type: "text" },
        //        { name: "PlatinumTerminated", text: "Platinum Terminated", type: "text", width: 250, type: "text" },
        //    ]
        //});
    }
});

