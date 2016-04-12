"use strict";
var stat = false;
function InquiryDataDCS($scope, $http, $injector) {

    var me = $scope;

    $injector.invoke(BaseController, this, { $scope: me });

    me.DataID = [
        { "value": 'SDSTK', "text": 'SDSTK' },
        { "value": 'SFREQ', "text": 'SFREQ' },
    ];

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
        $http.post('om.api/InqDCSData/Default').
          success(function (e) {
              me.data.FirstPeriod = e.FirstPeriod;
              me.data.EndPeriod = e.EndPeriod;
              stat = e.stat;
              $('#Status').html(e.Status);
              $('#Status').css(
               {
                   "font-size": "28px",
                   "color": "blue",
                   "font-weight": "bold",
                   "text-align": "right"
               });
          });
    }


    me.grid1 = new webix.ui({
        container: "wxgridDCSData",
        view: "wxtable", css:"alternating",
        scrollX: true,
        autoHeight: false,
        height: 400,
        columns: [
            { id: "CreatedDate", header: "Created Date", fillspace: true },
            { id: "DealerCode", header: "Dealer Code", fillspace: true },
            { id: "Status", header: "Status", fillspace: true },
            { id: "Info", header: "Info", fillspace: true },
        ],
        on: {
            onSelectChange: function () {
                if (me.gridDCSData.getSelectedId() !== undefined) {
                    me.data.Contents = this.getItem(me.gridDCSData.getSelectedId().id).Contents;
                    me.Apply();
                }
            }
        }
    });

    me.initialize = function () {
        me.default();
        me.clearTable(me.grid1);

        $('#lblStatus').css(
         {
             "font-size": "48px",
             "color": "blue",
             "font-weight": "bold",
             "text-align": "right"
         });

    }

    me.Retrieve = function () {
        if (!stat) {
            MsgBox("Web Service sedang offline, tidak bisa transfer data");
        }
        else {
            if (me.data.DataID == undefined || me.data.DataID == null) {
                MsgBox("Data ID harus dipilih salah satu!!!");
            } else {
                $http.post('om.api/InqDCSData/Retrieve', me.data)
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

    me.start();

}
 

$(document).ready(function () {
    var options = {
        title: "Inquiry Data DCS (Sales)",
        xtype: "panels",
        panels: [
            {
                name: "pnlA",
                items: [
                    {
                        text: "Periode",
                        type: "controls",
                        cls: "span6",
                        items: [
                            { name: "FirstPeriod", text: "", cls: "span3", type: "ng-datepicker" },
                            { type: "label", text: "s.d", cls: "span1 mylabel" },
                            { name: "EndPeriod", text: "", cls: "span3", type: "ng-datepicker" },
                        ]
                    },
                    { name: "Status", cls: "span2", readonly: true, type: "label" },
                    { name: "DataID", text: "Data ID", cls: "span3 full", type: "select2", datasource: "DataID" },
                    {
                        type: "buttons", cls: "span2", items: [
                             { name: "Retrieve", text: "Retrieve", icon: "", click: "Retrieve()", cls: "button small btn btn-success" },
                        ]
                    },
                    {type :"hr"},
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
            },
        ]
    };

    Wx = new SimDms.Widget(options);
    Wx.default = {};
    Wx.render(init);



    function init(s) {
        $(".mylabel").attr("style", "padding:9px 9px 0px 5px");
        SimDms.Angular("InquiryDataDCS");
    }



});