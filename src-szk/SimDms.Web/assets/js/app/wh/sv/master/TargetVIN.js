var widget = new SimDms.Widget({
    title: "Input Target Active VIN",
    xtype: "panels",
    toolbars: [
        { action: "GenerateTemplate", text: "Generate Template", icon: "fa fa-download" },
        { action: 'UploadExcel', text: 'Upload Excel', icon: 'fa fa-upload' },
        { action: 'DownloadReport', text: 'Download Report', icon: 'fa fa-file-excel-o'}
    ],
    panels: [
        {
            name: "pnlMain",
            cls: "full",
            items: [
                { name: "Year", text: "Year", cls: "span3", type: "select", readonly: false },
                {
                    name: "UploadExcel", text: "Upload Excel", readonly: true, type: "upload",
                    url: "wh.api/TargetVIN/UploadFile", icon: "fa fa-file-archive-o", cls: 'hide', callback: "uploadCallback"
                },
            ]
        },
    ],
    onToolbarClick: function (action) {
        switch (action) {
            case 'GenerateTemplate':
                generateTemplate(action);
                break;
            case 'UploadExcel':
                $('#btnUploadExcel').click();
                break;
            case 'DownloadReport':
                DownloadReport(action);
                break;
            default:
                break;
        }
    }
});
widget.render(function () {
    initComboYear();
});

function initComboYear() {
    $.ajax({
        async: false,
        type: "POST",
        url: 'wh.api/Combo/Years',
        success: function (data) {
            widget.setItems({ name: "Year", type: "select", data: data });

            $('#Year').select2('val', new Date().getFullYear())
        }
    });
}

function generateTemplate(e) {
    var Year = $('#Year').val();
    var generate = e == 'GenerateTemplate' ? true : false;
    sdms.info("Please wait...");
    window.location.href = 'wh.api/TargetVIN/GenerateTemplate?Year=' + Year + '&IsGenerate=' + generate;
};

function DownloadReport(e) {
    var Year = $('#Year').val();
    var generate = e == 'GenerateTemplate' ? true : false;

    widget.post('wh.api/TargetVIN/CheckTargetVIN', { Year: Year }, function (e) {
        if (e.success) {
            sdms.info("Please wait...");
            window.location.href = 'wh.api/TargetVIN/GenerateTemplate?Year=' + Year + '&IsGenerate=' + generate;
        }
        else {
            widget.showNotification(e.message);
        }
    })
};
function uploadCallback(e, obj) {
    if (e.success) {
        widget.showNotification(e.message);
    } else {
        widget.showNotification(e.message);
    }
}


