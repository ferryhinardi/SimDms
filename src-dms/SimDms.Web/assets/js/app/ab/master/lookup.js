
var widget;

$(document).ready(function () {
    var options = {
        title: "Master Lookup",
        xtype: "panels",
        panels: [
            {
                title: "Filter Lookup",
                items: [
                    {
                        type: "controls",
                        text: "Company",
                        items: [
                            { name: "CompanyCode", readonly: true, cls: "span2" },
                            { name: "CompanyName", readonly: true, cls: "span6" },
                        ]
                    },
                    {
                        text: "Group Filter",
                        type: "controls",
                        items: [
                            { name: "GroupFilter", type: "select", cls: "span4 full" },
                        ]
                    },
                ]
            },
            {
                title: "Detail Lookup",
                xtype: "grid",
                selectable: true,
                name: "tblLookup",
                tblname: "tblLookup",
                pnlname: "pnlLookup",
                source: "ab.api/grid/hrlookups",
                sortings: [[1, "asc"], [2, "asc"]],
                buttons: [{ name: "btnAddDtl", text: "Add New", icon: "icon-plus" }],
                items: [
                    {
                        text: "Group",
                        type: "controls",
                        items: [
                            { name: "CodeID", type: "select", cls: "span4 full", required: true },
                        ]
                    },
                    {
                        text: "Value",
                        type: "controls",
                        items: [
                            { name: "LookUpValue", cls: "span2", placeHolder: "Value", required: true },
                            { name: "ParaValue", cls: "span2", placeHolder: "Para Value", required: true },
                            { name: "LookUpValueName", cls: "span4", placeHolder: "Description", required: true },
                        ]
                    },
                    { name: "SeqNo", text: "SeqNo", cls: "span2 number-int", required: true },
                    { type: "buttons", items: [{ name: "btnDtlSave", text: "Save", icon: "icon-save" }, { name: "btnDtlCancel", text: "cancel", icon: "icon-undo" }] },
                    { type: "divider" }
                ],
                columns: [
                    {
                        mData: "LookUpValue", sTitle: "", sWidth: "40px",
                        mRender: function (data, type, full) {
                            return "<i class=\"icon icon-edit link\"></i>";
                        }
                    },
                    {
                        mData: "LookUpValue", sTitle: "", sWidth: "40px",
                        mRender: function (data, type, full) {
                            return "<i class=\"icon icon-trash link\"></i>";
                        }
                    },
                    { mData: "CodeDescription", sTitle: "Group", sWidth: "200px" },
                    { mData: "SeqNo", sTitle: "SeqNo", sWidth: "100px" },
                    { mData: "LookUpValue", sTitle: "Value", sWidth: "160px" },
                    { mData: "ParaValue", sTitle: "Para Value", sWidth: "140px" },
                    { mData: "LookUpValueName", sTitle: "Description" },
                ],
                additionalParams: [
                    { name: "GroupFilter", element: "GroupFilter", type: "select" },
                ],
            }
        ],
    }

    widget = new SimDms.Widget(options);

    widget.setSelect([
        { name: "GroupFilter", url: "ab.api/Combo/LookupCodes/", optionalText: "-- SELECT ALL --" },
        { name: "CodeID", url: "ab.api/combo/LookupCodes/" }
    ]);

    widget.onGridClick(function (icon, data) {
    console.log(icon);
        if(icon == "edit")
        {
            $('#CodeID').attr('disabled','disabled');
            $('#LookUpValue').attr('disabled','disabled');
            showPanelLookup(true, "edit", data);
        }
        else if(icon == "trash")
        {
            if (confirm("Are you sure that you want to delete this data?")) {
                console.log(data);
                widget.post("ab.api/Lookup/Delete", data, function (result) {
                    if (result.success) {
                        widget.reloadGridData("tblLookup");
                        showPanelLookup(false, "add", "");
                    }
                    else {
                        alert(result.message);
                    }
                });
            }
        }
    });

    widget.default = {}
    widget.render(renderCallback);
});

function renderCallback() {
    reloadData();
    evt_btnAddDtl();
    evt_GroupFilter();
    evt_btnSaveDtl();
    evt_btnCancelDtl();
    fixingFormPosition();
}

function reloadData() {
    widget.post("ab.api/lookup/default", function (result) {
        widget.default = result;
        widget.populate(widget.default);
    });
}

function evt_btnAddDtl() {
    $("#btnAddDtl").on("click", function (evt) {
        evt.preventDefault();
        $('#CodeID').removeAttr('disabled');
        $('#LookUpValue').removeAttr('disabled');
        clearPanelLookup();
        showPanelLookup(true, "add", "");
    });
}

function evt_GroupFilter() {
    $("#GroupFilter").on("change", function () {
        widget.reloadGridData("tblLookup");
    });
}

function evt_btnSaveDtl() {
    var btnSaveDtl = $("#btnDtlSave");

    btnSaveDtl.on("click", function (event) {
        event.preventDefault();
        saveLookup();

    });
}

function evt_btnCancelDtl() {
    var btnCancelDtl = $("#btnDtlCancel");
    btnCancelDtl.on("click", function (event) {
        event.preventDefault();
//        var isValid = widget.validate();
        showPanelLookup(false, "add", "");

    });
}

function showPanelLookup(isShowed, command, data) {
    var panelLookup = $("#pnlLookup");
    
    if(command == "edit")
    {
        $('#CodeID').val(data.CodeID);
        $('#CodeDescription').val(data.CodeDescription);
        $('#LookUpValue').val(data.LookUpValue);
        $('#LookUpValueName').val(data.LookUpValueName);
        $('#ParaValue').val(data.ParaValue);
        $('#SeqNo').val(data.SeqNo);
    }

    if (isShowed) {
        panelLookup.slideDown();
    }
    else {
        panelLookup.slideUp();
        widget.clearValidation();
    }
}

function clearPanelLookup() {
    $("#pnlLookup input").val("");
    $("#pnlLookup select").val("");
}

function fixingFormPosition() {
             
}

function saveLookup() {
    var url = "ab.api/Lookup/Save";
    var data = widget.getForms();

    console.log(url, data);
    
        if (widget.validate() == true) {
            widget.post("ab.api/Lookup/Save", data, function (result) {
                if (result.success) {
                    widget.reloadGridData("tblLookup");
                    showPanelLookup(false, "add", "");
                }
                else {
                    alert(result.message);
                }
            });
        }
}

function loadFilterContent() {
     
}
