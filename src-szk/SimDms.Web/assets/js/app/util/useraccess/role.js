$(document).ready(function () {
    var options = {
        title: "Management Role",
        xtype: "panels",
        panels: [
            {
                name: "panelInput",
                items: [
                    { type: 'text', name: "RoleId", text: "Role ID", cls: "span4 full ignore-uppercase", required: true },
                    { type: 'text', name: "RoleName", text: "Role Name", cls: "span4 full", required: true },
                    { type: 'text', name: "Description", text: "Description", cls: "span8 full", required: true },
                ]
            },
            { name: "gridRole", xtype: "k-grid", cls: 'hide' },
        ],
        toolbars: [
            { name: "btnAdd", text: "New", icon: "fa fa-file" },
            { name: "btnEdit", text: "Edit", icon: "fa fa-edit" },
            { name: "btnDelete", text: "Delete", icon: "fa fa-trash-o" },
            { name: "btnSave", text: "Save", icon: "fa fa-save", cls: 'hide' },
            { name: "btnCancel", text: "Cancel", icon: "fa fa-refresh", cls: 'hide' },
        ],
    }
    var widget = new SimDms.Widget(options);
    widget.render(renderCallback);
    

    function renderCallback() {
        initElementEvents();
        reloadGrid();
        showPanelInput(false);
    }

    function showPanelInput(state) {
        var panelInput = $('#panelInput');
        var gridRole = $('#gridRole');

        if (state) {
            gridRole.hide();
            panelInput.fadeIn();
        }
        else {
            panelInput.hide();
            gridRole.fadeIn();
        }
    }

    function initElementEvents() {
        var btnAdd = $('#btnAdd');
        var btnEdit = $('#btnEdit');
        var btnDelete = $('#btnDelete');
        var btnSave = $('#btnSave');
        var btnCancel = $('#btnCancel');

        btnAdd.off();
        btnAdd.on('click', function () {
            btnAdd_click();
        });

        btnEdit.off();
        btnEdit.on('click', function () {
            btnEdit_click();
        });

        btnDelete.off();
        btnDelete.on('click', function () {
            btnDelete_click();
        });

        btnSave.off();
        btnSave.on('click', function () {
            btnSave_click();
        });

        btnCancel.off();
        btnCancel.on('click', function () {
            btnCancel_click();
        });

    }

    function reloadGrid() {
        var params = {
            CompanyCode: $("[name=CompanyCode]").val(),
            Branch: $("[name=BranchCode]").val(),
            Position: $("[name=Position]").val(),
            Status: $("[name=Status]").val(),
        }
        widget.kgrid({
            url: "util/grid/roles",
            name: "gridRole",
            params: params,
            columns: [
                { field: "RoleId", title: "Role ID", width: 400 },
                { field: "RoleName", title: "Role Name" },
                { field: "Description", title: "Description", width: 400 },
            ],
        });
    }

    function btnAdd_click() {
        showPanelInput(true);
        widget.clearForm();
        widget.enableElements(['RoleId']);
        widget.showToolbars(['btnSave', 'btnCancel']);
    }

    function btnEdit_click() {
        widget.selectedRow("gridRole", function (data) {
            if (!widget.isNullOrEmpty(data)) {
                showPanelInput(true);
                widget.disableElements(['RoleId']);
                widget.clearForm();
                widget.showToolbars(['btnSave', 'btnCancel']);
                widget.populate(data);
            }
        });
    }

    function btnDelete_click() {
        widget.selectedRow("gridRole", function (data) {
            if (!widget.isNullOrEmpty(data)) {
                if (confirm('Apakah anda ingin menghapus data ini?')) {
                    delete data['parent'];
                    delete data['_events'];
                    delete data['__proto__'];
                    delete data['Users'];
                    delete data['init'];
                    delete data['uid'];
                    data['__proto__'] = null;

                    widget.post('util/role/delete', data, function (result) {
                        if (result.status) {
                            reloadGrid();
                        }

                        widget.showNotification(result.message);
                    });
                }
            }
        });
    }

    function btnSave_click() {
        if (widget.validate()) {
            var params = widget.serializeObject('panelInput');
            var url = 'util/role/save';

            widget.post(url, params, function (result) {
                if (result.status) {
                    showPanelInput(false);
                    reloadGrid();
                    widget.showToolbars(["btnAdd", "btnEdit", "btnDelete"]);
                    widget.showNotification(result.message || SimDms.defaultInformationMessage);
                }
                else {
                    widget.showNotification(result.message || SimDms.defaultErrorMessage);
                }
            });
        }
    }

    function btnCancel_click() {
        showPanelInput(false);
        widget.showToolbars(['btnAdd', 'btnEdit', 'btnDelete']);
    }
});