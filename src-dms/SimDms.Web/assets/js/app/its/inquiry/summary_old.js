$(document).ready(function () {
    var widget = new SimDms.Widget({
        title: "Inquiry - Summary",
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
                        type: "controls", items: [
                            { name: "CompanyCode", text: "Name", cls: "span2", readonly: true, type: "hidden" },
                            { name: "BranchCode", text: "Name", cls: "span2", readonly: true, type: "hidden" },
                            { name: "Position", text: "Name", cls: "span2", readonly: true, type: "hidden" },
                            { name: "coo", text: "coo", cls: "span2", readonly: true, type: "hidden" },
                            { name: "bytype", text: "byType", cls: "span2", readonly: true, type: "hidden" },
                            { name: "bysales", text: "bySales", cls: "span2", readonly: true, type: "hidden" },
                            { name: "bysource", text: "bySource", cls: "span2", readonly: true, type: "hidden" },
                        ]
                    },
                    { name: "NikBM", text: "Branch Manager", cls: "span6", type: "select" },
                    { name: "NikSH", text: "Sales Head", cls: "span6", type: "select" },
                    { name: "NikSC", text: "Sales Koordinator", cls: "span6", type: "select" },
                    { name: "NikSL", text: "Salesman", cls: "span6", type: "select" },
                ],
            },
            {
                xtype: "tabs",
                name: "tabSum",
                items: [
                    { name: "bySales", text: "Bedasarkan Salesman" },
                    { name: "byType", text: "Bedasarkan Tipe Kendaraan" },
                    { name: "bySource", text: "Bedasarkan Sumber data" },
                ]
            },
            {
                title: "Summary By Sales",
                cls: "bySales",
                tabName:"tabSum",
                xtype: "kgrid",
                name: "tabInqBySales",
            },
            {
                title: "Summary By Type",
                cls: "byType",
                tabName: "tabSum",
                xtype: "kgrid",
                name: "tabInqByType",
            },
            {
                title: "Summary By Source",
                cls: "bySource",
                tabName: "tabSum",
                xtype: "kgrid",
                name: "tabInqBySource",
            },
        ],
        toolbars: [
            { name: "btnRefresh", text: "Query", icon: "icon-refresh" },
        ]

    });
    widget.render(init);

    $('#btnRefresh').click(function () {
        //widget.reloadGridData("tabInqBySales");
        SummaryBySales();
        SummaryBySource();
        SummaryByType();
    });

    function SummaryByType() {
        //  var params = {
        //    companyCode: $("[name=CompanyCode]").val(), branchCode: $("[name=BranchCode]").val(),
        //    periodeBegin: getSqlDate($("[name=DateFrom]").val()), periodeEnd: getSqlDate($("[name=DateTo]").val()),
        //    loginAs: $("[name=Position]").val(), bm: $('#NikBM').val(),
        //    sh: $("#NikSH").val(), sc: $('#NikSC').val(),
        //    sales: $('#NikSL').val(), coo: "", tableType: "byType"
        //}

        var params = {
            companyCode: $("[name=CompanyCode]").val(), branchCode: '6006414',
            periodeBegin: getSqlDate($("[name=DateFrom]").val()), periodeEnd: getSqlDate($("[name=DateTo]").val()),
            loginAs: '20', bm: 'BM189',
            sh: '225', sc: '50395',
            sales: 'ALL', coo: "", tableType: "byType"
        }


        widget.kgrid({
            url: "its.api/inquiryits/summary",
            name: "tabInqByType",
            params: params,
            columns: [
                    { field: "TipeKendaraan", sTitle: "Tipe", sWidth: "100px" },
                    { field: "PROSPECT", sTitle: "P", sWidth: "250px" },
                    { field: "HOTPROSPECT", sTitle: "HP", sWidth: "100px" },
                    { field: "SPK", sTitle: "SPK", sWidth: "100px" },
                    { field: "DO", sTitle: "DO", sWidth: "100px" },
                    { field: "DELIVERY", sTitle: "DELIVERY", sWidth: "100px" },
                    { field: "LOST", sTitle: "LOST", sWidth: "100px" },
            ],
        });
    }

    function SummaryBySource() {
        //var params = {
        //    companyCode: $("[name=CompanyCode]").val(), branchCode: $("[name=BranchCode]").val(),
        //    periodeBegin: getSqlDate($("[name=DateFrom]").val()), periodeEnd: getSqlDate($("[name=DateTo]").val()),
        //    loginAs: $("[name=Position]").val(), bm: $('#NikBM').val(),
        //    sh: $("#NikSH").val(), sc: $('#NikSC').val(),
        //    sales: $('#NikSL').val(), coo: "", tableType: "bySource"
        //}

        var params = {
            companyCode: $("[name=CompanyCode]").val(), branchCode: '6006414',
            periodeBegin: getSqlDate($("[name=DateFrom]").val()), periodeEnd: getSqlDate($("[name=DateTo]").val()),
            loginAs: '20', bm: 'BM189',
            sh: '225', sc: '50395',
            sales: 'ALL', coo: "", tableType: "bySource"
        }

        widget.kgrid({
            url: "its.api/inquiryits/summary",
            name: "tabInqBySource",
            params: params,
            columns: [
                    { field: "Source", sTitle: "Source", sWidth: "100px" },
                    { field: "NEW", sTitle: "P", sWidth: "250px" },
                    { field: "PROSPECT", sTitle: "P", sWidth: "250px" },
                    { field: "HOTPROSPECT", sTitle: "HP", sWidth: "100px" },
                    { field: "SPK", sTitle: "SPK", sWidth: "100px" },
                    { field: "DO", sTitle: "DO", sWidth: "100px" },
                    { field: "DELIVERY", sTitle: "DELIVERY", sWidth: "100px" },
                    { field: "LOST", sTitle: "LOST", sWidth: "100px" },
            ],
        });
    }

    function SummaryBySales() {
        //var params = {
        //    companyCode: $("[name=CompanyCode]").val(), branchCode: $("[name=BranchCode]").val(),
        //    periodeBegin: getSqlDate($("[name=DateFrom]").val()), periodeEnd: getSqlDate($("[name=DateTo]").val()),
        //    loginAs: $("[name=Position]").val(), bm: $('#NikBM').val(),
        //    sh: $("#NikSH").val(), sc: $('#NikSC').val(),
        //    sales: $('#NikSL').val(), coo: "", tableType: "bySales"
        //}

        var params = {
            companyCode: $("[name=CompanyCode]").val(), branchCode: '6006414',
            periodeBegin: getSqlDate($("[name=DateFrom]").val()), periodeEnd: getSqlDate($("[name=DateTo]").val()),
            loginAs: '20', bm: 'BM189',
            sh: '225', sc: '50395',
            sales: 'ALL', coo: "", tableType: "bySales"
        }

        widget.kgrid({
            url: "its.api/inquiryits/summary",
            name: "tabInqBySales",
            params: params,
            columns: [
                    { field: "PositionName", sTitle: "Posisi", sWidth: "100px" },
                    { field: "EmployeeName", sTitle: "Employee Name", sWidth: "250px" },
                    { field: "PROSPECT", sTitle: "P", sWidth: "100px" },
                    { field: "HOTPROSPECT", sTitle: "HP", sWidth: "100px" },
                    { field: "SPK", sTitle: "SPK", sWidth: "100px" },
                    { field: "DO", sTitle: "DO", sWidth: "100px" },
                    { field: "DELIVERY", sTitle: "DELIVERY", sWidth: "100px" },
                    { field: "LOST", sTitle: "LOST", sWidth: "100px" },
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
                widget.select({ name: "NikBM", data: result.data.EmpBMList, optionText: "-- ALL BM --", optionValue: "--" });
                if (result.data.Position == "S") widget.enable({ value: false, items: ["NikSL", "NikSC", "NikSH", "NikBM"] });
                if (result.data.Position == "SC") widget.enable({ value: false, items: ["NikSC", "NikSH", "NikBM"] });
                if (result.data.Position == "SH") {
                    widget.enable({ value: false, items: ["NikSH", "NikBM"] });
                    widget.selectparam({ name: "NikSL", url: "its.api/combo/employee", param: "NikSC", optionText: "-- ALL SALESMAN --" });
                }
                if (result.data.Position == "BM") {
                    widget.enable({ value: false, items: ["NikBM"] });
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