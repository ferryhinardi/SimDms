$(document).ready(function () {
    var options = {
        title: "Input Master Dealer Mapping",
        xtype: "panels",
        panels: [
            {
                name: "pnlFilter",
                items: [
                    { name: "GroupNo", text: "Area", cls: "span6", type: "select", opt_text: "-- SELECT ALL --" },
                    { name: "CompanyCode", text: "Dealer", cls: "span6", type: "select", opt_text: "-- SELECT ALL --" },
                    { name: "BranchCode", text: "Outlet", cls: "span6", type: "select", opt_text: "-- SELECT ALL --" },
                    { type: "divider", text: "" },
                    {
                        text: "Area Service",
                        type: "controls",
                        items: [
                            { name: "GroupNoSrv", text: "Area", cls: "span6", type: "select" },
                            //{ name: "AreaCodeInput", text: "Area Code", type: "text", cls: "span2", maxlength: 10 },
                            //{ name: "AreaNameInput", text: "Area Dealer", type: "text", cls: "span2", maxlength: 100 },
                            //{
                            //    cls: "span2", type: "buttons", items: [
                            //        { name: "btnSaveArea", action: 'save', text: ' Save', icon: 'fa fa-save' },
                            //        { name: "btnEditArea", action: 'Edit', text: ' Edit', icon: 'fa fa-pencil', cls: 'hide' }
                            //    ],
                            //},
                        ]
                    },
                    {
                        text: "Dealer Service",
                        type: "controls",
                        items: [
                            { name: "CompanyCodeInput", text: "Kode Dealer Service", type: "text", cls: "span1", maxlength: 10 },
                            { name: "CompanyNameInput", text: "Nama Dealer Service", type: "text", cls: "span3", maxlength: 100 },
                            {
                                cls: "span3", type: "buttons", items: [
                                    { name: "btnSaveCompany", action: 'save', text: ' Save', icon: 'fa fa-save' },
                                    { name: "btnEditCompany", action: 'edit', text: ' Edit', icon: 'fa fa-pencil', cls: 'hide' },
                                    //{ name: "btnDeleteCompany", action: 'delete', text: ' Delete', icon: 'fa fa-trash-o', cls: 'hide' },
                                    { name: "btnCancelCompany", action: 'cancel', text: ' Cancel', icon: 'fa fa-cancel', cls: 'hide' },
                                ],
                            },
                        ]
                    },
                    {
                        text: "Outlet Service",
                        type: "controls",
                        items: [
                            { name: "BranchCodeInput", text: "Kode Outlet Service", type: "text", cls: "span1", maxlength: 10 },
                            { name: "BranchNameInput", text: "Nama Outlet Service", type: "text", cls: "span3", maxlength: 100 },
                            {
                                cls: "span3", type: "buttons", items: [
                                    { name: "btnSaveBranch", action: 'save', text: ' Save', icon: 'fa fa-save' },
                                    { name: "btnEditBranch", action: 'Edit', text: ' Edit', icon: 'fa fa-pencil', cls: 'hide' },
                                    { name: "btnDeleteBranch", action: 'delete', text: ' Delete', icon: 'fa fa-trash-o', cls: 'hide' },
                                    { name: "btnCancelBranch", action: 'cancel', text: ' Cancel', icon: 'fa fa-cancel', cls: 'hide' },
                                ],
                            },
                        ]
                    },
                ],
            },
        ],
        toolbars: [
            { action: "export", text: "Export", icon: "fa fa-download" },
            { action: 'expand', text: 'Expand', icon: 'fa fa-expand' },
            { action: 'collapse', text: 'Collapse', icon: 'fa fa-compress', cls: 'hide' },
        ],
        onToolbarClick: function (action) {
            switch (action) {
                case 'export':
                    exportXls();
                    break;
                default:
                    break;
            }
        }
    }
    widget = new SimDms.Widget(options);
    /*widget.setSelect([
        { name: "GroupArea", url: "wh.api/combo/GroupAreas", optionalText: "-- SELECT ALL --" },
        { name: "CompanyCode", url: "wh.api/combo/DealerList", params: { LinkedModule: "", GroupArea: "" }, optionalText: "-- SELECT ALL --" },
        { name: "BranchCode", url: "wh.api/combo/branchs", params: { comp: "" }, optionalText: "-- SELECT ALL --" }
    ]);*/
    widget.setSelect([
        { name: "GroupNo", url: "wh.api/combo/GroupAreas", optionalText: "-- SELECT ALL --" },
        { name: "GroupNoSrv", url: "wh.api/combo/SrvGroupAreas" },
    ]);
    widget.render(function () {
        $("[name=GroupNo]").on("change", function () {
            var value = $(this).val();
            if (value == "") {
                widget.select({ selector: "[name=CompanyCode]", data: [], optionText: "-- SELECT ALL --" });
                resetGroupArea();
            }
            else {
                console.log($('#GroupNo').val());
                widget.select({ selector: "[name=CompanyCode]", url: "wh.api/combo/DealerList", params: { LinkedModule: "", GroupArea: $('#GroupNo').val() }, optionalText: "-- SELECT ALL --" });
                widget.post("wh.api/SvMaster/findComboSrvByComboMarketing", { Type: "AREA", GroupNo: $('#GroupNo').val() }, function (data) {
                    var result = data[0]; 
                    if (result.length > 0) {
                        $("#AreaCodeInput").prop("disabled", true).val(result[0].Value);
                        $("#AreaNameInput").prop("disabled", true).val(result[0].Text);
                        $("#btnSaveArea").hide();
                        $("#btnEditArea").show();
                    }
                    else {
                        resetGroupArea();
                    }
                });
            }
            $("[name=GroupNoSrv]").select2('val', '');
            $("[name=CompanyCode]").prop("selectedIndex", 0);
            $("[name=CompanyCode]").change();
        });
        $("[name=CompanyCode]").on("change", function () {
            var value = $(this).val();
            if (value == "") {
                widget.select({ selector: "[name=BranchCode]", data: [], optionText: "-- SELECT ALL --" });
                resetCompany();
            }
            else {
                widget.select({ selector: "[name=BranchCode]", url: "wh.api/combo/BranchList", params: { companyCode: value, group: $('#GroupNo').val() }, optionalText: "-- SELECT ALL --" });
                refreshInput("Company", $('#GroupNo').val(), value, null);
            }
            $("[name=BranchCode]").prop("selectedIndex", 0);
            $("[name=BranchCode]").change();
        });
        $("[name=BranchCode]").on("change", function () {
            var value = $(this).val();
            if (value == "")
                resetBranch();
            else {
                refreshInput("Branch", $('#GroupNo').val(), $("[name=CompanyCode]").val(), value);
            }
        });

        // $("#btnEditArea, #btnEditCompany, #btnEditBranch").click(function (e) {
        $("#btnEditCompany, #btnEditBranch").click(function (e) {
            var id = this.id;
            var type = id.replace("btnEdit", "");
            $("#" + type + "CodeInput, #" + type + "NameInput").prop("disabled", false);
            $(this).hide();
            $("#btnSave" + type).show();
            $("#btnDelete" + type).show();
            $("#btnCancel" + type).show();
        });

        $("#btnSaveCompany, #btnSaveBranch").click(function (e) {
            var id = this.id;
            var type = id.replace("btnSave", "");
            if (type == "Company") type = "DEALER"
            else if (type == "Branch") type = "OUTLET"
            var param = $("#pnlFilter").serializeObject();
            param.Type = type;
            if ($("#GroupNoSrv").val() != "") param.GroupAreaSrv = $("#GroupNoSrv option:selected").text();

            if (param.GroupNo == "") {
                sdms.info("Please Select Area!");
                return;
            }
            if (type == "DEALER") {
                if (param.CompanyCode == "") {
                    sdms.info("Please Select Dealer!");
                    return;
                }
            }
            if (type == "OUTLET") {
                if (param.BranchCode == "") {
                    sdms.info("Please Select Outlet!");
                    return;
                }
            }
            widget.post("wh.api/SvMaster/SaveMappingComboSrvMarketing", param, function (data) {
                if (data.length) {
                    if (data[0] == 1) {
                        sdms.info("Save Success");
                        if (type == "DEALER") {
                            refreshInput("Company", $('#GroupNo').val(), $("[name=CompanyCode]").val(), null);
                            resetCompany();
                        }
                        else if (type == "OUTLET") {
                            refreshInput("Branch", $('#GroupNo').val(), $("[name=CompanyCode]").val(), $("[name=BranchCode]").val());
                            resetBranch();
                        }
                    }
                    else
                        sdms.info("Save Error");
                }
            });
        });

        $("#btnDeleteCompany, #btnDeleteBranch").click(function (e) {
            var id = this.id;
            var type = id.replace("btnDelete", "");
            if (type == "Company") type = "DEALER"
            else if (type == "Branch") type = "OUTLET"
            var param = $("#pnlFilter").serializeObject();
            param.Type = type;

            if (param.GroupNo == "") {
                sdms.info("Please Select Area!");
                return;
            }
            if (type == "DEALER") {
                if (param.CompanyCode == "") {
                    sdms.info("Please Select Dealer!");
                    return;
                }
            }
            if (type == "OUTLET") {
                if (param.BranchCode == "") {
                    sdms.info("Please Select Outlet!");
                    return;
                }
            }
            if (confirm("Do you want to delete data?")) {
                widget.post("wh.api/SvMaster/DeleteMappingComboSrvMarketing", param, function (data) {
                    if (data.length) {
                        if (data[0] == 1) {
                            sdms.info("Delete Success");
                            if (type == "DEALER") {
                                refreshInput("Company", $('#GroupNo').val(), $("[name=CompanyCode]").val(), null);
                                resetCompany();
                            }
                            else if (type == "OUTLET") {
                                refreshInput("Branch", $('#GroupNo').val(), $("[name=CompanyCode]").val(), $("[name=BranchCode]").val());
                                resetBranch();
                            }
                        }
                        else
                            sdms.info("Delete Error");
                    }
                });
            }
        });

        $("#btnCancelCompany, #btnCancelBranch").click(function (e) {
            var id = this.id;
            var type = id.replace("btnCancel", "");
            refreshInput(type, $('#GroupNo').val(), $("[name=CompanyCode]").val(), $("[name=BranchCode]").val());
            $("#btnDelete" + type).hide();
            $("#btnCancel" + type).hide();
        });
    });
    function resetGroupArea() {
        /*$("#AreaCodeInput, #AreaNameInput").prop("disabled", false).val("");
        $("#btnSaveArea").show();
        $("#btnEditArea").hide();*/
    }

    function resetCompany() {
        $("#CompanyCodeInput, #CompanyNameInput").prop("disabled", false).val("");
        $("#btnSaveCompany").show();
        $("#btnDeleteCompany").hide();
        $("#btnCancelCompany").hide();
        $("#btnEditCompany").hide();
    }

    function resetBranch() {
        $("#BranchCodeInput, #BranchNameInput").prop("disabled", false).val("");
        $("#btnSaveBranch").show();
        $("#btnDeleteBranch").hide();
        $("#btnCancelBranch").hide();
        $("#btnEditBranch").hide();
    }

    function resetAll() {
        $("[name=GroupNo]").select2('val', '').trigger("change");
    }

    function refreshInput(Type, GroupNo, CompanyCode, BranchCode) {
        if (Type == "Company") {
            $("#btnSave" + Type).hide();
            $("#btnDelete" + Type).hide();
            $("#btnCancel" + Type).hide();
            $("#btnEdit" + Type).show();
            widget.post("wh.api/SvMaster/findComboSrvByComboMarketing", { Type: "DEALER", GroupNo: GroupNo, CompanyCode: CompanyCode }, function (data) {
                var result = data[0];
                if (result.length > 0) {
                    $("#GroupNoSrv").select2('val', result[0].Area);
                    $("#CompanyCodeInput").prop("disabled", true).val(result[0].Value);
                    $("#CompanyNameInput").prop("disabled", true).val(result[0].Text);
                    $("#btnSave" + Type).hide();
                    $("#btnEdit" + Type).show();
                }
                else {
                    resetCompany();
                }
            });
        }
        else if (Type == "Branch") {
            widget.post("wh.api/SvMaster/findComboSrvByComboMarketing", { Type: "OUTLET", CompanyCode: CompanyCode, BranchCode: BranchCode }, function (data) {
                var result = data[0];
                if (result.length > 0) {
                    $("#GroupNoSrv").select2('val', result[0].Area);
                    $("#BranchCodeInput").prop("disabled", true).val(result[0].Value);
                    $("#BranchNameInput").prop("disabled", true).val(result[0].Text);
                    $("#btnSave" + Type).hide();
                    $("#btnEdit" + Type).show();
                }
                else {
                    resetBranch();
                }
            });
        }
    }

    function exportXls() {
        var params = $("#pnlFilter").serializeObject();

        widget.post("wh.api/report/MappingSrvGnReport", params, function (data) {
            if (data.message == "") {
                location.href = 'wh.api/report/DownloadExcelFile?key=' + data.value + '&filename=Report Mapping';
            } else {
                sdms.info(data.message, "Error");
            }
        });
    }
});