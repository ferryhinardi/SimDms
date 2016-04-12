$(document).ready(function () {
    var options = {
        title: "SDMS Data List",
        xtype: "panels",
        panels: [
            {
                name: "pnlFilter",
                items: [
                    { name: "CompanyCode", text: "Organization", cls: "span7", type: "select" },
                    { name: "DataType", text: "Data Type", cls: "span4", type: "select" },
                    {
                        name: "DataStatus", text: "Status", cls: "span3", type: "select", opt_text: "ALL STATUS",
                        items: [
                            { text: "A - Initial", value: "A" },
                            { text: "P - Processed", value: "P" },
                            { text: "X - Data Fail", value: "X" },
                        ]
                    },
                ]
            },
            {
                name: "KGrid",
                xtype: "k-grid",
            },
        ],
        toolbars: [
            { name: "btnRefresh", text: "Refresh", icon: "fa fa-refresh" },
        ],
    }

    var widget = new SimDms.Widget(options);
    widget.setSelect([
        { name: "CompanyCode", url: "wh.api/combo/organizations", optionalText: "-- SELECT ONE --" },
        { name: "DataType", url: "util.api/combo/datatypes", optionalText: "-- SELECT ALL --" },
    ]);
    widget.render(function () {
        widget.populate({ DataStatus: "P" });
        $("#btnRefresh").on("click", refreshGrid);
        $("select").on("change", refreshGrid);
    });

    function refreshGrid() {
        var params = $("#pnlFilter").serializeObject();
        widget.kgrid({
            url: "util.api/inquiry/datalist",
            name: "KGrid",
            params: params,
            serverBinding: true,
            sort: [{ field: "UpdatedDate", dir: "desc" }],
            columns: [
                { field: "DataType", width: 120, title: "Data Type" },
                { field: "Segment", width: 100, title: "Segment" },
                { field: "LastSendDate", width: 180, title: "Send Date", template: "#= (LastSendDate == undefined) ? '-' : moment(LastSendDate).format('DD MMM YYYY  HH:mm:ss') #" },
                { field: "Status", width: 100, title: "Status" },
                { field: "CreatedDate", width: 180, title: "Created Date", template: "#= (CreatedDate == undefined) ? '-' : moment(CreatedDate).format('DD MMM YYYY  HH:mm:ss') #" },
                { field: "UpdatedDate", width: 180, title: "Updated Date", template: "#= (UpdatedDate == undefined) ? '-' : moment(UpdatedDate).format('DD MMM YYYY  HH:mm:ss') #" },
            ],
            detailInit: detailInit
        });
    }

    function detailInit(e) {
        widget.post("util.api/inquiry/getdata/" + e.data.UniqueID, function (result) {
            if (result.success) {
                var columns = [];
                switch (result.type) {
                    case "EMPLY":
                        columns = [
                             { field: "CompanyCode", title: "CompanyCode", width: 100 },
                             { field: "EmployeeID", title: "EmployeeID", width: 100 },
                             { field: "EmployeeName", title: "EmployeeName", width: 200 },
                             { field: "JoinDate", title: "JoinDate", width: 140, template: "#= ((JoinDate == undefined) ? \"\" : moment(JoinDate).format('DD MMM YYYY')) #" },
                             { field: "Department", title: "Dept", width: 120 },
                             { field: "Position", title: "Pos", width: 80 },
                             { field: "Grade", title: "Grade", width: 80 },
                             { field: "Rank", title: "Rank", width: 80 },
                             { field: "Gender", title: "Gender", width: 80 },
                             { field: "TeamLeader", title: "Leader", width: 160 },
                             { field: "PersonnelStatus", title: "Status", width: 80 },
                             { field: "CreatedBy", title: "CreatedBy", width: 140 },
                             { field: "CreatedDate", title: "CreatedDate", width: 140, template: "#= ((CreatedDate == undefined) ? \"\" : moment(CreatedDate).format('DD MMM YYYY  HH:mm:ss')) #" },
                             { field: "UpdatedBy", title: "UpdatedBy", width: 140 },
                             { field: "UpdatedDate", title: "UpdatedDate", width: 140, template: "#= ((UpdatedDate == undefined) ? \"\" : moment(UpdatedDate).format('DD MMM YYYY  HH:mm:ss')) #" },
                        ];
                        break;
                    case "EMACH":
                        columns = [
                             { field: "CompanyCode", title: "CompanyCode", width: 100 },
                             { field: "EmployeeID", title: "EmployeeID", width: 100 },
                             { field: "AssignDate", title: "AssignDate", width: 140, template: "#= ((AssignDate == undefined) ? \"\" : moment(AssignDate).format('DD MMM YYYY')) #" },
                             { field: "Department", title: "Dept", width: 120 },
                             { field: "Position", title: "Pos", width: 80 },
                             { field: "Grade", title: "Grade", width: 80 },
                             { field: "IsJoinDate", title: "Join?", width: 80 },
                             { field: "CreatedBy", title: "CreatedBy", width: 140 },
                             { field: "CreatedDate", title: "CreatedDate", width: 140, template: "#= ((CreatedDate == undefined) ? \"\" : moment(CreatedDate).format('DD MMM YYYY  HH:mm:ss')) #" },
                             { field: "UpdatedBy", title: "UpdatedBy", width: 140 },
                             { field: "UpdatedDate", title: "UpdatedDate", width: 140, template: "#= ((UpdatedDate == undefined) ? \"\" : moment(UpdatedDate).format('DD MMM YYYY  HH:mm:ss')) #" },
                        ];
                        break;
                    case "EMUTA":
                        columns = [
                             { field: "CompanyCode", title: "CompanyCode", width: 100 },
                             { field: "EmployeeID", title: "EmployeeID", width: 100 },
                             { field: "MutationDate", title: "MutationDate", width: 140, template: "#= ((MutationDate == undefined) ? \"\" : moment(MutationDate).format('DD MMM YYYY')) #" },
                             { field: "BranchCode", title: "Branch", width: 100 },
                             { field: "IsJoinDate", title: "Join?", width: 80 },
                        ];
                        break;
                    case "EMSFM":
                        columns = [
                             { field: "CompanyCode", title: "CompanyCode", width: 100 },
                             { field: "EmployeeID", title: "EmployeeID", width: 100 },
                             { field: "SalesID", title: "SalesID", width: 100 },
                             { field: "CreatedBy", title: "CreatedBy", width: 100 },
                             { field: "CreatedDate", title: "CreatedDate", width: 140, template: "#= ((CreatedDate == undefined) ? \"\" : moment(CreatedDate).format('DD MMM YYYY')) #" },
                        ];
                        break;
                    case "SVSPK":
                        columns = [
                             { field: "CompanyCode", title: "CompanyCode", width: 100 },
                             { field: "BranchCode", title: "BranchCode", width: 100 },
                             { field: "ServiceNo", title: "ServiceNo", width: 100 },
                             { field: "JobOrderNo", title: "JobOrderNo", width: 100 },
                             { field: "JobOrderDate", title: "JobOrderDate", width: 140, template: "#= ((JobOrderDate == undefined) ? \"\" : moment(JobOrderDate).format('DD MMM YYYY  HH:mm:ss')) #" },
                             { field: "BasicModel", title: "BasicModel", width: 100 },
                             { field: "PoliceRegNo", title: "PoliceRegNo", width: 100 },
                             { field: "JobType", title: "JobType", width: 100 },
                             //{ field: "ServiceRequestDesc", title: "ServiceRequestDesc", width: 300 },
                             { field: "CreatedBy", title: "CreatedBy", width: 140 },
                             { field: "CreatedDate", title: "CreatedDate", width: 140, template: "#= ((CreatedDate == undefined) ? \"\" : moment(CreatedDate).format('DD MMM YYYY  HH:mm:ss')) #" },
                             { field: "LastUpdateBy", title: "UpdatedBy", width: 140 },
                             { field: "LastUpdateDate", title: "UpdatedDate", width: 140, template: "#= ((LastUpdateDate == undefined) ? \"\" : moment(LastUpdateDate).format('DD MMM YYYY  HH:mm:ss')) #" },
                        ];
                        break;
                    case "SVINV":
                        columns = [
                             { field: "CompanyCode", title: "CompanyCode", width: 100 },
                             { field: "BranchCode", title: "BranchCode", width: 100 },
                             { field: "InvoiceNo", title: "InvoiceNo", width: 100 },
                             { field: "InvoiceDate", title: "InvoiceDate", width: 140, template: "#= ((InvoiceDate == undefined) ? \"\" : moment(InvoiceDate).format('DD MMM YYYY  HH:mm:ss')) #" },
                             { field: "BasicModel", title: "BasicModel", width: 100 },
                             { field: "PoliceRegNo", title: "PoliceRegNo", width: 100 },
                             { field: "JobType", title: "JobType", width: 100 },
                             { field: "InvoiceStatus", title: "Inv Status", width: 100 },
                             { field: "FPJNo", title: "FPJNo", width: 100 },
                             { field: "FPJDate", title: "FPJDate", width: 140, template: "#= ((FPJDate == undefined) ? \"\" : moment(FPJDate).format('DD MMM YYYY  HH:mm:ss')) #" },
                             //{ field: "ServiceRequestDesc", title: "ServiceRequestDesc", width: 300 },
                             { field: "CreatedBy", title: "CreatedBy", width: 140 },
                             { field: "CreatedDate", title: "CreatedDate", width: 140, template: "#= ((CreatedDate == undefined) ? \"\" : moment(CreatedDate).format('DD MMM YYYY  HH:mm:ss')) #" },
                             { field: "LastupdateBy", title: "UpdatedBy", width: 140 },
                             { field: "LastupdateDate", title: "UpdatedDate", width: 140, template: "#= ((LastupdateDate == undefined) ? \"\" : moment(LastupdateDate).format('DD MMM YYYY  HH:mm:ss')) #" },
                        ];
                        break;
                    case "SVMSI":
                        columns = [
                             { field: "CompanyCode", title: "CompanyCode", width: 100 },
                             { field: "BranchCode", title: "BranchCode", width: 100 },
                             { field: "PeriodYear", title: "Year", width: 100 },
                             { field: "PeriodMonth", title: "Month", width: 80 },
                             { field: "SeqNo", title: "SeqNo", width: 80 },
                             { field: "MsiGroup", title: "MsiGroup", width: 250 },
                             { field: "MsiDesc", title: "MsiDesc", width: 400 },
                             { field: "MsiData", title: "Data", width: 120, template: "<div class='right'>#= ((MsiData == undefined) ? \"\" : number_format(MsiData, 2)) #</div>" },
                             { field: "CreatedBy", title: "CreatedBy", width: 140 },
                             { field: "CreatedDate", title: "CreatedDate", width: 140, template: "#= ((CreatedDate == undefined) ? \"\" : moment(CreatedDate).format('DD MMM YYYY  HH:mm:ss')) #" },
                        ];
                        break;
                    case "PMKDP":
                        columns = [
                             { field: "CompanyCode", title: "CompanyCode", width: 100 },
                             { field: "BranchCode", title: "BranchCode", width: 100 },
                             { field: "InquiryNumber", title: "InqNum", width: 100 },
                             { field: "EmployeeID", title: "EmployeeID", width: 100 },
                             { field: "InquiryDate", title: "InquiryDate", width: 140, template: "#= ((InquiryDate == undefined) ? \"\" : moment(InquiryDate).format('DD MMM YYYY  HH:mm:ss')) #" },
                             { field: "StatusProspek", title: "StatusProspek", width: 100 },
                             { field: "PerolehanData", title: "PerolehanData", width: 140 },
                             { field: "NamaProspek", title: "NamaProspek", width: 200 },
                             { field: "Handphone", title: "Handphone", width: 100 },
                             { field: "TipeKendaraan", title: "Tipe", width: 160 },
                             { field: "Variant", title: "Variant", width: 140 },
                             { field: "Transmisi", title: "Transmisi", width: 100 },
                             { field: "ColourCode", title: "ColourCode", width: 100 },
                             { field: "CaraPembayaran", title: "CaraBayar", width: 100 },
                             { field: "TestDrive", title: "TestDrive", width: 100 },
                             { field: "LastProgress", title: "LastProgress", width: 100 },
                             { field: "CreatedBy", title: "CreatedBy", width: 120 },
                             { field: "CreationDate", title: "CreatedDate", width: 140, template: "#= ((CreationDate == undefined) ? \"\" : moment(CreationDate).format('DD MMM YYYY  HH:mm:ss')) #" },
                             { field: "LastUpdateBy", title: "UpdatedBy", width: 120 },
                             { field: "LastUpdateDate", title: "UpdatedDate", width: 140, template: "#= ((LastUpdateDate == undefined) ? \"\" : moment(LastUpdateDate).format('DD MMM YYYY  HH:mm:ss')) #" },
                        ];
                        break;
                    case "PMSHS":
                        columns = [
                             { field: "CompanyCode", title: "CompanyCode", width: 100 },
                             { field: "BranchCode", title: "BranchCode", width: 100 },
                             { field: "InquiryNumber", title: "InqNum", width: 100 },
                             { field: "SequenceNo", title: "SeqNo", width: 100 },
                             { field: "LastProgress", title: "LastProgress", width: 100 },
                             { field: "UpdateDate", title: "UpdateDate", width: 140, template: "#= ((UpdateDate == undefined) ? \"\" : moment(UpdateDate).format('DD MMM YYYY  HH:mm:ss')) #" },
                        ];
                        break;
                    case "PMACT":
                        columns = [
                             { field: "CompanyCode", title: "CompanyCode", width: 80 },
                             { field: "BranchCode", title: "BranchCode", width: 80 },
                             { field: "InquiryNumber", title: "InqNum", width: 70 },
                             { field: "ActivityID", title: "ActID", width: 70 },
                             { field: "ActivityDate", title: "ActivityDate", width: 100, template: "#= ((ActivityDate == undefined) ? \"\" : moment(ActivityDate).format('DD MMM YYYY')) #" },
                             { field: "ActivityType", title: "ActType", width: 80 },
                             { field: "ActivityDetail", title: "ActivityDetail", width: 300 },
                             { field: "NextFollowUpDate", title: "NextFollowUpDate", width: 100, template: "#= ((NextFollowUpDate == undefined) ? \"\" : moment(NextFollowUpDate).format('DD MMM YYYY')) #" },
                             { field: "CreatedBy", title: "CreatedBy", width: 120 },
                             { field: "CreationDate", title: "CreatedDate", width: 140, template: "#= ((CreationDate == undefined) ? \"\" : moment(CreationDate).format('DD MMM YYYY  HH:mm:ss')) #" },
                             { field: "LastUpdateBy", title: "UpdatedBy", width: 120 },
                             { field: "LastUpdateDate", title: "UpdatedDate", width: 140, template: "#= ((LastUpdateDate == undefined) ? \"\" : moment(LastUpdateDate).format('DD MMM YYYY  HH:mm:ss')) #" },
                        ];
                        break;
                    case "CS3DCALL":
                        columns = [
                             { field: "CompanyCode", title: "CompanyCode", width: 100 },
                             { field: "Chassis", title: "Chassis", width: 70 },
                             { field: "CreatedBy", title: "CreatedBy", width: 140 },
                             { field: "CreatedDate", title: "CreatedDate", width: 140, template: "#= ((CreatedDate == undefined) ? \"\" : moment(CreatedDate).format('DD MMM YYYY  HH:mm:ss')) #" },
                             { field: "UpdatedBy", title: "UpdatedBy", width: 140 },
                             { field: "UpdatedDate", title: "UpdatedDate", width: 140, template: "#= ((UpdatedDate == undefined) ? \"\" : moment(UpdatedDate).format('DD MMM YYYY  HH:mm:ss')) #" },
                        ];
                        break;
                    default:
                        break;
                }

                populateDatail(e, columns, result);
            }
        })
    }

    function populateDatail(e, columns, result) {
        if (columns.length > 0) {
            if (result.data.length > 0) {
                $("<div/>").appendTo(e.detailCell).kendoGrid({
                    dataSource: { data: result.data, pageSize: 10 },
                    pageable: true,
                    columns: columns
                });
            }
            else {
                $("<div/>").appendTo(e.detailCell).kendoGrid({
                    dataSource: { data: [{ Info: "tidak ditemukan data untuk ditampilkan" }] },
                    columns: [{ field: "Info", title: "Info" }]
                });
            }
        }
    }
});