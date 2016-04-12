$(document).ready(function () {
    var options = {
        title: "History Mutation",
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
                            { name: "EmployeeID", cls: "span2", placeHolder: "NIK", type: "popup", btnName: "btnEmployeeBrowse" },
                            { name: "EmployeeName", cls: "span6", placeHolder: "Name", readonly: true }
                        ]
                    },
                    { name: "Email", text: "Email", cls: "span4", readonly: true },
                    { name: "Handphone1", text: "Handphone", cls: "span4", readonly: true },
                ]
            },
            {
                name: "MutationData",
                title: "Mutation List",
                xtype: "table",
                pnlname: "pnlMutation",
                tblname: "tblMutation",
                buttons: [{ name: "btnAdd", text: "Add New", icon: "icon-plus" }],
                items: [
                    { name: "MutationDate", text: "Mutation Date", cls: "span4", type: "datepicker", required: true },
                    { name: "BranchCode", text: "Mutation Branch", cls: "span4", type: "select", required: true },
                    { name: "IsJoinDate", text: "Is Join Date", type: "switch", float: "left", cls: "IsJoinDate" },
                    { name: "CompanyCode", readonly: true, type: "hidden" },
                    {
                        type: "buttons", items: [
                            { name: "btnSave", text: "Save", icon: "icon-save" },
                            { name: "btnCancel", text: "Cancel", icon: "icon-undo" }
                        ]
                    },
                ],
                columns: [
                    { name: "", text: "&nbsp;#", type: "action", width: 70 },
                    { name: "MutationDate", text: "Mutation Date", width: 180 },
                    { name: "BranchName", text: "Mutation Branch", width: 480 },
                    { name: "MutationInfo", text: "Mutation Info" },
                ]
            }
        ],
    }

    var widget = new SimDms.Widget(options);
    widget.render(function () {
        widget.select({ selector: "#BranchCode", url: "ab.api/Combo/Branch" });
        $('#BranchCode').attr('disabled', 'disabled');
    });

    widget.lookup.onDblClick(function (e, data, name) {
        widget.lookup.hide();
        if (name === "EmpList") {
            $("input[type='text']").val("");
            $("textarea").val("");
            widget.populate($.extend({}, widget.DefaultData, data));
            $(".toolbar > button").hide();
            $("#btnAdd").show();
            $("#EmployeeIDs").val($("#EmployeeID").val());
            $("#EmployeeNames").val($("#EmployeeName").val());
            populateData(data);
        }
    });

    //hide add mutation panel
    $('#btnAdd').hide();

    $('#EmployeeID').on('blur', function () {
        $('#pnlMutation').slideUp();
        getEmployeeDetails();
    });

    $('#EmployeeID').keypress(function (e) {
        if (e.which == 13) {
            $('#pnlMutation').slideUp();
            getEmployeeDetails();
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
                $("#btnAdd").show();
            }
            else {
                $(".toolbar > button").hide();
                $("#btnAdd").hide();
            }
        });
    }


    $('#btnAdd').on('click', function () {
        $('#pnlMutation').slideDown();
        $('input[name="MutationDate"]').removeAttr('disabled');
        $('#BranchCode').removeAttr('disabled');
        $('#BranchCode').focus();
        $('input[name="IsJoinDate"]').removeAttr('disabled');
        $("#btnAdd").hide();
        var dataPost = { EmployeeID: $('#EmployeeID').val(), CompanyCode: $("#CompanyCode").val() };
        widget.post("ab.api/mutation/IsJoinDateExist", dataPost, function (result) {
            if (result.status) {
                $('.IsJoinDate').hide();
            }
            else {
                $('.IsJoinDate').show();
            }
        });
    });


    $('#btnCancel').on('click', function () {
        $('#pnlMutation').slideUp();
        $('input[name="MutationDate"]').val('');
        widget.populate({ IsJoinDate: false });
        $('#BranchCode').val('');
        $('#BranchCode').attr('disabled', 'disabled');
        $("#btnAdd").show();
    });

    $("#btnSave").on("click", function () {
        var valid = $(".main form").valid();
        if (valid) {
            $('input[name="MutationDate"]').removeAttr('disabled');
            $('#BranchCode').removeAttr('disabled');
            $('input[name="IsJoinDate"]').removeAttr('disabled');
            var data = $(".main form").serialize();
            widget.post("ab.api/mutation/save", data, function (result) {
                if (result.success) {
                    populateData(data);
                    $('input[name="MutationDate"]').val('');
                    $('#BranchCode').val('');
                    $('#BranchCode').attr('disabled', 'disabled');
                    $('#pnlMutation').slideUp();
                    $("#btnAdd").show();
                    widget.populate({ IsJoinDate: false });
                    $('input[name="MutationDate"]').removeAttr('readonly');
                }
                else {
                    alert(result.message);
                }
            });
        }
    });

    $('input[name=MutationDate]').change(function () {
        var data = $(".main form").serializeObject();
        widget.post("sv.api/mutation/CheckMutationDate", data, function (result) {
            if (result.success) {
                $('#BranchCode').removeAttr('disabled');
            }
            else {
                alert(result.message);
                $('input[name=MutationDate]').val("");
                $('#BranchCode').val('');
                $('#BranchCode').attr('disabled', 'disabled');
            }
        });
    });

    $('#BranchCode').change(function () {
        var data = $(".main form").serializeObject();
        widget.post("sv.api/mutation/CheckMutationBranch", data, function (result) {
            if (result.success == false) {
                alert(result.message);
                $('#BranchCode').val("");
            }
            else {
                //$('#BranchCode').removeAttr('disabled');
            }
        });
    });

    function populateData(data) {
        widget.post("ab.api/mutation/GetDataMutation", data, function (result) {
            if (result.success) {
                widget.populateTable({ selector: "#tblMutation", data: result.list });
            }
            else {
                confirm(result.message);
            }
        });
    }

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

    $('input[name="IsJoinDate"]').on('change', function (event) {
        var dataPost = { CompanyCode: $('#CompanyCode').val(), EmployeeID: $('#EmployeeID').val() };
        setTimeout(function () {
            if ($('input[name="IsJoinDate"]').val() == "true") {
                widget.post("ab.api/mutation/GetJoinDetailsByEmployeeID", dataPost, function (result) {
                    $('input[name="MutationDate"]').val(result.data);
                    $('#BranchCode').removeAttr('disabled');
                    $('input[name="MutationDate"]').attr('disabled', 'disabled');
                });
            } else {
                $('input[name="MutationDate"]').removeAttr('disabled');
                $('input[name="MutationDate"]').val("");
                $('#BranchCode').attr('disabled', 'disabled');
            }
        }, 250);
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
        $('#pnlMutation').slideDown();
        $('#btnAdd').hide();
        $('input[name="MutationDate"]').val(data[1]);
        $('input[name="MutationDate"]').attr('disabled', 'disabled');
        $('input[name="IsJoinDate"]').attr('disabled', 'disabled');
        var branch = data[2].split(" - ");
        $('#BranchCode').val(branch[0]);
        $('#BranchCode').removeAttr('disabled');
        $('#BranchCode').focus();
        if (data[3] != "-") {
            $('.IsJoinDate').slideDown();
            widget.populate({ IsJoinDate: true });
        } else {
            $('.IsJoinDate').slideUp();
            widget.populate({ IsJoinDate: false });
        }
    }

    function deleteDetail(data) {
        var dataPost = { MutationDate: data[1], EmployeeID: $('#EmployeeID').val(), CompanyCode: $('#CompanyCode').val() };
        if (confirm("Anda yakin akan menghapus data ini?")) {
            widget.post("ab.api/mutation/delete", dataPost, function (result) {
                if (result.success) {
                    populateData(dataPost);
                    $('#pnlMutation').slideUp();
                    widget.populate({ IsJoinDate: false });
                    $('input[name="MutationDate"]').val('');
                    widget.populate({ IsJoinDate: false });
                    $('#BranchCode').val('');
                    $('#BranchCode').attr('disabled', 'disabled');
                    $("#btnAdd").show();
                }
                else {
                    alert(result.message);
                }
            });
        };
    }
});