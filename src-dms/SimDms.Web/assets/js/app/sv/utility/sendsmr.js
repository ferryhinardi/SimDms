"use strict"

function svUtlSendSMRController($scope, $http, $injector) {

    var me = $scope;

    $injector.invoke(BaseController, this, { $scope: me });

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

    me.inquiry = function () {
        $http.post('sv.api/sendsmr/inquiry', me.data)
       .success(function (e) {
           me.data.Contents = e.Text;
       })
       .error(function (e) {
           MsgBox('Terjadi Kesalahan, Hubungi SDMS Support');
       });
    }

    me.savefile = function () {
        window.location = "sv.api/sendsmr/savefile?Contents=" + me.data.Contents;
    }

    me.sendfile = function () {
        $http.post('sv.api/sendsmr/sendfile', me.data)
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

    me.default = function () {
        $http.post('sv.api/sendsmr/Default').
          success(function (e) {
              me.data.FirstPeriod = e.FirstPeriod;
              me.data.EndPeriod = e.EndPeriod;
          });
    }

    me.initialize = function () {
        me.default();
    }

    me.start();
}

$(document).ready(function () {
    var options = {
        title: "Send SMR Data",
        xtype: "panels",
        panels: [
            {
                name: "pnlSendSMR",
                items: [
                   { name: "FirstPeriod", text: "Periode", cls: "span4", type: "ng-datepicker" },
                   { name: "EndPeriod", text: "S/D", cls: "span4", type: "ng-datepicker" },
                   {
                       name: "Contents", type: "textarea", cls: "span4", text: "", style: "height: 400px; width: 900px"
                   },
                   {
                       type: "buttons",
                       items: [
                              { name: "btnInquiry", text: "Inquiry",  cls: "btn btn-info", click: "inquiry()" },
                              { name: "btnSave", text: "Save File",  cls: "btn btn-info", click: "savefile()" },
                              { name: "btnSend", text: "Send To DCS",  cls: "btn btn-info", click: "sendfile()" },
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
        SimDms.Angular("svUtlSendSMRController");
    }
});