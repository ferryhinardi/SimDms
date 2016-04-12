$(document).ready(function () {
    var options = {
        title: "Consolidation",
        xtype: "panels",
        panels: [
            {
                name: "pnlFilter",
                items: [
                    { name: "GroupArea", text: "Area", cls: "span4", type: "select", opt_text: "-- ALL AREA --" },
                    { name: "CompanyCode", text: "Organization", cls: "span6", type: "select", opt_text: "-- ALL DEALER --" },
                ],
            },
            {
                name: "InqPers",
                xtype: "k-grid",
            },
        ],
        toolbars: [
            { name: "btnRefresh", text: "Refresh", icon: "fa fa-refresh" },
            { name: "btnExportXls", text: "Export (xls)", icon: "fa fa-file-excel-o" },
        ],
    }
    var widget = new SimDms.Widget(options);
    widget.setSelect([{ name: "GroupArea", url: "wh.api/combo/GroupAreas", optionalText: "-- ALL AREA --" }]);
    widget.default = { Status: "1" };
    widget.render(function () {
        widget.populate(widget.default);
        $("[name=GroupArea]").on("change", function () {
            //widget.select({ selector: "[name=CompanyCode]", url: "wh.api/combo/Companies", params: { id: $("[name=GroupArea]").val() }, optionalText: "-- SELECT ALL --" });
            //$("[name=Department]").change();
            widget.select({ selector: "[name=CompanyCode]", url: "wh.api/combo/ComboDealerList", params: { GroupArea: $("#pnlFilter [name=GroupArea]").val() }, optionalText: "-- ALL DEALER --" });
            $("[name=CompanyCode]").change();
            $('#CompanyCode').select2('val', "");
        });
        //$("[name=GroupArea], [name=CompanyCode]").on("change", refreshGrid);
        $("#btnRefresh").on("click", refreshGrid);
        $("#btnExportXls").on("click", exportXls);
        $("[name=GroupArea]").change();
    });

    function refreshGrid() {
        widget.kgrid({
            url: "wh.api/inquiry/Consolidation",
            name: "InqPers",
            params: $("#pnlFilter").serializeObject(),
            columns: [
                { field: "AreaDealer", width: 200, title: "Area" },
                { field: "CompanyName", width: 250, title: "Dealer", filterable: { extra: false, operators: { string: { contains: "Contains", startswith: "Starts with", } } } },
                { field: "ActiveEmployee", width: 120, title: "Active Emnployee" },
                { field: "InvalidBranch", title: "Invalid Branch", width: 120 },
                { field: "InvalidActiveStatus", width: 120, title: "Invalid Active Status" },
                { field: "InvalidResign", width: 120, title: "Invalid Resign" },
            ],
        });
    }

    //function exportXls() {
    //    widget.post("wh.api/combo/getdealertype", function (result) {
    //        switch (result) {
    //            case "2W":
    //                widget.exportXls({
    //                    source: "wh.api/inquiry/sfmperslist?CompanyCode=" + $("[name='CompanyCode']").val(),
    //                    fileName: "personal_list",
    //                    items: [
    //                        { name: "EmployeeID", text: "NIK", type: "text" },
    //                        { name: "SalesID", text: "Sales ID", type: "text" },
    //                        { name: "SalesName", text: "Nama Sales" },
    //                        { name: "SalesFullName", text: "Nama Lengkap Sales" },
    //                        { name: "AreaCode", text: "Kode Area", type: "text" },
    //                        { name: "DealerGroup", text: "Group Dealer" },
    //                        { name: "DealerCode", text: "Kode Dealer", type: "text" },
    //                        { name: "DealerName", text: "Nama Dealer" },
    //                        { name: "OutletCode", text: "Kode Outlet", type: "text" },
    //                        { name: "OutletType", text: "Tipe Outlet", type: "text" },
    //                        { name: "OutletName", text: "Nama Outlet", width: 400 },
    //                        { name: "JobCode", text: "Kode Posisi" },
    //                        { name: "JobName", text: "Posisi" },
    //                        { name: "GradeCode", text: "Kode Grade" },
    //                        { name: "GradeName", text: "Grade" },
    //                        { name: "JoinDate", text: "Tanggal Bergabung", type: "date" },
    //                        { name: "IsAdditionalJob", text: "Mempunyai Pekerjaan Tambahan" },
    //                        { name: "AdditionalJob", text: "Pekerjaan Tambahan" },
    //                        { name: "BirthPlace", text: "Tempat Lahir" },
    //                        { name: "BirthDate", text: "Tanggal Lahir", type: "date" },
    //                        { name: "Education", text: "Pendidikan" },
    //                        { name: "Phone1", text: "Phone 1", type: "text" },
    //                        { name: "Phone2", text: "Phone 2", type: "text" },
    //                        { name: "Religion", text: "Agama" },
    //                        { name: "KtpNo", text: "KTP", type: "text", width: 100 },
    //                        { name: "Sim1", text: "Sim 1", type: "text" },
    //                        { name: "Sim2", text: "Sim 2", type: "text" },
    //                        { name: "TerminateStatus", text: "Terminate Status" },
    //                        { name: "TerminateReason", text: "Terminate Reason" },
    //                        { name: "TerminateDate", text: "Terminate Date", type: "date" },
    //                        { name: "MaritalStatus", text: "Marital Status" },
    //                        { name: "EmployeeStatus", text: "Employee Status" },
    //                        { name: "Gender", text: "Gender" },
    //                        { name: "Email", text: "Email" },
    //                        { name: "TeamLeader", text: "Team Leader ID", type: "text" },
    //                        { name: "TeamLeaderSalesID", text: "Team Leader Sales ID" },
    //                        { name: "TeamLeaderName", text: "Team Leader Name" },
    //                        { name: "Nik", text: "NIK", type: "text" },
    //                        { name: "StatusSf", text: "Status SF" },
    //                        { name: "StatusSm", text: "Status SM" },
    //                    ]
    //                });
    //                break;
    //            default:
    //                widget.exportXls({
    //                    name: "InqPers",
    //                    type: "kgrid",
    //                    fileName: "personal_list",
    //                    items: [
    //                        { field: "CompanyCode", width: 120, title: "Dealer Code" },
    //                        { field: "LastBranch", width: 120, title: "Branch Code" },
    //                        { field: "DealerName", width: 350, title: "Dealer" },
    //                        { field: "LastBranchName", width: 350, title: "Branch" },
    //                        { field: "Department", width: 250, title: "Department" },
    //                        { field: "LastPosition", width: 250, title: "Last Position" },
    //                        { field: "EmployeeID", width: 120, title: "NIK" },
    //                        { field: "EmployeeName", width: 250, title: "Name" },
    //                        { field: "TeamLeaderName", width: 250, title: "Leader Name" },
    //                        { field: "SubOrdinates", width: 80, title: "Subs" },
    //                        { field: "Status", width: 150, title: "Status" },
    //                        { field: "SubOrdinates", width: 150, title: "Subordinates" },
    //                        { field: "MutationTimes", width: 150, title: "Mutation Times" },
    //                        { field: "AchieveTimes", width: 150, title: "Achieve Times" },
    //                        { field: "LastBranch", width: 150, title: "Last Branch" },
    //                        { field: "TeamLeader", width: 250, title: "Team Leader" },
    //                        { field: "JoinDate", type: "date", width: 150, title: "Join Date" },
    //                        { field: "ResignDate", type: "date", width: 150, title: "Resign Date" },
    //                        { field: "ResignDescription", width: 250, title: "Resign Description" },
    //                        { field: "MaritalStatus", width: 150, title: "Marital Status" },
    //                        { field: "Religion", width: 150, title: "Religion" },
    //                        { field: "Gender", width: 150, title: "Gender" },
    //                        { field: "Education", width: 150, title: "Education" },
    //                        { field: "BirthPlace", width: 250, title: "Birth Place" },
    //                        { field: "BirthDate", type: "date", width: 150, title: "Birth Date" },
    //                        { field: "Address", width: 400, title: "Address" },
    //                        { field: "Province", width: 250, title: "Province" },
    //                        { field: "District", width: 250, title: "District" },
    //                        { field: "SubDistrict", width: 250, title: "Sub District" },
    //                        { field: "Village", width: 250, title: "Village" },
    //                        { field: "ZipCode", width: 150, title: "Zip Code" },
    //                        { field: "IdentityNo", width: 150, title: "Identity Number" },
    //                        { field: "NPWPNo", width: 150, title: "NPWP No" },
    //                        { field: "NPWPDate", type: "date", width: 150, title: "NPWP Date" },
    //                        { field: "Email", width: 150, title: "Email" },
    //                        { field: "Telephone1", width: 150, title: "Telephone 1" },
    //                        { field: "Telephone1", width: 150, title: "Telephone 2" },
    //                        { field: "Handphone1", width: 150, title: "Handphone 1" },
    //                        { field: "Handphone2", width: 150, title: "Handphone 2" },
    //                        { field: "Handphone3", width: 150, title: "Handphone 3" },
    //                        { field: "Handphone4", width: 150, title: "Handphone 4" },
    //                        { field: "DrivingLicense1", width: 150, title: "Driving License 1" },
    //                        { field: "DrivingLicense2", width: 150, title: "Driving License 2" },
    //                        { field: "Height", width: 150, title: "Height" },
    //                        { field: "Weight", width: 150, title: "Weight" },
    //                        { field: "BloodCode", width: 150, title: "Blood Type" },
    //                        { field: "UniformSize", width: 150, title: "Uniforms Size" },
    //                        { field: "UniformSizeAlt", width: 150, title: "Uniform Size Alt" },
    //                        { field: "ShoesSize", width: 150, title: "Shoes Size" },
    //                    ]
    //                });
    //                break;
    //        }
    //    });
    //}

    function exportXls() {
        var params = widget.serializeObject('pnlFilter');
        params.AreaName = $('[name=GroupArea] option:selected').text();
        params.CompanyName = $('[name=CompanyCode] option:selected').text();

        $('.page > .ajax-loader').show();

        $.fileDownload('doreport/Consolidation.xlsx', {
            httpMethod: "POST",
            preparingMessageHtml: "We are preparing your report, please wait...",
            //failMessageHtml: "There was a problem generating your report, please try again.",
            data: params
        }).done(function () {
            $('.page > .ajax-loader').hide();
        });
    }
});
