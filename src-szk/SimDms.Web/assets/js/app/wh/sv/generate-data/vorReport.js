"use strict";
$(document).ready(function () {
    var options = {
        title: "VEHICLE OFF THE ROAD",
        xtype: "panels",
        panels: [
            {
                name: "pnlFilter",
                items: [
                    { name: "ProblemService", text: "Penyebab Masalah", cls: "span4", type: "select", opt_text: "-- SELECT ALL --" },
                    { name: "Area", text: "Area", cls: "span4", type: "select", opt_text: "-- SELECT ALL --" },
                    { name: "Model", text: "Model Kendaraan", cls: "span4", type: "select", opt_text: "-- SELECT ALL --" },
                    { name: "Dealer", text: "Dealer", cls: "span4", type: "select", opt_text: "-- SELECT ALL --" },
                    {
                        text: "Periode Case",
                        type: "controls",
                        cls: "span4",
                        items: [
                            { name: "DateFrom", text: "Date From", cls: "span4", type: "datepicker" },
                            { name: "DateTo", text: "Date To", cls: "span4", type: "datepicker" },
                        ]
                    },
                    { name: "Outlet", text: "Outlet", cls: "span4", type: "select", opt_text: "-- SELECT ALL --" },
                    {
                        text: "Work in Progress",
                        cls: "span4",
                        type: "controls",
                        items: [
                            { name: "WIP", text: " Work in Progress", cls: "span3", type: "text" },
                            { name: "WIPlbl", text: "hari", cls: "span1", type: "label" },
                        ]
                    },
                    {
                        text: "Status",
                        type: "controls",
                        cls: "span4",
                        items: [
                            { name: "Open", text: "Open", type: "check", cls: "span4" },
                            { name: "Closed", text: "Closed", type: "check", cls: "span2" },
                        ]
                    },
                ],
            },
            {
                name: "VORTable",
                xtype: "k-grid",
            },
        ],
        toolbars: [
            { text: "Save Changes", name: "saveChanges", action: "saveChanges", icon: "fa fa-save", additional: "style='display: none'" },
            { text: "Cancel Save", name: "cancelSave", action: "cancelSave", icon: "fa fa-cancel", additional: "style='display: none'" },
            { text: "Refresh", name:"refresh", action: "refresh", icon: "fa fa-refresh" },
            { text: "Cancel VOR", name: "cancelVOR", action: "cancelVOR", icon: "fa fa-cancel", additional: "style='display: none'" },
            { text: "Print", name: "print", action: "export", icon: "fa fa-download" },
            { text: "Print Summary", name: "printsumm", action: "export_summary", icon: "fa fa-download" },
        ],
        onToolbarClick: function (action) {
            switch (action) {
                case "refresh":
                    refreshGrid();
                    $('#cancelVOR').show();
                    break;
                case "cancelVOR":
                    sdms.info("Silahkan pilih data VOR", "");
                    $('#saveChanges').show();
                    $('#cancelSave').show();
                    $('#cancelVOR').hide();
                    $('#refresh').hide();
                    $('#print').hide();
                    $('#printsumm').hide();
                    break;
                case "saveChanges":
                    cancelVOR();
                    break;
                case "cancelSave":
                    $('#saveChanges').hide();
                    $('#cancelSave').hide();
                    $('#cancelVOR').show();
                    $('#refresh').show();
                    $('#print').show();
                    $('#printsumm').show();
                    break;
                case "export":
                    exportXls();
                    break;
                case "export_summary":
                    exportSummaryXls();
                    break;
            }
        },
    };
    var widget = new SimDms.Widget(options);

    widget.setSelect([
        {
            name: "Area",
            url: "wh.api/combo/SrvGroupAreas",
            optionalText: "-- SELECT ALL --"
        },
        {
            name: "ProblemService",
            url: "wh.api/combo/ProblemService",
            optionalText: "-- SELECT ALL --"
        },
        {
            name: "Model",
            url: "wh.api/combo/ModelService",
            optionalText: "-- SELECT ALL --"
        }
    ]);

    widget.render(function () {
        var filter = {
            DateFrom: new Date(moment(moment().format('YYYY-MM-') + '01')),
            DateTo: new Date(),
        }
        widget.populate(filter);
        $('#Dealer, #Outlet').attr('disabled', 'disabled');
        $("[name=Area]").on("change", function () {
            widget.select({ selector: "[name=Dealer]", url: "wh.api/combo/SrvDealerList", params: { LinkedModule: "mp", GroupArea: $("[name=Area]").val() }, optionalText: "-- SELECT ALL --" });
            $("[name=Dealer]").prop("selectedIndex", 0);
            $("[name=Dealer]").change();
        });
        $("[name=Dealer]").on("change", function () {
            widget.select({ selector: "[name=Outlet]", url: "wh.api/combo/SrvBranchList", params: { area: $("#pnlFilter [name=Area]").val(), comp: $("#pnlFilter [name=Dealer]").val() }, optionalText: "-- SELECT ALL --" });
            $("[name=Outlet]").prop("selectedIndex", 0);
            $("[name=Outlet]").change();
        });

        $('select').on('change', ResetCombo);

        $('[name="Area"]').on('change', function () {
            $('[name="Outlet"]').html('<option value="">-- SELECT ALL --</option>');

        });
    });

    function ResetCombo() {
        if ($('#Area').val() == "") {
            $('#Dealer').val('');
            $('[name="Dealer"]').html('<option value="">-- SELECT ALL --</option>');
            $('#Dealer').attr('disabled', 'disabled');
            $('#Outlet').attr('disabled', 'disabled');
        }
        else {
            $('#Dealer').removeAttr('disabled', 'disabled');
        }

        if ($('#Dealer').val() == "") {
            $('#Outlet').attr('disabled', 'disabled');
        }
        else {
            $('#Outlet').removeAttr('disabled', 'disabled');
        }
    }

    function refreshGrid() {
        var params = $("#pnlFilter").serializeObject();

        widget.kgrid({
            url: "wh.api/VOR/VORTable",
            name: "VORTable",
            params: params,
            serverBinding: true,
            filterable: true,
            pageSize: 10,
            columns: [
                //{ field: "Cancel", title: " "/*, title: "<input type='checkbox' id='chkSelectAll' style='margin-bottom: 0px;'/>"*/, template: "<i class='icon fa fa-check-square'>", width: '50px', sortable: false, filterable: false, hidden: true },
                { field: "TicketNo", width: 120, title: "Ticket No" },
                { field: "DealerCode", width: 120, title: "Kode Dealer" },
                { field: "DealerName", width: 120, title: "Nama Dealer" },
                { field: "SPKDate", width: 120, title: "Tanggal Laporan", template: "#= (SPKDate == undefined) ? '' : moment(SPKDate).format('DD-MMM-YYYY') #" },
                { field: "SPKNo", width: 120, title: "No Atap / No. Urut Service" },
                { field: "PoliceRegNo", width: 120, title: "No. Polisi" },
                { field: "BasicModel", width: 100, title: "Model" },
                { field: "Customer", width: 150, title: "Nama Pelanggan" },
                { field: "SA", width: 150, title: "SA" },
                { field: "FM", width: 150, title: "Foreman" },
                { field: "Mech", width: 150, title: "Teknisi" },
                { field: "CreatedDate", width: 120, title: "Tanggal Masuk", template: "#= (CreatedDate == undefined) ? '' : moment(CreatedDate).format('DD-MMM-YYYY') #" },
                { field: "CreatedDate", width: 100, title: "Jam Masuk", template: "#= (CreatedDate == undefined) ? '' : moment(CreatedDate).format('HH:mm') #" },
                { field: "Job", width: 150, title: "Jenis Pekerjaan" },
                { field: "ServiceRequestDesc", width: 200, title: "Uraian Pekerjaan" },
                { field: "JobDelayDesc", width: 220, title: "Penyebab pekerjaan tunda" },
                { field: "PartNo", width: 150, title: "No. Suku Cadang" },
                { field: "PartName", width: 180, title: "Nama Suku Cadang" },
                { field: "JobReasonDesc", width: 200, title: "Alasan  Penyebab Tertunda" },
                { field: "WIP", width: 80, title: "WIP" },
                { field: "ClosedDate", width: 200, title: "Tanggal Selesai / Close", template: "#= (ClosedDate == undefined) ? '' : moment(ClosedDate).format('DD-MMM-YYYY') #" },
                { field: "StatusVOR", width: 80, title: "Cancel VOR" },
            ],
            onChange: function () {

            }
        });
    }

    function cancelVOR() {
        var grid = $("#VORTable").data("kendoGrid");
        var trs = grid.tbody.find('tr.k-state-selected');
        var datas = [];
        if (trs.length < 1) {
            sdms.info("Silahkan pilih data VOR", "Error");
            return;
        }
        $.each(trs, function (i, row) {
            var data = grid.dataItem(row);
            datas.push({
                GNDealerCode: data.GNDealerCode,
                GNOutletCode: data.GNOutletCode,
                ServiceNo: data.ServiceNo
            });
        });

        setTimeout(function () {

            $.ajax({
                url: "wh.api/VOR/CancelVOR",
                type: "POST",
                data: JSON.stringify({ 'VORs': datas }),
                dataType: "json",
                contentType: "application/json",
                success: function () {
                    $('#saveChanges').hide();
                    $('#cancelSave').hide();
                    $('#cancelVOR').show();
                    $('#refresh').show();
                    $('#print').show();
                    $('#printsumm').show();
                    refreshGrid();
                }
            });
        }, 200);
    }

    function exportXls() {
        var params = $("#pnlFilter").serializeObject();

        $.ajax({
            type: "POST",
            data: params,
            url: "wh.api/report/VORReport",
            success: function (data) {
                if (data.message == "") {
                    location.href = 'wh.api/report/DownloadExcelFile?key=' + data.value + '&filename=VOR Report';
                } else {
                    sdms.info(data.message, "Error");
                }
            }
        });
    }

    function exportSummaryXls() {
        var params = $("#pnlFilter").serializeObject();

        $.ajax({
            type: "POST",
            data: params,
            url: "wh.api/report/VORSummaryReport",
            success: function (data) {
                if (data.message == "") {
                    location.href = 'wh.api/report/DownloadExcelFile?key=' + data.value + '&filename=VOR Summary Report';
                } else {
                    sdms.info(data.message, "Error");
                }
            }
        });
    }
});