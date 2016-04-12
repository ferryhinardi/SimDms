
$(document).ready(function () {
    var options = {
        title: "Pemantauan Pekerjaan",
        xtype: "panels",
        toolbars: [
            //{ name: "btnProcess", text: "Process", icon: "icon-bolt" },
            //{ name: "btnMonitor", text: "Monitor", icon: "icon-fullscreen" }
        ],
        panels: [
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
    function alterUI(status) {
        if (status == 'N') {
            widget.clearForm();
            widget.post("sv.api/jobmonitoringscreen/default", function (result) {
                widget.default = $.extend({}, result);
                widget.populate(widget.default);
            });
        }
    }
});