var totalSrvAmt = 0;
var status = 'N';
var svType = '2';

"use strict";

function svDailyRetention($scope, $http, $injector) {

    var me = $scope;
    var RetentionNo = "";
    var CustomerCode = "";
    var PeriodYear = "";
    var PeriodMonth = "";

    $injector.invoke(BaseController, this, { $scope: me });

    me.initialize = function () {
        me.detail = {};

        me.data.Trans = "0";
        me.detail.IsReminder = "0";
        me.detail.IsFollowUp = "0";
        me.detail.IsConfirmed = "0";
        me.detail.IsBooked = "0";
        me.detail.IsVisited = "0";
        me.detail.IsSatisfied = "0";
        me.detail.StatisfyReasonGroup = "T";

        me.detail.ReminderDate = me.now();
        me.detail.FollowUpDate = me.now();
        me.detail.BookingDate = me.now();
        //console.log(me.detail.ReminderDate);
        me.data.DateReminder = me.now();
        me.data.DateFollowUp = me.now();
        me.data.Month = "3";
        me.data.Days = "3";
        me.data.Interval = "0";

        $('#btnSave').attr('disabled', 'disabled');

        me.clearTable(me.gridTransaksi);
        me.gridTransaksi.adjust();
        me.detailKen = {};

        $('#IsOdom').attr('checked', true);
        me.data.IsOdom = true;
    }

    me.$watch('data.Trans', function (newValue, oldValue) {
        if (newValue !== oldValue) {
            me.$broadcast(newValue);
            me.data.Trans = newValue;
        }
    });

    $http.post('sv.api/DailyServiceRetention/isCallInformation?RefferenceType=RSNC').
    success(function (data, status, headers, config) {
        me.CannotCallCode = data;
    });

    $http.post('sv.api/DailyServiceRetention/Kedatangan?CodeID=CIRS').
    success(function (data, status, headers, config) {
        me.VisitInitial = data;
    });

    me.$watch('detail.StatisfyReasonGroup', function (newValue2, oldValue2) {
        var RefferenceType = "";
        if (newValue2 !== oldValue2) {
            me.$broadcast(newValue2);
            if (newValue2 == "T") {
                RefferenceType = "RSNT";
            } else {
                RefferenceType = "RSNP";
            }
            $http.post('sv.api/DailyServiceRetention/isTidakPuasan?RefferenceType=' + RefferenceType).
            success(function (data, status, headers, config) {
                me.StatisfyReasonCode = data;
            });
        }
    });

    $("#IsOdom").on('change', function () {
        if ($('#IsOdom').is(':checked')) {
            me.data.IsOdom = true;
            me.data.Month = 3;
        }
        else {
            me.data.IsOdom = false;
            me.data.Month = 3;
        }
        me.Apply();
    });


    me.Exclude = function () {
        var datas;
        var inclPDI = false;
        //if (me.data.Trans == "0") {
        //    datas = {
        //        "date": me.data.DateReminder,
        //        "optionType": "REMINDER",
        //        "interval": me.data.Interval,
        //        "range": me.data.Month,
        //        "InclPdi": inclPDI            }
        //} else {
        //    datas = {
        //        "date": me.data.DateFollowUp,
        //        "optionType": "4FOLLOWUP",
        //        "interval": me.data.Days,
        //        "range": -1,
        //        "InclPdi": inclPDI
        //    }
        //}

        datas = {
            "date": me.data.Trans == "0" ? me.data.DateReminder : me.data.DateFollowUp,
            "optionType": me.data.Trans == "0" ? "REMINDER" : "4FOLLOWUP",
            //"interval": me.data.Trans == "0" ? me.data.Interval : "0",
            //"range": me.data.Month,
            "interval": me.data.Trans == "0" ? me.data.Interval : me.data.Days,
            "range": me.data.Trans == "0" ? me.data.Month : "-1",
            "InclPdi": inclPDI,
            "IsOdom": me.data.IsOdom
        }

        $http.post('sv.api/DailyServiceRetention/PopulateData', datas).
         success(function (data, status, headers, config) {
             me.loadTableData(me.gridTransaksi, data);
         }).
         error(function (e, status, headers, config) {
             MsgBox(e.message, MSG_ERROR);
         });
    }

    me.Include = function () {
        var datas;
        var inclPDI = true;
        //if (me.data.Trans == "0") {
        //    datas = {
        //        "date": me.data.DateReminder,
        //        "optionType": "REMINDER",
        //        "interval": me.data.Interval,
        //        "range": me.data.Month,
        //        "InclPdi": inclPDI
        //    }
        //} else {
        //    datas = {
        //        "date": me.data.DateFollowUp,
        //        "optionType": "4FOLLOWUP",
        //        "interval": me.data.Days,
        //        "range": -1,
        //        "InclPdi": inclPDI
        //    }
        //}

        datas = {
            "date": me.data.Trans == "0" ? me.data.DateReminder : me.data.DateFollowUp,
            "optionType": me.data.Trans == "0" ? "REMINDER" : "4FOLLOWUP",
            //"interval": me.data.Trans == "0" ? me.data.Interval : "0",
            //"range": me.data.Month,
            "interval": me.data.Trans == "0" ? me.data.Interval : me.data.Days,
            "range": me.data.Trans == "0" ? me.data.Month : "-1",
            "InclPdi": inclPDI,
            "IsOdom": me.data.IsOdom
        }

        $http.post('sv.api/DailyServiceRetention/PopulateData', datas).
         success(function (data, status, headers, config) {
             me.loadTableData(me.gridTransaksi, data);
         }).
         error(function (e, status, headers, config) {
             MsgBox(e.message, MSG_ERROR);
         });
    }

    me.followUpClick = function () {
        if (me.data.Trans == "1")
            me.detail.FollowUpDate = me.now();

    }

    me.reminderClick = function () {
        if (me.data.Trans == "0")
            me.detail.ReminderDate = me.now();

    }

    me.gridTransaksi = new webix.ui({
        container: "GridTransaksi",
        view: "wxtable",
        scrollX: true,
        scrollY: true,
        height: 350,
        autoHeight: false,
        autoConfig: true,
        leftSplit: 4,
        scheme: {
            $change: function (item) {
                if (me.data.Trans == "0") {
                    if (item.IsReminder != undefined) {
                        if (item.IsReminder === "YA")
                            item.$css = "followup-color";
                    }
                }
                else {
                    if (item.IsFollowUp != undefined) {
                        if (item.IsFollowUp === "YA")
                            item.$css = "followup-color";
                    }
                }
            }
        },
        columns: [
            { id: "CustomerCode", header: "Cust Code", width: 100, css: "split-color" },
            { id: "CustomerName", header: "Customer Name", width: 175, css: "split-color" },
            { id: "JobOrderNo", header: "No. SPK", width: 115, css: "split-color" },
            { id: "JobOrderDate", header: "SPK Date", width: 105, format: me.dateFormat, css: "split-color" },
            { id: "ContactName", header: "Contact Name", width: 155 },
            { id: "PhoneNo", header: "Phone No", width: 145 },
            { id: "BasicModel", header: "Basic Model", width: 125 },
            { id: "TransmissionType", header: "TM", width: 85 },
            { id: "ChassisCode", header: "Chassis Code", width: 125 },
            { id: "ChassisNo", header: "Chassis No", width: 105 },
            { id: "PoliceRegNo", header: "Police Registration", width: 175 },
            { id: "Odometer", header: "Odometer", width: 105 },
            { id: "LastServiceDate", header: "Last Service", width: 140, format: me.dateFormat },
            { id: "JobType", header: "Job Type", width: 125 },
            { id: "Remark", header: "Service Request", width: 275 },
            { id: "FollowUpDate", header: "Tgl FollowUp", width: 105, format: me.dateFormat },
            { id: "VisitInitialDesc", header: "Initial Kedatangan", width: 145 },
            { id: "RetentionNo", header: "No. Retention", width: 125 },
            { id: "IsConfirmed", header: "Hubungi", width: 105 },
            { id: "ReminderDate", header: "Tgl Reminder", width: 115, format: me.dateFormat },
            { id: "IsBooked", header: "Booking", width: 105 },
            { id: "BookingDate", header: "Tgl Booking", width: 105, format: me.dateFormat },
            { id: "IsVisited", header: "Datang", width: 105 },
            { id: "IsSatisfied", header: "Puas", width: 105 },
            { id: "Reason", header: "Alasan", width: 275 },
            { id: "Address", header: "Alamat", width: 525 },
            { id: "IsFollowUp", header: "", width: -1 },
            { id: "ContactName", header: "Nama Contact", width: 175 },
            { id: "ContactAddress", header: "Alamat Contact", width: 325 },
            { id: "ContactPhone", header: "Telephone Contact", width: 175 },
            { id: "MobilePhone", header: "Handphone No", width: 145 },
            { id: "AdditionalPhone1", header: "Additional Phone 1", width: 145 },
            { id: "AdditionalPhone2", header: "Additional Phone 2", width: 145 },
        ],
        on: {
            onSelectChange: function () {
                if (me.gridTransaksi.getSelectedId() !== undefined) {
                    Idx = me.gridTransaksi.getSelectedId().id;

                    $('#btnSave').removeAttr('disabled');
                    var data = this.getItem(me.gridTransaksi.getSelectedId().id);

                    PeriodYear = data.PeriodYear;
                    PeriodMonth = data.PeriodMonth;
                    RetentionNo = data.RetentionNo;
                    CustomerCode = data.CustomerCode;
                    var Kendaraan = {
                        "Year": data.PeriodYear,
                        "Month": data.PeriodMonth,
                        "RetentionNo": data.RetentionNo,
                        "CustomerCode": data.CustomerCode
                    }

                    var Customer = {
                        "CustomerCode": data.CustomerCode
                    }

                    $http.post('sv.api/DailyServiceRetention/GetKendaraan', Kendaraan)
                    .success(function (e) {
                        if (e.success) {
                            me.detailKen = e.data[0];
                            me.detail = e.data[0];
                            me.detail.IsReminder = e.data[0].IsReminder == true ? "1" : "0";
                            me.detail.IsFollowUp = e.data[0].IsFollowUp == true ? "1" : "0";
                            me.detail.IsConfirmed = e.data[0].IsConfirmed == true ? "1" : "0";
                            me.detail.IsBooked = e.data[0].IsBooked == true ? "1" : "0";
                            me.detail.IsVisited = e.data[0].IsVisited == true ? "1" : "0";
                            me.detail.IsSatisfied = e.data[0].IsSatisfied == true ? "1" : "0";
                        } else {
                            MsgBox(e.message, MSG_ERROR);
                        }
                    })
                    .error(function (e) {
                        MsgBox('Error pada saat get data kendaraan!', MSG_ERROR);
                    });

                    $http.post('sv.api/DailyServiceRetention/GetCustomer', Customer)
                    .success(function (e) {
                        if (e.success) {
                            me.detailCust = e.data[0];
                        } else {
                            MsgBox(e.message, MSG_ERROR);
                        }
                    })
                    .error(function (e) {
                        MsgBox('Error pada saat get data kendaraan!', MSG_ERROR);
                    });
                }
            }
        }
    });

    me.printPreview = function () {
        var data;
        if (me.data.Trans == "0") {
            data = {
                date: me.data.DateReminder,
                optionType: "REMINDER",
                interval: me.data.Interval,
                range: me.data.Month
            }
        } else {
            data = {
                date: me.data.DateFollowUp,
                optionType: "4FOLLOWUP",
                interval: me.data.Days,
                range: -1
            }
        }
        Wx.loadForm();
        Wx.showForm({ url: "sv/report/printdialogretention", params: JSON.stringify(data) });
    }

    webix.event(window, "resize", function () {
        me.gridTransaksi.adjust();
    })

    me.saveRDH = function (e, param) {
        var model = {
            "IsReminder": me.detail.IsReminder == "1" ? true : false,
            "ReminderDate": me.detail.ReminderDate,
            "IsFollowUp": me.detail.IsFollowUp == "1" ? true : false,
            "FollowUpDate": me.detail.FollowUpDate,
            "IsBooked": me.detail.IsBooked == "1" ? true : false,
            "BookingDate": me.detail.BookingDate,
            "IsConfirmed": me.detail.IsConfirmed == "1" ? true : false,
            "CannotCallCode": me.detail.CannotCallCode,
            "IsVisited": me.detail.IsVisited == "1" ? true : false,
            "VisitInitial": me.detail.VisitInitial,
            "IsSatisfied": me.detail.IsSatisfied == "1" ? true : false,
            "StatisfyReasonGroup": me.detail.StatisfyReasonGroup,
            "StatisfyReasonCode": me.detail.StatisfyReasonCode,
            "IsClosed": me.detail.IsClosed == "1" ? true : false,
            "Reason": me.detail.Reason,
        };
        $http.post('sv.api/DailyServiceRetention/SaveDataRDH', { PeriodYear: PeriodYear, PeriodMonth: PeriodMonth, RetentionNo: RetentionNo, CustomerCode: CustomerCode, model: model }).
            success(function (data, status, headers, config) {
                if (data.success) {
                    Wx.Success(data.message);
                    me.startEditing();
                    var sel = me.gridTransaksi.getSelectedId();

                    if (me.data.Trans == "0") {
                        console.log(me.detail.IsReminder);
                        if (me.detail.IsReminder == 1) {
                            if (!sel) return;
                            me.gridTransaksi.addRowCss(sel.row, "followup-color");
                        }
                        else {
                            if (!sel) return;
                            var item = me.gridTransaksi.getItem(sel.row);
                            var dat = item;
                            if (item.$css != undefined) {
                                item.$css = "";
                                me.gridTransaksi.updateItem(sel.row, item);
                                dat.$css = "";
                                if (item.$css == undefined || item.$css == "") {
                                    me.gridTransaksi.removeRowCss(sel.row, "followup-color");
                                }
                            }
                            else if (item.$css == undefined || item.$css == "") {
                                me.gridTransaksi.removeRowCss(sel.row, "followup-color");
                            }
                            else {
                                me.gridTransaksi.removeRowCss(sel.row, "followup-color");
                            }
                        }
                    }
                    else {
                        if (me.detail.IsFollowUp == 1) {
                            if (!sel) return;
                            me.gridTransaksi.addRowCss(sel.row, "followup-color");
                        }
                        else {
                            if (!sel) return;
                            var item = me.gridTransaksi.getItem(sel.row);
                            var dat = item;
                            if (item.$css != undefined) {
                                item.$css = "";
                                me.gridTransaksi.updateItem(sel.row, item);
                                dat.$css = "";
                                if (item.$css == undefined || item.$css == "") {
                                    me.gridTransaksi.removeRowCss(sel.row, "followup-color");
                                }
                            }
                            else if (item.$css == undefined || item.$css == "") {
                                me.gridTransaksi.removeRowCss(sel.row, "followup-color");
                            }
                            else {
                                me.gridTransaksi.removeRowCss(sel.row, "followup-color");
                            }
                        }
                    }

                    $('#btnSave').attr('disabled', 'disabled');
                } else {
                    MsgBox(data.message, MSG_ERROR);
                }
            }).
            error(function (e, status, headers, config) {
                MsgBox('Terjadi Kesalahan, Hubungi SDMS Support');
                console.log(e);
            });
    }

    me.start();
}


$(document).ready(function () {
    var options = {
        title: "Data Retensi Harian",
        xtype: "panels",
        toolbars: [
            { name: "btnSave", text: "Save", cls: "btn btn-success", icon: "icon-save", click: "saveRDH()" },
            { name: "btnPrintPreview", text: "Print", cls: "btn btn-primary", icon: "icon-print", click: "printPreview()" },
            { name: "btnCancel", text: "Cancel", cls: "btn btn-warning", icon: "icon-remove", click: "cancelOrClose()" },
        ],
        panels: [
            {
                name: "pnlDailyRetention",
                title: "Data Retensi Harian",
                items: [
                    {
                        type: "optionbuttons", name: "Trans", model: "data.Trans",
                        items: [
                            { name: "0", text: "Reminder" },
                            { name: "1", text: "Follow Up" },
                        ]
                    },
                    { name: "DateReminder", text: "Reminder", cls: "span4", type: "ng-datepicker", disable: "data.Trans == 1" },
                    { name: "Month", cls: "span2", text: "Month", disable: "data.Trans == 1  || data.IsOdom == 1" },
                    { name: "Interval", cls: "span2", text: "Interval", disable: "data.Trans == 1" },
                    { name: "DateFollowUp", text: "Follow Up", cls: "span4", type: "ng-datepicker", disable: "data.Trans == 0" },
                    { name: "Days", cls: "span2", text: "Days", disable: "data.Trans == 0" },
                    {
                        type: "buttons",
                        cls: "span4",
                        items: [
                             { name: "Exclude", text: "Inquiry Exclude PDI", icon: "", cls: "span4", click: "Exclude()" },
                             { name: "Include", text: "Inquiry Include PDI", icon: "", cls: "span4", click: "Include()" },
                        ]
                    },
                    { name: "IsOdom", text: "Odom", cls: "span4", type: "check" }
                ]
            },
            {
                name: "GridTransaksi",
                xtype: "wxtable"
            },
            {
                xtype: "tabs",
                name: "tabpage1",
                items: [
                    { name: "tA", text: "Retention", cls: "active" },
                    { name: "tB", text: "Kendaraan" },
                    { name: "tC", text: "Pelanggan" },
                ]
            },
            {
                title: "",
                cls: "tabpage1 tA",
                items: [
                    {
                        type: "optionbuttons", text: "Reminder?", name: "IsReminder", model: "detail.IsReminder", show: "data.Trans == 0",
                        items: [
                            {
                                name: "1", text: "Ya", show: "data.Trans == 0",
                                click: "reminderClick()"
                            },
                            { name: "0", text: "Tidak", show: "data.Trans == 0" },
                        ]
                    },
                    { name: "ReminderDate", model: "detail.ReminderDate", text: "Tanggal Remainder", cls: "span4", type: "ng-datepicker", show: "data.Trans == 0 && detail.IsReminder == 1" },
                    {
                        type: "optionbuttons", text: "Follow Up?", name: "IsFollowUp", model: "detail.IsFollowUp", show: "data.Trans == 1",
                        items: [
                            {
                                name: "1", text: "Ya", show: "data.Trans == 1",
                                click: "followUpClick()"
                            },
                            { name: "0", text: "Tidak", show: "data.Trans == 1" },
                        ]
                    },
                    { name: "FollowUpDate", model: "detail.FollowUpDate", text: "Tanggal Follow Up", cls: "span4", type: "ng-datepicker", show: "data.Trans == 1 && detail.IsFollowUp == 1" },
                    {
                        type: "optionbuttons", text: "Customer Booking?", name: "IsBooked", model: "detail.IsBooked", show: "data.Trans == 0",
                        items: [
                            { name: "1", text: "Ya", show: "data.Trans == 0" },
                            { name: "0", text: "Tidak", show: "data.Trans == 0" },
                        ]
                    },
                    { name: "BookingDate", model: "detail.BookingDate", text: "Tanggal Booking", cls: "span4", type: "ng-datepicker", show: "data.Trans == 0 && detail.IsBooked == 1" },
                    {
                        type: "optionbuttons", text: "Berhasi Di Hubungi?", name: "IsConfirmed", model: "detail.IsConfirmed", show: "data.Trans == 0",
                        items: [
                            { name: "1", text: "Ya", show: "data.Trans == 0" },
                            { name: "0", text: "Tidak", show: "data.Trans == 0" },
                        ]
                    },
                    { name: "CannotCallCode", model: "detail.CannotCallCode", cls: "span4", text: "Informasi", type: "select2", datasource: "CannotCallCode", show: "data.Trans == 0 && detail.IsConfirmed == 0" },
                    {
                        type: "optionbuttons", text: "Customer Datang?", name: "IsVisited", model: "detail.IsVisited", show: "data.Trans == 1",
                        items: [
                            { name: "1", text: "Ya", show: "data.Trans == 1" },
                            { name: "0", text: "Tidak", show: "data.Trans == 1" },
                        ]
                    },
                    { name: "VisitInitial", text: "Initial Kedatangan", model: "detail.VisitInitial", cls: "span4", type: "select2", datasource: "VisitInitial", show: "data.Trans == 1 && detail.IsVisited == 1" },
                    {
                        type: "optionbuttons", text: "Kepuasan Customer", name: "IsSatisfied", model: "detail.IsSatisfied", show: "data.Trans == 1",
                        items: [
                            { name: "1", text: "Ya", show: "data.Trans == 1" },
                            { name: "0", text: "Tidak", show: "data.Trans == 1" },
                        ]
                    },
                    {
                        type: "optionbuttons", text: "Alasan Tidak Puas?", name: "StatisfyReasonGroup", model: "detail.StatisfyReasonGroup", show: "data.Trans == 1 && detail.IsSatisfied == 0",
                        items: [
                            { name: "T", text: "Teknis", show: "data.Trans == 1 && detail.IsSatisfied == 0" },
                            { name: "P", text: "Pelayanan", show: "data.Trans == 1 && detail.IsSatisfied == 0" },
                        ]
                    },
                    { name: "StatisfyReasonCode", model: "detail.StatisfyReasonCode", cls: "span4", text: "Informasi", cls: "span4", type: "select2", datasource: "StatisfyReasonCode", show: "data.Trans == 1 && detail.IsSatisfied == 0" },
                    {
                        type: "controls",
                        cls: "span4",
                        show: "data.Trans == 1 && detail.IsSatisfied == 0",
                        items: [
                            { name: "IsClosed", model: "detail.IsClosed", cls: "span1", type: "ng-check" },
                            { type: "label", text: "Closed ?", cls: "span1" },
                        ]
                    },
                    { name: "Reason", model: "detail.Reason", text: "Keterangan", type: "textarea", show: "data.Trans == 1" },
                ]
            },
            {
                title: "",
                cls: "tabpage1 tB",
                items: [
                    { name: "PoliceRegNo", model: "detailKen.PoliceRegNo", text: "No. Polisi", cls: "span4", readonly: true },
                    {
                        text: "Basic Model/ Tech",
                        type: "controls",
                        cls: "span4",
                        items: [
                            { name: "BasicModel", model: "detailKen.BasicModel", text: "", cls: "span4", readonly: true },
                            { name: "TechnicalModel", model: "detailKen.TechnicalModel", text: "", cls: "span4", readonly: true },
                        ]
                    },
                    {
                        text: "Kode/No. Rangka",
                        type: "controls",
                        cls: "span4",
                        items: [
                            { name: "ChassisCode", model: "detailKen.ChassisCode", text: "", cls: "span4", readonly: true },
                            { name: "ChassisNo", model: "detailKen.ChassisNo", text: "", cls: "span4", readonly: true },
                        ]
                    },
                    { name: "ColourName", model: "detailKen.ColourName", text: "Warna Kendaraan", cls: "span4", readonly: true },
                    {
                        text: "Kode/No. Mesin",
                        type: "controls",
                        cls: "span4",
                        items: [
                            { name: "EngineCode", model: "detailKen.EngineCode", text: "", cls: "span4", readonly: true },
                            { name: "EngineNo", model: "detailKen.EngineNo", text: "", cls: "span4", readonly: true },
                        ]
                    },
                    { name: "Transmission", model: "detailKen.Transmission", text: "Transmisi", cls: "span2", readonly: true },
                    { name: "ProductionYear", model: "detailKen.ProductionYear", text: "Tahun", cls: "span2", readonly: true },
                    { name: "LastRemark", model: "detailKen.LastRemark", text: "Keterangan", type: "textarea", readonly: true },
                    { name: "Odometer", model: "detailKen.Odometer", text: "KM(Odometer)", cls: "span3", readonly: true },
                    { name: "LastServiceDate", model: "detailKen.LastServiceDate", text: "Tgl. Terakhir Service", cls: "span3", type: "ng-datepicker", readonly: true },
                    { name: "PMNow", model: "detailKen.PMNow", text: "#PM", cls: "span3", readonly: true },
                    { name: "RefferenceDate", model: "detailKen.RefferenceDate", text: "Tgl. Pengiriman", cls: "span3", type: "ng-datepicker", readonly: true },
                    { name: "PMNext", model: "detailKen.PMNext", text: "#PM Berikutnya", cls: "span3", readonly: true },
                ]
            },
            {
                title: "",
                cls: "tabpage1 tC",
                items: [
                    { name: "CustCategory", model: "detailKen.CustCategory", text: "Kategori", cls: "span3 full", readonly: true },
                    { name: "CustomerName", model: "detailCust.CustomerName", text: "Nama", cls: "span4 ", readonly: true },
                    {
                        type: "controls",
                        cls: "span4 ",
                        items: [
                            {
                                type: "optionbuttons", cls: "span2", text: "", name: "Gender", model: "detailCust.Gender",
                                items: [
                                    { name: "M", text: "L" },
                                    { name: "F", text: "P" },
                                ]
                            },
                            { type: "label", text: "Tgl.Lahir", cls: "span2 mylabel" },
                            { name: "BirthDate", model: "detailCust.BirthDate", text: "Tgl. Lahir", cls: "span4", type: "ng-datepicker", readonly: true },
                        ]
                    },
                    {
                        text: "Kota",
                        type: "controls",
                        cls: "span4 ",
                        items: [
                            { name: "CityCode", model: "detailCust.CityCode", text: "", cls: "span3", readonly: true },
                            { name: "City", model: "detailCust.City", text: "", cls: "span5", readonly: true },
                        ]
                    },
                    {
                        text: "Telepon",
                        type: "controls",
                        cls: "span4 ",
                        items: [
                            { name: "OfficePhoneNo", model: "detailCust.OfficePhoneNo", text: "", cls: "span3", readonly: true },
                            { type: "label", text: "/", cls: "span2 mylabel" },
                            { name: "PhoneNo", model: "detailCust.PhoneNo", text: "", cls: "span3", readonly: true },
                        ]
                    },
                    { name: "HPNo", model: "detailCust.HPNo", text: "HP", cls: "span4 ", readonly: true },
                    {type: "hr"},
                    { name: "Address1", model: "detailCust.Address1", text: "Alamat", cls: "span4", readonly: true },
                    { name: "Address2", model: "detailCust.Address2", text: "", cls: "span4", readonly: true },
                    { name: "Address3", model: "detailCust.Address3", text: "", cls: "span4", readonly: true },
                    { name: "Address4", model: "detailCust.Address4", text: "", cls: "span4", readonly: true },
                ]
            },
        ]
    };

    Wx = new SimDms.Widget(options);
    Wx.default = {};
    Wx.render(init);

    function init(s) {
        $(".mylabel").attr("style", "padding:9px 9px 5px 5px", "align: center");
        SimDms.Angular("svDailyRetention");
    }

});