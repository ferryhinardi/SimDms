var widget;
var transactionStatus;
var variables = {};

$(document).ready(function () {
    variables["DeleteAchievementStatus"] == false;

    var options = {
        title: "Service Information",
        xtype: "panels",
        toolbars: [
            //{ name: "btnClear", text: "Clear", icon: "icon-eraser" },
        ],
        panels: [
            {
                title: "Contact Information",
                items: [
                    {
                        text: "Employee",
                        type: "controls",
                        items: [
                            {
                                name: "EmployeeID", cls: "span2", placeHolder: "NIK", type: "popup-lookup", source: "ab.api/Grid/Employees", required: true, readonly: true,
                                columns: [
                                    { mData: "EmployeeID", sTitle: "NIK", sWidth: "80px" },
                                    { mData: "EmployeeName", sTitle: "Name", sWidth: "250px" },
                                    {
                                        mData: "Department", sTitle: "Department", sWidth: "100px"
                                    },
                                    { mData: "Position", sTitle: "Position", sWidth: "100px" },
                                    {
                                        mData: "JoinDate", sTitle: "Join Date", sWidth: "100px",
                                        mRender: function (prefix, value, fullData) {
                                            return widget.toDateFormat(widget.cleanJsonDate(fullData.JoinDate));
                                        }
                                    },
                                    {
                                        mData: "ResignDate", sTitle: "Resign Date", sWidth: "100px",
                                        mRender: function (prefix, value, fullData) {
                                            if (widget.isNullOrEmpty(fullData.ResignDate) == false) {
                                                return widget.toDateFormat(widget.cleanJsonDate(fullData.ResignDate));
                                            }
                                            else {
                                                return "-";
                                            }
                                        }
                                    },
                                    { mData: "Status", sTitle: "Status", sWidth: "100px" }
                                ],
                                additionalParams: [
                                    { name: "Department", value: "SERVICE" }
                                ]
                            },
                            { name: "EmployeeName", cls: "span6 ignore-uppercase", placeHolder: "Name", required: true, readonly: true }
                        ]
                    },
                    { text: "Service ID", cls: "span4", readonly: false, type: "popup", icon: "icon-save", hint: "clik here to save Sales ID ", name: "ServiceID" },
                    //{ text: "Service ID", cls: "span4", readonly: false, type: "text", icon: "icon-bolt", hint: "clik here to get ATPM ID", name: "ServiceID" },
                    { text: "Email", cls: "span4", name: "Email", readonly: true },
                    { text: "Handphone 1", cls: "span4", name: "Handphone1", readonly: true },
                    { text: "Handphone 2", cls: "span4", name: "Handphone2", readonly: true },
                    { text: "Office Location", type: "textarea", name: "OfficeLocation", readonly: true },
                ]
            },
            {
                title: "Job Information",
                items: [
                    { text: "Join Date", cls: "span4", name: "JoinDate", readonly: true },
                    { text: "Resign Date", cls: "span4", name: "ResignDate", readonly: true },
                    { text: "Department", cls: "span4", name: "DepartmentName", readonly: true },
                    { text: "Position", cls: "span4", name: "PositionName", readonly: true },
                    { text: "GradeName", cls: "span4", name: "GradeName", readonly: true },
                    { text: "Rank", cls: "span4", name: "RankName", readonly: true },
                    { text: "Manager", cls: "span4", name: "TeamLeaderName", readonly: true },
                    { text: "Additional Job 1", cls: "span4", name: "AdditionalJob1Name", readonly: true },
                    { text: "Additional Job 2", cls: "span4", name: "AdditionalJob2Name", readonly: true },
                    { text: "Personal Status", cls: "span4", name: "Status", readonly: true },
                ]
            },
            {
                xtype: "tabs",
                name: "tabHrEmployeeService",
                items: [
                    { name: "tabEmployeeAchievement", text: "Change Grade / Promotion" },
                    { name: "tabServiceTraining", text: "Service Training" },
                ]
            },
            {
                title: "Data",
                cls: "tabHrEmployeeService tabServiceTraining",
                xtype: "grid",
                name: "tblTrainingHistory",
                pnlname: "pnlTrainingHistory",
                source: "ab.api/Training/List",
                buttons: [
                    { name: "btnAddTraining", text: "Add Training", cls: "", icon: "icon-plus" }
                ],
                formName: "formTraining",
                selectable: true,
                multiselect: false,
                editButton: true,
                deleteButton: true,
                editAction: function (evt, data) {
                },
                deleteAction: function (evt, data) {
                    deleteTraining(evt, data);
                },
                sortings: [[1, "asc"]],
                items: [
                    { type: "datepicker", name: "TrainingDate", cls: "span5", text: "Training Date", required: true },
                    //{ type: "text", name: "DepartmentTraining", cls: "span5", text: "Department", required: false, readonly: true },
                    //{ type: "text", name: "PositionTraining", cls: "span5", text: "Position", required: false, readonly: true },
                    //{ type: "text", name: "GradeTraining", cls: "span5", text: "Grade", required: false, readonly: true },
                    {
                        type: "input-multi", cls: "columns-3", elements: [
                            { name: "DepartmentTraining", cls: "", readonly: true, placeholder: "Department" },
                            { name: "PositionTraining", cls: "", readonly: true, placeholder: "Position" },
                            { name: "GradeTraining", cls: "", readonly: true, placeholder: "Grade" },
                        ], text: "Employee Details", required: false
                    },
                    {
                        text: "Pre Test", type: "hetero-input", cls: "columns-2", elements: [
                            { type: "text", name: "PreTest", cls: "", readonly: false },
                            { type: "select", name: "PreTestAlt", cls: "", readonly: true, optionalText: "" },
                        ]
                    },
                    { type: "select", name: "TrainingCode", cls: "span5", text: "Training", required: true },
                    //{ type: "text", name: "PreTest", cls: "span5", text: "Pre Test" },
                    //{ type: "select", name: "PreTestAlt", cls: "span5", text: "Pre Test Alternative" },
                    {
                        text: "Post Test", type: "hetero-input", cls: "columns-2", elements: [
                            { type: "text", name: "PostTest", cls: "", readonly: false },
                            { type: "select", name: "PostTestAlt", cls: "", readonly: true, optionalText: "" },
                        ]
                    },
                    //{ type: "text", name: "PostTest", cls: "span5", text: "Post Test" },
                    //{ type: "select", name: "PostTestAlt", cls: "span5", text: "Pos Test Alternative" },
                    { type: "buttons", items: [{ name: "btnSaveTraining", text: "Save", icon: "icon-save" }, { name: "btnCancelTraining", text: "cancel", icon: "icon-undo" }] },
                ],
                columns: [
                    {
                        sTitle: "Action",
                        "mDataProp": "",
                        "sClass": "",
                        "sDefaultContent": "</i><i class='icon icon-trash'></i>",
                        sWidth: "100px"
                    },
                    { mData: "EmployeeID", sTitle: "NIK", sWidth: "100px" },
                    { mData: "EmployeeName", sTitle: "Name", sWidth: "200px" },
                    { mData: "TrainingName", sTitle: "Training Name", sWidth: "150px" },
                    {
                        mData: "TrainingDate", sTitle: "Training Date", sWidth: "100px",
                        mRender: function (data, type, full) {
                            if (widget.isNullOrEmpty(data) == false) {
                                return widget.toDateFormat(widget.cleanJsonDate(data));
                            }
                            return "-";
                        }
                    },
                    { mData: "PreTest", sTitle: "Pre", sWidth: "50px" },
                    { mData: "PostTest", sTitle: "Post", sWidth: "50px" },
                ],
                additionalParams: [
                    { name: "EmployeeID", element: "EmployeeID" },
                    { name: "Department", value: "SERVICE" },
                ],
            },
            {
                title: "Data",
                cls: "tabHrEmployeeService tabEmployeeAchievement",
                xtype: "grid",
                name: "tblEmployeeAchievement",
                pnlname: "pnlEmployeeAchievement",
                source: "ab.api/Achievement/List",
                buttons: [
                    { name: "btnAddAchievement", text: "Add Achievement", cls: "", icon: "icon-plus" }
                ],
                formName: "formAchievement",
                selectable: true,
                multiselect: false,
                editButton: true,
                deleteButton: true,
                editAction: function (evt, data) {
                },
                deleteAction: function (evt, data) {
                    deleteAchievement(evt, data);
                },
                sortings: [[1, "asc"]],
                items: [
                    { type: "select", name: "DepartmentAchievement", cls: "span5", text: "Department", required: true },
                    { type: "select", name: "PositionAchievement", cls: "span5", text: "Position", required: true },
                    { type: "select", name: "GradeAchievement", cls: "span5", text: "Grade", required: false },
                    { type: "datepicker", name: "AssignDate", cls: "span5", text: "Assign Date", required: true },
                    { type: "switch", name: "IsJoinDate", cls: "span5", text: "is Join Date", float: "left" },
                    { type: "buttons", items: [{ name: "btnSaveAchievement", text: "Save", icon: "icon-save" }, { name: "btnCancelAchievement", text: "cancel", icon: "icon-undo" }] },
                ],
                columns: [
                    {
                        sTitle: "Action",
                        "mDataProp": "",
                        "sClass": "",
                        "sDefaultContent": "</i><i class='icon icon-trash'></i>",
                        sWidth: "100px"
                    },
                    { mData: "EmployeeID", sTitle: "NIK", sWidth: "100px" },
                    { mData: "EmployeeName", sTitle: "Name", sWidth: "200px" },
                    { mData: "DepartmentName", sTitle: "Department", sWidth: "80px" },
                    { mData: "PositionName", sTitle: "Position", sWidth: "200px" },
                    { mData: "GradeName", sTitle: "Grade", sWidth: "100px" },
                    {
                        mData: "AssignDate", sTitle: "Assign Date", sWidth: "100px",
                        mRender: function (data, type, full) {
                            if (widget.isNullOrEmpty(data) == false) {
                                return widget.toDateFormat(widget.cleanJsonDate(data));
                            }
                            return "-";
                        }
                    },
                    { mData: "AssignDateStatus", sTitle: "Status", sWidth: "100px" },
                ],
                additionalParams: [
                    { name: "EmployeeID", element: "EmployeeID" }
                ],
            },
        ],
    }

    transactionStatus = false;
    widget = new SimDms.Widget(options);
    widget.lookup.onDblClick(function (e, data, name) {
        lookupOnDblClick(e, data, name);
    });
    var paramsEvent = [
        {
            name: "TrainingDate",
            type: "input",
            eventType: "change",
            event: function (evt) {
                var url = "ab.api/Training/TrainingList";
                var params = {
                    EmployeeID: $("[name='EmployeeID']").val(),
                    TrainingDate: $("[name='TrainingDate']").val()
                };

                widget.post(url, params, function (result) {
                    if (widget.isNullOrEmpty(result.status)) {
                        widget.setItems({
                            name: "TrainingCode",
                            type: "select",
                            data: result
                        });

                        url = "ab.api/Employee/GetDetailsEmployeePosition";
                        params = {
                            EmployeeID: $("[name='EmployeeID']").val(),
                            AssignDate: $("[name='TrainingDate']").val()
                        };

                        widget.post(url, params, function (result) {
                            if (widget.isNullOrEmpty(result.data) == false) {
                                $("[name='DepartmentTraining']").val(result.data.Department);
                                $("[name='PositionTraining']").val(result.data.Position);
                                $("[name='GradeTraining']").val(result.data.Grade);
                            }
                        });
                    }
                    else {
                        $("[name='TrainingDate']").val('');
                        alert(result.message);
                    }
                });
            }
        },
        {
            name: "btnAddAchievement",
            type: "button",
            eventType: "click",
            event: function (evt) {
                evt_btnAddAchievement(evt);
            }
        },
        {
            name: "btnSaveAchievement",
            type: "button",
            eventType: "click",
            event: function (evt) {
                evt_btnSaveAchievement(evt);
            }
        },
        {
            name: "btnCancelAchievement",
            type: "button",
            eventType: "click",
            event: function (evt) {
                evt_btnCancelAchievement(evt);
            }
        },
        {
            name: "btnServiceID",
            type: "button",
            eventType: "click",
            event: function (evt) {
                evt_btnServiceID(evt);
            }
        },
        {
            name: "btnClear",
            type: "button",
            eventType: "click",
            event: function (evt) {
                clearForm(evt);
            }
        },
        {
            name: "btnAddTraining",
            type: "button",
            eventType: "click",
            event: function (evt) {
                evt_btnAddTraining(evt);
            }
        },
        {
            name: "PositionTraining",
            type: "select",
            eventType: "change",
            event: function (evt) {
                var currentVal = widget.getValue({ name: "PositionTraining", type: "select" });
                var gradeElement = widget.getObject("GradeTraining", "select");
                if (currentVal == "S") {
                    gradeElement.attr("required", "required");
                }
                else {
                    gradeElement.removeAttr("required");
                    gradeElement.removeClass("error");
                    gradeElement.parent().children("label").remove();
                }
            }
        },
        {
            name: "btnSaveTraining",
            type: "button",
            eventType: "click",
            event: function (evt) {
                evt_btnSaveTraining(evt);
            }
        }, 
        {
            name: "btnCancelTraining",
            type: "button",
            eventType: "click",
            event: function (evt) {
                evt_btnCancelTraining(evt);
            }
        },
        {
            name: "PositionAchievement",
            type: "select",
            eventType: "change",
            event: function (evt) {
                var currentVal = widget.getValue({ name: "PositionAchievement", type: "select" });
                var gradeElement = widget.getObject("GradeAchievement", "select");
                if (currentVal == "S") {
                    gradeElement.attr("required", "required");
                    widget.showInputElement({
                        name: "GradeAchievement",
                        type: "select",
                        visible: true
                    });
                }
                else {
                    widget.showInputElement({
                        name: "GradeAchievement",
                        type: "select",
                        visible: false
                    });
                    gradeElement.removeAttr("required");
                    gradeElement.removeClass("error");
                    gradeElement.parent().children("label").remove();
                }
            }
        },
    ];
    widget.setEventList(paramsEvent);

    var paramsSelect = [
        {
            name: "PreTestAlt", url: "ab.api/Combo/TrainingPreScoresAlternative"
        },
        {
            name: "PostTestAlt", url: "ab.api/Combo/TrainingPostScoresAlternative"
        },
        //{
        //    name: "PositionTraining", url: "ab.api/Training/PositionList",
        //    cascade: {
        //        name: "DepartmentTraining",
        //        additionalParams: [
        //            { name: "employeeID", source: "EmployeeID", type: "input" }
        //        ]
        //    }
        //},
        {
            name: "DepartmentAchievement", url: "ab.api/Combo/Departments",
        },
        {
            name: "PositionAchievement", url: "ab.api/Combo/Positions",
            cascade: {
                name: "DepartmentAchievement",
            }
        },
        {
            name: "GradeTraining", url: "ab.api/Combo/Grades",
            cascade: {
                name: "PositionTraining",
                additionalParams: [],
                conditions: [
                    { name: "PositionTraining", condition: "=='S'" }
                ]
            }
        },
        {
            name: "GradeAchievement", url: "ab.api/Combo/Grades",
            cascade: {
                name: "PositionAchievement",
                additionalParams: [],
                conditions: [
                    { name: "PositionAchievement", condition: "=='S'" }
                ]
            }
        },
        {
            name: "TrainingCode", url: "ab.api/Training/TrainingList",
            cascade: {
                name: "PositionTraining",
                additionalParams: [
                    { name: "EmployeeID", source: "EmployeeID", type: "input" },
                    { name: "Department", source: "DepartmentTraining", type: "select" },
                    { name: "Position", source: "PositionTraining", type: "select" },
                    { name: "Grade", source: "GradeTraining", type: "select" },
                ]
            }
        },
        {
            name: "TrainingCode", url: "ab.api/Training/TrainingList",
            cascade: {
                name: "GradeTraining",
                additionalParams: [
                    { name: "EmployeeID", source: "EmployeeID", type: "input" },
                    { name: "Department", source: "DepartmentTraining", type: "select" },
                    { name: "Position", source: "PositionTraining", type: "select" },
                    { name: "Grade", source: "GradeTraining", type: "select" },
                ]
            }
        },
    ];
    widget.setSelect(paramsSelect);

    widget.render();
});





function lookupOnDblClick(e, data, name) {
    if (widget.isNullOrEmpty(data.JoinDate) == false) {
        try {
            data["JoinDate"] = widget.toDateFormat(widget.cleanJsonDate(data.JoinDate));
            data["ResignDate"] = widget.toDateFormat(widget.cleanJsonDate(data.ResignDate));
        } catch (ex) { }
    }
    widget.populate(data, populateCallback(e, data, name));
    widget.lookup.hide();
    transactionStatus = true;

    reloadTrainingData();
    reloadAchievementData();
}

function populateCallback(e, data, name) {
    var ServiceIDStatus = false;
    if (widget.isNullOrEmpty(data.ServiceID) == false) {
        ServiceIDStatus = false;
    }
    else {
        ServiceIDStatus = true;
    }
    widget.enableElement([
        { name: "ServiceID", type: "text", status: ServiceIDStatus },
        { name: "btnServiceID", type: "text", status: ServiceIDStatus },
    ]);
    //widget.enableElement([{ name: "btnServiceID", type: "text", status: ServiceIDStatus }]);

    //widget.selects({
    //    name: "DepartmentTraining",
    //    source: "ab.api/Training/DepartmentList",
    //    additionalParams: [
    //        { name: "EmployeeID", element: "EmployeeID", type: "input", value: data.EmployeeID }
    //    ]
    //});

    widget.selects({
        name: "DepartmentAchievement",
        source: "ab.api/Combo/Departments",
        additionalParams: [
            { name: "EmployeeID", element: "EmployeeID", type: "input", value: data.EmployeeID }
        ]
    });

    setTimeout(function () {
        widget.reloadGridData("tblTrainingHistory");
    }, 500);
}

function evt_btnServiceID(evt) {
    if (transactionStatus && widget.isNullOrEmpty($("#ServiceID").val()) == false) {
        var params = {
            EmployeeID: widget.getValue({ name: "EmployeeID", type: "text" }),
            ServiceID: widget.getValue({ name: "ServiceID", type: "text" }),
        };

        var url = "ab.api/Employee/SaveServiceID";

        widget.post(url, params, function (result) {
            if (result.status) {
                var salesIDElement = widget.getObject("SalesID");
                salesIDElement.val(result.atpmID);
                widget.enableElement([
                    { name: "ServiceID", type: "input", status: false },
                    { name: "btnServiceID", type: "input", status: false },
                ]);
            }
        });
    }
}

function clearForm(evt) {
    transactionStatus = false;
    widget.enableElement([
        { name: "ServiceID", type: "input", status: true },
        { name: "btnServiceID", type: "input", status: true },
    ]);
    //widget.enableElement([{ name: "btnServiceID", type: "button", status: true }]);
    widget.clearForm();

    widget.reloadGridData("tblTrainingHistory");
    $("#pnlTrainingHistory").hide();
    $("btnAddTraining").show();
    widget.reloadGridData("tblEmployeeAchievement");
    $("#pnlEmployeeAchievement").hide();
    $("#btnAddAchievement").show();
    $("[name='AssignDate']").removeAttr("disabled");
}





function evt_btnAddTraining(evt) {
    if (transactionStatus) {
        $("[name='btnAddTraining']").hide();
        $("#pnlTrainingHistory").slideDown();
    }
}

function evt_btnSaveTraining(evt) {
    if (transactionStatus) {
        //if (transactionStatus && widget.validate("formTraining")) {
        var params = widget.getForms("formTraining");
        params["EmployeeID"] = widget.getValue({ name: "EmployeeID", type: "input" });
        params["TrainingCode"] = widget.getValue({ name: "TrainingCode", type: "select" });
        params["TrainingDate"] = widget.getValue({ name: "TrainingDate", type: "input" });
        params["PreTest"] = widget.getValue({ name: "PreTest", type: "input" });
        params["PreTestAlt"] = widget.getValue({ name: "PreTestAlt", type: "input" });
        params["PostTest"] = widget.getValue({ name: "PostTest", type: "input" });
        params["PostTestAlt"] = widget.getValue({ name: "PostTestAlt", type: "input" });
        

        var url = "ab.api/Training/Save";
        widget.post(url, params, function (result) {
            if (result.status) {
                $("[name='btnAddTraining']").show();
                $("#pnlTrainingHistory").slideUp();
                widget.clearForm("formTraining");
                reloadTrainingData();
            }
            else {
                alert(result.message);
            }
        });
    }
}

function evt_btnCancelTraining(evt) {
    $("[name='btnAddTraining']").show();
    $("#pnlTrainingHistory").slideUp();
    widget.clearForm("formTraining");
    widget.clearValidation("formTraining");
}

function reloadTrainingData() {
    widget.reloadGridData("tblTrainingHistory");
}

function saveTraining(evt) {
    if (transactionStatus && widget.validate("formTraining")) {
        var params = widget.getForms("formTraining");
        params["EmployeeID"] = widget.getValue({ name: "EmployeeID", type: "input" });
        var url = "ab.api/Training/Save";
        widget.post(url, params, function (result) {
            if (result.status) {
                $("[name='btnAddTraining']").show();
                $("#pnlTrainingHistory").slideUp();
                widget.clearForm("formTraining");
                reloadTrainingData();
            }
            else {
                alert(result.message);
            }
        });
    }
}

function deleteTraining(evt, data) {
    if (confirm("Do you want to delete this data ?")) {
        var params = {
            EmployeeID: data.EmployeeID,
            TrainingDate: widget.toDateFormat(widget.cleanJsonDate(data.TrainingDate))
        };
        var url = "ab.api/Training/Delete";
        widget.post(url, params, function (result) {
            reloadTrainingData();
            clearForm();
        });
    }
}



function validateJoinDateEmployeeAchievement(hideAddAchievement) {
    var url = "ab.api/Employee/CheckJoinDate";
    var params = { EmployeeID: $("#EmployeeID").val() };

    widget.post(url, params, function (result) {
        var panelAchievementVisibility = $("#pnlEmployeeAchievement").css("display");

        if (panelAchievementVisibility != "none") {
            $("[name='btnAddAchievement']").hide();
        }
        else {
            $("[name='btnAddAchievement']").show();
        }

        widget.changeSwitchValue({
            panel: "pnlEmployeeAchievement",
            name: "IsJoinDate",
            value: false
        });

        if (widget.isNullOrEmpty(result.data) == false) {
            if (widget.isNullOrEmpty(result.data.HasJoinDateInAchievement) == false) {
                var panelJoinDate = $("#IsJoinDateN").parent().parent().parent();
                if (result.data.HasJoinDateInAchievement == true) {
                    panelJoinDate.hide();
                }
                else {
                    panelJoinDate.show();
                }
            }
            else {
                $("#IsJoinDateN").parent("div").show();
            }

            variables["DeleteAchievementStatus"] == false;


            $("#IsJoinDateN, #IsJoinDateY").on("change", function (event) {
                $("[name='AssignDate']").attr("disabled", true);

                if ($(this).attr("value") == "true") {
                    if (widget.isNullOrEmpty(result.data.JoinDate) == false) {
                        $("[name='AssignDate']").val(widget.toDateFormat(widget.cleanJsonDate(result.data.JoinDate)));
                    }
                }
                else {
                    $("[name='AssignDate']").val('');
                    $("[name='AssignDate']").removeAttr("disabled");
                }
            });
        }
    });
}

function evt_btnAddAchievement(evt) {
    if (transactionStatus) {
        $("#pnlEmployeeAchievement").slideDown();
        widget.showInputElement({
            name: "GradeAchievement",
            type: "select",
            visible: true
        });
        validateJoinDateEmployeeAchievement(true);
    }
}

function evt_btnSaveAchievement(evt) {
    if (transactionStatus && widget.validate("formAchievement")) {
        var params = widget.getForms("formAchievement");
        params["EmployeeID"] = widget.getValue({ name: "EmployeeID", type: "input" });
        var url = "ab.api/Achievement/Save";
        widget.post(url, params, function (result) {
            if (result.status) {
                $("[name='btnAddAchievement']").show();
                $("#pnlEmployeeAchievement").slideUp();
                widget.clearForm("formAchievement");
                updateEmployeeForm(params["EmployeeID"]);
                reloadAchievementData();
                $("[name='AssignDate']").removeAttr("disabled");
                $("[name='AssignDate']").val('');
            }
            else {
                alert(result.message);
            }
        });
    }
}

function evt_btnCancelAchievement(evt) {
    $("[name='btnAddAchievement']").show();
    $("#pnlEmployeeAchievement").slideUp();
    widget.clearForm("formAchievement");
    widget.clearValidation("formAchievement");
    $("[name='AssignDate']").val('');
    $("[name='AssignDate']").removeAttr("disabled");
}

function reloadAchievementData() {
    widget.reloadGridData("tblEmployeeAchievement");
}

function deleteAchievement(evt, data) {
    if (confirm("Do you want to delete this data ?")) {
        variables["DeleteAchievementStatus"] = false;
        var params = $.extend(data, {
            AssignDate: widget.toDateFormat(widget.cleanJsonDate(data.AssignDate))
        });

        var url = "ab.api/Achievement/Delete";
        widget.post(url, params, function (result) {
            if (result.status) {
                clearForm();
                reloadAchievementData();
                updateEmployeeForm(data.EmployeeID);
                variables["DeleteAchievementStatus"] == true;
                validateJoinDateEmployeeAchievement(true);
            }
            else {
                alert(result.message);
            }
            $("[name='btnAddAchievement']").show();
        });
    }
}

function updateEmployeeForm(employeeID) {
    var url = "ab.api/Achievement/UpdatedAchievement";
    var params = {
        EmployeeID: employeeID
    };

    widget.post(url, params, function (result) {
        if (result.status) {
            result.data["JoinDate"] = widget.toDateFormat(widget.cleanJsonDate(result.data.JoinDate));
            widget.populate(result.data);
        }
    });
}

