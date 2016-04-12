
$(document).ready(function () {
    var options = {
        title: "Pemantauan Pekerjaan",
        xtype: "panels",
        toolbars: [
            { name: "btnProcess", text: "Process", icon: "icon-bolt" },
            { name: "btnMonitor", text: "Monitor", icon: "icon-fullscreen" }
        ],
        panels: [
            {
                name: "pnlDataSelection",
                title: "Seleksi Data",
                items: [
                    { name: "ProcessDate", text: "Tgl.", cls: "span3 full", type: "datepicker" },
                    {
                        text: "Filter",
                        type: "controls",
                        items: [
                            { name: "IsFilter", type: "switch", cls: "span1" },
                            {
                                name: "FilterSelection",
                                type: "select",
                                cls: "span3",
                                readonly: true,
                                items: [
                                    { value: 'JobOrderNo', text: 'No. SPK' },
                                    { value: 'PoliceRegNo', text: 'No. Polisi' },
                                    { value: 'JobType', text: 'Kategori Pekerjaan' },
                                    { value: "ForemanID", text: 'SA' },
                                    { value: "MechanicID", text: 'Foreman' },
                                    { value: "ServiceStatus", text: 'Status Perbaikan' }
                                ]
                            },
                        ]
                    },
                    { name: "FilterText", cls: "span4 full", type: "popup", readonly: true },
                    { name: "Outstanding", type: "switch", text: "Outstanding SPK", cls: "span3 full" },
                    { name: "RTN", type: "switch", text: "Perawatan Berkala", cls: "span3" },
                    { name: "PDI", type: "switch", text: "Pre Delivery Inspection", cls: "span3" },
                    { name: "OTH", type: "switch", text: "Perawatan Lain-lain", cls: "span3" },
                    { name: "FSC", type: "switch", text: "Free Service Coupon", cls: "span3" },
                    { name: "BDR", type: "switch", text: "Body Repair", cls: "span3" },
                    { name: "CLM", type: "switch", text: "Warranty Claim", cls: "span3" },
                ]
            },
            {
                title: "Status Kendaraan",
                name: "pnlVehicleStatus",
                xtype: "table",
                tblname: "tblVehicleStatus",
                columns: [
                    { name: "No", text: "No", width: 30 },
                    { name: "JobOrderNo", text: "No SPK", width: 100 },
                    { name: "PoliceRegNo", text: "No Polisi", width: 100 },
                    { name: "BasicModel", text: "Tipe Unit", width: 100 },
                    { name: "JobType", text: "Kategori Pekerjaan", width: 100 },
                    { name: "EstimateFinishDate", text: "(Estimasi) Tanggal", type: "date", width: 90 },
                    { name: "EstimateFinishTime", text: "(Estimasi) Jam", type: "time", width: 90 },
                    { name: "ForemanID", text: "SA", width: 150 },
                    { name: "MechanicID", text: "Foreman", width: 150 },
                    { name: "Description", text: "Status Perbaikan", width: 150 },
                ]
            },
        ]
    }
    widget = new SimDms.Widget(options);
    widget.default = {};
    widget.render(function () { alterUI("N"); });
    widget.lookup.onDblClick(function (e, data, name) {
        widget.lookup.hide();
        switch (name) {
            case "JobOrderList":
                getJobOrder(data);
                break;
            default:
                break;
        }
    });
    $('[name=IsFilter]').on('click', function () {
        if ($('[name=IsFilter]').val() == 'false') {
            $('#FilterSelection').removeAttr('disabled');
            $('#btnFilterText').removeAttr('disabled');
        } else {
            $('#FilterSelection').attr('disabled', 'disabled');
            $('#btnFilterText').attr('disabled', 'disabled');
        }
    });
    $('#btnFilterText').click(lookup);
    $('#btnProcess').click(process);
    $('#btnMonitor').click(monitor);
    function lookup() {
        widget.lookup.init({
            name: "JobOrderList",
            title: "Job Orders",
            source: "sv.api/grid/joborders", //
            sortings: [[0, "asc"]],
            columns: [
                { mData: "JobOrderNo", sTitle: "No. SPK", sWidth: "150px" },
                { mData: "PoliceRegNo", sTitle: "No. Polisi", sWidth: "150px" },
                { mData: "JobType", sTitle: "Kategori Pekerjaan", sWidth: "150px" },
                { mData: "ForemanID", sTitle: "SA", sWidth: "150px" },
                { mData: "MechanicID", sTitle: "Foreman", sWidth: "150px" },
                { mData: "ServiceStatus", sTitle: "Status Perbaikan", sWidth: "150px" }
            ]
        });
        widget.lookup.show();
    }
    function getJobOrder(data) {
        var selectedFilter = $('#FilterSelection').val();
        var result = '';
        switch(selectedFilter)
        {
            case 'JobOrderNo': result = data.JobOrderNo; break;
            case 'PoliceRegNo': result = data.PoliceRegNo; break;
            case 'JobType': result = data.JobType; break;
            case 'ForemanID': result = data.ForemanID; break;
            case 'MechanicID': result = data.MechanicID; break;
            case 'ServiceStatus': result = data.ServiceStatusDesc; break;
        }
        $('#FilterText').val(result);
    }
    function process() {
        var data = {
            jobOrderDate: $('[name=ProcessDate]').val(),
            showOutstdSpk: $('[name=Outstanding]').val(),
            text: $('#FilterText').val(),
            fieldName: $('#FilterSelection').val(),
            RTN: $('[name=RTN]').val() == 'true' ? 'RTN' : '',
            OTH: $('[name=OTH]').val() == 'true' ? 'OTH' : '',
            PDI: $('[name=PDI]').val() == 'true' ? 'PDI' : '',
            FSC: $('[name=FSC]').val() == 'true' ? 'FSC' : '',
            CLM: $('[name=CLM]').val() == 'true' ? 'CLM' : '',
            BDR: $('[name=BDR]').val() == 'true' ? 'BDR' : ''
        }
        $.ajax({
            type: 'POST',
            data: data,
            url: 'sv.api/jobmonitoring/processinquiry',
            success: function (result) {
                if (result.message != "") {
                    alert(result.message);
                    return;
                }
                widget.populateTable({ selector: "#tblVehicleStatus", data: result.data });
            }
        });
    }
    function monitor() {
        window.open("sv.api/inquiry/jobmonitoringscreen", "_blank" );
    }
    function alterUI(status) {
        if (status == 'N') {
            widget.clearForm();
            $('#btnFilterText').attr('disabled', 'disabled');
            $('[name="Outstanding"]').click();
            $('[name="Outstanding"]').val(true);
            $('[name=RTN]').click();
            $('[name=RTN]').val(true);
            $('[name=OTH]').click();
            $('[name=OTH]').val(true);
            $('[name=FSC]').click();
            $('[name=FSC]').val(true);
            $('#FilterSelection').val('JobOrderNo');
            $('#FilterSelection option[value=""]').remove();
            widget.post("sv.api/jobmonitoring/default", function (result) {
                widget.default = $.extend({}, result);
                widget.populate(widget.default);
                process();
            });
        }
    }
});