var rbDataSource;
$(document).ready(function () {
    var options = {
        title: "Inquiry MSI V2",
        xtype: "panels",
        panels: [
            {
                name: "pnlFilter",
                items: [
                    {
                        text: "Area",
                        type: "controls",
                        items: [
                            { name: "Area", text: "Areas", cls: "span6", type: "select", opt_text: "-- SELECT ALL --" },
                        ]
                    },

                    {
                        text: "Dealer Name",
                        type: "controls",
                        items: [
                            { name: "CompanyCode", cls: "span6", type: "select", opt_text: "-- SELECT ALL --" },
                        ]
                    },
                    {
                        text: "Outlet Name",
                        type: "controls",
                        items: [
                            { name: "BranchCode", cls: "span6", type: "select", opt_text: "-- SELECT ALL --" },
                        ]
                    },
                    {
                        text: "Tahun",
                        type: "controls",
                        items: [
                            { name: "Year", text: "Year", cls: "span2", type: "select" },
                        ]
                    },
                    {
                        name: "DataSource", id: "DataSource", text: "Data Source", cls: "span4", type: "radiobutton", items: [
                        { id: 'inv', value: 'Invoice', label: 'Invoice', checked: true },
                        { id: 'spk', value: 'SPK', label: 'SPK' }
                        ]
                    },
                ],
            },
            {
                name: "InqPers",
                xtype: "k-grid",
            },
        ],
        toolbars: [
            //{ name: "btnRefresh", text: "Refresh", icon: "fa fa-refresh" },
            { name: "btnRefresh", text: "Load Data", icon: "fa fa-search" },
            { name: "btnExportXls", text: "Export (xls)", icon: "fa fa-file-excel-o" },
        ],
    }


    var widget = new SimDms.Widget(options);

    widget.setSelect([{ name: "Area", url: "wh.api/combo/SrvGroupAreas", optionalText: "-- SELECT ALL --" }]);
    widget.select({ selector: "[name=Year]", url: "wh.api/combo/years", optionalText: "-- SELECT ONE --" });
    widget.default = { Year: new Date().getFullYear() };

    var area = "";

    console.log(new Date().getFullYear());
    widget.render(function () {
        widget.populate(widget.default);
        $('#CompanyCode, #BranchCode').attr('disabled', 'disabled');
        $("[name=Area]").on("change", function () {
            //widget.select({ selector: "[name=CompanyCode]", url: "wh.api/combo/OrganizationsV2", params: { area: $("#pnlFilter [name=Area]").val() }, optionalText: "-- SELECT ALL --" });
            widget.select({ selector: "[name=CompanyCode]", url: "wh.api/combo/SrvDealerList", params: { LinkedModule: "mp", GroupArea: $("[name=Area]").val() }, optionalText: "-- SELECT ALL --" });
            $("[name=CompanyCode]").prop("selectedIndex", 0);
            $("[name=CompanyCode]").change();

            console.log($("#pnlFilter [name=Area]").val(), $("#pnlFilter [name=CompanyCode]").val(), $("#pnlFilter [name=BranchCode]").val(""));
        });
        $("[name=CompanyCode]").on("change", function () {
            //widget.select({ selector: "[name=BranchCode]", url: "wh.api/combo/branchsV2", params: { area: $("#pnlFilter [name=Area]").val(), comp: $("#pnlFilter [name=CompanyCode]").val() }, optionalText: "-- SELECT ALL --" });
            widget.select({ selector: "[name=BranchCode]", url: "wh.api/combo/SrvBranchList", params: { area: $("#pnlFilter [name=Area]").val(), comp: $("#pnlFilter [name=CompanyCode]").val() }, optionalText: "-- SELECT ALL --" });
            $("[name=BranchCode]").prop("selectedIndex", 0);
            $("[name=BranchCode]").change();

            console.log($("#pnlFilter [name=CompanyCode]").val(), $('#CompanyCode option:selected').text());
        });

        // no need
        //widget.post("wh.api/inquiry/isNational", function (result) {
        //    isNational = result.isNational;
        //    area = result.area;
            //isNational = 1;
            //if (isNational == 0) {

            //    $("#Area").attr("disabled", "disabled");
            //    $("#CompanyCode").attr("disabled", "disabled");
            //    $("#BranchCode").attr("disabled", "disabled");

            //    $('#Area option:selected ').val(result.area);
            //    $('#Area option:selected ').text(result.area);

            //    $('#CompanyCode option:selected ').val(result.dealerCd);
            //    $('#CompanyCode option:selected ').text(result.dealerNm);

            //    $('#BranchCode option:selected ').val(result.outletCd);
            //    $('#BranchCode option:selected ').text(result.outletNm);
            //}
            //else {
            //    $("#Area").removeAttr("disabled");
            //    $("#CompanyCode").removeAttr("disabled");
            //    $("#BranchCode").removeAttr("disabled");

            //}
        //    console.log($('#Area option:selected ').text(), $('#CompanyCode option:selected ').text(), result);
        //});

        rbDataSource = $("input[name=DataSource]").val();
        console.log(rbDataSource);

        $("input[name=DataSource]").on("change", function () {

            rbDataSource = $(this).val();
            console.log(rbDataSource);
        });
        
        $('select').on('change', ResetCombo);
    });

    $("#btnRefresh").on("click", refreshGrid);
    $("#btnExportXls").on("click", exportXls);
    //$("#pnlFilter select").on("change", refreshGrid);
    $("#pnlFilter select").on("change", clearGrid);

    function ResetCombo() {
        if ($('#Area').val() == "") {
            $('#CompanyCode').val('');
            $('[name="CompanyCode"]').html('<option value="">-- SELECT ALL --</option>');
            $('#CompanyCode').attr('disabled', 'disabled');
            $('#BranchCode').attr('disabled', 'disabled');
        }
        else {
            $('#CompanyCode').removeAttr('disabled', 'disabled');
        }

        if ($('#CompanyCode').val() == "") {
            $('#BranchCode').attr('disabled', 'disabled');
        }
        else {
            $('#BranchCode').removeAttr('disabled', 'disabled');
        }
    }

    function clearGrid() {
        $("#InqPers").empty();
    }    

    function refreshGrid() {
        var params = $("#pnlFilter").serializeObject();
        console.log(params);
        params.Department = "SERVICE",
        widget.kgrid({
            url: "wh.api/inquiry/SvMsiV2",
            name: "InqPers",
            params: params,
            sortable: false,
            filterable: false,
            pageable: false,
            pageSize: 200,
            group: [
                   { field: "MsiGroup" },
            ],
            columns: [
                { field: "SeqNo", title: "No", width: 60 },
                { field: "MsiDesc", title: "Description", width: 650 },
                { field: "Unit", title: "Unit", width: 100 },
                { field: "Average", title: "Average", width: 150, type: 'decimal' },
                { field: "Total", title: "Total", width: 150, type: 'decimal' },
                { field: "Month01", title: "Jan", width: 150, type: 'decimal' },
                { field: "Month02", title: "Feb", width: 150, type: 'decimal' },
                { field: "Month03", title: "Mar", width: 150, type: 'decimal' },
                { field: "Month04", title: "Apr", width: 150, type: 'decimal' },
                { field: "Month05", title: "May", width: 150, type: 'decimal' },
                { field: "Month06", title: "Jun", width: 150, type: 'decimal' },
                { field: "Month07", title: "Jul", width: 150, type: 'decimal' },
                { field: "Month08", title: "Aug", width: 150, type: 'decimal' },
                { field: "Month09", title: "Sep", width: 150, type: 'decimal' },
                { field: "Month10", title: "Oct", width: 150, type: 'decimal' },
                { field: "Month11", title: "Nov", width: 150, type: 'decimal' },
                { field: "Month12", title: "Dec", width: 150, type: 'decimal' },
            ],
        });

        console.log(params);
    }

    function exportXls() {
        var spID = "";
        if (rbDataSource=="Invoice") {
            spID = "uspfn_WhInqMsiV2";
        }
        else {
            spID = "uspfn_WhInqMsiV2_SPK";
        }
        var url = "wh.api/inquiry/exportExcel?";
        //var spID = "uspfn_WhInqMsiV2";
        var area = $('[name=Area]').val();
        var dealer = $('[name=CompanyCode]').val();
        var outlet = $('[name=BranchCode]').val();
        var year = $('[name=Year]').val();
        var textArea = $('#Area option:selected').text();
        var textDealer = $('#CompanyCode option:selected').text();
        var textOutlet = $('#BranchCode option:selected ').text();
        
        var
        params = "&Area=" + area;
        params += "&Dealer=" + dealer;
        params += "&Outlet=" + outlet;
        params += "&SpID=" + spID;
        params += "&Year=" + year;
        params += "&TextArea=" + textArea;
        params += "&TextDealer=" + textDealer;
        params += "&TextOutlet=" + textOutlet;
        params += "&DataSource=" + rbDataSource;

        url = url + params;
        window.location = url;

        console.log(url);
    }

    //var isNational = "";
    //var area = "";
    //$.post('wh.api/inquiry/isNational', function (result) {
    //    widget.default = $.extend({}, result);
    //    isNational = result.isNational;
    //    area = result.area;
    //    //isNational = 1;
    //    if (isNational == 0) {

    //        $("#Area").attr("disabled", "disabled");
    //        $("#CompanyCode").attr("disabled", "disabled");
    //        $("#BranchCode").attr("disabled", "disabled");

    //        $('#Area option:selected ').val(result.area);
    //        $('#Area option:selected ').text(result.area);

    //        $('#CompanyCode option:selected ').val(result.dealerCd);
    //        $('#CompanyCode option:selected ').text(result.dealerNm);

    //        $('#BranchCode option:selected ').val(result.outletCd);
    //        $('#BranchCode option:selected ').text(result.outletNm);
    //    }
    //    else {
    //        $("#Area").removeAttr("disabled");
    //        $("#CompanyCode").removeAttr("disabled");
    //        $("#BranchCode").removeAttr("disabled");

    //    }
    //    console.log($('#Area option:selected ').text(), $('#CompanyCode option:selected ').text(), result);
    //});
    //console.log($('#Area option:selected ').text());    
});
