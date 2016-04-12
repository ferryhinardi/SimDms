$(document).ready(function () {
    var options = {
        title: "Training Detail",
        xtype: "panels",
        panels: [
            {
                name: "pnlFilter",
                items: [
                    {
                        text: "Area",
                        type: "controls",
                        items: [
                            { name: "GroupArea", text: "Area", cls: "span4", type: "select", opt_text: "-- SELECT ALL --" },
                        ]
                    },
                    {
                        text: "Dealer Name",
                        type: "controls",
                        items: [
                            { name: "CompanyCode", cls: "span4", type: "select", opt_text: "-- SELECT ALL --" },
                        ]
                    },
                    {
                        text: "Outlet",
                        type: "controls",
                        items: [
                            { name: "BranchCode", cls: "span4", type: "select", opt_text: "-- SELECT ALL --" },
                        ]
                    },
                    {
                        text: "Position",
                        type: "controls",
                        items: [
                            { name: "Position", cls: "span4", type: "select", opt_text: "-- SELECT ALL --" },
                        ]
                    },
                    {
                        text: "Training Program",
                        type: "controls",
                        items: [
                            { name: "TrainingProgram", cls: "span4", type: "select", opt_text: "-- SELECT ONE --" },
                        ]
                    },
                ],
            },
            { name: "TrainingDetail", xtype: "k-grid" },
        ],
        toolbars: [
            { name: "btnRefresh", action: 'refresh', text: "Refresh", icon: "fa fa-refresh" },
            { action: 'expand', text: 'Expand', icon: 'fa fa-expand' },
            { action: 'collapse', text: 'Collapse', icon: 'fa fa-compress', cls: 'hide' },
            { name: "btnExportXls", action: 'export', text: "Export (xls)", icon: "fa fa-file-excel-o" },
        ],
        onToolbarClick: function (action) {
            //console.log(action);
            switch (action) {
                case 'collapse':
                    widget.exitFullWindow();
                    widget.showToolbars(['refresh', 'export', 'expand']);
                    break;
                case 'expand':
                    widget.requestFullWindow();
                    widget.showToolbars(['refresh', 'export', 'collapse']);
                    break;
                case 'export':
                    exportXls();
                    break;
                default:
                    break;
            }
        },
    }
    var widget = new SimDms.Widget(options);
    widget.setSelect([{ name: "GroupArea", url: "wh.api/combo/GroupAreas", optionalText: "-- SELECT ALL --" }]);
    widget.select({ selector: "[name=TrainingProgram]", url: "wh.api/combo/TrainingPrograms", params: { dept: "SALES" }, optionalText: "-- SELECT ONE --" });
    widget.default = { Status: "1", CompanyCode: "", PeriodeBeg: new Date(), PeriodeEnd: new Date() };

    widget.render(function () {
        $("#CompanyCode").prop('disabled', true);
        $("#BranchCode").prop('disabled', true);
        widget.populate(widget.default);
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

            widget.select({ selector: "[name=CompanyCode]", url: "wh.api/combo/ComboDealerList", params: { LinkedModule: "mp", GroupArea: $("[name=GroupArea]").val() }, optionalText: "-- SELECT ALL --" });
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
            widget.select({ selector: "[name=Position]", url: "wh.api/combo/Positionsv2", params: { dept: "SALES" }, optionalText: "-- SELECT ALL --" });
            $("[name=BranchCode]").prop("selectedIndex", 0);
            $("[name=Position]").prop("selectedIndex", 0);
            $("[name=BranchCode]").change();
        });
        $("#btnRefresh").off().on("click", refreshGrid);
    });

    function refreshGrid() {

        var areas = $("[name=GroupArea]").val();
        var comps = $("[name=CompanyCode]").val();
        var outls = $("[name=BranchCode]").val();
        var posts = $("[name=Position]").val();
        var tprgs = $("[name=TrainingProgram]").val()

        if (tprgs == '') {
            //alert("Training Program harus dipilih");
            sdms.info({ type: "warning", text: "Training Program harus dipilih" });
            return;
        }
        var params = {
            GroupArea: areas,
            CompanyCode: comps,
            outl: outls,
            post: posts,
            tprg: tprgs,
            flt1: "",
            flt2: ""
        }
        widget.kgrid({
            url: "wh.api/TrainingDetailDashboard/QueryTrainingDetailNew",
            name: "TrainingDetail",
            params: params,
            serverBinding: true,
            pageable: false,
            pageSize: 1000,
            dataBound: onDataBound,
            columns: [
                    { field: "OutletAbbr", width: 500, title: "Outlet Name", footerTemplate: 'TOTAL' },
                    { field: "Jml", width: 50, title: "JML", footerTemplate: '#=sum#', type: 'align-right' },
                    { field: "T", width: 50, title: "T", footerTemplate: '#=sum#', type: 'align-right' },
                    { field: "NT", width: 50, title: "NT", footerTemplate: '#=sum#', type: 'align-right' },
                    { field: "OutletCode", width: 75, title: "Outlet Code", hidden: 'true' },
            ],
            aggregate: [

                { field: "Jml", aggregate: "sum" },
                { field: "T", aggregate: "sum" },
                { field: "NT", aggregate: "sum" }
            ],
        });

        $('#TrainingDetail').css({
            "min-height": "100px",
            "max-height": "300px",
            "overflow": "scroll"
        });
    }

    $("#window").html("");
    function onDataBound(e) {
        var grid = $("#TrainingDetail").data("kendoGrid");
        $(grid.tbody).on("dblclick", "td", function (e) {
            var row = $(this).closest("tr");
            var rowDtl = $(this).closest("tr").children('td');
            var rowIdx = $("tr", grid.tbody).index(row);
            var colIdx = $("td", row).index(this);
            var value = rowDtl.eq(colIdx).text();
            var valColumns = rowDtl.eq(0).text();
            var outlt = rowDtl.eq(4).text();
            var colName = $("#TrainingDetail thead th").eq(colIdx).text();
            //alert('area :' + $("[name=GroupArea]").val() + ' ' + rowIdx + '-' + colIdx + '-' + value + ' - ' + valColumns + ' - ' + outlt+ ' - ' + colName);

            getDetail(rowIdx, colIdx, value, valColumns, outlt, colName);
        });
    }

    function getDetail(rowIdx, colIdx, value, valColumns, outlt, colName) {
        var kolom = "";

        if (colIdx == 1) {
            if (value == 0) {
                sdms.notify({ type: "warning", text: "Tidak ada data detail" });
                return;
            }
            else {
                kolom = 'Detail MP';
                createGridDtl(valColumns, kolom, colName, outlt);
            }
        }
        if (colIdx == 2) {
            if (value == 0) {
                sdms.notify({ type: "warning", text: "Tidak ada data detail" });
                return;
            }
            else {
                kolom = 'Detail T';
                createGridDtl(valColumns, kolom, colName, outlt);
            }
        }
        if (colIdx == 3) {
            if (value == 0) {
                sdms.notify({ type: "warning", text: "Tidak ada data detail" });
                return;
            }
            else {
                kolom = 'Detail NT';
                createGridDtl(valColumns, kolom, colName, outlt);
            }
        }
    }

    function createGridDtl(valColumns, kolom, clm, outlt) {

        var area = $("[name=GroupArea]").val();
        var comp = $("[name=CompanyCode]").val();
        var outl = $("[name=BranchCode]").val();
        var post = $("[name=Position]").val();
        var tprg = $("[name=TrainingProgram]").val();
        var flt1 = outlt;
        var flt2 = clm;

        sdms.lookup({
            title: valColumns + ' : ' + kolom,
            url: 'wh.api/TrainingDetailDashboard/QueryTrainingDetailDataNew?GroupArea=' + area + '&CompanyCode=' + comp + '&outl=' + outl + '&post=' + post + '&tprg=' + tprg + '&flt1=' + flt1 + '&flt2=' + flt2,
            fields: [
                { name: 'EmployeeName', text: 'Nama', width:250 },
                { name: 'Position', text: 'Jabatan', width: 120 },
                { name: 'Grade', text: 'Grade', width: 120 },
                { name: 'OutletAbbr', text: 'Outlet', width: 350 },
                { name: 'JoinDate', text: 'Join Date', width: 110, type: 'date' },
                { name: 'Gender', text: 'Jenis Kelamin', width: 50 },
                { name: 'BirthDate', text: 'Tanggal lahir', width: 110, type: 'date' },
                { name: 'Email', text: 'Email', width: 220 },
                { name: "Telephone", text: "No telp", width: 120 }
            ],
        });

        var ftr = $('.modal-footer :nth-child(1)');
        ftr.hide();
    }

    function exportXls() {
        var prms = {
            GroupArea: $("[name=GroupArea]").val(),
            CompanyCode: $("[name=CompanyCode]").val(),
            outl: $("[name=BranchCode]").val(),
            post: $("[name=Position]").val(),
            tprg: $("[name=TrainingProgram]").val(),
            flt1: "",
            flt2: ""
        }
        sdms.report({
            url: 'wh.api/TrainingDetailDashboard/ExportTrainingDetailNew',
            type: 'xlsx',
            params: prms
        });
    }
});