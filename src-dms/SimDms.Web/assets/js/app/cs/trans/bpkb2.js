$(document).ready(function () {
    var records = {};
    
    var states = {
        lockVisibility: false,
        transactionStatus: false,
    };

    var options = {
        title: "BPKB",
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
                            { name: "CustomerCode", cls: "span2", placeHolder: "Customer Code", readonly: true, type: "popup" },
                            { name: "CustomerName", cls: "span6", placeHolder: "Customer Name", readonly: true }
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
                    { name: "BpkbReadyDate", text: "BPKB Diterima", cls: "span4", readonly: true },
                    { name: "StnkDate", text: "Tgl STNK", cls: "span4", readonly: true, type: "text" },
                ]
            },
            {
                title: "BPKB Requirements",
                items: [
                    { name: "Category", text: "Category", cls: "span4", type: "text", readonly: true },
                    { name: "Tenor", text: "Tenor", cls: "tenor span4", type: "text", readonly: true },
                    {
                        text: "Leasing", type: "controls",
                        items: [
                            { name: "LeasingCode", placeHolder: "Leasing Code", cls: "span2", readonly: true },
                            { name: "LeasingName", placeHolder: "Leasing Company", cls: "span6", readonly: true },
                        ]
                    },
                    { name: "ReqKtp", text: "KTP", type: "switch", float: "left", cls: "span2 hide" },
                    { name: "ReqStnk", text: "STNK", type: "switch", float: "left", cls: "span2 hide" },
                    { name: "ReqSuratKuasa", text: "Surat Kuasa", type: "switch", cls: "span2 hide", float: "left" },
                    { name: "BpkbPickUp", text: "BPKB Diserahkan", cls: "TanggalInput span4", readonly: true },
                    { name: "ReqInfoLeasing", text: "Info ke Leasing", type: "switch", cls: "leasing span2", float: "left" },
                    { name: "ReqInfoCust", text: "Info ke Pelanggan", type: "switch", cls: "leasing span2", float: "left" },
                    //{ name: "StatusInfo", text: "Status", readonly: true },
                ]
            },
            {
                title: "Next Appointment",
                name: "panelPendingBpkbRetrieval",
                items: [
                    { text: "Tanggal Penundaan", name: "RetrievalEstimationDate", type: "datepicker", cls: "span3", readonly: true },
                    { text: "Alasan", name: "Notes", type: "text", cls: "span8", readonly: true },
                    {
                        type: "buttons",
                        items: [
                            { name: "btnSavePendingBpkbRetrieval", type: "button", text: "Save", cls: "btn small", icon: "icon-save" },
                            { name: "btnCancelPendingBpkbRetrieval", type: "button", text: "Cancel", cls: "btn small", icon: "icon-trash" }
                        ]
                    }
                ]
            },
            {
                xtype: "kgrid",
                title: "Next Appointment",
                name: "gridPendingBpkbRetrieval",
            },
            {
                items: [
                    { name: "Comment", text: "Customer Voice", type: "textarea" },
                    { name: "Additional", text: "Additional inquiries", type: "text", cls: "hide" },
                    { name: "Finish", text: "Finish", cls: "span2", type: "switch", float: "left" },
                    { name: "Reason", text: "Alasan", cls: "span6", type: "select" },
                    { name: "field_kosong", text: "  ", cls: "span6", type: "divider" }
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
        widget.post("cs.api/bpkb/default", function (result) {
            widget.default = $.extend({}, result, { ReqInfoLeasing: false, ReqInfoCust: false, ReqKtp: false, ReqStnk: false, ReqSuratKuasa: false });
            widget.populate(widget.default);

            $("[name=Tenor]").parent().parent().slideUp();
            
            $("[name=LeasingCo]").parent().parent().parent().parent().slideUp();
            $("[name=ReqInfoLeasing]").parent().parent().parent().slideUp();
            $("[name=ReqInfoCust]").parent().parent().parent().slideUp();
            $("[name=Category]").parent().parent().addClass("full");
            $("#gridPendingBpkbRetrieval").parent().hide();
            $("#gridPendingBpkbRetrieval").parent().siblings().hide();
        });
        states.transactionStatus = false;

        refreshGridPendingBpkbRetrieval();
        checkValidation();
        showPanelPendingBpkbRetrieval(false);
        initEvent();
    });

    $("#btnNew").on("click", function () {
        $("input[type='text'],textarea,select").val("");
        widget.populate(widget.default);
        widget.showToolbars(["btnBrowse"]);
        refreshGridPendingBpkbRetrieval();

        $("[name=Tenor]").parent().parent().slideUp();
        
        $("[name=LeasingCo]").parent().parent().parent().parent().slideUp();
        $("[name=ReqInfoLeasing]").parent().parent().parent().slideUp();
        $("[name=ReqInfoCust]").parent().parent().parent().slideUp();
        $("[name=Category]").parent().parent().addClass("full");
        $("[name=Reason]").parent().parent().slideDown();
        states.transactionStatus = false;
    });

    $("#btnBrowse").on("click", function () {
        //states.transactionStatus = false;
    });

    $("#btnCustomerCode").on("click", function () {
        //setTimeout(function () {
        //    $("#btnSelectData").on("click", function () {
        //        states.transactionStatus = true;
        //    });
        //    $("#btnCancelPanel").on("click", function () {
        //        states.transactionStatus = false;
        //    });
        //}, 500);
    });

    $("#btnCustomerCode").on("click", function () {
        var id = this.id;

        var lookup = widget.klookup({
            name: "LkuBpkb",
            title: "Customer List",
            url: "cs.api/lookup/CsBpkbReminders",
            params: { OutStanding: (id == "btnBrowse") ? "N" : "Y" },
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
                { field: "SalesModelYear", title: "Year", width: 80 },
                { field: "Category", title: "Category", width: 100 },
                { field: "BPKDate", title: "Bpk Date", width: 120, template: "#= (BPKDate == undefined) ? '' : moment(BPKDate).format('DD MMM YYYY') #" },
                { field: "BpkbReadyDate", title: "Bpkb Diterima", width: 150, template: "#= (BpkbReadyDate == undefined) ? '' : moment(BpkbReadyDate).format('DD MMM YYYY') #" },
                { field: "BpkbPickUp", title: "Bpkb Diserahkan", width: 150, template: "#= (BpkbPickUp == undefined) ? '' : moment(BpkbPickUp).format('DD MMM YYYY') #" },
                { field: "LeasingName", title: "Leasing Name", width: 300 },
                { field: "BpkbDate", title: "Bpkb Date", width: 120, template: "#= (BpkbDate == undefined) ? '' : moment(BpkbDate).format('DD MMM YYYY') #" },
                { field: "StnkDate", title: "Stnk Date", width: 120, template: "#= (StnkDate == undefined) ? '' : moment(StnkDate).format('DD MMM YYYY') #" },
            ],
        });
        lookup.dblClick(refreshData);
    });

    $("#btnBrowse").on("click", function () {
        var id = this.id;

        var lookup = widget.klookup({
            name: "LkuBpkb",
            title: "Customer List",
            url: "cs.api/lookup/CsBpkbReminders",
            params: { OutStanding: (id == "btnBrowse") ? "N" : "Y" },
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
                { field: "SalesModelYear", title: "Year", width: 80 },
                { field: "Category", title: "Category", width: 100 },
                { field: "BPKDate", title: "Bpk Date", width: 120, template: "#= (BPKDate == undefined) ? '' : moment(BPKDate).format('DD MMM YYYY') #" },
                { field: "BpkbReadyDate", title: "Bpkb Diterima", width: 150, template: "#= (BpkbReadyDate == undefined) ? '' : moment(BpkbReadyDate).format('DD MMM YYYY') #" },
                { field: "BpkbPickUp", title: "Bpkb Diserahkan", width: 150, template: "#= (BpkbPickUp == undefined) ? '' : moment(BpkbPickUp).format('DD MMM YYYY') #" },
                { field: "LeasingName", title: "Leasing Name", width: 300 },
                { field: "BpkbDate", title: "Bpkb Date", width: 120, template: "#= (BpkbDate == undefined) ? '' : moment(BpkbDate).format('DD MMM YYYY') #" },
                { field: "StnkDate", title: "Stnk Date", width: 120, template: "#= (StnkDate == undefined) ? '' : moment(StnkDate).format('DD MMM YYYY') #" },
                { field: "Status", title: "Status", width: 100, template: "#= (Status == 1) ? 'FINISH' : '<span class=font-red>NOT FINISH</span>' #" },
            ],
        });
        lookup.dblClick(refreshData);
    });

    $("#btnSave").on("click", function () {
        var data = $(".main form").serializeObject();
        var error = false;
        
        //BEGIN VALIDATE
        if (data.Finish == "true") {
            var params = { CompanyCode: $("[name=CompanyCode]").val(), CustomerCode: $("[name=CustomerCode]").val(), Chassis: $("[name=Chassis]").val() };
            
            $.ajax({
                type: "POST",
                url: SimDms.baseUrl + "cs.api/bpkb/GetRetrievalEstimation",
                data: params,
                traditional: true,
                async: false,
                success: function (result) {
                    if (result.success) {
                        if (widget.isNullOrEmpty(result.data.RetrievalEstimationDate) == true && data.Category == "Tunai") {
                            widget.Error("Silahkan input data Next Appointment terlebih dahulu");
                            $('#gridPendingBpkbRetrieval').focus();
                            error = true;
                        }
                    }
                    $(".page .ajax-loader").fadeOut();

                    widget.showNotification(result.message);
                },
                error: function (XMLHttpRequest, textStatus, errorThrown) {
                    _this.Error(errorThrown);
                    $(".page .ajax-loader").fadeOut();
                }
            });
            /*var cekfinish = true;
                var errmsgbpkb = $('input[name=BpkbReadyDate]').parent();
                var errmsgpickup = $('input[name=BpkbPickUp]').parent();
                if (data.BpkbReadyDate == "") { errmsgbpkb.addClass('error'); cekfinish = false; }
                if (!cekfinish) {
                    if (errmsgbpkb.hasClass("error")) { widget.Error("Field BPKB Ready Date masih kosong"); }
                    setTimeout(function () {
                        if (errmsgbpkb.hasClass("error"))   { errmsgbpkb.removeClass('error'); }
    
                    }, 1500);
                    return;
                }*/
        }
        else {
            if (data.Reason == "") {
                $('#Reason').focus();
                widget.Error("Silahkan input field Alasan");

                var puterror = setInterval(function () {
                    $('#Reason').addClass('error');
                    setTimeout(function () { if ($('#Reason').hasClass("error")) { $('#Reason').removeClass('error'); } }, 250);
                }, 300);
                setTimeout(function () { clearInterval(puterror); }, 2400);

                return;
            }
        }
        //END VALIDATE

        data.Status = ((data.Finish == "true") ? 1 : 0);

        if (!error) {
            $.ajax({
                type: "POST",
                url: SimDms.baseUrl + "cs.api/bpkb/save",
                data: data,
                traditional: true,
                async: false,
                success: function (result) {
                    if (result.success) {
                        //states.transactionStatus = true;
                        refreshData();
                    }
                    $(".page .ajax-loader").fadeOut();

                    widget.showNotification(result.message);
                },
                error: function (XMLHttpRequest, textStatus, errorThrown) {
                    _this.Error(errorThrown);
                    $(".page .ajax-loader").fadeOut();
                }
            });
            //widget.post("cs.api/bpkb/save", data, function (result) {
            //    if (result.success) {
            //        //states.transactionStatus = true;
            //        refreshData();
            //    }

            //    widget.showNotification(result.message);
            //});
        }
    });

    $("#btnDelete").on("click", function () {
        var data = $(".main form").serializeObject();
        widget.confirm("Anda yakin akan menghapus data ini?", function (result) {
            if (result == "Yes") {
                widget.post("cs.api/bpkb/delete", data, function (result) {
                    if (result.success) {
                        $("#btnNew").click();
                    }
                });
            }
        });
    });

    $("#FinishN").on("click", function () {
        $("[name=Reason]").parent().parent().slideDown();
    });

    $("#FinishY").on("click", function () {
        $("[name=Reason]").parent().parent().slideUp();
    });

    function initEvent() {
        $("#btnSavePendingBpkbRetrieval").on("click", function (evt) {
            var url = "cs.api/PendingBpkbRetrieval/Save";
            var params = {
                CustomerCode: $("[name=CustomerCode]").val(),
                RetrievalEstimationDate: $("[name=RetrievalEstimationDate]").val(),
                Notes: $("[name=Notes]").val(),
                Chassis: records["Chassis"]
            };

            widget.post(url, params, function (result) {
                if (result.success) {
                    showPendingBpkbRetrieval(false);
                    refreshGridPendingBpkbRetrieval();
                }

                widget.showNotification(result.message);
            });
        });

        $("#btnCancelPendingBpkbRetrieval").on("click", function (evt) {
            showPendingBpkbRetrieval(false);
        });


        widget.setSelect([
            { name: "Reason", url: "cs.api/Combo/Reasons/" },
        ]);
    }

    function showPendingBpkbRetrieval(state) {
        $("[name=RetrievalEstimationDate]").removeAttr("disabled");
        $("[name=Notes]").removeAttr("readonly");

        if (state) {
            states.lockVisibility = true;
            showGridPendingBpkbRetrieval(false);
            showPanelPendingBpkbRetrieval(true);
        }
        else {
            states.lockVisibility = false;
            showGridPendingBpkbRetrieval(true);
            showPanelPendingBpkbRetrieval(false);
        }
    }

    function refreshData(data) {
        if (widget.isNullOrEmpty(data) == false) {
            records["Chassis"] = data.Chassis;
        }

        var params = undefined;
        if (data === undefined) {
            params = { CompanyCode: $("[name=CompanyCode]").val(), CustomerCode: $("[name=CustomerCode]").val(), Chassis: $("[name=Chassis]").val() };
        } else {
            params = { CompanyCode: data.CompanyCode, CustomerCode: data.CustomerCode, Chassis: data.Chassis };
        }

        if (!widget.isNullOrEmpty(params.Chassis)) {
            widget.post("cs.api/bpkb/get", params, function (result) {
                if (result.success) {
                    var ErrorOnGet = false, msg = "";
                    if (widget.isNullOrEmpty(result.data.BpkbReadyDate) == true) {
                        msg += 'Field "BPKB Ready date" kosong (BPKB tsb belum diterima). ';
                        ErrorOnGet = true;
                    }

                    if (widget.isNullOrEmpty(result.data.BpkbPickUp) == false) {
                        var BpkbPickUp = widget.toDateFormat(widget.cleanJsonDate(result.data.BpkbPickUp));
                        result.data.ReqInfoLeasing = "true";
                    }
                    else if (result.data.IsLeasing) {
                        msg += 'Field "BPKB Diserahkan" kosong (BPKB tsb belum diserahkan ke Leasing). ';
                        ErrorOnGet = true;
                    }

                    if (ErrorOnGet) {
                        msg += "Silahkan hubungi Sales admin untuk menginput data tsb pada menu SPK & Tracking BBN";
                        widget.alert(msg);
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

                    if (result.data.IsLeasing) {
                        $("[name=Tenor]").parent().parent().slideDown();
                        $("[name=LeasingCo]").parent().parent().parent().parent().slideDown();
                        $("[name=ReqInfoLeasing]").parent().parent().parent().slideDown();
                        $("[name=ReqInfoCust]").parent().parent().parent().slideUp();
                        $("[name=Category]").parent().parent().removeClass("full");
                        $("#gridPendingBpkbRetrieval").parent().slideUp();
                        $("#gridPendingBpkbRetrieval").parent().siblings().slideUp();
                        states.transactionStatus = false;
                    }
                    else {
                        $("[name=Tenor]").parent().parent().slideUp();
                        $("[name=LeasingCo]").parent().parent().parent().parent().slideUp();
                        $("[name=ReqInfoLeasing]").parent().parent().parent().slideUp();
                        $("[name=ReqInfoCust]").parent().parent().parent().slideDown();
                        $("[name=Category]").parent().parent().addClass("full");
                        $("#gridPendingBpkbRetrieval").parent().slideDown();
                        $("#gridPendingBpkbRetrieval").parent().siblings().slideDown();
                        states.transactionStatus = true;
                    }

                    if (widget.isNullOrEmpty(result.data) == false) {
                        if (widget.isNullOrEmpty(result.data.BpkbDate) == false) {
                            var bpkbDate = widget.toDateFormat(widget.cleanJsonDate(result.data.BpkbDate));
                            $("[name=BpkbDate]").val(bpkbDate);
                        }

                        if (widget.isNullOrEmpty(result.data.StnkDate) == false) {
                            var stnkDate = widget.toDateFormat(widget.cleanJsonDate(result.data.StnkDate));
                            $("[name=StnkDate]").val(stnkDate);
                        }

                        if (widget.isNullOrEmpty(result.data.BpkbReadyDate) == false) {
                            var BpkbReadyDate = widget.toDateFormat(widget.cleanJsonDate(result.data.BpkbReadyDate));
                            $("[name=BpkbReadyDate]").val(BpkbReadyDate);
                        }

                        if (widget.isNullOrEmpty(result.data.BpkbPickUp) == false) {
                            var BpkbPickUp = widget.toDateFormat(widget.cleanJsonDate(result.data.BpkbPickUp));
                            $("[name=BpkbPickUp]").val(BpkbPickUp);
                        }

                    }


                    refreshGridPendingBpkbRetrieval();
                }
            });
        }
        else {
            widget.showNotification('Maaf, data terpilih tidak valid.\nNomor Chassis masih kosong, tolong cek data penjualan customer terpilih pada SDMS.'
                                   + '\nJika ada data yang kurang sesuai, tolong disesuaikan terlebih dahulu.'
                                   );
        }
    }

    function refreshGridPendingBpkbRetrieval() {
        var url = "cs.api/lookup/PendingBpkbRetrieval";
        var params = {
            CompanyCode: $("[name=CompanyCode]").val(),
            CustomerCode: $("[name=CustomerCode]").val()
        };

        widget.kgrid({
            url: url,
            name: "gridPendingBpkbRetrieval",
            params: params,
            columns: [
                { field: "RetrievalEstimationDate", title: "Tanggal Penundaan", width: 100, template: "#= (RetrievalEstimationDate == undefined) ? '' : moment(RetrievalEstimationDate).format('DD MMM YYYY') #" },
                { field: "Notes", title: "Alasan Penundaan", width: 280, maxlength: 200 },
            ],
            toolbars: [
                { name: "btnNewPendingBpkbRetrieval", text: "New Record", icon: "icon-file" },
                { name: "btnEditPendingBpkbRetrieval", text: "Edit", icon: "icon-edit" },
                { name: "btnDeletePendingBpkbRetrieval", text: "Delete", icon: "icon-trash" },
            ],
        }, gridAction);
    }

    function gridAction() {
        $("#btnNewPendingBpkbRetrieval").on("click", function (evt) {
            console.log(states.transactionStatus)
            if (states.transactionStatus) {
                showPendingBpkbRetrieval(true);
                clearPendingBpkpRetrievalInput();
            }
        });

        $("#btnEditPendingBpkbRetrieval").on("click", function (evt) {
            if (states.transactionStatus) {
                widget.selectedRows("gridPendingBpkbRetrieval", function (data) {
                    var params = {
                        CustomerCode: data[0].CustomerCode,
                        RetrievalEstimationDate: widget.toDateFormat(widget.cleanJsonDate(data[0].RetrievalEstimationDate)),
                        Notes: data[0].Notes
                    };

                    clearPendingBpkpRetrievalInput();
                    showPendingBpkbRetrieval(true);

                    $("[name=RetrievalEstimationDate]").attr("disabled", true);
                    $("[name=RetrievalEstimationDate]").val(params.RetrievalEstimationDate);
                    $("[name=Notes]").val(params.Notes);
                });
            }
        });

        $("#btnDeletePendingBpkbRetrieval").on("click", function (evt) {
            if (states.transactionStatus) {
                widget.confirm("Apakah anda yakin ingin mengapus data terpilih ?", function (result) {
                    switch (result) {
                        case "Yes":
                            widget.selectedRows("gridPendingBpkbRetrieval", function (data) {
                                var params = {
                                    CustomerCode: data[0].CustomerCode,
                                    RetrievalEstimationDate: widget.toDateFormat(widget.cleanJsonDate(data[0].RetrievalEstimationDate)),
                                    Chassis: records["Chassis"]
                                };
                                var url = "cs.api/PendingBpkbRetrieval/Delete";

                                widget.post(url, params, function (result) {
                                    if (result.success) {

                                        var params2 = { CompanyCode: $("[name=CompanyCode]").val(), CustomerCode: $("[name=CustomerCode]").val(), Chassis: $("[name=Chassis]").val() };
                                        widget.post("cs.api/bpkb/GetRetrievalEstimation", params2, function (result) {
                                            if (result.success) {
                                                if (widget.isNullOrEmpty(result.data.RetrievalEstimationDate) == true) {
                                                    $('#FinishN').trigger('click');
                                                    $('input[name=Finish]').val('false');
                                                }
                                            }
                                        });


                                        refreshGridPendingBpkbRetrieval();
                                    }

                                    widget.showNotification(result.message);
                                });
                            });
                            break;
                    }
                });
            }
        });
    }

    function checkValidation() {
        var bpkbPickUp = $("[name=BpkbPickUp]").val();
        var panelPendingBpkbRetrieval = $("#gridPendingBpkbRetrieval").parent().parent();

        if (widget.isNullOrEmpty(bpkbPickUp) || bpkbPickUp == "") {
            if (states.lockVisibility == false) {
                panelPendingBpkbRetrieval.fadeIn();
            }
        }
        else {
            if (states.lockVisibility == false) {
                panelPendingBpkbRetrieval.fadeOut();
            }
        }

        //var customerCode = $("[name=CustomerCode]").val();
        //if (widget.isNullOrEmpty(customerCode)) {
        //    states.transactionStatus = false;
        //}
        //else {
        //    states.transactionStatus = true;
        //}

        setTimeout(checkValidation, 1000);
    }

    function showPanelPendingBpkbRetrieval(status) {
        var panel = $("#panelPendingBpkbRetrieval");

        if (status) {
            panel.fadeIn();
        }
        else {
            panel.fadeOut();
        }
    }

    function showGridPendingBpkbRetrieval(status) {
        var panel = $("#gridPendingBpkbRetrieval").parent().parent();

        if (status) {
            panel.fadeIn();
        }
        else {
            panel.fadeOut();
        }
    }

    function clearPendingBpkpRetrievalInput() {
        $("[name='RetrievalEstimationDate'], [name='Notes']").val("");
    }
});

