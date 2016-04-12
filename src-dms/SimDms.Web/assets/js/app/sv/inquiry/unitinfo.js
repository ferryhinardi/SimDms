$(document).ready(function () {
    var options = {
        title: "Inquiry Informasi Kendaraan",
        xtype: "panels",
        toolbars: [
            //{ name: "btnNew", text: "New", icon: "icon-file" },
            { name: 'btnLoadData', text: 'Load Data', icon: 'icon-search' },
        ],
        panels: [
            {
                name: "pnlUnitInfo",
                title: "Inquiry Informasi Kendaraan",
                items: [
                    {
                        name: "Month", text: "Periode", type: "select",cls:"span4 full",
                        items: [
                            { value: "1", text: "January" },
                            { value: "2", text: "February" },
                            { value: "3", text: "March" },
                            { value: "4", text: "April" },
                            { value: "5", text: "May" },
                            { value: "6", text: "June" },
                            { value: "7", text: "July" },
                            { value: "8", text: "August" },
                            { value: "9", text: "September" },
                            { value: "10", text: "October" },
                            { value: "11", text: "November" },
                            { value: "12", text: "December" },
                        ]
                    },
                    { name: "Year", text: "Year", cls: "span4" },
                ]
            },
            {
                name: "KGridUnitInfo",
                title: "Unit Info",
                xtype: "k-grid",
            },
            {
                name: "KGridUnitDetail",
                title: "Detail Unit",
                xtype: "k-grid",
            },
            {
                name: "KGridInvoiceDetail",
                title: "Detail Invoice",
                xtype: "k-grid",
            },
            {
                name: "KGridTaskDetail",
                title: "Detail Task",
                xtype: "k-grid",
            },
        ]
    }

    var widget = new SimDms.Widget(options);
    widget.default = {};
    widget.render(function () {
        widget.post('sv.api/unitinfo/default',  function (result) {
            widget.default = result;
            widget.populate(result);
        });
    });

    $('#btnLoadData').on('click', function (e) {
        UnitInfo();
    });

    $("#KGridUnitInfo").on("click", "table", function (e) {
        widget.selectedRow("KGridUnitInfo", function (e) {
            var param = {
                month: $('#Month').val(),
                year: $('#Year').val(),
                JobType: e.JobType
            };

            UnitDtl(param);
            InvoiceDtl(param);
            TaskDtl(param);
        });
    });

    function UnitInfo() {
        widget.kgrid({
            url: "sv.api/unitinfo/GetUnitInfo",
            name: "KGridUnitInfo",
            params: $("#pnlUnitInfo").serializeObject(),
            columns: [
                { field: "JobType", title: "Tipe Pekerjaan" },
                { field: "Unit", title: "Jumlah Unit" },
                { field: "Invoice", title: "Jumlah Invoice" },
                { field: "Task", title: "Jumlah Task" }
            ],
        });
    }

    function UnitDtl(params) {
        widget.kgrid({
            url: "sv.api/unitinfo/getunitdetail",
            name: "KGridUnitDetail",
            params: params,
            columns: [
                { field: "No", title: "No." },
                {
                    field: "JobOrderDate", title: "Tgl. Invoice", sWidth: "130px",
                    template: "#= (JobOrderDate == undefined) ? '' : moment(JobOrderDate).format('DD MMM YYYY') #"
                },
                { field: "ChassisCode", title: "Kode Rangka" },
                { field: "ChassisNo", title: "No. Rangka" }
            ],
        });
    }

    function InvoiceDtl(params) {
        widget.kgrid({
            url: "sv.api/unitinfo/getinvoicedetail",
            name: "KGridInvoiceDetail",
            params: params,
            columns: [
                { field: "No", title: "No." },
                { field: "InvoiceNo", title: "No. Invoice" },
                { field: "JobOrderNo", title: "No. SPK" },
                {
                    field: "JobOrderDate", title: "Tgl. SPK", sWidth: "130px",
                    template: "#= (JobOrderDate == undefined) ? '' : moment(JobOrderDate).format('DD MMM YYYY') #"
                },
                { field: 'ChassisCode', title: 'Kode Rangka' },
                { field: 'ChassisNo', title: 'No. Rangka' },
            ],
        });
    }

    function TaskDtl(params) {
        widget.kgrid({
            url: "sv.api/unitinfo/gettaskdetail",
            name: "KGridTaskDetail",
            params: params,
            columns: [
                { field: "No", title: "No." },
                { field: "InvoiceNo", title: "No. Invoice" },
                { field: "JobOrderNo", title: "No. SPK" },
                {
                    field: "JobOrderDate", title: "Tgl. SPK", sWidth: "130px",
                    template: "#= (JobOrderDate == undefined) ? '' : moment(JobOrderDate).format('DD MMM YYYY') #"
                },
                { field: 'OperationNo', title: 'Pekerjaan' },
            ],
        });
    }


});

