"use strict"

function uploadfile($scope, $http, $injector) {

    var me = $scope;

    $injector.invoke(BaseController, this, { $scope: me });

    webix.event(window, "resize", function () {
        me.grid1.adjust();
    });

    me.grid1 = new webix.ui({
        container: "wxuploadfile",
        view: "wxtable", css:"alternating",
        autowidth: false,
        width: 670,
        autoHeight: false,
        height: 420,
        scrollY: true,
        columns: [
            { id: "SeqNo", header: "No", width: 70 },
            { id: "InquiryNumber", header: "No. Inquiry", width: 200 },
            { id: "NewInquiryNumber", header: "No. Inquiry Baru", width: 200 },
            { id: "Status", header: "Status", width: 200 },
        ],
    });

    me.Browse = function () {

    }

    me.Process = function () {

    }
    
    me.initialize = function () {
        $('#btnProcess').attr('disabled', 'disabled');
    }

    me.start();
}

$(document).ready(function () {
    var options = {
        title: "Upload File",
        xtype: "panels",
        toolbars: [
            { name: "btnNew", text: "New", cls: "btn btn-success", icon: "icon-refresh", click: "initialize()" }
        ],
        panels: [
             {
                 items: [
                     { name: "txtFile", text: "Direktori File ", cls: "span4", readonly: true, type: "upload", url: "its.api/uploadfile/UploadFile", icon: "icon-upload", callback: "uploadCallback", onUpload: "onUpload" },
                     {
                         type: "buttons",
                         items: [
                             { name: "btnProcess", cls: "btn btn-success", text: " Proses", icon: "icon-gear", click: "Process()" }
                         ]
                     },
                     {
                         name: "wxuploadfile",
                         type: "wxdiv",
                     }
                 ]
             }
        ]
    };

    Wx = new SimDms.Widget(options);
    Wx.default = {};
    Wx.render(init);


    function init(s) {
        SimDms.Angular("uploadfile");
    }
});

function uploadCallback(result, obj) {
    if (result.message == "") {
        $("[name=txtFileShowed]").val(result.fileName);
        $('#btnProcess').removeAttr('disabled');
        
        //Contents = result.details;
        //$('#Contents').val(result.details);
        //$('#btnProcess').removeAttr('disabled');
        //$('#lblStatus').html("NEW");
    } else {
        MsgBox(result.message);
    }
}

function onUpload(uploadProgress) {
    //Wx.Success("Uploading file : " + uploadProgress + " %");
    console.log("this is onUpload");
}
