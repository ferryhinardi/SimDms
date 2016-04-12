"use strict"

function SetFakPajak($scope, $http, $injector) {
    var me = $scope;

    $injector.invoke(BaseController, this, { $scope: me });

    me.initialize = function () {
        var FpjID = "FPJSales";
        $http.post('tax.api/utility/FpjConfig?FpjID=' + FpjID)
           .success(function (e) {
               if (e.FpjValue == "Form1") {
                   me.data.FpjValue = "Form1";
               } else if (e.FpjValue == "Form2") {
                   me.data.FpjValue = "Form2";
               } else {
                   MsgBox('Value Tidak Sesuai Dengan Format !', MSG_ERROR);
               }
           })
           .error(function (e) {
           MsgBox('Terjadi Kesalahan, Hubungi SDMS Support', MSG_ERROR);
           });
    }

    me.Save = function (e, param) {
        me.data.FpjID = "FPJSales";
        $http.post('tax.api/utility/Save', me.data).
            success(function (data, status, headers, config) {
                if (data.success) {
                    Wx.Success("Data saved...");
                    me.startEditing();

                } else {
                    MsgBox(data.message, MSG_ERROR);
                }
            }).
            error(function (e, status, headers, config) {
                //MsgBox('Terjadi Kesalahan, Hubungi SDMS Support');
                console.log(e);
            });
    }

    me.start();
}

$(document).ready(function () {
    var options = {
        title: "Setting Faktur Pajak",
        xtype: "panels",
        panels: [
            {
                name: "pnlA",
                title: "Pre-Printed Fak.Pajak.Sales",
                items: [
                    {
                        type: "optionbuttons", name: "Status", model: "data.FpjValue", text: "Status",
                        items: [
                            { name: "Form1", text: "Using Form 1" },
                            { name: "Form2", text: "Using Form 2" },
                        ]
                    },
                    {
                        type: "buttons", cls: "span2", items: [
                            { name: "btnSave", text: "Save", icon: "icon-Save", click: "Save()", cls: "button small btn btn-success" },
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
        SimDms.Angular("SetFakPajak");
    }



});