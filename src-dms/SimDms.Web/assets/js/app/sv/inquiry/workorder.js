$(document).ready(function () {
    var options = {
        title: "Surat Perintah Kerja",
        xtype: "panels",
        toolbars: [
            { name: "btnNew", text: "New", icon: "icon-file" },
            { name: 'btnBrowse', text: 'Browse', icon: 'icon-search' }
        ],
        panels: [
            {
                name: "pnlServiceInfo",
                title: 'Service Information',
                items: [
                    { name: 'JobOrderNo', text: 'SPK No.', cls: 'span4', readonly: true },
                    { name: "JobOrderDate", text: "SPK Date", cls: "span4", type:"datetimepicker",readonly: true },
                    { name: "ServiceStatusDesc", text: "Status", readonly: true, type: 'textarea' }

                ]
            },
            {
                xtype: "tabs",
                name: "tabpage1",
                items: [
                    { name: "CustVehicle", text: "Customer & Vehicle" },
                    { name: "TaskPart", text: "Task & Part" },
                ]
            },
            {
                name: "CustVehicle",
                title: "Customer & Vehicle",
                cls: "tabpage1 CustVehicle",
                items: [
                    { name: "PoliceRegNo", text: "Police Reg No", cls: "span4",  btnName: "btnPoliceRegNo", readonly: true },
                    { name: "ServiceBookNo", text: "Service Book No", cls: "span4", readonly: true },
                    { name: "BasicModel", text: "Basic Model", cls: "span4", readonly: true },
                    { name: "TransmissionType", text: "Trans Type", cls: "span4", readonly: true },
                    { name: "ChassisCode", text: "Chassis Code", cls: "span4", readonly: true },
                    { name: "ChassisNo", text: "Chassis No", cls: "span4", readonly: true },
                    { name: "EngineCode", text: "Engine Code", cls: "span4", readonly: true },
                    { name: "EngineNo", text: "Engine No", cls: "span4", readonly: true },
                    { name: "ColorCodeDesc", text: "Color", cls: "span4", readonly: true },
                    { name: "Odometer", text: "Odometer", cls: "span4 number", readonly: true },
                    {
                        text: "Customer",
                        type: "controls",
                        items: [
                            { name: "CustomerCode", cls: "span2", placeHolder: "Cust Code", readonly: true },
                            { name: "CustomerName", cls: "span6", placeHolder: "Cust Name", readonly: true },
                        ]
                    },
                    { name: "CustAddr1", text: "Address", maxlength: 100, readonly: true },
                    { name: "CustAddr2", text: "", maxlength: 100, readonly: true },
                    { name: "CustAddr3", text: "", maxlength: 100, readonly: true },
                    {
                        text: "City",
                        type: "controls",
                        items: [
                            { name: "CityCode", cls: "span2", placeHolder: "City Code", readonly: true },
                            { name: "CityName", cls: "span6", placeHolder: "City Name", readonly: true },
                        ]
                    },
                ]
            },
            {
                cls: "tabpage1 CustVehicle",
                title: "Contract & Club",
                items: [
                    { name: "ContractNo", text: "Contract No", placeHolder: "No", cls: "span2", readonly: true },
                    { name: "ContractExpired", text: "Expired", cls: "span3", readonly: true },
                    { name: "ContractStatus", text: "Status", cls: "span3", readonly: true },
                    { name: "ClubNo", text: "Club No", placeHolder: "No", cls: "span2", readonly: true },
                    { name: "ClubExpired", text: "Expired", cls: "span3", readonly: true },
                    { name: "ClubStatus", text: "Status", cls: "span3", readonly: true },
                ]
            },
            {
                cls: "tabpage1 CustVehicle",
                title: "Customer Bill",
                items: [
                    { name: "InsurancePayFlag", text: "Insurance", cls: "span4", type: "switch", float: "left", readonly: true },
                    { name: "InsuranceOwnRisk", text: "Own Risk", cls: "span4 number", readonly: true },
                    { name: "InsuranceNo", text: "Polis No", cls: "span4", readonly: true },
                    { name: "InsuranceJobOrderNo", text: "Insurance No", cls: "span4", readonly: true },
                    {
                        text: "Customer",
                        type: "controls",
                        items: [
                            { name: "CustomerCodeBill", cls: "span2", placeHolder: "Cust Code", readonly: true },
                            { name: "CustomerNameBill", cls: "span6", placeHolder: "Cust Name", readonly: true },
                        ]
                    },
                    { name: "CustAddr1Bill", text: "Address", maxlength: 100, readonly: true },
                    { name: "CustAddr2Bill", text: "", readonly: true },
                    { name: "CustAddr3Bill", text: "", readonly: true },
                    {
                        text: "City",
                        type: "controls",
                        items: [
                            { name: "CityCodeBill", cls: "span2", placeHolder: "City Code", readonly: true },
                            { name: "CityNameBill", cls: "span6", placeHolder: "City Name", readonly: true },
                        ]
                    },
                    {
                        text: "Phone",
                        type: "controls",
                        items: [
                            { name: "PhoneNo", cls: "span2", placeHolder: "Telephone", readonly: true },
                            { name: "FaxNo", cls: "span3", placeHolder: "Fax", readonly: true },
                            { name: "HPNo", cls: "span3", placeHolder: "HP", readonly: true },
                        ]
                    },
                    {
                        text: "Discount (%)",
                        type: "controls",
                        items: [
                            { name: "LaborDiscPct", cls: "span2 number", placeHolder: "Task", readonly: true },
                            { name: "PartsDiscPct", cls: "span3 number", placeHolder: "Part", readonly: true },
                            { name: "MaterialDiscPct", cls: "span3 number", placeHolder: "Material", readonly: true },
                        ]
                    },
                    { name: "IsPPN", text: "PPN", type: "switch", float: "left", readonly: true },
                ]
            },
            {
                cls: "tabpage1 TaskPart",
                title: "Job Request",
                items: [
                    { name: "ServiceRequestDesc", text: "Job Request", type: "textarea", readonly: true },
                    {
                        text: "Job Type",
                        type: "controls",
                        items: [
                            { name: "JobType", cls: "span2", placeHolder: "Code",  readonly: true },
                            { name: "JobTypeDesc", cls: "span6", placeHolder: "Description", readonly: true },
                        ]
                    },
                    { name: "ConfirmChangingPart", text: "Allow Change Part", type: "switch", float: "left", readonly: true },
                    {
                        text: "Service Advisor (SA)",
                        type: "controls",
                        items: [
                            { name: "ForemanID", cls: "span2", placeHolder: "Code",  readonly: true },
                            { name: "ForemanName", cls: "span6", placeHolder: "Name", readonly: true },
                        ]
                    },
                    {
                        text: "Foreman (FM)",
                        type: "controls",
                        items: [
                            { name: "MechanicID", cls: "span2", placeHolder: "Code",  readonly: true },
                            { name: "MechanicName", cls: "span6", placeHolder: "Name", readonly: true },
                        ]
                    },
                    {
                        text: "Estimate Finish",
                        type: "controls",
                        items: [
                            { name: "EstimateFinishDate", cls: "span2", readonly: true },
                        ]
                    },
                    { name: "IsSparepartClaim", text: "Sparepart Claim?", type: "switch", cls: "span4", float: "left" , readonly: true}
                ]
            },
            {
                cls: "tabpage1 TaskPart summary",
                title: "Total Service Amount",
                items: [
                    { name: "LaborDppAmt", text: "DPP - Jasa", cls: "span6 number", readonly: true },
                    { name: "PartsDppAmt", text: "DPP - Part", cls: "span6 number", readonly: true },
                    { name: "MaterialDppAmt", text: "DPP - Material", cls: "span6 number", readonly: true },
                    { name: "TotalDppAmt", text: "Total DPP", cls: "span6 indent number", readonly: true },
                    { name: "TotalPpnAmt", text: "Total PPN", cls: "span6 indent number", readonly: true },
                    { name: "TotalSrvAmt", text: "Total Amount", cls: "span6 indent number", readonly: true },
                ]
            },
            {
                cls: "tabpage1 TaskPart",
                title: "Detail Task / Part",
                xtype: "table",
                tblname: "tblTaskPart",
                columns: [
                    { name: "BillType", cls: "hide" },
                    { name: "ItemType", cls: "hide" },
                    { text: "Action", type: "action", width: 80 },
                    { name: "BillTypeDesc", text: "Ditanggung Oleh" },
                    { name: "TypeOfGoodsDesc", text: "Jenis Item" },
                    { name: "TaskPartNo", text: "Task/Part" },
                    { name: "TaskPartDesc", cls: "hide" },
                    { name: "OprHourDemandQty", text: "Qty/NK", cls: "right", width: 80 },
                    { name: "Price", text: "Price", cls: "right", width: 110 },
                    { name: "QtyAvail", text: "Available", cls: "right", width: 90 },
                    { name: "DiscPct", text: "Discount", cls: "right", width: 80 },
                    { name: "PriceNet", text: "Net Price", cls: "right", width: 120 },
                    { name: "SeqNo", cls: "hide" },
                ]
            },
        ]
    }

    var widget = new SimDms.Widget(options);
    widget.default = {};
    widget.render(function () {
        $.post('sv.api/workorder/default', function (result) {
            widget.default = result;
            widget.populate(result);
        });
        $("[type=radio]").attr("disabled", "disabled");
        $('#JobOrderDate').attr('disabled', 'disabled');
        //$(".number").val(0.00)
    });

    $('#btnNew').on('click', function (e) {
        widget.clearForm();
        $.post('sv.api/workorder/default', function (result) {
            widget.default = result;
            widget.populate(result);
        });
    });
    $('#btnBrowse').on('click', browseJobOrder);

    function browseJobOrder() {
        var lookup = widget.klookup({
            name: "trnSPK",
            title: "Perawatan Kendaraan Lookup",
            url: "sv.api/grid/kjoborders",
            serverBinding: true,
            pageSize: 12,
            sort: ({field: 'JobOrderNo', dir: 'desc'}),
            filters: [
                { text: "SPK No.", name: "fltSPKNo", cls: "span4" },
                { text: "Service Book No.", name: "fltServiceBookNo", cls: "span4" },
                { text: "Chassis No.", name: "fltChassisNo", cls: "span4" },
                { text: "Police No.", name: "fltPoliceNo", cls: "span4" },
                { text: "Customer", name: "fltCustomer", cls: "span4" },
                { text: "Engine No.", name: "fltEngineNo", cls: "span4" },
            ],
            columns: [
                { field: "JobOrderNo", title: "SPK No.", width: 150 },
                {
                    field: "JobOrderDate", title: "SPK Date", sWidth: "130px",
                    template: "#= (JobOrderDate == undefined) ? '' : moment(JobOrderDate).format('DD MMM YYYY') #"
                },
                { field: 'PoliceRegNo', title: 'Police No.' },
                { field: 'ServiceBookNo', title: 'Service Book No.' },
                { field: 'BasicModel', title: 'Basic Model' },
                { field: 'TransmissionType', title: 'Transmission Type' },
                { field: 'ChassisCode', title: 'Chassis Code' },
                { field: 'EngineCode', title: 'Engine Code' },
                { field: 'ColorName', title: 'Color' },
                { field: 'Customer', title: 'Customer' },
                //{ field: 'CustomerBill', title: 'Customer Bill' },
                //{ field: 'AddressBill', title: 'Address' },
                //{ field: 'NPWPNo', title: 'NPWP No.' },
                //{ field: 'PhoneNo', title: 'Phone No.' },
                //{ field: 'HPNo', title: 'HP No.' },
                //{ field: 'Odometer', title: 'Odometer' },
                //{ field: 'JobType', title: 'Job Type' },
                //{ field: 'ForemanID', title: 'Foreman' },
                //{ field: 'MechanicID', title: 'Mechanic' },
                { field: 'ServiceStatus', title: 'Status', template:"#= (ServiceStatus == 0) ? 'Aktif' : 'Tidak Aktif '#" },
            ],
        });

        lookup.dblClick(function (data) {
            widget.post("sv.api/workorder/get", { ServiceNo: data.ServiceNo }, function (result) {
                if (result.success) {

                    populateData(result);
                } else { }
            });
        });
    };

    function populateData(result) {
        var data = result.data || {};
        var header = {
            ServiceNo: data.ServiceNo,
            JobOrderNo: data.JobOrderNo,
            JobOrderDate: data.JobOrderDate,
            EstimationNo: data.EstimationNo,
            EstimationDate: data.EstimationDate,
            BookingNo: data.BookingNo,
            BookingDate: data.BookingDate,
            ServiceStatusDesc: data.ServiceStatusDesc,
            CustomerCode: data.CustomerCode,
            CustomerName: data.CustomerName,
            CustAddr1: data.CustAddr1,
            CustAddr2: data.CustAddr2,
            CustAddr3: data.CustAddr3,
            PoliceRegNo: data.PoliceRegNo,
            CityCode: data.CityCode,
            CityName: data.CityName,
            PhoneNo: data.PhoneNo,
            FaxNo: data.FaxNo,
            HPNo: data.HPNo,
            ServiceBookNo: data.ServiceBookNo,
            BasicModel: data.BasicModel,
            TransmissionType: data.TransmissionType,
            ChassisCode: data.ChassisCode,
            ChassisNo: data.ChassisNo,
            EngineCode: data.EngineCode,
            EngineNo: data.EngineNo,
            ColorCode: data.ColorCode,
            ColorCodeDesc: data.ColorCodeDesc,
            Odometer: data.Odometer,
            ContractNo: data.ContractNo,
            ContractExpired: ((data.ContractEndPeriod || "").length > 0) ? moment(data.ContractEndPeriod).format(SimDms.dateFormat) : "",
            ContractStatus: data.ContractStatusDesc,
            ClubNo: data.ClubCode,
            ClubExpired: ((data.ClubEndPeriod || "").length > 0) ? moment(data.ClubEndPeriod).format(SimDms.dateFormat) : "",
            ClubStatus: data.ClubStatusDesc,
            InsurancePayFlag: data.InsurancePayFlag,
            InsuranceOwnRisk: data.InsuranceOwnRisk,
            InsuranceNo: data.InsuranceNo,
            InsuranceJobOrderNo: data.InsuranceJobOrderNo,
            CustomerCodeBill: data.CustomerCodeBill,
            CustomerNameBill: data.CustomerNameBill,
            CustAddr1Bill: data.CustAddr1Bill,
            CustAddr2Bill: data.CustAddr2Bill,
            CustAddr3Bill: data.CustAddr3Bill,
            CityCodeBill: data.CityCodeBill,
            CityNameBill: data.CityNameBill,
            LaborDiscPct: data.LaborDiscPct,
            PartsDiscPct: data.PartsDiscPct,
            MaterialDiscPct: data.MaterialDiscPct,
            IsPPN: ((data.TaxCode || "PPN") === "PPN" ? true : false),
            ServiceRequestDesc: data.ServiceRequestDesc,
            JobType: data.JobType,
            JobTypeDesc: data.JobTypeDesc,
            ConfirmChangingPart: data.ConfirmChangingPart,
            ForemanID: data.ForemanID,
            ForemanName: data.ForemanName,
            MechanicID: data.MechanicID,
            MechanicName: data.MechanicName,
            EstimateFinishDate: data.EstimateFinishDate,
            LaborDppAmt: data.LaborDppAmt,
            PartsDppAmt: data.PartsDppAmt,
            MaterialDppAmt: data.MaterialDppAmt,
            TotalDppAmt: data.TotalDppAmt,
            TotalPpnAmt: data.TotalPpnAmt,
            TotalSrvAmt: data.TotalSrvAmt,
            IsSparepartClaim: data.IsSparepartClaim
        }
    
        widget.populate(header);
        widget.populateTable({ selector: "#tblTaskPart", data: result.list });
    }
});