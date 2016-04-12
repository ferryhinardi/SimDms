$(document).ready(function () {
    var options = {
        title: "Personal Information by Cut Off Date",
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
                    { name: "CutOff", text: "Cut Off", cls: "span4", type: "datepicker" },
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
    widget.default = { Status: "1", CompanyCode: "" , CutOff: new Date()};

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
        $("#btnExportXls").on("click", exportXls);
        //$("#CutOff").val(new Date());
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
            CutOff: $("[name=CutOff]").val()
        }
        widget.kgrid({
            url: "wh.api/inquiry/sfmpersinfocutoffNew",
            name: "SfmPersInfo",
            params: params,
            serverBinding: true,
            pageSize: 10,
            columns: [
                { field: "SalesID", title: "ATPM ID", width: 200 },
                { field: "EmployeeID", title: "NIK", width: 125 },
                { field: "DeptCode", title: "Department", width: 125 },
                { field: 'CompanyCode', title: 'Kode Dealer', width: 125 },
                { field: "DealerName", title: "Nama Dealer", width: 200 },
                { field: 'BranchCode', title: 'Kode Outlet', width: 125 },
                { field: "BranchName", title: "Outlet", width: 150 },
                { field: "EmployeeName", title: "Nama", width: 280 },
                { field: "PosName", title: "Jabatan", width: 200 },
                { field: "Grade", title: "Grade", width: 120 },
                { field: "AdditionalJob", title: "Additional Job", width: 120 },
                { field: "TeamLeader", title: "Leader Name", width: 200 },
                { field: "Status", title: "Status", width: 120 },
                { field: "JoinDate", title: "Tanggal Join", width: 125, type: "date" },
                { field: "ResignDate", title: "tanggal Resign", width: 125, type: "date" },
                { field: "ResignDescription", title: "Alasan Resign", width: 300 },
                { field: "BirthDate", title: "Tanggal Lahir", width: 125, type: "date" },
                { field: "BirthPlace", title: "Tempat Lahir", width: 150 },
                { field: "Gender", title: "Jenis Kelamin", width: 110 },
                { field: "Religion", title: "Agama", width: 110 },
                { field: "MaritalStatus", title: "Status PErkawinan", width: 125 },
                { field: "Education", title: "Pendidikan", width: 110 },
                { field: "Address", title: "Alamat", width: 500 },
                { field: "City", title: "Kota", width: 200 },
                { field: "Province", title: "Provinsi", width: 200 },
                { field: "ZipCode", title: "Kode Pos", width: 90 },
                { field: "IdentityNo", title: "KTP", width: 200 },
                { field: "NPWPNo", title: "NPWP", width: 200 },
                { field: "DrivingLicense1", title: "SIM 1", width: 200 },
                { field: "DrivingLicense2", title: "SIM 2", width: 200 },
                { field: "Telephone1", title: "Phone", width: 180 },
                { field: "Handphone1", title: "HP 1", width: 180 },
                { field: "Handphone2", title: "HP 2", width: 180 },
                { field: "Handphone3", title: "PIN BB", width: 120 },
                { field: "Email", title: "Email", width: 200 },
                { field: "Height", title: "Tinggi Badan", width: 120 },
                { field: "Weight", title: "Berat Badan", width: 120 },
                { field: "UniformSize", title: "Size", width: 100 },
                { field: "UniformSizeAlt", title: "Size Alt", width: 100 },
                { field: "PreTraining",title: "Pre Training",width: 125, type: "date" },
                { field: "PreTrainingPostTest", title: "Nilai Pre Training", width: 125 },
                { field: "Pembekalan",  title:"Pembekalan",width: 125, type: "date" },
                { field: "PembekalanPostTest", title: "Nilai Pembekalan", width: 125 },
                { field: "Salesmanship", title: "Salesmanship", width: 100 },
                { field: "SalesmanshipPostTest", title: "Nilai Salesmanship", width: 125 },
                { field: "OJT", title: "OJT", width: 125, type: "date" },
                { field: "FinalReview", Title:"Final Review",width: 125, type: "date" },
                { field: "FinalReviewPostTest", title: "Nilai Akhir STDP", width: 90 },
                { field: "SpsSlv", title: "SPS Silver", width: 125, type: "date" },
                { field: "SpsGld", title: "SPS Gold", width: 125, type: "date" },
                { field: "SpsPlt", title: "SPS Platinum", width: 125, type: "date" },
                { field: "SHBsc", title: "SH Basic", width: 125, type: "date" },
                { field: "SHInt", title: "SH Intermediate", width: 125, type: "date" },
                { field: "BMBsc", title: "BMDP Basic", width: 125, type: "date" },
                { field: "BMInt", title: "BMDP Intermediate", width: 125, type: "date" },
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
            CutOff: $("[name=CutOff]").val()
        }
        $.ajax({
            async: true,
            type: "POST",
            data: params,
            url: "wh.api/report/SfmPersInfoCutOff",
            success: function (data) {
                if (data.message == "") {
                    location.href = 'wh.api/report/DownloadExcelFile?key=' + data.value + '&filename=Personal Information by Cut Off Date';
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
