$(document).ready(function () {
    var options = {
        xtype: "panels",
        title: "Management User",
        toolbars: [
            { name: "btnCreate", text: "Create", icon: "icon-file", cls: "button small" },
            { name: "btnSave", text: "Save", icon: "icon-save", cls: "button small" },
            { name: "btnCancel", text: "Cancel", icon: "icon-undo", cls: "button small" },
            { name: "btnEdit", text: "Edit", icon: "icon-edit", cls: "button small" },
            //{ name: "btnDelete", text: "Delete", icon: "icon-trash", cls: "button small" },
        ],
        panels: [
            {
                name: "panelFilter",
                items: [
                    { name: "filterUserId", text: "User ID", cls: "span3 full  ignore-uppercase" },
                    { name: "filterFullName", text: "Full Name", cls: "span5 full  ignore-uppercase" },
                    { name: "filterEmail", text: "Email", cls: "span5 full  ignore-uppercase" },
                    {
                        type: "buttons",
                        items: [
                            { name: "btnSearch", text: "Search", cls: "button small", icon: "icon-search" },
                            { name: "btnClear", text: "Clear", cls: "button small", icon: "icon-eraser" },
                        ]
                    }
                ]
            },
            {
                name: "gridUser",
                xtype: "k-grid"
            },
            {
                name: "panelInput",
                items: [
                    {
                        text: "Company",
                        type: "controls",
                        items: [
                            { name: "CompanyCode", cls: "span2", placeHolder: "Company Code", readonly: true },
                            { name: "CompanyName", cls: "span6", placeHolder: "Company Name", readonly: true }
                        ]
                    },
                    { name: "UserId", text: "Kode User", cls: "span4 full ignore-uppercase", required: true, maxlength: 20 },
                    { name: "FullName", text: "Nama Lengkap", required: true, cls: " ignore-uppercase", maxlength: 100 },
                    { name: "Email", text: "Email", cls: "span5 ignore-uppercase", maxlength: 50 },
                    { name: "RoleId", text: "Role", cls: "span6  ignore-uppercase", type: "select", required: true },
                    { name: "BranchCode", text: "BranchCode", cls: "span8", type: "select", required: true },
                    { name: "TypeOfGoods", text: "Type Part", cls: "span4", type: "select" },
                    { name: "ProfitCenter", text: "Profit Center", cls: "span4", type: "select", required: true },
                    { name: "IsActive", text: "Active", cls: "span4", type: "switch", float: "left" },
                    {
                        type: "buttons", items: [
                            { name: "btnReset", text: "Reset Password", icon: "fa fa-eraser", cls: "hide" }
                        ]
                    },
                ]
            },
        ]
    };

    var vars = {};
    var widget = new SimDms.Widget(options);
    widget.setSelect([
        { name: "RoleId", url: "gn.api/combo/roles" },
        { name: "BranchCode", url: "gn.api/combo/branchs" },
        { name: "TypeOfGoods", url: "gn.api/combo/typeofgoods" },
        { name: "ProfitCenter", url: "gn.api/combo/profitcenters" },
    ]);
    widget.render(renderCallback);

    function renderCallback() {
        initElementEvent();
        widget.showToolbars(["btnCreate", "btnEdit", "btnDelete"]);
        showPanelInput(false);
        reloadGrid();

        widget.post("gn.api/User/Default", function (result) {
            vars.default = result;
        });
    }

    function initElementEvent() {
        $("#btnCreate").on("click", function () {
            widget.clearPanelInputs(["panelInput"]);
            showPanelInput(true);
            widget.showToolbars(["btnSave", "btnCancel"]);
            $("[name='UserId']").removeAttr("disabled");
            $('#btnReset').hide();
            widget.populate(vars.default);
        });
        $("#btnEdit").on("click", function () {
            widget.selectedRow("gridUser", function (data) {
                showPanelInput(true);
                widget.populate($.extend(data, vars.default));
                $("[name='UserId']").attr("disabled", true);
                $('#btnReset').show();
                widget.showToolbars(["btnSave", "btnCancel"]);
            });
        });
        $("#btnDelete").on("click", function () {
            widget.confirm("Do you want to delete this data?", function (result) {
                if (result == "Yes") {
                    widget.selectedRow("gridUser", function (data) {
                        var url = "gn.api/User/Delete";
                        var params = {
                            UserId: data.UserId
                        };

                        widget.post(url, params, function (result) {
                            if (result.status) {
                                reloadGrid();
                            }

                            widget.showNotification(result.message);
                        });
                    });
                }
            });
        });
        $("#btnSave").on("click", function () {
            var validation = widget.validate();

            if (validation) {
                var data = $("#panelInput").serializeObject();
                var url = "gn.api/User/Save";

                widget.post(url, data, function (result) {
                    if (result.status) {
                        reloadGrid();
                        showPanelInput(false);
                        widget.showToolbars(["btnCreate", "btnEdit", "btnDelete"]);
                    }

                    widget.showNotification(result.message);
                });
            }
        });
        $("#btnCancel").on("click", function () {
            widget.showToolbars(["btnCreate", "btnEdit", "btnDelete"]);
            showPanelInput(false);
        });
        $("[name='UserId'], [name='FullName'], [name='Email']").on("change", function () {
            reloadGrid();
        });
        $("#btnSearch").on("click", function (evt) {
            reloadGrid();
        });
        $("#btnClear").on("click", function () {
            widget.clearPanelInputs(["panelFilter"]);
            reloadGrid();
        });

        $('#btnReset').on('click', function (e) {
            var data = $("#panelInput").serializeObject();
            if (data !== undefined) {
                widget.confirm("Apakah anda ingin me-reset password???", function (result) {
                    if (result == "Yes") {
                        widget.post('gn.api/User/reset', data, function (result) {
                            if (result.status) {
                                widget.showNotification(result.message || SimDms.defaultInformationMessage);
                            }
                            else {
                                widget.showNotification(result.message || SimDms.defaultErrorMessage);
                            }
                        });
                    }
                });
            }
        });

    }

    function reloadGrid() {
        var data = $("#panelFilter").serializeObject();
        widget.kgrid({
            url: "gn.api/Grid/Users",
            name: "gridUser",
            params: data,
            columns: [
                { field: "UserId", title: "User ID", width: 100 },
                { field: "FullName", title: "Name", width: 200 },
                { field: "Email", title: "Email", width: 150 },
                { field: "RoleName", title: "Role", width: 200 },
                { field: "BranchCode", title: "Branch", width: 100 },
                { field: "IsActive", title: "Is Active", width: 100, template: "#= (IsActive)==true ? 'Yes' : 'No' #" },
            ],
            onDblClick: function (a, b, c) {
                $("#btnEdit").click();
            }
        });
    }
    
    function showPanelInput(state) {
        if (state) {
            widget.hidePanel([
                { name: "panelFilter", type: "" },
                { name: "gridUser", type: "k-grid" },
            ]);
            widget.showPanel([
                { name: "panelInput", type: "", hideDivider: true }
            ]);
        }
        else {
            widget.showPanel([
                { name: "panelFilter", type: "" },
                { name: "gridUser", type: "k-grid" },
            ]);
            widget.hidePanel(["panelInput"]);
        }
    }

    $("#ProfitCenter").on("change", function () {
         //alert($("#ProfitCenter").val());
        if ($("#ProfitCenter").val()=='300') {
            $("#TypeOfGoods").attr('required', 'required');
        } else {
            //alert($("#ProfitCenter").val());
            $("#TypeOfGoods").removeAttr('required');
        }
    });

});