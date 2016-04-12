$(document).ready(function () {
    var widget = new SimDms.Widget({
        title: "Inquiry - By Periode",
        xtype: "panels",
        panels: [
            {
                name: "pnlFilter",
                items: [
                    {
                        text: "Date (From - To)",
                        type: "controls", items: [
                            { name: "DateFrom", text: "Date From", cls: "span2", type: "kdatepicker" },
                            { name: "DateTo", text: "Date To", cls: "span2", type: "kdatepicker" },
                        ]
                    },
                    {
                        text: "Outlet",
                        type: "controls", items: [
                            { name: "OutletCode", text: "NIK", cls: "span2", readonly: true, type: "hidden" },
                            { name: "OutletName", text: "Name", cls: "span4", readonly: true },
                            { name: "CompanyCode", text: "Name", cls: "span2", readonly: true, type: "hidden" },
                            { name: "BranchCode", text: "Name", cls: "span2", readonly: true, type: "hidden" },
                            { name: "Position", text: "Name", cls: "span2", readonly: true, type: "hidden" },
                            { name: "coo", text: "coo", cls: "span2", readonly: true, type: "hidden" },
                            { name: "bytype", text: "byType", cls: "span2", readonly: true, type: "hidden" },
                            { name: "bysales", text: "bySales", cls: "span2", readonly: true, type: "hidden" },
                            { name: "bysource", text: "bySource", cls: "span2", readonly: true, type: "hidden" },
                        ]
                    },
                    { name: "NikSH", text: "Sales Head", cls: "span6", type: "select" },
                    { name: "NikSC", text: "Sales Koordinator", cls: "span6", type: "select" },
                    { name: "NikSL", text: "Salesman", cls: "span6", type: "select" },
                ],
            },
            {
                title: "Inquiry by Periode",
                xtype: "kgrid",
                name: "tabInqByType"
            },
        ],
        toolbars: [
            { name: "btnRefresh", text: "Query", icon: "icon-refresh" },
        ]

    });
    widget.render(init);

    $('#btnRefresh').click(function () {
        //widget.reloadGridData("tabInqBySales");
        SummaryByType();
    });

    function SummaryByType() {
        var params = {
            companyCode: $("[name=CompanyCode]").val(),
            branchCode: $("[name=BranchCode]").val(),
            periodeBegin: getSqlDate($("[name=DateFrom]").val()),
            periodeEnd: getSqlDate($("[name=DateTo]").val()),
            loginAs: $("[name=Position]").val(),            
            sh: $("#NikSH").val(),
            sc: $('#NikSC').val(),
            sales: $('#NikSL').val(),
            coo: "",
            tableType: "byType"
        }

        widget.kgrid({
            url: "its.api/inquiryits/InqPeriode",
            name: "tabInqByType",
            params: params,
            columns: [
                    { field: "InquiryNumber", sTitle: "No", sWidth: "100px" },
                    { field: "Pelanggan", sTitle: "Pelanggan", sWidth: "250px" },
                    { field: "InquiryDate", sTitle: "Tgl. KDP", sWidth: 120, template: "#= (InquiryDate == undefined) ? '' : moment(InquiryDate).format('DD MMM YYYY') #" },
                    { field: "TipeKendaraan", sTitle: "Tipe", sWidth: "100px" },
                    { field: "Variant", sTitle: "Varian", sWidth: "100px" },
                    { field: "Transmisi", sTitle: "AT/MT", sWidth: "100px" },
                    { field: "Warna", sTitle: "Warna", sWidth: "100px" },
                    { field: "PerolehanData", sTitle: "Perolehan Data", sWidth: "100px" },
                    { field: "Employee", sTitle: "Wiraniaga", sWidth: "100px" },
                    { field: "Supervisor", sTitle: "Koordinator", sWidth: "100px" },
                    { field: "NextFollowUpDate", sTitle: "Next Followup Date", sWidth: 120, template: "#= (InquiryDate == undefined) ? '' : moment(InquiryDate).format('DD MMM YYYY') #" },
                    { field: "LastProgress", sTitle: "Last Progress", sWidth: "100px" },
                    { field: "ActivityDetail", sTitle: "Follow Up Detail", sWidth: "100px" },
            ],
        });
    }

    function init() {
        widget.post("its.api/inquiryits/default", function (result) {
            if (result.success) {
                widget.default = result.data;
                widget.select({ name: "NikSL", data: result.data.EmpSLList });
                widget.select({ name: "NikSC", data: result.data.EmpSCList, optionText: "-- ALL SC --", optionValue: "--" });
                widget.select({ name: "NikSH", data: result.data.EmpSHList, optionText: "-- ALL SH --", optionValue: "--" });
                if (result.data.Position == "S") widget.enable({ value: false, items: ["NikSL", "NikSC", "NikSH"] });
                if (result.data.Position == "SC") widget.enable({ value: false, items: ["NikSC", "NikSH"] });
                if (result.data.Position == "SH") {
                    widget.enable({ value: false, items: ["NikSH"] });
                    widget.selectparam({ name: "NikSL", url: "its.api/combo/employee", param: "NikSC", optionText: "-- ALL SALESMAN --" });
                }
                widget.populate(widget.default);
            }
            else {
                widget.alert(result.message || "User belum terdaftar di Master Position !");
                widget.showToolbars([]);
            }
        });
    };

    function getSqlDate(value) {
        return moment(value, "DD-MMM-YYYY").format("YYYYMMDD");
    }
});