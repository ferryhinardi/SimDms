var totalSrvAmt = 0;
var status = 'N';
var svType = '2';

"use strict";

function spLookupController($scope, $http, $injector) {

    var me = $scope;
    $injector.invoke(BaseController, this, { $scope: me });
    
    me.Upload = function () {
        if (Wx.validate()) {
            me.data.Content = fileContent;
            var params = {
                
            }
            $http.post('gn.api/uploadBranchData/UploadFile', params)
            .success(function (result, status, headers, config) {
                if (result.success) {
                    me.PopulatePanelInfo(result);
                    me.data.datPartInfo = result.PartInfo;
                    me.SetViewMode('upload');
                }
                else {
                    MsgBox(result.message, MSG_ERROR);
                    return;
                }
            }).
            error(function (data, status, headers, config) {
                MsgBox('Terjadi Kesalahan, Hubungi SDMS Support');
            });
        }
        else {
            MsgBox('Ada informasi yang tidak lengkap.', MSG_WARNING);
        }
    };

    me.initGrid = function () {
        me.grid1 = new webix.ui({
            container: "wxsalestarget",
            view: "wxtable", css:"alternating", scrollX: true,
            columns: [
                { id: "TableName", header: "Table Name", width: 300 },
                { id: "DataNew", header: "Data New", width: 300 },
                { id: "DataUpdate", header: "Data Update", width: 300 },
                { id: "FileName", header: "File Name", width: 300 },
            ],
            on: {
                onSelectChange: function () {
                    if (me.grid1.getSelectedId() !== undefined) {
                        me.detail = this.getItem(me.grid1.getSelectedId().id);
                        me.detail.oid = me.grid1.getSelectedId();
                        me.Apply();
                    }
                }
            }
        });
    }

    me.initialize = function () {
        me.clearTable(me.grid1);
        $('#Upl').prop('type', 'File');
        me.Apply();
    }


    me.initGrid();

    webix.event(window, "resize", function () {
        me.grid1.adjust();
    })

    me.start();
}

$(document).ready(function () {
    var options = {
        title: "File Information",
        xtype: "panels",
        //toolbars: [{ name: "btnApprove", text: "Browse", cls: "btn btn-info", icon: "icon-ok", click: "approve()", type:"File" }],//,WxButtons,
        panels: [
             {// 
                 name: "pnlPO",
                 items: [
                             {
                                 text: "File Name",
                                 type: "controls",
                                 cls: "span8",
                                 items: [
                                     { name: "FileID", model: 'data.FileID', type: "hidden" },
                                     {
                                        name: "UploadFileName",
                                        text: "File",
                                        readonly: true,
                                        type: "upload",
                                        url: "gn.api/uploadBranchData/UploadFile",
                                        icon: "icon-folder-open",
                                        callback: "uploadCallback",
                                        cls: "span4",
                                        required: true
                                     },
                                     {
                                        type: "buttons",
                                        items: [
                                            { name: "btnUpload", text: "Upload", icon: "icon-upload", cls: "btn btn-primary", click: "Upload()" }
                                        ]
                                     }
                         ]
                     },
                    
                 ]
             },
             {
                name: "wxsalestarget",
                xtype: "wxtable",
            },
        ]
    };

    Wx = new SimDms.Widget(options);
    Wx.default = {};
    Wx.render(init);

    function init() {
        SimDms.Angular("spLookupController");
    }
});




