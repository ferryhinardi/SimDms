"use strict";

var Engine = "";
var SalesModel = "";
var Colour = "";
var ServiceBookNo = "";
var KeyNo = "";
var Dealer = "";
var DONo = "";
var SJNo = "";

function ReceivingUnitController($scope, $http, $injector) {

    var me = $scope;

    $injector.invoke(BaseController, this, { $scope: me });

    me.initialize = function () {
        $("#VIN").css({ "line-height": "100px", "height": "75px", "font-size": "50px" });
        $("#Vehicle").css({ "height": "100px", "font-size": "75px" });
        $("#Driv").css({ "line-height": "100px", "height": "75px", "font-size": "50px" });
        $("#Driver").css({ "height": "75px", "font-size": "50px" });
        $("#Engine, #SalesModel, #Colour, #ServiceBookNo, #KeyNo, #Dealer, #DONo, #SJNo, #btnAdd, #btnCencel").css({ "height": "75px", "font-size": "50px" });

        $("#Vehicle").focus(); 

    }

    me.renderTable = function(){
        var table = "";

        table = " <table style='width:100%'>"
          + "<tr>"
          + "  <td style='width:25%; line-height: 2px; height: 75px; font-size: 50px;'>Engine</td>"
          + "  <td style='width:75%; line-height: 2px; height: 75px; font-size: 50px;'>: " + Engine + "</td>"
          + "</tr>"
          + "<tr>"
          + "  <td style='width:25%; line-height: 50px; height: 75px; font-size: 50px;'>Sales Model</td>"
          + "  <td style='width:75%; line-height: 2px; height: 75px; font-size: 50px;'>: " + SalesModel + "</td>"
          + "</tr>"
          + "<tr>"
          + "  <td style='width:25%; line-height: 2px; height: 75px; font-size: 50px;'>Colour</td>"
          + "  <td style='width:75%; line-height: 2px; height: 75px; font-size: 50px;'>: " + Colour + "</td>"
          + "</tr>"
          + "<tr>"
          + "  <td style='width:25%; line-height: 50px; height: 75px; font-size: 50px;'>Service Book No</td>"
          + "  <td style='width:75%; line-height: 2px; height: 75px; font-size: 50px;'>: " + ServiceBookNo + "</td>"
          + "</tr>"
          + "<tr>"
          + "  <td style='width:25%; line-height: 2px; height: 75px; font-size: 50px;'>Key No</td>"
          + "  <td style='width:75%; line-height: 2px; height: 75px; font-size: 50px;'>: " + KeyNo + "</td>"
          + "</tr>"
          + "<tr>"
          + "  <td style='width:25%; line-height: 2px; height: 75px; font-size: 50px;'>Dealer</td>"
          + "  <td style='width:75%; line-height: 2px; height: 75px; font-size: 50px;'>: " + Dealer + "</td>"
          + "</tr>"
          + "<tr>"
          + "  <td style='width:25%; line-height: 2px; height: 75px; font-size: 50px;'>DO No</td>"
          + "  <td style='width:75%; line-height: 2px; height: 75px; font-size: 50px;'>: " + DONo + "</td>"
          + "</tr>"
          + "<tr>"
          + "  <td style='width:25%; line-height: 2px; height: 75px; font-size: 50px;'>SJ No</td>"
          + "  <td style='width:75%; line-height: 2px; height: 75px; font-size: 50px;'>: " + SJNo + "</td>"
          + "</tr>"
          + "</table> ";

        $("#pnlDashboard").html(table);

    };

    $("[name='Vehicle']").on('keypress', function (e) {
        if (e.keyCode === 13) {
            $http.post('om.api/ReceivingUnit/GeData?Vehicle=' + me.data.Vehicle).
               success(function (data, status, headers, config) {

                   Engine = data.data.EngineCode + data.data.EngineNo;
                   SalesModel = data.data.SalesModelCode + data.data.SalesModelYear;
                   Colour = data.Colour + '(' + data.data.ColourCode + ')';
                   ServiceBookNo = data.data.ServiceBookNo;
                   KeyNo = data.data.KeyNo;
                   Dealer = data.DealerAbbreviation + '(' + data.data.CompanyCode + ')';
                   DONo = data.data.DONo;
                   SJNo = "";

                   me.renderTable();

                   //$('#Engine').html('Engine : ' + data.data.EngineCode + data.data.EngineNo);
                   //$('#SalesModel').html('SalesModel : ' + data.data.SalesModelCode + data.data.SalesModelYear);
                   //$('#Colour').html('Colour : ' + data.Colour + '(' + data.data.ColourCode + ')');
                   //$('#ServiceBookNo').html('ServiceBookNo : ' + data.data.ServiceBookNo);
                   //$('#KeyNo').html('KeyNo : ' + data.data.KeyNo);
                   //$('#Dealer').html('Dealer : ' + data.DealerAbbreviation + '(' + data.data.CompanyCode + ')');
                   //$('#DONo').html('DONo : ' + data.data.DONo );
                   //$('#SJNo').html('SJNo : ');

               }).
               error(function (data, status, headers, config) {
                   alert("error");
               });
        }
    });

    me.Save = function () {
        $http.post('om.api/ReceivingUnit/Save?Vehicle=' + me.data.Vehicle + '&Driver=' + me.data.Driver)
        .success(function (e) {
            if (e.success) {
                alert('Data Saved');
                me.initialize();
                me.data.Vehicle = "";
                me.data.Driver = "";

                Engine = "";
                SalesModel = "";
                Colour = "";
                ServiceBookNo = "";
                KeyNo = "";
                Dealer = "";
                DONo = "";
                SJNo = "";

                me.renderTable();
            } else {
                alert(e.message);
            }
        })
        .error(function (e) {
            alert('Terjadi Kesalahan, Hubungi SDMS Support');
        });
    }

    me.Cencel = function () {
        me.initialize();
        me.data.Vehicle = "";
        me.data.Driver = "";

        Engine = "";
        SalesModel = "";
        Colour = "";
        ServiceBookNo = "";
        KeyNo = "";
        Dealer = "";
        DONo = "";
        SJNo = "";

        me.renderTable();
    }

    me.start();
}



$(document).ready(function () {
    var options = {
        title: "Receiving Unit",
        xtype: "panels",
        panels: [
            {
                name: "ReceivingUnit",
                title: "Receiving Unit",
                items: [
                        { name: "VIN", type: "label", text: "VIN" },
                        { name: "Vehicle" },
                        { name: "Driv", type: "label", text: "Driver" },
                        { name: "Driver" },
                        {
                            type: "buttons",
                            items: [
                                    { name: "btnAdd", text: "Save", click: "Save()" },
                                    { name: "btnCencel", text: "Cancel", click: "Cencel()" },
                            ]
                        },
                        //{ type : "hr"},
                        //{ name: "Engine", type: "label", text: " " },
                        //{ name: "SalesModel", type: "label", text: " " },
                        //{ name: "Colour", type: "label", text: " " },
                        //{ name: "ServiceBookNo", type: "label", text: " " },
                        //{ name: "KeyNo", type: "label", text: " " },
                        //{ name: "Dealer", type: "label", text: " " },
                        //{ name: "DONo", type: "label", text: " " },
                        //{ name: "SJNo", type: "label", text: " " },
                ]
            },
            {
                name: "pnlDashboard",
                xtype: "k-grid",
            },
        ]
    };

    Wx = new SimDms.Widget(options);
    Wx.default = {};
    Wx.render(init);

    function init(s) {
        SimDms.Angular("ReceivingUnitController");
    }
});

