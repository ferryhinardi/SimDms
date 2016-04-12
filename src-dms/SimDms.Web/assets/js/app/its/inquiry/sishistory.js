var isCEO = false;
var empID = "";
var pType = "";
var Branch = "";
"use strict"

function itsOutletsController($scope, $http, $injector) {

    var me = $scope;

    $injector.invoke(BaseController, this, { $scope: me });

    me.getUserProperties = function () {
        $.ajax({
            async: false,
            type: "POST",
            url: 'its.api/InquiryIts/itsUserProperties',
            success: function (dt) {
                isCEO = dt.data.isCOO;
                empID = dt.data.EmployeeID;
                pType = dt.data.ProductType;
                Branch = dt.data.Branch;
            }
        });
    }

    me.Generate = function () {
        var datefrom = moment(me.data.FromDate).format("YYYYMMDD");
        var dateto = moment(me.data.ToDate).format("YYYYMMDD");
        $http.post('its.api/SISHistory/SISHistoryLoad?DateFrom=' + datefrom + '&DateTo=' + dateto + '&Branch=' + Branch + '&isGM=true')
        //$http.post('its.api/SISHistory/SISHistoryLoad')
           .success(function (dt, status, headers, config) {
               if (dt.success) {
                   me.loadTableData(me.grid1, dt)
                   var sessionName = dt.sessionName;
                   location.href = 'its.api/SISHistory/DownloadFile?sessionName=' + sessionName;
                   MsgBox("Generate HSITSD sukses.\n" + dt.rowCount + " HSITSD telah tergenerate");
               } else {
                   MsgBox("Data tidak ditemukan!", MSG_INFO)
                   me.clearTable(me.grid1);
               }
           })
           .error(function (e, status, header, config) {
               MsgBox("Terjadi kesalahan dalam proses data, silahkan hubungi SDMS support!", MSG_INFO)
           });
    }

    me.grid1 = new webix.ui({
        container: "wxOutlets",
        view: "wxtable", css:"alternating",
        autoWitdh: false,
        width: 1000,
        autoHeight: false,
        height : 450,
        columns: [
            { id: "IncNo", header: "No.Urut", width: 250 },
            { id: "InquiryNumber", header: "No.Inquiry", width: 250 },
            { id: "Resend_1", header: "Frequensi", width: 250 },
            { id: "Resend_2", header: "Resend", width: 250 },
        ],
        on: {
            onSelectChange: function () {
                //if (me.grid1.getSelectedId() !== undefined) {
                //    me.data = this.getItem(me.grid1.getSelectedId().id);
                //    me.data.old = me.grid1.getSelectedId();
                //    me.Apply();
                //}
            }
        }
    });

    me.initialize = function () {
        me.clearTable(me.grid1);
        me.data.FromDate = me.now();
        me.data.ToDate = me.now();
        me.getUserProperties();
    }


    webix.event(window, "resize", function () {
        me.grid1.adjust();
    })

    me.start();
}

$(document).ready(function () {
    var options = {
        title: "SIS History",
        xtype: "panels",
        panels: [
            {
                name: "pnlOutlets",
                items: [
                    {
                        type: "controls",
                        text: "Tanggal",
                        cls: "span6",
                        items: [
                            { name: "FromDate", text: "Tanggal", cls: "span3", type: "ng-datepicker" },
                            { name: "lbl", text: "s/d", cls: "span1", type: "label" },
                            { name: "ToDate", text: "s/d", cls: "span3", type: "ng-datepicker" },
                            {
                                type: "buttons",
                                cls: "span1",
                                items: [
                                        { name: "btnGenerate", text: "Generate SIS", cls: "btn btn-info", click: "Generate()" }
                                ]
                            },
                        ]
                    },
                   
                   { name: "wxOutlets", type: "wxdiv" },
                ]
            }
        ]
    };

    Wx = new SimDms.Widget(options);
    Wx.default = {};
    Wx.render(init);

    function init(s) {
        SimDms.Angular("itsOutletsController");
    }
});