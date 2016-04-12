var widget;
var transactionStatus = false;
var variables = {};

$(document).ready(function () {
    var options = {
        title: "Personal Information",
        xtype: "panels",
        toolbars: [
            { name: "btnClear", text: "New", icon: "icon-file" },
            { name: "btnSave", text: "Save", icon: "icon-save", cls: "" },
            { name: "btnBrowse", text: "Browse", icon: "icon-search" },
            { name: "btnChangeEmployeeID", text: "Change NIK", icon: "icon-exchange" },
        ],
        panels: [
            {
                title: "Contact Information",
                name: "contactInformation",
                items: [
                    {
                        text: "Employee",
                        type: "controls",
                        items: [
                            { name: "EmployeeID", cls: "span2", placeHolder: "NIK", type: "popup", required: false },
                            { name: "EmployeeName", cls: "span6 ignore-uppercase", placeHolder: "Name", required: false }
                        ]
                    },
                    { name: "Email", text: "Email", cls: "span4 ignore-uppercase", maxlength: 50 },
                    { name: "FaxNo", text: "Fax No", cls: "span4", maxlength: 50 },
                    { name: "Handphone1", text: "Handphone 1", cls: "span4", required: true, dataInputType: "phone-number" },
                    { name: "Telephone1", text: "Telephone 1", cls: "span4", maxlength: 25, dataInputType: "phone-number" },
                    { name: "Handphone2", text: "Handphone 2", cls: "span4", maxlength: 25, dataInputType: "phone-number" },
                    { name: "Telephone2", text: "Telephone 2", cls: "span4", maxlength: 25, dataInputType: "phone-number" },
                    { name: "Handphone3", text: "Handphone 3", cls: "span4 full", maxlength: 25, dataInputType: "phone-number" },
                    { name: "Handphone4", text: "PIN BB / WA", cls: "span4", maxlength: 25, dataInputType: "" },
                    { name: "OfficeLocation", text: "Office Location", type: "textarea", required: false, maxlength: 250 },
                    {
                        text: "User ID",
                        type: "controls",
                        items: [
                            {
                                name: "RelatedUser", text: "User ID", cls: "span2", placeHolder: "User ID", maxlength: 25, type: "popup-lookup",
                                readonly: true,
                                maxlength: 20,
                                source: "ab.api/Grid/Users",
                                title: "User List",
                                columns: [
                                    { mData: "RelatedUser", sTitle: "User ID", sWidth: "200px" },
                                    { mData: "FullName", sTitle: "Full Name", sWidth: "500px" }
                                ]
                            },
                            { name: "FullName", text: "Full Name", cls: "span6", placeHolder: "Full Name", readonly: true }
                        ]
                    },
                ]
            },
            {
                title: "Job Information",
                items: [
                    { name: "JoinBranch", text: "Join Branch", cls: "span4", type: "select", url: "", dataSource: "ab.api/Combo/Branch", required: true },
                    { name: "JoinDate", text: "Join Date", cls: "span4", type: "datepicker", required: true },
                    { name: "Department", text: "Department", cls: "span4", type: "select", url: "", dataSource: "ab.api/Combo/Departments", required: true },
                    { name: "Position", text: "Position", cls: "span4", type: "select", required: true },
                    { name: "Grade", text: "Grade", cls: "span4", type: "select" },
                    { name: "TeamLeader", text: "Direct Leader", cls: "span4", type: "select" },
                    { name: "Rank", text: "Rank", cls: "span4", type: "select", dataSource: "ab.api/Combo/Ranks", required: false },
                    { name: "AdditionalJob1", text: "Additional Job 1", cls: "span4", type: "select" },
                    { name: "AdditionalJob2", text: "Additional Job 2", cls: "span4", type: "select" },
                    { name: "PersonnelStatus", text: "Personnel Status", cls: "span4", type: "select", dataSource: "ab.api/Combo/PersonnelStatus", required: true },
                    { name: "ResignDate", text: "Resign Date", cls: "span4", type: "datepicker" },
                    { name: "ResignCategory", text: "Resign Category", cls: "span4", type: "select" },
                    { name: "ResignDescription", text: "Resign Description", type: "textarea", maxlength: 500 },
                ]
            },
            {
                title: "Detail Information",
                items: [
                    { name: "IdentityNo", text: "Identity No", cls: "span4", required: true, maxlength: 80 },
                    { name: "NPWPNo", text: "NPWP No", cls: "span4", required: true, maxlength: 50 },
                    { name: "NPWPDate", text: "NPWP Date", cls: "span4", type: "datepicker", required: false },
                    { name: "BirthDate", text: "Birth Date", cls: "span4", type: "datepicker", required: true },
                    { name: "BirthPlace", text: "Birth Place", cls: "span4", required: true },
                    { name: "Address1", text: "Address", required: true, maxlength: 150 },
                    { name: "Address2", text: "", maxlength: 150 },
                    { name: "Address3", text: "", maxlength: 150 },
                    { name: "Address4", text: "", maxlength: 150 },
                    { name: "Province", text: "Province", cls: "span4", type: "select", dataSource: "ab.api/Combo/Provinces", required: false },
                    { name: "District", text: "District", cls: "span4", type: "select", required: false },
                    { name: "SubDistrict", text: "Sub Distric", cls: "span4", type: "select", required: false },
                    { name: "Village", text: "Village", cls: "span4", type: "select", required: false },
                    { name: "ZipCode", text: "Zip Code", cls: "span4", required: false, maxlength: 10 },
                    { name: "Gender", text: "Gender", cls: "span4", type: "select", dataSource: "ab.api/Combo/Genders", required: true },
                    { name: "Religion", text: "Religion", cls: "span4", type: "select", dataSource: "ab.api/Combo/Religions", required: true },
                ]
            },
            {
                title: "Other Information",
                items: [
                    { name: "DrivingLicense1", text: "Driving License 1", cls: "span4", maxlength: 50 },
                    { name: "DrivingLicense2", text: "Driving License 2", cls: "span4", maxlength: 50 },
                    { name: "MaritalStatus", text: "Marital Status", cls: "span4", type: "select", dataSource: "ab.api/Combo", required: true },
                    { name: "MaritalStatusCode", text: "Marital Status Details", cls: "span4", type: "select" },
                    { name: "Height", text: "Height", cls: "span4", maxlength: 5 },
                    { name: "Weight", text: "Weight", cls: "span4", maxlength: 5 },
                    { name: "UniformSize", text: "Uniform Size", cls: "span4", type: "select", dataSource: "ab.api/Combo/UniformSizes", required: true },
                    { name: "UniformSizeAlt", text: "Uniform Size Alt", cls: "span4", type: "select", dataSource: "ab.api/Combo/UniformSizeAlts", required: true },
                    { name: "ShoesSize", text: "Shoes Size", cls: "span4", type: "select", dataSource: "ab.api/Combo/ShoesSize" },
                    { name: "BloodCode", text: "Blood Code", cls: "span4", type: "select", dataSource: "ab.api/Combo/BloodTypes" },
                    { name: "FormalEducation", text: "Formal Education", cls: "span4", type: "select", dataSource: "ab.api/Combo/FormalEducations", required: true },
                    { name: "OtherInformation", text: "Other Information", type: "textarea", maxlength: 500 },
                ]
            },
            {
                xtype: "tabs",
                name: "tabPersonnel",
                items: [
                    { name: "tabPhotos", text: "Photos" },
                    { name: "tabSubordinate", text: "Subordinates" },
                    { name: "tabEmployeeMutation", text: "Mutation" },
                    { name: "tabEmployeeAchievement", text: "Employee Achievement" },
                    { name: "tabTrainingHistory", text: "Training" },
                    { name: "tabVehicle", text: "Vehicle Ownership" },
                    { name: "tabWorkingExperience", text: "Experience" },
                    { name: "tabHistoryEducation", text: "Education" },
                ]
            },
            {
                title: "Employee",
                cls: "tabPersonnel tabPhotos",
                items: [
                    {
                        name: "SelfPhoto", type: "image", cls: "span4", events: [
                            {
                                eventType: "click",
                                event: function (evt) {
                                    evt_SelfPhotoClick(evt);
                                }
                            }
                        ],
                        size: [125, 150],
                        margins: {
                            left: -70
                        },
                        src: SimDms.baseUrl + "assets/img/employee/person.png"
                    },
                    {
                        name: "IdentityCardPhoto", type: "image", cls: "span2 margin-bottom-15", events: [
                            {
                                eventType: "click",
                                event: function (evt) {
                                    evt_IdentityCardPhotoClick(evt);
                                }
                            }
                        ],
                        size: [290, 150],
                        src: SimDms.baseUrl + "assets/img/employee/ktp.png",
                        margins: {
                            left: -445
                        },
                    },
                    {
                        name: "FamilyCardPhoto", type: "image", cls: "span2 margin-bottom-15", events: [
                            {
                                eventType: "click",
                                event: function (evt) {
                                    evt_FamilyCardPhotoClick(evt);
                                }
                            }
                        ],
                        size: [290, 150],
                        src: SimDms.baseUrl + "assets/img/employee/kk.png",
                        margins: {
                            left: -400
                        },
                    },
                ]
            },
            {
                title: "Data",
                cls: "tabPersonnel tabSubordinate",
                xtype: "grid",
                name: "tblSubordinate",
                pnlname: "pnlSubordinate",
                source: "ab.api/grid/Subordinates",
                selectable: true,
                multiselect: false,
                sortings: [[1, "asc"]],
                items: [],
                columns: [
                    { mData: "EmployeeID", sTitle: "NIK", sWidth: "100px" },
                    { mData: "EmployeeName", sTitle: "Name", sWidth: "250px" },
                    { mData: "Department", sTitle: "Department", sWidth: "100px" },
                    { mData: "Position", sTitle: "Position", sWidth: "100px" },
                    { mData: "Rank", sTitle: "Rank", sWidth: "100px" },
                ],
                additionalParams: [
                    { name: "TeamLeaderID", element: "EmployeeID" },
                    { name: "Department", element: "Department", type: "select" },
                ],
            },
            {
                title: "Data",
                cls: "tabPersonnel tabVehicle",
                xtype: "grid",
                name: "tblVehicle",
                tblname: "tblVehicle",
                pnlname: "pnlVehicle",
                source: "ab.api/Vehicle/List",
                buttons: [
                    { name: "btnAddVehicle", text: "Add Vehicle", cls: "", icon: "icon-plus" }
                ],
                formName: "formVehicle",
                selectable: true,
                multiselect: false,
                editButton: true,
                deleteButton: true,
                editAction: function (evt, data) {
                },
                deleteAction: function (evt, data) {
                    deleteVehicle(evt, data);
                },
                sortings: [[1, "asc"]],
                items: [
                    { name: "Type", text: "Type", cls: "span3", type: "text", required: true, maxlength: 20 },
                    {
                        name: "Brand", text: "Brand", cls: "span3", type: "select", required: true,
                        items: [
                            { value: "Daihatsu", text: "Daihatsu", required: true },
                            { value: "Honda", text: "Honda", required: true },
                            { value: "Nissan", text: "Nissan", required: true },
                            { value: "Suzuki", text: "Suzuki", required: true },
                            { value: "Toyota", text: "Toyota", required: true },
                            { value: "Yamaha", text: "Yamaha", required: true },
                            { value: "Others", text: "Others", required: true },
                        ]
                    },
                    { name: "Model", text: "Model", cls: "span3", type: "text", required: true, maxlength: 20 },
                    { name: "PoliceRegNo", text: "Police Reg. No.", cls: "span3", type: "text", required: true, maxlength: 20 },
                    {
                        type: "buttons", items: [
                            { name: "btnSaveVehicle", text: "Save", icon: "icon-save" },
                            { name: "btnCancelVehicle", text: "Cancel", icon: "icon-undo" }
                        ]
                    },
                ],
                columns: [
                    {
                        sTitle: "Action",
                        "mDataProp": "",
                        "sClass": "",
                        "sDefaultContent": "<i class='icon icon-trash'></i>",
                        sWidth: "50px"
                    },
                    { mData: "Type", sTitle: "Type", text: "Type", cls: "", width: 150 },
                    { mData: "Brand", sTitle: "Brand", text: "Brand", cls: "", width: 150 },
                    { mData: "Model", sTitle: "Model", text: "Model", cls: "", width: 150 },
                ],
                additionalParams: [
                    { name: "EmployeeID", element: "EmployeeID" }
                ],
            },
            {
                title: "Data",
                cls: "tabPersonnel tabEmployeeAchievement",
                xtype: "grid",
                name: "tblEmployeeAchievement",
                tblname: "tblEmployeeAchievement",
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
                    editAchievement(evt, data);
                },
                deleteAction: function (evt, data) {
                    deleteAchievement(evt, data);
                },
                sortings: [[1, "asc"]],
                items: [
                    {
                        text: "Department/Position",
                        type: "controls",
                        items: [
                            { type: "select", name: "DepartmentAchievement", cls: "span2", text: "Department", required: true },
                            { type: "select", name: "PositionAchievement", cls: "span2", text: "Position", required: true },
                            { type: "select", name: "GradeAchievement", cls: "span2", text: "Grade", required: false },
                        ]
                    },
                    { text: "Is Join Date", type: "switch", name: "IsJoinDate", cls: "span2 full", text: "is Join Date", float: "left" },
                    {
                        text: "Assign Date",
                        type: "controls",
                        items: [
                            { type: "datepicker", name: "AssignDate", cls: "span2", text: "Assign Date", required: true },
                        ]
                    },
                    { type: "buttons", items: [{ name: "btnSaveAchievement", text: "Save", icon: "icon-save" }, { name: "btnCancelAchievement", text: "cancel", icon: "icon-undo" }] },
                ],
                columns: [
                    {
                        sTitle: "Action",
                        "mDataProp": "",
                        "sClass": "",
                        "sDefaultContent": "<i class='icon icon-trash'></i><i class='icon icon-edit'></i>",
                        sWidth: "80px"
                    },
                    { mData: "EmployeeID", sTitle: "NIK", sWidth: "100px" },
                    { mData: "EmployeeName", sTitle: "Name", sWidth: "180px" },
                    { mData: "DepartmentName", sTitle: "Department", sWidth: "80px" },
                    { mData: "PositionName", sTitle: "Position", sWidth: "250px" },
                    { mData: "GradeName", sTitle: "Grade", sWidth: "100px" },
                    {
                        mData: "AssignDate", sTitle: "Assign Date", sWidth: "120px",
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
            {
                title: "Data",
                cls: "tabPersonnel tabTrainingHistory",
                xtype: "grid",
                name: "tblTrainingHistory",
                tblname: "tblTrainingHistory",
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
                    editTraining(evt, data);
                },
                deleteAction: function (evt, data) {
                    deleteTraining(evt, data);
                },
                sortings: [[1, "asc"]],
                items: [
                    {
                        text: "Training",
                        type: "controls",
                        items: [
                            { name: "TrainingDate", cls: "span2", placeholder: "Training Date", type: "datepicker", readonly: false },
                            { type: "select", name: "TrainingCode", cls: "span4", text: "Training", required: true },
                        ]
                    },
                    {
                        text: "Position",
                        type: "controls",
                        items: [
                            { name: "DepartmentTraining", cls: "span2", placeholder: "Department", readonly: true },
                            { name: "PositionTraining", cls: "span2", placeholder: "Position", readonly: true },
                            { name: "GradeTraining", cls: "span2", placeholder: "Grade", readonly: true },
                        ]
                    },
                    {
                        text: "Pre Test",
                        type: "controls",
                        items: [
                            { name: "PreTest", cls: "span2", placeholder: "Pre Test", readonly: false },
                            { name: "PreTestAlt", cls: "span4", placeholder: "Pre Test Alternative", readonly: false, type: "select" },
                        ]
                    },
                    {
                        text: "Post Test",
                        type: "controls",
                        items: [
                            { name: "PostTest", cls: "span2", placeholder: "Post Test", readonly: false },
                            { name: "PostTestAlt", cls: "span4", placeholder: "Post Test Alternative", readonly: false, type: "select" },
                        ]
                    },
                    { type: "buttons", items: [{ name: "btnSaveTraining", text: "Save", icon: "icon-save" }, { name: "btnCancelTraining", text: "cancel", icon: "icon-undo" }] },
                ],
                columns: [
                    {
                        sTitle: "Action",
                        "mDataProp": "",
                        "sClass": "",
                        "sDefaultContent": "<i class='icon icon-trash'></i><i class='icon icon-edit'></i>",
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
                    { name: "Department", value: "SALES" },
                ],
            },
            {
                title: "Data",
                cls: "tabPersonnel tabEmployeeMutation",
                xtype: "grid",
                name: "tblEmployeeMutation",
                tblname: "tblEmployeeMutation",
                pnlname: "pnlEmployeeMutation",
                source: "ab.api/Mutation/List",
                buttons: [
                    { name: "btnAddMutation", text: "Add Mutation", cls: "", icon: "icon-plus" }
                ],
                formName: "formEmployeeMutation",
                selectable: true,
                multiselect: false,
                editButton: true,
                deleteButton: false,
                editAction: function (evt, data) {
                    editHistoryMutation(evt, data);
                },
                //deleteAction: function (evt, data) {
                //    deleteMutation(evt, data);
                //},
                sortings: [[1, "asc"]],
                items: [
                    { name: "MutationDate", type: "datepicker", text: "Mutation Date", cls: "span3", required: true },
                    { name: "BranchCode", type: "select", text: "Mutation Branch", cls: "span5", required: true },
                    { name: "IsJoinDateMutation", type: "switch", text: "Is Join Date", cls: "span3" },
                    {
                        name: "", type: "buttons", items: [
                            { name: "btnSaveMutation", text: "Save", icon: "icon-save" },
                            { name: "btnCancelMutation", text: "Cancel", icon: "icon-refresh" },
                        ]
                    },
                ],
                columns: [
                    {
                        sTitle: "Action",
                        "mDataProp": "",
                        "sClass": "",
                        //"sDefaultContent": "<i class='icon icon-trash'></i><i class='icon icon-edit'></i>",
                        "sDefaultContent": "<i class='icon icon-edit'></i>",
                        sWidth: "100px"
                    },
                    { mData: "EmployeeID", sTitle: "NIK", sWidth: "200px" },
                    { mData: "MutationDate", sTitle: "Mutation Date", sWidth: "150px" },
                    { mData: "BranchName", sTitle: "Branch Name", sWidth: "400px" },
                    { mData: "MutationInfo", sTitle: "Mutation Info" },
                ],
                additionalParams: [
                    { name: "EmployeeID", element: "EmployeeID" },
                ],
            },
            {
                title: "Data",
                cls: "tabPersonnel tabWorkingExperience",
                xtype: "grid",
                name: "tblWorkingExperience",
                tblname: "tblWorkingExperience",
                pnlname: "pnlWorkingExperience",
                source: "ab.api/WorkingExperience/List",
                buttons: [
                    { name: "btnAddWorkingExperience", text: "Add Working Experience", cls: "", icon: "icon-plus" }
                ],
                formName: "formWorkingExperience",
                selectable: true,
                multiselect: false,
                editButton: true,
                deleteButton: true,
                editAction: function (evt, data) {
                    editWorkingExperience(evt, data);
                },
                deleteAction: function (evt, data) {
                    deleteWorkingExperience(evt, data);
                },
                sortings: [[1, "asc"]],
                items: [
                    { name: "WorkingExperienceNameOfCompany", type: "text", text: "Company Name", cls: "span5 full", required: true },
                    { name: "WorkingExperienceJoinDate", type: "datepicker", text: "Join Date", cls: "span3 full", required: true },
                    { name: "WorkingExperienceResignDate", type: "datepicker", text: "Resign Date", cls: "span3 full" },
                    { name: "WorkingExperienceReasonOfResign", type: "text", text: "Resign Reason", cls: "span8" },
                    { name: "WorkingExperienceLeaderName", type: "text", text: "Leader Name", cls: "span" },
                    { name: "WorkingExperienceLeaderPhone", type: "text", text: "Leader Phone", cls: "span" },
                    { name: "WorkingExperienceLeaderHP", type: "text", text: "Leader HP", cls: "span" },
                    {
                        name: "", type: "buttons", items: [
                            { name: "btnSaveWorkingExperience", text: "Save", icon: "icon-save" },
                            { name: "btnCancelWorkingExperience", text: "Cancel", icon: "icon-refresh" },
                        ]
                    },
                ],
                columns: [
                    {
                        sTitle: "Action",
                        "mDataProp": "",
                        "sClass": "",
                        "sDefaultContent": "<i class='icon icon-trash'></i><i class='icon icon-edit'></i>",
                        sWidth: "100px"
                    },
                    { mData: "EmployeeID", sTitle: "NIK", sWidth: "65px" },
                    { mData: "NameOfCompany", sTitle: "Company Name", sWidth: "150px" },
                    {
                        mData: "JoinDate", sTitle: "Join Date", sWidth: "120px",
                        mRender: function (data, type, full) {
                            if (widget.isNullOrEmpty(data) == false) {
                                return widget.toDateFormat(widget.cleanJsonDate(data));
                            }

                            return "-";
                        }
                    },
                    {
                        mData: "ResignDate", sTitle: "Resign Date", sWidth: "120px",
                        mRender: function (data, type, full) {
                            if (widget.isNullOrEmpty(data) == false) {
                                return widget.toDateFormat(widget.cleanJsonDate(data));
                            }

                            return "-";
                        }
                    },
                    { mData: "ReasonOfResign", sTitle: "ResignReason" },
                ],
                additionalParams: [
                    { name: "EmployeeID", element: "EmployeeID" },
                ],
            },
            {
                title: "Data",
                cls: "tabPersonnel tabHistoryEducation",
                xtype: "grid",
                name: "tblHistoryEducation",
                tblname: "tblHistoryEducation",
                pnlname: "pnlHistoryEducation",
                source: "ab.api/HistoryEducation/List",
                buttons: [
                    { name: "btnAddHistoryEducation", text: "Add History Education", cls: "", icon: "icon-plus" }
                ],
                formName: "formHistoryEducation",
                selectable: true,
                multiselect: false,
                editButton: true,
                deleteButton: true,
                editAction: function (evt, data) {
                    editHistoryEducation(evt, data);
                },
                deleteAction: function (evt, data) {
                    deleteHistoryEducation(evt, data);
                },
                sortings: [[1, "asc"]],
                items: [
                    { name: "HistoryEducationCollege", type: "text", text: "College", cls: "span5 full", required: true },
                    { name: "HistoryEducationYearBegin", type: "text", text: "Year Begin", maxlength: 4, cls: "span3 full", required: true },
                    { name: "HistoryEducationYearFinish", type: "text", text: "Year Finish", maxlength: 4, cls: "span3 full", required: true },
                    {
                        name: "", type: "buttons", items: [
                            { name: "btnSaveHistoryEducation", text: "Save", icon: "icon-save" },
                            { name: "btnCancelHistoryEducation", text: "Cancel", icon: "icon-refresh" },
                        ]
                    },
                ],
                columns: [
                    {
                        sTitle: "Action",
                        "mDataProp": "",
                        "sClass": "",
                        "sDefaultContent": "<i class='icon icon-trash'></i><i class='icon icon-edit'></i>",
                        sWidth: "100px"
                    },
                    { mData: "College", sTitle: "College" },
                    { mData: "YearBegin", sTitle: "Begin", sWidth: "150px" },
                    { mData: "YearFinish", sTitle: "Finish", sWidth: "150px" },
                ],
                additionalParams: [
                    { name: "EmployeeID", element: "EmployeeID" },
                ],
            },
        ],
    }

    widget = new SimDms.Widget(options);
    var paramEvents = [
        {
            name: "btnChangeEmployeeID",
            type: "button",
            eventType: "click",
            event: function (evt) {
                evt_BtnChangeEmployeeID(evt);
            }
        },
        {
            name: "btnAddHistoryEducation",
            type: "button",
            eventType: "click",
            event: function (evt) {
                evt_btnAddHistoryEducation(evt);
            }
        },
        {
            name: "btnSaveHistoryEducation",
            type: "button",
            eventType: "click",
            event: function (evt) {
                evt_btnSaveHistoryEducation(evt);
            }
        },
        {
            name: "btnCancelHistoryEducation",
            type: "button",
            eventType: "click",
            event: function (evt) {
                evt_btnCancelHistoryEducation(evt);
            }
        },
        {
            name: "btnAddWorkingExperience",
            type: "button",
            eventType: "click",
            event: function (evt) {
                evt_btnAddWorkingExperience(evt);
            }
        },
        {
            name: "btnSaveWorkingExperience",
            type: "button",
            eventType: "click",
            event: function (evt) {
                evt_btnSaveWorkingExperience(evt);
            }
        },
        {
            name: "btnCancelWorkingExperience",
            type: "button",
            eventType: "click",
            event: function (evt) {
                evt_btnCancelWorkingExperience(evt);
            }
        },
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
                        widget.showNotification(result.message);
                    }
                });
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
                    gradeElement.parent().children("label").parent().remove();
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
            name: "PositionAchievement",
            type: "select",
            eventType: "change",
            event: function (evt) {
                var currentVal = widget.getValue({ name: "PositionAchievement", type: "select" });
                var gradeElement = $("[name='GradeAchievement']");
                gradeElement.show();
                if (currentVal == "S") {
                    gradeElement.attr("required", "required");
                    widget.showInputElement({
                        name: "GradeAchievement",
                        type: "select",
                        visible: true,
                        type: "controls"
                    });
                }
                else {
                    widget.showInputElement({
                        name: "GradeAchievement",
                        type: "select",
                        visible: false,
                        type: "controls"
                    });
                    gradeElement.removeAttr("required");
                    gradeElement.removeClass("error");
                    gradeElement.parent().children("label").remove();
                }
            }
        },
        {
            name: "btnAddTraining",
            type: "button",
            eventType: "click",
            event: function (evt) {
                if (transactionStatus) {
                    $("[name='btnAddTraining']").hide();
                    $("#pnlTrainingHistory").slideDown();
                }
            }
        },
        {
            name: "btnCancelTraining",
            type: "button",
            eventType: "click",
            event: function (evt) {
                $("[name='btnAddTraining']").show();
                $("#pnlTrainingHistory").slideUp();
                widget.clearForm("formTraining");
                widget.clearValidation("formTraining");
            }
        },
        {
            name: "btnAddVehicle",
            type: "button",
            eventType: "click",
            event: function (evt) {
                if (transactionStatus) {
                    $("[name='btnAddVehicle']").hide();
                    $("#pnlVehicle").slideDown();
                }
            }
        },
        {
            name: "btnSaveVehicle",
            type: "button",
            eventType: "click",
            event: function (evt) {
                saveVehicle(evt);
            }
        },
        {
            name: "btnCancelVehicle",
            type: "button",
            eventType: "click",
            event: function (evt) {
                $("[name='btnAddVehicle']").show();
                $("#pnlVehicle").slideUp();
                widget.clearForm("formVehicle");
                widget.clearValidation("formVehicle");
            }
        },
        {
            name: "EmployeeID",
            type: "input",
            eventType: "blur",
            event: function (evt) {
                findEmployee();
            }
        },
        {
            name: "EmployeeID",
            type: "input",
            eventType: "keyup",
            event: function (evt) {
                if (evt.keyCode == '13' || evt.wich == 13) {
                    findEmployee();
                }
            }
        },
        {
            name: "btnBrowse",
            type: "button",
            eventType: "click",
            event: function (evt) {
                initLookupEmployee(widget);
            }
        },
        {
            name: "btnEmployeeID",
            type: "button",
            eventType: "click",
            event: function (evt) {
                initLookupEmployee(widget);
            }
        },
        {
            name: "btnProcess",
            type: "button",
            eventType: "click",
            event: function (evt) {
                widget.showToolbars(["btnSave", "btnCancel"]);
            }
        },
        {
            name: "btnClear",
            type: "button",
            eventType: "click",
            event: function (evt) {
                clearForm();
            }
        },
        {
            name: "btnSave",
            type: "button",
            eventType: "click",
            event: function (evt) {
                saveEmployee(evt);
            }
        },
        {
            name: "btnSaveNew",
            type: "button",
            eventType: "click",
            event: function (evt) {
                saveEmployee(evt, true);
            }
        },
        {
            name: "btnCancel",
            type: "button",
            eventType: "click",
            event: function (evt) {
                widget.showToolbars(["btnBrowse", "btnProcess", "btnClear"]);
            }
        },
        {
            name: "Village",
            type: "select",
            eventType: "change",
            event: function (evt) {
                var params = {
                    provinceCode: widget.getValue({ name: "Province", type: "select" }),
                    cityCode: widget.getValue({ name: "District", type: "select" }),
                    districtCode: widget.getValue({ name: "SubDistrict", type: "select" }),
                    villageCode: widget.getValue({ name: "Village", type: "select" })
                };

                widget.post("ab.api/Combo/ZipCode", params, function (result) {
                    widget.getObject("ZipCode").val(result);
                });
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
            name: "btnAddMutation",
            type: "button",
            eventType: "click",
            event: function (evt) {
                evt.preventDefault();
                evt_btnAddMutation(evt);
                return false;
            }
        },
        {
            name: "btnSaveMutation",
            type: "button",
            eventType: "click",
            event: function (evt) {
                evt_btnSaveMutation(evt);
            }
        },
        {
            name: "btnCancelMutation",
            type: "button",
            eventType: "click",
            event: function (evt) {
                evt_btnCancelMutation(evt);
            }
        },
        {
            name: "BranchCode",
            type: "select",
            eventType: "change",
            event: function (evt) {
                evt_MutationBranch(evt);
            }
        },
        {
            name: "MutationDate",
            type: "text",
            eventType: "change",
            event: function (evt) {
                evt_MutationDate(evt);
            }
        }
    ];
    widget.setEventList(paramEvents);

    var paramsSelect = [
        {
            name: "BranchCode", url: "ab.api/Combo/Branch",
        },
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
            name: "GradeAchievement", url: "ab.api/Combo/Grades",
            cascade: {
                name: "PositionAchievement",
                additionalParams: [],
                conditions: [
                    { name: "PositionAchievement", condition: "=='S'" }
                ]
            }
        },
        { name: "Department", url: "ab.api/Combo/Departments" },
        {
            name: "Position", url: "ab.api/Combo/Positions",
            cascade: {
                name: "Department"
            }
        },
        {
            name: "TeamLeader",
            url: "ab.api/Combo/TeamLeaders",
            cascade: {
                name: "Position",
                additionalParams: [
                    { name: "Department", source: "Department", type: "select" },
                    { name: "Position", source: "Position", type: "select" }
                ]
            }
        },
        {
            name: "JoinBranch", url: "ab.api/Combo/Branch"
        },
        {
            name: "Rank", url: "ab.api/Combo/Ranks"
        },
        {
            name: "Grade", url: "ab.api/Combo/Grades",
            cascade: {
                name: "Position",
                conditions: [
                    { name: "Position", condition: "=='S'" }
                ]
            }
        },
        {
            name: "AdditionalJob1", url: "ab.api/Combo/Positions",
            cascade: {
                name: "Department"
            }
        },
        {
            name: "AdditionalJob2", url: "ab.api/Combo/Positions",
            cascade: {
                name: "Department"
            }
        },
        {
            name: "PersonnelStatus", url: "ab.api/Combo/PersonnelStatus"
        },
        {
            name: "Province", url: "ab.api/Combo/Provinces"
        },
        {
            name: "District", url: "ab.api/Combo/Cities",
            cascade: {
                name: "Province"
            }
        },
        {
            name: "SubDistrict", url: "ab.api/Combo/Districts",
            cascade: {
                name: "District"
            }
        },
        {
            name: "Village", url: "ab.api/Combo/Villages",
            cascade: {
                name: "SubDistrict"
            }
        },
        {
            name: "Gender", url: "ab.api/Combo/Genders"
        },
        {
            name: "MaritalStatus", url: "ab.api/Combo/MaritalStatus"
        },
        {
            name: "MaritalStatusCode", url: "ab.api/Combo/MaritalStatusDetails",
            cascade: {
                name: "MaritalStatus",
                conditions: [
                    { name: "MaritalStatus", condition: "=='K'" }
                ]
            }
        },
        {
            name: "UniformSize", url: "ab.api/Combo/UniformSizes",
            params: { Param1: "param1", Param2: "param2", Param3: "param3" }
        },
        {
            name: "ResignCategory", url: "ab.api/Combo/ResignCategories"
        },
        {
            name: "Religion", url: "ab.api/Combo/Religions"
        },
        {
            name: "UniformSizeAlt", url: "ab.api/Combo/UniformSizeAlts"
        },
        {
            name: "ShoesSize", url: "ab.api/Combo/ShoesSize"
        },
        {
            name: "BloodCode", url: "ab.api/Combo/BloodTypes"
        },
        {
            name: "FormalEducation", url: "ab.api/Combo/FormalEducations"
        },
        {
            name: "PreTestAlt", url: "ab.api/Combo/TrainingPreScoresAlternative"
        },
        {
            name: "PostTestAlt", url: "ab.api/Combo/TrainingPostScoresAlternative"
        },
        {
            name: "PositionTraining", url: "ab.api/Training/PositionList",
            cascade: {
                name: "DepartmentTraining",
                additionalParams: [
                    { name: "employeeID", source: "EmployeeID", type: "input" }
                ]
            }
        },
        {
            name: "GradeTraining", url: "ab.api/Combo/Grades",
            cascade: {
                name: "PositionTraining",
                additionalParams: []
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

    widget.render(renderCallback);

    widget.lookup.onDblClick(function (e, data, name) {
        widget.populate(data);

        hideInputTab();
        enableEmployeeID(true);
        widget.lookup.hide();
        transactionStatus = true;

        if (name == "Employee" || name == "gridEmployeeID") {
            showJoinBranch(false);
        }
    });

    widget.onTabsChanged(function (obj, parent, name) { });
        
});

//CALLBACK AREA

function renderCallback() {
    evt_PersonnelStatus();
    showResignDetails(false);
    validateResignDate();
    evt_MaritalStatusChange();
    evt_IsJoinDateMutation();
    evt_ImgPhotos();

    $.each($("button"), function (key, val) {
        $(this).on("click", function (evt) {
            evt.preventDefault();
        });
    });   
}

function populateCallback(data, widget) {
    if (widget.isNullOrEmpty(data.ResignDate) == false) {
        showResignDetails(true);
    }
    else {
        showResignDetails(false);
    }

    if (widget.isNullOrEmpty(data.Position) == false) {
        widget.cascade({
            source: "Department",
            target: "Position",
            sourceValue: data.Department,
            targetValue: data.Position,
            url: "ab.api/Combo/Positions",
            additionalParams: [],
            enabled: true
        });
        widget.cascade({
            source: "Department",
            target: "AdditionalJob1",
            sourceValue: data.Department,
            targetValue: data.AdditionalJob1,
            url: "ab.api/Combo/Positions",
            additionalParams: [],
            enabled: true
        });
        widget.cascade({
            source: "Department",
            target: "AdditionalJob2",
            sourceValue: data.Department,
            targetValue: data.AdditionalJob2,
            url: "ab.api/Combo/Positions",
            additionalParams: [],
            enabled: true
        });
    }

    widget.cascade({
        source: "Position",
        target: "TeamLeader",
        sourceValue: data.Position,
        targetValue: data.TeamLeader,
        url: "ab.api/Combo/TeamLeaders",
        additionalParams: [
            { name: "Department", element: "Department", type: "select", value: data.Department },
            { name: "Position", element: "Position", type: "select", value: data.Position }
        ]
    });

    if (widget.isNullOrEmpty(data.AdditionalJob1) == false) {
        widget.cascade({
            source: "Department",
            target: "AdditionalJob1",
            sourceValue: data.Department,
            targetValue: data.AdditionalJob1,
            url: "ab.api/Combo/Positions",
            additionalParams: []
        });
    }

    if (widget.isNullOrEmpty(data.AdditionalJob2) == false) {
        widget.cascade({
            source: "Department",
            target: "AdditionalJob2",
            sourceValue: data.Department,
            targetValue: data.AdditionalJob2,
            url: "ab.api/Combo/Positions",
            additionalParams: []
        });
    }

    if (widget.isNullOrEmpty(data.Grade) == false) {
        widget.cascade({
            source: "Position",
            target: "Grade",
            sourceValue: data.Position,
            targetValue: data.Grade,
            url: "ab.api/Combo/Grades",
            additionalParams: [],
            enabled: false
        });
    }

    if (widget.isNullOrEmpty(data.District) == false) {
        widget.cascade({
            source: "Province",
            target: "District",
            sourceValue: data.Province,
            targetValue: data.District,
            url: "ab.api/Combo/Cities",
            additionalParams: []
        });
    }

    if (widget.isNullOrEmpty(data.SubDistrict) == false) {
        widget.cascade({
            source: "District",
            target: "SubDistrict",
            sourceValue: data.District,
            targetValue: data.SubDistrict,
            url: "ab.api/Combo/Districts",
            additionalParams: []
        });
    }

    if (widget.isNullOrEmpty(data.Village) == false) {
        widget.cascade({
            source: "SubDistrict",
            target: "Village",
            sourceValue: data.SubDistrict,
            targetValue: data.Village,
            url: "ab.api/Combo/Villages",
            additionalParams: []
        });
    }

    if (widget.isNullOrEmpty(data.MaritalStatusCode) == false) {
        widget.cascade({
            source: "MaritalStatus",
            target: "MaritalStatusCode",
            sourceValue: data.MaritalStatus,
            targetValue: data.MaritalStatusCode,
            url: "ab.api/Combo/MaritalStatusDetails",
            additionalParams: []
        });
    }

    widget.selects({
        name: "DepartmentTraining",
        source: "ab.api/Training/DepartmentList",
        additionalParams: [
            { name: "EmployeeID", element: "EmployeeID", type: "input", value: data.EmployeeID }
        ]
    });
    clearPhoto();
    if (widget.isNullOrEmpty(data.SelfPhoto) == false) {
        $("#SelfPhoto").attr("src", SimDms.baseUrl + "ab.api/Employee/Photo/?fileID=" + data.SelfPhoto);
    }
    if (widget.isNullOrEmpty(data.IdentityCardPhoto) == false) {
        $("#IdentityCardPhoto").attr("src", SimDms.baseUrl + "ab.api/Employee/Photo/?fileID=" + data.IdentityCardPhoto);
    }
    if (widget.isNullOrEmpty(data.FamilyCardPhoto) == false) {
        $("#FamilyCardPhoto").attr("src", SimDms.baseUrl + "ab.api/Employee/Photo/?fileID=" + data.FamilyCardPhoto);
    }

    widget.clearValidation();
}

//INIT AREA

function initLookupEmployee(widget)
{
    var lookup = widget.klookup({
        name: "Employee",
        title: "Employee List",
        url: "ab.api/kgrid/Employees",
        serverBinding: true,
        pageSize: 12,
        filters: [
            {
                text: "Nik",
                type: "controls",
                items: [
                    { name: "fltNik", text: "NIK", cls: "span2" },
                    { name: "fltEmplName", text: "Name", cls: "span6" },
                ]
            },
            {
                text: "Position",
                type: "controls",
                items: [
                    { name: "fltDepartment", text: "Department", cls: "span2" },
                    { name: "fltPosition", text: "Position", cls: "span4" },
                    {
                        name: "fltPersonnelStatus", type: "select", text: "Status", cls: "span2", items: [
                            { value: "", text: "Pilih Semua" },
                            { value: "1", text: "Aktif" },
                            { value: "2", text: "Non Aktif" },
                            { value: "3", text: "Keluar" },
                            { value: "4", text: "Pensiun" },
                        ]
                    },
                ]
            },
        ],
        columns: [
            { field: "EmployeeID", title: "NIK", width: 150 },
            { field: "EmployeeName", title: "Name", width: 250 },
            { field: "DepartmentName", title: "Department", width: 150 },
            { field: "PositionName", title: "Position", width: 250 },
            { field: "JoinDate", title: "Join Date", width: 100, template: "#= (JoinDate == undefined) ? '' : moment(JoinDate).format('DD MMM YYYY') #" },
            { field: "ResignDate", title: "Resign Date", width: 100, template: "#= (ResignDate == undefined) ? '' : moment(ResignDate).format('DD MMM YYYY') #" },
            { field: "Status", title: "Status", width: 100 },
        ],
    });

    lookup.dblClick(function (data) {
        widget.populate(data, populateCallback(data, widget));
        widget.clearValidation();
        checkPosition(data.EmployeeID);

        setTimeout(function () {
            reloadVehicleData();
            reloadTrainingData();
            reloadSubordinateData();
            reloadAchievementData();
            reloadTrainingData();
            reloadMutationData();
            reloadWorkingExperienceData();
            reloadHistoryEducationData();
        }, 1000);

        hideInputTab();
        enableEmployeeID(true);
        widget.lookup.hide();
        transactionStatus = true;
        showJoinBranch(false);
    });
}

//EMPLOYEE AREA

function enableEmployeeID(isEnabled) {
    if (isEnabled) {
        widget.getObject("EmployeeID").removeAttr("readonly");
    }
    else {
        widget.getObject("EmployeeID").attr("readonly", "readonly");
    }

}

function enablePosition(enabled) {
    var departmentElement = widget.getObject("Department", "select");
    var positionElement = widget.getObject("Position", "select");
    var gradeElement = widget.getObject("Grade", "select");

    widget.enableElement([
        { name: "Department", type: "select", status: enabled },
        { name: "Position", type: "select", status: enabled },
        { name: "Grade", type: "select", status: enabled },
    ]);
}

function checkPosition(employeeID) {
    var url = "ab.api/Achievement/IsHasHistories";
    var params = {
        employeeID: employeeID
    };

    widget.post(url, params, function (result) {
        if (result.status == true) {
            enablePosition(false);
        }
        else {
            enablePosition(true);
        }
    });
}

function reloadSubordinateData() {
    widget.reloadGridData("tblSubordinate");
}

function reloadTrainingData(employeeID) {
    widget.reloadGridData("tblTrainingHistory");
}

function reloadVehicleData(employeeID) {
    reloadWorkingExperienceData();
    reloadHistoryEducationData();
    widget.reloadGridData("tblVehicle");
}

function hideInputTab() {
    $("#pnlTrainingHistory").hide();
    $("#pnlVehicle").hide();
    $("#pnlEmployeeMutation").hide();
    $("#pnlEmployeeAchievement").hide();

    $("#btnAddTraining").show();
    $("#btnAddVehicle").show();
    $("#btnAddMutation").show();
    $("#btnAddAchievement").show();
}

function clearForm() {
    widget.clearForm();
    widget.clearValidation();
    enableEmployeeID(true);
    enablePosition(true);
    hideInputTab();
    reloadSubordinateData();
    reloadVehicleData();
    reloadTrainingData();
    reloadAchievementData();
    reloadTrainingData();
    reloadMutationData();
    transactionStatus = false;
    hideInputTab();
    showJoinBranch(true);
    showResignDetails(false);

    widget.clearForm("formVehicle");
    widget.clearValidation("formVehicle");
    widget.clearForm("formTraining");
    widget.clearValidation("formTraining");
    clearPhoto();
}

function findEmployee() {
    var url = "ab.api/Employee/Find";
    var params = {
        employeeID: widget.getValue({ name: "EmployeeID", type: "text" })
    };

    widget.post(url, params, function (result) {
        if (widget.isNullOrEmpty(result.data) == false) {
            widget.populate(result.data, populateCallback(result.data, widget));
            showJoinBranch(false);

            widget.clearValidation();
            checkPosition(result.data.EmployeeID);

            setTimeout(function () {
                reloadWorkingExperienceData();
                reloadHistoryEducationData();
                reloadVehicleData(result.data.EmployeeID);
                reloadTrainingData();
                reloadSubordinateData();
                reloadAchievementData();
                reloadTrainingData();
                reloadMutationData();
            }, 1000);

            hideInputTab();
            enableEmployeeID(true);
            widget.lookup.hide();
            transactionStatus = true;

            if (name == "Employee" || name == "gridEmployeeID") {
                showJoinBranch(false);
            }
        }
        else {
            var employeeID = $("[name='EmployeeID']").val();
            clearForm();
            $("[name='EmployeeID']").val(employeeID);
        }
    });
}

function saveEmployee(evt, isSaveNew) {
    if (widget.validate() == true && onBeforeSaving() == true) {
        if (onBeforeSaving() == true) {
            var pStatus = parseInt($("#PersonnelStatus").val());
            if (pStatus != 1) {
                var params = {
                    employeeID: $("#EmployeeID").val(),
                }
                widget.post("ab.api/Employee/CheckKdp", widget.getForms(), function (result) {
                    if (!result.success) {
                        widget.alert(result.msg);
                    }
                    else {
                        SaveEmployeeData();
                    }
                });
            }
            else {
                SaveEmployeeData();
            }
        }
    }
}

function SaveEmployeeData() {
    widget.post("ab.api/Employee/Save", widget.getForms(), function (result) {
        if (result.status == true || result.status == "true") {
            enableEmployeeID(true);
            //enablePosition(false);
            transactionStatus = true;
            showJoinBranch(false);
            findEmployee();
            reloadSubordinateData();
            reloadMutationData();
            reloadAchievementData();
            reloadTrainingData();
            reloadVehicleData();
            reloadWorkingExperienceData();
            reloadHistoryEducationData();

            if (isSaveNew) {
                clearForm();
            }
        }
        widget.showNotification(result.message);

    });
}

function showJoinBranch(isShowed) {
    if (isShowed) {
        $("[name='JoinDate']").attr("required", true);
        widget.showInputElement({
            name: "JoinBranch",
            type: "select",
            visible: true
        });
    }
    else {
        $("[name='JoinDate']").removeAttr("required");
        widget.showInputElement({
            name: "JoinBranch",
            type: "select",
            visible: false
        });
    }
}

function evt_ResignDate() {
    var dateValue = $("[name='ResignDate']").val();

    if (widget.isNullOrEmpty(dateValue) == false) {
        showResignDetails(true);
    }
    else {
        showResignDetails(false);
    }
}

function evt_PersonnelStatus() {
    var personnelStatusElement = $("[name='PersonnelStatus']");
    var persStatus = personnelStatusElement.val();

    personnelStatusElement.on("change", function (evt) {
        if (persStatus != "3" && persStatus != "4") {
            $("[name='ResignDate']").val("");
            showResignDetails(false);
        }
        else {
            showResignDetails(true);
        }
    });
}

function evt_MaritalStatusChange() {
    $("[name='MaritalStatus']").on("change", function () {

        var cascadeOption = {
            source: "MaritalStatus",
            target: "MaritalStatusCode",
            url: "ab.api/Combo/MaritalStatusDetails",
            additionalParams: [],
        };

        if ($("[name='MaritalStatus']").val() != 'K') {
            cascadeOption["selectedIndex"] = "1";
        }

        widget.cascade(cascadeOption);
    });
}

function evt_BtnChangeEmployeeID() {
    if (transactionStatus) {
        var html = "<div class='crop-image'>";
        html += "<div class='content' style='background-color: inherit'>";
        html += "<div style='text-align: left; padding-top: 15px; padding-left: 11px;'>";
        html += "<label style='display: inline; width: 200px;'>Current NIK</label>";
        html += "<div style='display: inline;'><input style='margin-top: -23px; margin-left: 130px; width: 300px;' type='text' readonly='true' name='CurrentEmployeeID' id='CurrentEmployeeID' /></div>";
        html += "</div>";
        html += "<div style='text-align: left; padding-top: 0px; padding-left: 11px;'>";
        html += "<label style='display: inline; width: 200px;'>New NIK</label>";
        html += "<div style='display: inline;'><input style='margin-top: -23px; margin-left: 130px; width: 300px;' type='text' name='NewEmployeeID' id='NewEmployeeID' /></div>";
        html += "</div>";
        html += "</div>";
        html += "<div class='footer' style='text-align: right;'>";
        html += "<button id='btnChangeNIK' class='small'>Save</button>";
        html += "<button id='btnCancelChangeNIK' class='small' style='margin-right: 11px;'>Cancel</button>";
        html += "</div>";
        html += "</div>";

        var paramSize = {
            status: true,
            height: 170,
            width: 500
        };

        widget.showCustomPopup(html, paramSize, fixingPopupLayout);

        setTimeout(function () {
            $("[name='CurrentEmployeeID']").val($("[name='EmployeeID']").val());

            var btnChangeNIK = $("#btnChangeNIK");
            var btnCancelChangeNIK = $("#btnCancelChangeNIK");

            btnChangeNIK.off();
            btnCancelChangeNIK.off();

            btnChangeNIK.on("click", function (evt) {
                var url = "ab.api/Employee/ChangeEmployeeID";
                var params = {
                    CurrentEmployeeID: $("[name='CurrentEmployeeID']").val(),
                    NewEmployeeID: $("[name='NewEmployeeID']").val()
                };

                widget.post(url, params, function (result) {
                    if (result.status) {
                        btnCancelChangeNIK.click();
                        $("[name='EmployeeID']").val(params.NewEmployeeID);
                        findEmployee();
                    }

                    widget.showNotification(result.message);
                });
            });
            btnCancelChangeNIK.on("click", function (evt) {
                $(".overlay").click();
            });
        }, 500);
    }
}

function createAdditionalFormUpload(objectName) {
    var html = "";
    html += "<form style='display: none;' method='POST' name='form" + (objectName || "") + "' id='form" + (objectName || "") + "'  enctype='multipart/form-data'>";
    html += "<input type='file' name='file' id='file" + (objectName || "") + "' />";
    html += "</form>";

    $("body").append(html);
}

function isElementExist(elementName, elementID) {
    var isExist = false;

    var selectorName = "[name='" + (elementName || "") + "']";
    var selectorID = "#" + (elementID || "");

    if ($(selectorName).length > 0 || $(selectorID).length > 0) {
        isExist = true;
    }

    return isExist;
}

function showResignDetails(resignDetailsStatus) {
    var resignCategory = $("[name='ResignCategory']");
    var resignDescription = $("[name='ResignDescription']");

    if (resignDetailsStatus != true) {
        resignCategory.val("");
        resignDescription.val("");

        resignCategory.removeAttr("required");
        resignDescription.removeAttr("required");
    }
    else {
        resignCategory.attr("required", true);
        resignDescription.attr("required", true);
    }

    widget.showItem([
        {
            name: "ResignCategory",
            isVisible: resignDetailsStatus
        },
        {
            name: "ResignDescription",
            isVisible: resignDetailsStatus
        }
    ]);
}

function validateResignDate() {
    setInterval(evt_ResignDate, 1000);
}

//VEHICLE AREA
function saveVehicle() {
    if (transactionStatus && widget.validate("formVehicle")) {
        var params = $("#pnlVehicle").serializeObject();
        params["EmployeeID"] = $("[name='EmployeeID']").val();
        var url = "ab.api/Vehicle/Save";

        widget.post(url, params, function (result) {
            if (result.status == true) {
                $("#pnlVehicle").slideUp();
                $("#btnAddVehicle").show();
                widget.clearForm("formVehicle");
                reloadVehicleData(params.EmployeeID);
            }
        });
    }
}

function deleteVehicle(evt, data) {
    if (confirm("Do you want to delete this data ?")) {
        var url = "ab.api/Vehicle/Delete";
        widget.post(url, data, function (result) {
            if (result.status) {
                reloadVehicleData();
            }
            else {
                widget.showNotification(result.message);
            }
        });
    }
}

//PHOTOS AREA                  

function evt_SelfPhotoClick() {
    if (transactionStatus) {
        generatePopupCrop("SelfPhoto", 400, 600, [3, 4]);
        variables["KindOfPhoto"] = "SelfPhoto";
    }
}

function evt_IdentityCardPhotoClick() {
    if (transactionStatus) {
        generatePopupCrop("IdentityCardPhoto", 400, 600, [4, 3]);
        variables["KindOfPhoto"] = "IdentityCardPhoto";
    }
}

function evt_FamilyCardPhotoClick() {
    if (transactionStatus) {
        generatePopupCrop("FamilyCardPhoto", 400, 600, [4, 3]);
        variables["KindOfPhoto"] = "FamilyCardPhoto";
    }
}

function generatePopupCrop(name, height, width, ratio) {
    var paramSize = {
        status: true,
        height: height,
        width: width
    };

    var html = "<div class='crop-image' style='height: 380px; background-color: #FFFFFF; padding-top: 10px;'>";
    html += "<div class='content' style='text-align: center; margin-left: 10px; margin-right: 10px;'>";

    if (name == "SelfPhoto") {
        html += "<img data-tooltip class='has-tip' title='Click here to change photo!' id='" + name + "Image' style='height: 320px; width: 240px; cursor: pointer;' src='" + $("#" + name).attr("src") + "' />";
    }
    else {
        html += "<img data-tooltip class='has-tip' title='Click here to change photo!' id='" + name + "Image' style='height: 320px; width: 420px; cursor: pointer;' src='" + $("#" + name).attr("src") + "' />";
    }

    html += "<input type='file' class='hide' id='" + name + "File' />";
    html += "<input type='hidden' id='" + name + "FileName' />";
    html += "<div class='information hide' id='" + name + "LabelInfo'></div>";
    html += "<input type='hidden' id='x1' />";
    html += "<input type='hidden' id='x2' />";
    html += "<input type='hidden' id='y1' />";
    html += "<input type='hidden' id='y2' />";
    html += "</div>";
    html += "<div class='footer' style='border-top: 1px solid gray; padding-top: 5px; margin-left: 10px; margin-right: 10px;'>";
    html += "<button id='btn" + name + "Browse' class='small hide'>Browse</button>";
    html += "<button id='btn" + name + "Save' class='small' style='margin-left: 6px;'>Save</button>";
    html += "<button id='btn" + name + "Cancel' class='small' style='margin-left: 3px;'>Cancel</button>";
    html += "</div>";
    html += "</div>";

    widget.showCustomPopup(html, paramSize, fixingPopupLayout);
    initializePopupEvent(name, ratio);
}

function fixingPopupLayout() {
    var cropWrapper = $(".popup-frame.outer .popup-frame.inner .crop-image");
    var cropContent = cropWrapper.children(".content");
    var cropFooter = cropWrapper.children(".footer");
    var imgCrop = $("#employeePhotoCrop");
    var labelCrop = $("#" + name + "LabelInfo");

    var additionalSeparatorSpace = 3;
    var wrapperHeight = cropWrapper.height();
    var footerHeight = 40;
    var contentHeight = wrapperHeight - footerHeight - additionalSeparatorSpace;
    var labelCropTop = (wrapperHeight / 2) - 17;

    cropContent.css({
        "height": contentHeight + "px"
    });

    cropFooter.css({
        "text-align": "center", 
        "height": footerHeight + "px",
        "margin-top": additionalSeparatorSpace + "px",
        "padding-top": "-1px"
    });
}

function initializePopupEvent(name, ratio) {
    $("#btn" + name + "Save").on("click", function (evt) {
        evt_buttonSaveCropImage(name);
    });
    $("#btn" + name + "Browse").on("click", function (evt) {
        $("#" + name + "File").click();
    });
    $("#btn" + name + "Cancel").on("click", function (evt) {
        $(".overlay").click();
    });
    $(".popup-frame.outer .popup-frame.inner .crop-image .content img#" + name + "Image").on("click", function (evt) {
        $("#" + name + "File").click();
    });
    $("#" + name + "File").on("change", function (evt) {
        var formData = new FormData();
        var fileElement = $("#" + name + "File")[0].files[0];
        var labelInfo = $("#" + name + "LabelInfo");
        if (this.files[0].size > 3000000) {
            sdms.info("Maximum size Photo 3 MB!");
        }
        formData.append("EmployeeID", $("[name='EmployeeID']").val());
        formData.append("PhotoType", name == "SelfPhoto" ? "1" : "2");
        formData.append("berkas", fileElement);
        
        widget.uploadFile({
            url: "ab.api/Employee/UploadPhoto",
            data: formData,
            events: {
                progress: function (progress) {
                    var cropWrapper = $(".popup-frame.outer .popup-frame.inner .crop-image");
                    var cropWrapperHeight = (cropWrapper.height() / 2) - 25;
                    labelInfo.show();
                    labelInfo.css({
                        "margin-top": cropWrapperHeight + "px"
                    });
                    labelInfo.html("Uploading file : " + progress + "% &nbsp; <i class='icon icon-spin icon-refresh'></i>");
                },
                success: function (result) {
                    if (result.status) {
                        if (result.status) {
                            $("#" + name + "Image").attr("src", "ab.api/Employee/TempPhoto/?fileID=" + result.data.fileID);
                            variables.imageID = result.data.fileID;
                            initializeCropImage(name, ratio);
                        }
                        else {
                            widget.showNotification(result.message);
                        }
                    }
                },
                error: function (a, b, c) {
                    widget.showNotification("Sorry, your photo cannot be uploaded.\nTry again later!");
                },
                complete: function (result) { }
            }
        });
    });
}

function initializeCropImage(name, ratio) {
    var frame = $(".crop-image .content");
    var image = frame.children("img#" + name + "Image");

    var frameHeight = frame.height();
    var frameWidth = frame.width();
    var imageHeight = image.height();
    var imageWidth = image.width();
    var newImageWidth = 0;
    var newImageHeight = 0;

    var tempImage = new Image();
    tempImage.onload = function () {
        imageHeight = this.height;
        imageWidth = this.width;
        variables.originalImageHeight = this.height;
        variables.originalImageWidth = this.width;

        if (imageHeight > imageWidth) {
            newImageHeight = frameHeight;
            newImageWidth = imageWidth * newImageHeight / imageHeight;
        }

        if (imageHeight < imageWidth) {
            newImageWidth = frameWidth;
            newImageHeight = newImageWidth * imageHeight / imageWidth;
        }

        imageHeight = newImageHeight;
        imageWidth = newImageWidth;

        if (newImageHeight > frameHeight) {
            newImageHeight = frameHeight;
            newImageWidth = imageWidth * newImageHeight / imageHeight;
        }

        if (newImageWidth > frameWidth) {
            newImageWidth = frameWidth;
            newImageHeight = newImageWidth * imageHeight / imageWidth;
        }

        variables.imageHeight = newImageHeight;
        variables.imageWidth = newImageWidth;

        image.css({
            "height": newImageHeight + "px",
            "width": newImageWidth + "px"
        });

        var x1 = 0;
        var x2 = 0;
        var y1 = 0;
        var y2 = 0;

        x1 = (frameWidth - newImageWidth) / 2;
        x2 = x1 + newImageWidth;
        y1 = 0;
        y2 = frameHeight;


        var selectedArea = calculateInitializeCropArea(ratio, newImageWidth, newImageHeight);

        image.imgAreaSelect({
            handles: true,
            aspectRatio: (ratio[0] + ":" + ratio[1]),
            onSelectEnd: function (img, selection) {
                $("#x1").val(selection.x1);
                $("#x2").val(selection.x2);
                $("#y1").val(selection.y1);
                $("#y2").val(selection.y2);
                variables.x1 = selection.x1;
                variables.x2 = selection.x2;
                variables.y1 = selection.y1;
                variables.y2 = selection.y2;
            },
            x1: selectedArea.x1,
            x2: selectedArea.x2,
            y1: selectedArea.y1,
            y2: selectedArea.y2
        });

        $(".overlay").click(function (evt) {
            image.imgAreaSelect({
                remove: true
            });
        });

    };
    tempImage.src = image.attr("src");
}

function calculateInitializeCropArea(ratio, imageWidth, imageHeight) {
    /*
        ratio = [width, height]
    */

    var margin = 10;
    var area = {
        x1: 0,
        x2: 200,
        y1: 0,
        y2: 0
    };

    if (ratio[0] < ratio[1]) {
        area.y1 = margin;
        area.y2 = imageHeight - margin;
        var rectWidth = ratio[0] * (area.y2 - area.y1) / ratio[1];
        area.x1 = (imageWidth - rectWidth) / 2;
        area.x2 = area.x1 + rectWidth;
    }

    if (ratio[0] > ratio[1]) {
        area.x1 = margin;
        area.x2 = imageWidth - margin;
        var rectHeight = ratio[1] * (area.x2 - area.x1) / ratio[0];
        area.y1 = (imageHeight - rectHeight) / 2;
        area.y2 = area.y1 + rectHeight
    }


    variables.x1 = area.x1;
    variables.x2 = area.x2;
    variables.y1 = area.y1;
    variables.y2 = area.y2;

    return area;
}

function initializeCropElementEvent(name) {
    evt_buttonSaveCropImage(name);
}

function evt_buttonSaveCropImage(name) {
    var url = "ab.api/Employee/SaveImage";
    var params = {
        EmployeeID: $("[name='EmployeeID']").val(),
        ImageID: variables.imageID,
        OriginalImageHeight: variables.originalImageHeight,
        OriginalImageWidth: variables.originalImageWidth,
        ImageHeight: variables.imageHeight,
        ImageWidth: variables.imageWidth,
        x1: variables.x1,
        x2: variables.x2,
        y1: variables.y1,
        y2: variables.y2,
        KindOfPhoto: variables["KindOfPhoto"]
    };
    widget.post(url, params, function (result) {
        if (result.status) {
            var imageUrl = "ab.api/Employee/Photo?fileID=" + result.data.ImageID;
            $("#" + name).attr("src", imageUrl);
            $(".overlay").click();
        }
    });
}

function evt_ImgPhotos() {
    $("img#SelfPhoto").error(function () {
        $("#SelfPhoto").attr("src", (SimDms.defaultEmployeePhoto || ""));
    });

    $("img#IdentityCardPhoto").error(function () {
        $("#IdentityCardPhoto").attr("src", (SimDms.defaultIdentityCardPhoto || ""));
    });

    $("img#FamilyCardPhoto").error(function () {
        $("#FamilyCardPhoto").attr("src", (SimDms.defaultFamilyCardPhoto || ""));
    });
}

function clearPhoto() {
    $("#SelfPhoto").attr("src", (SimDms.defaultEmployeePhoto || ""));
    $("#IdentityCardPhoto").attr("src", (SimDms.defaultIdentityCardPhoto || ""));
    $("#FamilyCardPhoto").attr("src", (SimDms.defaultFamilyCardPhoto || ""));
}




//ACHIEVEMENT AREA

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

function validateJoinDateEmployeeAchievement(hideAddAchievement) {
    var url = "ab.api/Employee/CheckJoinDate";
    var params = { EmployeeID: $("[name='EmployeeID']").val() };

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

            $("#IsJoinDateN, #IsJoinDateY").on("click", function (event) {
                var _this = this;
                setTimeout(function () {
                    $("[name='AssignDate']").attr("disabled", true);
                    var isJoinDate = $(_this).attr("value");

                    if ($(_this).attr("value") == "true") {
                        if (widget.isNullOrEmpty(result.data.JoinDate) == false) {
                            $("[name='AssignDate']").val(widget.toDateFormat(widget.cleanJsonDate(result.data.JoinDate)));
                        }
                    }
                    else {
                        $("[name='AssignDate']").val('');
                        $("[name='AssignDate']").removeAttr("disabled");
                    }
                }, 250);
            });
        }
    });
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
                //updateEmployeeForm(params["EmployeeID"]);
                reloadAchievementData();
                $("[name='AssignDate']").removeAttr("disabled");
                $("[name='AssignDate']").val('');
                enablePosition(false);
                updatePositionDetails();
            }
            else {
                widget.showNotification(result.message);
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
                reloadAchievementData();
                variables["DeleteAchievementStatus"] == true;
                validateJoinDateEmployeeAchievement(true);
            }
            else {
                widget.showNotification(result.message);
            }
            //updateEmployeeForm(data.EmployeeID);
            updatePositionDetails();
            checkPosition(data.EmployeeID);
            $("[name='btnAddAchievement']").show();
        });
    }
}

function editAchievement(evt, data) {
    if (transactionStatus) {
        $("#pnlEmployeeAchievement").slideDown();
        $("#btnAddAchievement").hide();
        widget.clearForm("formEmployeeAchievement");
        widget.clearValidation("formEmployeeAchievement");

        widget.changeSwitchValue({ name: "IsJoinDate", value: data.IsJoinDate || false });
        var panelJoinDate = $("#IsJoinDateN").parent().parent().parent();
        panelJoinDate.show();

        $("[name='DepartmentAchievement']").val(data.Department);
        widget.post("ab.api/Combo/Positions", { id: data.Department }, function (result) {
            var positionHtml = "<option value=''>" + (SimDms.selectOneText || "") + "</option>";
            $.each(result, function (key, val) {
                positionHtml += "<option value='" + val.value + "'>" + val.text + "</option>";
            });

            $("[name='PositionAchievement']").html(positionHtml);
            $("[name='PositionAchievement']").val(data.Position);

            if (data.Department == "SALES" && data.Position == "S") {
                $("[name='GradeAchievement']").show();
                $("[name='GradeAchievement']").val(data.Grade);
            }
            else {
                $("[name='GradeAchievement']").hide();
            }

            var assignDateObj = $("[name=AssignDate]");
            var assignDate = widget.toDateFormat(widget.cleanJsonDate(data.AssignDate));
            assignDateObj.val(assignDate);
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

function updatePositionDetails() {
    var url = "ab.api/Achievement/UpdatedAchievement";
    var data = {
        EmployeeID: $("[name='EmployeeID']").val()
    };

    widget.post(url, data, function (result) {
        if (result.status) {
            if (widget.isNullOrEmpty(result.data) == false) {
                $("[name='Department']").val(result.data.Department);
                $("[name='Position']").val(result.data.Position);
                $("[name='Grade']").val(result.data.Grade);

                try {
                    widget.cascade({
                        source: "Department",
                        target: "Position",
                        sourceValue: result.data.Department,
                        targetValue: result.data.Position,
                        url: "ab.api/Combo/Positions",
                        additionalParams: [],
                        enabled: false
                    });

                    widget.cascade({
                        source: "Position",
                        target: "Grade",
                        sourceValue: result.data.Position,
                        targetValue: result.data.Grade,
                        url: "ab.api/Combo/Grades",
                        additionalParams: [],
                        enabled: false
                    });
                }
                catch (ex) { }

            }
        }
    });
}






//TRAINING  AREA

function evt_btnAddTraining(evt) {
    if (transactionStatus) {
        $("[name='btnAddTraining']").hide();
        $("#pnlTrainingHistory").slideDown();
    }
}

function evt_btnSaveTraining(evt) {
    if (transactionStatus) {
        if (transactionStatus && widget.validate("formTraining")) {
            var params = widget.getForms("formTraining");
            params["EmployeeID"] = widget.getValue({ name: "EmployeeID", type: "input" });
            params["TrainingCode"] = widget.getValue({ name: "TrainingCode", type: "select" });
            params["TrainingDate"] = widget.getValue({ name: "TrainingDate", type: "input" });
            params["PreTest"] = widget.getValue({ name: "PreTest", type: "input" });
            params["PreTestAlt"] = widget.getValue({ name: "PreTestAlt", type: "select" });
            params["PostTest"] = widget.getValue({ name: "PostTest", type: "input" });
            params["PostTestAlt"] = widget.getValue({ name: "PostTestAlt", type: "select" });

            console.log(params);
            var url = "ab.api/Training/Save";
            widget.post(url, params, function (result) {
                if (result.status) {
                    $("[name='btnAddTraining']").show();
                    $("#pnlTrainingHistory").slideUp();
                    widget.clearForm("formTraining");
                    reloadTrainingData();
                }
                else {
                    widget.showNotification(result.message);
                }
            });
        }
    }
}

function evt_btnCancelTraining(evt) {
    $("[name='btnAddTraining']").show();
    $("#pnlTrainingHistory").slideUp();
    widget.clearForm("formTraining");
    widget.clearValidation("formTraining");
}

function reloadTrainingData() {
    setTimeout(function () {
        widget.reloadGridData("tblTrainingHistory");
    }, 1000);
}

function saveTraining(evt) {
    if (transactionStatus && widget.validate("formTraining")) {
        var params = widget.getForms("formTraining");
        params["EmployeeID"] = widget.getValue({ name: "EmployeeID", type: "input" });

        var additionalParams = {
            PreTestAlt: $("[name='PreTestAlt']").val(),
            PostTestAlt: $("[name='PostTestAlt']").val()
        };

        var url = "ab.api/Training/Save";
        widget.post(url, params, function (result) {
            if (result.status) {
                $("[name='btnAddTraining']").show();
                $("#pnlTrainingHistory").slideUp();
                //widget.clearForm("formTraining");
                reloadTrainingData();
            }
            else {
                widget.showNotification(result.message);
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
            widget.reloadGridData("tblTrainingHistory");
        });
    }
}

function editTraining(evt, data) {
    if (transactionStatus) {
        $("#btnAddTraining").click();

        var url = "ab.api/Training/TrainingList";
        var params = {
            EmployeeID: data.EmployeeID,
            TrainingDate: widget.toDateFormat(widget.cleanJsonDate(data.TrainingDate))
        };

        widget.post(url, params, function (result) {
            if (widget.isNullOrEmpty(result.status)) {
                widget.setItems({
                    name: "TrainingCode",
                    type: "select",
                    data: result
                });

                $("[name='TrainingCode']").val(data.TrainingCode || "");
                $("[name='TrainingDate']").val(widget.toDateFormat(widget.cleanJsonDate(data.TrainingDate)));
                $("[name='PreTest']").val(data.PreTest || "");
                $("[name='PreTestAlt']").val(data.PreTestAlt || "");
                $("[name='PostTest']").val(data.PostTest || "");
                $("[name='PostTestAlt']").val(data.PostTestAlt || "");

                url = "ab.api/Employee/GetDetailsEmployeePosition";
                params = {
                    EmployeeID: $("[name='EmployeeID']").val(),
                    AssignDate: params.TrainingDate
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
                widget.showNotification(result.message);
            }
        });
    }
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
                widget.showNotification(result.message);
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
        });
    }
}





//WORKING EXPERIENCE
function reloadWorkingExperienceData() {
    widget.reloadGridData("tblWorkingExperience");
}

function evt_btnAddWorkingExperience(evt) {
    if (transactionStatus) {
        $("#pnlWorkingExperience").slideDown();
        $("#btnAddWorkingExperience").hide();
        widget.clearForm("formWorkingExperience");
        widget.clearValidation("formWorkingExperience");
        enableWorkingExperienceAllInput(true);
    }
}

function evt_btnSaveWorkingExperience(evt) {
    var validation = widget.validate("formWorkingExperience");

    if (validation) {
        var url = "ab.api/WorkingExperience/Save";
        var data = widget.getForms("formWorkingExperience");
        data.EmployeeID = $("[name='EmployeeID']").val();

        widget.post(url, data, function (result) {
            if (result.status) {
                $("#btnCancelWorkingExperience").click();
                reloadWorkingExperienceData();
            }
            widget.showNotification(result.message);
        });
    }
}

function evt_btnCancelWorkingExperience(evt) {
    $("#pnlWorkingExperience").slideUp();
    $("#btnAddWorkingExperience").show();
    widget.clearForm("formWorkingExperience");
}

function editWorkingExperience(evt, data) {
    $("#btnAddWorkingExperience").click();

    enableWorkingExperienceAllInput(false);
    $("[name='WorkingExperienceNameOfCompany']").val(data.NameOfCompany || "");
    $("[name='WorkingExperienceJoinDate']").val(widget.toDateFormat(widget.cleanJsonDate(data.JoinDate)));
    $("[name='WorkingExperienceResignDate']").val(widget.toDateFormat(widget.cleanJsonDate(data.ResignDate)));
    $("[name='WorkingExperienceReasonOfResign']").val(data.ReasonOfResign);
    $("[name='WorkingExperienceLeaderName']").val(data.LeaderName);
    $("[name='WorkingExperienceLeaderPhone']").val(data.LeaderPhone);
    $("[name='WorkingExperienceLeaderHP']").val(data.LeaderHP);

    $("#pnlWorkingExperience").slideDown();
}

function deleteWorkingExperience(evt, data) {
    var confirmation = confirm("Do you want to delete this data?");

    if (widget.isNullOrEmpty(data.JoinDate) == false) {
        data.JoinDate = widget.toDateFormat(widget.cleanJsonDate(data.JoinDate));
    }

    if (widget.isNullOrEmpty(data.ResignDate) == false) {
        data.ResignDate = widget.toDateFormat(widget.cleanJsonDate(data.ResignDate));
    }

    if (confirmation) {
        var url = "ab.api/WorkingExperience/Delete";
        widget.post(url, data, function (result) {
            if (result.status) {
                reloadWorkingExperienceData();
            }

            widget.showNotification(result.message);
        });
    }
}

function enableWorkingExperienceAllInput(state) {
    if (state) {
        $("[name='WorkingExperienceNameOfCompany']").removeAttr("disabled");
        $("[name='WorkingExperienceJoinDate']").removeAttr("disabled");
    }
    else {
        $("[name='WorkingExperienceNameOfCompany']").attr("disabled", true);
        $("[name='WorkingExperienceJoinDate']").attr("disabled", true);
    }
}





//HISTORY EDUCATION

function reloadHistoryEducationData() {
    widget.reloadGridData("tblHistoryEducation");
}

function evt_btnAddHistoryEducation(evt) {
    widget.clearForm("formHistoryEducation");
    widget.clearValidation("formHistoryEducation");
    $("#btnAddHistoryEducation").hide();
    $("#pnlHistoryEducation").slideDown();
    enableHistoryEducationAllInput(true);
}

function evt_btnSaveHistoryEducation(evt) {
    var validation = widget.validate("formHistoryEducation");

    if (validation) {
        var data = widget.getForms("formHistoryEducation");
        var url = "ab.api/HistoryEducation/Save";

        data.EmployeeID = $("[name='EmployeeID']").val();
        widget.post(url, data, function (result) {
            if (result.status) {
                $("#btnCancelHistoryEducation").click();
                reloadHistoryEducationData();
            }

            widget.showNotification(result.message);
        });
    }
}

function evt_btnCancelHistoryEducation(evt) {
    $("#pnlHistoryEducation").slideUp();
    $("#btnAddHistoryEducation").show();
    widget.clearForm("formHistoryEducation");
    widget.clearValidation("formHistoryEducation");
}

function editHistoryEducation(evt, data) {
    $("#btnAddHistoryEducation").click();
    enableHistoryEducationAllInput(false);

    $("[name='HistoryEducationCollege']").val(data.College);
    $("[name='HistoryEducationYearBegin']").val(data.YearBegin);
    $("[name='HistoryEducationYearFinish']").val(data.YearFinish);
}

function deleteHistoryEducation(evt, data) {
    var confirmation = confirm("Do you want to delete this data?");

    if (confirmation) {
        var url = "ab.api/HistoryEducation/Delete";

        widget.post(url, data, function (result) {
            if (result.status) {
                reloadHistoryEducationData();
            }

            widget.showNotification(result.message);
        });
    }
}

function enableHistoryEducationAllInput(state) {
    if (state) {
        $("[name='HistoryEducationCollege']").removeAttr("disabled");
    }
    else {
        $("[name='HistoryEducationCollege']").attr("disabled", true);
    }
}





//MUTATION AREA      

function reloadMutationData() {
    widget.reloadGridData("tblEmployeeMutation");
}

function evt_btnAddMutation(evt) {
    if (transactionStatus) {
        widget.clearForm("formEmployeeMutation");
        widget.clearValidation("formEmployeeMutation");
        showEditableEmployeeMutationPanel(true);
    }
}

function evt_btnSaveMutation(evt) {
    evt.preventDefault();
    if (transactionStatus) {
        var validation = widget.validate("formEmployeeMutation");
        if (validation) {
            var url = "ab.api/Mutation/CheckKDPCount";
            var data = $.extend(widget.getForms("formEmployeeMutation"), {
                EmployeeID: $("[name='EmployeeID']").val()
            });
            widget.post(url, data, function (result) {
                if (result.message == '') {
                    url = "ab.api/Mutation/Save";
                    widget.post(url, data, function (result) {
                        if (result.status) {
                            $("#btnAddMutation").show();
                            $("#pnlEmployeeMutation").slideUp();
                        }

                        reloadMutationData();
                        widget.showNotification(result.message);
                    });
                } else {
                    widget.showNotification(result.message);
                }
            });


            
        }
    }
}

function evt_btnCancelMutation(evt) {
    $("#pnlEmployeeMutation").slideUp();
    $("#btnAddMutation").show();
    widget.clearForm("formEmployeeMutation");
    widget.clearValidation("formEmployeeMutation");
}

function evt_IsJoinDateMutation() {
    $("#IsJoinDateMutationY, #IsJoinDateMutationN").on("change", function (evt) {
        var _this = $(this);
        setTimeout(function () {
            if (_this.val() == true || _this.val() == "true") {
                getJoinDateMutation();
            }
            else {
                $("[name='MutationDate']").removeAttr("disabled");
                $("[name='MutationDate']").val("");
            }
        }, SimDms.switchChangeDelay);
    });
}

function deleteMutation(evt, data) {
    var url = "ab.api/Mutation/CheckKDPCount";
    widget.post(url, data, function (result) {
        if (result.message == '') {
            var deleteConfirmation = confirm("Do you want to delete this data?");
            if (deleteConfirmation) {
                var url = "ab.api/Mutation/Delete";

                widget.post(url, data, function (result) {
                    if (result.success) {
                        reloadMutationData();
                        $("#btnAddMutation").show();
                        $("#pnlEmployeeMutation").slideUp();
                    }
                    else {
                        widget.showNotification("Sorry, cannot delete mutation data.");
                    }
                });
            }
        } else {
            widget.showNotification(result.message);
        }
    });

    
}

function editHistoryMutation(evt, data) {
    var url = "ab.api/Mutation/CheckKDPCount";
    widget.post(url, data, function (result) {
        if (result.message == '') {
            if (transactionStatus) {
                showEditableEmployeeMutationPanel(false, data, true);
            }
        } else {
            widget.showNotification(result.message);
        }
    });
}

function getJoinDateMutation() {
    var params = {
        EmployeeID: $("[name='EmployeeID']").val()
    };
    var url = "ab.api/Mutation/GetJoinDetailsByEmployeeID";

    widget.post(url, params, function (result) {
        if (result.status) {
            //$("[name='MutationDate']").attr("disabled", true);
            $("[name='MutationDate']").val(result.data);
        }
    });
}

function checkMutationBranch() {

}

function evt_MutationDate(evt) {
    var url = "ab.api/Mutation/CheckMutationDate";
    var params = $.extend(widget.getForms("formEmployeeMutation"), {
        EmployeeID: $("[name='EmployeeID']").val()
    });

    widget.post(url, params, function (result) {
        if (result.success == false) {
            widget.showNotification(result.message);
        }
    });
}

function evt_MutationBranch(evt) {
    var url = "sv.api/Mutation/CheckMutationBranch";
    var params = $.extend(widget.getForms("formEmployeeMutation"), {
        EmployeeID: $("[name='EmployeeID']").val()
    });

    widget.post(url, params, function (result) {
        if (!result.success) {
            widget.showNotification(result.message);
        }
    });
}

function showEditableEmployeeMutationPanel(isAddNewData, data, isEdit) {
    if (widget.isNullOrEmpty(data)) {
        $("#pnlEmployeeMutation").slideDown(isJoinDateExist(isAddNewData, data));
    }
    else {
        $("#pnlEmployeeMutation").slideDown();
        $("[name='BranchCode']").val(data.BranchCode);
        $("[name='MutationDate']").val(data.MutationDate.toString().replace(" ", "-"));

        var panelIsJoinDateMutation = $("[name='IsJoinDateMutation']").parent().parent().parent();
        panelIsJoinDateMutation.show();
        //console.log(panelIsJoinDateMutation);

        if (data.MutationInfo == "-") {
            widget.changeSwitchValue({
                name: "IsJoinDateMutation",
                value: false
            });
        }
        else {
            widget.changeSwitchValue({
                name: "IsJoinDateMutation",
                value: false
            });
        }

        //console.log(data);
    }
    $("#btnAddMutation").hide();

    var elementStatus = true;

    if (isEdit) {
        elementStatus = false;
    }

    //widget.enableElement([
    //    {
    //        type: "datepicker",
    //        name: "MutationDate",
    //        status: elementStatus
    //    },
        //{
        //    type: "switch",
        //    name: "IsJoinDateMutation",
        //    visible: false
        //}
    //]);

    widget.showInputElement({
        type: "switch",
        name: "IsJoinDateMutation",
        visible: elementStatus
    });

    //var mutationSwitch = $("[name='IsJoinDateMutation']");
    //mutationSwitch.off();
    //mutationSwitch.on("click", function (evt) {
    //    var url = "ab.api/Mutation/GetJoinDetailsByEmployeeID";
    //    var params = {
    //        EmployeeID: $("[name='EmployeeID']").val()
    //    };

    //    widget.post("");
    //});
}

function isJoinDateExist(isAddNewData, data) {
    var url = "ab.api/Mutation/IsJoinDateExist";
    var params = {
        EmployeeID: $("[name='EmployeeID']").val(),
    };

    widget.post(url, params, function (result) {
        var visibility = true;

        if (result.status==true) {
            visibility = false;
        }
        else if (result.status == false) {
            visibility = true;
        }

        widget.showInputElement({
            name: "IsJoinDateMutation",
            type: "switch",
            visible: visibility
        });

        widget.changeSwitchValue({
            name: "IsJoinDateMutation",
            value: false
        });

        if (isAddNewData == false) {
            widget.populate(data);
            //$("[name='MutationDate']").attr("disabled", true);
        }
        else {
            $("[name='MutationDate']").removeAttr("disabled");
        }

        widget.changeSwitchValue({
            name: "IsJoinDateMutation",
            value: result.status
        });

        var panelIsJoinDateMutation = $("[name='IsJoinDateMutation']").parent().parent().parent();

        setTimeout(function () {
            if (result.status == false) {
                panelIsJoinDateMutation.show();
            }
            else {
                panelIsJoinDateMutation.hide();
                widget.changeSwitchValue({
                    name: "IsJoinDateMutation",
                    value: false
                });
            }
        }, SimDms.defaultTimeout);
    });
}






//VALIDATION

function onBeforeSaving() {
    var validationStatus = false;
    if (validatePhoneNumber() == true) {
        validationStatus = true;
    }

    return validationStatus;
}

function validatePhoneNumber() {
    var validationStatus = true;
    var strRegex = "[0-9]{10}";
    var regex = new RegExp(strRegex);
    var elements = $("[data-input-type='phone-number']");
    var isValid = false;

    $.each(elements || [], function (key, val) {
        var element = $(this);
        var phoneNumber = element.val();
        var elementLabelText = element.parent("div").parent("div").children("label").text();
        if (widget.isNullOrEmpty(phoneNumber) == false && validationStatus == true) {
            validationStatus = regex.test(phoneNumber);

            if (validationStatus == false) {
                var notificationMessage = "Only number should be entered for \"" + $.trim((elementLabelText || "").replace("*", "")) + "\" and must be 10 char minimum of length.";
                widget.showNotification(notificationMessage);
            }
        }
    });

    return validationStatus;
}


