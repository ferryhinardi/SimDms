$(document).ready(function () {
    var edit = false;
    var seq;
    var tblpartcount = 0;

    var options = {
        title: "Input Warranty Claim",
        xtype: "panels",
        toolbars: [
            { name: "btnNew", text: "New", icon: "icon-file"},
            { name: 'btnBrowse', text: 'Browse', icon: 'icon-search' },
            { name: 'btnSave', text: 'Save', icon: 'icon-save' },
            { name: 'btnEdit', text: 'Edit', icon: 'icon-edit', cls: 'hide' },
            { name: "btnPrint", text: "Print", icon: "icon-print" }
        ],
        panels: [
            {
                name: "pnlSubDealerBranch",
                title: "Sub Dealer / Branch",
                items: [
                    { name: "GenerateNo", text: "Claim No.", placeHolder: "CLA/XX/YYYYYY", cls: "span4", type: "popup", required: true, readonly: true },
                    { name: "GenerateDate", text: "Claim Date", cls: "span4", type: "datepicker" },
                    { name: "SenderDealerCode", text: "Dealer Code", cls: "span4 Full", type: "popup", required: true },
                    { name: "SenderDealerName", text: "Dealer Name", cls: "span8", readonly: true },
                    { name: "RefferenceNo", text: "Refference No.", cls: "span4", required: true },
                    { name: "RefferenceDate", text: "Refference Date", cls: "span4", type: "datepicker" },
                    { name: "FPJNo", text: "Tax Invoice No.", cls: "span4", required: true },
                    { name: "FPJDate", text: "Tax Invoice Date", cls: "span4", type: "datepicker" },
                    { name: "FPJGovNo", text: "Series of Tax", cls: "span4", required: true }
                ]
            },
            {
                title: "Total Amount Info",
                xtype: "table",
                tblname: "tblTotalInfo",
                columns: [
                    { name: "No", text: "No.", width: 25 },
                    { name: "BasicModel", text: "Model" },
                    { name: "Total", text: "Total" }
                ]
            },
            {
                title: "Claim Data Info",
                xtype: "table",
                pnlname: "pnlInsertClaimInfo",
                tblname: "tblClaimDataInfo",
                selectable: true,
                buttons: [
                    { name: "btnAddClaimDtl", text: "New", icon: "icon-plus" },
                    { name: "btnEditClaimDtl", text: "Edit", icon: "icon-edit"},
                    { name: "btnDeleteClaimDtl", text: "Delete", icon: "icon-remove" }
                ],
                items: [
                    { name: "GenerateSeq", cls:"hide"},
                    {
                        text: "Category Code",
                        type: "controls",
                        required: true,
                        items: [
                            { name: "CategoryCode", cls: "span2", placeHolder: "Category Code", readonly: true, type: "popup" ,required: true},
                            { name: "CategoryName", cls: "span6", placeHolder: "Category Name", readonly: true },
                        ]
                    },
                    { name: "IssueNo", text: "Issue No.", cls: "span4", readonly: true },
                    { name: "IssueDate", text: "Issue Date", cls: "span4", type: "datepicker" },
                    { name: "ServiceBookNo", text: "Service Book No.", cls: "span4", required: true },
                    { name: "BasicModel", text: "Basic Model", cls: "span4", type: "popup", readonly: true, required: true },
                    { name: "ChassisCode", text: "Chassis Code", cls: "span4", required: true },
                    { name: "ChassisNo", text: "Chassis No.", cls: "span4 number", required: true },
                    { name: "EngineCode", text: "Engine Code", cls: "span4", required: true },
                    { name: "EngineNo", text: "Engine No", cls: "span4 number", required: true },
                    { name: "RegisteredDate", text: "Registered Date", cls: "span4", type: "datepicker" },
                    { name: "RepairedDate", text: "Repaired Date", cls: "span4", type: "datepicker" },
                    {
                        text: "CC",
                        type: "controls",
                        required: true,
                        items: [
                            { name: "ComplainCode", cls: "span2", placeHolder: "Complain Code", readonly: true, type: "popup", required: true},
                            { name: "ComplainDesc", cls: "span6", placeHolder: "Description", readonly: true },
                        ]
                    },
                    {
                        text: "DC",
                        type: "controls",
                        required: true,
                        items: [
                            { name: "DefectCode", cls: "span2", placeHolder: "Defect Code", readonly: true, type: "popup", required: true },
                            { name: "DefectDesc", cls: "span6", placeHolder: "Description", readonly: true },
                        ]
                    },
                    { name: "SubletHour", text: "Sublet", cls: "span4 number" },
                    { name: "Odometer", text: "Odometer", cls: "span4", required: true },
                    {
                        text: "Basic Code",
                        type: "controls",
                        required: true,
                        items: [
                            { name: "BasicCode", cls: "span2", placeHolder: "Basic Code", readonly: true, type: "popup" ,required: true },
                            { name: "Description", cls: "span6", placeHolder: "Description", readonly: true },
                        ]
                    },
                    { name: "VarCom", text: "Var / COM", readonly: true, cls: "span4" },
                    { name: "isCBU", text: "Is CBU", cls: "span2", type: "switch" },
                    { name: "TroubleDescription", text: "Trouble", type: "textarea" },
                    { name: "ProblemExplanation", text: "Explain", type: "textarea" },
                    { name: "OperationNo" , cls:"hide"},
                    { name: "OperationHour", cls:"hide"},
                    {
                        type: "buttons", items: [
                            { name: "btnSaveClaimDtl", text: "Save", icon: "icon-save" },
                            { name: "btnCancelClaimDtl", text: "Cancel", icon: "icon-undo" }
                        ]
                    },
                ],
                columns: [
                    { name: "GenerateSeq", cls: "hide"},
                    { name: "No", text: "No" },
                    { name: "CategoryCode", text: "Category" },
                    { name: "IssueNo", text: "Issue No." },
                    { name: "IssueDate", text: "Issue Date", type: "date" },
                    { name: "ServiceBookNo", text: "Service Book No." },
                    { name: "ChassisCode", text: "Chassis Code" },
                    { name: "ChassisNo", text: "Chassis No." },
                    { name: "EngineCode", text: "Engine Code" },
                    { name: "EngineNo", text: "Engine No" },
                    { name: "isCbu", text: "CBU" },
                    { name: "BasicModel", text: "Basic Model" },
                    { name: "RegisteredDate", text: "Registered Date", type: "date" },
                    { name: "RepairedDate", text: "Repaired Date", type: "date" },
                    { name: "Odometer", text: "Odometer" },
                    { name: "ComplainCode", text: "CC" },
                    { name: "DefectCode", text: "DC" },
                    { name: "SubletHour", text: "Sublet" },
                    { name: "BasicCode", text: "Basic Code" },
                    { name: "VarCom", text: "VarCom" },
                    { name: "OperationHour", text: "Hours" },
                    { name: "TroubleDescription", cls: 'hide' },
                    { name: "ProblemExplanation", cls: 'hide' },
                ]
            },
            {
                name: "pnlTroubleInfo",
                title: "Trouble Information",
                items: [
                      { name: "TroubleDescription", text: "Trouble", type: "textarea" },
                      { name: "ProblemExplanation", text: "Explain", type: "textarea" }
                ]
            },
            {
                title: "Part Info",
                xtype: "table",
                pnlname: "pnlInsertPartInfo",
                tblname: "tblPartInfo",
                buttons: [{ name: "btnAddPartDtl", text: "Add New Part Info", icon: "icon-plus" }],
                items: [
                    { name: "PartSeq", cls: "hide" },
                    { name: "IsCasual", text: "IsCasual", type: "switch", cls: "span2 full" },
                    { name: "PartNo", text: "Part No.", cls: "span4 full", type: "popup", required: true },
                    { name: "Quantity", text: "Quantity", cls: "span4 number full", required: true },
                    { name: "ClaimPrice", text: "Price", cls: "span4 number", readonly: true },
                    {
                        type: "buttons", items: [
                            { name: "btnSavePartDtl", text: "Save", icon: "icon-save" },
                            { name: "btnCancelPartDtl", text: "Cancel", icon: "icon-undo" }
                        ]
                    },
                ],
                columns: [
                    { name: "PartSeq", cls: "hide" },
                    { text: "Action", type: "action", width: 25 },
                    { name: "No", text: "No.", width: 20 },
                    { name: "IsCausal", text: "Casual Part No.", width: 20 },
                    { name: "PartNo", text: "Part No.", width: 50 },
                    { name: "Quantity", text: "Qty", width: 50, cls: "right number" },
                    { name: "PartName", text: "Part Name", width: 250 },
                    { name: "UnitPrice", cls:'hide'}
                ]
            }
        ]
    }

    var widget = new SimDms.Widget(options);
    widget.default = {};

    widget.render(function () {
        widget.post('sv.api/inputwarrantyclaim/default', function (result) {
            widget.default = result;
            widget.populate(result);
        });
        $('#btnAddClaimDtl,#btnEditClaimDtl,#btnDeleteClaimDtl,#btnAddPartDtl').attr('disabled', 'disabled');
    });

    $('#btnAddClaimDtl').on('click', function () {
        addClaimDetail();

        var data = {
            GenerateSeq: '',
            CategoryCode: '',
            IssueNo: '',
            IssueDate: new Date(),
            ServiceBookNo: '',
            ChassisCode: '',
            ChassisNo: '',
            EngineCode: '',
            EngineNo: '',
            isCbu: false,
            BasicModel: '',
            RegisteredDate: new Date(),
            RepairedDate: new Date(),
            Odometer: 0,
            ComplainCode: '',
            ComplainDesc: '',
            DefectCode: '',
            DefectDesc: '',
            SubletHour: '',
            BasicCode: '',
            VarCom: '',
            OperationHour: '',
            TroubleDescription: '',
            ProblemExplanation: ''
        }

        widget.populate(data, "#pnlInsertClaimInfo")
    });

    $('#btnAddPartDtl').on('click', function () {
        addPartDetail();
        var data = {
            PartSeq: '',
            IsCasual: false,
            PartNo: '',
            Quantity: 0,
            ClaimPrice: 0
        }
        widget.populate(data, "#pnlInsertPartInfo")
        edit = false;
    });

    $('#btnCancelClaimDtl').on('click', ClaimDetail);

    $('#btnCancelPartDtl').on('click', PartDetail);

    $('#btnBrowse, #btnGenerateNo').on('click', browseWarranty);

    $('#btnSenderDealerCode').on('click', browseSenderDealer);

    $('#btnCategoryCode').on('click', browseCategory);

    $('#btnBasicModel').on('click', browseBasicModel);

    $('#btnComplainCode').on('click', browseComplain);

    $('#btnDefectCode').on('click', browseDefect);

    $('#btnBasicCode').on('click', function (e) {
        var data = $(".main .gl-widget").serializeObject();
        var basicmodel = data.BasicModel
        if (basicmodel == "") {
            widget.alert("Silahkan input Basic Model terlebih dahulu!!!");
        }
        else {
            browseBasicCode(basicmodel);
        }
    });

    $('#btnPartNo').on('click', browsePartNo);

    $('#btnNew').on('click', New);

    $('#btnSave').on('click', function (e) {
        if ($(".main form").valid()) {
            var data = $(".main .gl-widget").serializeObject();
            widget.post('sv.api/inputwarrantyclaim/save', data, function (result) {
                if (result.success) {
                    widget.showNotification("Data Saved");
                    $('#GenerateNo').val(result.data.GenerateNo);
                }
                else {
                    widget.alert(result.message);
                }
            });
        }
        else {
            MsgBox('Ada informasi yang belum lengkap', MSG_WARNING);
        }
    });

    $('#btnPrint').on('click', PrintClaim);

    $('#btnSaveClaimDtl').on('click', function (e) {
        var data = $(".main .gl-widget").serializeObject();

        if (data.Odometer == 0)
        {
            widget.alert("Odometer tidak boleh kurang atau sama dengan 0");
        }
        else
        {
            widget.post('sv.api/inputwarrantyclaim/saveclaim', data, function (result) {
                if (result.success) {
                    ClaimInfoApp(data.GenerateNo, null);
                    widget.clearForm("#pnlInsertClaimInfo");
                    ClaimDetail();
                }
            });
        }
    });

    $('#btnEditClaimDtl').on('click', editClaimDetail);

    $('#btnDeleteClaimDtl').on('click', function (e) {
        if (tblpartcount > 0) {
            widget.alert("Claim ini masih ada detail part, data tidak bisa dihapus!");
        }
        else {
            var data = $(".main .gl-widget").serializeObject();
            $.post('sv.api/inputwarrantyclaim/deleteclaim', data, function (result) {
                if (result.success) {
                    widget.showNotification(result.message);
                    ClaimInfoApp($('#GenerateNo').val(), null);
                }
                else {
                    widget.alert(result.message);
                }
            });
        }
    });

    $('#btnSavePartDtl').on('click', function (e) {
        if ($('#PartNo').val() == "") {
            widget.alert("Part No. Harus diisi!!!");
        }
        else {
            var data = $(".main .gl-widget").serializeObject();
            var datas = { model: data, tblpart: tblpartcount };

            if (edit == false) {
                $.post('sv.api/inputwarrantyclaim/validatepart', data, function (result) {
                    if (result > 0) {
                        widget.confirm("Claim Part sudah ada didatabase, tetap ditambahkan?", function (con) {
                            if (con == 'Yes') {
                                SavePart(datas);
                            }
                            else {
                            }
                        });
                    }
                    else {
                        SavePart(datas);
                    }
                });
            }
            else {
                SavePart(datas);
            }
        }
    });

    function SavePart(datas) {
        $.ajax({
            type: 'POST',
            data: JSON.stringify(datas),
            url: 'sv.api/inputwarrantyclaim/savepart',
            contentType: "application/json; charset=utf-8",
            dataType: "json",
            async: false,
            success: function (result) {
                if (result.success) {
                    //New();
                    PartInfo(result.data.GenerateNo, seq);
                    PartDetail();
                }
            }
        });
    }

    widget.lookup.onDblClick(function (e, data, name) {
        widget.lookup.hide();
        switch (name) {
            case "CategoryList":
                $('#CategoryCode').val(data.RefferenceCode);
                $('#CategoryName').val(data.DescriptionEng);
                break;
            case "BasicModelList":
                widget.populate(data);
                break;
            case "ComplainList":
                $('#ComplainCode').val(data.RefferenceCode);
                $('#ComplainDesc').val(data.DescriptionEng);
                break;
            case "DefectList":
                $('#DefectCode').val(data.RefferenceCode);
                $('#DefectDesc').val(data.DescriptionEng);
                break;
            case "BasicCodeList":
                widget.populate(data);
                break;
            case "PartNoList":
                widget.populate(data);
                break;
            default:
                break;
        }
    });

    function browseSenderDealer() {
        var lookup = widget.klookup({
            name: "SenderDealerList",
            title: "Dealer Lookup",
            url: "sv.api/grid/senderdealers",
            serverBinding: true,
            pageSize: 12,
            sort: [
                { field: 'CustomerName', dir: 'asc' },
                { field: 'Status', dir: 'asc' }
            ],
            columns: [
                { field: "CustomerCode", title: "Kode Dealer", width: 160 },
                { field: "CustomerName", title: "Nama Dealer", width: 200 },
                { field: "CustomerAbbrName", title: "Abbreviation Name", width: 200 },
                { field: "Status", title: "Status", width: 130 },
            ],
        });
        lookup.dblClick(function (data) {
            $('#SenderDealerCode').val(data.CustomerCode);
            $('#SenderDealerName').val(data.CustomerName);
        });
    };

    function browseCategory()
    {
        widget.lookup.init({
            name: "CategoryList",
            title: "Category Lookup",
            source: "sv.api/grid/category",
            sortings: [[0, "asc"]],
            columns: [
			    { mData: "RefferenceCode", sTitle: "Category Code" ,sWidth:"100px"},
                { mData: 'DescriptionEng', sTitle: 'Description' }
            ]
        });
        widget.lookup.show();
    }

    function browseBasicModel() {
        widget.lookup.init({
            name: "BasicModelList",
            title: "Master Model",
            source: "sv.api/grid/basicmodelclaim",
            columns: [
                { mData: "BasicModel", sTitle: "Basic Model", sWidth: "100px" },
            ]
        });
        widget.lookup.show();
    }

    function browseComplain() {
        widget.lookup.init({
            name: "ComplainList",
            title: "Complain Code Lookup",
            source: "sv.api/grid/complain",
            sortings: [[0, "asc"]],
            columns: [
			    { mData: "RefferenceCode", sTitle: "Category Code", sWidth: "100px" },
                { mData: 'DescriptionEng', sTitle: 'Description' }
            ]
        });
        widget.lookup.show();
    }

    function browseDefect() {
        widget.lookup.init({
            name: "DefectList",
            title: "Defect Code Lookup",
            source: "sv.api/grid/defect",
            sortings: [[0, "asc"]],
            columns: [
			    { mData: "RefferenceCode", sTitle: "Category Code", sWidth: "100px" },
                { mData: 'DescriptionEng', sTitle: 'Description' }
            ]
        });
        widget.lookup.show();
    }

    function browseBasicCode(basicmodel) {
        widget.lookup.init({
            name: "BasicCodeList",
            title: "Basic Code Lookup",
            source: "sv.api/grid/basiccode?basicmodel=" + basicmodel,
            columns: [
			    { mData: "BasicCode", sTitle: "Basic Code" },
                { mData: 'VarCom', sTitle: 'Var / Com' },
                { mData: 'LaborPrice', sTitle: 'Labor Price' },
                { mData: 'Description', sTitle: 'Description' },
            ]
        });
        widget.lookup.show();
    }

    function browsePartNo() {
        widget.lookup.init({
            name: "PartNoList",
            title: "Master Part Lookup",
            source: "sv.api/grid/partnoclaim",
            sortings: [[1, "asc"]],
            columns: [
			    { mData: "PartNo", sTitle: "Part No" },
                { mData: 'PartName', sTitle: 'Part Name' },
                { mData: 'RetailPriceInclTax', sTitle: 'Retail Price Include Tax'},
                { mData: 'RetailPrice', sTitle: 'Retail Price' },
                { mData: 'ClaimPrice', sTitle: 'Claim Price' }
            ]
        });
        widget.lookup.show();
    }

    function browseWarranty() {
      var lookup = widget.klookup({
            name: "trnWarranty",
            title: "Transaction - Warranty Lookup",
            url: "sv.api/grid/claim?SourceData=1",
            serverBinding: true,
            pageSize: 12,
            filters: [
                {
                    text: "Status",
                    type: "controls",
                    items: [
                        {
                            name: "fltStatus", type: "select", text: "Status", cls: "span2", items: [
                                { value: "", text: "PILIH SEMUA" },
                                { value: "0", text: "BELUM POSTING" },
                                { value: "1", text: "SUDAH POSTING" },
                                { value: "2", text: "DRAFT CLAIM" },
                                { value: "3", text: "GENERATE FILE" },
                                { value: "4", text: "RECEIVE HASIL CLAIM" },
                                { value: "5", text: "SEND HASIL CLAIM" },
                            ]
                        },
                    ]
                },
            ],
            columns: [
                { field: "GenerateNo", title: "Warranty Claim No.", width: 150 },
                {
                    field: "GenerateDate", title: "Warranty Claim Date", width: 130,
                    template: "#= (GenerateDate == undefined) ? '' : moment(GenerateDate).format('DD MMM YYYY') #"
                },
                { field: 'Invoice', title: 'Invoice No.' },
                { field: 'FPJNo', title: 'Tax Invoice No.' },
                {
                    field: "FPJDate", title: "Tax Invoice Date", sWidth: "130px",
                    template: "#= (FPJDate == undefined) ? '' : moment(FPJDate).format('DD MMM YYYY') #"
                },
                { field: 'FPJGovNo', title: 'Tax Invoice GOV No.' },
                { field: 'SourceDataDesc', title: 'Source Data' },
                { field: 'TotalNoOfItem', title: 'Record Total' },
                { field: 'TotalClaimAmt', title: 'Warranty Claim Total' },
                { field: 'SenderDealerName', title: 'Sender' },
                { field: 'RefferenceNo', title: 'Refference No.' },
                {
                    field: "RefferenceDate", title: "Refference Date", sWidth: "130px",
                    template: "#= (RefferenceDate == undefined) ? '' : moment(RefferenceDate).format('DD MMM YYYY') #"
                },
                { field: 'PostingFlagDesc', title: 'Status' }
            ],
        });

        lookup.dblClick(function (data) {
            widget.populate(data);
            if (data.GenerateNo != "") {
                $('#btnAddClaimDtl').removeAttr('disabled');
                widget.showToolbars(["btnNew", "btnBrowse", "btnSave"]);
                ClaimDetail();
                PartDetail();
                InfoCost(data.GenerateNo);
                ClaimInfoApp(data.GenerateNo, null);
            }
        });
    };

    function New() {
        widget.clearForm();
        widget.post('sv.api/inputwarrantyclaim/default', function (result) {
            widget.default = result;
            widget.populate(result);
        });
        ClaimDetail();
        PartDetail();
        ClaimInfoApp(null, null);
        PartInfo(null, null);
        $('#btnAddClaimDtl,#btnEditClaimDtl,#btnDeleteClaimDtl,#btnAddPartDtl').attr('disabled', 'disabled');
        widget.showToolbars(["btnNew", "btnBrowse", "btnSave"]);
    }
    
    function PrintClaim() {
        createCookie('sdc', '1', 1);
        widget.loadForm();
        widget.showForm({ url: 'sv/report/svrptrn013' });
    }

    $('#tblClaimDataInfo').on('click', function (e) {
        ClaimDetail();
        PartDetail();
        $('#tblClaimDataInfo .row_selected').map(function (idx, el) {
            var td = $(el).find('td');
            seq = td[0].textContent;
            $('[name=TroubleDescription]').val(td[21].textContent);
            $('[name=ProblemExplanation]').val(td[22].textContent);
        });
        $('#GenerateSeq').val(seq);
        $('#btnAddPartDtl').removeAttr('disabled');
        
        PartInfo($('#GenerateNo').val(), seq);
    });

    widget.onTableClick(function (icon, row) {
        switch (icon) {
            case "edit":
                editPartDetail(row);
                edit = true;
                break;
            case "trash":
                deletePartDetail(row);
                break;
            default:
                break;
        } 
    });

    function editPartDetail(row) {
        var data = {
            PartSeq: row[0],
            IsCasual: row[3],
            PartNo: row[4],
            Quantity: row[5],
            ClaimPrice: row[7]
        }
        widget.populate(data, "#pnlInsertPartInfo")

        addPartDetail();
    }

    function deletePartDetail(row) {
        var data = {
            PartSeq: row[0],
            IsCasual: row[3],
            PartNo: row[4],
            Quantity: row[5],
            ClaimPrice: row[7],
            GenerateNo: $('#GenerateNo').val(),
            GenerateSeq: seq
        }

        var datas = { model: data, tblpart: tblpartcount };
        widget.confirm("Hapus Part Detail?", function (con) {
            if (con == "Yes") {
                $.ajax({
                    type: 'POST',
                    data: JSON.stringify(datas),
                    url: 'sv.api/inputwarrantyclaim/deletepart',
                    contentType: "application/json; charset=utf-8",
                    dataType: "json",
                    async: false,
                    success: function (result) {
                        if (result.success) {
                            widget.showNotification(result.message);
                            PartInfo(data.GenerateNo, data.GenerateSeq);
                        }
                        else {
                            widget.alert(result.message);
                        }
                    }
                });
            }
        })
    }

    function ClaimInfoApp(generateNo, generateSeq) {
        widget.post('sv.api/inputwarrantyclaim/getclaimappdata', { generateNo: generateNo , generateSeq: generateSeq}, function (result) {
            if (result.length > 0) {
                $('#btnEditClaimDtl,#btnDeleteClaimDtl').removeAttr('disabled');
            }
            if (generateSeq != null) {
                widget.populate(result[0]);
            }
            else {
                widget.populateTable({ selector: "#tblClaimDataInfo", data: result, selectable: true });
            }
        });
    }
    
    function PartInfo(generateNo, generateSeq) {
        widget.post('sv.api/inputwarrantyclaim/getpartdata', { generateNo: generateNo, generateSeq: generateSeq }, function (e) {
            widget.populateTable({ selector: "#tblPartInfo", data: e.data });
            tblpartcount = e.count;
            InfoCost(generateNo);
        });
    }

    function InfoCost(generateNo) {
        widget.post('sv.api/inputwarrantyClaim/informationcost', { generateNo: generateNo }, function (result) {
            widget.populateTable({ selector: "#tblTotalInfo", data: result });
        });
    }

    function ClaimDetail() {
        $("#pnlInsertClaimInfo").slideUp();
        $("#btnAddClaimDtl").parent().show();
        $("#btnDeleteClaimDtl").parent().show();
        $("#btnEditClaimDtl").parent().show();

        //$("#pnlSubDealerBranch").slideDown();
    }

    function PartDetail() {
        $("#pnlInsertPartInfo").slideUp();
        $("#btnAddPartDtl").parent().show();
        //$("#pnlSubDealerBranch").slideDown();
    }

    function addClaimDetail() {
        $("#pnlInsertClaimInfo").slideDown();
        $("#btnAddClaimDtl").parent().hide();
        $("#btnEditClaimDtl").parent().hide();
        $("#btnDeleteClaimDtl").parent().hide();

        //$("#pnlSubDealerBranch").slideUp();
    }

    function addPartDetail() {
        $("#pnlInsertPartInfo").slideDown();
        $("#btnAddPartDtl").parent().hide();

        //$("#pnlSubDealerBranch").slideUp();
    }

    function editClaimDetail() {
        $("#pnlInsertClaimInfo").slideDown();
        $("#btnAddClaimDtl").parent().hide();
        $("#btnEditClaimDtl").parent().hide();
        $("#btnDeleteClaimDtl").parent().hide();
       
        var genNo = $('#GenerateNo').val();
        ClaimInfoApp(genNo, seq);
    }
});

function Model(data,row) {
    this.GenerateSeq = data[0][row];
    this.CategoryCode = data[1][row];
    this.IssueNo = data[2][row];
    this.IssueDate = data[3][row];
    this.ServiceBookNo = data[4][row];
    this.ChassisCode = data[5][row];
    this.ChassisNo = data[6][row];
    this.EngineCode = data[7][row];
    this.EngineNo = data[8][row];
    this.isCbu = data[9][row];
    this.BasicModel = data[10][row];
    this.RegisteredDate =new Date(data[11][row]);
    this.RepairedDate = data[12][row];
    this.Odometer = data[13][row];
    this.ComplainCode = data[14][row];
    this.DefectCode = data[15][row];
    this.SubletHour = data[16][row];
    this.BasicCode = data[17][row];
    this.VarCom = data[18][row];
    this.OperationHour = data[19][row];
}