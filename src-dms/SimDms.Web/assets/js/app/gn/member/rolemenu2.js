$(document).ready(function () {
    var options = {
        title: "RoleMenu",
        xtype: "panels",
        panels: [
            {
                name: "pnlRoleMenus",
                items: [
                    { name: "ModuleId", cls: "span4 full", placeHolder: "Module", type: "select", text: "Module" },
                    { name: "SubModuleId", cls: "span4 full", placeHolder: "Sub Module", type: "select", text: "Menu Lvl 1"},
                    { name: "SubSubModuleId", cls: "span4 full", placeHolder: "Sub Module", type: "select", text: "Menu Lvl 2" },
                    {
                        text: "Role",
                        type: "controls",
                        cls: "span4 full",
                        items: [
                            { name: "RoleId", cls: "span8", placeHolder: "Kode Role", type: "select" },
                        ]
                    },
                    
                    {
                        type: "buttons",
                        items: [
                        { name: "btnFilter", text: "Search", icon: "icon-search" },
                        { name: "btnAssign", text: "Assign", icon: "icon-bolt" },
                        { name: "btnUnAssign", text: "UnAssign", icon: "icon-unbolt" }
                        ]
                    }
                ]
            },
            //{
            //    xtype: "k-grid",
            //    name: "gridRoleMenus"
            //},
            {
                xtype: "wxtable",
                name: "txPermission"
            }
        ]
    }

    var widget = new SimDms.Widget(options);
    widget.setSelect([
        { name: "RoleId", url: "gn.api/combo/roles" },
        { name: "ModuleId", url: "gn.api/combo/Modules" },
        {
            name: "SubModuleId",
            url: "gn.api/Combo/SubModule",
            optionalText: "--ALL Sub Module --",
            cascade: {
                name: "ModuleId"
            }
        },
        {
            name: "SubSubModuleId",
            url: "gn.api/Combo/SubSubModule",
            optionalText: "--ALL Sub Module --",
            cascade: {
                name: "SubModuleId"
            }
        },
    ]);
    widget.setEventList([
        {
            name: "btnFilter",
            type: "button",
            eventType: "click",
            event: function (evt) {
                var data = $("#pnlRole").serializeObject();
                widget.reloadGridData("tblDetailInfo");
            }
        },
        {
            name: "btnUnAssign",
            type: "button",
            eventType: "click",
            event: function (evt) {
                unassignMenu();
            }
        }
    ]);
    widget.default = { DateFrom: new Date(), DateTo: new Date() };
    widget.render(function () {
        widget.populate(widget.default);
        reloadGrid();
        $("[name=SubModuleId]").attr("disabled", true);
        $("[name=SubSubModuleId]").attr("disabled", true);
    });
    $("[name=ModuleId]").on("change", function () {
        $("[name=SubModuleId]").removeAttr("disabled");
        $("[name=SubSubModuleId]").val(null);
        $("[name=SubSubModuleId]").attr("disabled", true);
        reloadGrid();
    });
    $("[name=SubModuleId]").on("change", function () {
        $("[name=SubSubModuleId]").removeAttr("disabled");
        reloadGrid();
    });
    $("[name=SubSubModuleId]").on("change", function () {
        reloadGrid();
    });
    $("[name=RoleId]").on("change", function () {
        reloadGrid();
        reloadtreemenus();
    });

    $("#btnAssign").on("click", function () {
        if ($("[name=SubModuleId]").val() == "") 
            //widget.showNotification("Silahkan pilih role/group!");
            MsgBox("Silahkan pilih Role terlebih dahulu!");
        else assignMenu();
    });

    function populateData(data) {
        widget.post("gn.api/rolemenu/search", data, function (result) {
            if (result.success) {
                widget.populateTable({ selector: "#tblDetailInfo", data: result.list });
                $('td:nth-child(6)').hide();
            }
            else {
                //confirm(result.message);
                widget.showNotification(result.message || "");
            }
        });
    }

    function reloadGrid() {
        var data = $("#pnlRoleMenus").serializeObject();
        widget.kgrid({
            url: "gn.api/Grid/RoleMenus",
            name: "gridRoleMenus",
            params: data,
            columns: [
                //{ field: "MenuId", title: "Menu ID", width: 100 },
                { field: "MenuCaption", title: "Menu yang dapat di Akses", width: 150 },
                //{ field: "MenuHeader", title: "Header", width: 200 },
                //{ field: "MenuLevel", title: "Level", width: 70 },
                //{ field: "MenuIndex", title: "Index", width: 70 },
                //{ field: "MenuUrl", title: "URL", width: 200 },
            ],
        });
    }

    function assignMenu(data) {
        $("#btnAssign").off();

        var params = $("#pnlRoleModule").serializeObject();
        widget.klookup({
            name: "btnAssign",
            title: "Menu List",
            url: "gn.api/grid/Menus",
            serverBinding: true,
            multiSelect: true,
            filters: [
                {
                    text: "Filter",
                    type: "controls",
                    left: 80,
                    items: [
                        //{ name: "filterMenuID", text: "Kode Menu", cls: "span2" },
                        { name: "filterMenuCaption", text: "Nama Menu", cls: "span4" },
                    ]
                }
            ],
            columns: [
                //{ field: "MenuId", title: "Kode Menu", width: 180 },
                { field: "MenuCaption", title: "Nama Menu" },
                //{ field: "MenuHeader", title: "Header Menu", width: 120 },
                //{ field: "MenuLevel", title: "Level Menu", width: 120 },
                //{ field: "MenuIndex", title: "Index Menu", width: 120 }
            ],
            dynamicParams: [
                { name: "RoleId", element: "RoleId" },
                { name: "ModuleId", element: "ModuleId" },
                { name: "SubModuleId", element: "SubModuleId" },
                { name: "SubSubModuleId", element: "SubSubModuleId" },
            ],
            onSelected: function (doc) { },
            onSelectedRows: function (data) {
                assigningMenu(data);
            }
        });

        $("#btnAssign").click();
    }

    function assigningMenu(data) {
        var url = "gn.api/RoleMenu/Save";
        var menuIDs = new Array();
        var params = {};
        
        $.each(data || [], function (key, val) {
            menuIDs.push(val.MenuId);
        });
        params.RoleID = $("[name=RoleId]").val();
        params.MenuID = menuIDs;

        widget.post(url, params, function (result) {
            if (result.status) {
                reloadGrid();
            }

            widget.showNotification(result.message);
        });

    }

    function unassignMenu() {
        var dataMain = widget.getForms();
        widget.selectedRows("gridRoleMenus", function (data) {
            if (widget.isNullOrEmpty(data) == false) {
                var menuIDs = new Array();

                $.each(data, function (key, val) {
                    menuIDs.push(val.MenuId);
                });

                var params = {
                    RoleID: $("[name='RoleId']").val(),
                    MenuID: menuIDs
                };

                widget.post("gn.api/RoleMenu/UnassignMenus", params, function (result) {
                    if (result.status) {
                        reloadGrid();
                    }
                    widget.showNotification(result.message);
                });
            }
            else {
                widget.showNotification("You have to select modules to be unassigned.");
            }
        });
    }

    function reloadtreemenus()
    {
        //var service = "gn.api/grid/RolePermission?RoleId" + $("[name=RoleId]").val();

        //$("#txPermission").kendoTreeList({
        //    dataSource: {
        //        transport: {
        //            read: {
        //                url: service,
        //                dataType: "jsonp"
        //            }
        //        },
        //        schema: {
        //            model: {
        //                id: "EmployeeID",
        //                fields: {
        //                    parentId: { field: "MenuHeader", nullable: true },
        //                    EmployeeID: { field: "EmployeeId", type: "number" },
        //                    Extension: { field: "Extension", type: "number" }
        //                },
        //                expanded: true
        //            }
        //        }
        //    },
        //    height: 540,
        //    filterable: true,
        //    sortable: true,
        //    columns: [
        //        {
        //            field: "FirstName", title: "First Name", width: 280,
        //            template: $("#photo-template").html()
        //        },
        //        { field: "LastName", title: "Last Name", width: 160 },
        //        { field: "Position" },
        //        { field: "Phone", width: 200 },
        //        { field: "Extension", width: 140 },
        //        { field: "Address" }
        //    ]
        //});

        //var data = { RoleId: $("[name=RoleId]").val() }
        //widget.post("gn.api/grid/RolePermission", data, function (result) {

            var dataSource = new kendo.data.TreeListDataSource({
                transport: {
                    read: {
                        url: "gn.api/grid/RolePermission",
                        dataType: "jsonp"
                    }
                },
                batch: true,
                schema: {
                    model: {
                        id: "MenuId",
                        fields: {
                            parentId: { field: "MenuHeader", nullable: true, type: "string" },
                            MenuId: { field: "MenuId", type: "string" },
                            MenuCaption: { field: "MenuCaption", type: "string" },
                            Navigation: { field: "Navigation", type: "boolean" },
                            AllowCreate: { field: "AllowCreate", type: "boolean" },
                            AllowEdit: { field: "AllowEdit", type: "boolean" },
                            AllowDelete: { field: "AllowDelete", type: "boolean" },
                            AllowPrint: { field: "AllowPrint", type: "boolean" }
                        },
                        expanded: false
                    }
                }
            });

            function onChange(arg) {
                var selected = $.map(this.select(), function (item) {
                    return $(item).text();
                });

                console.log("Selected: " + selected.length + " item(s), [" + selected.join(", ") + "]");
            }

            $("#txPermission").kendoTreeList({
                dataSource: dataSource,
                height: 640,
                resizable: true,
                columnMenu: true,
                filterable: true,
                selectable: true,
                //sortable: true,
                change: onChange,
                editable: "inline",
                columns: [
                    {
                        field: "MenuCaption",
                        locked: true,
                        lockable: false,
                        title: "Menu Caption",
                        width: 350
                    },
                    {
                        field: "Navigation", width: 100, title: 'View',
                        template: "<input type='checkbox' #= (Navigation === true) ? checked='checked' : '' #  />"
                    },
                    {
                        field: "AllowCreate", width: 100, title: 'Create',
                        template: "<input type='checkbox' #= (AllowCreate === true) ? checked='checked' : '' #  />"
                    },
                    {
                        field: "AllowEdit", width: 100, title: 'Edit',
                        template: "<input type='checkbox' #= (AllowEdit === true) ? checked='checked' : '' #  />"
                    },
                    {
                        field: "AllowDelete", width: 100, title: 'Delete',
                        template: "<input type='checkbox' #= (AllowDelete === true) ? checked='checked' : '' #  />"
                    },
                    {
                        field: "AllowPrint", width: 100, title: 'Print',
                        template: "<input type='checkbox' #= (AllowPrint === true) ? checked='checked' : '' #  />"
                    },
                    //{
                    //    title: "Edit", command: ["edit"], width: 150,
                    //    attributes: {
                    //        style: "text-align: center;"
                    //    }
                    //}

                ]
            });

        //});
    }

});