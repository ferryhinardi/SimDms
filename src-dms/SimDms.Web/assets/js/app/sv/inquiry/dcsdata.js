"use strict"

var stat = false;

function svInqDcsDataController($scope, $http, $injector) {

    var me = $scope;

    $injector.invoke(BaseController, this, { $scope: me });

    $http.post('sv.api/InqDCSData/DataID').
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
        $http.post('sv.api/InqDCSData/Default').
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
                $http.post('sv.api/InqDCSData/Retrieve', me.data)
                .success(function (e) {
                    console.log(e);
                    if (e.success) {
                        me.loadTableData(me.gridDCSData, e.data);
                    }
                    else {
                        MsgBox(e.message, MSG_ERROR);
                    }
                })
                .error(function (e) {
                    MsgBox('Terjadi Kesalahan, Hubungi SDMS Support');
                });
            }
        }
    }

    me.gridDCSData = new webix.ui({
        container: "wxgridDCSData",
        view: "wxtable", css:"alternating",
        scrollX: true,
        autoHeight : false,
        height: 400,
        columns: [
            { id: "CreatedDate", header: "Created Date", format: me.dateFormat, width: 110},
            { id: "DealerCode", header: "Dealer Code",  width: 110 },
            { id: "Status", header: "Status",  width: 65 },
            { id: "Info", header: "Info", fillspace: true },
        ],
        on: {
            onSelectChange: function () {
                if (me.gridDCSData.getSelectedId() !== undefined) {
                    //me.data.panels = this.getItem(me.gridDCSData.getSelectedId().id);
                    me.data.Contents = this.getItem(me.gridDCSData.getSelectedId().id).Contents;
                    //me.dtlPart.old = me.gridOrderDetail.getSelectedId();
                    me.Apply();
                }
            }
        }
    });


    me.initialize = function () {
        me.default();
        me.clearTable(me.gridDCSData);
        
        $('#lblStatus').css(
         {
             "font-size": "28px",
             "color": "blue",
             "font-weight": "bold",
             "text-align": "right"
         });
    }

    me.start();
}

$(document).ready(function () {
    var options = {
        title: "Inquiry Data DCS (Service)",
        xtype: "panels",
        toolbars: [
            { name: "btnRetrieve", text: "Retrieve", icon: "icon-refresh", cls: "btn btn-success", click: "retrieve()" }
        ],
        panels: [
             {
                 name: "pnlStatus",
                 items: [
                     { name: "lblStatus", text: "", cls: "span12", readonly: true, type: "label" },
                 ]
             },
            {
                name: "pnl",
                items: [
                   { name: "FirstPeriod", text: "Periode", cls: "span4", type: "ng-datepicker" },
                   { name: "EndPeriod", text: "S/D", cls: "span4", type: "ng-datepicker" },
                   { name: "DataID", text: "Data ID", cls: "span4 full", type: "select2", datasource: "comboDataID" },
                   {
                       name: "wxgridDCSData",
                       cls: "span4",
                       type: "wxdiv",
                       style: "padding-top:0px"
                   },
                   {
                       name: "Contents",
                       cls: "span4 left",
                       type: "textarea",
                       style: "height:400px; width:600px; margin-left:-150px; margin-top:25px"
                   }
                ]
            }
        ]
    };

    Wx = new SimDms.Widget(options);
    Wx.default = {};
    Wx.render(init);

    function init(s) {
        SimDms.Angular("svInqDcsDataController");
    }
});