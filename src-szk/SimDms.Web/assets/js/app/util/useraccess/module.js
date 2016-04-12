//$(document).ready(function () {
//    var options = {
//        title: "Management Module",
//        xtype: "panels",
//        panels: [
//            {
//                name: "panelInput",
//                items: [
//                    { type: 'text', name: "ModuleId", text: "Module ID", cls: "span4 full ignore-uppercase", required: true },
//                    { type: 'text', name: "ModuleCaption", text: "Module Caption", cls: "span4 full ignore-uppercase", required: true },
//                    { type: 'spinner', name: "ModuleIndex", text: "Module Index", cls: "span4 full ignore-uppercase", required: true },
//                    { type: 'text', name: "ModuleUrl", text: "Module Url", cls: "span4 full ignore-uppercase", required: false },
//                    { type: 'switch', name: "InternalLink", text: "Is Internal Link", cls: "span4 full ignore-uppercase", required: true },
//                    { type: 'switch', name: "IsPublish", text: "Is Published", cls: "span4 full ignore-uppercase", required: true },
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
//            url: "util/grid/modules",
//            name: "gridRole",
//            params: params,
//            columns: [
//                { field: "ModuleId", title: "Module ID", width: 200 },
//                { field: "ModuleCaption", title: "Caption", width: 400 },
//                { field: "ModuleIndex", title: "Index", width: 200 },
//                { field: "ModuleUrl", title: "Url", width: 200 },
//                { field: "InternalLink", title: "Is Internal Link", width: 200 },
//                { field: "IsPublish", title: "Is Published", width: 200 },
//            ],
//        });
//    }

//    function btnAdd_click() {
//        widget.enableElements(['ModuleId']);
//        showPanelInput(true);
//        widget.clearForm();
//        widget.showToolbars(['btnSave', 'btnCancel']);
//    }

//    function btnEdit_click() {
//        widget.selectedRow("gridRole", function (data) {
//            if (!widget.isNullOrEmpty(data)) {
//                widget.disableElements(['ModuleId']);
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

//                    widget.post('util/module/delete', data, function (result) {
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
//            var url = 'util/module/save';

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
        title: "Management Module",
        xtype: "grid-form",
        urlList: "util/grid/modules",
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
            { name: "ModuleId", type: "text", maxlength: "36", text: "Module ID", cls: "span4 full", required: true },
            { name: "ModuleCaption", maxlength: 250, text: "Caption", required: true },
            { name: "ModuleIndex", type: "spinner", maxlength: 3, text: "Index", cls: "span4 full" },
            { name: "ModuleUrl", maxlength: 250, text: "Url", cls: "span4", type: "text", float: "left" },
            { name: "InternalLink", type: "switch", text: "Internal Link", cls: "span4 full", type: "switch", float: "left" },
            { name: "IsPublish", type: "switch", text: "Is Publish", cls: "span4 full", type: "switch", float: "left" },
        ],
        columns: [
            { mData: "ModuleId", sTitle: "Module ID", sWidth: "180px" },
            { mData: "ModuleCaption", sTitle: "Caption" },
            { mData: "ModuleIndex", sTitle: "Index", sWidth: "180px" },
            { mData: "ModuleUrl", sTitle: "Url", sWidth: "180px" },
            {
                mData: "InternalLink", sTitle: "Internal Link", sWidth: "130px",
                mRender: function (data, type, full) {
                    return (data)==true ? "Yes" : "No";
                }
            },
            {
                mData: "IsPublish", sTitle: "Is Published", sWidth: "130px",
                mRender: function (data, type, full) {
                    return (data)==true ? "Yes" : "No";
                }
            },
        ],
    }
    var widget = new SimDms.Widget(options);
    widget.default = {};
    widget.render(function () {
        $.post("util.api/role/default", function (result) {
            widget.default = result;
            widget.populate(result);
        });
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
            widget.post("util/module/save", data, function (result) {
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
            if (confirm("Do you want to delete selected data?")) {
                widget.post("util/module/delete", row, function (result) {
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
});