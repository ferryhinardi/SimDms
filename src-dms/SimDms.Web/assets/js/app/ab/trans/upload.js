var widget;
var vars = {};

$(document).ready(function () {
    var options = {
        title: "Upload",
        xtype: "panels",
        toolbars: [
            { name: "btnClear", text: "Clear", icon: "icon-search", cls: "" },
            { name: "btnBrowse", text: "Browse", icon: "icon-search" },
            { name: "btnProcess", text: "Process", icon: "icon-bolt", cls: "hide" },
        ],
        panels: [
            {
                title: "File Information",
                items: [
                    { name: "FileID", type: "hidden"},
                    { name: "FileName", text: "File Name", readonly: true, type: "upload", url: "ab.api/Attendance/UploadFile", icon: "icon-upload", callback: "uploadCallback", onUpload: "onUpload", onProgress: "onProgress" },
                    { name: "FileSize", text: "File Size", readonly: true, cls: "span4" },
                    { name: "UploadedDate", text: "Uploaded Date", readonly: true, cls: "span4", type: "text" },
                    { name: "Processed", text: "Processed Record", readonly: true, cls: "span4", type: "text" },
                    { name: "Unprocessed", text: "Unprocessed Record", readonly: true, cls: "span4", type: "text" },
                ]
            },
            {
                name: "panelAttendanceDetails",
                title: "Attendance Details",
                xtype: "kgrid",
            },
        ],
    }

    widget = new SimDms.Widget(options);
    widget.render(renderCallback);
});


function renderCallback() {
    evt_btnClear();
    evt_btnBrowse();
    evt_btnProcess();
}

function loadAttendanceHeaders() {
    var lookup = widget.klookup({
        name: "panelAttendanceHeaders",
        title: "Attendance Files Upload",
        url: "ab.api/grid/Attendance",
        sort: ({ field: "FileID", dir: "asc" }),
        serverBinding: true,
        filters: [
            {
                text: "Filter",
                type: "controls",
                left: 80,
                items: [
                    { name: "FileName", text: "Filename", cls: "span4" },
                ]
            }
        ],
        columns: [
            { field: "FileID", title: "File ID", width: 600 },
            { field: "IsTransfered", title: "Status", width: 200 },
            { field: "FileName", title: "Filename", width: 250 },
            { field: "Size", title: "Filesize", width: 150 },
            { field: "FileType", title: "Filetype", width: 150 },
        ]
    });

    lookup.dblClick(function (data) {
        widget.clearForm();
        widget.showToolbars(["btnBrowse", "btnProcess"]);

        var datas = $.extend({
            AbsenceFileShowed: data.FileName,
            FileNameShowed: data.FileName,
            UploadedDate: widget.toDateFormat(widget.cleanJsonDate(data.CreatedDate)),
            AbsenceFile: data.FileID,
            FileID: data.FileID,
        }, data);

        widget.populate(datas);
        widget.lookup.hide();
        setTimeout(loadAttendanceDetails, 500);
        widget.showToolbars(["btnClear", "btnBrowse", "btnProcess"]);
    });
}

function loadAttendanceDetails() {
    vars["GridAttendanceDetails"] = widget.kgrid({
        name: "panelAttendanceDetails",
        url: "ab.api/Attendance/LoadUploadedFileData",
        params: { AbsenceFile: $("#FileID").val() },
        serverBinding: true,
        pageSize: 10,
        columns: [
            { field: "AttendanceTime", title: "Date", width: 140, template: "#= (AttendanceTime == undefined) ? '' : moment(AttendanceTime).format('DD MMM YYYY') #" },
            { field: "EmployeeID", title: "NIK", width: 140 },
            { field: "EmployeeName", title: "Name", width: 250 },
            { field: "MachineCode", title: "Machine", width: 100 },
            { field: "AttendanceStatus", title: "Status", width: 100 },
            { field: "ClockTime", title: "Clock Time", width: 120 },
            { field: "Shift", title: "Shift", width: 150 },
            { field: "Status", title: "Processed", width: 140 },
        ],
    });
}

function evt_btnClear() {
    $("#btnClear").on("click", function (evt) {
        widget.clearForm();
        $("[name='FileID']").val("");
        reloadGrid();
    });
}

function evt_btnBrowse() {
    $("#btnBrowse").on("click", function (evt) {
        loadAttendanceHeaders();
    });
}

function evt_btnProcess() {
    $("#btnProcess").on("click", function (evt) {
        var params = widget.getForms();
        var url = "ab.api/Attendance/Process";

        if (widget.isNullOrEmpty(params.FileID) == false) {
            widget.post(url, params, function (result) {
                widget.showNotification(result.message);
                reloadGrid();
            });
        }
    });
}

function reloadGrid() {
    loadAttendanceDetails();
}

function uploadCallback(result, obj) {
    if (result.status) {
        $("[name=FileID]").val(result.data.FileID);
        $("[name=FileNameShowed]").val(result.data.FileName);
        $("[name=FileName]").val(result.data.FileName);
        $("[name=FileSize]").val(result.data.FileSize);
        var uploadedDateInput = $("[name='UploadedDate']"); 
        if (widget.isNullOrEmpty(result.data.UploadedDate) == false) {
            uploadedDateInput.val(widget.toDateFormat(widget.cleanJsonDate(result.data.UploadedDate)));
        }
        else {
            uploadedDateInput.val("");
        }
        reloadGrid();
        widget.showToolbars(["btnClear", "btnBrowse", "btnProcess"]);
    }
}

function onUpload(uploadProgress) {
    widget.showCornerNotification("Uploading file : " + uploadProgress + " %");
}

function onProgress() {
    $.post("ab.api/Attendance/Progress", function (result) {
        if (widget.isNullOrEmpty(result) == false) {
            var message = "";

            if (result.progress == 0) {
                message = "Extracting data, please wait ... !";
            }
            else {
                message = "Processing data : " + result.progress + " %";
            }
            widget.showCornerNotification(message);

            if (result.progress == "100" || result.progress == 100) {
                widget.hideNotification();
            }
        }
    });
}