$(document).ready(function () {
    var options = {
        title: "Laporan Custsomer Aktif/Pasif",
        xtype: "panels",
        toolbars: [
            { name: "btnProcess", text: "Print", icon: "icon-print" },
        ],
        panels: [
            {
                title: "Laporan Custsomer Aktif/Pasif",
                name: "perincianintensif",
                items: [
                    { name: "Year", required: true, cls: "span4 full", text: "Periode tahun" },
                    { name: "rbActive", cls: "span2", type: "switch", text: "Aktif" },
                    { name: "rbPasive", cls: "span2", type: "switch", text: "Pasif" },
                ],
            },
        ]
    }

    var widget = new SimDms.Widget(options);
    widget.render(function () {
        var date1 = new Date();
        var year = date1.getFullYear();
        var data = {
            Year: year,
            rbActive: true,
            rbPasive: true
        };
        widget.populate(data);
    });

    $('#btnProcess').on('click', function (e) {
        var data = $(".main .gl-widget").serializeObject();
        var year = data.Year;
        var status = (data.rbActive && data.rbPasive) ? "0" : data.rbActive ? "1" : "2";
        widget.XlsxReport({
            url: 'sv.api/report/GnRpMst004',
            type: 'xlsx',
            params: {
                year: year,
                status: status
            }
        });
    });
});