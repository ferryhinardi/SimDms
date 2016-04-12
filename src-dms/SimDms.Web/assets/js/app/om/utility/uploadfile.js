"use strict"
var Contents = "";

function omUtilityUploadFileController($scope, $http, $injector) {

    var me = $scope;

    $injector.invoke(BaseController, this, { $scope: me });

    $http.post('om.api/combo/uploadtype')
    .success(function (e) {
        me.comboUploadType = e;
        me.init();
    });

    me.$watch('data.UploadType', function (a,b) {
        if (a !== b) {
            $("[name=txtFileShowed]").val("");
            $('#Contents').val("");
            $('#btntxtFile').removeAttr('disabled');
        }
    })

    me.Process = function () {
        me.data.Contents = Contents;
        if (me.data.UploadType == undefined || me.data.UploadType == null)
        {
            MsgBox("Upload Data harus dipilih salah satu !!!");
        }
        else {
            $http.post('om.api/uploadfile/process', me.data)
            .success(function (e) {
                if (e.success) {
                    Wx.Success("Proses Upload Berhasil");
                    $('#lblStatus').html("[UPLOADED]");
                    $('#btnProcess').attr('disabled', true);
                    $('#btntxtFile').attr('disabled', true);
                    
                } else {
                    MsgBox(e.message, MSG_ERROR);
                }
            })
            .error(function (e) {
                MsgBox('Terjadi Kesalahan, Hubungi SDMS Support');
            });
        }
    }

    me.default = function () {
        $http.post('om.api/uploadfile/default')
        .success(function (e) {
            if (e.IsBranch) {
                $('#btnProcess').attr('disabled', true);
                $('#btntxtFile').attr('disabled', true);
            }
        });
    }

    me.initialize = function () {
        me.default();
        $('#lblStatus').html("[STATUS]");
        $('#lblStatus').css(
         {
             "font-size": "28px",
             "color": "red",
             "font-weight": "bold",
             "text-align": "right"
         });
        $('#UploadType').select2('val', '');
        $('#btntxtFile').attr('disabled', false);
    }
    
    me.start();
}

$(document).ready(function () {
    var options = {
        title: "Upload File",
        xtype: "panels",
        toolbars: [
            { name: "btnNew", text: "New", cls: "btn btn-success", icon: "icon-refresh", click: "cancelOrClose()" },
        ],
        panels: [
             {
                 name: "pnlStatus",
                 items: [
                     { name: "lblStatus", text: "", cls: "span4", readonly: true, type: "label" },
                 ]
             },
            {
                name: "pnlUploadFile",
                items: [
                    { name: "UploadType", id: "UploadType", text: "Pilihan Upload Data", cls: "span4", type: "select2", datasource: "comboUploadType" },
                    { name: "txtFile", text: "Direktori File ", cls: "span6 ", readonly: true, type: "upload", url: "om.api/uploadfile/UploadFile",icon: "icon-upload", callback: "uploadCallback", onUpload: "onUpload" },
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
        SimDms.Angular("omUtilityUploadFileController");
    }
});

function uploadCallback(result, obj) {
    $("[name=txtFileShowed]").val(result.message);
    Contents = result.details;
    $('#Contents').val(result.details);
    $('#btnProcess').removeAttr('disabled');
    $('#lblStatus').html("[OPEN]");
}

function onUpload(uploadProgress) {
    Wx.Success("Uploading file : " + uploadProgress + " %");
}




