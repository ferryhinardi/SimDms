$(document).ready(function () {
    var options = {
        title: "History Education",
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
                            { name: "EmployeeID", cls: "span2", placeHolder: "NIK", type: "popup", btnName:"btnEmployeeBrowse"  },
                            { name: "EmployeeName", cls: "span6", placeHolder: "Name", readonly: true }
                        ]
                    },
                    { text: "Email", cls: "span4", readonly:true },
                    { name: "Handphone1", text: "Handphone", cls: "span4", readonly: true },
                ]
            },
            {
                name:"EducationData",
                title: "Education List",
                xtype: "table",
                pnlname: "pnlEducation",
                tblname: "tblEducation",
                buttons: [{ name: "btnAdd", text: "Add New", icon: "icon-plus" }],
                items: [
                        { name: "College", text: "College", required: true, cls: "span4" },
                        { name: "Education", text: "Education Level", cls: "span4", type: "select", required: true },
                        { name: "YearBegin", text: "Year Begin", cls: "span4", type: "select", required: true },
                        { name: "YearFinish", text: "Year End", cls: "span4", type: "select", required: true },
                        { name: "CompanyCode", readonly: true, type: "hidden" },
                        { name: "EduSeq", readonly: true, type: "hidden" },
                        {
                            type: "buttons", items: [
                                { name: "btnSave", text: "Save", icon: "icon-save" },
                                { name: "btnCancel", text: "Cancel", icon: "icon-undo" }
                            ]
                        },
                ],
                columns: [
                    { name: "", text: "&nbsp;#", type: "action", width: 65 },
                    { name: "Education", text: "Edu Level", width: 120 },
                    { name: "College", text: "Institution Name", width: 280 },
                    { name: "YearBegin", text: "Year From", width: 100 },
                    { name: "YearFinish", text: "Year To", width: 100 },
                    { name: "EduSeq", text: "Edu Seq", width: 120},
                ]
            }
        ],
    }

    var widget = new SimDms.Widget(options);
    widget.render(function () {
        widget.select({ selector: "#Education", url: "ab.api/Combo/FormalEducations" });
        widget.select({ selector: "#YearBegin", url: "ab.api/Combo/Years" });
        widget.select({ selector: "#YearFinish", url: "ab.api/Combo/Years" });
    });

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
            $('input[name="JoinDate"]').val("");
        }
    });

    //hide add button
    $('#btnAdd').hide();

    //hide column in table
    $('th[data-field="EduSeq"]').hide();
   

    $('#EmployeeID').on('blur', function () {
        $('#pnlEducation').slideUp();
        getEmployeeDetails();
    });

    $('#EmployeeID').keypress(function (e) {
        if (e.which == 13) {
            getEmployeeDetails();
            $('#pnlEducation').slideUp();
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
        $('#pnlEducation').slideDown();
        //widget.showToolbars(["btnSave", "btnCancel"]);
        $('#btnAdd').hide();
    });

    $('#btnCancel').on('click', function () {
        $('#pnlEducation').slideUp();
        $('#btnAdd').show();
        $('#College,#Education,#YearBegin,#YearFinish,#EduSeq').val("");
    });

    $('#btnSave').on('click', function () {
        var valid = $(".main form").valid();
        if (valid) {
            var data = $(".main form").serialize();
            widget.post("ab.api/education/save", data, function (result) {
                if (result.success) {
                    populateData(data);
                    $('#pnlEducation').slideUp();
                    $('#btnAdd').show();
                    $('#College,#Education,#YearBegin,#YearFinish,#EduSeq').val("");
                    $('input[name="MutationDate"]').removeAttr('readonly');
                }
                else {
                    alert(result.message);
                }
            });
        }
    });

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
        $('#pnlEducation').slideDown();
        $('#College').val(data[2]);
        var valedu = $("#Education option:contains(" + data[1] + ")").val();
        $("#Education").val(valedu);
        $('#YearBegin').val(data[3]);
        $('#YearFinish').val(data[4]);
        $('#EduSeq').val(data[5]);
        //widget.showToolbars(["btnSave", "btnCancel"]);
        $('#btnAdd').hide();
    }

    function deleteDetail(data) {
        if (confirm("Anda yakin akan menghapus data ini?")) {
            var dataPostDelete = { CompanyCode: $('#CompanyCode').val(), EmployeeID: $('#EmployeeID').val(), EduSeq: data[5] };
            widget.post("ab.api/education/Delete", dataPostDelete, function (result) {
                populateData({ EmployeeID: $('#EmployeeID').val() });
                $('#pnlEducation').slideUp();
                $('#btnAdd').show();
                $('#College,#Education,#YearBegin,#YearFinish,#EduSeq').val("");
            });
        }
    }

    function getEmployeeDetails() {
        var dataPost = { EmployeeID: $('#EmployeeID').val() };
        widget.post("ab.api/mutation/GetEmployeeDetails", dataPost, function (result) {
            $('#Email').val(result.Email);
            $('#Handphone1').val(result.Handphone1);
            $('#EmployeeName').val(result.EmployeeName);
            $('#CompanyCode').val(result.CompanyCode);
            $('#EmployeeIDs').val($('#EmployeeID').val())   ;
            $('#EmployeeNames').val($('#EmployeeName').val());
            populateData({ EmployeeID: $('#EmployeeID').val() });
            if (result.CompanyCode != null) {
                $('#btnAdd').show();
            }
            else {
                $('#btnAdd').hide();
                $(".toolbar > button").hide();
            }
        });
    }

    function populateData(data) {
        widget.post("ab.api/education/GetEducationData", data, function (result) {
            if (result.success) {
                widget.populateTable({ selector: "#tblEducation", data: result.list });
                $('td:nth-child(6)').hide();
            }
            else {
                confirm(result.message);
            }
        });
    }
});