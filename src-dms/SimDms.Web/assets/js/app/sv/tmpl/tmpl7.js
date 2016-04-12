$(document).ready(function () {
    var options = {
        title: "Multi Panel",
        xtype: "panels",
        toolbars: [
            { name: "btnAdd", text: "Add", icon: "icon-plus", cls: "hide" },
            { name: "btnClear", text: "Clear", icon: "icon-undo", cls: "hide" },
            { name: "btnSave", text: "Save", icon: "icon-save", cls: "hide" },
            { name: "btnCancel", text: "Cancel", icon: "icon-undo", cls: "hide" },
        ],
        panels: [
            {
                name: "Panel01",
                title: "Panel 01",
                items: [
                    {
                        text: "Employee",
                        type: "controls",
                        items: [
                            { name: "EmployeeID", cls: "span2", placeHolder: "NIK", type: "popup", btnName: "btnEmployeeBrowse" },
                            { name: "EmployeeName", cls: "span6", placeHolder: "Name", readonly: true }
                        ]
                    },
                    { name: "Email", text: "Email", cls: "span4", readonly: true },
                    { name: "Handphone1", text: "Handphone", cls: "span4", readonly: true },
                ]
            },
            {
                name: "Panel02",
                title: "Panel 02",
                xtype: "table",
                pnlname: "pnlMutation",
                tblname: "tblMutation",
                showcheckbox: true,
                items: [
                    { name: "MutationDate", text: "Mutation Date", cls: "span4", type: "datepicker", required: true },
                    { name: "BranchCode", text: "Mutation Branch", cls: "span4", type: "select", required: true },
                    { name: "IsJoinDate", text: "Is Join Date", type: "switch", float: "left", cls: "IsJoinDate" },
                    { name: "CompanyCode", readonly: true, type: "hidden" }
                ],
                columns: [
                    { name: "MutationDate", text: "Mutation Date", width: 120 },
                    { name: "BranchName", text: "Mutation Branch", width: 480 },
                    { name: "MutationInfo", text: "Mutation Info" },
                    { name: "", text: "&nbsp;#", type: "action", width: 70 },
                ]
            }
        ],
    }

    var widget = new SimDms.Widget(options);
    widget.render(function () {
        widget.select({ selector: "#BranchCode", url: "ab.api/Combo/Branch" });
        $('#BranchCode').attr('disabled', 'disabled');

        //var opts = {
        //    selector: "#tblMutation",
        //    url: "ab.api/mutation/GetDataMutation",
        //    params: { CompanyCode: "6115204", EmployeeID: "00021" }
        //}
        //widget.populateTable(opts);

        widget.post("ab.api/mutation/GetDataMutation",
            { CompanyCode: "6115204", EmployeeID: "00021" }, function (result) {
            if (result.success) {
                widget.populateTable({
                    selector: "#tblMutation",
                    data: result.list,
                    selectable: true,
                    multiselect: true
                });
            }
        });
    });
});