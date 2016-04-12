var jenisPvt = '';
$(document).ready(function () {
    var options = {
        title: "Pivot Suzuki MSI",
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
                        text: "Dealer",
                        type: "controls",
                        items: [
                            { name: "CompanyCode", text: "Dealer Name", cls: "span6", type: "select", opt_text: "-- SELECT ALL -- " },
                        ]
                    },
                    {
                        text: "Outlet",
                        type: "controls",
                        cls: 'span8',
                        items: [
                            { name: 'Outlet', text: 'outlet', type: 'select', cls: 'span6', opt_text: "-- SELECT ALL -- " }
                        ]
                    },
                    {
                        text: "Jenis",
                        type: "controls",
                        items: [
                            {
                                name: "JenisPivot",
                                type: "select",
                                cls: "span4",
                                readonly: true,
                                items: [
                                    { value: 'LSR', text: 'Labor Sales Revenue' },
                                    { value: 'SSR', text: 'Sparepart Sales Revenue' },
                                    { value: 'UINTAKE', text: 'Unit Intake' },
                                    { value: "JTYPE", text: 'Job Types' },
                                    { value: "FSERVICE", text: 'Free Service' },
                                ]
                            },
                        ]
                    },
                   {
                       text: "Tgl. Transaksi",
                       type: "controls",
                       cls: "span8",
                       items: [
                           //{ name: "isLock", cls: "span1", type: "check", text :'' },
                           { name: "StartDate", cls: "span2", placeHolder: "", type: "datepicker" },
                           { name: "EndDate", cls: "span2", placeHolder: "", type: "datepicker" },
                           {
                               type: "buttons", cls: "span2", items: [
                                   { name: "btnCari", text: "   Query", icon: "icon-search", cls: "button small btn btn-success" },
                               ]
                           },
                       ]
                   },
                ],
            },
            {
                name: 'wxpivotgrid',
                xtype: 'wxtable',
                style: 'margin-top: 35px;'
            }
        ],
    }
    var widget = new SimDms.Widget(options);
    widget.render(renderCallback);

    function renderCallback() {
        initElementEvents();
    }

    function initElementEvents() {
        //console.log(localStorage.getItem('insPVT'));
        jenisPvt = localStorage.getItem('JnsPVT') == null ? 'nilai null' : localStorage.getItem('JnsPVT');
        console.log(jenisPvt);
        var filter = {
            StartDate: new Date(moment(moment().format('YYYY-MM-') + '01')),
            EndDate: new Date(),
            JenisPivot: localStorage.getItem('JnsPVT') == null ? 'LSR' : localStorage.getItem('JnsPVT')
        }
        $("[name=GroupArea]").on("change", function () {
            widget.select({ selector: "[name=CompanyCode]", url: "wh.api/combo/SrvDealerList", params: { LinkedModule: "mp", GroupArea: $("[name=GroupArea]").val() }, optionalText: "-- SELECT ALL --" });
            $("[name=CompanyCode]").prop("selectedIndex", 0);
            $("[name=CompanyCode]").change();
        });
        $("[name=CompanyCode]").on("change", function () {
            widget.select({ selector: "[name=Outlet]", url: "wh.api/combo/SrvBranchList", params: { area: $("#pnlFilter [name=GroupArea]").val(), comp: $("#pnlFilter [name=CompanyCode]").val() }, optionalText: "-- SELECT ALL --" });
            $("[name=Outlet]").prop("selectedIndex", 0);
            $("[name=Outlet]").change();
        });
        
        widget.post('wh.api/indent/default', function (r) {
            if (r.data.CompanyCode && r.data.CompanyCode != "") {
                widget.select({
                    selector: "select[name=GroupArea]",
                    url: "wh.api/combo/SrvGroupAreas",
                }, function () {
                    $('#GroupArea').select2('val', r.data.GroupArea);
                    if (r.data.RoleID != 'DEV-ADMIN' && r.data.RoleID != 'B13ADCEA-E538-49FD-B9E0-34901CE26E2E') {
                        $('#GroupArea').prop('disabled', 'disabled');
                    }
                });
                widget.select({
                    selector: "select[name=CompanyCode]",
                    url: "wh.api/combo/SrvDealerList",
                }, function () {
                    $('#CompanyCode').select2('val', r.data.CompanyCode);
                    if (r.data.RoleID != 'DEV-ADMIN' && r.data.RoleID != 'B13ADCEA-E538-49FD-B9E0-34901CE26E2E') {
                        $('#CompanyCode').prop('disabled', 'disabled');
                    }
                    widget.select({
                        selector: "select[name=Outlet]",
                        url: "wh.api/combo/SrvBranchList",
                        params: { area: r.data.GroupArea, comp: r.data.CompanyCode }
                    }, function () {
                        if (r.data.Outlet) {
                            $('#Outlet').select2('val', r.data.Outlet);
                            if (r.data.RoleID != 'DEV-ADMIN' && r.data.RoleID != 'B13ADCEA-E538-49FD-B9E0-34901CE26E2E') {
                                $('#Outlet').prop('disabled', 'disabled');
                            }
                        }
                    });
                });
            } else {
               // if (r.data.RoleID == 'DEV-ADMIN' || r.data.RoleID == 'B13ADCEA-E538-49FD-B9E0-34901CE26E2E') {
                    widget.select({
                        selector: "select[name=GroupArea]",
                        url: "wh.api/combo/SrvGroupAreas",
                    })
                //} else {
                //    $('#btnCari').prop('disabled', 'disabled');
                //    $('#Outlet, #CompanyCode, #GroupArea, #TypeReport, #Mode, #StartDate, #EndDate').prop('disabled', 'disabled');
                //    sdms.info({ type: "error", text: "User bukan Admin/ Dealer user belum di set! " });
                //}
            }
        });
        
        widget.populate(filter);
        $("#btnCari").on("click", createPivot);

        $('#JenisPivot').select().on("change", function () {
            var data = $("#pnlFilter").serializeObject();
            var pivot = data.JenisPivot;

            localStorage.setItem('JnsPVT', data.JenisPivot);
            jenisPvt = localStorage.getItem('JnsPVT');
            console.log(data.JenisPivot);
            //widget.render(renderCallback);
            //var tbl = document.getElementById("wxpivotgrid");
            //var chld = tbl.firstElementChild;
            //var prnt = tbl.parentElement;
            //var list = document.getElementById("wxpivotgrid");
            //list.removeChild(list.childNodes[0]);
            //if (chld != null) {
            //    $("#wxpivotgrid").pivotUI([], {});
            //    $("#wxpivotgrid").html(null);
            //}
            //console.log(tbl, chld, prnt);
        });
    }

    function createPivot() {
        var data = $("#pnlFilter").serializeObject();
        var pivot = data.JenisPivot;
        if (pivot == "LSR") {
            LabourSlsRevenue(data);
        }
        if (pivot == "SSR") {
            SpSlsRevenue(data);
        }
        if (pivot == "UINTAKE") {
            UnitInTake(data);
        }
        if (pivot == "JTYPE") {
            JobType(data);
        }
        if (pivot == "FSERVICE") {
            FreeService(data);
        }
    }

    function LabourSlsRevenue (data) {
        var params = {
            StartDate: moment(data.StartDate).format('YYYYMMDD'),
            EndDate: moment(data.EndDate).format('YYYYMMDD'),
            CompanyCode: data.CompanyCode,
            BranchCode : data.Outlet,
            PivotId: "usppvt_SvMsiP01",
        }
        $('.page > .ajax-loader').show();
        $.ajax({
            type: "POST",
            url: 'wh.api/InquiryPivot/ServiceTransaction',
            dataType: 'json',
            data: params,
            success: function (response) {
                if (response.length !== 0) {
                    window.pivotdata = response;
                    $("#wxpivotgrid").pivotUI(window.pivotdata, {
                        derivedAttributes: {
                            "Tanggal Inv.": function (mp) {
                                return moment(mp["InvoiceDate"]).format('DD-MMM-YYYY');
                            },
                            "SubCon": function (mp) {
                                return mp.IsSubCon;
                            },
                            "Group": function (mp) {
                                return mp.GroupJobType;
                            },
                            "Job Type": function (mp) {
                                return mp.JobType;
                            },
                            "No. Invoice": function (mp) {
                                return mp.InvoiceNo;
                            },
                            "Labor Amt": function (mp) {
                                return mp.TaskAmt;
                            },
                            "Hour Sold": function (mp) {
                                return mp.OperationHour;
                            },
                            "Group Type": function (mp) {
                                return mp.GroupType;
                            },
                        },
                        rows: ["SubCon", "Group"],
                        aggregatorName: "Integer Sum",
                        vals: ["Labor Amt"],
                        rendererName: "Table",
                        hiddenAttributes: ["InvoiceDate", "IsSubCon", "GroupJobType", "InvoiceNo", "JobType", "TaskAmt", "OperationHour"],
                    });
                    $('.page > .ajax-loader').hide();
                }
                else {
                    sdms.info({ type: "error", text: "Tidak ada data yang ditampilkan" });
                    $('.page > .ajax-loader').hide();
                }
                console.log(response)
            }
        });
    }

    function SpSlsRevenue(data) {
        var params = {
            StartDate: moment(data.StartDate).format('YYYYMMDD'),
            EndDate: moment(data.EndDate).format('YYYYMMDD'),
            CompanyCode: data.CompanyCode,
            BranchCode: data.Outlet,
            PivotId: "usppvt_SvMsiP05",
        }
        $('.page > .ajax-loader').show();
        $.ajax({
            type: "POST",
            url: 'wh.api/InquiryPivot/ServiceTransaction',
            dataType: 'json',
            data: params,
            success: function (response) {
                if (response.length !== 0) {
                    window.pivotdata = response;
                    $("#wxpivotgrid").pivotUI(window.pivotdata, {
                        derivedAttributes: {
                            "Tanggal Inv.": function (mp) {
                                return moment(mp["InvoiceDate"]).format('DD-MMM-YYYY');
                            },
                            "Job Type": function (mp) {
                                return mp.JobType;
                            },
                            "Group Sublet": function (mp) {
                                return mp.GroupPart;
                            },
                            "Group TPGO": function (mp) {
                                return mp.TypeOfGoodDesc;
                            },
                            "Group": function (mp) {
                                return mp.GroupJobType;
                            },
                            "No. Invoice": function (mp) {
                                return mp.InvoiceNo;
                            },
                            "Sparepart Amt": function (mp) {
                                return mp.PartAmount;
                            },
                            "TypeOfGood Name": function (mp) {
                                return mp.TypeOfGoodName;
                            },
                            "Group Type": function (mp) {
                                return mp.GroupType;
                            },
                        },
                        rows: ["Group Sublet", "Group TPGO", "TypeOfGood Name"],
                        cols: ["Group"],
                        aggregatorName: "Integer Sum",
                        vals: ["Sparepart Amt"],
                        rendererName: "Table",
                        hiddenAttributes: ["InvoiceDate", "JobType", "GroupPart", "TypeOfGoodDesc", "GroupJobType", "InvoiceNo", "PartAmount", "TypeOfGoodName"],
                    });
                    $('.page > .ajax-loader').hide();
                }
                else {
                    sdms.info({ type: "error", text: "Tidak ada data yang ditampilkan" });
                    $('.page > .ajax-loader').hide();
                }
            }
        });
    }

    function UnitInTake(data) {
        var params = {
            StartDate: moment(data.StartDate).format('YYYYMMDD'),
            EndDate: moment(data.EndDate).format('YYYYMMDD'),
            CompanyCode: data.CompanyCode,
            BranchCode: data.Outlet,
            PivotId: "usppvt_SvMsiP29",
        }
        $('.page > .ajax-loader').show();
        $.ajax({
            type: "POST",
            url: 'wh.api/InquiryPivot/ServiceTransaction',
            dataType: 'json',
            data: params,
            success: function (response) {
                if (response.length !== 0) {
                    window.pivotdata = response;
                    $("#wxpivotgrid").pivotUI(window.pivotdata, {
                        derivedAttributes: {
                            "Tanggal SPK.": function (mp) {
                                return moment(mp["JobOrderDate"]).format('DD-MMM-YYYY');
                            },
                            "Jml Inv": function (jmlI) {
                                return jmlI.JmlInvoice;
                            },
                            "Basic Model": function (mp) {
                                return mp.BasicModel;
                            },
                            "Company Code": function (mp) {
                                return mp.CompanyCode;
                            },
                            "Chassis Code": function (mp) {
                                return mp.ChassisCode;
                            },
                            "Group Type": function (mp) {
                                return mp.GroupType;
                            },
                        },
                        rows: ["Company Code", "Group Type", "Basic Model"],
                        aggregatorName: "Count",
                        hiddenAttributes: ["JobOrderDate", "JmlInvoice", "BasicModel", "CompanyCode", "ChassisCode", "GroupType"],
                    });
                    $('.page > .ajax-loader').hide();
                }
                else {
                    sdms.info({ type: "error", text: "Tidak ada data yang ditampilkan" });
                    $('.page > .ajax-loader').hide();
                }
            }
        });
    }

    function JobType(data) {
        var params = {
            StartDate: moment(data.StartDate).format('YYYYMMDD'),
            EndDate: moment(data.EndDate).format('YYYYMMDD'),
            CompanyCode: data.CompanyCode,
            BranchCode: data.Outlet,
            PivotId: "usppvt_SvMsiP33",
        }
        $('.page > .ajax-loader').show();
        $.ajax({
            type: "POST",
            url: 'wh.api/InquiryPivot/ServiceTransaction',
            dataType: 'json',
            data: params,
            success: function (response) {
                if (response.length !== 0) {
                    window.pivotdata = response;
                    $("#wxpivotgrid").pivotUI(window.pivotdata, {
                        derivedAttributes: {
                            "Tanggal SPK.": function (mp) {
                                return moment(mp["JobOrderDate"]).format('DD-MMM-YYYY');
                            },
                            "Chassis Code": function (mp) {
                                return mp.ChassisCode;
                            },
                            "Chassis No": function (mp) {
                                return mp.ChassisNo;
                            },
                            "VIN No": function (mp) {
                                return mp.VinNo;
                            },
                            "Job Type": function (mp) {
                                return mp.JobType;
                            },
                            "Group Job Type": function (mp) {
                                return mp.GroupJobType;
                            },
                            "Warranty Odometer": function (mp) {
                                return mp.WarrantyOdometer;
                            },
                            "Group Type": function (mp) {
                                return mp.GroupType;
                            },
                        },
                        rows: ["Group Job Type", "Job Type"],
                        aggregatorName: "Count",
                        hiddenAttributes: ["JobOrderDate", "ChassisCode", "ChassisNo", "VinNo", "JobType", "GroupJobType", "WarrantyOdometer"],
                    });
                    $('.page > .ajax-loader').hide();
                }
                else {
                    sdms.info({ type: "error", text: "Tidak ada data yang ditampilkan" });
                    $('.page > .ajax-loader').hide();
                }
            }
        });
    }

    function FreeService(data) {
        var params = {
            StartDate: moment(data.StartDate).format('YYYYMMDD'),
            EndDate: moment(data.EndDate).format('YYYYMMDD'),
            CompanyCode: data.CompanyCode,
            BranchCode: data.Outlet,
            PivotId: "usppvt_SvMsiP44",
        }
        $('.page > .ajax-loader').show();
        $.ajax({
            type: "POST",
            url: 'wh.api/InquiryPivot/ServiceTransaction',
            dataType: 'json',
            data: params,
            success: function (response) {
                if (response.length !== 0) {
                    window.pivotdata = response;
                    $("#wxpivotgrid").pivotUI(window.pivotdata, {
                        derivedAttributes: {
                            "Tanggal Inv.": function (mp) {
                                return moment(mp["InvoiceDate"]).format('DD-MMM-YYYY');
                            },
                            "Fsc Group": function (mp) {
                                return mp.FscGroup;
                            },
                            "Chassis Code": function (mp) {
                                return mp.ChassisCode;
                            },
                            "Chassis No": function (mp) {
                                return mp.ChassisNo;
                            },
                            "Group Job Type": function (mp) {
                                return mp.GroupJobType;
                            },
                            "Group Type": function (mp) {
                                return mp.GroupType;
                            },
                        },
                        rows: ["Group Job Type", "Chassis Code"],
                        cols: ["Fsc Group"],
                        aggregatorName: "Count",
                        hiddenAttributes: ["InvoiceDate", "FscGroup", "ChassisCode", "ChassisNo", "GroupJobType"],
                    });
                    $('.page > .ajax-loader').hide();
                }
                else {
                    sdms.info({ type: "error", text: "Tidak ada data yang ditampilkan" });
                    $('.page > .ajax-loader').hide();
                }
                console.log(response);
            }
        });
    }
});
