$(document).ready(function () {
    var widget = new SimDms.Widget({
        title: "Inquiry - Outstanding Prospek",
        xtype: "panels",
        panels: [
            {
                name: "pnlFilter",
                items: [
                    {
                        text: "Periode", name: "DateTo", text: "Periode", cls: "span3", type: "kdatepicker" 
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
                    {
                        text: "Branch Manger (BM)",
                        type: "controls", items: [
                            { name: "NikBM", text: "NIK", cls: "span4", readonly: true, type: "select" },
                        ]
                    },
                    {
                        text: "Sales Head (SH)",
                        type: "controls", items: [
                            { name: "NikSH", text: "NIK", cls: "span4", readonly: true, type: "select" },
                        ]
                    },
                    {
                        text: "Sales Coordinator (SC)",
                        type: "controls", items: [
                            { name: "NikSC", text: "NIK", cls: "span4", readonly: true, type: "select" },
                        ]
                    },
                    {
                        text: "Salesman (S)",
                        type: "controls", items: [
                            { name: "Nik", text: "NIK", cls: "span4", readonly: true, type: "select" },
                        ]
                    },
                ],
            },
            {
                xtype: "tabs",
                name: "tabInq",
                items: [
                    { name: "bySales", text: "Bedasarkan Salesman" },
                    { name: "byType", text: "Bedasarkan Tipe Kendaraan" },
                    { name: "bySource", text: "Bedasarkan Sumber data" },
                ]
            },
            {
                title: "Outstanding Prospect By Sales",
                cls: "bySales",
                xtype: "kgrid",
                name: "tabInqBySales",
                tabName: "tabInq",
                pnlname: "pnlBySales",
            },
            {
                title: "Outstanding Prospect By Type",
                cls: "byType",
                xtype: "kgrid",
                tabName: "tabInq",
                name: "tabInqByType",
                pnlname: "pnlByType",
            },
            {
                title: "Outstanding Prospect By Source",
                cls: "bySource",
                xtype: "kgrid",
                tabName: "tabInq",
                name: "tabInqBySource",
                pnlname: "pnlbySource",
            },
        ],
        toolbars: [
            { name: "btnRefresh", text: "Query", icon: "icon-refresh" }
        ]
    });

    widget.render(init);
    function init() {
        $("#pnlList").css({ width: 1400, height: 400 });
        widget.post("its.api/inquiryits/default", function (result) {
            if (result.success) {
                widget.default = result.data;
                widget.select({ name: "Nik", data: result.data.EmpSLList, optionText: "-- ALL SALES --", optionValue: "--" });
                widget.select({ name: "NikSC", data: result.data.EmpSCList, optionText: "-- ALL SC --", optionValue: "--" });
                widget.select({ name: "NikSH", data: result.data.EmpSHList, optionText: "-- ALL SH --", optionValue: "--" });
                widget.select({ name: "NikBM", data: result.data.EmpBMList, optionText: "-- ALL BM --", optionValue: "--" });
                if (result.data.Position == "S") {
                    widget.enable({ value: false, items: ["Nik", "NikSC", "NikSH","NikBM"] });
                    $('#Nik').val(result.data.EmpSLList[0].value);
                }
                if (result.data.Position == "SC") { widget.enable({ value: false, items: ["NikSC", "NikSH", "NikBM"] }); }
                if (result.data.Position == "SH") {
                    widget.enable({ value: false, items: ["NikSH", "NikBM"] });
                    widget.selectparam({ name: "Nik", url: "its.api/combo/employee", param: "NikSC", optionText: "-- ALL SALESMAN --" });
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

            $('[name=bytype]').val('bytype');
            $('[name=bysales]').val('bysales');
            $('[name=bysource]').val('bysource');

        });
    }

    $('#btnRefresh').click(function () {
        var params = {
            companyCode: $("[name=CompanyCode]").val(),
            branchCode: $("[name=BranchCode]").val(),
            periode: getSqlDate($("[name=DateTo]").val()),
            loginAs: $("[name=Position]").val(),
            bm: $('#NikBM').val(),
            sh: $("#NikSH").val(),
            sc: $('#NikSC').val(),
            sales: $('#Nik').val(),
            coo: "",
            tableType: ""
        }

        OutStandingByType(params, "byType");
        OutStandingBySource(params, "bySource");
        OutStandingBySales(params, "bySales");
    });

    function OutStandingByType(params, tableType) {
        params.tableType = tableType;

        widget.kgrid({
            url: "its.api/inquiryits/OutstandingProspek",
            name: "tabInqByType",
            params: params,
            columns: [
                    { field: "TipeKendaraan", sTitle: "Tipe", sWidth: "100px" },
                    { field: "PROSPECT", sTitle: "P", sWidth: "250px" },
                    { field: "HOTPROSPECT", sTitle: "HP", sWidth: "100px" },
                    { field: "SPK", sTitle: "SPK", sWidth: "100px" },
            ],
        });
    }

    function OutStandingBySource(params, tableType) {
        params.tableType = tableType;

        widget.kgrid({
            url: "its.api/inquiryits/OutstandingProspek",
            name: "tabInqBySource",
            params: params,
            columns: [
                    { field: "Source", sTitle: "Source", sWidth: "100px" },
                    { field: "PROSPECT", sTitle: "P", sWidth: "250px" },
                    { field: "HOTPROSPECT", sTitle: "HP", sWidth: "100px" },
                    { field: "SPK", sTitle: "SPK", sWidth: "100px" },
            ],
        });
    }
    
    function OutStandingBySales(params, tableType) {
        params.tableType = tableType;

        var params = {
            companyCode: $("[name=CompanyCode]").val(),
            branchCode: $("[name=BranchCode]").val(),
            periode: getSqlDate($("[name=DateTo]").val()),
            loginAs: $("[name=Position]").val(),
            bm: $('#NikBM').val(),
            sh: $("#NikSH").val(),
            sc: $('#NikSC').val(),
            sales: $('#Nik').val(),
            coo: "",
            tableType: "bySales"
        }

        widget.kgrid({
            url: "its.api/inquiryits/OutstandingProspek",
            name: "tabInqBySales",
            params: params,
            columns: [
                    { field: "PositionName", sTitle: "Posisi", sWidth: "100px" },
                    { field: "EmployeeName", sTitle: "Employee Name", sWidth: "250px" },
                    { field: "PROSPECT", sTitle: "P", sWidth: "100px" },
                    { field: "HOTPROSPECT", sTitle: "HP", sWidth: "100px" },
                    { field: "SPK", sTitle: "SPK", sWidth: "100px" },
            ],
        });
    }
    
    function getSqlDate(value) {
        return moment(value, "DD-MMM-YYYY").format("YYYYMMDD");
    }
});