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
                        text: "Outlet",
                        type: "controls", items: [
                            { name: "OutletCode", text: "NIK", cls: "span2", readonly: true },
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
                cls: "tabSum bySales",
                xtype: "kgrid",
                name: "tabInqBySales",
                pnlname: "pnlBySales",
            },
            {
                title: "Outstanding Prospect By Type",
                cls: "tabSum byType",
                xtype: "kgrid",
                name: "tabInqByType",
                pnlname: "pnlByType",
            },
            {
                title: "Outstanding Prospect By Source",
                cls: "tabSum bySource",
                xtype: "kgrid",
                name: "tabInqBySource",
                pnlname: "pnlbySource",
            },
        ],
        toolbars: [
            { name: "btnRefresh", text: "Refresh", icon: "icon-refresh" },
            { name: "btnExportXls", text: "Export (xls)", icon: "icon-xls" },
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
                    console.log(result.data.EmpSLList[0]);
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
        OutStandingByType();
        OutStandingBySource();
        OutStandingBySales();
        //widget.reloadGridData("tabInqBySales");
        //widget.reloadGridData("tabInqByType");
        //widget.reloadGridData("tabInqBySource");
    });

    function OutStandingByType() {
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
            tableType: "byType"
        }

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

    function OutStandingBySource() {
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
            tableType: "bySource"
        }

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
    
    function OutStandingBySales() {
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

    //$('#btnExportXls').click(function () {

    //    widget.showReport({
    //        id: "PmRpInqFollowUpDet",
    //        par: [$('#CompanyCode').val(), $('#BranchCode').val(), getSqlDate($("input[name='DateFrom']").val()), getSqlDate($("input[name='DateTo']").val()), $('#OutletCode').val(), $('#NikSC').val(),
    //                $('#Nik').val(), "", $('#NikSH').val()],
    //        panel: "pnlList",
    //        type: "rdlc"
    //    });

    //    widget.exportXls({
    //        name: "pnlList",
    //        type: "kgrid",
    //        items: [
    //            { field: "InquiryNumber", title: "No. Inquiry", width: 100 },
    //            { field: "Pelanggan", title: "Pelanggan", width: 280 },
    //            { field: "InquiryDate", title: "Tgl KDP", width: 120, type: "date" },
    //            { field: "TipeKendaraan", title: "Tipe", width: 120 },
    //            { field: "Variant", title: "Varian", width: 200 },
    //            { field: "Transmisi", title: "AT/MT", width: 180 },
    //            { field: "Warna", title: "Warna", width: 200 },
    //            { field: "PerolehanData", title: "Perolehan Data", width: 120 },
    //            { field: "Employee", title: "Wiraniaga", width: 300 },
    //            { field: "Supervisor", title: "Koordinator", width: 300 },
    //            { field: "NextFollowUpDate", title: "Next Follow Up", width: 300 },
    //            { field: "LastProgress", title: "Last Progress", width: 300 },
    //            { field: "LastCaseCategory", title: "Follow Up Detail", width: 300 },
    //        ]
    //    });
    //});


    function getSqlDate(value) {
        return moment(value, "DD-MMM-YYYY").format("YYYYMMDD");
    }
});