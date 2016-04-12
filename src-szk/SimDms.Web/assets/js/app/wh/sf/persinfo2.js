$(document).ready(function () {
    var options = {
        title: "Personal Information",
        xtype: "panels",
        panels: [
            {
                name: "pnlFilter",
                items: [
                    {
                        text: "Area",
                        type: "controls",
                        items: [
                            { name: "GroupArea", text: "Area", cls: "span6", type: "select", opt_text: "-- SELECT ALL --" },
                        ]
                    },
                    {
                        text: "Dealer Name",
                        type: "controls",
                        items: [
                            { name: "CompanyCode", cls: "span6", type: "select", opt_text: "-- SELECT ALL --" },
                        ]
                    },
                    {
                        text: "Outlet Name",
                        type: "controls",
                        items: [
                            { name: "BranchCode", cls: "span6", type: "select", opt_text: "-- SELECT ALL --" },
                        ]
                    },
                    {
                        text: "Position",
                        type: "controls",
                        items: [
                            { name: "Position", text: "Position", cls: "span4", type: "select", opt_text: "-- SELECT ALL --" },
                            {
                                name: "Status", text: "Status", cls: "span2", type: "select", opt_text: "ALL STATUS",
                                items: [
                                    { text: "AKTIF", value: "1" },
                                    { text: "NON AKTIF", value: "2" },
                                    { text: "KELUAR", value: "3" },
                                    { text: "PENSIUN", value: "4" },
                                ]
                            },
                        ]
                    },
                ],
            },
            { name: "SfmPersInfo", xtype: "k-grid" },
        ],
        toolbars: [
            { name: "btnRefresh", action: 'refresh', text: "Refresh", icon: "fa fa-refresh" },
            { action: 'expand', text: 'Expand', icon: 'fa fa-expand' },
            { action: 'collapse', text: 'Collapse', icon: 'fa fa-compress', cls: 'hide' },
            { name: "btnExportXls", action: 'export', text: "Export (xls)", icon: "fa fa-file-excel-o" },
        ],
        onToolbarClick: function (action) {
            console.log(action);
            switch (action) {
                case 'collapse':
                    widget.exitFullWindow();
                    widget.showToolbars(['refresh', 'export', 'expand']);
                    break;
                case 'expand':
                    widget.requestFullWindow();
                    widget.showToolbars(['refresh', 'export', 'collapse']);
                    break;
                case 'export':
                    exportXls();
                    break;
                default:
                    break;
            }
        },
    }
    var widget = new SimDms.Widget(options);
    widget.setSelect([{ name: "GroupArea", url: "wh.api/combo/GroupAreas", optionalText: "-- SELECT ALL --" }]);
    // widget.setSelect([{ name: "CompanyCode", url: "wh.api/combo/DealerList?LinkedModule=mp", optionalText: "-- SELECT ONE --" }]);
    // widget.default = { Status: "1", CompanyCode: "6021406" };
    widget.default = { Status: "1", CompanyCode: "" };

    widget.render(function () {
        widget.populate(widget.default);
        $("#CompanyCode").prop('disabled', true);
        $("[name=GroupArea]").on("change", function () {
            var groupArea = $("[name=GroupArea]").val();
            if(groupArea == '' || groupArea == undefined){
                $("#CompanyCode").prop('disabled', true);
            }
            else{
                $("#CompanyCode").prop('disabled', false);
            }

            widget.select({ selector: "[name=CompanyCode]", url: "wh.api/combo/ComboDealerList", params: { LinkedModule: "mp", GroupArea: groupArea }, optionalText: "-- SELECT ALL --" });
            $("[name=CompanyCode]").prop("selectedIndex", 0);
            $("[name=CompanyCode]").change();
        });
        $("[name=CompanyCode]").on("change", function () {
            widget.select({ selector: "[name=BranchCode]", url: "wh.api/combo/ComboOutletList", params: { CompanyCode: $("#pnlFilter [name=CompanyCode]").val() }, optionalText: "-- SELECT ALL --" });
            widget.select({ selector: "[name=Position]", url: "wh.api/combo/positions", params: { comp: $("#pnlFilter [name=CompanyCode]").val(), dept: "SALES" }, optionalText: "-- SELECT ALL --" });
            $("[name=BranchCode]").prop("selectedIndex", 0);
            $("[name=Position]").prop("selectedIndex", 0);
            $("[name=BranchCode]").change();
        });
        $("#btnRefresh").off().on("click", refreshGrid);
        //$("#btnExportXls").on("click", exportXls);
        // $("select[name=Position],select[name=Status], select[name=BranchCode]").on("change", refreshGrid);
        // $("#pnlFilter select").on("change", refreshGrid);
        // setTimeout(function () { $("#pnlFilter [name=CompanyCode]").change() }, 500);
    });

    function refreshGrid() {
        var params = {
            GroupArea: $("[name=GroupArea]").val(),
            CompanyCode: $("[name=CompanyCode]").val(),
            Branch: $("[name=BranchCode]").val(),
            Position: $("[name=Position]").val(),
            Status: $("[name=Status]").val(),
        }
        widget.kgrid({
            url: "wh.api/inquiry/sfmpersinfoNew",
            name: "SfmPersInfo",
            params: params,
            serverBinding: true,
            pageSize: 10,
            columns: [
                { field: "BranchName", title: "Outlet", width: 420 },
                { field: "DeptCode", title: "Department", width: 125 },
                { field: "EmployeeID", title: "NIK", width: 125 },
                //{ field: "SalesID", title: "Sales ID", width: 100 },
                { field: "EmployeeName", title: "Nama", width: 280 },
                { field: "PosName", title: "Jabatan", width: 200 },
                { field: "Grade", title: "Grade", width: 120 },
                { field: "JobOther1", title: "Additional Job 1", width: 150 },
                { field: "JobOther2", title: "Additional Job 2", width: 150 },
                { field: "Status", title: "Status", width: 120 },
                { field: "JoinDate", title: "Join Date", width: 125, type: "date" },
                { field: "TeamLeader", title: "Team Leader", width: 400 },
                { field: "ResignDate", title: "Resign Date", width: 125, type: "date" },
                { field: "ResignDescription", title: "Resign Description", width: 225 },
                { field: "MaritalStatus", title: "Marital Status", width: 125 },
                { field: "Religion", title: "Religion", width: 110 },
                { field: "Gender", title: "Gender", width: 110 },
                { field: "Education", title: "Education", width: 110 },
                { field: "BirthPlace", title: "Birth Place", width: 150 },
                { field: "BirthDate", title: "Birth Date", width: 125, type: "date" },
                { field: "Address", title: "Address", width: 750 },
                { field: "City", title: "City", width: 320 },
                { field: "Province", title: "Province", width: 350 },
                { field: "ZipCode", title: "ZipCode", width: 90 },
                { field: "IdentityNo", title: "Identity No", width: 200 },
                { field: "NPWPNo", title: "NPWP", width: 200 },
                { field: "Email", title: "Email", width: 200 },
                { field: "Height", title: "Height", width: 100 },
                { field: "Weight", title: "Weight", width: 100 },
                { field: "UniformSize", title: "Size", width: 100 },
                { field: "UniformSizeAlt", title: "Size Alt", width: 100 },
            ],
        });
    }

    function exportXls() {
        //window.location.href = "wh.api/inquiryprod/SfmPersInfo?CompanyCode=" + $('[name="CompanyCode"]').val() + '&BranchCode=' + $('[name="BranchCode"]').val() + '&Position=' + $('[name="Position"]').val() + '&Status=' + $('[name="Status"]').val();        
        
        var params = {
            GroupArea: $("[name=GroupArea]").val(),
            CompanyCode: $("[name=CompanyCode]").val(),
            Branch: $("[name=BranchCode]").val(),
            Position: $("[name=Position]").val(),
            Status: $("[name=Status]").val(),
        }
        $.ajax({
            async: true,
            type: "POST",
            data: params,
            url: "wh.api/report/SfmPersInfoNew",
            success: function (data) {
                if (data.message == "") {
                    location.href = 'wh.api/report/DownloadExcelFile?key=' + data.value + '&filename=Personal Information Sales Forces';
                } else {
                    sdms.info(data.message, "Error");
                }
            }
        });
        /*
        sdms.report({
            url: 'wh.api/inquiry/SfmPersInfoXls',
            type: 'xlsx',
            params: widget.serializeObject('pnlFilter')
        });
        */
    }
});
