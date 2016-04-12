var totalSrvAmt = 0;
var status = 'N';
var svType = '2';

"use strict";

function svDailyRetention($scope, $http, $injector) {

    var me = $scope;

    $injector.invoke(BaseController, this, { $scope: me });

    me.initialize = function () {
        me.data.isCheck = false;
        me.data.isCheck1 = false;
        me.data.isCheck2 = false;
        me.data.isCheck3 = false;

        $http.post('sv.api/PrintRetention/Default').
        success(function (data) {
            me.data.ServiceDateFrom = data.ServiceDateFrom;
            me.data.ServiceDateTo = data.ServiceDateTo;
            me.data.ReminderDateFrom = data.ReminderDateFrom;
            me.data.ReminderDateTo = data.ReminderDateTo;
            me.data.FollowUpDateFrom = data.FollowUpDateFrom;
            me.data.FollowUpDateTo = data.FollowUpDateTo;
            me.data.Priode = data.Priode;
        }).
        error(function (e, status, headers, config) {
            MsgBox('Terjadi Kesalahan, Hubungi SDMS Support');
            console.log(e);
        });

        me.data.Trans = "0";
        var params = localStorage.getItem('params');

        me.data.date = JSON.parse(params).date;
        me.data.optionType = JSON.parse(params).optionType;
        me.data.interval = JSON.parse(params).interval;
        me.data.range = JSON.parse(params).range;
    }

    me.Print = function () {
        var url;
        var params
        if (me.data.Trans == "0") {
            url = "sv.api/PrintRetention/SvRpCrm002V2?";
            params = "DateParam=" + moment(me.data.date).format('YYYY-MM-DD');
            params += "&OptionType=" + me.data.optionType;
            params += "&Range=" + me.data.range;
            params += "&Interval=" + me.data.interval;
            params += "&Priode=" + moment(me.now()).format('MMMM YYYY');
            url = url + params;
            window.location = url;
        } else if (me.data.Trans == "1" || me.data.Trans == "2") {
            var ServiceDateFrom = (me.data.isCheck1 == false) ? '' : moment(me.data.ServiceDateFrom).format('YYYYMMDD');
            var ServiceDateTo = (me.data.isCheck1 == false) ? '' : moment(me.data.ServiceDateTo).format('YYYYMMDD');
            var ReminderDateFrom = (me.data.isCheck2 == false) ? '' : moment(me.data.ReminderDateFrom).format('YYYYMMDD');
            var ReminderDateTo = (me.data.isCheck2 == false) ? '' : moment(me.data.ReminderDateTo).format('YYYYMMDD');
            var FollowUpDateFrom = (me.data.isCheck3 == false) ? '' : moment(me.data.FollowUpDateFrom).format('YYYYMMDD');
            var FollowUpDateTo = (me.data.isCheck3 == false) ? '' : moment(me.data.FollowUpDateTo).format('YYYYMMDD');


                url = "sv.api/PrintRetention/SvRpCrm003V2?";
                params = "&DateParam=" + moment(me.data.date).format('YYYY-MM-DD');
                params += "&OptionType=" + me.data.optionType;
                params += "&Range=" + me.data.range;
                params += "&Interval=" + me.data.interval;
                params += "&IncPDI=" + me.data.isCheck;
                params += "&ServiceDateFrom=" + ServiceDateFrom;
                params += "&ServiceDateTo=" + ServiceDateTo;
                params += "&ReminderDateFrom=" + ReminderDateFrom;
                params += "&ReminderDateTo=" + ReminderDateTo;
                params += "&FollowUpDateFrom=" + FollowUpDateFrom;
                params += "&FollowUpDateTo=" + FollowUpDateTo;
                params += "&PrintOption=" + me.data.Trans;
                url = url + params;
                window.location = url;
        }
        else if (me.data.Trans == "3") {
            var Priode = new Date(me.data.Priode).getFullYear();
            //url = "sv.api/PrintRetention/SvRpCrm004?";
            //params = "&Year=" + Priode;
            //url = url + params;
            //window.location = url;

            Wx.XlsxReport({
                url: 'sv.api/PrintRetention/SvRpCrm004',
                type: 'xlsx',
                params: { Year: Priode }
            });
        }
    }

    me.resetChecked = function () {
        me.data.isCheck = false;
        me.data.isCheck1 = false;
        me.data.isCheck2 = false;
        me.data.isCheck3 = false;
    }

    me.$watch('data.Trans', function (newValue, oldValue) {
        if (newValue !== oldValue) {
            me.$broadcast(newValue);
            if (newValue == "0") 
            {
                $("#isCheck").attr('disabled', 'disabled');

                $("#isCheck1").attr('disabled', 'disabled');
                $("#ServiceDateFrom").attr('disabled', 'disabled');
                $("#ServiceDateTo").attr('disabled', 'disabled');

                $("#isCheck2").attr('disabled', 'disabled');
                $("#ReminderDateFrom").attr('disabled', 'disabled');
                $("#ReminderDateTo").attr('disabled', 'disabled');

                $("#isCheck3").attr('disabled', 'disabled');
                $("#FollowUpDateFrom").attr('disabled', 'disabled');
                $("#FollowUpDateTo").attr('disabled', 'disabled');

                $("#Priode").attr('disabled', 'disabled');
            }
            else if (newValue == "1")
            {
                $("#isCheck").removeAttr('disabled');

                $("#isCheck1").attr('disabled', 'disabled');
                $("#ServiceDateFrom").attr('disabled', 'disabled');
                $("#ServiceDateTo").attr('disabled', 'disabled');

                $("#isCheck2").attr('disabled', 'disabled');
                $("#ReminderDateFrom").attr('disabled', 'disabled');
                $("#ReminderDateTo").attr('disabled', 'disabled');

                $("#isCheck3").attr('disabled', 'disabled');
                $("#FollowUpDateFrom").attr('disabled', 'disabled');
                $("#FollowUpDateTo").attr('disabled', 'disabled');

                $("#Priode").attr('disabled', 'disabled');
            }
            else if (newValue == "2")
            {
                $("#isCheck").attr('disabled', 'disabled');

                $("#isCheck1").removeAttr('disabled');
                $("#ServiceDateFrom").removeAttr('disabled');
                $("#ServiceDateTo").removeAttr('disabled');

                $("#isCheck2").removeAttr('disabled');
                $("#ReminderDateFrom").removeAttr('disabled');
                $("#ReminderDateTo").removeAttr('disabled');

                $("#isCheck3").removeAttr('disabled');
                $("#FollowUpDateFrom").removeAttr('disabled');
                $("#FollowUpDateTo").removeAttr('disabled');

                $("#Priode").attr('disabled', 'disabled');
            }
            else if (newValue == "3")
            {
                $("#isCheck").attr('disabled', 'disabled');

                $("#isCheck1").attr('disabled', 'disabled');
                $("#ServiceDateFrom").attr('disabled', 'disabled');
                $("#ServiceDateTo").attr('disabled', 'disabled');

                $("#isCheck2").attr('disabled', 'disabled');
                $("#ReminderDateFrom").attr('disabled', 'disabled');
                $("#ReminderDateTo").attr('disabled', 'disabled');

                $("#isCheck3").attr('disabled', 'disabled');
                $("#FollowUpDateFrom").attr('disabled', 'disabled');
                $("#FollowUpDateTo").attr('disabled', 'disabled');

                $("#Priode").removeAttr('disabled');
            }
        }
    });

    me.start();
}

$(document).ready(function () {
    var options = {
        title: "Print Report Retention",
        xtype: "panels",
        panels: [
            {
                title: "Jenis Report",
                name: "pnlPrint",
                items: [
                    {
                        type: "optionbuttons", name: "Trans", model: "data.Trans",
                        items: [
                            { name: "0", text: "Master Pelanggan Service", click: "resetChecked()" },
                            { name: "1", text: "Data Retensi Harian", click: "resetChecked()" },
                            { name: "2", text: "Data Retensi Harian Bedasarkan Priode", click: "resetChecked()" },
                            { name: "3", text: "Laporan Tindak Lanjut Pelayanan", click: "resetChecked()" },
                        ]
                    },
                    { type: "hr" },
                    {
                        type: "controls",
                        cls: "span4 full",
                        text: "Include PDI",
                        items: [
                            { name: "isCheck", model: "data.isCheck", cls: "span1", type: "ng-check" },
                        ]
                    },
                    { type: "hr" },
                    {
                        type: "controls",
                        cls: "span8 full",
                        text: "Tgl. Service",
                        items: [
                            { name: "isCheck1", model: "data.isCheck1", cls: "span1", type: "ng-check" },
                            { name: "ServiceDateFrom", text: "", cls: "span2", type: "ng-datepicker" },
                            { type: "label", text: "s/d", cls: "span1 mylabel" },
                            { name: "ServiceDateTo", text: "", cls: "span2", type: "ng-datepicker" },
                        ]
                    },
                    {
                        type: "controls",
                        cls: "span8 full",
                        text: "Tgl. Reminder",
                        items: [
                            { name: "isCheck2", model: "data.isCheck2", cls: "span1", type: "ng-check" },
                            { name: "ReminderDateFrom", text: "", cls: "span2", type: "ng-datepicker" },
                            { type: "label", text: "s/d", cls: "span1 mylabel" },
                            { name: "ReminderDateTo", text: "", cls: "span2", type: "ng-datepicker" },
                        ]
                    },
                    {
                        type: "controls",
                        cls: "span8 full",
                        text: "Tgl. Follow Up",
                        items: [
                            { name: "isCheck3", model: "data.isCheck3", cls: "span1", type: "ng-check" },
                            { name: "FollowUpDateFrom", text: "", cls: "span2", type: "ng-datepicker" },
                            { type: "label", text: "s/d", cls: "span1 mylabel" },
                            { name: "FollowUpDateTo", text: "", cls: "span2", type: "ng-datepicker" },
                        ]
                    },
                    { type: "hr" },
                    {
                        type: "controls",
                        cls: "span4 full",
                        text: "Priode",
                        items: [
                            { name: "Priode", text: "", cls: "span6", type: "ng-datepicker", format: "MM yyyy" },
                        ]
                    },
                    { type: "hr" },
                    {
                        type: "buttons", cls: "span2", items: [
                             { name: "Print", text: "Print", icon: "icon-print", click: "Print()", cls: "button small btn btn-success" },
                        ]
                    },
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