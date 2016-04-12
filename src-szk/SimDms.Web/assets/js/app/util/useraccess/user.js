//$(document).ready(function () {
//    var options = {
//        title: "Management User",
//        xtype: "panels",
//        panels: [
//            {
//                name: "panelInput",
//                items: [
//                    { type: 'hidden', name: "UserId", text: "", cls: "" },
//                    { type: 'text', name: "Username", text: "Username", cls: "span4 ignore-uppercase", required: true },
//                    { type: 'text', name: "Email", text: "Email", cls: "span4 ignore-uppercase", required: true },
//                    { type: 'text', name: "FirstName", text: "First Name", cls: "span4  ignore-uppercase", required: true },
//                    { type: 'text', name: "LastName", text: "Last Name", cls: "span4  ignore-uppercase", required: true },
//                    { type: 'select', name: "DealerCode", text: "Dealer", cls: "span4  ignore-uppercase", required: true },
//                    { type: 'select', name: "RoleId", text: "Role", cls: "span4  ignore-uppercase", required: true },
//                    { type: 'switch', name: "IsApproved", text: "Is Approved", cls: "span4  ignore-uppercase", required: true },
//                ]
//            },
//            { name: "gridRole", xtype: "k-grid", cls: 'hide' },
//        ],
//        toolbars: [
//            { name: "btnAdd", text: "New", icon: "fa fa-file" },
//            { name: "btnEdit", text: "Edit", icon: "fa fa-edit" },
//            { name: "btnDelete", text: "Delete", icon: "fa fa-trash-o" },
//            { name: "btnSave", text: "Save", icon: "fa fa-save", cls: 'hide' },
//            { name: "btnCancel", text: "Cancel", icon: "fa fa-refresh", cls: 'hide' },
//        ],
//    }
//    var widget = new SimDms.Widget(options);
//    widget.render(renderCallback);


//    function renderCallback() {
//        initElementEvents();
//        reloadGrid();
//        showPanelInput(false);
//        initElementStates();
//    }

//    function initElementStates() {
//        widget.setSelect([
//            { name: "DealerCode", url: "util/user/dealerlist" },
//            { name: "RoleId", url: "util/role/list" }
//        ]);
//    }

//    function showPanelInput(state) {
//        var panelInput = $('#panelInput');
//        var gridRole = $('#gridRole');

//        if (state) {
//            gridRole.hide();
//            panelInput.fadeIn();
//        }
//        else {
//            panelInput.hide();
//            gridRole.fadeIn();
//        }
//    }

//    function initElementEvents() {
//        var btnAdd = $('#btnAdd');
//        var btnEdit = $('#btnEdit');
//        var btnDelete = $('#btnDelete');
//        var btnSave = $('#btnSave');
//        var btnCancel = $('#btnCancel');

//        btnAdd.off();
//        btnAdd.on('click', function () {
//            btnAdd_click();
//        });

//        btnEdit.off();
//        btnEdit.on('click', function () {
//            btnEdit_click();
//        });

//        btnDelete.off();
//        btnDelete.on('click', function () {
//            btnDelete_click();
//        });

//        btnSave.off();
//        btnSave.on('click', function () {
//            btnSave_click();
//        });

//        btnCancel.off();
//        btnCancel.on('click', function () {
//            btnCancel_click();
//        });

//    }

//    function reloadGrid() {
//        var params = {
//            CompanyCode: $("[name=CompanyCode]").val(),
//            Branch: $("[name=BranchCode]").val(),
//            Position: $("[name=Position]").val(),
//            Status: $("[name=Status]").val(),
//        }
//        widget.kgrid({
//            url: "util/grid/users",
//            name: "gridRole",
//            params: params,
//            columns: [
//                { field: "Username", title: "Username", width: 200 },
//                { field: "FirstName", title: "First Name", width: 150 },
//                { field: "LastName", title: "Last Name", width: 150 },
//                { field: "Email", title: "Email", width: 150 },
//                { field: "RoleName", title: "Role Name", width: 200 },
//                { field: "IsApprovedDescription", title: "Is Approved", width: 150 },
//            ],
//        });
//    }

//    function btnAdd_click() {
//        showPanelInput(true);
//        widget.clearForm();
//        widget.showToolbars(['btnSave', 'btnCancel']);
//    }

//    function btnEdit_click() {
//        widget.selectedRow("gridRole", function (data) {
//            if (!widget.isNullOrEmpty(data)) {
//                showPanelInput(true);
//                widget.clearForm();
//                widget.showToolbars(['btnSave', 'btnCancel']);
//                widget.populate(data);
//                $('[name="DealerCode"]').val(data.DealerCode);
//                $('[name="RoleId"]').val(data.RoleId);
//            }
//        });
//    }

//    function btnDelete_click() {
//        widget.selectedRow("gridRole", function (data) {
//            if (!widget.isNullOrEmpty(data)) {
//                if (confirm('Apakah anda ingin menghapus data ini?')) {
//                    delete data['parent'];
//                    delete data['_events'];
//                    delete data['__proto__'];
//                    delete data['Users'];
//                    delete data['init'];
//                    delete data['uid'];
//                    data['__proto__'] = null;

//                    widget.post('util/user/delete', data, function (result) {
//                        if (result.status) {
//                            reloadGrid();
//                        }

//                        widget.showNotification(result.message);
//                    });
//                }
//            }
//        });
//    }

//    function btnSave_click() {
//        if (widget.validate()) {
//            var params = widget.serializeObject('panelInput');
//            var url = 'util/user/save';

//            widget.post(url, params, function (result) {
//                if (result.status) {
//                    showPanelInput(false);
//                    reloadGrid();
//                    widget.showToolbars(["btnAdd", "btnEdit", "btnDelete"]);
//                    widget.showNotification(result.message || SimDms.defaultInformationMessage);
//                }
//                else {
//                    widget.showNotification(result.message || SimDms.defaultErrorMessage);
//                }
//            });
//        }
//    }

//    function btnCancel_click() {
//        showPanelInput(false);
//        widget.showToolbars(['btnAdd', 'btnEdit', 'btnDelete']);
//    }
//});



$(document).ready(function () {
    var options = {
        title: "Management User",
        xtype: "grid-form",
        urlList: "util/grid/users",
        toolbars: [
            { name: "btnCreate", text: "Create", icon: "fa fa-file" },
            { name: "btnEdit", text: "Edit", icon: "fa fa-edit" },
            { name: "btnDelete", text: "Delete", icon: "fa fa-trash-o" },
            { name: "btnSave", text: "Save", icon: "fa fa-save", cls: "hide" },
            { name: "btnCancel", text: "Cancel", icon: "fa fa-undo", cls: "hide" },
            { name: "btnGenerate", text: "Excel", icon: "fa fa-file-excel-o"},
        ],
        items: [
            { name: "UserId", type: "text", maxlength: "36", text: "", cls: "hide", required: false },
            { name: "Username", maxlength: 250, text: "Username", required: true },
            { name: "Email", type: "text", maxlength: 50, text: "Email", cls: "span4 full" },
            { name: "FirstName", maxlength: 50, text: "First Name", cls: "span4", type: "text", float: "left" },
            { name: "LastName", type: "text", text: "Last Name", cls: "span4 full", float: "left" },
            { name: "DealerCode", type: "select", text: "Dealer", cls: "span4 full", float: "left" },
            { name: "OutletCode", type: "select", text: "Outlet", cls: "span4 full", float: "left", disabled: true },
            { name: "RoleId", type: "select", text: "Role", cls: "span4 full", float: "left" },
            { name: "IsApproved", type: "switch", text: "Is Approved", cls: "span4 full", type: "switch", float: "left" },
            {
                type: "buttons", items: [
                    { name: "btnReset", text: "Reset Password", icon: "fa fa-eraser", cls: "hide" }
                ]
            },
        ],
        columns: [
            { mData: "Username", sTitle: "Username" },
            { mData: "FirstName", sTitle: "First Name", sWidth: "200px" },
            { mData: "LastName", sTitle: "Last Name", sWidth: "200px" },
            { mData: "Email", sTitle: "Email", sWidth: "180px" },
            { mData: "RoleName", sTitle: "Role", sWidth: "180px" },
            { mData: "IsApprovedDescription", sTitle: "Approved", sWidth: "180px" }
        ],
    }
    var widget = new SimDms.Widget(options);
    widget.default = {};
    widget.render(function () {
        widget.setSelect([
            { name: "DealerCode", url: "util/user/dealerlist" },
            { name: "RoleId", url: "util/role/list" }
        ]);

        $.post("util/user/default", function (result) {
            widget.default = result;
            widget.populate(result);
        });
    });

    $("#btnCreate").on("click", function () {
        $(".main .gl-form input").val("");
        $(".main .gl-form").show();
        $(".main .gl-grid").hide();
        $('#btnReset').hide();
        widget.showToolbars(["btnSave", "btnCancel"]);
        widget.populate(widget.default);
    });

    $("#btnSave").on("click", function () {
        var valid = $(".main form").valid();
        if (valid) {
            var data = $(".main form").serializeObject();
            widget.post("util/user/save", data, function (result) {
                if (result.status) {
                    $(".main .gl-form").hide();
                    $(".main .gl-grid").show();
                    widget.refreshGrid();
                    widget.showToolbars(["btnCreate", "btnEdit", "btnDelete"]);
                    widget.showNotification(result.message || SimDms.defaultInformationMessage);
                }
                else {
                    widget.showNotification(result.message || SimDms.defaultErrorMessage);
                }
            });
        }
    });

    $("#btnEdit").on("click", function () {
        var row = widget.selectedRow();
        if (row !== undefined) {
            widget.showToolbars(["btnSave", "btnReset", "btnCancel"]);
            widget.populate(row, function () {
                $(".main .gl-form").show();
                $(".main .gl-grid").hide();
                $('#btnReset').show();
            });
        }
    });

    $("#btnDelete").on("click", function () {
        var row = widget.selectedRow();
        if (row !== undefined) {
            if (confirm("Do you want to delete selected data?")) {
                widget.post("util/user/delete", row, function (result) {
                    if (result.status) {
                        $(".main .gl-form").hide();
                        $(".main .gl-grid").show();
                        widget.showToolbars(["btnCreate", "btnEdit", "btnDelete"]);
                        widget.showNotification(result.message || SimDms.defaultInformationMessage);
                        widget.refreshGrid();
                    }
                    else {
                        widget.showNotification(result.message || SimDms.defaultErrorMessage);
                    }
                });
            };
        }
    });

    $("#btnCancel").on("click", function () {
        $(".main .gl-form").hide();
        $(".main .gl-grid").show();
        widget.showToolbars(["btnCreate", "btnEdit", "btnDelete"]);
    });

    $('#btnReset').on('click', function (e) {
        var row = widget.selectedRow();
        if (row !== undefined) {
            var con = confirm("Apakah anda ingin me-reset password???");
            if (con) {
                widget.post('util/user/reset', row, function (result) {
                    if (result.status) {
                        widget.showNotification(result.message + " , default password = 123456" || SimDms.defaultInformationMessage);
                    }
                    else {
                        widget.showNotification(result.message || SimDms.defaultErrorMessage);
                    }
                });
            }
        }
    });

    $('#DealerCode').on('change', function (e) {
        var dealerCode = $('#DealerCode').val();
        if (dealerCode != '')
        {
            widget.select({ selector: "#OutletCode", url: "util/user/outletlist?dealerCode=" + dealerCode });
            $('#OutletCode').removeAttr('disabled');
        }
        else
        {
            widget.select({ selector: "#OutletCode", url: "util/user/outletlist?dealerCode=" + dealerCode });
            $('#OutletCode').attr('disabled', 'disabled');
        }
    });

    $('#btnGenerate').on('click', function (e) {
        widget.XlsxReport({
            url: 'util/user/generateuser',
            type: 'xlsx',
        });
    });

});