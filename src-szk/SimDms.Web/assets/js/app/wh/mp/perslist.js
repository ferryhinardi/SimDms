$(document).ready(function () {
    var options = {
        title: "Personal List",
        xtype: "panels",
        panels: [
            {
                name: "pnlFilter",
                items: [
                    {
                        text: "Area",
                        type: "controls",
                        items: [
                            { name: "GroupArea", text: "Area", cls: "span7", type: "select", opt_text: "-- SELECT ALL --" },
                        ]
                    },
                    {
                        text: "Dealer Name",
                        type: "controls",
                        items: [
                            { name: "CompanyCode", text: "Organization", cls: "span7", type: "select", opt_text: "-- SELECT ALL -- " },
                        ]
                    },
                    {
                        text: "Dept - Position",
                        type: "controls",
                        items: [
                            { name: "Department", text: "Department", cls: "span2", type: "select", opt_text: "-- SELECT ALL -- " },
                            { name: "Position", text: "Position", cls: "span3", type: "select", opt_text: "-- SELECT ALL --" },
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
            {
                name: "InqPers",
                xtype: "k-grid",
            },
        ],
        toolbars: [
            { name: "btnRefresh", text: "Refresh", icon: "fa fa-refresh" },
            { name: "btnExportXls", text: "Export (xls)", icon: "fa fa-file-excel-o" },
            { name: "btnLinkPhoto", text: "Download Photo", icon: "fa fa-file-o" },
        ],
    }
    var widget = new SimDms.Widget(options);
    // widget.setSelect([{ name: "CompanyCode", url: "wh.api/combo/organizations", optionalText: "-- SELECT ONE --" }]);
    // widget.setSelect([{ name: "CompanyCode", url: "wh.api/combo/DealerList", optionalText: "-- SELECT ONE --", params: { LinkedModule: "MP" } }]);
    widget.setSelect([{ name: "GroupArea", url: "wh.api/combo/GroupAreas", optionalText: "-- SELECT ALL --" }]);
    widget.default = { Status: "1" };
    widget.render(function () {
        widget.populate(widget.default);
        $("#CompanyCode").prop('disabled', true);
        $("[name=GroupArea]").on("change", function () {
            var groupArea = $("[name=GroupArea]").val();
            if (groupArea == '' || groupArea == undefined) {
                $("#CompanyCode").prop('disabled', true);
            }
            else {
                $("#CompanyCode").prop('disabled', false);
            }

            widget.select({ selector: "[name=CompanyCode]", url: "wh.api/combo/ComboDealerList", params: { GroupArea: groupArea }, optionalText: "-- SELECT ALL --" });
            $("[name=CompanyCode]").prop("selectedIndex", 0);
            $("[name=CompanyCode]").change();
        });
        $("[name=CompanyCode]").on("change", function () {
            widget.select({ selector: "[name=Department]", url: "wh.api/combo/departments", params: { comp: $("[name=CompanyCode]").val() }, optionalText: "-- SELECT ALL --" });
            $("[name=Department]").prop("selectedIndex", 0);
            $("[name=Department]").change();
        });
        $("[name=Department]").on("change", function () {
            widget.select({ selector: "[name=Position]", url: "wh.api/combo/positions", params: { comp: $("[name=CompanyCode]").val(), dept: $("[name=Department]").val() }, optionalText: "-- SELECT ALL --" });
            $("[name=Position]").prop("selectedIndex", 0);
            $("[name=Position]").change();
        });
        $("#btnRefresh").on("click", refreshGrid);
        $("#btnExportXls").on("click", exportXls);
        $("#btnLinkPhoto").on("click", gotoPhoto);
        // $("select[name=Position],select[name=Status]").on("change", refreshGrid);
    });

    function refreshGrid() {
        widget.kgrid({
            url: "wh.api/inquiry/employeesNew",
            name: "InqPers",
            params: $("#pnlFilter").serializeObject(),
            serverBinding: true,
            pageSize: 10,
            columns: [
                { field: "EmployeeID", width: 120, title: "NIK" },
                { field: "EmployeeName", width: 250, title: "Name", filterable: { extra: false, operators: { string: { contains: "Contains", startswith: "Starts with", } } } },
                { field: "TeamLeaderName", width: 250, title: "Leader Name" },
                { field: "LastPosition", title: "Last Position" },
                { field: "SubOrdinates", width: 80, title: "Subs" },
                { field: "Status", width: 100, title: "Status" },
            ],
            detailInit: detailInit
        });
    }

    function detailInit(e) {
        widget.post("wh.api/inquiry/employeesubordinates", { CompanyCode: $("#pnlFilter [name=CompanyCode]").val(), EmployeeID: e.data.EmployeeID }, function (data) {
            if (data.length > 0) {
                $("<div/>").appendTo(e.detailCell).kendoGrid({
                    dataSource: { data: data, pageSize: 10 },
                    pageable: true,
                    columns: [
                        { field: "EmployeeID", width: 120, title: "NIK" },
                        { field: "EmployeeName", width: 280, title: "Name" },
                        { field: "JoinDate", title: "Join Date", width: 160, template: "#= ((JoinDate === undefined) ? \"\" : moment(JoinDate).format('DD MMM YYYY')) #" },
                        { field: "LastPosition", title: "Last Position" },
                    ]
                });
            }
            else {
                $("<div/>").appendTo(e.detailCell).kendoGrid({
                    dataSource: { data: [{ Info: "Employee ini tidak memiliki sub-ordinate" }] },
                    columns: [{ field: "Info", title: "Info" }]
                });
            }
        })
    }

    function gotoPhoto() {
        var params = $("#pnlFilter").serializeObject();
        console.log(params)

        var win = window.open('../reportserver/?id=empphoto', '_blank');
        win.focus();
    }

    function exportXls() {
        widget.post("wh.api/combo/getdealertype", function (result) {
            switch (result) {
                case "2W":
                    widget.exportXls({
                        source: "wh.api/inquiry/sfmperslist?CompanyCode=" + $("[name='CompanyCode']").val(),
                        fileName: "personal_list",
                        items: [
                            { name: "EmployeeID", text: "NIK", type: "text" },
                            { name: "SalesID", text: "Sales ID", type: "text" },
                            { name: "SalesName", text: "Nama Sales", type: "text" },
                            { name: "SalesFullName", text: "Nama Lengkap Sales", type: "text" },
                            { name: "AreaCode", text: "Kode Area", type: "text" },
                            { name: "DealerGroup", text: "Group Dealer", type: "text" },
                            { name: "DealerCode", text: "Kode Dealer", type: "text" },
                            { name: "DealerName", text: "Nama Dealer" },
                            { name: "OutletCode", text: "Kode Outlet", type: "text" },
                            { name: "OutletType", text: "Tipe Outlet", type: "text" },
                            { name: "OutletName", text: "Nama Outlet", width: 400 },
                            { name: "JobCode", text: "Kode Posisi" },
                            { name: "JobName", text: "Posisi" },
                            { name: "GradeCode", text: "Kode Grade" },
                            { name: "GradeName", text: "Grade" },
                            { name: "JoinDate", text: "Tanggal Bergabung", type: "date" },
                            { name: "IsAdditionalJob", text: "Mempunyai Pekerjaan Tambahan" },
                            { name: "AdditionalJob", text: "Pekerjaan Tambahan" },
                            { name: "BirthPlace", text: "Tempat Lahir" },
                            { name: "BirthDate", text: "Tanggal Lahir", type: "date" },
                            { name: "Education", text: "Pendidikan" },
                            { name: "Phone1", text: "Phone 1", type: "text" },
                            { name: "Phone2", text: "Phone 2", type: "text" },
                            { name: "Religion", text: "Agama" },
                            { name: "KtpNo", text: "KTP", type: "text", width: 100 },
                            { name: "Sim1", text: "Sim 1", type: "text" },
                            { name: "Sim2", text: "Sim 2", type: "text" },
                            { name: "TerminateStatus", text: "Terminate Status" },
                            { name: "TerminateReason", text: "Terminate Reason" },
                            { name: "TerminateDate", text: "Terminate Date", type: "date" },
                            { name: "MaritalStatus", text: "Marital Status" },
                            { name: "EmployeeStatus", text: "Employee Status" },
                            { name: "Gender", text: "Gender" },
                            { name: "Email", text: "Email" },
                            { name: "TeamLeader", text: "Team Leader ID", type: "text" },
                            { name: "TeamLeaderSalesID", text: "Team Leader Sales ID" },
                            { name: "TeamLeaderName", text: "Team Leader Name" },
                            { name: "Nik", text: "NIK", type: "text" },
                            { name: "StatusSf", text: "Status SF" },
                            { name: "StatusSm", text: "Status SM" },
                        ]
                    });
                    break;
                default:
                    
                    $.ajax({
                        async: true,
                        type: "POST",
                        data: $("#pnlFilter").serializeObject(),
                        url: "wh.api/report/InqEmployeesNew?type=list",
                        success: function (data) {
                            if (data.message == "") {
                                location.href = 'wh.api/report/DownloadExcelFile?key=' + data.value + '&filename=Personal List Employee';
                            } else {
                                sdms.info(data.message, "Error");
                            }
                        }
                    });
                    
                    /*
                    widget.exportXls({
                        name: "InqPers",
                        type: "kgrid",
                        fileName: "personal_list",
                        items: [
                            { field: "CompanyCode", width: 120, title: "Dealer Code" },
                            { field: "LastBranch", width: 120, title: "Branch Code" },
                            { field: "DealerName", width: 350, title: "Dealer" },
                            { field: "LastBranchName", width: 350, title: "Branch" },
                            { field: "Department", width: 250, title: "Department" },
                            { field: "LastPosition", width: 250, title: "Last Position" },
                            { field: "EmployeeID", width: 120, title: "NIK" },
                            { field: "EmployeeName", width: 250, title: "Name" },
                            { field: "TeamLeaderName", width: 250, title: "Leader Name" },
                            { field: "SubOrdinates", width: 80, title: "Subs" },
                            { field: "Status", width: 150, title: "Status" },
                            { field: "SubOrdinates", width: 150, title: "Subordinates" },
                            { field: "MutationTimes", width: 150, title: "Mutation Times" },
                            { field: "AchieveTimes", width: 150, title: "Achieve Times" },
                            { field: "LastBranch", width: 150, title: "Last Branch" },
                            { field: "TeamLeader", width: 250, title: "Team Leader" },
                            { field: "JoinDate", type: "date", width: 150, title: "Join Date" },
                            { field: "ResignDate", type: "date", width: 150, title: "Resign Date" },
                            { field: "ResignDescription", width: 250, title: "Resign Description" },
                            { field: "MaritalStatus", width: 150, title: "Marital Status" },
                            { field: "Religion", width: 150, title: "Religion" },
                            { field: "Gender", width: 150, title: "Gender" },
                            { field: "Education", width: 150, title: "Education" },
                            { field: "BirthPlace", width: 250, title: "Birth Place" },
                            { field: "BirthDate", type: "date", width: 150, title: "Birth Date" },
                            { field: "Address", width: 400, title: "Address" },
                            { field: "Province", width: 250, title: "Province" },
                            { field: "District", width: 250, title: "District" },
                            { field: "SubDistrict", width: 250, title: "Sub District" },
                            { field: "Village", width: 250, title: "Village" },
                            { field: "ZipCode", width: 150, title: "Zip Code", type: "text" },
                            { field: "IdentityNo", width: 150, title: "Identity Number", type: "text" },
                            { field: "NPWPNo", width: 150, title: "NPWP No", type: "text" },
                            { field: "NPWPDate", type: "date", width: 150, title: "NPWP Date" },
                            { field: "Email", width: 150, title: "Email" },
                            { field: "Telephone1", width: 150, title: "Telephone 1", type: "text" },
                            { field: "Telephone1", width: 150, title: "Telephone 2", type: "text" },
                            { field: "Handphone1", width: 150, title: "Handphone 1", type: "text" },
                            { field: "Handphone2", width: 150, title: "Handphone 2", type: "text" },
                            { field: "Handphone3", width: 150, title: "Handphone 3", type: "text" },
                            { field: "Handphone4", width: 150, title: "Handphone 4", type: "text" },
                            { field: "DrivingLicense1", width: 150, title: "Driving License 1" },
                            { field: "DrivingLicense2", width: 150, title: "Driving License 2" },
                            { field: "Height", width: 150, title: "Height" },
                            { field: "Weight", width: 150, title: "Weight" },
                            { field: "BloodCode", width: 150, title: "Blood Type" },
                            { field: "UniformSize", width: 150, title: "Uniforms Size" },
                            { field: "UniformSizeAlt", width: 150, title: "Uniform Size Alt" },
                            { field: "ShoesSize", width: 150, title: "Shoes Size" },
                        ]
                    });
                    */

                    break;
            }
        });
    }
});
