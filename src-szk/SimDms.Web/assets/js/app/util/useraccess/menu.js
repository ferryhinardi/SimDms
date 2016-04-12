//$(document).ready(function () {
//    var options = {
//        title: "Management Menu",
//        xtype: "panels",
//        panels: [
//            {
//                name: "panelInput",
//                items: [
//                    { type: 'text', name: "MenuId", text: "Menu ID", cls: "span4  ignore-uppercase", required: true },
//                    { type: 'text', name: "MenuCaption", text: "Menu Caption", cls: "span4  ignore-uppercase", required: true },
//                    { type: 'select', name: "MenuHeader", text: "Menu Header", cls: "span4  ignore-uppercase", required: true },
//                    { type: 'spinner', name: "MenuIndex", text: "Menu Index", cls: "span4  ignore-uppercase", required: true },
//                    { type: 'spinner', name: "MenuLevel", text: "Menu Level", cls: "span4  ignore-uppercase", required: true },
//                    { type: 'text', name: "MenuUrl", text: "Menu Url", cls: "span4  ignore-uppercase", required: false },
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
//            url: "util/grid/menus",
//            name: "gridRole",
//            params: params,
//            columns: [
//                { field: "MenuId", title: "Menu ID", width: 200 },
//                { field: "MenuCaption", title: "Caption", width: 200 },
//                { field: "MenuHeaderDescription", title: "Header", width: 200 },
//                { field: "MenuIndex", title: "Index", width: 200 },
//                { field: "MenuLevel", title: "Level", width: 200 },
//                { field: "MenuUrl", title: "Url", width: 200 },
//            ],
//        });
//    }

//    function btnAdd_click() {
//        showPanelInput(true);
//        widget.enableElements(['MenuId']);
//        widget.clearForm();
//        widget.showToolbars(['btnSave', 'btnCancel']);
//    }

//    function btnEdit_click() {
//        widget.selectedRow("gridRole", function (data) {
//            if (!widget.isNullOrEmpty(data)) {
//                widget.disableElements(['MenuId']);
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

//                    widget.post('util/menu/delete', data, function (result) {
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
//            var url = 'util/menu/save';

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
        title: "Management Menu",
        xtype: "grid-form",
        urlList: "util/grid/menus",
        toolbars: [
            { name: "btnCreate", text: "Create", icon: "fa fa-file" },
            { name: "btnEdit", text: "Edit", icon: "fa fa-edit" },
            { name: "btnDelete", text: "Delete", icon: "fa fa-trash-o" },
            { name: "btnSave", text: "Save", icon: "fa fa-save", cls: "hide" },
            { name: "btnCancel", text: "Cancel", icon: "fa fa-undo", cls: "hide" },
        ],
        items: [
            //{
            //    text: "Company",
            //    type: "controls",
            //    items: [
            //        { name: "CompanyCode", cls: "span2", placeHolder: "Company Code", readonly: true },
            //        { name: "CompanyName", cls: "span6", placeHolder: "Company Name", readonly: true }
            //    ]
            //},
            { name: "MenuId", text: "Menu ID", cls: "span4 full ignore-uppercase", required: true },
            { name: "MenuCaption", text: "Caption", cls: "ignore-uppercase", required: true },
            { name: "MenuHeader", text: "Header", cls: "span4", type: "select" },
            { name: "MenuIndex", text: "Index", cls: "span4", type: "spinner" },
            { name: "MenuLevel", text: "Level", cls: "span4", type: "spinner" },
            { name: "MenuUrl", text: "Url", cls: "span4 ignore-uppercase" }
        ],
        columns: [
            { mData: "MenuId", sTitle: "Menu ID", sWidth: "180px" },
            { mData: "MenuCaption", sTitle: "Caption" },
            { mData: "MenuHeaderDescription", sTitle: "Header", sWidth: "180px" },
            { mData: "MenuIndex", sTitle: "Index", sWidth: "180px" },
            { mData: "MenuLevel", sTitle: "Level", sWidth: "180px" },
            { mData: "MenuUrl", sTitle: "Url", sWidth: "180px" }
        ],
    }
    var widget = new SimDms.Widget(options);
    widget.default = {};
    widget.render(function () {
        //$.post("util/menu/default", function (result) {
        //    widget.default = result;
        //    widget.populate(result);
        //    widget.select({ selector: "[name=MenuHeader]", url: "util/combo/menus" });
        //});
        widget.select({ selector: "[name=MenuHeader]", url: "util/Menu/Headers" });
    });

    $("#btnCreate").on("click", function () {
        $(".main .gl-form input").val("");
        $(".main .gl-form").show();
        $(".main .gl-grid").hide();
        widget.showToolbars(["btnSave", "btnCancel"]);
        widget.populate(widget.default);
    });

    $("#btnSave").on("click", function () {
        var valid = $(".main form").valid();
        if (valid) {
            var data = $(".main form").serializeObject();
            widget.post("util.api/menu/save", data, function (result) {
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
            widget.showToolbars(["btnSave", "btnCancel"]);
            widget.populate(row, function () {
                $(".main .gl-form").show();
                $(".main .gl-grid").hide();
            });
        }
    });

    $("#btnDelete").on("click", function () {
        var row = widget.selectedRow();
        if (row !== undefined) {
            if (confirm("Anda yakin akan menghapus data ini?")) {
                widget.post("util.api/menu/delete", row, function (result) {
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
            };
        }
    });

    $("#btnCancel").on("click", function () {
        $(".main .gl-form").hide();
        $(".main .gl-grid").show();
        widget.showToolbars(["btnCreate", "btnEdit", "btnDelete"]);
    });
});