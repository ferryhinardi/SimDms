var totalSrvAmt = 0;
var status = 'N';
var svType = '2';

"use strict";

function spUploadDataDelearSparepartController($scope, $http, $injector) {

    var me = $scope;

    $injector.invoke(BaseController, this, { $scope: me });

    $http.post('sp.api/UploadDataDealer/DataID').
    success(function (data, status, headers, config) {
        me.comboDataID = data;
    });

    $('#FirstPeriod').on('change', function (e) {
        var firstPeriod = $('#FirstPeriod').val();
        var endPeriod = $('#EndPeriod').val();

        if (firstPeriod > endPeriod) { $('#EndPeriod').val(firstPeriod) }
    });

    $('#EndPeriod').on('change', function (e) {
        var firstPeriod = $('#FirstPeriod').val();
        var endPeriod = $('#EndPeriod').val();

        if (firstPeriod < endPeriod) { $('#FirstPeriod').val(endPeriod) }
    });

    me.default = function () {
        $http.post('sp.api/UploadDataDealer/Default').
          success(function (e) {
              me.data.FirstPeriod = e.FirstPeriod;
              me.data.EndPeriod = e.EndPeriod;
              stat = e.stat;
              $('#lblStatus').html(e.Status);
          });
    }

    me.retrieve = function () {
        if (!stat) {
            MsgBox("Web Service sedang offline, tidak bisa transfer data");
        }
        else {
            if (me.data.DataID == undefined || me.data.DataID == null) {
                MsgBox("Data ID harus dipilih salah satu!!!");
            } else {
                $http.post('sp.api/UploadDataDealer/Retrieve', me.data)
                .success(function (e) {
                        me.loadTableData(me.gridDataDealer, e);
                })
                .error(function (e) {
                    MsgBox('Terjadi Kesalahan, Hubungi SDMS Support');
                });
            }
        }
    }

    me.upload = function () {
        $http.post('sp.api/UploadDataDealer/upload', me.data)
        .success(function (e) {
            if (e.success) {
                Wx.Success(e.message);
            } else {
                MsgBox(e.message, MSG_ERROR);
            }
        })
        .error(function (e) {
            MsgBox('Terjadi Kesalahan, Hubungi SDMS Support');
        });
    }
    

    me.gridDataDealer = new webix.ui({
        container: "wxgridDCSData",
        view: "wxtable", css:"alternating",
        scrollX: true,
        autoHeight: false,
        height: 400,
        columns: [
            { id: "CreatedDate", header: "Created Date", format: me.dateFormat, width: 110 },
            { id: "CustomerCode", header: "Dealer Code", width: 110 },
            { id: "Status", header: "Status", width: 65 },
            { id: "Contents", header: "Info", fillspace: true },
        ],
        on: {
            onSelectChange: function () {
                if (me.gridDataDealer.getSelectedId() !== undefined) {
                    //me.data.panels = this.getItem(me.gridDataDealer.getSelectedId().id);
                    me.data.Contents = this.getItem(me.gridDataDealer.getSelectedId().id).Contents;
                    //me.dtlPart.old = me.gridOrderDetail.getSelectedId();
                    me.data.ID = this.getItem(me.gridDataDealer.getSelectedId().id).ID;
                    me.Apply();
                }
            }
        }
    });


    me.initialize = function () {
        me.default();
        me.clearTable(me.gridDataDealer);

        $('#lblStatus').css(
         {
             "font-size": "28px",
             "color": "blue",
             "font-weight": "bold",
             "text-align": "right"
         });
        me.data.IsAll = false;
    }

    me.start();
}

$(document).ready(function () {
    var options = {
        title: "Upload Data Dealer (Sparepart)",
        xtype: "panels",
        panels: [
             {
                name: "pnl",
                items: [
                   { name: "lblStatus", text: "", cls: "span12", readonly: true, type: "label" },
                   { type: "label", text: "Inquiri DCS Data", style: "font-size: 14px; color : blue;" },
                   { type: "div", cls: "divider" },
                   { name: "FirstPeriod", text: "Periode", cls: "span4", type: "ng-datepicker" },
                   { name: "EndPeriod", text: "S/D", cls: "span4", type: "ng-datepicker" },
                   { name: "DataID", text: "Data ID", cls: "span4", type: "select2", datasource: "comboDataID" },
                   { name: "IsAll", text: "All Status", cls: "span4", type: "x-switch" },
                   {
                       type: "buttons", cls: "full", items: [
                             {
                                 name: "btnRetrieve", text: " Retrieve", cls: "btn btn-primary span8 full", icon: "icon icon-search", click: "retrieve()"
                             }
                       ]
                   },
                   { type: "label", text: "Preview DCS Data", style: "font-size: 14px; color : blue;" },
                   { type: "div", cls: "divider" },
                   {
                       name: "ctlTextData",
                       type: "controls",
                       text: "Preview DCS Data",
                       cls: "span8",
                       style: "margin-top: 15px;",
                       items: [
                           {
                               name: "wxgridDCSData",
                               cls: "span4",
                               type: "wxdiv"
                           },
                           {
                               name: "Contents",  type: "textarea", cls: "span4", text: "", style: "height: 250px; max-height: 250px;"
                           },
                           {
                               type: "buttons", cls: "span1", items: [
                                   {
                                       name: "btnUpload", text: " Upload", cls: "btn btn-success span2", icon: "icon icon-upload", click: "upload()"
                                   },
                               ]
                           },
                          
                       ]
                   },
                ]
            }
        ]
    };
 
    Wx = new SimDms.Widget(options);
    Wx.default = {};
    Wx.render(init);

    function init(s) {
        SimDms.Angular("spUploadDataDelearSparepartController");
    }

});