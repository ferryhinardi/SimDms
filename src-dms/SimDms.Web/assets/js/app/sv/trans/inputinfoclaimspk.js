var totalSrvAmt = 0;
var status = 'N';
var svType = '2';

$(document).ready(function () {
    var options = {
        title: "Input Information Claim(SPK)",
        xtype: "panels",
        toolbars: [
            { name: "btnCreate", text: "New", icon: "icon-file" }
        ],
        panels: [
            {
                name: "pnlRefService",
                //title: "Service Information",
                items: [
                    { name: "CausalPartNo", cls: "hide" },
                    {
                        name: "ClaimType",
                        text: "Jenis Claim",
                        required:"required",
                        type: "select",
                        items: [
                            { value: '0', text: 'Service Claim' },
                            { value: '1', text: 'Sparepart Claim' },
                        ]
                    },
                    {
                        name: "JobOrderNo",
                        cls: "span4",
                        text: "No. SPK",
                        type: "popup",
                        btnName: "btnJobOrderNo",
                        readonly: true,
                        required: "required"
                    },
                    {
                        name: "JobOrderNoEnd",
                        text: "s/d",
                        cls: "span4",
                        type: "popup",
                        btnName: "btnJobOrderNoEnd",
                        readonly: true,
                        required: "required"
                    },
                    {
                        name: "JobOrderDateBegin",
                        text: "Tgl SPK",
                        cls: "span4",
                        type: "datepicker",
                    },
                    {
                        name: "JobOrderDateEnd",
                        text: "s/d",
                        cls: "span4",
                        type: "datepicker",
                    },
                    {
                        name: "TotalRecord",
                        text: "Total Record",
                        readonly: true
                    },
                    {
                        name: "TroubleDescription",
                        text: "Keluhan / Masalah",
                        type:"textarea",
                        readonly: true
                    },
                    {
                        name: "ProblemExplanation",
                        text: "Keluhan Masalah",
                        type: "textarea",
                        readonly: true
                    },
                    {
                        type: "buttons",
                        items: [
                            { name: "btnQuery", text: "QUERY", icon: "icon-search", cls: "span4" },
                        ]
                    }
                ]
            },
            {
                name: "PnlInfoJR",
                title: "Informasi Jumlah Rupiah",
                cls: "hide",
                xtype: "table",
                tblname: "tblInfoJR",
                columns: [
                 //   { text: "Action", type: "action", width: 80 },
                    { name: "SeqNo", text: "No.", width: 80 },
                    { name: "BasicModel", text: "BasicModel", width: 100 },
                    { name: "Qty", text: "Qty", width: 110, type: "price", cls: "right" },
                    { name: "TotalSrvAmt", text: "TotalSrvAmt", width: 110, type: "price", cls: "right" }
                ]
            },
            {
                name: "PnlDataClaim",
                pnlname: "PnlDC",
                title: "Informasi Data Claim",
                xtype: "table",
                cls:"hide",
                tblname: "tblDataClaim",
               // buttons: [{ name: "btnAddDtl", text: "Add Mechanic", icon: "icon-plus" }],
                items: [
                    {
                        name: "RefferenceCode",
                        text: "Category Code",
                        cls: "span5",
                        type: "popup",
                        btnName: "btnRefferenceCode",
                        readonly: true,
                        required: "required"
                    },
                    {
                        name: "ComCode",
                        text: "Complain Code",
                        cls: "span4",
                        type: "popup",
                        btnName: "btnComCode",
                        readonly: true,
                        required: "required"
                    },
                    {
                        name: "DefCod",
                        text: "Defect Code",
                        cls: "span4",
                        type: "popup",
                        btnName: "btnDefCod",
                        readonly: true,
                        required: "required"
                    },
                    {
                        name: "OperationNoDtl",
                        text: "Operation No",
                        cls: "span4",
                        type: "popup",
                        btnName: "btnOperationNoDtl",
                        readonly: true,
                        required: "required"
                    },
                    {
                        name: "PartNo",
                        text: "Part No",
                        cls: "span4",
                        type: "popup",
                        btnName: "btnPartNo",
                        readonly: true
                    },
                    {
                        name: "TroubleDesc",
                        text: "Trouble Description",
                        type: "textarea",
                        required: "required"
                    },
                    {
                        name: "ProblemExp",
                        text: "Problem Explanation",
                        type: "textarea",
                        required: "required"
                    },
                    {
                        name: "isCbu2",
                        text: "isCbu2",
                        cls: "span4",
                        type: "switch",
                        float: "left"
                    },
                    { name: "ServiceNo", cls: "hide" },
                    { name: "InvoiceNo", cls: "hide" },
                    {
                        type: "buttons", items: [
                            { name: "btnSaveDtl", text: "Save", icon: "icon-save" },
                            { name: "btnCancelDtl", text: "Cancel", icon: "icon-undo" }
                        ]
                    },
                ],
                columns: [
                    { text: "Action", type: "edit", width: 80 },
                    { name: "SeqNo", text: "No.", width: 50 },
                    { name: "CategoryCode", text: "Category", width: 80 },
                    { name: "ComplainCode", text: "CC", width: 50 },
                    { name: "DefectCode", text: "DC", width: 50 },
                    { name: "SubletHour", text: "Sublet", width: 80, type: "numeric", cls: "right"},
                    { name: "JobOrderNo", text: "No. SPK", width: 80 },
                    { name: "JobOrderDate", text: "Tgl SPK", type: "dateTime", width: 80 },
                    { name: "CausalPartNo", text: "Causal Part", width: 100 },
                    { name: "BasicModel", text: "BasicModel", width: 80 },
                    { name: "ServiceBookNo", text: "Book service", width: 100 },
                    { name: "ChassisCode", text: "Chassis Code", width: 80 },
                    { name: "ChassisNo", text: "Chassis No", width: 100 },
                    { name: "EngineCode", text: "Engine Code", width: 100 },
                    { name: "EngineNo", text: "Engine No", width: 100 },
                    { name: "FakturPolisiDate", text: "Reg. Date", type: "dateTime",cls:"hide", width: 80 },
                    { name: "JobOrderDate", text: "Pair Date", type: "dateTime", cls: "hide", width: 100 },
                    { name: "Odometer", text: "Odometer", width: 50, type: "price", cls: "right" },
                    { name: "TotalSrvAmt", text: "Claim Amt.", width: 80, type: "price", cls: "right" },
                    { name: "TroubleDescription", cls:"hide" },
                    { name: "ProblemExplanation", cls: "hide" },
                    { name: "ServiceNo", cls:"hide" },
                    { name: "OperationNo", cls: "hide" },
                    { name: "isCbu", cls: "hide" },

                ]
            },
            {
                name: "PnlInfoPart",
                title: "Informasi Part",
                cls: "hide",
                xtype: "table",
                tblname: "tblInfoPart",
                columns: [
                 //   { text: "Action", type: "action", width: 80 },
                    //{ name: "No", text: "No.", width: 80 },
                    { name: "PartNo", text: "Part No", width: 100 },
                    //{ name: "No Part", text: "No Part", width: 110 },
                    { name: "PartQty", text: "Qty", width: 110, type: "numeric", cls: "right" },
                    { name: "PartName", text: "Part Name", width: 110 },
                ]
            }
            

        ],
    }

    var widget = new SimDms.Widget(options);

    widget.default = {};

    widget.render(function () {
        $.post('sv.api/InfoSPK/default', function (result) {
            widget.default = result;
            widget.populate(result);

        });
    });

    $("#btnJobOrderNo").on("click", function () {
        var param = $(".main .gl-widget").serializeObject();
        var lookup = widget.klookup({
            name: "lookupJobOrderNo",
            title: "Lookup SPK",
            url: "sv.api/grid/TrnNoSpk?ClaimType=" + param.ClaimType,
            serverBinding: true,
            pageSize: 10,
            filters: [
                {
                    name: "JobOrderNo",
                    text: "No SPK",
                    cls: "span4",
                },
                {
                    name: "PoliceRegNo",
                    text: "PoliceRegNo",
                    cls: "span4",
                },
                {
                    name: "ServiceBookNo",
                    text: "ServiceBookNo",
                    cls: "span4",
                },
                {
                    name: "ChassisCode",
                    text: "Chassis Code",
                    cls: "span4",
                },
                {
                    name: "BasicModel",
                    text: "Basic Model",
                    cls: "span4",
                },
                {
                    name: "EngineCode",
                    text: "Engine Code",
                    cls: "span4",
                },
            ],
            columns: [
                { field: "JobOrderNo", title: "No SPK", sWidth: "110px" },
                {
                    field: "JobOrderDate", title: "SPK Date", sWidth: "130px",
                    template: "#= (JobOrderDate == undefined) ? '' : moment(JobOrderDate).format('DD MMM YYYY') #"
                },
                { field: "PoliceRegNo", title: "PoliceRegNo", sWidth: "110px" },
                { field: "ServiceBookNo", title: "ServiceBookNo", sWidth: "110px" },
                { field: "ChassisCode", title: "Chassis Code", sWidth: "110px" },
                { field: "ChassisNo", title: "Chassis No", sWidth: "110px" },
                { field: "BasicModel", title: "Basic Model", sWidth: "110px" },
                { field: "TransmissionType", title: "Transmission Type", sWidth: "110px" },
                { field: "EngineCode", title: "Engine Code", sWidth: "110px" },
                { field: "ColorCode", title: "Color Code", sWidth: "50px" },
                { field: "Customer", title: "Customer", sWidth: "180px" },
            ],
        });
        lookup.dblClick(function (data) {
            widget.populate(data);
            $("#JobOrderNoEnd").val(data["JobOrderNo"]);
        });
    });

    $("#btnJobOrderNoEnd").on("click", function () {
        var param = $(".main .gl-widget").serializeObject();
        var lookup = widget.klookup({
            name: "lookupJobOrderNoEnd",
            title: "Lookup SPK",
            url: "sv.api/grid/TrnNoSpk?ClaimType=" + param.ClaimType,
            serverBinding: true,
            pageSize: 10,
            filters: [
                {
                    name: "JobOrderNo",
                    text: "No SPK",
                    cls: "span4",
                },
                {
                    name: "PoliceRegNo",
                    text: "PoliceRegNo",
                    cls: "span4",
                },
                {
                    name: "ServiceBookNo",
                    text: "ServiceBookNo",
                    cls: "span4",
                },
                {
                    name: "ChassisCode",
                    text: "Chassis Code",
                    cls: "span4",
                },
                {
                    name: "BasicModel",
                    text: "Basic Model",
                    cls: "span4",
                },
                {
                    name: "EngineCode",
                    text: "Engine Code",
                    cls: "span4",
                },
            ],
            columns: [
                { field: "JobOrderNo", title: "No SPK", sWidth: "110px" },
                {
                    field: "JobOrderDate", title: "SPK Date", sWidth: "130px",
                    template: "#= (JobOrderDate == undefined) ? '' : moment(JobOrderDate).format('DD MMM YYYY') #"
                },
                { field: "PoliceRegNo", title: "PoliceRegNo", sWidth: "110px" },
                { field: "ServiceBookNo", title: "ServiceBookNo", sWidth: "110px" },
                { field: "ChassisCode", title: "Chassis Code", sWidth: "110px" },
                { field: "ChassisNo", title: "Chassis No", sWidth: "110px" },
                { field: "BasicModel", title: "Basic Model", sWidth: "110px" },
                { field: "TransmissionType", title: "Transmission Type", sWidth: "110px" },
                { field: "EngineCode", title: "Engine Code", sWidth: "110px" },
                { field: "ColorCode", title: "Color Code", sWidth: "50px" },
                { field: "Customer", title: "Customer", sWidth: "180px" },
            ],
        });
        lookup.dblClick(function (data) {
          //  widget.populate(data);
            $("#JobOrderNoEnd").val(data["JobOrderNo"]);
        });
    });

    $("#btnRefferenceCode").on("click", function () {
        //  loadData('browse');
        var param = $(".main .gl-widget").serializeObject();
        widget.lookup.init({
            name: "CatCod",
            title: "Category",
            source: "sv.api/grid/SvTrnCatCode",
            sortings: [[0, "asc"]],
            columns: [
                { mData: "RefferenceCode", sTitle: "Category Code", sWidth: "110px" },
                { mData: "Description", sTitle: "Description", sWidth: "110px" },
                { mData: "DescriptionEng", sTitle: "Description(Eng.)", sWidth: "110px" },
                { mData: "Status", sTitle: "IsActive", sWidth: "80px" },
                
            ]
        });
        widget.lookup.show();
    });

    $("#btnComCode").on("click", function () {
        //  loadData('browse');
        var param = $(".main .gl-widget").serializeObject();
        widget.lookup.init({
            name: "ComCod",
            title: "Complain",
            source: "sv.api/grid/SvTrnComCode",
            sortings: [[1, "asc"]],
            columns: [
                { mData: "ComCode", sTitle: "Complain Code", sWidth: "110px" },
                { mData: "Description", sTitle: "Description", sWidth: "110px" },
                { mData: "DescriptionEng", sTitle: "Description(Eng.)", sWidth: "110px" },
                { mData: "Status", sTitle: "IsActive", sWidth: "80px" },
            ]
        });
        widget.lookup.show();
    });

    $("#btnDefCod").on("click", function () {
        //  loadData('browse');
        var param = $(".main .gl-widget").serializeObject();
        widget.lookup.init({
            name: "DefCod",
            title: "Defect",
            source: "sv.api/grid/SvTrnDefCode",
            sortings: [[0, "asc"]],
            columns: [
                { mData: "DefCod", sTitle: "Defect Code", sWidth: "110px" },
                { mData: "Description", sTitle: "Description", sWidth: "110px" },
                { mData: "DescriptionEng", sTitle: "Description(Eng.)", sWidth: "110px" },
                { mData: "Status", sTitle: "IsActive", sWidth: "80px" },

            ]
        });
        widget.lookup.show();
    });

    $("#btnOperationNoDtl").on("click", function () {
        //  loadData('browse');
        var param = $(".main .gl-widget").serializeObject();
        widget.lookup.init({
            name: "OP",
            title: "Defect",
            source: "sv.api/grid/SvTrnOpNo?ServiceNo=" + param.ServiceNo,
            sortings: [[0, "asc"]],
            columns: [
                { mData: "OperationNo", sTitle: "Operation No", sWidth: "110px" },
                { mData: "OperationHour", sTitle: "N/K", sWidth: "110px" },
                { mData: "Description", sTitle: "Description", sWidth: "110px" },

            ]
        });
        widget.lookup.show();
    });

    $("#btnPartNo").on("click", function () {
        var param = $(".main .gl-widget").serializeObject();
        var lookup = widget.klookup({
            name: "CausalPart",
            title: "Causal Part",
            url: "sv.api/grid/CausalPart?ServiceNo=" + param.ServiceNo,
            serverBinding: true,
            pageSize: 10,
            filters: [
            {
                text: "Semua Part",
                type: "controls",
                cls: "span8",
                items: [
                        {
                            name: "ShowAll", type: "select", text: "", cls: "span2", items: [
                                { value: "0", text: "Ya" },
                                { value: "1", text: "Tidak", selected: 'selected' }
                            ]
                        }
                ]
            },
            {
                name: "PartNo",
                text: "Part No",
                cls: "span3",
            },
            {
                name: "PartName",
                text: "Part Name",
                cls: "span3",
            },
            ],
            columns: [
                { field: "PartNo", title: "Part No", sWidth: "110px" },
                { field: "PartName", title: "Description", sWidth: "110px" },
            ],
        });
        lookup.dblClick(function (data) {
            widget.populate(data);
        });
    });


    widget.lookup.onDblClick(function (e, data, name) {
        widget.lookup.hide();
        switch (name) {
            case "jobtype":
                widget.populate($.extend({}, data));
                break;
            case "CatCod":
                widget.populate($.extend({}, data));
                break;
            case "ComCod":
                widget.populate($.extend({}, data));
                break;
            case "OP":
                $("#OperationNoDtl").val(data["OperationNo"]);
                break;
            case "DefCod":
                widget.populate($.extend({}, data));
                break;
            default:
                break;
        }
    });
    
    widget.onTableClick(function (icon, row, selector) {
        switch (selector.selector) {
            case "#tblDataClaim":
                switch (icon) {
                    case "edit":
                        editDetail(row);
                        break;
                    case "trash":
                        deleteDetail(row);
                        break;
                    default:
                        break;
                }
                break;
            default: break;
        }

    });


    function editDetail(row) {
        $("#PnlDC").slideDown();
        //$("#btnDlt").addClass("hide", "hide");
        var data = {
            JobOrderNo: row[6],
            CausalPartNo: row[8],

        }
        var item = {
            RefferenceCode: row[2],
            ComCode: row[3],
            DefCod: row[4],
            InvoiceNo: row[6],
            PartNo: row[8],
            TroubleDesc: row[19],
            ProblemExp: row[20],
            ServiceNo: row[21],
            OperationNoDtl: row[22],
            isCbu2: row[23] == "false" ? false : true
        }
        getInfoPart2(data);
        $("#pnlRefService").addClass('hide', 'hide');
        $("#PnlInfoPart").removeClass('hide');

        widget.populate(item, "#PnlDC");
    };

    function deleteDetail(row) {
        $("#PnlDC").slideUp();
        $("#PnlInfoPart").addClass('hide', 'hide');
        
        /*$("#btnDlt").removeClass("hide");
        var data = {
            PartNo: row[1],
            Quantity: row[2],
            RetailPrice: row[3],
            PartName: row[4],
            BillTypePart: row[6],
        }
        widget.populate(data, "#pnlRincianPanel");*/

    }

    $("#btnQuery").on("click", function (e) {
        $("#PnlDC").slideUp();
        var param = $(".main .gl-widget").serializeObject();
        if (param.JobOrderNo == '') {
            MsgBox('No. SPK Begin harus diisi!', MSG_WARNING);
        } else if (param.JobOrderNoEnd == '') {
            MsgBox('No. SPK End harus diisi!', MSG_WARNING);
        } else {
            $("#PnlInfoJR").removeClass('hide');
            $("#PnlDataClaim").removeClass('hide');
            //$("#PnlDC").slideDown();
            $("#PnlDataClaim").removeClass('hide');
            $("#PnlInfoPart").removeClass('hide');
            getJR();
            getDC(getInfoPart2);
        }

        //getInfoPart();
        
        //if (param.JobOrderNo == param.JobOrderNoEnd) {
        //    //   alert("sama");
        //    //$("#PnlDC").slideUp();
        //    getInfoPart();
        //} else {
        //    $("#PnlInfoPart").addClass("hide", "hide");
        //}
    });
   
    $("#btnCancelDtl").on("click", function (e) {
        $("#pnlRefService").removeClass('hide');
        $("#PnlDC").slideUp();
        $("#PnlDataClaim").clearForm();
        $("#PnlInfoPart").addClass("hide", "hide");
    });

    function getInfoPart() {
        var param = $(".main .gl-widget").serializeObject();
        widget.post("sv.api/InfoSPK/getTablePart", param, function (result) {
            widget.populateTable({ selector: "#tblInfoPart", data: result });
        });
    };

    function getInfoPart2(data) {
        // var param = $(".main .gl-widget").serializeObject();
        widget.post("sv.api/InfoSPK/getTablePart", data, function (result) {
            widget.populateTable({ selector: "#tblInfoPart", data: result });
        });
    };

    function getJR() {
        var param = $(".main .gl-widget").serializeObject();
        widget.post("sv.api/InfoSPK/getTableJR", param, function (result) {
            widget.populateTable({ selector: "#tblInfoJR", data: result });
        });
    };

    function getDC(callback) {
        var param = $(".main .gl-widget").serializeObject();
        widget.post("sv.api/InfoSPK/getTableDC", param, function (result) {
            //if (result.success) {
                $.each(result["list"], function (i, val) {
                    result["list"][i].isCbu = (val.isCbu == false) ? "false" : "true";
                });

                $("#TotalRecord").val(result["jum"]);
                widget.populate($.extend({}, widget.default, result["list"][0]));
                widget.populateTable({ selector: "#tblDataClaim", data: result["list"] });

                if (callback !== undefined) {
                    var dat = {
                        JobOrderNo: (result.list[0] == undefined) ? '' : result["list"][0].JobOrderNo,
                        CausalPartNo: (result.list[0] == undefined) ? '' : result["list"][0].JobOrderNo
                    }
                    callback(dat);
                }
            //}
            /*if (result == '') {
                SimDms.Error("Data not found");
            } else {
                widget.populateTable({ selector: "#tblDataClaim", data: result });
            }*/
        });
    }


    $("#btnSaveDtl").on("click", SaveData);
    $("#btnDelete").on("click", deleteData);
    $('#btnCreate').on('click', function (e) {
        clear("new");
    });
    $('#btnEdit').on('click', function (e) {
        clear("btnEdit");
    });
 
    function SaveData() {
        var isValid = $(".main form").valid();
        if (isValid) {
            var param = $(".main .gl-widget").serializeObject();
            param.IsCbu = param.isCbu2;
            widget.post("sv.api/infospk/Save", param, function (result) {
                if (result.success) {
                    SimDms.Success("data updated...");
                    $("#PnlDC").slideUp();
                    getDC();
                    var dat = {
                        JobOrderNo: param.InvoiceNo,
                        CausalPartNo: param.PartNo,
                    }
                    getInfoPart2(dat);
                }
            });
        }
        $("#pnlRefService").removeClass('hide');
    }
    function deleteData() {
        if (confirm("Apakah anda yakin???")) {
            var param = $(".main .gl-widget").serializeObject();
            widget.post("sv.api/InfoSPK/deletedata", param, function (result) {
                if (result.success) {
                    SimDms.Success("data deleted...");
                    clear("new");
                } else {
                    SimDms.Error("fail deleted...");
                }
            });
        }
    }

    $("#ClaimType").on("change", function e() {
        clear("typeonchange");
    });
    $("#btnJobOrderNo").attr("disabled", "disabled");
    $("#btnJobOrderNoEnd").attr("disabled", "disabled");
    $("#btnQuery").attr("disabled", "disabled");

    function clear(p) {
        if (p == "clear") {
            $("#btnEdit").addClass("hide");
            $("#btnDelete").addClass("hide");
        } else if (p == "dbclick") {
            $("#btnEdit").removeClass('hide');
            $("#btnDelete").removeClass('hide');
        } else if (p == "new") {
            $("#pnlRefService").removeClass('hide');
            $("#PnlInfoJR").addClass("hide", "hide");
            $("#PnlDataClaim").addClass("hide", "hide");
            $("#PnlInfoPart").addClass("hide", "hide");
            $("#btnJobOrderNo").attr("disabled", "disabled");
            $("#btnJobOrderNoEnd").attr("disabled", "disabled");
            $("#btnQuery").attr("disabled", "disabled");
            clearData(); 
          //  $("#PnlTabel").addClass("hide");
            $("#btnEdit").addClass("hide");
            $("#btnDelete").addClass("hide");
        //    $("#Description").attr('readonly', 'readonly');


        } else if (p == "btnEdit") {
            $("#btnSave").removeClass('hide');
        }
        else if (p == "typeonchange") {
            var k = $('[name=ClaimType]').val();
            $('#ClaimType').removeAttr('disabled');
            $('#btnJobOrderNo').removeAttr('disabled');
            $('#btnJobOrderNoEnd').removeAttr('disabled');
            $('#btnQuery').removeAttr('disabled');
            if (k == '') {
                $("#ClaimType").attr("readonly", "readonly");
                $("#btnJobOrderNo").attr("disabled", "disabled");
                $("#btnJobOrderNoEnd").attr("disabled", "disabled");
                $("#btnQuery").attr("disabled", "disabled");
            } else {
                $('#ClaimType').removeAttr('readonly');
                $('#btnJobOrderNo').removeAttr('disabled');
                $('#btnJobOrderNoEnd').removeAttr('disabled');
                $('#btnQuery').removeAttr('disabled');
            }
        }
    }
  
    function clearData() {
        widget.clearForm();
        widget.post("sv.api/InfoSPK/default", function (result) {
            widget.default = $.extend({}, result);
            widget.populate(widget.default);
        });
    }
});