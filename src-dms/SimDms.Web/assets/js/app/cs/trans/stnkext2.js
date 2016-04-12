$(document).ready(function () {
    var options = {
        title: "Reminder Perpanjangan STNK",
        xtype: "panels",
        panels: [
            {
                title: "Customer Details",
                items: [
                    {
                        text: "Company",
                        type: "controls",
                        items: [
                            { name: "CompanyCode", cls: "span2", placeHolder: "Company Code", readonly: true },
                            { name: "CompanyName", cls: "span6", placeHolder: "Company Name", readonly: true }
                        ]
                    },
                    {
                        text: "Branch",
                        type: "controls",
                        items: [
                            { name: "BranchCode", cls: "span2", placeHolder: "Branch Code", readonly: true },
                            { name: "BranchName", cls: "span6", placeHolder: "Branch Name", readonly: true }
                        ]
                    },
                    {
                        text: "Customer",
                        type: "controls",
                        items: [
                            { name: "CustomerCode", cls: "span2", placeHolder: "Branch Code", readonly: true, type: "popup" },
                            { name: "CustomerName", cls: "span6", placeHolder: "Branch Name", readonly: true }
                        ]
                    },
                    { name: "Address", text: "Address", type: "textarea", readonly: true },
                    { name: "PhoneNo", text: "Telephone", readonly: true },
                ]
            },
            {
                items: [
                    { name: "SalesModel", text: "Car Type", readonly: true },
                    { name: "Colour", text: "Warna", readonly: true },
                    { name: "PoliceRegNo", text: "No Polisi", readonly: true },
                    { name: "Engine", text: "Engine", readonly: true },
                    { name: "Chassis", text: "Chassis", readonly: true },
                    { name: "SalesmanName", text: "Sales Name", readonly: true },
                ]
            },
            {
                items: [
                    { name: "BpkbDate", text: "BPKB Date", cls: "span4", readonly: true, type: "text" },
                    { name: "StnkDate", text: "STNK Date", cls: "span4", readonly: true, type: "text" },
                ]
            },
            {
                title: "STNK Requirements",
                items: [
                    { name: "Ownership", text: "Kepemilikan Kendaraan", type: "switch", cls: "span4", float: "left" },
                    { name: "IsStnkExtend", text: "Bantu Perpanjang", type: "switch", cls: "span4", float: "left" },
                    { name: "StnkExpiredDate", text: "STNK Expired Date", type: "text", cls: "span4", readonly: true },
                ]
            },
            {
                items: [
                    { name: "Category", text: "Category", cls: "span4", type: "text", readonly: true },
                    { name: "Tenor", text: "Tenor", cls: "tenor span4", type: "text", readonly: true },
                    {
                        text: "Leasing", type: "controls",
                        items: [
                            { name: "LeasingCo", placeHolder: "Leasing Code", cls: "span2", readonly: true },
                            { name: "LeasingName", placeHolder: "Leasing Company", cls: "span6", readonly: true },
                        ]
                    },
                    { name: "ReqKtp", text: "KTP", type: "switch", float: "left", cls: "span2" },
                    { name: "ReqStnk", text: "STNK", type: "switch", float: "left", cls: "span2" },
                    { name: "ReqBpkb", text: "BPKB asli", type: "switch", float: "left", cls: "span2" },
                    { name: "ReqSuratKuasa", text: "Surat Kuasa", type: "switch", float: "left", cls: "span2" },
                    { name: "Comment", text: "Customer Voice", type: "textarea" },
                    { name: "Additional", text: "Additional inquiries", cls: "hide" },
                    { name: "Finish", text: "Finish", cls: "span2", type: "switch", float: "left" },
                    { name: "Reason", text: "Alasan", cls: "span6", type: "select" },
                    { name: "field_kosong", text: "  ", cls: "span6", type: "divider" }
                    //{ name: "StatusInfo", text: "status", readonly: true },
                ]
            },
        ],
        toolbars: [
            { name: "btnNew", text: "New", icon: "icon-file", cls: "hide" },
            { name: "btnBrowse", text: "Browse", icon: "icon-search" },
            { name: "btnSave", text: "Save", icon: "icon-save", cls: "hide" },
            { name: "btnDelete", text: "Delete", icon: "icon-trash", cls: "hide" },
        ],
    }
    var widget = new SimDms.Widget(options);
    widget.default = {};
    widget.render(function () {
        widget.setSelect([
            { name: "Reason", url: "cs.api/Combo/Reasons/" },
        ]);
        widget.post("cs.api/stnkext/default", function (result) {
            widget.default = $.extend({}, result, { IsStnkExtend: false, ReqKtp: false, ReqSuratKuasa: false, ReqBpkb: false, ReqStnk: false, Ownership: false });
            widget.populate(widget.default);

            $("[name=Tenor]").parent().parent().slideUp();
            $("[name=LeasingCo]").parent().parent().parent().parent().slideUp();
            $("[name=ReqBpkb],[name=ReqSuratKuasa]").parent().parent().parent().slideUp();
            $("[name=ReqKtp],[name=ReqStnk]").parent().parent().parent().slideUp();
            
        });
    });

    $("#btnNew").on("click", function () {
        $("input[type='text'],textarea,select").val("");
        widget.populate(widget.default);
        widget.showToolbars(["btnBrowse"]);

        $("[name=Tenor]").parent().parent().slideUp();
        $("[name=LeasingCo]").parent().parent().parent().parent().slideUp();
        $("[name=ReqBpkb],[name=ReqSuratKuasa]").parent().parent().parent().slideUp();
        $("[name=ReqKtp],[name=ReqStnk]").parent().parent().parent().slideUp();
    });

    $("#btnCustomerCode").on("click", function () {
        var id = this.id;
        var lookup = widget.klookup({
            name: "StnkExt",
            title: "Customer List",
            url: "cs.api/Lookup/CsStnkExtensions",
            params: { Outstanding: (id == "btnBrowse") ? "N" : "Y" },
            sort: ({ field: "StnkDate", dir: "desc" }),
            serverBinding: true,
            filters: [
                {
                    text: "Filter",
                    type: "controls",
                    left: 80,
                    items: [
                        { name: "fltCustName", text: "Customer Name", cls: "span4" },
                        { name: "fltVinNo", text: "Vin No", cls: "span2" },
                        { name: "fltPolReg", text: "Police No", cls: "span2" },
                    ]
                }
            ],
            columns: [
                { field: "BranchCode", title: "Outlet", width: 100 },
                { field: "CustomerCode", title: "ID Cust", width: 100 },
                { field: "CustomerName", title: "Customer Name", width: 250 },
                { field: "PoliceRegNo", title: "No Polisi", width: 180 },
                { field: "Chassis", title: "Vin No", width: 180 },
                { field: "SalesModelCode", title: "Sales Model", width: 160 },
                { field: "SalesModelYear", title: "Year", width: 80, filterable: false },
                { field: "Category", title: "Category", width: 100 },
                { field: "LeasingName", title: "Leasing Name", width: 300 },
                { field: "BPKDate", title: "Bpk Date", width: 120, filterable: false, template: "#= (BPKDate == undefined) ? '' : moment(BPKDate).format('DD MMM YYYY') #" },
                { field: "BpkbDate", title: "Bpkb Date", width: 120, filterable: false, template: "#= (BpkbDate == undefined) ? '' : moment(BpkbDate).format('DD MMM YYYY') #" },
                { field: "StnkExpiredDate", title: "Stnk Expired Date", width: 120, filterable: false, template: "#= (StnkExpiredDate == undefined) ? '' : moment(StnkExpiredDate).format('DD MMM YYYY') #" },
            ],
        });
        lookup.dblClick(refreshData);
    });

    $("#btnBrowse").on("click", function () {
        var id = this.id;
        var lookup = widget.klookup({
            name: "StnkExt",
            title: "Customer List",
            url: "cs.api/Lookup/CsStnkExtensions",
            params: { Outstanding: (id == "btnBrowse") ? "N" : "Y" },
            sort: ({ field: "StnkDate", dir: "desc" }),
            serverBinding: true,
            filters: [
                {
                    text: "Filter",
                    type: "controls",
                    left: 80,
                    items: [
                        { name: "fltCustName", text: "Customer Name", cls: "span4" },
                        { name: "fltVinNo", text: "Vin No", cls: "span2" },
                        { name: "fltPolReg", text: "Police No", cls: "span2" },
                    ]
                }
            ],
            columns: [
                { field: "BranchCode", title: "Outlet", width: 100 },
                { field: "CustomerCode", title: "ID Cust", width: 100 },
                { field: "CustomerName", title: "Customer Name", width: 250 },
                { field: "PoliceRegNo", title: "No Polisi", width: 180 },
                { field: "Chassis", title: "Vin No", width: 180 },
                { field: "SalesModelCode", title: "Sales Model", width: 160 },
                { field: "SalesModelYear", title: "Year", width: 80, filterable: false },
                { field: "Category", title: "Category", width: 100 },
                { field: "LeasingName", title: "Leasing Name", width: 300 },
                { field: "BPKDate", title: "Bpk Date", width: 120, filterable: false, template: "#= (BPKDate == undefined) ? '' : moment(BPKDate).format('DD MMM YYYY') #" },
                { field: "BpkbDate", title: "Bpkb Date", width: 120, filterable: false, template: "#= (BpkbDate == undefined) ? '' : moment(BpkbDate).format('DD MMM YYYY') #" },
                { field: "StnkExpiredDate", title: "Stnk Expired Date", width: 120, filterable: false, template: "#= (StnkExpiredDate == undefined) ? '' : moment(StnkExpiredDate).format('DD MMM YYYY') #" },
                { field: "Status", title: "Status", width: 100, template: "#= (Status == 1) ? 'FINISH' : '<span class=font-red>NOT FINISH</span>' #" },
            ],
        });
        lookup.dblClick(refreshData);
    });

    $("#btnSave").on("click", function () {
        var data = $(".main form").serializeObject();
        data.Status = ((data.Finish == "true") ? 1 : 0);
        //console.log(data)

        if (data.Finish == "false" && data.Reason == "") {
            $('#Reason').focus();
            widget.Error("Silahkan input field Alasan");

            var puterror = setInterval(function () {
                $('#Reason').addClass('error');
                setTimeout(function () { if ($('#Reason').hasClass("error")) { $('#Reason').removeClass('error'); } }, 250);
            }, 300);
            setTimeout(function () { clearInterval(puterror); }, 2400);

            return;
        }

        widget.post("cs.api/stnkext/save", data, function (result) {
            if (result.success) {
                refreshData();
                widget.showNotification("Data has been saved.");
            }
        });
    });

    $("#btnDelete").on("click", function () {
        var data = $(".main form").serializeObject();
        widget.confirm("Anda yakin akan menghapus data ini?", function (result) {
            if (result == "Yes") {
                widget.post("cs.api/stnkext/delete", data, function (result) {
                    if (result.success) {
                        $("#btnNew").click();
                    }
                });
            }
        });
    });

    $("#IsStnkExtendN").on("click", function () {
        $("[name=ReqKtp],[name=ReqStnk]").parent().parent().parent().slideUp();
    });

    $("#IsStnkExtendY").on("click", function () {
        $("[name=ReqKtp],[name=ReqStnk]").parent().parent().parent().slideDown();
    });

    $("#FinishN").on("click", function () {
        $("[name=Reason]").parent().parent().slideDown();
    });

    $("#FinishY").on("click", function () {
        $("[name=Reason]").parent().parent().slideUp();
    });

    function refreshData(data) {
        var params = undefined;
        if (data === undefined) {
            params = { CompanyCode: $("[name=CompanyCode]").val(), CustomerCode: $("[name=CustomerCode]").val(), Chassis: $("[name=Chassis]").val() };
        } else {
            params = { CompanyCode: data.CompanyCode, CustomerCode: data.CustomerCode, Chassis: data.Chassis };
        }

        if (!widget.isNullOrEmpty(params.Chassis)) {
            widget.post("cs.api/stnkext/get", params, function (result) {
                if (result.success) {
                    if (widget.isNullOrEmpty(result.data.StnkExpiredDate) == true) {
                        widget.alert('Field "STNK Date" kosong. Silahkan hubungi Sales admin untuk menginput data tsb pada menu SPK & Tracking BBN');
                        return;
                    }

                    if (result.data.Status == 1) {
                        result.data.Finish = true;
                        $("[name=Reason]").parent().parent().slideUp();
                    }
                    else {
                        result.data.Finish = false;
                        $("[name=Reason]").parent().parent().slideDown();
                    }
                    widget.populate(result.data);

                    if (result.data.IsNew === 1) {
                        widget.showToolbars(["btnNew", "btnBrowse", "btnSave"]);
                    }
                    else {
                        widget.showToolbars(["btnNew", "btnBrowse", "btnSave", "btnDelete"]);
                    }

                    if (widget.isNullOrEmpty(result.data) == false) {
                        if (widget.isNullOrEmpty(result.data.BpkbDate) == false) {
                            var bpkbDate = widget.toDateFormat(widget.cleanJsonDate(result.data.BpkbDate));
                            $("[name=BpkbDate]").val(bpkbDate);
                        }

                        if (widget.isNullOrEmpty(result.data.StnkDate) == false) {
                            var stnkDate = widget.toDateFormat(widget.cleanJsonDate(result.data.StnkDate));
                            $("[name=StnkDate]").val(stnkDate);
                            
                            //var stnkexpDate = moment(widget.cleanJsonDate(result.data.StnkDate)).format('DD-MMM-');
                            //$("[name=StnkExpiredDate]").val(stnkexpDate + (parseInt(moment(new Date()).format('YYYY'), 0) + 1));
                        }
                        //else { $("[name=StnkExpiredDate]").val(''); }

                        if (widget.isNullOrEmpty(result.data.StnkExpiredDate) == false) {
                            var stnkexpDate = widget.toDateFormat(widget.cleanJsonDate(result.data.StnkExpiredDate));
                            $("[name=StnkExpiredDate]").val(stnkexpDate);
                        }
                    }

                    if (result.data.IsStnkExtend == "true") {
                        $("[name=ReqKtp],[name=ReqStnk]").parent().parent().parent().slideDown();
                    }
                    else {
                        $("[name=ReqKtp],[name=ReqStnk]").parent().parent().parent().slideUp();
                    }

                    if (result.data.IsLeasing) {
                        $("[name=Tenor]").parent().parent().slideDown();
                        $("[name=LeasingCo]").parent().parent().parent().parent().slideDown();
                        $("[name=ReqSuratKuasa]").parent().parent().parent().slideDown();
                        $("[name=ReqBpkb]").parent().parent().parent().slideUp();
                        $("[name=Category]").parent().parent().removeClass("full");

                        if (result.data.IsStnkExtend == "true") {
                            $("[name=ReqSuratKuasa]").parent().parent().parent().slideDown();
                        }
                        else {
                            $("[name=ReqSuratKuasa]").parent().parent().parent().slideUp();
                        }

                        $("#IsStnkExtendN").off('click');
                        $("#IsStnkExtendN").on("click", function () {
                            $("[name=ReqKtp],[name=ReqStnk],[name=ReqSuratKuasa]").parent().parent().parent().slideUp();
                        });

                        $("#IsStnkExtendY").off("click");
                        $("#IsStnkExtendY").on("click", function () {
                            $("[name=ReqKtp],[name=ReqStnk],[name=ReqSuratKuasa]").parent().parent().parent().slideDown();
                            //$("[name=ReqBpkb]").parent().parent().parent().hide();
                        });
                    }
                    else {
                        $("[name=Tenor]").parent().parent().slideUp();
                        $("[name=LeasingCo]").parent().parent().parent().parent().slideUp();
                        $("[name=ReqSuratKuasa]").parent().parent().parent().slideUp();
                        $("[name=ReqBpkb]").parent().parent().parent().slideDown();
                        $("[name=Category]").parent().parent().addClass("full");

                        if (result.data.IsStnkExtend == "true") {
                            $("[name=ReqBpkb]").parent().parent().parent().slideDown();
                        }
                        else {
                            $("[name=ReqBpkb]").parent().parent().parent().slideUp();
                        }

                        $("#IsStnkExtendN").off('click');
                        $("#IsStnkExtendN").on("click", function () {
                            $("[name=ReqKtp],[name=ReqStnk],[name=ReqBpkb]").parent().parent().parent().slideUp();
                        });

                        $("#IsStnkExtendY").off("click");
                        $("#IsStnkExtendY").on("click", function () {
                            $("[name=ReqKtp],[name=ReqStnk],[name=ReqBpkb]").parent().parent().parent().slideDown();
                            //$("[name=ReqSuratKuasa]").parent().parent().parent().hide();
                        });
                    }
                }
            });
        }
        else {
            widget.showNotification('Maaf, data terpilih tidak valid.\nNomor Chassis masih kosong, tolong cek data penjualan customer terpilih pada SDMS.'
                                    + '\nJika ada data yang kurang sesuai, tolong disesuaikan terlebih dahulu.'
                                    );
        }
    }
});