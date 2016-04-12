var totalSrvAmt = 0;
var status = 'N';
var svType = '2';

"use strict";

function gnMasterTaxController($scope, $http, $injector) {

    var me = $scope;

    $injector.invoke(BaseController, this, { $scope: me });

    me.browse = function () {
        var lookup = Wx.blookup({
            name: "MasterReminderBrowse",
            title: "Reminder Browse",
            manager: gnManager,
            query: "ReminderBrowse",
            defaultSort: "ReminderKey asc",
            columns: [
            { field: "ReminderKey", title: "Kode", width: 150 },
            { field: "ReminderDescription", title: "Deskripsi", width: 200 },
             { field: "ReminderDate", title: "Tanggal Pengingat", width: 200, template: "#= (ReminderDate == undefined) ? '' : moment(ReminderDate).format('DD MMM YYYY') #" },
             { field: "ReminderTimePeriod", title: "Period", width: 150 },
            { field: "ReminderTimeDim", title: "Type", width: 200 },
             { field: "ReminderStatus", title: "Status", width: 200 },
            ]
        });
        lookup.dblClick(function (data) {
            if (data != null) {
                me.lookupAfterSelect(data);
                //me.GetCustomerInfo(data.CustomerCode);
                if ( me.data.ReminderStatus == true)
                    $('#ReminderStatus').prop('checked', true);
                else $('#ReminderStatus').prop('checked', false);
                me.isSave = false;
                me.Apply();
            }
        });
    }

    me.initialize = function () {
        $('#ReminderTimePeriod').css("text-align", "right");
        me.data.ReminderTimeDim = "D";
        me.data.ReminderCode = "BPK";
        me.data.ReminderDate = me.now();
        me.hasChanged = false;
    }

    me.delete = function () {
        MsgConfirm("Are you sure to delete current record?", function (result) {
            if (result) {
                $http.post('gn.api/Reminder/Delete', me.data).
                success(function (data, status, headers, config) {
                    if (data.success) {
                        me.init();
                        Wx.Success("Data deleted...");
                    } else {
                        MsgBox(data.message, MSG_ERROR);
                    }
                }).
                error(function (data, status, headers, config) {
                    MsgBox('Terjadi Kesalahan, Hubungi SDMS Support');
                });
            }
        });
    }

    me.saveData = function (e, param) {
        if ($('#ReminderStatus').prop('checked') == true)
            me.data.ReminderStatus = 1;
        else me.data.ReminderStatus = 0;

        $http.post('gn.api/Reminder/Save', me.data).
            success(function (data, status, headers, config) {
                if (data.status) {
                    Wx.Success("Data saved...");
                    me.startEditing();
                } else {
                    MsgBox(data.message, MSG_ERROR);
                }
            }).
            error(function (data, status, headers, config) {
                MsgBox('Terjadi Kesalahan, Hubungi SDMS Support');
            });
    };

    me.$watch('data.ReminderCode', function (a, b) {
        if (a == "RELIGION") {
            $('#ReminderDate').removeAttr('disabled');
        } else {
            $('#ReminderDate').attr('disabled', true);
        }
    }, true);

    me.start();
}

$(document).ready(function () {
    var options = {
        title: "Reminder",
        xtype: "panels",
        toolbars: WxButtons,
        panels: [
            {
                name: "DataCustomerClass",
                title: "Setting Reminder",
                items: [
                    {
                        type: "optionbuttons",
                        name: "ReminderCode",
                        model: "data.ReminderCode",
                        text: "Reminder Code",
                        items: [
                            { name: "BPK", text: "Tgl. BPK" },
                            { name: "BIRTH", text: "Ulang Tahun" },
                            { name: "RELIGION", text: "Hari Raya" },
                        ]
                    },
                    { name: "ReminderDescription", model: "data.ReminderDescription", text: "Description", cls: "span4 full", validasi: "required", type: "text" },
                     { name: 'ReminderDate', type: 'ng-datepicker', text: 'Reminder Date ', cls: 'span4', disable: true },
                     {
                         type: "controls",
                         cls: "span8 full",
                         text: "Interval",
                         required: true,
                         items: [
                             { name: "ReminderTimePeriod", cls: "span1", type: "text", validasi: "required", placeHolder: "0"},
                              {
                                  type: "optionbuttons",
                                  name: "ReminderTimeDim",
                                  model: "data.ReminderTimeDim",
                                  items: [
                                      { name: "D", text: "Hari" },
                                      { name: "M", text: "Bulan" },
                                      { name: "Y", text: "Tahun" },
                                  ]
                              },
                         ]
                     },
                    {
                        type: "controls",
                        cls: "span4 full",
                        text: "Reminder Status",
                        items: [
                            { name: "ReminderStatus", cls: "span1", type: "check" },
                            { type: "label", text: "Aktif", cls: "span7 mylabel" },
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
        SimDms.Angular("gnMasterTaxController");
    }




});