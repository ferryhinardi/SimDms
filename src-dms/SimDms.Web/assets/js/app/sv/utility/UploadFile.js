"use strict"

function svUtlUploadFile($scope, $http, $injector) {

    var me = $scope;

    $injector.invoke(BaseController, this, { $scope: me });

    me.fileType = function () {
        var lookup = Wx.blookup({
            name: "FileTypeLookup",
            title: "Upload Master",
            manager: SvUtilityManager,
            query: "FileTypeLookup",
            defaultSort: "RefferenceCode asc",
            columns: [
                { field: "RefferenceCode", title: "Kode Reference", width: 150 },
                { field: "Description", title: "Keterangan", width: 200 },
                { field: "DescriptionEng", title: "Keterangan (ENG)", width: 200 },
                { field: "Status", title: "Status", width: 50 }
            ]
        });
        lookup.dblClick(function (data) {
            me.lookupAfterSelect(data);
        });
    }

    me.initialize = function () {
        me.data.ProcessDate = me.now();
        $('#lblStatus').html("");
        $('#lblStatus').css(
         {
             "font-size": "20px",
             "color": "red",
             "font-weight": "bold",
             "text-align": "left"
         });
    }

    me.start();
}

$(document).ready(function () {
    var options = {
        title: "Upload Data Master",
        xtype: "panels",
        toolbars: [
            { name: "btnNew", text: "New", cls: "btn btn-success", icon: "icon-refresh", click: "cancelOrClose()" },
        ],
        panels: [
             {
                 name: "pnlUpload",
                 items: [
                     { name: "ProcessDate", text: "Tanggal Proses", cls: "span3", type: "ng-datepicker" },
                     { name: "lblStatus", cls: "span1", readonly: true, type: "label" },
                     {
                         text: "Tipe File",
                         type: "controls",
                         items: [
                             { name: "RefferenceCode", cls: "span2", placeHolder: "Kode File", readonly: true, type: "popup", click:"fileType()" },
                             { name: "Description ", cls: "span6", placeHolder: "Keterangan", readonly: true }
                         ]
                     },
                     { name: "txtFile", text: "Direktori File ", cls: "span6 ", readonly: true, type: "upload", url: "sv.api/uploadfile/UploadFile", icon: "icon-upload", callback: "uploadCallback", onUpload: "onUpload" },
                     {
                         type: "buttons", cls: "span2", items: [
                             { name: "btnProcess", text: "Process", icon: "icon-gear", click: "Process()", cls: "button small btn btn-warning", disable: true },
                         ]
                     },
                     { name: "Contents", text: "", type: "textarea", style: " height: 400px;", readonly: true },
                 ]
             },
            
        ]
    };

    Wx = new SimDms.Widget(options);
    Wx.default = {};
    Wx.render(init);

    function init(s) {
        SimDms.Angular("svUtlUploadFile");
    }
});

function uploadCallback(result, obj) {
    $("[name=txtFileShowed]").val(result.message);
    Contents = result.details;
    $('#Contents').val(result.details);
    $('#btnProcess').removeAttr('disabled');
    $('#lblStatus').html("NEW");
}

function onUpload(uploadProgress) {
    Wx.Success("Uploading file : " + uploadProgress + " %");
}