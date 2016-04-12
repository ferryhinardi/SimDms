$('head').append("<link href='assets/css/vendor/common.css' rel='stylesheet'>");
var isChanged = false;
var treeItem = "";
var groupCodes = [];
var typeCodes = [];
$(document).ready(function () {
    var options = {
        title: 'Master Model Mapping',
        xtype: 'panels',
        toolbars: [
            { name: "btnNew", text: " New...", cls: "btn btn-primary", icon: "fa fa-file-o" },
            { name: "btnSave", text: " Save", cls: "btn btn-success", icon: "fa fa-save" },
            { name: "btnReload", text: " Reload", cls: "btn btn-warning", icon: "fa fa-refresh" },
            { name: "btnHelp", text: "Help", cls: "btn btn-danger", icon: "fa fa-book" }
        ],
        panels: [
            { xtype: "panelStart" },
            {
                name: "wxTree",
                title: "Model",
                width: "370px",
                xtype: "wxTbl",
                cls: "span3"
            },
            {
                name: "wxEdit",
                xtype: "wxdiv",
                cls: "",
                width: 90
            },
            {
                name: "wxSmc",
                title: "Sales Model Code",
                width: "250px",
                xtype: "wxTbl",
                cls: "span1"
            },
            { xtype: "panelEnd" }
        ]
    }

    var widget = new SimDms.Widget(options);
    widget.render(init);

    function init() {
        loadTree();
        loadSmc();
    }
       
    var toolEdit = webix.ui({
        container: "wxEdit",
        view: "list",
        id: "toolEdit",
        template: "#value#",
        scroll: false,
        drag: "target",
        layout: "x",
        width: 80,
        autoheight: false,
        height: 471,
        type: {
            width: 80,
            height: 471
        },
        data: [{ value: "[Edit]" }],
        on: {
            onBeforeAdd: _onBeforeAdd,
            onBeforeDragIn:function (context, event) {
                var target = toolEdit.getItemNode(context.target);
                ChangeCursor("toolEdit", context, target);
            },
            onMouseMoving: _Edit_onMouseMoving,
            onBeforeDrop: function (context, event) {
                if (context.from.config.id == "table1") return false;
                else if (context.from.config.id == "tree1") {
                    editItem(tree1.getItem(context.start));
                }
                return false;
            }
        }
    });

    function _onBeforeAdd(context, event) { return false; }

    function ChangeCursor(sender, context, target) {
        if (sender == "toolEdit") {
            if (context.from.config.id == "table1") {
                $(target).css("cursor", "no-drop");
            } else if (context.from.config.id == "tree1") {
                var item = tree1.getItem(context.start);
                if (item.lvl == 3) {
                    $(target).css("cursor", "no-drop");
                    return;
                }
                $(target).css("cursor", "-webkit-grabbing");
                $(target).css("cursor", "-moz-grabbing");
            }
        }
        else if (sender == "table1") {
            if (context.from.config.id == "tree1") {
                var item = tree1.getItem(context.start);
                if (item.lvl == 3) {
                    $(target).css("cursor", "-webkit-grabbing");
                    $(target).css("cursor", "-moz-grabbing");
                    return;
                }
                $(target).css("cursor", "no-drop");
            }
        }       
    }

    function _Edit_onMouseMoving (e) {
        if (e.type == "mouseout") {
            var target = e["target"];
            $(target).css("cursor", "-webkit-grab");
        }
    }

    function _table1_onMouseMoving(e) {
        if (e.type == "mouseout") {
            var target = e["target"];
            $(target).css("cursor", "default");
        }
    }

    var tree1 = webix.ui({
        container: "wxTree",
        view: "treetable",
        id: "tree1",
        autowidth: false,
        width: 370,
        height: 441,
        css: "alternating",
        drag: true,
        dragscroll: false,
        resizecolumn: false,
        header: false,
        scrollX: false,
        threeState: true,
        columns: [
            {
                id: "Item", width: 280, css: { "font-family": "Segoe UI" },
                template: "{common.space()}{common.icon()}{common.treecheckbox()}{common.folder()} #Item#"
            },
            { id: "id", width: 70, css: { "font-family": "Segoe UI" } }
        ],
        on: {
            onBeforeDrop: _tree1BeforeDrop
        }
    });

    var table1 = webix.ui({
        container: "wxSmc",
        view: "datatable",
        id: "table1",
        css: "alternating",
        header: false,
        autowidth: false,
        width: 250,
        height: 441,
        dragScroll: true,
        drag: true,
        scrollX: false,
        resizecolumn: false,
        select: "row",
        multiselect: true,
        columns: [
            { id: "SalesModelCode", header: "Sales Model Code", css: { "font-family": "Segoe UI" }, width: 230 }
        ],
        on: {
            onBeforeDrop: _table1BeforeDrop,
            onBeforeDragIn: function (context, event) {
                var target = table1.getItemNode(context.target);
                ChangeCursor("table1", context, target);
            },
            onMouseMoving: _table1_onMouseMoving,
        }
    });

    function _tree1BeforeDrop(context, event) {
        if (context.from.config.id == "table1") {
            var target = tree1.getItem(context.target.row);
            if (target.lvl != 2) return false;
            for (var i = 0; i < context.source.length; i++) {
                var source = table1.getItem(context.source[i]);
                assignSmc(source, target);
            }
        }
        return false;
    }

    function assignSmc(source, target) {
        $.ajax({
            async: false,
            type: "POST",
            data: {
                groupCode: target.GroupCode,
                typeCode: target.TypeCode,
                trans: target.TransmissionType,
                salesModelCode: source.SalesModelCode
            },
            url: "wh.api/ModelMapping/AssignSmc",
            success: function (data) {
                if (data.message == "") {
                    var parentid = target.id;
                    tree1.add({
                        GroupCode: target.GroupCode,
                        GroupCodeSeq: target.GroupCodeSeq,
                        TypeCode: target.TypeCode,
                        TypeCodeSeq: target.TypeCodeSeq,
                        IsSelected: target.IsSelected,
                        Item: source.SalesModelCode,
                        SalesModelCode: source.SalesModelCode,
                        SalesModelSeq: 0,
                        TransmissionType: target.TransmissionType,
                        TransmissionSeq: target.TransmissionSeq,
                        lvl: 3,
                        id: target.id + "." + data.count
                    }, data.count, parentid);
                    table1.remove(source.id);
                    tree1.open(parentid);
                    return false;
                } else {
                    MsgBox(data.message);
                    return false;
                }
            }
        });
    }

    function _table1BeforeDrop(context, event){
        if (context.from.config.id == "tree1") {
            var source = tree1.getItem(context.start);
            if (source.lvl != 3) return false;
            MsgConfirm("Do you really want to remove this mapping?", function (ok) {
                if (!ok) return false;
                $.ajax({
                    async: false,
                    type: "POST",
                    data: {
                        groupCode: source.GroupCode,
                        typeCode: source.TypeCode,
                        trans: source.TransmissionType,
                        salesModelCode: source.SalesModelCode
                    },
                    url: "wh.api/ModelMapping/WithdrawSmc",
                    success: function (data) {
                        if (data.message == "") {
                            tree1.remove(source.id);
                            table1.add({
                                SalesModelCode: source.SalesModelCode
                            });
                            table1.sort("SalesModelCode");
                            return false;
                        } else {
                            MsgBox(data.message);
                            return false;
                        }
                    }
                });
            });
        }
        return false;
    }

    var window1 = webix.ui({
        view: "window",
        id: "windowNewItem",
        move: false,
        modal: true,
        position: "center",
        hidden: true,
        width: 460,
        head: {
            view: "toolbar",
            cols: [
                { view: "label", label: "<h3 id='windowTitle' style='font-family:Segoe UI'>" + "Create New Model" + "</h3>", width: 270 },
                { view: "button", id: "winCreate", value: "Create", width: 80, align: 'right' },
                { view: "button", id: "winCancel", value: "Cancel", width: 80, align: 'right', click: "$$('windowNewItem').hide();" }
            ]
        },
        body: {
            rows: [
                { view: "label", label: "" },
                {
                    id: "winGroupCode",
                    cols: [
                        { view: "label", label: "", width: 40 },
                        { id: "cmbGroupCode", view: "text", label: "Type", value: "", labelWidth: 110, width: 380, suggest: [], on: { onChange: _onCmbGroupCodeChanged } },
                        { view: "label", label: "", width: 40 },
                    ]
                },
                {
                    id: "winGroupCodeSeq",
                    cols: [
                        { view: "label", label: "", width: 40 },
                        { id: "txtGroupCodeSeq", view: "text", label: "Seq", labelWidth: 110, value: "0", width: 190, attributes: { type: "number" } },
                        { view: "label", label: "", width: 230 },
                    ]
                },
                {
                    id: "winTypeCode",
                    cols: [
                        { view: "label", label: "", width: 40 },
                        { id: "cmbTypeCode", view: "text", label: "Variant", value: "", labelWidth: 110, width: 380, suggest: [] },
                        { view: "label", label: "", width: 40 },
                    ]
                },
                {
                    id: "winTypeCodeSeq",
                    cols: [
                        { view: "label", label: "", width: 40 },
                        { id: "txtTypeCodeSeq", view: "text", label: "Seq", labelWidth: 110, value: "0", width: 190, attributes: { type: "number" } },
                        { view: "label", label: "", width: 230 },
                    ]
                },
                {
                    id: "winTrans",
                    cols: [
                        { view: "label", label: "", width: 40 },
                        {
                            id: "cmbTrans", view: "select", label: "Transmission", value: "MT", labelWidth: 110, width: 190,
                            options: [
                                { id: "MT", value: "MT" },
                                { id: "AT", value: "AT" },
                            ]
                        },
                        { view: "label", label: "", width: 230 },
                    ]
                },
                { view: "label", label: "" }
            ]
        }
    });

    function loadGroupCodes() {
        $.ajax({
            async: false,
            type: "POST",
            data: {
            },
            url: "wh.api/ModelMapping/LoadGroupCodes",
            success: function (data) {
                if (data.message == "") {
                    var suggest = $$('cmbGroupCode').data.suggest;
                    var list = $$(suggest).getList();
                    list.clearAll();
                    list.parse(data.items);
                }
            }
        });
    }

    function loadTypeCodes(groupCode) {
        $.ajax({
            async: false,
            type: "POST",
            data: {
                groupCode: groupCode
            },
            url: "wh.api/ModelMapping/LoadTypeCodes",
            success: function (data) {
                if (data.message == "") {
                    var suggest = $$('cmbTypeCode').data.suggest;
                    var list = $$(suggest).getList();
                    list.clearAll();
                    list.parse(data.items);
                }
            }
        });
    }

    function loadTree() {
        $.ajax({
            async: true,
            type: "POST",
            data: {
            },
            url: "wh.api/ModelMapping/LoadTree",
            success: function (data) {
                if (data.message != "") {
                    tree1.clearAll();
                    MsgBox(data.message);
                    return;
                }
                if (data.result.length == 0) {
                    tree1.clearAll();
                    return;
                }
                var str = JSON.stringify(data.result);
                tree1.clearAll();
                tree1.parse(str, "json");
                tree1.closeAll();
                $.each(data.result, function (index, value) {
                    if(value.IsSelected) tree1.checkItem(value.id);
                });

                tree1.attachEvent("onItemCheck", function (id) {
                    isChanged = true;
                    $('#btnSave').removeAttr("disabled");
                })

                $('#btnSave').attr("disabled", "disabled");
            }
        });
    }

    function loadSmc() {
        $.ajax({
            async: true,
            type: "POST",
            data: {
            },
            url: "wh.api/ModelMapping/LoadSmc",
            success: function (data) {
                if (data.result.length == 0) {
                    table1.clearAll();
                    return;
                }
                var str = JSON.stringify(data.result);
                table1.clearAll();
                table1.parse(str, "json");
            }
        });
    }

    function save() {
        if (!isChanged) return;
        else MsgConfirm("Save changes?", function (ok) {
            if (ok) {
                var objects = [];
                var checklist = tree1.getChecked();
                $.each(checklist, function (index, value) {
                    var count = value.split('.').length - 1;
                    if (count != 2) return;
                    objects.push(tree1.getItem(value));
                });
                $.ajax({
                    async: false,
                    type: "POST",
                    data: JSON.stringify({ "list" : objects }),
                    contentType: "application/json",
                    dataType: "json",
                    url: "wh.api/ModelMapping/SaveChecklist",
                    success: function (data) {
                        if (data.message == "") {
                            $('#btnSave').attr("disabled", "disabled");
                            return;
                        } else {
                            MsgBox(data.message);
                        }
                    }
                });

            }
        });
    }

    function _onCmbGroupCodeChanged(newv, oldv) {
        if (newv != "") {
            loadTypeCodes(newv);
        }
    }

    function saveCreateCheck() {
        var groupCode = $$('cmbGroupCode').getValue();
        if (groupCode == "") {
            MsgBox("Type can't be empty");
            return;
        }
        var typeCode = $$('cmbTypeCode').getValue();
        if (typeCode == "") {
            MsgBox("Variant can't be empty");
            return;
        }
        var groupSeq = $$("txtGroupCodeSeq").getValue();
        if (groupSeq <= 0) {
            MsgBox("Type sequence can't be zero or less");
            return;
        }
        var typeSeq = $$("txtTypeCodeSeq").getValue();
        if (typeSeq <= 0) {
            MsgBox("Variant sequence can't be zero or less");
            return;
        }
        var trans = $$('cmbTrans').getValue();
        var params = {
            groupCode: groupCode,
            typeCode: typeCode,
            groupSeq: groupSeq,
            typeSeq: typeSeq,
            trans: trans
        }
        $.ajax({
            async: false,
            type: "POST",
            data: params,
            url: "wh.api/ModelMapping/CheckNewTrans",
            success: function (data) {
                if (data.message != "") {
                    if (data.errorType = 1) {
                        MsgBox(data.message);
                        return;
                    } else if (data.errorType = 2) {
                        MsgConfirm(data.message, function (ok) {
                            if (ok) saveCreate(params);
                        });
                    }
                } else {
                    saveCreate(params);
                }
            }
        });
    }

    function saveCreate(params) {
        $.ajax({
            async: false,
            type: "POST",
            data: params,
            url: "wh.api/ModelMapping/NewItem",
            success: function (data) {
                if (data.message == "") {
                    reorderTypeCodes(params.groupCode);
                    reorderGroupCodes();
                    window1.hide();
                    return;
                } else {
                    MsgBox(data.message);
                    return;
                }
            }
        });
    }

    function saveEdit(event) {
        var item = event.data.item;
        var newSeq = item.lvl == 0 ? $$("txtGroupCodeSeq").getValue() :
            item.lvl == 1 ? $$("txtTypeCodeSeq").getValue() : "";
        var oldSeq = item.lvl == 0 ? item.GroupCodeSeq :
            item.lvl == 1 ? item.TypeCodeSeq : 0;
        if (newSeq == oldSeq) {
            MsgBox("Please enter a new sequence");
            return;
        }
        if (newSeq == "") {
            MsgBox("Sequence can't be empty");
            return;
        }
        if (newSeq <= 0) {
            MsgBox("Sequence can't be zero or less");
            return;
        }
        $.ajax({
            async: false,
            type: "POST",
            data: {
                groupCode: item.GroupCode,
                oldSeq: oldSeq,
                newSeq: newSeq,
                lvl: item.lvl
            },
            url: "wh.api/ModelMapping/SwapSequences",
            success: function (data) {
                if (data.message == "") {
                    window1.hide();
                    if (item.lvl == 1) reorderTypeCodes(item.GroupCode);
                    reorderGroupCodes();
                    return;
                } else {
                    MsgBox(data.message);
                    return;
                }
            }
        });

    }

    function editItem(item) {
        resetWindow();
        $('#windowTitle').html("Edit Model");
        $('[view_id=winCreate]').children().children()[0].value = "Save";
        $('[view_id=winCreate]').children().children().on("click", { item: item }, saveEdit);
        if (item.lvl == 0) {
            $('[view_id=winTypeCode]').css('display', 'none');
            $('[view_id=winTypeCodeSeq]').css('display', 'none');
            $('[view_id=winTrans]').css('display', 'none');
            $$('cmbGroupCode').$setValue(item.GroupCode);
            $$('txtGroupCodeSeq').setValue(item.GroupCodeSeq);
            $($$('cmbGroupCode').getNode()).children().find("input").attr("disabled", "disabled");
        } else if (item.lvl == 1) {
            $('[view_id=winGroupCode]').css('display', 'none');
            $('[view_id=winGroupCodeSeq]').css('display', 'none');
            $('[view_id=winTrans]').css('display', 'none');
            $$('cmbTypeCode').$setValue(item.TypeCode);
            $$('txtTypeCodeSeq').setValue(item.TypeCodeSeq);
            $($$('cmbTypeCode').getNode()).children().find("input").attr("disabled", "disabled");
        } else if (item.lvl == 2) {
            var msg = "Type: " + item.GroupCode +
                "\nVariant: " + item.TypeCode +
                "\nTransmissionType: " + item.TransmissionType + "\n";
            if (item.$count > 0) msg += "This entry already has Sales Model Code(s) assigned. Delete anyway?";
            else msg += "Do you really want to delete this data?";
            MsgConfirm(msg, function (ok) {
                if (!ok) return false;
                $.ajax({
                    async: false,
                    type: "POST",
                    data: {
                        groupCode: item.GroupCode,
                        typeCode: item.TypeCode,
                        trans: item.TransmissionType
                    },
                    url: "wh.api/ModelMapping/DeleteItem",
                    success: function (data) {
                        if (data.message == "") {
                            reorderGroupCodes();
                            return false;
                        } else {
                            MsgBox(data.message);
                            return false;
                        }
                    }
                });
            });
            return false;
        } else return false;
                    
        fixWindowCss();
    }

    function newItem() {
        resetWindow();
        $('[view_id=winCreate]').children().children()[0].value = "Create";
        $('[view_id=winCreate]').children().children().on("click", saveCreateCheck);
        $('#windowTitle').html("Create New Model");
        $$('cmbGroupCode').$setValue("");
        $$('cmbTypeCode').$setValue("");
        $$('cmbTrans').setValue("MT");
        $$('txtGroupCodeSeq').setValue("0");
        $$('txtTypeCodeSeq').setValue("0");
        fixWindowCss();
    }

    function reorderGroupCodes() {
        $.ajax({
            async: false,
            type: "POST",
            url: "wh.api/ModelMapping/ReorderGroupCodes",
            success: function (data) {
                if (data.message != "") MsgBox(data.message)
                else init();
            }
        });
    }

    function reorderTypeCodes(groupCode) {
        $.ajax({
            async: false,
            type: "POST",
            data: {
                groupCode: groupCode
            },
            url: "wh.api/ModelMapping/ReorderTypeCodes",
            success: function (data) {
                if (data.message != "") MsgBox(data.message);
            }
        });
    }

    function resetWindow() {
        $('[view_id=winCreate]').children().children().off("click");
        $('[view_id=winGroupCode]').css('display', 'inline-block');
        $('[view_id=winGroupCodeSeq]').css('display', 'inline-block');
        $('[view_id=winTypeCode]').css('display', 'inline-block');
        $('[view_id=winTypeCodeSeq]').css('display', 'inline-block');
        $('[view_id=winTrans]').css('display', 'inline-block');
        $($$('cmbGroupCode').getNode()).children().find("input").removeAttr("disabled");
        $($$('cmbTypeCode').getNode()).children().find("input").removeAttr("disabled");
    }

    function fixWindowCss() {
        window1.show();
        $(".webix_view.webix_toolbar.webix_layout_toolbar").css("background-image", "linear-gradient(to bottom,#fff,#f2f2f2)");
        $('input.webixtype_base,.webix_el_box,.webix_el_box>input').css("font-family", "Segoe UI");
        $('input.webixtype_base').css("line-height", "1.6");
        $('.webix_modal').css("height", "150%");
        loadGroupCodes();
    }

    function help() {
        var helptext = "Enabled actions:\n\n" +
            "New Model:\n- Click the New button to create new Type, Variant and Transmission Type\n\n" +
            "Assign Sales Model Code:\n- Drag from right table to Transmission Type on left table. Multi-select and multi-drag is enabled.\n\n" +
            "Withdraw Assigned Sales Model Code:\n- Drag Sales Model Code from left table to right table\n\n" +
            "Change Sequence:\n- Drag Type or Variant to the Edit Box\n\n" +
            "Delete: \n- Drag Transmission Type to the Edit Box\n\n" +
            "Select item for Mssi Report:\n- Tick the checkboxes for items in left table then use the Save button"
            ;
        MsgBox(helptext);
    }

    $('#btnNew').click(newItem);
    $('#btnReload').click(init);
    $('#btnSave').click(save);
    $('#btnHelp').click(help);

    $(window).on('beforeunload', function (e) {
        if (!isChanged) return;
        var msg = "You are trying to reload or navigate away from this page without saving.";
        return msg;
    });

    var nEdit = $($$('toolEdit').getNode()).children().children()[0];
    $(nEdit).css("background", "linear-gradient(to bottom,#ffffff,#e8e8e8)");
    $(nEdit).css("color", "black");
    $(nEdit).css("display", "table-cell");
    $(nEdit).css("vertical-align", "middle");
    $(nEdit).css("cursor", "-webkit-grab");
    window1.show(); window1.hide();
});
$(".webix_list_item").css("text-align", "center");
$(".webix_list_item").css("font-family", "Segoe UI");
$(".webix_list_item").css("color", "black");
$(".webix_view.webix_list").css("margin-top", "8px");
$(".webix_view.webix_control.webix_el_label").css("text-align", "center");
$(".webix_view.webix_control.webix_el_label").css("font-family", "Segoe UI");