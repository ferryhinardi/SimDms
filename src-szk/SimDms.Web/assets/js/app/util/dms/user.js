
var widget;

$(document).ready(function () {
    var widgetOptions = {
        name: "panelsUserRole",
        title: "User Role",
        xtype: "panels",
        panels: [
            {
                name: "panelUser",
                cls: "hide",
                items: [
                    { name: "Username", type: "popup", text: "Username", cls: "span3 full", readonly: true, placeholder: "Username" },
                    { name: "Email", type: "text", text: "Email", cls: "span5 full", readonly: true, placeholder: "Email" },
                    { name: "First Name", type: "text", text: "First Name", cls: "span5 full", readonly: true, placeholder: "First Name" },
                    { name: "Last Name", type: "text", text: "Last Name", cls: "span5 full", readonly: true, placeholder: "Last Name" },
                    {
                        type: "controls",
                        text: "IsApproved",
                        items: [
                            { name: "IsApproved", type: "switch", text: "Is Approved", cls: "span2 full hide", float: "left", readonly: true, placeholder: "Is Approved" },
                            { name: "IsApprovedDescription", type: "text", readonly: true, cls: "span1 full"}
                        ]
                    }
                ]
            },
            {
                name: "KGrid",
                xtype: "k-grid",
            },
        ],
        toolbars: [
            { name: "btnCreate", text: " Create", cls: "small", icon: "fa fa-new" },
            { name: "btnEdit", text: " Edit&nbsp; ", cls: "small", icon: "fa fa-edit" },
            { name: "btnDelete", text: " Delete ", cls: "small", icon: "fa fa-trash" },
            { name: "btnSave", text: " Save ", cls: "small", icon: "fa fa-file" },
            { name: "btnCancel", text: " Cancel ", cls: "small", icon: "fa fa-refresh" },
        ]
    };

    widget = new SimDms.Widget(widgetOptions);
    widget.render(renderCallback);
});

function renderCallback() {
    widget.showToolbars(["btnCreate", "btnEdit", "btnDelete"]);

    initializeElementEvents();
    reloadData();
}

function initializeElementEvents() {
    evt_Username();
    evt_btnUsername();
    evt_btnCreate();
    evt_btnEdit();
    evt_btnDelete();
    evt_btnCancel();
    evt_btnSave();
}

function evt_Username() {
    var textUsername = $("[name='Username']");
    textUsername.on("input", function (evt) {
        var value = $(this).val();
        var buttons = ["btnCreate", "btnEdit", "btnDelete"];

        if (widget.isNullOrEmpty(value) == false) {
            buttons.push("btnClear");
        }

        widget.showToolbars(buttons);
    });
}

function evt_btnUsername() {
    var btnUsername = $("#btnUsername");

    btnUsername.on("click", function (evts) {
    });
}

function evt_btnCreate() {
    var evt_btnCreate = $("#btnCreate");

    evt_btnCreate.on("click", function (evts) {
        widget.showToolbars(["btnSave", "btnCancel"]);
    });
}

function evt_btnEdit() {
    var btnEdit = $("#btnEdit");

    btnEdit.on("click", function (evts) {
        widget.showToolbars(["btnSave", "btnCancel"]);
    });
}

function evt_btnDelete() {
    var btnDelete = $("#btnDelete");

    btnDelete.on("click", function (evts) {
    });
}


function evt_btnSave() {
    var btnSave = $("#btnSave");

    btnSave.on("click", function (evts) {
    });
}

function evt_btnCancel() {
    var btnCancel = $("#btnCancel");

    btnCancel.on("click", function (evts) {
        widget.showToolbars(["btnCreate", "btnEdit", "btnDelete"]);
    });
}

function reloadData() {
    var params = {
        DataType: "EMPLY",
        DataStatus: "1"
    };
    widget.kgrid({
        url: "util.api/inquiry/datalist",
        name: "KGrid",
        params: params,
        sort: { field: "CreatedDate", dir: "desc" },
        columns: [
            { field: "Company", width: 100, title: "Company" },
            { field: "Username", width: 120, title: "Username" },
            { field: "Email", width: 100, title: "Email" },
            { field: "FirstName", width: 180, title: "First Name"},
            { field: "LastName", width: 100, title: "Last Name" },
            { field: "Comment", width: 180, title: "Comment" },
            { field: "IsApproved", width: 180, title: "Is Approved" },
            { field: "isLockedOut", width: 100, title: "Is Locked" },
            { field: "UserGroup", width: 100, title: "Group" },
        ]
    });
}