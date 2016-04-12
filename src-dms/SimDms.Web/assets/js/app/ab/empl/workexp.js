$(document).ready(function () {
    var options = {
        title: "Working Experience",
        xtype: "panels",
        panels: [
            {
                name: "ContactInfo",
                title: "Contact Information",
                items: [
                    {
                        text: "Employee",
                        type: "controls",
                        items: [
                            { name: "EmployeeID", cls: "span2", placeHolder: "NIK", type: "popup" },
                            { name: "EmployeeName", cls: "span6", placeHolder: "Name", readonly: true }
                        ]
                    },
                    { name: "Email", text: "Email", cls: "span4", readonly: true },
                    { name: "Handphone1", text: "Handphone", cls: "span4", readonly: true },
                ]
            },
            {
                name: "WorkExpData",
                title: "Experience List",
                xtype: "table",
                pnlname: "pnlWorkExp",
                tblname: "tblWorkExp",
                buttons: [{ name: "btnAdd", text: "Add New", icon: "icon-plus" }],
                items: [
                        { name: "NameOfCompany", text: "Company Name", required: true, cls: "span4" },
                        { name: "LeaderName", text: "Leader Name", cls: "span4", type: "text", required: true },
                        { name: "LeaderPhone", text: "Phone", cls: "span4", type: "text", required: true },
                        { name: "LeaderHp", text: "Handphone", cls: "span4", type: "text", required: true },
                        { name: "JoinDate", text: "Join Date", cls: "span4", type: "datepicker", required: true },
                        { name: "ResignDate", text: "Resign Date", cls: "span4", type: "datepicker", required: true },
                        { name: "ReasonOfResign", text: "Resign Reason", cls: "span8", type: "textarea", required: true },
                        { name: "CompanyCode", readonly: true, type: "hidden" },
                        { name: "ExpSeq", readonly: true, type: "hidden" },
                        {
                            type: "buttons", items: [
                                { name: "btnSave", text: "Save", icon: "icon-save" },
                                { name: "btnCancel", text: "Cancel", icon: "icon-undo" }
                            ]
                        },
                ],
                columns: [
                    { name: "", text: "&nbsp;#", type: "action", width: 80 },
                    { name: "NameOfCompany", text: "Company Name", width: 200 },
                    { name: "JoinDate", text: "Join Date", width: 120 },
                    { name: "ResignDate", text: "Resign Date", width: 120 },
                    { name: "ReasonOfResignShort", text: "Resign Description" },
                    { name: "ReasonOfResign", text: "" },
                    { name: "LeaderName", text: "" },
                    { name: "LeaderPhone", text: "" },
                    { name: "LeaderHP", text: "" },
                    { name: "ExpSeq", text: "" }
                ]
            }
        ],
    }

    var widget = new SimDms.Widget(options);
    widget.render();

    widget.lookup.onDblClick(function (e, data, name) {
        widget.lookup.hide();
        if (name === "EmpList") {
            $("input[type='text']").val("");
            widget.populate($.extend({}, widget.DefaultData, data));
            $(".toolbar > button").hide();
            $('#btnAdd').show();
            $('#EmployeeIDs').val($('#EmployeeID').val());
            $('#EmployeeNames').val($('#EmployeeName').val());
            populateData(data);
        }
    });

    //hide column in table
    $('th[data-field="ReasonOfResign"]').hide();
    $('th[data-field="LeaderName"]').hide();
    $('th[data-field="LeaderPhone"]').hide();
    $('th[data-field="LeaderHP"]').hide();
    $('th[data-field="ExpSeq"]').hide();

    $('#btnAdd').hide();

    $('#EmployeeID').on('blur', function () {
        getEmployeeDetails();
        $('#pnlWorkExp').slideUp();
    });

    $('#EmployeeID').keypress(function (e) {
        if (e.which == 13) {
            getEmployeeDetails();
            $('#pnlWorkExp').slideUp();
        }
    });

    $('#btnEmployeeID').on('click', function () {
        widget.lookup.init({
            name: "EmpList",
            title: "Employee List",
            source: "ab.api/grid/Employees",
            columns: [
                { mData: "EmployeeID", sTitle: "Cust Code", sWidth: "100px" },
                { mData: "EmployeeName", sTitle: "Customer Name", sWidth: "200px" },
                { mData: "Email", sTitle: "Email" },
                { mData: "Handphone1", sTitle: "Handphone No" },
            ]
        });
        widget.lookup.show();
    });

    $('#btnAdd').on('click', function () {
        $('#pnlWorkExp').slideDown();
        $('#btnAdd').hide();
        $('#NameOfCompany,#LeaderName,#LeaderPhone,#LeaderHp,#JoinDate,#ResignDate,#ReasonOfResign,#ExpSeq').val("");
        $('input[name="ResignDate"]').val('');
        $('input[name="JoinDate"]').val('');
        $('#NameOfCompany').focus();
    });

    $('#btnCancel').on('click', function () {
        $('#pnlWorkExp').slideUp();
        $('#btnAdd').show();
        $('#NameOfCompany,#LeaderName,#LeaderPhone,#LeaderHp,#JoinDate,#ResignDate,#ReasonOfResign,#ExpSeq').val("");
        $('input[name="ResignDate"]').val('');
        $('input[name="JoinDate"]').val('');
    });

    $('#btnSave').on('click', function () {
        var valid = $(".main form").valid();
        if (valid) {
            var data = $(".main form").serialize();
            widget.post("ab.api/experience/save", data, function (result) {
                if (result.success) {
                    populateData(data);
                    $('#pnlWorkExp').slideUp();
                    $('#btnAdd').show();
                    $('#NameOfCompany,#LeaderName,#LeaderPhone,#LeaderHp,#JoinDate,#ResignDate,#ReasonOfResign,#ExpSeq').val("");
                    $('input[name="ResignDate"]').val('');
                    $('input[name="JoinDate"]').val('');
                    populateData({ EmployeeID: $('#EmployeeID').val() });
                }
                else {
                    alert(resul.message);
                }
            });
        }
    });

    function getEmployeeDetails() {
        var dataPost = { EmployeeID: $('#EmployeeID').val() };
        widget.post("ab.api/mutation/GetEmployeeDetails", dataPost, function (result) {
            $('#Email').val(result.Email);
            $('#Handphone1').val(result.Handphone1);
            $('#EmployeeName').val(result.EmployeeName);
            $('#CompanyCode').val(result.CompanyCode);
            $('#EmployeeIDs').val($('#EmployeeID').val());
            $('#EmployeeNames').val($('#EmployeeName').val());
            populateData({ EmployeeID: $('#EmployeeID').val() });
            if (result.CompanyCode != null) {
                $('#btnAdd').show();
            }
            else {
                $(".toolbar > button").hide();
                $('#btnAdd').hide();
            }
        });
    }

    function populateData(data) {
        widget.post("ab.api/experience/GetWorkingExperinceData", data, function (result) {
            if (result.success) {
                widget.populateTable({ selector: "#tblWorkExp", data: result.list });
                $('td:nth-child(6)').hide();
                $('td:nth-child(7)').hide();
                $('td:nth-child(8)').hide();
                $('td:nth-child(9)').hide();
                $('td:nth-child(10)').hide();
            }
            else {
                confirm(result.message);
            }
        });
    }

    widget.onTableClick(function (icon, data) {
        switch (icon) {
            case "edit":
                editDetail(data);
                break;
            case "trash":
                deleteDetail(data);
                break;
            default:
                break;
        }
    });

    function editDetail(data) {
        $('#pnlWorkExp').slideDown();
        $('#NameOfCompany').val(data[1]);
        $('input[name="JoinDate"]').val(data[2]);
        $('input[name="ResignDate"]').val(data[3]);
        $('#ReasonOfResign').val(data[5]);
        $('#LeaderName').val(data[6]);
        $('#LeaderPhone').val(data[7]);
        $('#LeaderHp').val(data[8]);
        $('#ExpSeq').val(data[9]);
        //widget.showToolbars(["btnSave", "btnCancel"]);
        $('#btnAdd').hide();
        $('#NameOfCompany').focus();
    }

    function deleteDetail(data) {
        var dataDelete = { CompanyCode: $('#CompanyCode').val(), EmployeeID: $('#EmployeeID').val(), ExpSeq: data[9] };
        if (confirm("Anda yakin akan menghapus data ini?")) {
            widget.post("ab.api/experience/Delete", dataDelete, function (result) {
                if (result.success) {
                    $('#pnlWorkExp').slideUp();
                    $('#NameOfCompany,#LeaderName,#LeaderPhone,#LeaderHp,#JoinDate,#ResignDate,#ReasonOfResign,#ExpSeq').val("");
                    $('input[name="ResignDate"]').val('');
                    $('input[name="JoinDate"]').val('');
                    populateData({ EmployeeID: $('#EmployeeID').val() });
                    $('#btnAdd').show();
                }
                else {
                }
            });
        }
    }

});