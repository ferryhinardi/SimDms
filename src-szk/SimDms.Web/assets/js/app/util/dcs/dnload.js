$(document).ready(function () {
    var options = {
        title: "Maintain Flat File (Download DCS)",
        xtype: "panels",
        panels: [
            {
                title: "Filter Data",
                items: [
                    {
                        name: "DataID", text: "Data ID", cls: "span4", type: "select",
                        items: [
                            { value: "SITSD", text: "SITSD" },
                            { value: "SITSP", text: "SITSP" }
                        ]
                    },
                    {
                        name: "DealerCode", text: "Dealer", cls: "span4", type: "select",
                        dataSource: "util.api/Lookup/Dealers",
                        dataSourceParams: {}
                    },
                    { name: "StartDate", text: "Date Start", cls: "span4", type: "datepicker" },
                    { name: "EndDate", text: "Date End", cls: "span4", type: "datepicker" },
                    {
                        name: "Status", text: "Status", cls: "span4", type: "select",
                        items: [
                            { value: "A", text: "A" },
                            { value: "P", text: "P" },
                            { value: "X", text: "X" }
                        ]
                    }
                ]
            },
            {
                xtype: "table"  
            }
        ],
        toolbars: [
            { name: "btnClear", text: "Clear", icon: "fa fa-gear" },
            { name: "btnBrowse", text: "Browse", icon: "fa fa-search" },
            { name: "btnProcess", text: "Process", icon: "fa fa-gear" }
        ],
    }
    var widget = new SimDms.Widget(options);
    widget.render();

    $("#btnClear").on("click", function () {
        $("#DataID").val("[Select One]");
        $("#DealerCode").val("");
        $(".datepicker").datepicker("setDate", null);
        $("#Status").val("[Select One]");
        $("#gridDownloadFile").html("");
    });

    $("#btnProcess").on("click", function () {
        var updatedStatus = prompt("Masukkan perubahan status ! [A, P, X]");
        if (updatedStatus !== undefined && updatedStatus !== null && $.trim(updatedStatus) != "") {
            if (updatedStatus == "A" || updatedStatus == "P" || updatedStatus == "X") {
                var params = {
                    DataID: $("#DataID").val(),
                    Status: $("#Status").val(),
                    StartDate: $("input[name='StartDate']").val(),
                    EndDate: $("input[name='EndDate']").val(),
                    DealerCode: $("#DealerCode").val(),
                    UpdatedStatus: updatedStatus
                };

                widget.post("util.api/DownloadFile/UpdateDcsDownloadFileStatus", params, function (result) {
                    alert(result.message);
                });
            }
        }
    });
       
    $("#btnBrowse").on("click", function () {
        var params = {
            DataID: $("#DataID").val(),
            Status: $("#Status").val(),
            StartDate: $("input[name='StartDate']").val(),
            EndDate: $("input[name='EndDate']").val(),
            DealerCode: $("#DealerCode").val()
        };

        $(".gl-widget .panel:nth-child(2)").append("<div id='gridDownloadFile'></div>");

        widget.post("util.api/grid/downloadfiles", params, function (result) {
            if(result.length > 0) {
                var htmlContent = "<table>";
                htmlContent += "<thead>";
                htmlContent += "<td>ID</td>";
                htmlContent += "<td>Data ID</td>";
                htmlContent += "<td>Status</td>";
                htmlContent += "<td>Customer Code</td>";
                htmlContent += "<td>Created Date</td>";
                htmlContent += "</thead>";

                htmlContent += "<tbody>";

                $.each(result, function (key, val) {
                    var tempDate = new Date(parseInt(val.CreatedDate.replace("/Date(", "").replace(")/", ""), 10));
                
                    htmlContent += "<tr>";
                    htmlContent += "<td>" + val.ID + "</td>";
                    htmlContent += "<td>" + val.DataID + "</td>";
                    htmlContent += "<td>" + val.Status + "</td>";
                    htmlContent += "<td>" + val.CustomerCode + "</td>";
                    htmlContent += "<td>" + tempDate.getDate() + "-" + (tempDate.getMonth() + 1) + "-" + tempDate.getFullYear() + "</td>";
                    htmlContent += "</tr>";
                });

                htmlContent += "</tbody>";
                htmlContent += "</table>";

                $("#gridDownloadFile").html(htmlContent);
            }
            else {
                $("#gridDownloadFile").html("<strong>No record was found</strong>");
            }
        });
    });
});