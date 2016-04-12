$(document).ready(function () {
    var widget;
    var transactionStatus;
    var variables = {};
    variables["DeleteAchievementStatus"] == false;

    var options = {
        title: "Service Information",
        xtype: "panels",
        toolbars: [
            { name: "btnSaveServiceID", text: "Save", icon: "icon-save" }
        ],
        panels: [
            {
                title: "Contact Information",
                items: [
                    {
                        text: "Employee",
                        type: "controls",
                        items: [
                            { name: "EmployeeID", cls: "span2", placeHolder: "NIK", type: "popup", readonly: true },
                            { name: "EmployeeName", cls: "span6 ignore-uppercase", placeHolder: "Name", required: true, readonly: true }
                        ]
                    },
                    { name: "ServiceID", text: "Service ID", placeholder: "Service ID", cls: "span4" },
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
        ],
    }

    transactionStatus = false;
    widget = new SimDms.Widget(options);
    widget.setEventList([{ name: "btnSaveServiceID", type: "button", eventType: "click", event: saveServiceID }]);
    widget.render(function () {
        $("#btnEmployeeID").on("click", function () {
            var lookup = widget.klookup({
                name: "Employee",
                title: "Employee List",
                url: "ab.api/kgrid/Employees",
                params: { Department: "SERVICE", Status: "Aktif" },
                serverBinding: true,
                columns: [
                    { field: "EmployeeID", title: "NIK", width: 80 },
                    { field: "EmployeeName", title: "Name", width: 250 },
                    { field: "Department", title: "Department", width: 100 },
                    { field: "ServiceID", title: "Service ID", width: 100 },
                    { field: "Position", title: "Position", width: 100 },
                    { field: "JoinDate", title: "Join Date", width: 100, template: "#= (JoinDate == undefined) ? '' : moment(JoinDate).format('DD MMM YYYY') #" },
                    { field: "ResignDate", title: "Resign Date", width: 100, template: "#= (ResignDate == undefined) ? '' : moment(ResignDate).format('DD MMM YYYY') #" },
                    { field: "Status", title: "Status", width: 100 },
                ],
            });

            lookup.dblClick(function (data) {
                if (widget.isNullOrEmpty(data.JoinDate) == false) {
                    try {
                        data["JoinDate"] = widget.toDateFormat(widget.cleanJsonDate(data.JoinDate));
                        data["ResignDate"] = widget.toDateFormat(widget.cleanJsonDate(data.ResignDate));
                    } catch (ex) { }
                }
                widget.populate(data);
                widget.lookup.hide();
                transactionStatus = true;
            });
        });
    });

    function saveServiceID() {
        var params = { EmployeeID: $("[name=EmployeeID]").val(), ServiceID: $("[name=ServiceID]").val() };
        if (params.EmployeeID !== "" && params.ServiceID !== "") {
            var url = "ab.api/employee/SaveServiceID";
            widget.post(url, params, function (result) {
                widget.showNotification(result.message);
            });
        }
        else {
            widget.showNotification("Pilih Employee dan isi Service ID terlebih dahulu");
        }
    }
});




