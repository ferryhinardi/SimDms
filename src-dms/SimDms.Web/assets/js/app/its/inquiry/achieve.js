$(document).ready(function () {
    var widget = new SimDms.Widget({
        title: "Inquiry - Sales Achievement",
        xtype: "panels",
        toolbars: [
           { name: "btnRefresh", text: "Query", icon: "icon-refresh" },
        ],
        panels: [
            {
                name: "pnlFilter",
                items: [
                    { name: "Year", text: "Tahun", cls: "span2 full", type: "text", required:"true" },
                    { name: "NikBM", text: "Branch Manager", cls: "span6", type: "select" },
                    { name: "NikSH", text: "Sales Head", cls: "span6", type: "select" },
                    { name: "NikSC", text: "Sales Koordinator", cls: "span6", type: "select" },
                    { name: "NikSL", text: "Salesman", cls: "span6", type: "select" },
                    { name: "CompanyCode", cls: "span6", type: "hidden" }
                ],
            },
            {
                xtype: "tabs",
                name: "tabAchieve",
                items: [
                    { name: "bySales", text: "Sales" },
                    { name: "bySource", text: "Source" },
                    { name: "byType", text: "Type" },
                    { name: "byProspect", text: "Prospect" },
                ]
            },
            {
                cls: "bySales",
                tabName:"tabAchieve",
                name: "pnlSales",
                title: "Salesman",
                xtype: "kgrid",
            },
            {
                cls: "bySource",
                tabName: "tabAchieve",
                name: "pnlSource",
                title: "Source Data",
                xtype: "kgrid",
            },
            {
                cls: "byType",
                tabName: "tabAchieve",
                name: "pnlType",
                title: "Sales Type",
                xtype: "kgrid",
            },
            {
                cls: "byProspect",
                tabName: "tabAchieve",
                name: "pnlProspect",
                title: "Prospect Status",
                xtype: "kgrid",
            }

        ],
    });
    widget.render(init);

    function init() {
        $(".frame").css({ top: 210 });
        $(".panel").css({ 'max-width': 1300 });

        widget.post("its.api/inquiryits/default", function (result) {
            if (result.success) {
                widget.default = result.data;
                widget.select({ name: "NikSL", data: result.data.EmpSLList });
                widget.select({ name: "NikSC", data: result.data.EmpSCList, optionText: "-- ALL SC --", optionValue: "--" });
                widget.select({ name: "NikSH", data: result.data.EmpSHList });
                widget.select({ name: "NikBM", data: result.data.EmpBMList, optionText: "-- ALL BM --", optionValue: "--" });
                if (result.data.Position == "S") widget.enable({ value: false, items: ["NikSL", "NikSC", "NikSH", "NikBM"] });
                if (result.data.Position == "SC") widget.enable({ value: false, items: ["NikSC", "NikSH", "NikBM"] });
                if (result.data.Position == "SH") {
                    widget.enable({ value: false, items: ["NikSH"] });
                    widget.selectparam({ name: "NikSL", url: "its.api/combo/employee", param: "NikSC", optionText: "-- ALL SALESMAN --" });
                }
                widget.populate(widget.default);
                if (result.data.Position == "BM") {
                    widget.enable({ value: false, items: ["NikBM"] });
                }
            }
            else {
                widget.alert(result.message || "User belum terdaftar di Master Position !");
                widget.showToolbars([]);
            }
        });
    };

    $('#btnRefresh').click(function () {
        showReport();
    });

    function showReport() {
        //AchievementSales(AchievementSourceData(AchievementSalesType(AchievementProspect)));
        var companyCode = $('[name="CompanyCode"]').val();
        var bm = $('[name="NikBM"]').val();
        var sh = $('[name="NikSH"]').val();
        var sc = $('[name="NikSC"]').val();
        var sl = $('[name="NikSL"]').val();
        var year = $('[name="Year"]').val();

        var params = { CompanyCode: companyCode, NikBM: bm, NikSH: sh, NikSC: sc, NikSL: sl, Year: year };
        //var params = { CompanyCode: "6006406", NikBM: "%", NikSH: "%", NikSC: "%", NikSL: "%", Year: year };

        AchievementSales(params);
        AchievementSourceData(params);
        AchievementProspect(params);
        AchievementSalesType(params);
    }

    function AchievementSales(params) {
        
        
        widget.kgrid({
            url: "its.api/inquiryits/AchievementSalesMan",
            name: "pnlSales",
            params: params,
            columns: [
                { field: "EmployeeName", title: "Employee Name", width: 150 },
                { field: "Jan", title: "Jan", width: 50, align:"right" },
                { field: "Feb", title: "Feb", width: 50 },
                { field: "Mar", title: "Mar", width: 50 },
                { field: "Apr", title: "Apr", width: 50 },
                { field: "May", title: "May", width: 50 },
                { field: "Jun", title: "Jun", width: 50 },
                { field: "Jul", title: "Jul", width: 50 },
                { field: "Aug", title: "Aug", width: 50 },
                { field: "Sep", title: "Sep", width: 50 },
                { field: "Oct", title: "Oct", width: 50 },
                { field: "Nov", title: "Nov", width: 50 },
                { field: "Dec", title: "Dec", width: 50 },
                { field: "Sem1", title: "Semester 1", width: 80 },
                { field: "Sem2", title: "Semester 2", width: 80 },
                { field: "Total", title: "Total", width: 80 },
            ],
        });
    }

    function AchievementSourceData(params) {
        
        widget.kgrid({
            url: "its.api/inquiryits/AchievementSourceData",
            name: "pnlSource",
            params: params,
            columns: [
                { field: "TypeOf1", title: "Employee Name", width: 150 },
                { field: "Jan", title: "Jan", width: 50 },
                { field: "Feb", title: "Feb", width: 50 },
                { field: "Mar", title: "Mar", width: 50 },
                { field: "Apr", title: "Apr", width: 50 },
                { field: "May", title: "May", width: 50 },
                { field: "Jun", title: "Jun", width: 50 },
                { field: "Jul", title: "Jul", width: 50 },
                { field: "Aug", title: "Aug", width: 50 },
                { field: "Sep", title: "Sep", width: 50 },
                { field: "Oct", title: "Oct", width: 50 },
                { field: "Nov", title: "Nov", width: 50 },
                { field: "Dec", title: "Dec", width: 50 },
                { field: "Sem1", title: "Semester 1", width: 80 },
                { field: "Sem2", title: "Semester 2", width: 80 },
                { field: "Total", title: "Total", width: 80 },
            ],
        });
    }

    function AchievementProspect(params) {
        
        widget.kgrid({
            url: "its.api/inquiryits/AchievementProspect",
            name: "pnlProspect",
            params: params,
            columns: [
                { field: "TypeOf1", title: "Prospect Type", width: 100 },
                { field: "Jan", title: "Jan", width: 50 },
                { field: "Feb", title: "Feb", width: 50 },
                { field: "Mar", title: "Mar", width: 50 },
                { field: "Apr", title: "Apr", width: 50 },
                { field: "May", title: "May", width: 50 },
                { field: "Jun", title: "Jun", width: 50 },
                { field: "Jul", title: "Jul", width: 50 },
                { field: "Aug", title: "Aug", width: 50 },
                { field: "Sep", title: "Sep", width: 50 },
                { field: "Oct", title: "Oct", width: 50 },
                { field: "Nov", title: "Nov", width: 50 },
                { field: "Dec", title: "Dec", width: 50 },
                { field: "Sem1", title: "Semester 1", width: 80 },
                { field: "Sem2", title: "Semester 2", width: 80 },
                { field: "Total", title: "Total", width: 80 },
            ],                                           
        });
    }

    function AchievementSalesType(params) {
        
        widget.kgrid({
            url: "its.api/inquiryits/AchievementSalesType",
            name: "pnlType",
            params: params,
            columns: [
                { field: "GroupCode", title: "Group Code", width: 100 },
                { field: "typeCode", title: "Type Code", width: 100 },
                { field: "Jan", title: "Jan", width: 50 },
                { field: "Feb", title: "Feb", width: 50 },
                { field: "Mar", title: "Mar", width: 50 },
                { field: "Apr", title: "Apr", width: 50 },
                { field: "May", title: "May", width: 50 },
                { field: "Jun", title: "Jun", width: 50 },
                { field: "Jul", title: "Jul", width: 50 },
                { field: "Aug", title: "Aug", width: 50 },
                { field: "Sep", title: "Sep", width: 50 },
                { field: "Oct", title: "Oct", width: 50 },
                { field: "Nov", title: "Nov", width: 50 },
                { field: "Dec", title: "Dec", width: 50 },
                { field: "Sem1", title: "Semester 1", width: 80 },
                { field: "Sem2", title: "Semester 2", width: 80 },
                { field: "Total", title: "Total", width: 80 },
            ],
        });
    }

});