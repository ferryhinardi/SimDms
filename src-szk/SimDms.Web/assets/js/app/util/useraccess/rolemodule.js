//$(document).ready(function () {
//    var options = {
//        title: "Role Module",
//        xtype: "panels",
//        panels: [
//            {
//                name: "pnlRoleModule",
//                items: [
//                    {
//                        text: "Role",
//                        type: "controls",
//                        items: [
//                            { name: "RoleId", cls: "span4", placeHolder: "Role", type: "select" },
//                        ]
//                    },
//                    {
//                        type: "buttons",
//                        items: [
//                            { name: "btnFilter", text: "Search", icon: "fa fa-search" },
//                            { name: "btnAssign", text: "Assign", icon: "fa fa-file-excel-o" },
//                            { name: "btnUnAssign", text: "UnAssign", icon: "fa fa-unbolt" }
//                        ]
//                    }
//                ]
//            },
//            {
//                xtype: "k-grid",
//                name: "gridRoleModules"
//            },
//        ]
//    }

//    var widget = new SimDms.Widget(options);
//    widget.setSelect([
//        { name: "RoleId", url: "util/Role/List" },
//    ]);
//    widget.setEventList([
//        {
//            name: "btnFilter",
//            type: "button",
//            eventType: "click",
//            event: function (evt) {
//                var data = $("#pnlRoleModule").serializeObject();
//                widget.reloadGridData("tblDetailInfo");
//            }
//        },
//        {
//            name: "btnAssign",
//            type: "button",
//            eventType: "click",
//            event: function (evt) {
//                //AssignModules();
//            }
//        },
//        {
//            name: "btnUnAssign",
//            type: "button",
//            eventType: "click",
//            event: function (evt) {
//                UnassignModule();
//            }
//        }
//    ]);
//    widget.default = { DateFrom: new Date(), DateTo: new Date() };
//    widget.render(function () {
//        widget.populate(widget.default);
//        reloadGrid();
//        AssignModules();
//    });

//    widget.lookup.onDblClick(function (e, data, name) {
//        widget.showToolbars(["btnBrowse", "btnProcess"]);
//        if (1 == 1) {
//            var dataMain = widget.getForms();
//            dataMain.ModuleID = data.ModuleID;
//            widget.post("gn.api/RoleModule/Save", dataMain, function (result) {
//                if (result.status) {
//                    widget.reloadGridData("tblDetailInfo");
//                    widget.showNotification(result.message || SimDms.defaultInformationMessage);
//                }
//                else {
//                    widget.showNotification(result.message || SimDms.defaultErrorMessage);
//                }
//            });
//        }
//        widget.lookup.hide();
//    });

//    $("[name=RoleId]").on("change", function () {
//        reloadGrid();
//    });

//    function populateData(data) {
//        widget.post("gn.api/rolemenu/search", data, function (result) {
//            if (result.success) {
//                widget.populateTable({ selector: "#tblDetailInfo", data: result.list });
//                $('td:nth-child(6)').hide();
//            }
//            else {
//                confirm(result.message);
//            }
//        });
//    }

//    function reloadGrid() {
//        //var data = $("#pnlRoleModule").serializeObject();
//        //widget.kgrid({
//        //    url: "gn.api/Grid/RoleModules",
//        //    name: "gridRoleModules",
//        //    params: data,
//        //    columns: [
//        //        { field: "ModuleID", title: "Module ID", width: 100 },
//        //        { field: "ModuleCaption", title: "Caption", width: 150 },
//        //        { field: "ModuleIndex", title: "Index", width: 50 },
//        //        { field: "ModuleUrl", title: "Url", width: 150 },
//        //        { field: "IsPublishDescription", title: "Is Publish", width: 100 },
//        //        { field: "InternalLinkDescription", title: "Is Internal Link", width: 100 },
//        //    ],
//        //});

//        var params = {
//            RoleID: $("[name=RoleId]").val(),
//        }

//        widget.kgrid({
//            url: "util/Grid/RoleModules",
//            name: "gridRoleModule",
//            params: params,
//            columns: [
//                { field: "RoleName", title: "Role", width: 200 },
//                { field: "ModuleID", title: "Module ID", width: 200 },
//                { field: "ModuleCaption", title: "Caption", width: 200 },
//                { field: "ModuleIndex", title: "Index", width: 100 },
//                { field: "ModuleUrl", title: "Module URL", width: 250 },
//            ],
//        });
//    }

//    function AssignModules() {
//        var params = {};
//        widget.klookup({
//            name: "btnAssign",
//            title: "City List",
//            url: "gn.api/Grid/Modules",
//            serverBinding: true,
//            multiSelect: true,
//            filters: [
//                {
//                    text: "Filter",
//                    type: "controls",
//                    left: 80,
//                    items: [
//                        { name: "fltMenuId", text: "Kode Menu", cls: "span2" },
//                        { name: "fltMenuCaption", text: "Nama Menu", cls: "span4" },
//                    ]
//                }
//            ],
//            columns: [
//                { field: "ModuleID", title: "Module ID", width: 100 },
//                { field: "ModuleCaption", title: "Caption", width: 150 },
//                { field: "ModuleIndex", title: "Index", width: 50 },
//                { field: "ModuleUrl", title: "Url", width: 150 },
//                { field: "IsPublishDescription", title: "Is Publish", width: 100 },
//                { field: "InternalLinkDescription", title: "Is Internal Link", width: 100 },
//            ],
//            dynamicParams: [
//                { name: "RoleID", element: "RoleId" }
//            ],
//            filters: [
//                {
//                    text: "Filter",
//                    type: "controls",
//                    left: 80,
//                    items: [
//                        { name: "filterModuleID", text: "Kode Menu", cls: "span2" },
//                        { name: "filterModuleCaption", text: "Nama Menu", cls: "span4" },
//                    ]
//                }
//            ],
//            params: params,
//            onSelected: function (doc) { },
//            onSelectedRows: function (data) {
//                var dataMain = widget.getForms();
//                var moduleIDs = new Array();

//                $.each(data || [], function (key, val) {
//                    moduleIDs.push(val.ModuleID);
//                });
//                dataMain.ModuleID = moduleIDs;

//                widget.post("gn.api/RoleModule/Save", dataMain, function (result) {
//                    if (result.status) {
//                        reloadGrid();
//                        widget.showNotification(result.message || SimDms.defaultInformationMessage);
//                    }
//                    else {
//                        widget.showNotification(result.message || SimDms.defaultErrorMessage);
//                    }
//                });
//            }
//        });
//    }

//    function UnassignModule() {
//        var dataMain = widget.getForms();
//        widget.selectedRows("gridRoleModules", function (data) {
//            if (widget.isNullOrEmpty(data) == false) {
//                var moduleIDs = new Array();
//                var formData = new FormData();

//                $.each(data, function (key, val) {
//                    moduleIDs.push(val.ModuleID);
//                });
//                formData.append("ModuleID", moduleIDs);
//                formData.append("RoleID", $("[name='RoleId']").val());

//                var params = {
//                    RoleID: $("[name='RoleId']").val(),
//                    ModuleID: moduleIDs
//                };

//                widget.post("gn.api/RoleModule/UnassignModules", params, function (result) {
//                    if (result.status) {
//                        reloadGrid();
//                    }
//                    widget.showNotification(result.message);
//                });
//            }
//            else {
//                widget.showNotification("You have to select modules to be unassigned.");
//            }
//        });
//    }
//});



$(document).ready(function () {
    var options = {
        title: "Management User",
        xtype: "panels",
        panels: [
            {
                name: "panelInput",
                items: [
                    { type: 'select', name: "RoleId", text: "Role", cls: "span4 full", required: true },
                    {
                        type: "buttons",
                        items: [
                            { name: "btnFilter", text: "Filter", icon: "fa fa-search" },
                            { name: "btnAssign", text: "Assign", icon: "fa fa-file-excel-o" },
                            { name: "btnUnAssign", text: "UnAssign", icon: "fa fa-unbolt" }
                        ]
                    },
                ]
            },
            { name: "gridRoleModule", xtype: "k-grid", cls: 'hide' },
        ],
    }
    var widget = new SimDms.Widget(options);
    widget.render(renderCallback);

    function renderCallback() {
        initElementStates();
        initElementEvents();

        reloadLookupModule();
    }

    function initElementStates() {
        widget.setSelect([
            { name: "RoleId", url: "util/Role/List" },
        ]);
    }

    function initElementEvents() {
        var btnFilter = $('#btnFilter');
        var btnAssign = $('#btnAssign');
        var btnUnAssign = $('#btnUnAssign');
        var cmbRoleId = $('#RoleId');

        btnFilter.off();
        btnFilter.on('click', function (evt) {
            evt.preventDefault();
            btnFilter_click();
        });

        btnAssign.off();
        btnAssign.on('click', function (evt) {
            evt.preventDefault();
            btnAssign_click();
        });

        btnUnAssign.off();
        btnUnAssign.on('click', function (evt) {
            evt.preventDefault();
            btnUnAssign_click();
        });

        cmbRoleId.off();
        cmbRoleId.on('change', function () {
            cmbRoleId_click();
        });
    }

    function cmbRoleId_click() {
        reloadGridModule();
    }

    function btnAssign_click() {
    }

    function btnUnAssign_click() {
        console.log('unassign');
    }

    function btnFilter_click() {
        console.log('filter');
    }

    function reloadGridModule() {
        var params = {
            RoleID: $("[name=RoleId]").val(),
        }

        widget.kgrid({
            url: "util/Grid/RoleModules",
            name: "gridRoleModule",
            params: params,
            columns: [
                { field: "RoleName", title: "Role", width: 200 },
                { field: "ModuleID", title: "Module ID", width: 200 },
                { field: "ModuleCaption", title: "Caption", width: 200 },
                { field: "ModuleIndex", title: "Index", width: 100 },
                { field: "ModuleUrl", title: "Module URL", width: 250 },
            ],
        });
    }

    function reloadLookupModule() {
        var params = {
            RoleID: $('[name="RoleId"]').val()
        };
        widget.klookup({
            name: "btnAssign",
            title: "Module List",
            url: "util/grid/Modules",
            serverBinding: true,
            multiSelect: true,
            filters: [
                {
                    text: "Filter",
                    type: "controls",
                    left: 80,
                    items: [
                        //{ name: "fltMenuId", text: "Kode Menu", cls: "span2" },
                    ]
                }
            ],
            columns: [
                { field: "ModuleId", title: "Module ID", width: 150 },
                { field: "ModuleCaption", title: "Caption", width: 150 },
                { field: "ModuleIndex", title: "Index", width: 150 },
                { field: "ModuleUrl", title: "Url", width: 150 },
            ],
            dynamicParams: [
                { name: "RoleID", element: "RoleId" }
            ],
            //params: params,
            onSelected: function (doc) { },
            onSelectedRows: function (data) {
                console.log(data);
                //var dataMain = widget.getForms();
                //var moduleIDs = new Array();

                //$.each(data || [], function (key, val) {
                //    moduleIDs.push(val.ModuleID);
                //});
                //dataMain.ModuleID = moduleIDs;

                //widget.post("gn.api/RoleModule/Save", dataMain, function (result) {
                //    if (result.status) {
                //        reloadGrid();
                //        widget.showNotification(result.message || SimDms.defaultInformationMessage);
                //    }
                //    else {
                //        widget.showNotification(result.message || SimDms.defaultErrorMessage);
                //    }
                //});
            }
        });
    }
});


$(document).ready(function () {
    var options = {
        title: "Role Module",
        xtype: "panels",
        panels: [
            {
                name: "pnlRoleModule",
                title: "Role Search",
                items: [
                    {
                        text: "Role",
                        type: "controls",
                        items: [
                            { name: "RoleId", cls: "span4", placeHolder: "Kode Role", type: "select" },
                        ]
                    },
                    {
                        type: "buttons",
                        items: [
                            { name: "btnFilter", text: "Search", icon: "fa fa-search" },
                            { name: "btnAssign", text: "Assign", icon: "fa fa-file-excel-o" },
                            { name: "btnUnAssign", text: "UnAssign", icon: "fa fa-unbolt" }
                        ]
                    }
                ]
            },
            {
                title: "Detail Information",
                xtype: "grid",
                name: "tblDetailInfo",
                tblname: "tblDetailInfo",
                source: "util/Grid/RoleModules",
                selectable: true,
                multiselect: false,
                columns: [
                    { mData: "RoleName", sTitle: "Role Name", sWidth: "150px" },
                    { mData: "ModuleID", sTitle: "Module ID", sWidth: "150px" },
                    { mData: "ModuleCaption", sTitle: "Caption" },
                    { mData: "ModuleIndex", sTitle: "Index", sWidth: "160px" },
                    { mData: "ModuleUrl", sTitle: "URL", sWidth: "160px" },
                ],
                selectable: true,
                multiselect: false,
                additionalParams: [
                    { name: "RoleId", element: "RoleId", type: "select" },
                ]
            }
        ]
    }

    var widget = new SimDms.Widget(options);
    widget.setSelect([
        { name: "RoleId", url: "util/Role/List" },
    ]);
    widget.setEventList([
        {
            name: "btnFilter",
            type: "button",
            eventType: "click",
            event: function (evt) {
                var data = $("#pnlRoleModule").serializeObject();
                widget.reloadGridData("tblDetailInfo");
            }
        },
        {
            name: "btnAssign",
            type: "button",
            eventType: "click",
            event: function (evt) {
                if (widget.isNullOrEmpty($("[name='RoleId']").val()) == false) {
                    widget.lookup.init({
                        name: "GridAssignMenu",
                        source: "util/grid/Modules",
                        columns: [
                            { mData: "ModuleId", sTitle: "Module ID", sWidth: "150px" },
                            { mData: "ModuleCaption", sTitle: "Caption" },
                            { mData: "ModuleIndex", sTitle: "Index", sWidth: "160px" },
                            { mData: "ModuleUrl", sTitle: "URL", sWidth: "160px" },
                        ],
                        additionalParams: [
                            { name: "RoleId", element: "RoleId", type: "select" }
                        ]
                    });
                    widget.lookup.show();
                }
            }
        },
        {
            name: "btnUnAssign",
            type: "button",
            eventType: "click",
            event: function (evt) {
                var dataMain = widget.getForms();
                var row = $('#tblDetailInfo tr.row_selected td:nth-child(2)').text();
                dataMain.ModuleID = row;
                if (dataMain !== undefined) {
                    if (confirm("Anda yakin akan menghapus data ini?")) {
                        widget.post("util/RoleModule/Delete", dataMain, function (result) {
                            if (result.status) {
                                widget.reloadGridData("tblDetailInfo");
                                widget.showNotification(result.message || SimDms.defaultInformationMessage);
                            }
                            else {
                                widget.showNotification(result.message || SimDms.defaultErrorMessage);
                            }
                        });
                    };
                }
            }
        }
    ]);
    widget.default = { DateFrom: new Date(), DateTo: new Date() };
    widget.render(function () {
        widget.populate(widget.default);
    });

    widget.lookup.onDblClick(function (e, data, name) {
        widget.showToolbars(["btnBrowse", "btnProcess"]);
        if (1 == 1) {
            var dataMain = widget.getForms();
            dataMain.ModuleID = data.ModuleId;
            widget.post("util/RoleModule/Save", dataMain, function (result) {
                if (result.status) {
                    widget.reloadGridData("tblDetailInfo");
                    widget.showNotification(result.message || SimDms.defaultInformationMessage);
                }
                else {
                    widget.showNotification(result.message || SimDms.defaultErrorMessage);
                }
            });
        }
        widget.lookup.hide();
    });

    $("[name=RoleId]").on("change", function () {
        var data = $("#pnlRoleModule").serializeObject();
        widget.reloadGridData("tblDetailInfo");
    });

    function populateData(data) {
        widget.post("gn.api/rolemenu/search", data, function (result) {
            if (result.success) {
                widget.populateTable({ selector: "#tblDetailInfo", data: result.list });
                $('td:nth-child(6)').hide();
            }
            else {
                confirm(result.message);
            }
        });
    }

});