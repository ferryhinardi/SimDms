$(document).ready(function () {
    var vars = {
        productType: undefined
    };

    var options = {
        title: "Personal Information",
        xtype: "panels",
        panels: [
            {
                name: "pnlFilter",
                items: [
                    {
                        text: "Position - Status",
                        type: "controls",
                        items: [
                            { name: "Position", text: "Position", cls: "span3", type: "select" },
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
                    {
                        text: "Branch (Outlet)",
                        type: "controls",
                        items: [{ name: "Branch", text: "Branch", cls: "span5", type: "select" }]
                    },
                ],
            },
            {
                name: "SfmPersInfo",
                xtype: "k-grid",
            },
        ],
        toolbars: [
            { name: "btnRefresh", text: "Refresh", icon: "icon-refresh" },
            { name: "btnExportXls", text: "Export (xls)", icon: "icon-xls" },
        ],
    }
    var widget = new SimDms.Widget(options);
    widget.setSelect([
        { name: "Branch", url: "ab.api/combo/branch", optionalText: "-- SELECT ALL --" },
        { name: "Position", url: "ab.api/combo/positions", optionalText: "-- SELECT ALL --", params: { id: "SALES" } }
    ]);
    widget.default = { Status: "1" };
    widget.render(function () {
        widget.populate(widget.default);
        $("#btnRefresh").on("click", refreshGrid);
        $("#btnExportXls").on("click", exportXls);
        $("#pnlFilter select").on("change", refreshGrid);
        setProductType();
        setTimeout(refreshGrid, 3000);
    });

    function setProductType() {
        widget.post("ab.api/Employee/ProductType", function (result) {
            vars.productType = result;
        });
    }

    function refreshGrid() {
        var url = "ab.api/inquiry/sfmpersinfo";
        //if (vars.productType == "4W") {
        //    url = "ab.api/inquiry/sfmpersinfo";
        //}
        //else {
        //    url = "ab.api/inquiry/sfmpersinfo2W";
        //}

        widget.kgrid({
            url: url , // "wh.api/inquiry/sfmpersinfo",
            name: "SfmPersInfo",
			params: $("#pnlFilter").serializeObject(),
            columns: [
                { field: "BranchName", title: "Outlet", width: 420 },
                { field: "DeptCode", title: "Department", width: 125 },
                { field: "EmployeeID", title: "NIK", width: 125 },
                //{ field: "SalesID", title: "Sales ID", width: 100 },
                { field: "EmployeeName", title: "Nama", width: 280 },
                { field: "PosName", title: "Jabatan", width: 200 },
                { field: "Grade", title: "Grade", width: 120 },
                { field: "JobOther1", title: "Additional Job 1", width: 120 },
                { field: "JobOther2", title: "Additional Job 2", width: 120 },
                { field: "Status", title: "Status", width: 120 },
                { field: "JoinDate", type: "date", title: "Join Date", width: 125 },
                { field: "TeamLeader", title: "Team Leader", width: 150 },
                { field: "ResignDate", type: "date", title: "Resign Date", width: 125 },
                { field: "ResignDescription", title: "Resign Description", width: 225 },
                { field: "MaritalStatus", title: "Marital Status", width: 125 },
                { field: "Religion", title: "Religion", width: 110 },
                { field: "Gender", title: "Gender", width: 110 },
                { field: "Education", title: "Education", width: 110 },
                { field: "BirthPlace", title: "Birth Place", width: 150 },
                { field: "BirthDate", type: "date", title: "Birth Date", width: 125 },
                { field: "Address", title: "Address", width: 550 },
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

        var url = "ab.api/inquiry/SfmPersInfoXls";
        //if (vars.productType == "4W") {
        //    url = "ab.api/inquiry/SfmPersInfoXls";
        //}
        //else {
        //    url = "ab.api/inquiry/SfmPersInfoXls";
        //}

        //if (vars.productType == "2W") {
        //    exportXlsR2();
        //}
        //else {
        //    exportXlsR4();
        //}

        sdms.report({
            url: url,
            type: 'xlsx',
            params: widget.serializeObject('pnlFilter')
        });
    }

    function exportXlsR2() {
        widget.exportXls({
            name: "SfmPersInfo",
            type: "kgrid",
            items: [
                { field: "EmployeeID", width: 120, title: "NIK" },
                { field: "SalesID", width: 120, title: "Sales ID" },
                { field: "EmployeeName", width: 450, title: "Nama Sales" },
                { field: "EmployeeName", width: 450, title: "Nama Lengkap Sales" },
                { field: "AreaCode", width: 120, title: "Kode Area" },
                { field: "DealerGroup", width: 150, title: "Group Dealer" },
                { field: "CompanyCode", width: 120, title: "Kode Dealer" },
                { field: "CompanyName", width: 500, title: "Nama Dealer" },
                { field: "BranchCode", width: 120, title: "Kode Outlet" },
                { field: "OutletType", width: 120, title: "Tipe Outlet" },
                { field: "BranchName", width: 500, title: "Nama Outlet" },
                { field: "PositionCode", width: 120, title: "Kode Posisi" },
                { field: "Position", width: 250, title: "Posisi" },
                { field: "Grade", width: 120, title: "Kode Grade" },
                { field: "GradeCode", width: 120, title: "Grade" },
                { field: "JoinDate", width: 120, title: "Tanggal Bergabung"},
                //{ field: "IsAdditionalJob", width: 120, title: "Mempunyai Pekerjaan Tambahan" },
                //{ field: "AdditionalJob", width: 120, title: "Pekerjaan Tambahan" },
                { field: "BirthPlace", width: 200, title: "Tempat Lahir" },
                { field: "BirthDate", width: 120, title: "Tanggal Lahir"},
                { field: "Education", width: 120, title: "Pendidikan" },
                { field: "Telephone1", width: 150, title: "Phone 1", type: "text" },
                { field: "Telephone2", width: 150, title: "Phone 2", type: "text" },
                { field: "Handphone1", width: 150, title: "Handphone 1", type:"text" },
                { field: "Handphone2", width: 150, title: "Handphone 2", type:"text" },
                { field: "Handphone3", width: 150, title: "Handphone 3", type:"text" },
                { field: "Handphone4", width: 150, title: "Handphone 4", type:"text" },
                { field: "Religion", width: 120, title: "Agama" },
                { field: "IdentityNo", width: 150, title: "KTP", type: "text" },
                { field: "DrivingLicense1", width: 180, title: "SIM 1" },
                { field: "DrivingLicense2", width: 180, title: "SIM 2" },
                { field: "TerminateStatus", width: 120, title: "Terminate Status" },
                { field: "TerminateReason", width: 200, title: "Terminate Reason" },
                { field: "TerminateDate", width: 120, title: "Terminate Date"},
                { field: "MaritalStatus", width: 120, title: "Marital Status" },
                { field: "Status", width: 120, title: "Employee Status" },
                { field: "Gender", width: 120, title: "Gender" },
                { field: "Email", width: 250, title: "Email" },
                { field: "TeamLeaderID", width: 120, title: "Team Leader ID" },
                { field: "TeamLeaderSalesID", width: 180, title: "Team Leader Sales ID" },
                { field: "TeamLeaderName", width: 450, title: "Team Leader Name" },
                { field: "StatusSF", width: 120, title: "Status SF" },
                { field: "StatusSM", width: 120, title: "Status SM" },
            ]
        });
    }

    function exportXlsR4() {
        widget.exportXls({
            name: "SfmPersInfo",
            type: "kgrid",
            items: [
                //{ name: "BranchCode", text: "Outlet", width: 380 },
                //{ name: "EmployeeID", text: "Empl ID", width: 100 },
                //{ name: "SalesID", text: "Sales ID", width: 100 },
                //{ name: "EmployeeName", text: "Employee Name", width: 250 },
                //{ name: "Position", text: "Position", width: 300 },
                //{ name: "Status", text: "Status", width: 120 },
                //{ field: "CompanyCode", width: 120, title: "Dealer Code" },
                { field: "BranchName", width: 700, title: "Branch Name" },
                //{ field: "DealerName", width: 350, title: "Dealer" },
                //{ field: "LastBranchName", width: 350, title: "Branch" },
                { field: "Department", width: 250, title: "Department" },
                { field: "Position", width: 250, title: "Last Position" },
                { field: "EmployeeID", width: 200, title: "NIK" },
                { field: "EmployeeName", width: 350, title: "Name" },
                { field: "Status", width: 150, title: "Status" },
                { field: "SubOrdinates", width: 150, title: "Subordinates" },
                { field: "MutationTimes", width: 150, title: "Mutation Times" },
                { field: "AchieveTimes", width: 150, title: "Achieve Times" },
                { field: "TeamLeader", width: 350, title: "Team Leader" },
                { field: "JoinDate", type: "date", width: 150, title: "Join Date" },
                { field: "ResignDate", type: "date", width: 150, title: "Resign Date" },
                { field: "ResignDescription", width: 550, title: "Resign Description" },
                { field: "MaritalStatus", width: 150, title: "Marital Status" },
                { field: "Religion", width: 150, title: "Religion" },
                { field: "Gender", width: 150, title: "Gender" },
                { field: "Education", width: 150, title: "Education" },
                { field: "BirthPlace", width: 250, title: "Birth Place" },
                { field: "BirthDate", type: "date", width: 150, title: "Birth Date" },
                { field: "Address", width: 500, title: "Address" },
                { field: "Province", width: 350, title: "Province" },
                { field: "District", width: 350, title: "District" },
                { field: "SubDistrict", width: 350, title: "Sub District" },
                { field: "Village", width: 250, title: "Village" },
                { field: "ZipCode", width: 150, title: "Zip Code" },
                { field: "IdentityNo", width: 150, title: "Identity Number" },
                { field: "NPWPNo", width: 150, title: "NPWP No" },
                { field: "NPWPDate", type: "date", width: 150, title: "NPWP Date" },
                { field: "Email", width: 150, title: "Email" },
                { field: "Telephone1", width: 150, title: "Telephone 1" },
                { field: "Telephone1", width: 150, title: "Telephone 2" },
                { field: "Handphone1", width: 150, title: "Handphone 1" },
                { field: "Handphone2", width: 150, title: "Handphone 2" },
                { field: "Handphone3", width: 150, title: "Handphone 3" },
                { field: "Handphone4", width: 150, title: "Handphone 4" },
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
    }
});
